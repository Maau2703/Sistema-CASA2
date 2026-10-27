using Microsoft.EntityFrameworkCore;
using Proyecto.Models;

namespace Proyecto.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Distritos> Distritos { get; set; }
    public DbSet<Ciudadanos> Ciudadanos { get; set; }
    public DbSet<Rescatistas> Rescatistas { get; set; }
    public DbSet<Mascotas> Mascotas { get; set; }
    public DbSet<Reportes> Reportes { get; set; }
    public DbSet<TipoReporte> TipoReportes { get; set; }
    public DbSet<SolicitudesAdopcion> SolicitudesAdopcion { get; set; }
    public DbSet<Adopciones> Adopciones { get; set; }
    public DbSet<Publicaciones> Publicaciones { get; set; }
    public DbSet<Comentarios> Comentarios { get; set; }
    public DbSet<DenunciaAbandonos> DenunciaAbandonos { get; set; }
    public DbSet<Establecimientos> Establecimientos { get; set; }
    public DbSet<TipoEstablecimientos> TipoEstablecimientos { get; set; }
    public DbSet<Tramites> Tramites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuarios>()
            .HasOne(u => u.Roles)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolesId);

        modelBuilder.Entity<Usuarios>()
            .HasOne(u => u.Distritos)
            .WithMany(d => d.Usuarios)
            .HasForeignKey(u => u.DistritoId);

        modelBuilder.Entity<Ciudadanos>()
            .HasOne(c => c.usuarios)
            .WithOne(u => u.ciudadanos)
            .HasForeignKey<Ciudadanos>(c => c.UsuarioId);

        modelBuilder.Entity<Reportes>()
            .HasOne(r => r.TipoReporte)
            .WithMany(t => t.Reportes)
            .HasForeignKey(r => r.tipoReportesId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reportes>()
            .HasOne(r => r.Ciudadanos)
            .WithMany(c => c.Reportes)
            .HasForeignKey(r => r.CiudadanoId);

        modelBuilder.Entity<Reportes>()
            .HasOne(r => r.Rescatistas)
            .WithMany()
            .HasForeignKey(r => r.RescatistasId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudesAdopcion>()
            .HasOne(s => s.Mascotas)
            .WithMany()
            .HasForeignKey(s => s.mascotaId);

        modelBuilder.Entity<SolicitudesAdopcion>()
            .HasOne(s => s.Ciudadanos)
            .WithMany(c => c.Solicitud)
            .HasForeignKey(s => s.CiudadanoId);

        modelBuilder.Entity<Adopciones>()
            .HasOne(a => a.solicitudesAdopcion)
            .WithOne(s => s.Adopciones)
            .HasForeignKey<Adopciones>(a => a.SolicitudId);
    }
}