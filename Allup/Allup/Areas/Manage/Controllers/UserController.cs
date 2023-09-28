using Microsoft.AspNetCore.Mvc;

namespace Allup.Areas.Manage.Controllers;
public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}