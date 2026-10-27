using System.Collections.Generic;

namespace Proyecto.Models;

public class Roles
{
    public int Id { get; set; }
    public string nombre { get; set; }
    public DateOnly Created_at { get; set; }
    
    public List<Usuarios> Usuarios { get; set; } = new();
}