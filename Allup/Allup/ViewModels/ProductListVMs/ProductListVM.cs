using Allup.Models;

namespace Allup.ViewModels.ProductListVMs;
public class ProductListVM
{
    public IEnumerable<Product> NewArrivals { get; set; }
    public IEnumerable<Product> BestSellers { get; set; }
    public IEnumerable<Product> Featured { get; set; }
}