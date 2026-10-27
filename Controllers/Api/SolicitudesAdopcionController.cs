using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class SolicitudesAdopcionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SolicitudesAdopcionController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SolicitudesAdopcion>>> GetAll()
    {
        return await _context.SolicitudesAdopcion
            .Include(s => s.Mascotas)
            .Include(s => s.Ciudadanos)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize(Roles = "Ciudadano")]
    public async Task<ActionResult<SolicitudesAdopcion>> Create(SolicitudesAdopcion solicitud)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var usuario = await _context.Usuarios.Include(u => u.ciudadanos).FirstOrDefaultAsync(u => u.Id == int.Parse(userIdClaim));
        
        solicitud.CiudadanoId = usuario.ciudadanos.Id;
        solicitud.fecha = DateTime.Now;
        solicitud.Estado = "Pendiente";

        _context.SolicitudesAdopcion.Add(solicitud);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = solicitud.Id }, solicitud);
    }

    [HttpPut("{id}/aprobar")]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<IActionResult> AprobarYGenerarAdopcion(int id, [FromBody] AdopcionRequest request)
    {
        var solicitud = await _context.SolicitudesAdopcion
            .Include(s => s.Mascotas)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (solicitud == null) return NotFound();

        solicitud.Estado = "Aprobada";

        var nuevaAdopcion = new Adopciones
        {
            numeroSerie = request.NumeroSerie,
            firma = request.FirmaHash,
            fecha = DateTime.Now,
            SolicitudId = solicitud.Id
        };

        if (solicitud.Mascotas != null)
        {
            solicitud.Mascotas.Estado = "Adoptado";
        }

        _context.Adopciones.Add(nuevaAdopcion);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Solicitud aprobada y contrato de adopción generado.", adopcionId = nuevaAdopcion.Id });
    }
}

public class AdopcionRequest
{
    public string NumeroSerie { get; set; } = string.Empty;
    public string FirmaHash { get; set; } = string.Empty;
}