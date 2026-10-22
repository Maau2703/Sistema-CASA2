namespace Proyecto.Models;

public class Ciudadanos
{
    public int Id{get;set;}
    public string barrio{set;get;}
    public int UsuarioId {get;set;}
    public Usuarios usuarios {get;set;}
    public List<SolicitudesAdopcion> Solicitud {get;set;}
    public List<Reportes> Reportes {get;set;}
}