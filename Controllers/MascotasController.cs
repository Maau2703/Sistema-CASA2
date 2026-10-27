using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto.Controllers;

public class MascotasController : Controller
{
    private readonly ApplicationDbContext _context;

    public MascotasController(ApplicationDbContext context)
    {
        _context = context;
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            ViewBag.Mascotas = new List<Mascotas>();
            ViewBag.Distritos = new List<Distritos>();
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
    public async Task<IActionResult> Create(Mascotas mascota)
    {
        Console.WriteLine("=== INTENTO DE CREAR MASCOTA ===");
        Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
        Console.WriteLine($"Nombre recibido: '{mascota.Nombre}'");
        Console.WriteLine($"Especie recibida: '{mascota.Especie}'");
        Console.WriteLine($"DistritoId recibido: {mascota.DistritoId}");
        
        if (!ModelState.IsValid)
        {
            Console.WriteLine("❌ ERRORES DE VALIDACIÓN:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"   - {error.ErrorMessage}");
            }
        }

        if (ModelState.IsValid)
        {
            mascota.Estado = "En adopción";
            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ MASCOTA GUARDADA CON ÉXITO");
            TempData["Success"] = "Mascota registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        
        Console.WriteLine("❌ NO SE GUARDÓ, DEVOLVIENDO LA VISTA CON ERRORES");
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
    public async Task<IActionResult> Edit(int id, Mascotas mascota)
    {
        if (id != mascota.Id) return NotFound();
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Entry(mascota).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mascota actualizada correctamente.";
                _context.ChangeTracker.Clear();
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