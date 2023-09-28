using Allup.Models;
using Allup.ViewModels.BasketVMs;

namespace Allup.Services.Interfaces;
public interface ILayoutService
{
    Task<List<BasketVM>> GetBasketAsync();

    Task<Dictionary<string, string>> GetSettingsAsync();
    Task<List<Category>> GetCategoriesAsync();
}