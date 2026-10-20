using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
//using sistemaVentas.Models;

namespace sistemaVentas.Data;
public class AppDbContext : DbContext
{
    //constructor que va a recibir las opciones de configuracion
    public AppDbContext(DbContextOptions<AppDbContext>options):base(options){}
    //definir los Dbset, que se traduciran en las tablas de datos
    //public DbSet<Categoria> Categorias => Set<Categoria>();
    //Metodos paraconfigurar restricciones , relaciones y datos iniciales
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        //Insercion de datos a la tabla Categoria
        //modelBuilder.Entity<Categoria>().HasData(
           // new Categoria {Id = 1,Nombre = "Limpieza"},
           // new Categoria {Id = 2,Nombre = "Abarrotes"},
           // new Categoria {Id = 3,Nombre = "Bebidas"}
       // );
        //modelBuilder.Entity<Producto>().HasData(
            //new Producto {Id = 1,Nombre = "Jabon",Precio = 8,Stock = 100,StockMinimo = 5,CategoriaId = 1},
            //new Producto {Id = 2,Nombre = "Arroz",Precio = 16,Stock = 100,StockMinimo = 10,CategoriaId = 2},
            //new Producto {Id = 3,Nombre = "Coca cola",Precio = 13,Stock = 100,StockMinimo = 10,CategoriaId = 3}
       // );
    }
}