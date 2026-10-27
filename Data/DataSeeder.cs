using Proyecto.Models;

namespace Proyecto.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Roles { Id = 1, nombre = "Ciudadano", Created_at = DateOnly.FromDateTime(DateTime.Now) },
                new Roles { Id = 2, nombre = "Rescatista", Created_at = DateOnly.FromDateTime(DateTime.Now) },
                new Roles { Id = 3, nombre = "Administrador", Created_at = DateOnly.FromDateTime(DateTime.Now) }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Distritos.Any())
        {
            context.Distritos.AddRange(
                new Distritos { Id = 1, nombre = "Distrito 1", ColorCinta = "Rojo", Poligono = "POLIGONO_1" },
                new Distritos { Id = 2, nombre = "Distrito 2", ColorCinta = "Azul", Poligono = "POLIGONO_2" },
                new Distritos { Id = 3, nombre = "Distrito 3", ColorCinta = "Verde", Poligono = "POLIGONO_3" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TipoEstablecimientos.Any())
        {
            context.TipoEstablecimientos.AddRange(
                new TipoEstablecimientos { Id = 1, nombre = "Veterinaria" },
                new TipoEstablecimientos { Id = 2, nombre = "Albergue" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Usuarios.Any(u => u.ci == "0000001"))
        {
            context.Usuarios.Add(new Usuarios
            {
                nombre = "Administrador del Sistema",
                Email = "admin@casa.bo",
                ci = "0000001",
                celular = "70000000",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                RolesId = 3,
                DistritoId = 1,
                fotoPerfil = "",
                Created_at = DateOnly.FromDateTime(DateTime.Now)
            });
            await context.SaveChangesAsync();
        }

        if (!context.Usuarios.Any(u => u.ci == "0000002"))
        {
            context.Usuarios.Add(new Usuarios
            {
                nombre = "Rescatista Demo",
                Email = "rescatista@casa.bo",
                ci = "0000002",
                celular = "70000001",
                Password = BCrypt.Net.BCrypt.HashPassword("Rescate1234!"),
                RolesId = 2,
                DistritoId = 1,
                fotoPerfil = "",
                Created_at = DateOnly.FromDateTime(DateTime.Now)
            });
            await context.SaveChangesAsync();
        }

        if (!context.Usuarios.Any(u => u.ci == "0000003"))
        {
            context.Usuarios.Add(new Usuarios
            {
                nombre = "Ciudadano Demo",
                Email = "ciudadano@casa.bo",
                ci = "0000003",
                celular = "70000002",
                Password = BCrypt.Net.BCrypt.HashPassword("Casa1234!"),
                RolesId = 1,
                DistritoId = 1,
                fotoPerfil = "",
                Created_at = DateOnly.FromDateTime(DateTime.Now)
            });
            await context.SaveChangesAsync();
        }
    }
}