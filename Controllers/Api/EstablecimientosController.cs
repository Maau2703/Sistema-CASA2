using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class EstablecimientosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EstablecimientosController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Establecimientos>>> GetAll()
    {
        return await _context.Establecimientos
            .Include(e => e.TipoEstablecimiento)
            .Include(e => e.Distritos)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Establecimientos>> Create(Establecimientos establecimiento)
    {
        _context.Establecimientos.Add(establecimiento);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = establecimiento.Id }, establecimiento);
    }
}