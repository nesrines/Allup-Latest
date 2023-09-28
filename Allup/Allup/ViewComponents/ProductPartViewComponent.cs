using Allup.DataAccessLayer;
using Allup.ViewModels.ProductListVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents;
public class ProductPartViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    public ProductPartViewComponent(AppDbContext context) { _context = context; }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        ProductListVM productListVM = new ProductListVM
        {
            NewArrivals = await _context.Products.Where(p => !p.IsDeleted && p.IsNewArrival).ToListAsync(),
            BestSellers = await _context.Products.Where(p => !p.IsDeleted && p.IsBestSeller).ToListAsync(),
            Featured = await _context.Products.Where(p => !p.IsDeleted && p.IsFeatured).ToListAsync()
        };
        return View(productListVM);
    }
}