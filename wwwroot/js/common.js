/* ==========================================================================
   Proyecto C.A.S.A. — utilidades compartidas por todas las vistas
   Versión mejorada con navegación por roles
   ========================================================================== */
(function () {
  'use strict';

  const CASA = {};
  window.CASA = CASA;

  /* ---------- 1. Configuración ------------------------------------------ */
  CASA.API_BASE = 'http://localhost:5000';

  CASA.endpoints = {
    mascotas:             '/api/Mascotas',
    reportes:             '/api/Reportes',
    tiposReporte:         '/api/TipoReporte',
    denuncias:            '/api/DenunciasAbandonos',
    solicitudes:          '/api/SolicitudesAdopcion',
    adopciones:           '/api/Adopciones',
    establecimientos:     '/api/Establecimientos',
    tiposEstablecimiento: '/api/TipoEstableciminetos',
    distritos:            '/api/Distritos',
    rescatistas:          '/api/Rescatistas'
  };

  CASA.SUCRE = { lat: -19.0333, lng: -65.2627, zoom: 13 };

  /* ---------- 2. Utilidades básicas ------------------------------------- */
  const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
  CASA.esc = (v) => String(v ?? '').replace(/[&<>"']/g, (c) => ESC[c]);

  CASA.g = function (obj, ...names) {
    if (obj == null || typeof obj !== 'object') return undefined;
    const keys = Object.keys(obj);
    for (const name of names) {
      const hit = keys.find((k) => k.toLowerCase() === name.toLowerCase());
      if (hit !== undefined && obj[hit] != null) return obj[hit];
    }
    return undefined;
  };

  const strip = (s) => String(s ?? '').normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase().trim();
  CASA.strip = strip;

  CASA.fmtDate = function (v, withTime) {
    if (!v) return '—';
    const d = new Date(v);
    if (isNaN(d)) return CASA.esc(v);
    const opts = withTime
      ? { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }
      : { day: '2-digit', month: 'short', year: 'numeric' };
    return d.toLocaleDateString('es-BO', opts);
  };

  CASA.imgUrl = function (v) {
    const s = String(v ?? '').trim();
    if (!s) return '';
    if (/^data:image\/(png|jpe?g|gif|webp|avif);base64,[a-z0-9+/=]+$/i.test(s)) return s;
    if (/^https?:\/\//i.test(s)) return s;
    return CASA.API_BASE + (s.startsWith('/') ? s : '/' + s);
  };

  /* ---------- 3. Estados y Badges -------------------------------------- */
  CASA.estadoClass = function (estado) {
    const s = strip(estado);
    if (!s) return 'is-neutral';
    if (/(no disponible|rechaz|cancel|inactiv|cerrad)/.test(s)) return 'is-bad';
    if (/(adoptad|resuelt|finaliz|complet)/.test(s)) return 'is-done';
    if (/(disponible|aprob|activ|abiert|en adopcion)/.test(s)) return 'is-ok';
    if (/(pendiente|proceso|rescate|espera|perdid|abandon)/.test(s)) return 'is-wait';
    return 'is-neutral';
  };
  
  CASA.badge = (estado) =>
    `<span class="casa-badge ${CASA.estadoClass(estado)}">${CASA.esc(estado || 'Sin estado')}</span>`;

  CASA.especieEmoji = function (especie) {
    const s = strip(especie);
    if (/(perr|can|dog)/.test(s)) return '🐶';
    if (/(gat|fel|cat)/.test(s)) return '🐱';
    if (/(conej)/.test(s)) return '🐰';
    if (/(ave|pajar)/.test(s)) return '🐦';
    return '🐾';
  };

  const COLORES = {
    rojo: '#E5384F', naranja: '#FF7A00', amarillo: '#F6C21A', verde: '#2E9E5B',
    azul: '#0B47C9', celeste: '#3DA5F4', morado: '#7A4FD6', rosa: '#F26FA8'
  };
  
  CASA.cintaColor = function (v) {
    const raw = String(v ?? '').trim();
    if (!raw) return '#FF7A00';
    const k = strip(raw).replace(/\s+/g, '_');
    return COLORES[k] || raw || '#FF7A00';
  };

  /* ---------- 4. Cliente API ------------------------------------------- */
  class ApiError extends Error {
    constructor(message, status, data) { 
      super(message); 
      this.name = 'ApiError'; 
      this.status = status; 
      this.data = data; 
    }
  }
  CASA.ApiError = ApiError;

  async function request(path, options = {}) {
    const url = /^https?:\/\//i.test(path) ? path : CASA.API_BASE + path;
    const headers = { Accept: 'application/json', ...(options.headers || {}) };
    const token = CASA.auth.getToken();
    if (token) headers['Authorization'] = `Bearer ${token}`;
    if (options.body !== undefined) headers['Content-Type'] = 'application/json';
    
    try {
      const res = await fetch(url, {
        method: options.method || 'GET',
        headers,
        body: options.body !== undefined ? JSON.stringify(options.body) : undefined
      });
      
      const text = await res.text();
      let data = null;
      if (text) { try { data = JSON.parse(text); } catch (_) { data = text; } }
      
      if (!res.ok) throw new ApiError(data?.message || `Error ${res.status}`, res.status, data);
      return data;
    } catch (e) {
      throw new ApiError(e.message || 'Error de conexión', 0, null);
    }
  }

  CASA.list = function (data) {
    if (Array.isArray(data)) return data;
    if (data && typeof data === 'object') {
      for (const k of ['$values', 'items', 'data', 'value']) 
        if (Array.isArray(data[k])) return data[k];
    }
    return [];
  };

  CASA.api = {
    get:  (path) => request(path),
    list: async (path) => CASA.list(await request(path)),
    listSoft: async (path) => { try { return CASA.list(await request(path)); } catch (_) { return []; } },
    post: (path, body) => request(path, { method: 'POST', body }),
    put:  (path, body) => request(path, { method: 'PUT', body }),
    del:  (path) => request(path, { method: 'DELETE' })
  };

  CASA.byId = (items) => Object.fromEntries(CASA.list(items).map((x) => [CASA.g(x, 'id'), x]));

  /* ---------- 5. Autenticación ----------------------------------------- */
  CASA.auth = {
    _tokenKey: 'casa.token',
    _userKey: 'casa.user',
    
    getToken() {
      try { return localStorage.getItem(this._tokenKey) || ''; } catch (_) { return ''; }
    },
    
    getUser() {
      try { 
        const u = localStorage.getItem(this._userKey);
        return u ? JSON.parse(u) : null;
      } catch (_) { return null; }
    },

    isLoggedIn() {
      return !!this.getToken();
    },

    hasRole(...roles) {
      const user = this.getUser();
      if (!user || !user.Rol) return false;
      return roles.some(r => user.Rol.toLowerCase() === r.toLowerCase());
    },

    async login(identifier, password) {
      const res = await CASA.api.post('/api/Auth/login', { 
        ci: identifier, 
        email: identifier, 
        password 
      });
      
      localStorage.setItem(this._tokenKey, res.token);
      localStorage.setItem(this._userKey, JSON.stringify(res.usuario));
      window.location.reload();
      return res;
    },

    async register(data) {
      await CASA.api.post('/api/Auth/registro', {
        nombre: data.nombre,
        email: data.email,
        ci: data.ci,
        celular: data.celular,
        password: data.password
      });
      return this.login(data.email, data.password);
    },

    logout() {
      localStorage.removeItem(this._tokenKey);
      localStorage.removeItem(this._userKey);
      window.location.reload();
    }
  };

  CASA.session = {
    getCiudadanoId() { try { return localStorage.getItem('casa.ciudadanoId') || ''; } catch (_) { return ''; } },
    setCiudadanoId(v) { try { localStorage.setItem('casa.ciudadanoId', String(v)); } catch (_) { } }
  };

  /* ---------- 6. UI Components ----------------------------------------- */
  CASA.ui = {
    skeletons(el, n = 6, colClass = 'col-12 col-md-6 col-xl-4') {
      el.innerHTML = Array.from({ length: n }, () => 
        `<div class="${colClass}"><div class="casa-skeleton" aria-hidden="true"></div></div>`).join('');
    },
    done(el) { el.removeAttribute('aria-busy'); },
    empty(el, emoji, title, msg) {
      el.innerHTML = `<div class="col-12"><div class="casa-state"><span class="casa-state__emoji">${emoji}</span><h3>${CASA.esc(title)}</h3><p>${CASA.esc(msg)}</p></div></div>`;
    },
    error(el, err, endpoint) {
      const offline = err instanceof ApiError && err.status === 0;
      el.innerHTML = `<div class="col-12"><div class="casa-state casa-state--error"><span class="casa-state__emoji">🐕‍</span><h3>${offline ? 'Sin conexión' : 'Error'}</h3><p>${CASA.esc(err.message)}</p>${endpoint ? `<small>Ruta: ${CASA.esc(endpoint)}</small>` : ''}</div></div>`;
    },
    toast(msg, kind = '') {
      let zone = document.querySelector('.casa-toast-zone');
      if (!zone) {
        zone = document.createElement('div');
        zone.className = 'casa-toast-zone';
        document.body.appendChild(zone);
      }
      const t = document.createElement('div');
      t.className = 'casa-toast' + (kind ? ' is-' + kind : '');
      t.textContent = msg;
      zone.appendChild(t);
      setTimeout(() => t.remove(), 4000);
    }
  };

  CASA.districtHtml = function (distrito) {
    if (!distrito) return '';
    const color = CASA.cintaColor(CASA.g(distrito, 'colorCinta'));
    return `<span class="pet-card__district"><span class="ribbon-dot" style="--ribbon:${CASA.esc(color)}"></span>${CASA.esc(CASA.g(distrito, 'nombre'))}</span>`;
  };

  CASA.petCard = function (m, distritosById, opts = {}) {
    const g = CASA.g, esc = CASA.esc;
    const id = g(m, 'id');
    const nombre = g(m, 'nombre') || 'Sin nombre';
    const especie = g(m, 'especie');
    const foto = CASA.imgUrl(g(m, 'foto'));
    const estado = g(m, 'estado');
    const distrito = (distritosById || {})[g(m, 'distritoId')];
    const disponible = CASA.estadoClass(estado) === 'is-ok';
    
    const traits = [
      [CASA.especieEmoji(especie), especie],
      ['', g(m, 'edad')],
      ['📏', g(m, 'tamaño', 'tamano')],
      ['🎨', g(m, 'color')]
    ].filter(([, t]) => t).map(([e, t]) => 
      `<span class="pet-trait">${e} ${esc(t)}</span>`).join('');

    return `
      <article class="pet-card">
        <div class="pet-card__photo">
          <div class="pet-card__fallback">${CASA.especieEmoji(especie)}</div>
          ${foto ? `<img src="${esc(foto)}" loading="lazy" data-fallback>` : ''}
          <span class="pet-card__status">${CASA.badge(estado)}</span>
          <span class="pet-card__ribbon"></span>
        </div>
        <div class="pet-card__body">
          <h3 class="pet-tag">${esc(nombre)}</h3>
          <div class="pet-card__meta">${traits}</div>
          <div class="pet-card__foot">
            ${distrito ? CASA.districtHtml(distrito) : ''}
            ${opts.adopt ? `<button class="btn-casa btn-casa-sm" data-adopt="${id}" ${disponible ? '' : 'disabled'}>Adoptar</button>` : ''}
          </div>
        </div>
      </article>`;
  };

  /* ---------- 7. Navegación por ROLES (CORREGIDO) --------------------- */
  const NAV_ITEMS = [
    { key: 'index', href: '/', label: 'Inicio', public: true },
    { key: 'mascotas', href: '/Mascotas', label: 'Mascotas', public: true },
    { key: 'mapa', href: '/Mapa', label: 'Mapa', public: true },
    { key: 'reportes', href: '/Reportes', label: 'Reportes', public: false },
    { key: 'adopciones', href: '/Adopciones', label: 'Adopciones', public: false },
    { key: 'establecimientos', href: '/Establecimientos', label: 'Veterinarias', public: false }
  ];

  function renderNav() {
    const navHost = document.getElementById('casa-nav');
    if (!navHost) return;

    const isLoggedIn = CASA.auth.isLoggedIn();
    const user = CASA.auth.getUser();
    const activePage = document.body.dataset.page || '';

    // Filtrar items según autenticación
    const visibleItems = NAV_ITEMS.filter(item => {
      if (item.public) return true;
      return isLoggedIn;
    });

    // Generar HTML de items de menú
    const menuHTML = visibleItems.map(item => `
      <li class="nav-item">
        <a class="nav-link ${item.key === activePage ? 'active' : ''}" 
           href="${item.href}" 
           ${item.key === activePage ? 'aria-current="page"' : ''}>
          ${item.label}
        </a>
      </li>
    `).join('');

    // Generar botones de auth
    let authHTML = '';
    if (isLoggedIn && user) {
      authHTML = `
        <li class="nav-item dropdown ms-3">
          <a class="nav-link dropdown-toggle btn btn-casa btn-casa-sm text-white" 
             href="#" role="button" data-bs-toggle="dropdown">
            ${CASA.esc(user.nombre || 'Mi Cuenta')}
          </a>
          <ul class="dropdown-menu dropdown-menu-end">
            <li><a class="dropdown-item" href="/Perfil">Mi Perfil</a></li>
            ${CASA.auth.hasRole('Administrador') ? '<li><a class="dropdown-item" href="/Admin/Usuarios">Administración</a></li>' : ''}
            <li><hr class="dropdown-divider"></li>
            <li><a class="dropdown-item text-danger" href="#" id="btn-logout">Cerrar Sesión</a></li>
          </ul>
        </li>
      `;
    } else {
      authHTML = `
        <li class="nav-item ms-3">
          <a class="btn btn-casa-ghost btn-casa-sm me-2" href="/Auth/Login">Iniciar Sesión</a>
          <a class="btn btn-casa btn-casa-sm" href="/Auth/Login#crear">Crear Cuenta</a>
        </li>
      `;
    }

    // Renderizar nav completo
    navHost.innerHTML = `
      <nav class="navbar navbar-expand-lg casa-nav sticky-top">
        <div class="container">
          <a class="casa-brand" href="/">
            <span class="casa-brand__logo"></span>
            <span>C.A.S.A. <small>Sucre, Bolivia</small></span>
          </a>
          <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#casaMenu">
            <span class="navbar-toggler-icon"></span>
          </button>
          <div class="collapse navbar-collapse justify-content-end" id="casaMenu">
            <ul class="navbar-nav align-items-lg-center gap-lg-1 pt-3 pt-lg-0">
              ${menuHTML}
              ${authHTML}
            </ul>
          </div>
        </div>
      </nav>
    `;

    // Agregar evento de logout
    setTimeout(() => {
      const logoutBtn = document.getElementById('btn-logout');
      if (logoutBtn) {
        logoutBtn.addEventListener('click', (e) => {
          e.preventDefault();
          CASA.auth.logout();
        });
      }
    }, 100);
  }

  function renderFooter() {
    const footHost = document.getElementById('casa-footer');
    if (!footHost) return;

    footHost.innerHTML = `
      <footer class="casa-footer mt-5">
        <div class="container d-flex flex-column flex-md-row justify-content-between gap-3">
          <div>
            <a class="casa-brand" href="/">
              <span class="casa-brand__logo">🐾</span>
              <span>C.A.S.A.</span>
            </a>
            <p class="mt-2 mb-0 small">Rescate, adopción y gestión animal.<br>Sucre, Bolivia</p>
          </div>
          <ul class="list-unstyled small mb-0">
            ${NAV_ITEMS.filter(i => !i.key === 'index').map(i => 
              `<li><a href="${i.href}">${i.label}</a></li>`).join('')}
          </ul>
        </div>
      </footer>
    `;
  }

  // Inicializar cuando el DOM esté listo
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
      renderNav();
      renderFooter();
    });
  } else {
    renderNav();
    renderFooter();
  }

})();

/* ==========================================================================
   EFECTOS VISUALES
   ========================================================================== */
(function () {
  'use strict';
  
  const LOGO = '/img/logo.png';

  // Cargar logo
  document.querySelectorAll('.casa-brand__logo').forEach((box) => {
    const img = new Image();
    img.onload = () => { 
      box.textContent = ''; 
      box.classList.add('has-img'); 
      box.appendChild(img); 
    };
    img.src = LOGO;
  });

  // Animaciones de elementos flotantes en hero
  const hero = document.querySelector('.casa-hero:not(.casa-hero--page)');
  if (hero) {
    [['🐾', '6%', '18%'], ['', '38%', '10%'], ['🐾', '24%', '62%']]
      .forEach(([emoji, x, y]) => {
        const el = document.createElement('span');
        el.className = 'casa-float';
        el.textContent = emoji;
        el.style.cssText = `--x:${x};--y:${y}`;
        hero.appendChild(el);
      });
  }

  // Efecto scroll en navbar
  window.addEventListener('scroll', () => {
    const nav = document.querySelector('.casa-nav');
    if (nav) nav.classList.toggle('is-scrolled', window.scrollY > 8);
  }, { passive: true });

})();