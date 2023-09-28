using System.ComponentModel.DataAnnotations;

namespace Allup.Models;
public class Brand : BaseEntity
{
    [StringLength(50)]
    public string Name { get; set; }
    public IEnumerable<Product>? Products { get; set; }
}