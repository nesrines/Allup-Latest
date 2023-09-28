using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Manage.Controllers;
[Area("manage")]
public class BrandController : Controller
{
    private readonly AppDbContext _context;
    public BrandController(AppDbContext context) { _context = context; }

    public IActionResult Index(int currentPage = 1)
    {
        IQueryable<Brand> brands = _context.Brands
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .Where(b => !b.IsDeleted).OrderByDescending(b => b.Id);

        return View(PaginatedList<Brand>.Create(brands, currentPage, 10, 5));
    }
    
    public IActionResult Create() { return View(); }

    [HttpPost]
    public async Task<IActionResult> Create(Brand brand)
    {
        if (!ModelState.IsValid) return View(brand);

        if (await _context.Brands.AnyAsync(b => !b.IsDeleted && b.Name.ToLower() == brand.Name.Trim().ToLower()))
        {
            ModelState.AddModelError("Name", $"{brand.Name.Trim()} already exists in the Database");
            return View(brand);
        }

        brand.Name = brand.Name.Trim();

        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return BadRequest();

        Brand brand = await _context.Brands
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (brand == null) return NotFound();

        return View(brand);
    }
    
    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        Brand brand = await _context.Brands.FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);
        if (brand == null) return NotFound();
        return View(brand);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, Brand brand)
    {
        if (id == null || id != brand.Id) return BadRequest();

        if (!ModelState.IsValid) return View(brand);

        Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (dbBrand == null) return NotFound();

        if (await _context.Brands.AnyAsync(b => !b.IsDeleted && b.Name.ToLower() == brand.Name.Trim().ToLower() && b.Id != id))
        {
            ModelState.AddModelError("Name", $"{brand.Name.Trim()} already exists in the Database");
            return View(brand);
        }

        dbBrand.Name = brand.Name.Trim();
        dbBrand.UpdatedBy = "User";
        dbBrand.UpdatedDate = DateTime.UtcNow.AddHours(4);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();

        Brand brand = await _context.Brands
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (brand == null) return NotFound();
        return View(brand);
    }

    public async Task<IActionResult> DeleteBrand(int? id)
    {
        if (id == null) return BadRequest();

        Brand brand = await _context.Brands
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (brand == null) return NotFound();

        brand.IsDeleted = true;
        brand.DeletedBy = "User";
        brand.DeletedDate = DateTime.UtcNow.AddHours(4);

        if (brand.Products != null && brand.Products.Count() > 0)
        {
            foreach(Product product in brand.Products)
            {
                product.BrandId = null;
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}