using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Proyecto.Data;
using Proyecto.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
[HttpPost("registro")]
public async Task<IActionResult> Registro([FromBody] Usuarios nuevoUsuario)
{
    if (await _context.Usuarios.AnyAsync(u => u.ci == nuevoUsuario.ci || u.Email == nuevoUsuario.Email))
        return BadRequest("Ya existe un usuario con ese CI o Email.");

    nuevoUsuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevoUsuario.Password);
    nuevoUsuario.Created_at = DateOnly.FromDateTime(DateTime.Now);
    nuevoUsuario.RolesId = 1; // Rol Ciudadano por defecto

    _context.Usuarios.Add(nuevoUsuario);
    await _context.SaveChangesAsync();

    return Ok(new { mensaje = "Usuario registrado exitosamente" });
}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.ci == request.Ci || u.Email == request.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
            return Unauthorized("Credenciales inválidas.");

        var token = GenerarTokenJWT(usuario);
        return Ok(new { 
            token, 
            usuario = new { 
                usuario.Id, 
                usuario.nombre, 
                usuario.Email, 
                Rol = usuario.Roles.nombre 
            } 
        });
    }

    private string GenerarTokenJWT(Usuarios usuario)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "ClaveSuperSecretaDeAlMenos32Caracteres1234";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "CASA_Sucre";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.nombre),
            new Claim(ClaimTypes.Role, usuario.Roles.nombre),
            new Claim("Ci", usuario.ci)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Ci { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
