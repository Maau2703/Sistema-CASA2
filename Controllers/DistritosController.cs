using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto.Controllers;

public class DistritosController : Controller
{
    private readonly ApplicationDbContext _context;
    public DistritosController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        ViewBag.Distritos = await _context.Distritos.ToListAsync();
        return View();
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Distritos model)
    {
        if (ModelState.IsValid)
        {
            _context.Distritos.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Distrito registrado.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var model = await _context.Distritos.FindAsync(id);
        if (model != null)
        {
            _context.Distritos.Remove(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Distrito eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }
}