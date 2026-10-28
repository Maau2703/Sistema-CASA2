using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class MascotasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MascotasController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        try
        {
            // PROYECCIÓN: Solo traemos los campos necesarios, evitando ciclos y errores
            var mascotas = await _context.Mascotas
                .AsNoTracking()
                .Select(m => new
                {
                    m.Id,
                    m.Nombre,
                    m.Especie,
                    m.Edad,
                    m.Tamaño,
                    m.Color,
                    m.Caracteristica,
                    m.Foto,
                    m.Estado,
                    m.DistritoId,
                    DistritoNombre = m.Distritos != null ? m.Distritos.nombre : null,
                    DistritoColor = m.Distritos != null ? m.Distritos.ColorCinta : null
                })
                .ToListAsync();

            Console.WriteLine($"✅ API Mascotas: {mascotas.Count} mascotas cargadas");
            return Ok(mascotas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR API MASCOTAS: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        try
        {
            var mascota = await _context.Mascotas
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    m.Id,
                    m.Nombre,
                    m.Especie,
                    m.Edad,
                    m.Tamaño,
                    m.Color,
                    m.Caracteristica,
                    m.Foto,
                    m.Estado,
                    m.DistritoId,
                    DistritoNombre = m.Distritos != null ? m.Distritos.nombre : null,
                    DistritoColor = m.Distritos != null ? m.Distritos.ColorCinta : null
                })
                .FirstOrDefaultAsync();

            if (mascota == null) return NotFound();
            return Ok(mascota);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR API MASCOTAS GETBYID: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<ActionResult<Mascotas>> Create(Mascotas mascota)
    {
        try
        {
            _context.Mascotas.Add(mascota);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = mascota.Id }, mascota);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR API MASCOTAS CREATE: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Rescatista,Administrador")]
    public async Task<IActionResult> Update(int id, Mascotas mascota)
    {
        try
        {
            if (id != mascota.Id) return BadRequest();
            _context.Entry(mascota).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR API MASCOTAS UPDATE: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();
            _context.Mascotas.Remove(mascota);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR API MASCOTAS DELETE: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}