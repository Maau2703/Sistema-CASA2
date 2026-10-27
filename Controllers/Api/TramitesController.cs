using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class TramitesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TramitesController(ApplicationDbContext context) => _context = context;

    [HttpGet("establecimiento/{establecimientoId}")]
    public async Task<ActionResult<IEnumerable<Tramites>>> GetByEstablecimiento(int establecimientoId)
    {
        return await _context.Tramites
            .Where(t => t.EstableciminetosId == establecimientoId)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<Tramites>> Create(Tramites tramite)
    {
        _context.Tramites.Add(tramite);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByEstablecimiento), new { establecimientoId = tramite.EstableciminetosId }, tramite);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, Tramites tramite)
    {
        if (id != tramite.Id) return BadRequest();
        _context.Entry(tramite).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var tramite = await _context.Tramites.FindAsync(id);
        if (tramite == null) return NotFound();
        _context.Tramites.Remove(tramite);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}