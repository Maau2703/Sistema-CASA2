using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers;

public class AdminController : Controller
{
    public IActionResult Usuarios()
    {
        ViewData["Title"] = "Gestión de Usuarios";
        ViewData["Page"] = "admin-usuarios";
        return View();
    }

    public IActionResult Roles()
    {
        ViewData["Title"] = "Gestión de Roles";
        ViewData["Page"] = "admin-roles";
        return View();
    }
}