using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class EstablecimientosController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Veterinarias y albergues";
        ViewData["Page"] = "establecimientos";
        return View();
    }
}