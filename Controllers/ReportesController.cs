using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Proyecto.Data;
using Proyecto.Models;
using Proyecto.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto.Controllers;

public class ReportesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ReportesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var reportes = await _context.Reportes
                .Include(r => r.TipoReporte)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            ViewBag.Reportes = reportes;

            var userCi = User.FindFirst("ci")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            ViewBag.EsAdmin = userRole?.ToLower() == "administrador";
            ViewBag.EsRescatista = userRole?.ToLower() == "rescatista";

            if (!ViewBag.EsAdmin && !ViewBag.EsRescatista && !string.IsNullOrEmpty(userCi))
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
                if (usuario != null)
                {
                    var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                    ViewBag.CiudadanoId = ciudadano?.Id; 
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR INDEX REPORTES: {ex.Message}");
            ViewBag.Reportes = new List<Reportes>();
        }
        return View();
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Tipos = await _context.TipoReportes.ToListAsync() ?? new List<TipoReporte>();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reportes model, IFormFile imagenFile)
    {
        try
        {
            if (ModelState.IsValid)
            {
                model.Fecha = DateTime.Now;
                model.estado = "Pendiente";
                
                var userCi = User.FindFirst("ci")?.Value;
                if (!string.IsNullOrEmpty(userCi))
                {
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
                    if (usuario != null)
                    {
                        var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                        if (ciudadano != null) model.CiudadanoId = ciudadano.Id;
                    }
                }

                if (imagenFile != null && imagenFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "reportes");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = $"reporte_{Guid.NewGuid()}{Path.GetExtension(imagenFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagenFile.CopyToAsync(stream);
                    }
                    model.ImagenUrl = $"/uploads/reportes/{fileName}";
                }

                _context.Reportes.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reporte enviado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR CREATE POST: {ex.Message}");
            ModelState.AddModelError("", $"Error: {ex.Message}");
        }
        
        ViewBag.Tipos = await _context.TipoReportes.ToListAsync() ?? new List<TipoReporte>();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        
        var reporte = await _context.Reportes.Include(r => r.TipoReporte).FirstOrDefaultAsync(r => r.Id == id);
        if (reporte == null) return NotFound();

        var userCi = User.FindFirst("ci")?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        int? ciudadanoIdActual = null;

        if (!string.IsNullOrEmpty(userCi))
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
            if (usuario != null)
            {
                var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                ciudadanoIdActual = ciudadano?.Id;
            }
        }

        if (!PermissionHelper.CanManageReport(userRole, ciudadanoIdActual, reporte.CiudadanoId))
        {
            TempData["Error"] = "No tienes permisos para editar este reporte.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Tipos = await _context.TipoReportes.ToListAsync();
        return View(reporte);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Reportes model)
    {
        if (id != model.Id) return NotFound();

        var reporteOriginal = await _context.Reportes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        var userCi = User.FindFirst("ci")?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        int? ciudadanoIdActual = null;

        if (!string.IsNullOrEmpty(userCi))
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
            if (usuario != null)
            {
                var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                ciudadanoIdActual = ciudadano?.Id;
            }
        }

        if (!PermissionHelper.CanManageReport(userRole, ciudadanoIdActual, reporteOriginal.CiudadanoId))
        {
            TempData["Error"] = "No tienes permisos para editar este reporte.";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var reporte = await _context.Reportes.FindAsync(id);
                if (reporte == null) return NotFound();

                // ✅ ACTUALIZAR ESTADO Y EL NUEVO COMENTARIO
                reporte.estado = model.estado;
                reporte.ComentarioResolucion = model.ComentarioResolucion; 
                
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reporte actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportesExists(id)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR AL ACTUALIZAR: {ex.Message}");
                TempData["Error"] = $"Error al actualizar: {ex.Message}";
                ViewBag.Tipos = await _context.TipoReportes.ToListAsync();
                return View(model);
            }
        }
        
        ViewBag.Tipos = await _context.TipoReportes.ToListAsync();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var model = await _context.Reportes.FindAsync(id);
        if (model != null)
        {
            var userCi = User.FindFirst("ci")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            int? ciudadanoIdActual = null;

            if (!string.IsNullOrEmpty(userCi))
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
                if (usuario != null)
                {
                    var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                    ciudadanoIdActual = ciudadano?.Id;
                }
            }

            if (PermissionHelper.CanManageReport(userRole, ciudadanoIdActual, model.CiudadanoId))
            {
                if (!string.IsNullOrEmpty(model.ImagenUrl))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, model.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                _context.Reportes.Remove(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reporte eliminado.";
            }
            else
            {
                TempData["Error"] = "No tienes permisos para eliminar este reporte.";
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ReportesExists(int id)
    {
        return _context.Reportes.Any(e => e.Id == id);
    }
}