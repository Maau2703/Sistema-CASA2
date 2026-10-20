namespace Proyecto.Models;

public class Adopciones
{
    public int Id {get;set;}
    public string numeroSerie {get;set;}
    public string firma {get;set;}
    public DateTime fecha {get;set;}
    //relacion 
    public int SolicitudId {get;set;}
    public SolicitudesAdopcion solicitudesAdopcion {get;set;}
}