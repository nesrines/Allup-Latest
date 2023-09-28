using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Controllers;
public class BasketController : Controller
{
    private readonly AppDbContext _context;
    public BasketController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        List<BasketVM> basketVMs = new();
        string? basket = Request.Cookies["basket"];
        if (!string.IsNullOrWhiteSpace(basket))  basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

        foreach (BasketVM basketVM in basketVMs)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
            basketVM.Title = product.Title;
            basketVM.Image = product.MainImage;
            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            basketVM.ExTax = product.ExTax;
        }

        return View(basketVMs);
    }

    public async Task<IActionResult> AddBasket(int? id)
    {
        if (id == null) return BadRequest("Id cannot be null.");
        if (!await _context.Products.AnyAsync(p => !p.IsDeleted && p.Id == id)) return NotFound("Id is incorrect.");

        List<BasketVM> basketVMs = new();
        string? basket = Request.Cookies["basket"];

        if (!string.IsNullOrWhiteSpace(basket))
        {
            basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            if (basketVMs.Exists(b => b.Id == id))
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (basketVMs.Find(b => b.Id == id).Count < product.Count) basketVMs.Find(b => b.Id == id).Count += 1;
            }
            else basketVMs.Add(new BasketVM { Id = (int)id, Count = 1 });
        }
        else basketVMs.Add(new BasketVM { Id = (int)id, Count = 1 }); 
        
        Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));

        foreach (BasketVM basketVM in basketVMs)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
            basketVM.Title = product.Title;
            basketVM.Image = product.MainImage;
            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            basketVM.ExTax = product.ExTax;
        }

        return PartialView("_BasketPartial", basketVMs);
    }

    public async Task<IActionResult> RemoveBasket(int? id)
    {
        if (id == null) return BadRequest("Id cannot be null.");

        List<BasketVM> basketVMs = new();
        string? basket = Request.Cookies["basket"];

        if (!string.IsNullOrWhiteSpace(basket))
        {
            basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            if (!basketVMs.Remove(basketVMs.Find(b => b.Id == id))) return NotFound("Id is incorrect.");
        }
        else return BadRequest("Basket does not exist.");

        Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));

        foreach (BasketVM basketVM in basketVMs)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
            basketVM.Title = product.Title;
            basketVM.Image = product.MainImage;
            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            basketVM.ExTax = product.ExTax;
        }
        
        return PartialView("_BasketPartial", basketVMs);
    }

    public async Task<IActionResult> ChangeCount(int? id, int count)
    {
        if (id == null) return BadRequest("Id cannot be null.");

        List<BasketVM> basketVMs = new();
        string? basket = Request.Cookies["basket"];

        if (!string.IsNullOrWhiteSpace(basket))
        {
            basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            if (basketVMs.Exists(b => b.Id == id))
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                basketVMs.Find(b => b.Id == id).Count = (count > product.Count) ? product.Count : (count < 1 ? 1 : count);
            }
            else return NotFound("Id is incorrect.");
        }
        else return BadRequest("Basket does not exist.");

        Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));

        foreach (BasketVM basketVM in basketVMs)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
            basketVM.Title = product.Title;
            basketVM.Image = product.MainImage;
            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            basketVM.ExTax = product.ExTax;
        }
        
        return PartialView("_BasketPartial", basketVMs);
    }
    
    public async Task<IActionResult> UpdateCart()
    {
        List<BasketVM> basketVMs = new();
        string? basket = Request.Cookies["basket"];
        if (!string.IsNullOrWhiteSpace(basket)) basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

        foreach (BasketVM basketVM in basketVMs)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
            basketVM.Title = product.Title;
            basketVM.Image = product.MainImage;
            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            basketVM.ExTax = product.ExTax;
        }
        return PartialView("_BasketPagePartial", basketVMs);
    }
}