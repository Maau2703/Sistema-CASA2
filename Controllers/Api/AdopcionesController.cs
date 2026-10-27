using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class AdopcionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdopcionesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<ActionResult<IEnumerable<Adopciones>>> GetAll()
    {
        return await _context.Adopciones
            .Include(a => a.solicitudesAdopcion)
            .ThenInclude(s => s.Mascotas)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Adopciones>> GetById(int id)
    {
        var adopcion = await _context.Adopciones
            .Include(a => a.solicitudesAdopcion)
            .FirstOrDefaultAsync(a => a.Id == id);
            
        if (adopcion == null) return NotFound();
        return adopcion;
    }
}