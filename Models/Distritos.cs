namespace Proyecto.Models;
public class Distritos
{
    public int Id { get; set; }
    public string nombre { get; set; }
    public string ColorCinta {get;set;}
    public string Poligono {get;set;}
    public List<Usuarios> Usuarios {get;set;} = new();
    public List<Mascotas> Mascotas {get;set;} = new();
    public List<Establecimientos> Establecimientos {get;set;} = new();
}