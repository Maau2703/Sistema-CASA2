using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ComentariosController(ApplicationDbContext context) => _context = context;

    [HttpGet("publicacion/{publicacionId}")]
    public async Task<ActionResult<IEnumerable<Comentarios>>> GetByPublicacion(int publicacionId)
    {
        return await _context.Comentarios
            .Include(c => c.usuarios)
            .Where(c => c.PublicacionId == publicacionId)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Comentarios>> Create(Comentarios comentario)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        comentario.UsuarioId = int.Parse(userIdClaim);
        comentario.Fecha = DateTime.Now;

        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByPublicacion), new { publicacionId = comentario.PublicacionId }, comentario);
    }
}