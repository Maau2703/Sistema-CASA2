namespace Proyecto.Models;
public class Tramites
{
    public int Id {get;set;}
    public string nombre {get;set;}
    public string descripcion {get;set;}
    public string requisitos {get;set;}
    public string enlace {get;set;}
    //relaciones
    public int EstableciminetosId {get;set;}
    public Establecimientos Establecimientos {get;set;}
}