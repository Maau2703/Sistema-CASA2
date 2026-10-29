namespace Proyecto.Models;

public class Reportes
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string estado { get; set; } = string.Empty;
    public string Ubicaion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    
    // ✅ NUEVO: Campo para el comentario de resolución
    public string? ComentarioResolucion { get; set; }

    // Relaciones
    public int? tipoReportesId { get; set; }
    public TipoReporte? TipoReporte { get; set; }
    
    public int? CiudadanoId { get; set; }
    public Ciudadanos? Ciudadanos { get; set; }

    // ✅ RELACIÓN CON RESCATISTAS (Necesaria para que tu proyecto compile)
    public int? RescatistasId { get; set; }
    public Rescatistas? Rescatistas { get; set; }
}