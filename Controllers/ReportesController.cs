using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Proyecto.Data;
using Proyecto.Models;
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
            ViewBag.Reportes = await _context.Reportes
                .Include(r => r.TipoReporte)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
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
                
                // Asignar ciudadano actual si está logueado, o dejar null si es anónimo
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
                    try
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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error al subir imagen: {ex.Message}");
                    }
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

    // GET: Editar Reporte (Con validación de permisos)
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        
        var reporte = await _context.Reportes.Include(r => r.TipoReporte).FirstOrDefaultAsync(r => r.Id == id);
        if (reporte == null) return NotFound();

        // 1. Obtener datos del usuario logueado desde el JWT
        var userCi = User.FindFirst("ci")?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        bool esAdmin = userRole == "Administrador";
        bool esRescatista = userRole == "Rescatista";
        bool esDueno = false;

        // 2. Si no es admin ni rescatista, verificar si es el ciudadano que lo creó
        if (!esAdmin && !esRescatista && !string.IsNullOrEmpty(userCi))
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
            if (usuario != null)
            {
                var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                if (ciudadano != null && ciudadano.Id == reporte.CiudadanoId)
                {
                    esDueno = true;
                }
            }
        }

        // 3. Si no cumple ninguna condición, denegar acceso
        if (!esAdmin && !esRescatista && !esDueno)
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        ViewBag.Tipos = await _context.TipoReportes.ToListAsync();
        return View(reporte);
    }

    // POST: Editar Reporte (Re-validación de seguridad)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Reportes model)
    {
        if (id != model.Id) return NotFound();

        var reporteOriginal = await _context.Reportes.FindAsync(id);
        var userCi = User.FindFirst("ci")?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        
        bool esAdmin = userRole == "Administrador";
        bool esRescatista = userRole == "Rescatista";
        bool esDueno = false;

        if (!esAdmin && !esRescatista && !string.IsNullOrEmpty(userCi))
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ci == userCi);
            if (usuario != null)
            {
                var ciudadano = await _context.Ciudadanos.FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
                if (ciudadano != null && ciudadano.Id == reporteOriginal.CiudadanoId)
                {
                    esDueno = true;
                }
            }
        }

        if (!esAdmin && !esRescatista && !esDueno)
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        if (ModelState.IsValid)
        {
            // Mantener el CiudadanoId original por seguridad
            model.CiudadanoId = reporteOriginal.CiudadanoId;
            model.RescatistasId = reporteOriginal.RescatistasId;
            
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Estado del reporte actualizado correctamente.";
            return RedirectToAction(nameof(Index));
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
            if (!string.IsNullOrEmpty(model.ImagenUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, model.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }
            _context.Reportes.Remove(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Reporte eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}