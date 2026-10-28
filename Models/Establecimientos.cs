namespace Proyecto.Models;

public class Establecimientos
{
    public int Id { get; set; }
    public string nombre { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
    public string ubicacion { get; set; }
    public string horario { get; set; }
    public int capacidad { get; set; }
    public decimal calificacion { get; set; }
    public string estado { get; set; }
    
    public int TipoEstablecimientoId { get; set; }
    public TipoEstablecimientos TipoEstablecimiento { get; set; }
    
    public int DistritoId { get; set; }
    public Distritos Distritos { get; set; }
    
    public List<Tramites> Tramites { get; set; } = new();
}