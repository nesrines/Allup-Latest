using Microsoft.AspNetCore.Mvc;

namespace Allup.ViewComponents;
public class NewsletterViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() { return View(); }
}