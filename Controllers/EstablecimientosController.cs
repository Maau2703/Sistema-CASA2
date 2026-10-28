using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto.Controllers;

public class EstablecimientosController : Controller
{
    private readonly ApplicationDbContext _context;

    public EstablecimientosController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Veterinarias y Albergues";
        ViewData["Page"] = "establecimientos";
        
        var data = await _context.Establecimientos
            .AsNoTracking()
            .Include(e => e.TipoEstablecimiento)
            .Include(e => e.Distritos)
            .ToListAsync();
            
        ViewBag.Establecimientos = data ?? new List<Establecimientos>();
        return View();
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            ViewData["Title"] = "Registrar Establecimiento";
            ViewBag.Tipos = await _context.TipoEstablecimientos.AsNoTracking().ToListAsync();
            ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync();
            Console.WriteLine($"✅ Tipos cargados: {ViewBag.Tipos.Count}, Distritos: {ViewBag.Distritos.Count}");
            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR EN CREATE GET: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw; // Lanza el error para verlo en la terminal
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Establecimientos model)
    {
        if (ModelState.IsValid)
        {
            model.estado = "Activo";
            _context.Establecimientos.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Establecimiento registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tipos = await _context.TipoEstablecimientos.AsNoTracking().ToListAsync();
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var model = await _context.Establecimientos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (model == null) return NotFound();

        ViewBag.Tipos = await _context.TipoEstablecimientos.AsNoTracking().ToListAsync();
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Establecimientos model)
    {
        if (id != model.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Establecimiento actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tipos = await _context.TipoEstablecimientos.AsNoTracking().ToListAsync();
        ViewBag.Distritos = await _context.Distritos.AsNoTracking().ToListAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        
        var model = await _context.Establecimientos
            .AsNoTracking()
            .Include(e => e.TipoEstablecimiento)
            .Include(e => e.Distritos)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (model == null) return NotFound();
        
        ViewData["Title"] = "Eliminar Establecimiento";
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var model = await _context.Establecimientos.FindAsync(id);
        if (model != null)
        {
            _context.Establecimientos.Remove(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Establecimiento eliminado correctamente.";
        }
        return RedirectToAction(nameof(Index));
    }
}