using Allup.DataAccessLayer;
using Allup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents;
public class SliderViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    public SliderViewComponent(AppDbContext context) { _context = context; }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<Slider> sliders = await _context.Sliders.Where(s => !s.IsDeleted).ToListAsync();
        return View(sliders);
    }
}