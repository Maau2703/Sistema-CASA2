using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;


public class AdopcionesController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Adopciones";
        ViewData["Page"] = "adopciones";
        return View();
    }
}