namespace Proyecto.Models;
public class Roles
{
    public int Id { get; set; }
    public string nombre { get; set; }
    public Usuarios usuarios {get;set;}
    public DateOnly Created_at {get;set;}
}