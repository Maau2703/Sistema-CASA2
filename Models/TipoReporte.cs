using System.Collections.Generic;

namespace Proyecto.Models;

public class TipoReporte
{
    public int Id { get; set; }
    public string nombre { get; set; }
    public ICollection<Reportes> Reportes { get; set; } = new List<Reportes>();
}