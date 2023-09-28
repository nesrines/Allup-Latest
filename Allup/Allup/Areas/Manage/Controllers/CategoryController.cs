using Allup.DataAccessLayer;
using Allup.Helpers;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Manage.Controllers;
[Area("manage")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    public CategoryController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env; 
    }

    public IActionResult Index(int currentPage = 1)
    {
        IQueryable<Category> categories = _context.Categories
            .Include(c => c.Products.Where(p => !p.IsDeleted))
            .Include(c => c.Children.Where(ch => !ch.IsDeleted))
            .Where(c => !c.IsDeleted && c.IsMain).OrderByDescending(c => c.Id);

        return View(PaginatedList<Category>.Create(categories, currentPage, 5, 5));
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.ParentCategories = await _context.Categories.Where(c => !c.IsDeleted && c.IsMain).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);

        if (await _context.Categories.AnyAsync(c => !c.IsDeleted && c.Name.ToLower() == category.Name.Trim().ToLower()))
        {
            ModelState.AddModelError("Name", $"{category.Name.Trim()} already exists in the Database.");
            return View(category);
        }
        else category.Name = category.Name.Trim();

        if (category.IsMain)
        {
            if (category.File == null)
            {
                ModelState.AddModelError("File", "Image file is required.");
                return View(category);
            }

            category.Image = await category.File.SaveAsync(_env.WebRootPath, "assets", "images" );
            category.ParentId = null;
        }
        else
        {
            if (category.ParentId == null)
            {
                ModelState.AddModelError("ParentId", "Parent Id id required.");
                return View(category);
            }
            if (!await _context.Categories.AnyAsync(c => !c.IsDeleted && c.IsMain && c.Id == category.ParentId))
            {
                ModelState.AddModelError("ParentId", "Parent Id is incorrect.");
                return View(category);
            }
            category.Image = null;
        }

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return BadRequest();
        Category category = await _context.Categories
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .Include(c => c.Children.Where(ch => !ch.IsDeleted)).ThenInclude(ch => ch.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (category == null) return NotFound();

        return View(category);
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        Category category = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);
        if (category == null) return NotFound();
        ViewBag.ParentCategories = await _context.Categories.Where(c => !c.IsDeleted && c.IsMain).ToListAsync();
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int? id, Category category)
    {
        ViewBag.ParentCategories = await _context.Categories.Where(c => !c.IsDeleted && c.IsMain).ToListAsync();

        if (id == null || id != category.Id) return BadRequest();

        if (!ModelState.IsValid) return View(category);

        Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

        if (dbCategory == null) return NotFound();

        if (await _context.Categories.AnyAsync(c => !c.IsDeleted && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != id))
        {
            ModelState.AddModelError("Name", $"{category.Name.Trim()} already exists in the Database");
            return View(category);
        }

        if(dbCategory.IsMain != category.IsMain)
        {
            ModelState.AddModelError("IsMain", "It cannot be changed.");
            category.IsMain = dbCategory.IsMain;
            return View(category);
        }

        if (category.IsMain && category.File != null)
        {
            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", category.Image);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            category.Image = await category.File.SaveAsync(_env.WebRootPath, "assets", "images");
            category.ParentId = null;
        }
        else
        {
            if (category.ParentId == null)
            {
                ModelState.AddModelError("ParentId", "Parent Id is required.");
                return View(category);
            }
            if (!await _context.Categories.AnyAsync(c => !c.IsDeleted && c.IsMain && c.Id == category.ParentId))
            {
                ModelState.AddModelError("ParentId", "Parent Id is incorrect.");
                return View(category);
            }
            category.Image = null;
        }

        dbCategory.Name = category.Name.Trim();
        dbCategory.Image = category.Image;
        dbCategory.ParentId = category.ParentId;
        dbCategory.UpdatedBy = "User";
        dbCategory.UpdatedDate = DateTime.UtcNow.AddHours(4);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();

        Category category = await _context.Categories
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .Include(c => c.Children.Where(ch => !ch.IsDeleted)).ThenInclude(ch => ch.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (category == null) return NotFound();
        ViewBag.CannotDelete = category.Children != null && category.Children.Count() > 0 ||
                                category.Products != null && category.Products.Count() > 0;
        return View(category);
    }

    public async Task<IActionResult> DeleteCategory(int? id)
    {
        if (id == null) return BadRequest();

        Category category = await _context.Categories
            .Include(b => b.Products.Where(p => !p.IsDeleted))
            .Include(c => c.Children.Where(ch => !ch.IsDeleted)).ThenInclude(ch => ch.Products.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

        if (category == null) return NotFound();

        if (category.Children != null && category.Children.Count() > 0) return BadRequest();
        if (category.Products != null && category.Products.Count() > 0) return BadRequest();

        category.IsDeleted = true;
        category.DeletedBy = "User";
        category.DeletedDate = DateTime.UtcNow.AddHours(4);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(category.Image))
        {
            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", category.Image);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        }

        return RedirectToAction(nameof(Index));
    }
}