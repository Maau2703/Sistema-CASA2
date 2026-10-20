namespace Proyecto.Models;
public class SolicitudesAdopcion
{
    public int Id {get;set;}
    public DateTime fecha {get;set;}
    public string Estado {get;set;}
    //relaciones
    public int mascotaId{get;set;}
    public Mascotas Mascotas {get;set;}
    public int CiudadanoId{get;set;}
    public Ciudadanos Ciudadanos {get;set;}

    public Adopciones Adopciones {get;set;}

}