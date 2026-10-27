using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class CiudadanosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CiudadanosController(ApplicationDbContext context) => _context = context;

    [HttpGet("mi-perfil")]
    [Authorize]
    public async Task<ActionResult<Ciudadanos>> GetMiPerfil()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var ciudadano = await _context.Ciudadanos
            .Include(c => c.usuarios)
            .FirstOrDefaultAsync(c => c.UsuarioId == int.Parse(userIdClaim));

        if (ciudadano == null) return NotFound("Perfil de ciudadano no encontrado.");
        return ciudadano;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Ciudadanos>> Create(Ciudadanos ciudadano)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        ciudadano.UsuarioId = int.Parse(userIdClaim);
        _context.Ciudadanos.Add(ciudadano);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMiPerfil), new { }, ciudadano);
    }
}