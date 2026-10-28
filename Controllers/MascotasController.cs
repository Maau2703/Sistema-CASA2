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

public class MascotasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MascotasController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: Mascotas
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Mascotas en adopción";
        ViewData["Page"] = "mascotas";
        
        try
        {
            var mascotas = await _context.Mascotas
                .AsNoTracking()
                .Include(m => m.Distritos)
                .ToListAsync();
            
            ViewBag.Mascotas = mascotas ?? new List<Mascotas>();
            ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync() ?? new List<Distritos>();
            
            // ✅ Obtener IDs de mascotas con solicitudes pendientes o aprobadas
            var mascotasConSolicitud = await _context.SolicitudesAdopcion
                .Where(s => s.Estado == "Pendiente" || s.Estado == "Aprobada")
                .Select(s => s.mascotaId)
                .ToListAsync();
            
            ViewBag.SolicitudesPendientes = mascotasConSolicitud;
            
            Console.WriteLine($"🔍 DEBUG - Total mascotas: {mascotas.Count}");
            Console.WriteLine($" DEBUG - Mascotas con solicitud: {mascotasConSolicitud.Count}");
            
            foreach(var m in mascotas)
            {
                bool tieneSolicitud = mascotasConSolicitud.Contains(m.Id);
                Console.WriteLine($"   🐾 {m.Nombre}: Estado='{m.Estado}', TieneSolicitud={tieneSolicitud}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR INDEX MASCOTAS: {ex.Message}");
            ViewBag.Mascotas = new List<Mascotas>();
            ViewBag.Distritos = new List<Distritos>();
            ViewBag.SolicitudesPendientes = new List<int>();
        }
        
        return View();
    }

    // GET: Mascotas/Create
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Registrar Mascota";
        ViewData["Page"] = "mascotas";
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync() ?? new List<Distritos>();
        return View();
    }

    // POST: Mascotas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mascotas mascota, IFormFile fotoFile)
    {
        Console.WriteLine("=== CREANDO MASCOTA ===");
        Console.WriteLine($"Nombre: {mascota.Nombre}, Estado: {mascota.Estado}");
        
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($" Error: {error.ErrorMessage}");
            }
        }

        if (ModelState.IsValid)
        {
            // ✅ FORZAR estado a "En adopción" (case-sensitive exacto)
            mascota.Estado = "En adopción";
            
            if (fotoFile != null && fotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "mascotas");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = $"mascota_{Guid.NewGuid()}{Path.GetExtension(fotoFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fotoFile.CopyToAsync(stream);
                }
                
                mascota.Foto = $"/uploads/mascotas/{uniqueFileName}";
            }
            
            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"✅ MASCOTA CREADA: {mascota.Nombre}, ID: {mascota.Id}, Estado: {mascota.Estado}");
            TempData["Success"] = "Mascota registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync() ?? new List<Distritos>();
        return View(mascota);
    }

    // GET: Mascotas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        
        var mascota = await _context.Mascotas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (mascota == null) return NotFound();
        
        ViewData["Title"] = "Editar Mascota";
        ViewData["Page"] = "mascotas";
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync() ?? new List<Distritos>();
        
        return View(mascota);
    }

    // POST: Mascotas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Mascotas mascota, IFormFile fotoFile)
    {
        if (id != mascota.Id) return NotFound();
        
        if (ModelState.IsValid)
        {
            try
            {
                if (fotoFile != null && fotoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "mascotas");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = $"mascota_{Guid.NewGuid()}{Path.GetExtension(fotoFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fotoFile.CopyToAsync(stream);
                    }
                    
                    mascota.Foto = $"/uploads/mascotas/{uniqueFileName}";
                }

                _context.Entry(mascota).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
                
                TempData["Success"] = "Mascota actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MascotaExists(mascota.Id)) return NotFound();
                throw;
            }
        }
        
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync() ?? new List<Distritos>();
        return View(mascota);
    }

    // GET: Mascotas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        
        var mascota = await _context.Mascotas
            .AsNoTracking()
            .Include(m => m.Distritos)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (mascota == null) return NotFound();
        
        ViewData["Title"] = "Eliminar Mascota";
        return View(mascota);
    }

    // POST: Mascotas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota != null)
        {
            if (!string.IsNullOrEmpty(mascota.Foto))
            {
                var filePath = Path.Combine(_environment.WebRootPath, mascota.Foto.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Mascota eliminada correctamente.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool MascotaExists(int id)
    {
        return _context.Mascotas.Any(e => e.Id == id);
    }
}