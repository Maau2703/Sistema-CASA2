namespace Proyecto.Models;

public class Ciudadanos
{
    public int Id{get;set;}
    public int UsuariosId{get;set;}
    public Usuarios usuarios{get;set;}
    public string barrio{get;set;}
    public List<Reportes> Reporte {get;set;} = new List<Reportes>();
    public List<SolicitudesAdopcion> SolicitudesAdopcion {get;set;} = new List<SolicitudesAdopcion>();
}