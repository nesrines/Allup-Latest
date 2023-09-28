using Allup.DataAccessLayer;
using Allup.Models;
using Allup.ViewModels.BasketVMs;
using Allup.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Allup.Services.Implementations;
public class LayoutService : ILayoutService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _contextAccessor;
    public LayoutService(AppDbContext context, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }
    public async Task<List<BasketVM>> GetBasketAsync()
    {
        List<BasketVM> basketVMs = new List<BasketVM>();
        string? cookie = _contextAccessor.HttpContext.Request.Cookies["basket"];
        if (!string.IsNullOrWhiteSpace(cookie)) basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
        foreach (BasketVM basketVM in basketVMs)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
            basketVM.Title = product.Title;
            basketVM.Image = product.MainImage;
            basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
            basketVM.ExTax = product.ExTax;
        }
        return basketVMs;
    }
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Include(c => c.Children.Where(ch => !ch.IsDeleted))
            .Where(c => !c.IsDeleted && c.IsMain).ToListAsync();
    }
    public async Task<Dictionary<string, string>> GetSettingsAsync()
    {
        return await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
    }
}