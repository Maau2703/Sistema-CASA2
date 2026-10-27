using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class DistritosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DistritosController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Distritos>>> GetAll()
    {
        return await _context.Distritos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Distritos>> GetById(int id)
    {
        var distrito = await _context.Distritos.FindAsync(id);
        if (distrito == null) return NotFound();
        return distrito;
    }
}