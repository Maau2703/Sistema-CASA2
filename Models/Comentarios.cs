namespace Proyecto.Models;
public class Comentarios
{
    public int Id { get; set; }
    public string texto { get; set; }
    public int PublicacionId {get;set;}
    public Publicaciones Publicacion {get;set;}
      public int UsuarioId {get;set;}
    public Usuarios usuarios {get;set;}
    public DateTime Fecha {get;set;}
}