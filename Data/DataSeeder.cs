using Proyecto.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Solo crear si no existe (NO borrar)
        context.Database.EnsureCreated();



        // 1. ROLES
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Roles { Id = 1, nombre = "Ciudadano", Created_at = DateOnly.FromDateTime(DateTime.Now) },
                new Roles { Id = 2, nombre = "Rescatista", Created_at = DateOnly.FromDateTime(DateTime.Now) },
                new Roles { Id = 3, nombre = "Administrador", Created_at = DateOnly.FromDateTime(DateTime.Now) }
            );
            await context.SaveChangesAsync();

        }

        // 2. DISTRITOS (8 Distritos de Sucre) - ACTUALIZAR O INSERTAR
        var distritosData = new[]
        {
            new { Id = 1, nombre = "Distrito 1", ColorCinta = "Rojo", Poligono = "San Sebastián, Santa Ana, San Lázaro, San Roque, La Rotonda, María Auxiliadora, Tata San Juan, 25 de Mayo, Hernando Siles, Huayrapata" },
            new { Id = 2, nombre = "Distrito 2", ColorCinta = "Azul", Poligono = "San Juanillo, Mercado Campesino, Normal, Garcilazo, Santa Bárbara, Petrolero, Mesa Verde, Alto Delicias, Kurusa Llawi" },
            new { Id = 3, nombre = "Distrito 3", ColorCinta = "Verde", Poligono = "Yurac Yurac, Lajastambo, Villa Marlecita, Alto Sucre, Nuevo Mundo, Japonesa, Libertadores" },
            new { Id = 4, nombre = "Distrito 4", ColorCinta = "Amarillo", Poligono = "Cementerio, El Tejar, La Florida, Max Toledo, Aranjuez, La Calancha, Universitario, Molle Pampa, Ferroviario, Alto San Martín" },
            new { Id = 5, nombre = "Distrito 5", ColorCinta = "Naranja", Poligono = "Azari, San Antonio, Huayrapata, Villa Rosario, Eva Perón, Patacón, Bella Vista, El Palmar" },
            new { Id = 6, nombre = "Distrito 6", ColorCinta = "Morado", Poligono = "Sonkho Chata, Imilla Pampa, Totacoa, Chuclla, Melchora, K'atalla" },
            new { Id = 7, nombre = "Distrito 7", ColorCinta = "Celeste", Poligono = "Chuqui Chuquí, Huanifaya, Mojotoro, Chaco, Surima, El Cortijo, Bella Vista" },
            new { Id = 8, nombre = "Distrito 8", ColorCinta = "Marrón", Poligono = "Potolo, Quila Quila, Mama Huasi, Punilla, Maragua, Chaunaca" }
        };

        foreach (var d in distritosData)
        {
            var existente = context.Distritos.FirstOrDefault(x => x.Id == d.Id);
            if (existente != null)
            {
                existente.nombre = d.nombre;
                existente.ColorCinta = d.ColorCinta;
                existente.Poligono = d.Poligono;
                context.Distritos.Update(existente);
            }
            else
            {
                context.Distritos.Add(new Distritos { Id = d.Id, nombre = d.nombre, ColorCinta = d.ColorCinta, Poligono = d.Poligono });
            }
        }
        await context.SaveChangesAsync();


        // 3. TIPOS DE REPORTE (5 tipos) - ACTUALIZAR O INSERTAR
        var tiposReporteData = new[]
        {
            new { Id = 1, nombre = "Mascota Accidentada / Atropellada" },
            new { Id = 2, nombre = "Avistamiento en Situación de Calle" },
            new { Id = 3, nombre = "Mascota Perdida" },
            new { Id = 4, nombre = "Mascota Encontrada" },
            new { Id = 5, nombre = "Maltrato o Abandono Animal" }
        };

        foreach (var tr in tiposReporteData)
        {
            var existente = context.TipoReportes.FirstOrDefault(x => x.Id == tr.Id);
            if (existente != null)
            {
                existente.nombre = tr.nombre;
                context.TipoReportes.Update(existente);
            }
            else
            {
                context.TipoReportes.Add(new TipoReporte { Id = tr.Id, nombre = tr.nombre });
            }
        }
        await context.SaveChangesAsync();

        // 4. TIPOS DE ESTABLECIMIENTOS (14 tipos) - ACTUALIZAR O INSERTAR
        var tiposEstData = new[]
        {
            new { Id = 1, nombre = "Clínica Veterinaria" },
            new { Id = 2, nombre = "Consultorio Veterinario" },
            new { Id = 3, nombre = "Hospital Veterinario" },
            new { Id = 4, nombre = "Laboratorio Veterinario" },
            new { Id = 5, nombre = "Albergue / Refugio Municipal" },
            new { Id = 6, nombre = "Albergue de ONG / Fundación" },
            new { Id = 7, nombre = "Santuario de Animales" },
            new { Id = 8, nombre = "Hogar Temporal (Casa Cuna)" },
            new { Id = 9, nombre = "Tienda de Mascotas (Pet Shop)" },
            new { Id = 10, nombre = "Peluquería y Spa Canino/Felino" },
            new { Id = 11, nombre = "Hotel y Guardería para Mascotas" },
            new { Id = 12, nombre = "Centro de Adiestramiento y Psicología Canina" },
            new { Id = 13, nombre = "Cementerio de Mascotas" },
            new { Id = 14, nombre = "Crematorio Veterinario" }
        };

        foreach (var te in tiposEstData)
        {
            var existente = context.TipoEstablecimientos.FirstOrDefault(x => x.Id == te.Id);
            if (existente != null)
            {
                existente.nombre = te.nombre;
                context.TipoEstablecimientos.Update(existente);
            }
            else
            {
                context.TipoEstablecimientos.Add(new TipoEstablecimientos { Id = te.Id, nombre = te.nombre });
            }
        }
        await context.SaveChangesAsync();


        // 5. USUARIOS DE PRUEBA
        if (!context.Usuarios.Any(u => u.ci == "0000001"))
        {
            var admin = new Usuarios
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
            };
            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();

        }

        if (!context.Usuarios.Any(u => u.ci == "0000002"))
        {
            var rescatista = new Usuarios
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
            };
            context.Usuarios.Add(rescatista);
            await context.SaveChangesAsync();

        }

        if (!context.Usuarios.Any(u => u.ci == "0000003"))
        {
            var ciudadano = new Usuarios
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
            };
            context.Usuarios.Add(ciudadano);
            await context.SaveChangesAsync();

            if (!context.Ciudadanos.Any(c => c.UsuarioId == ciudadano.Id))
            {
                context.Ciudadanos.Add(new Ciudadanos 
                { 
                    UsuarioId = ciudadano.Id, 
                    barrio = "Centro" 
                });
                await context.SaveChangesAsync();
            }

        }

        Console.WriteLine("🎉 Seed completado exitosamente!");
    }
}