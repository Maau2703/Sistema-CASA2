using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class ReportesController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Reportes de animales";
        ViewData["Page"] = "reportes";
        return View();
    }
}