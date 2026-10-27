using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Inicio";
        ViewData["Page"] = "index";
        return View();
    }
}