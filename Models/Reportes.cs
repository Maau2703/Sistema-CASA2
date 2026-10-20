namespace Proyecto.Models;
public class Reportes
{
    public int Id {get;set;}
    public string estado{get;set;}
    public string Ubicaion {get;set;}
    public string Descripcion {get;set;}
    public DateTime Fecha {get;set;}
    //relaciones
    public int tipoReportesId {get;set;}
    public TipoReporte TipoReporte {get;set;}
    public int CiudadanoId {get;set;}
    public Ciudadanos Ciudadanos {get;set;}
    public int RescatistasId {get;set;}
    public Rescatistas Rescatistas {get;set;}

}