using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context) => _context = context;

    [HttpGet("mi-perfil")]
    [Authorize]
    public async Task<ActionResult<Usuarios>> GetMiPerfil()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var usuario = await _context.Usuarios
            .Include(u => u.Roles)
            .Include(u => u.Distritos)
            .FirstOrDefaultAsync(u => u.Id == int.Parse(userIdClaim));

        if (usuario == null) return NotFound();
        
        usuario.Password = null; 
        return usuario;
    }

    [HttpPut("actualizar")]
    [Authorize]
    public async Task<IActionResult> UpdatePerfil(Usuarios usuarioActualizado)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var usuario = await _context.Usuarios.FindAsync(int.Parse(userIdClaim));
        if (usuario == null) return NotFound();

        usuario.nombre = usuarioActualizado.nombre;
        usuario.Email = usuarioActualizado.Email;
        usuario.celular = usuarioActualizado.celular;
        usuario.fotoPerfil = usuarioActualizado.fotoPerfil;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}