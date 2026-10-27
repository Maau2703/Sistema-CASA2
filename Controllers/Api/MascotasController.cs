using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class MascotasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MascotasController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mascotas>>> GetAll()
    {
        return await _context.Mascotas.Include(m => m.Distritos).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Mascotas>> GetById(int id)
    {
        var mascota = await _context.Mascotas.Include(m => m.Distritos).FirstOrDefaultAsync(m => m.Id == id);
        if (mascota == null) return NotFound();
        return mascota;
    }

    [HttpPost]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<ActionResult<Mascotas>> Create(Mascotas mascota)
    {
        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = mascota.Id }, mascota);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<IActionResult> Update(int id, Mascotas mascota)
    {
        if (id != mascota.Id) return BadRequest();
        _context.Entry(mascota).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota == null) return NotFound();
        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}