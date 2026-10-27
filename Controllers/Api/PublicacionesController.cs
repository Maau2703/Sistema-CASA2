using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class PublicacionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PublicacionesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Publicaciones>>> GetAll()
    {
        return await _context.Publicaciones
            .Include(p => p.Usuario)
            .Include(p => p.Mascotas)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Publicaciones>> GetById(int id)
    {
        var publicacion = await _context.Publicaciones
            .Include(p => p.Usuario)
            .Include(p => p.Mascotas)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (publicacion == null) return NotFound();
        return publicacion;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Publicaciones>> Create(Publicaciones publicacion)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        publicacion.UsuarioId = int.Parse(userIdClaim);
        publicacion.fecha = DateTime.Now;

        _context.Publicaciones.Add(publicacion);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = publicacion.Id }, publicacion);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var publicacion = await _context.Publicaciones.FindAsync(id);
        if (publicacion == null) return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (publicacion.UsuarioId.ToString() != userIdClaim && !User.IsInRole("Administrador"))
            return Forbid();

        _context.Publicaciones.Remove(publicacion);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}