using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class PerfilController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Mi Perfil";
        ViewData["Page"] = "perfil";
        return View();
    }
}