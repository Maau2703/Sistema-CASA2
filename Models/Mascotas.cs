using System.Runtime.InteropServices.Marshalling;

namespace Proyecto.Models;
public class Mascotas
{
    public int Id {get;set;}
    public string nombre {get;set;}
    public string especie {get;set;}
    public string edad {get;set;}
    public string tamaño {get;set;}
    public string color {get;set;}
    public string caracteristica {get;set;}
    public string foto {get;set;}
    public string Estado {get;set;}
    //relaciones
    public int DistritoId {get;set;}
    public Distritos Distritos {get;set;}
}