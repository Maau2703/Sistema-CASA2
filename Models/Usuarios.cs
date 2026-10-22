
namespace Proyecto.Models;
public class Usuarios
{
    public int Id { get; set; }
    public string nombre { get; set; }
    public string Email {get; set;}
    public string celular {get;set;}
    public string ci {get; set;}
    public string Password {get;set;}
    public string fotoPerfil {get;set;}
    //Relaciones
    public int RolesId {get;set;}
    public Roles Roles {get;set;}
    public int DistritoId {get;set;}
    public Distritos Distritos {get;set;}
    public List<Comentarios> comentarios {get;set;} = new ();
    public Ciudadanos ciudadanos{get;set;}
    //fecha de registro
    public DateOnly Created_at {get;set;}
}