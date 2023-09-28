using System.ComponentModel.DataAnnotations;

namespace Allup.Models;
public class ProductImage : BaseEntity
{
    [StringLength(255)]
    public string Image { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}