using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto.Controllers;

public class AdopcionesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdopcionesController(ApplicationDbContext context) => _context = context;

    // INDEX: Dashboard personal
    public async Task<IActionResult> Index()
    {
        try
        {
            var userCi = User.FindFirst("ci")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            bool esAdmin = userRole?.ToLower() == "administrador";
            bool esRescatista = userRole?.ToLower() == "rescatista";

            Console.WriteLine($" DEBUG - Role: {userRole}, EsAdmin: {esAdmin}");

            var solicitudes = _context.SolicitudesAdopcion
                .Include(s => s.Mascotas)
                .Include(s => s.Ciudadanos)
                    .ThenInclude(c => c.usuarios)
                .AsQueryable();

            // Si NO es admin ni rescatista, filtrar solo sus solicitudes
            if (!esAdmin && !esRescatista && !string.IsNullOrEmpty(userCi))
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
                if (usuario != null)
                {
                    var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                    if (ciudadano != null)
                    {
                        solicitudes = solicitudes.Where(s => s.CiudadanoId == ciudadano.Id);
                    }
                }
            }

            ViewBag.Solicitudes = await solicitudes.OrderByDescending(s => s.fecha).ToListAsync();
            ViewBag.EsAdmin = esAdmin;
            ViewBag.EsRescatista = esRescatista;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR INDEX ADOPCIONES: {ex.Message}");
            ViewBag.Solicitudes = new List<SolicitudesAdopcion>();
            ViewBag.EsAdmin = false;
            ViewBag.EsRescatista = false;
        }
        return View();
    }

    // ✅ Panel exclusivo para Admin/Rescatista
    public async Task<IActionResult> Solicitudes()
    {
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        bool esAdmin = userRole?.ToLower() == "administrador";
        bool esRescatista = userRole?.ToLower() == "rescatista";

        Console.WriteLine($"🔍 DEBUG Solicitudes - Role: {userRole}, EsAdmin: {esAdmin}");

        if (!esAdmin && !esRescatista)
        {
            TempData["Error"] = "No tienes permisos para acceder a este panel.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var todasSolicitudes = await _context.SolicitudesAdopcion
                .Include(s => s.Mascotas)
                .Include(s => s.Ciudadanos)
                    .ThenInclude(c => c.usuarios)
                .OrderByDescending(s => s.fecha)
                .ToListAsync();

            var solicitudesPorMascota = todasSolicitudes
                .GroupBy(s => s.mascotaId)
                .Select(g => new
                {
                    Mascota = g.First().Mascotas,
                    Solicitudes = g.ToList()
                })
                .ToList();

            ViewBag.SolicitudesPorMascota = solicitudesPorMascota;
            ViewBag.TotalSolicitudes = todasSolicitudes.Count;
            ViewBag.Pendientes = todasSolicitudes.Count(s => s.Estado == "Pendiente");
            ViewBag.Aprobadas = todasSolicitudes.Count(s => s.Estado == "Aprobada");
            ViewBag.Rechazadas = todasSolicitudes.Count(s => s.Estado == "Rechazada");

            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR PANEL SOLICITUDES: {ex.Message}");
            TempData["Error"] = $"Error: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adoptar(int mascotaId)
    {
        try
        {
            var userCi = User.FindFirst("ci")?.Value;
            if (string.IsNullOrEmpty(userCi))
            {
                TempData["Error"] = "Debes iniciar sesión para adoptar.";
                return RedirectToAction("Login", "Auth");
            }

            var mascota = await _context.Mascotas.FindAsync(mascotaId);
            if (mascota == null || mascota.Estado != "En adopción")
            {
                TempData["Error"] = "La mascota no está disponible.";
                return RedirectToAction("Index", "Mascotas");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index", "Mascotas");
            }

            var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
            if (ciudadano == null)
            {
                ciudadano = new Ciudadanos { UsuarioId = usuario.Id, barrio = "Por definir" };
                _context.Ciudadanos.Add(ciudadano);
                await _context.SaveChangesAsync();
            }

            var solicitudPropia = await _context.SolicitudesAdopcion
                .FirstOrDefaultAsync(s => s.mascotaId == mascotaId && 
                                         s.CiudadanoId == ciudadano.Id && 
                                         s.Estado == "Pendiente");

            if (solicitudPropia != null)
            {
                TempData["Warning"] = $"Ya tienes una solicitud pendiente para '{mascota.Nombre}'.";
                return RedirectToAction(nameof(Index));
            }

            var nuevaSolicitud = new SolicitudesAdopcion
            {
                mascotaId = mascotaId,
                CiudadanoId = ciudadano.Id,
                fecha = DateTime.Now,
                Estado = "Pendiente"
            };

            _context.SolicitudesAdopcion.Add(nuevaSolicitud);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"¡Solicitud creada para {mascota.Nombre}!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR ADOPTAR: {ex.Message}");
            TempData["Error"] = $"Error: {ex.Message}";
            return RedirectToAction("Index", "Mascotas");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
    {
        try
        {
            var solicitud = await _context.SolicitudesAdopcion
                .Include(s => s.Mascotas)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null) return NotFound();

            solicitud.Estado = nuevoEstado;

            if (nuevoEstado.ToLower() == "aprobada" && solicitud.Mascotas != null)
            {
                solicitud.Mascotas.Estado = "Adoptado";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Estado actualizado a: {nuevoEstado}";
            return RedirectToAction("Solicitudes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR CAMBIAR ESTADO: {ex.Message}");
            TempData["Error"] = $"Error: {ex.Message}";
            return RedirectToAction("Solicitudes");
        }
    }
        // GET: Mostrar vista de confirmación de eliminación (necesario para que el formulario funcione bien)
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        
        var model = await _context.SolicitudesAdopcion
            .Include(s => s.Mascotas)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (model == null) return NotFound();
        
        return View(model);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var model = await _context.SolicitudesAdopcion.FindAsync(id);
        if (model != null)
        {
            _context.SolicitudesAdopcion.Remove(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Solicitud eliminada.";
        }
        return RedirectToAction(nameof(Index));
    }
}