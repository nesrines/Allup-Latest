using Allup.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace Allup.Controllers;
public class BlogController : Controller
{
    private readonly AppDbContext _context;
    public BlogController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        return View();
    }
}