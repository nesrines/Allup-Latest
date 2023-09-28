using Microsoft.AspNetCore.Mvc;

namespace Allup.ViewComponents;
public class FeaturesViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() { return View(); }
}