using Allup.DataAccessLayer;
using Allup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents;
public class CategoryPartViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    public CategoryPartViewComponent(AppDbContext context) { _context = context; }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View(await _context.Categories.Where(c => !c.IsDeleted && c.IsMain).ToListAsync());
    }
}