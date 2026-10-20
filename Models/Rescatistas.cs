namespace Proyecto.Models;
public class Rescatistas
{
    public int Id {get;set;}
    public int UsuariosId{set;get;}
    public Usuarios usuarios {get;set;}
    public string disponible {get;set;}
}