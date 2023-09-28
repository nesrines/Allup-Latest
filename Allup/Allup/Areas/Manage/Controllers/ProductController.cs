using Allup.DataAccessLayer;
using Allup.Helpers;
using Allup.Models;
using Allup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Manage.Controllers;
[Area("manage")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private IWebHostEnvironment _env;
    public ProductController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index(int currentPage = 1)
    {
        IQueryable<Product> products = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductTags.Where(pt => !pt.IsDeleted)).ThenInclude(pt => pt.Tag)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(products => products.Id);

        return View(PaginatedList<Product>.Create(products, currentPage, 5, 5));
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
        ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
        ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
        ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
        ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

        Console.WriteLine(ModelState.IsValid);
        if (!ModelState.IsValid) return View(product);

        if (product.CategoryId == null || !await _context.Categories.AnyAsync(c => !c.IsDeleted && c.Id == product.CategoryId))
        {
            ModelState.AddModelError("CategoryId", "Category is incorrect.");
            return View(product);
        }

        if (product.BrandId != null && !await _context.Brands.AnyAsync(b => !b.IsDeleted && b.Id == product.BrandId))
        {
            ModelState.AddModelError("BrandId", "Brand is incorrect.");
            return View(product);
        }
        
        if (product.TagIds != null && product.TagIds.Count() > 0)
        {
            product.ProductTags = new();
            foreach (int tagId in product.TagIds)
            {
                if (!await _context.Tags.AnyAsync(t => !t.IsDeleted && t.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", $"Tag Id = {tagId} is incorrect.");
                    return View(product);
                }
                product.ProductTags.Add(new ProductTag { TagId = tagId });
            }
        }

        if (product.Files == null)
        {
            ModelState.AddModelError("Files", "There must be at least 1 file.");
            return View(product);
        }

        if (product.Files.Count() > 10)
        {
            ModelState.AddModelError("Files", "There must be 10 files maximum.");
            return View(product);
        }

        product.ProductImages = new();
        foreach (IFormFile file in product.Files)
        {
            product.ProductImages.Add(new ProductImage { Image = await file.SaveAsync(_env.WebRootPath, "assets", "images", "product") });
        }

        if (product.MainFile == null)
        {
            ModelState.AddModelError("MainFile", "Required.");
            return View(product);
        }
        else product.MainImage = await product.MainFile.SaveAsync(_env.WebRootPath, "assets", "images", "product");
        
        if (product.HoverFile == null)
        {
            ModelState.AddModelError("HoverFile", "Required.");
            return View(product);
        }
        else product.HoverImage = await product.HoverFile.SaveAsync(_env.WebRootPath, "assets", "images", "product");

        if (product.Price <= 0)
        {
            ModelState.AddModelError("Price", "Price must be more than $0.");
            return View(product);
        }

        if (product.DiscountedPrice < 0 || product.DiscountedPrice > product.Price)
        {
            ModelState.AddModelError("DiscountedPrice", "Discounted Price cannot be more than old price or less than $0.");
            return View(product);
        }

        if (product.ExTax < 0 || product.ExTax > product.Price * 0.5)
        {
            ModelState.AddModelError("ExTax", "ExTax cannot be more than 50% of price or less than $0.");
            return View(product);
        }
        
        if (product.Count < 0)
        {
            ModelState.AddModelError("Count", "Count cannot be less than 0.");
            return View(product);
        }
        Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
        Brand? brand = product.BrandId != null ? null : await _context.Brands.FirstOrDefaultAsync(b => b.Id == product.BrandId);
        product.Seria = (category.Name.Substring(0, 2) + (brand != null ? brand.Name.Substring(0, 2) : "xx")).ToLower();

        Product? prod = await _context.Products
            .Where(p => p.Seria == product.Seria)
            .OrderByDescending(p => p.Number).FirstOrDefaultAsync();

        product.Number = prod != null ? prod.Number + 1 : 1;

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return BadRequest();

        Product? product = await _context.Products
            .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
            .Include(p => p.ProductTags.Where(pt => !pt.IsDeleted))
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();

        Product? product = await _context.Products
            .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

        if (product == null) return NotFound();
        
        product.TagIds = await _context.ProductTags
            .Where(pt => !pt.IsDeleted && pt.ProductId == product.Id)
            .Select(x => x.TagId).ToListAsync();

        ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
        ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
        ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
        
        return View(product);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(int? id, Product product)
    {
        ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
        ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
        ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

        if (id == null || id != product.Id) return BadRequest();

        Product? dbProduct = await _context.Products
            .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
            .Include(p => p.ProductTags.Where(pt => !pt.IsDeleted))
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);
 
        if (dbProduct == null) return NotFound();

        product.MainImage = dbProduct.MainImage;
        product.HoverImage = dbProduct.HoverImage;
        product.ProductImages = dbProduct.ProductImages;

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        if (product.Files != null)
        {
            int canUpload = 10 - dbProduct.ProductImages.Count();
            if (product.Files.Count() > canUpload)
            {
                ModelState.AddModelError("Files", $"You can only upload {canUpload} more files.");
                return View(product);
            }
            foreach (IFormFile file in product.Files)
            {
                dbProduct.ProductImages.Add(new ProductImage { Image = await file.SaveAsync(_env.WebRootPath, "assets", "images", "product") });
            }
        }

        if (product.MainFile != null)
        {
            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", dbProduct.MainImage);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            dbProduct.MainImage = await product.MainFile.SaveAsync(_env.WebRootPath, "assets", "images", "product");
        }

        if (product.HoverFile != null)
        {
            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", dbProduct.HoverImage);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            dbProduct.HoverImage = await product.HoverFile.SaveAsync(_env.WebRootPath, "assets", "images", "product");
        }

        if (product.CategoryId == null || !await _context.Categories.AnyAsync(c => !c.IsDeleted && c.Id == product.CategoryId))
        {
            ModelState.AddModelError("CategoryId", "Invalid");
            return View(product);
        }

        if (product.BrandId != null && !await _context.Brands.AnyAsync(b => !b.IsDeleted && b.Id == product.BrandId))
        {
            ModelState.AddModelError("BrandId", "Invalid");
            return View(product);
        }

        if (dbProduct.ProductTags != null && dbProduct.ProductTags.Count() > 0)
        {
            foreach (ProductTag productTag in dbProduct.ProductTags)
            {
                productTag.IsDeleted = true;
                productTag.DeletedBy = "User";
                productTag.DeletedDate = DateTime.UtcNow.AddHours(4);
            }
        }

        if (product.TagIds != null && product.TagIds.Count() > 0)
        {
            product.ProductTags = new();
            foreach (int tagId in product.TagIds)
            {
                if (!await _context.Tags.AnyAsync(t => !t.IsDeleted && t.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", $"Tag Id = {tagId} is incorrect.");
                    return View(product);
                }
                product.ProductTags.Add(new ProductTag { TagId = tagId });
            }
        }

        if (product.Price <= 0)
        {
            ModelState.AddModelError("Price", "Price must be more than $0.");
            return View(product);
        }

        if (product.DiscountedPrice < 0 || product.DiscountedPrice > product.Price)
        {
            ModelState.AddModelError("DiscountedPrice", "Discounted Price cannot be more than old price or less than $0.");
            return View(product);
        }

        if (product.ExTax < 0 || product.ExTax > product.Price * 0.5)
        {
            ModelState.AddModelError("ExTax", "ExTax cannot be more than 50% of price or less than $0.");
            return View(product);
        }

        if (product.Count < 0)
        {
            ModelState.AddModelError("Count", "Count cannot be less than 0.");
            return View(product);
        }

        dbProduct.Title = product.Title;
        dbProduct.Price = product.Price;
        dbProduct.DiscountedPrice = product.DiscountedPrice;
        dbProduct.ExTax = product.ExTax;
        dbProduct.Count = product.Count;
        dbProduct.SmallDescription = product.SmallDescription;
        dbProduct.Description = product.Description;
        dbProduct.IsBestSeller = product.IsBestSeller;
        dbProduct.IsNewArrival = product.IsNewArrival;
        dbProduct.IsFeatured = product.IsFeatured;
        dbProduct.CategoryId = product.CategoryId;
        dbProduct.BrandId = product.BrandId;
        dbProduct.ProductTags = product.ProductTags;
        dbProduct.UpdatedBy = "User";
        dbProduct.UpdatedDate = DateTime.UtcNow.AddHours(4);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteImage(int? id, int? imageId)
    {
        if (id == null || imageId == null) return BadRequest();

        Product? product = await _context.Products
            .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

        if (product == null) return NotFound();

        ProductImage? image = product.ProductImages.FirstOrDefault(pi => pi.Id == imageId);
        if (image == null) return NotFound();

        if (product.ProductImages.Count() == 1) return Conflict();

        image.IsDeleted = true;
        image.DeletedBy = "User";
        image.DeletedDate = DateTime.UtcNow.AddHours(4);

        await _context.SaveChangesAsync();

        string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", image.Image);

        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

        return PartialView("_ProductImagesPartial", product.ProductImages.Where(pi => !pi.IsDeleted));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();

        Product? product = await _context.Products
            .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
            .Include(p => p.ProductTags.Where(pt => !pt.IsDeleted))
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }

    public async Task<IActionResult> DeleteProduct(int? id)
    {
        if (id == null) return BadRequest();

        Product? product = await _context.Products
            .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
            .Include(p => p.ProductTags.Where(pt => !pt.IsDeleted))
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

        if (product == null) return NotFound();

        product.IsDeleted = true;

        string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", product.MainImage);
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        
        filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", product.HoverImage);
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

        if (product.ProductImages != null && product.ProductImages.Count() > 0)
        {
            foreach (ProductImage productImage in product.ProductImages)
            {
                productImage.IsDeleted = true;
                filePath = Path.Combine(_env.WebRootPath, "assets", "images", "product", productImage.Image);
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }
        }

        if (product.ProductTags != null && product.ProductTags.Count() > 0)
        {
            foreach (ProductTag productTag in product.ProductTags) productTag.IsDeleted = true;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}