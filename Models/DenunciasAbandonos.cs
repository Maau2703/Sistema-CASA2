namespace Proyecto.Models;
public class DenunciaAbandonos
{
    public int Id {get;set;}
    public string denunciado {get;set;}
    public string evidencia {get;set;}
    public int ReporteId {get;set;}
    public Reportes Reportes {get;set;}
}