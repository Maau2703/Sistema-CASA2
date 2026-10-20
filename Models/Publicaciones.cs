namespace Proyecto.Models;

public class Publicaciones
{
    public int Id {get;set;}
    public string titulo {get;set;}
    public string descripcion {get;set;}
    public DateTime fecha {get;set;}
    //relaciones 
    public int UsuarioId {get;set;}
    public List<Usuarios> Usuarios {get;set;} = new List<Usuarios>;
    public int MascotaId {get;set;}
    public Mascotas Mascotas {get;set;}
}