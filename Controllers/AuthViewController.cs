using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class AuthViewController : Controller
{
    public IActionResult Login()
    {
        ViewData["Title"] = "Entrar";
        ViewData["Page"] = "login";
        return View();
    }
}