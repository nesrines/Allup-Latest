using Allup.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers;
public class ContactController : Controller
{
    private readonly AppDbContext _context;
    public ContactController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        return View();
    }
}