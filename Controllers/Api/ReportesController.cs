using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class ReportesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reportes>>> GetAll()
    {
        return await _context.Reportes
            .Include(r => r.Ciudadanos)
            .Include(r => r.Rescatistas)
            .Include(r => r.TipoReporte)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize(Roles = "Ciudadano")]
    public async Task<ActionResult<Reportes>> Create(Reportes reporte)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var usuario = await _context.Usuarios.Include(u => u.ciudadanos).FirstOrDefaultAsync(u => u.Id == int.Parse(userIdClaim));
        if (usuario?.ciudadanos == null) return BadRequest("El usuario no tiene perfil de ciudadano registrado.");

        reporte.CiudadanoId = usuario.ciudadanos.Id;
        reporte.Fecha = DateTime.Now;
        reporte.estado = "Pendiente";

        _context.Reportes.Add(reporte);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = reporte.Id }, reporte);
    }

    [HttpPut("{id}/asignar")]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<IActionResult> AsignarRescatista(int id, [FromBody] int rescatistaId)
    {
        var reporte = await _context.Reportes.FindAsync(id);
        if (reporte == null) return NotFound();

        reporte.RescatistasId = rescatistaId;
        reporte.estado = "En Proceso";
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/cerrar")]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<IActionResult> CerrarReporte(int id)
    {
        var reporte = await _context.Reportes.FindAsync(id);
        if (reporte == null) return NotFound();

        reporte.estado = "Resuelto";
        await _context.SaveChangesAsync();
        return NoContent();
    }
}