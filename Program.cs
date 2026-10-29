using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Nj.EntityFrameworkCore.LibSql;
using Proyecto.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN JSON: Ignorar ciclos de referencia y usar camelCase
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

var connectionString = builder.Configuration.GetConnectionString("TursoConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLibSql(connectionString));

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveSuperSecretaDeAlMenos32Caracteres1234";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CASA_Sucre";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name
        };

        // ✅ TRUCO CLAVE: Leer el token desde la Cookie si no viene en el Header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["auth_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// ✅ IMPORTANTE: Configurar el mapeo de claims para que funcione en los controllers
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// ✅ CORRECCIÓN CLAVE: Crear tablas automáticamente antes de ejecutar el Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        Console.WriteLine("🔄 Verificando/Creando base de datos en Turso...");
        db.Database.EnsureCreated();
        Console.WriteLine("✅ Base de datos lista. Tablas creadas o verificadas.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al verificar/crear la BD: {ex.Message}");
    }

    // Ahora sí, ejecutar el seed con las tablas ya existentes
    await DataSeeder.SeedAsync(db);
}

app.Run();