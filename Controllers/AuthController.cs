using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        ViewData["Title"] = "Iniciar Sesión";
        ViewData["Page"] = "login";
        return View();
    }

    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "Acceso Denegado";
        return View();
    }
}