using Microsoft.AspNetCore.Mvc;

namespace FurnitureFactoryManager.Controllers;

public class HomeController : Controller
{
    public IActionResult Error()
    {
        return View();
    }
}
