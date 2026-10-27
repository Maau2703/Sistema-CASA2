using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class DenunciaAbandonosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DenunciaAbandonosController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<ActionResult<IEnumerable<DenunciaAbandonos>>> GetAll()
    {
        return await _context.DenunciaAbandonos
            .Include(d => d.Reportes)
            .ToListAsync();
    }

    [HttpPost]
    [Authorize(Roles = "Ciudadano,Rescatista,Administrador")]
    public async Task<ActionResult<DenunciaAbandonos>> Create(DenunciaAbandonos denuncia)
    {
        var reporte = await _context.Reportes.FindAsync(denuncia.ReporteId);
        if (reporte == null) return BadRequest("El reporte asociado no existe.");

        _context.DenunciaAbandonos.Add(denuncia);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = denuncia.Id }, denuncia);
    }
}