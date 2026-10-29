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

    public async Task<IActionResult> Index()
    {
        try
        {
            var userCi = User.FindFirst("ci")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            bool esAdmin = userRole?.ToLower() == "administrador";
            bool esRescatista = userRole?.ToLower() == "rescatista";

            var solicitudes = _context.SolicitudesAdopcion
                .Include(s => s.Mascotas)
                .Include(s => s.Ciudadanos).ThenInclude(c => c.usuarios)
                .AsQueryable();

            // ✅ FILTRAR: Si NO es admin ni rescatista, solo ve SUS propias solicitudes
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
            // Si es admin o rescatista, ve TODAS las solicitudes (ya está configurado así)

            ViewBag.Solicitudes = await solicitudes.OrderByDescending(s => s.fecha).ToListAsync();
            ViewBag.EsAdmin = esAdmin;
            ViewBag.EsRescatista = esRescatista;
        }
        catch (Exception ex) { Console.WriteLine($"❌ ERROR INDEX: {ex.Message}"); }
        return View();
    }

    public async Task<IActionResult> Solicitudes()
    {
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        bool esAdmin = userRole?.ToLower() == "administrador";
        bool esRescatista = userRole?.ToLower() == "rescatista";

        if (!esAdmin && !esRescatista) 
        {
            TempData["Error"] = "No tienes permisos para acceder a este panel.";
            return RedirectToAction(nameof(Index));
        }

        var todas = await _context.SolicitudesAdopcion
            .Include(s => s.Mascotas)
            .Include(s => s.Ciudadanos).ThenInclude(c => c.usuarios)
            .OrderByDescending(s => s.fecha)
            .ToListAsync();
        
        var agrupadas = todas
            .GroupBy(s => s.mascotaId)
            .Select(g => new { 
                Mascota = g.First().Mascotas, 
                Solicitudes = g.ToList() 
            })
            .ToList();

        ViewBag.SolicitudesPorMascota = agrupadas;
        ViewBag.TotalSolicitudes = todas.Count;
        ViewBag.Pendientes = todas.Count(s => s.Estado == "Pendiente");
        ViewBag.Aprobadas = todas.Count(s => s.Estado == "Aprobada");
        ViewBag.Rechazadas = todas.Count(s => s.Estado == "Rechazada");

        return View();
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
            var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
            
            if (ciudadano == null)
            {
                ciudadano = new Ciudadanos { UsuarioId = usuario.Id, barrio = "Por definir" };
                _context.Ciudadanos.Add(ciudadano);
                await _context.SaveChangesAsync();
            }

            var solicitudPropia = await _context.SolicitudesAdopcion.FirstOrDefaultAsync(s => s.mascotaId == mascotaId && s.CiudadanoId == ciudadano.Id && s.Estado == "Pendiente");

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
            Console.WriteLine($"💥 ERROR CRÍTICO EN ADOPTAR: {ex.Message}");
            TempData["Error"] = $"Error: {ex.Message}";
            return RedirectToAction("Index", "Mascotas");
        }
    }

    // ✅ CORREGIDO: Con logs y redirección correcta
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
    {
        Console.WriteLine($"🔄 DEBUG: Intentando cambiar estado. Id={id}, NuevoEstado={nuevoEstado}");
        
        var solicitud = await _context.SolicitudesAdopcion.Include(s => s.Mascotas).FirstOrDefaultAsync(s => s.Id == id);
        if (solicitud == null) 
        {
            Console.WriteLine($"❌ DEBUG: Solicitud con Id {id} no encontrada.");
            return NotFound();
        }

        solicitud.Estado = nuevoEstado;
        if (nuevoEstado.ToLower() == "aprobada" && solicitud.Mascotas != null) 
        {
            solicitud.Mascotas.Estado = "Adoptado";
            Console.WriteLine($"✅ DEBUG: Mascota {solicitud.Mascotas.Nombre} marcada como Adoptado.");
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Estado actualizado a: {nuevoEstado}";
        
        Console.WriteLine($"✅ DEBUG: Cambio de estado exitoso. Redirigiendo a Solicitudes.");
        return RedirectToAction("Solicitudes"); // ✅ Se queda en el panel
    }

    // ✅ CORREGIDO: Con logs y redirección correcta
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        Console.WriteLine($"🗑️ DEBUG: Intentando eliminar solicitud. Id={id}");
        
        var model = await _context.SolicitudesAdopcion.FindAsync(id);
        if (model != null)
        {
            _context.SolicitudesAdopcion.Remove(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Solicitud eliminada correctamente.";
            Console.WriteLine($"✅ DEBUG: Solicitud {id} eliminada exitosamente.");
        }
        else
        {
            Console.WriteLine($"❌ DEBUG: No se encontró la solicitud {id} para eliminar.");
        }
        
        // ✅ CORRECCIÓN CLAVE: Redirigir de vuelta al panel de Solicitudes, no al Index
        return RedirectToAction("Solicitudes"); 
    }
}