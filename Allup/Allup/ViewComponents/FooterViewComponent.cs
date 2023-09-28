using Allup.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents;
public class FooterViewComponent : ViewComponent    
{
    public async Task<IViewComponentResult> InvokeAsync(Dictionary<string, string> settings) { return View(settings); }
}