using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers;
public class HomeController : Controller
{
    public async Task<IActionResult> Index() { return View(); }
}