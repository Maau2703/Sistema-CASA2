using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class RescatistasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RescatistasController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rescatistas>>> GetAll()
    {
        return await _context.Rescatistas
            .Include(r => r.usuarios)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Rescatistas>> GetById(int id)
    {
        var rescatista = await _context.Rescatistas
            .Include(r => r.usuarios)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (rescatista == null) return NotFound();
        return rescatista;
    }

    [HttpPut("{id}/disponibilidad")]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<IActionResult> UpdateDisponibilidad(int id, [FromBody] string disponible)
    {
        var rescatista = await _context.Rescatistas.FindAsync(id);
        if (rescatista == null) return NotFound();

        rescatista.disponible = disponible;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}