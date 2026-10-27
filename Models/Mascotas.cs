using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto.Models;

public class Mascotas
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Nombre { get; set; }
    
    [Required]
    public string Especie { get; set; }
    
    [Required]
    public string Edad { get; set; }
    
    [Required]
    public string Tamaño { get; set; }
    
    [Required]
    public string Color { get; set; }
    
    public string Caracteristica { get; set; }
    
    public string Foto { get; set; }
    
    public string Estado { get; set; }
    
    // Relaciones
    [Required]
    public int DistritoId { get; set; }
    
    [ForeignKey("DistritoId")]
    public Distritos Distritos { get; set; }
}