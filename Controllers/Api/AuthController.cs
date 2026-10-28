using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proyecto.Data;
using Proyecto.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtKey = configuration["Jwt:Key"] ?? "ClaveSuperSecretaDeAlMenos32Caracteres1234";
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "CASA_Sucre";
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] Usuarios nuevoUsuario)
    {
        try
        {
            if (string.IsNullOrEmpty(nuevoUsuario.ci) || string.IsNullOrEmpty(nuevoUsuario.Password))
                return BadRequest(new { error = "CI y Contraseña son obligatorios" });

            if (await _context.Usuarios.AnyAsync(u => u.ci == nuevoUsuario.ci))
                return BadRequest(new { error = "Ya existe un usuario con ese CI" });

            // BLINDAJE: Si el frontend no envía RolesId o DistritoId, usamos valores por defecto válidos (1)
            nuevoUsuario.RolesId = nuevoUsuario.RolesId > 0 ? nuevoUsuario.RolesId : 1;
            nuevoUsuario.DistritoId = nuevoUsuario.DistritoId > 0 ? nuevoUsuario.DistritoId : 1;
            nuevoUsuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevoUsuario.Password);
            nuevoUsuario.Created_at = DateOnly.FromDateTime(DateTime.Now);

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Si es ciudadano, crear su registro asociado
            if (nuevoUsuario.RolesId == 1)
            {
                _context.Ciudadanos.Add(new Ciudadanos 
                { 
                    UsuarioId = nuevoUsuario.Id, 
                    barrio = "Por definir" 
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Usuario registrado correctamente" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR REGISTRO: {ex.Message}");
            return StatusCode(500, new { error = "Error interno al registrar usuario", detalle = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Roles)
                .Include(u => u.Distritos)
                .FirstOrDefaultAsync(u => u.ci == request.ci || u.Email == request.ci);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.password, usuario.Password))
            {
                return Unauthorized(new { error = "Credenciales inválidas" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim("ci", usuario.ci),
                new Claim("nombre", usuario.nombre),
                new Claim(ClaimTypes.Role, usuario.Roles?.nombre ?? "Ciudadano")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // ✅ GUARDAR EL TOKEN EN UNA COOKIE para que las vistas Razor lo lean automáticamente
            Response.Cookies.Append("auth_token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Cambiar a true si usas HTTPS en producción
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddDays(1)
            });

            return Ok(new 
            { 
                token = tokenString,
                usuario = new 
                {
                    id = usuario.Id,
                    nombre = usuario.nombre,
                    ci = usuario.ci,
                    rol = usuario.Roles?.nombre,
                    distrito = usuario.Distritos?.nombre
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR LOGIN: {ex.Message}");
            return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
        }
    }
}

// Clase auxiliar para recibir el login
public class LoginRequest
{
    public string ci { get; set; }
    public string password { get; set; }
}