using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class MapaController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Mapa de Sucre";
        ViewData["Page"] = "mapa";
        return View();
    }
}