using Allup.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup.Models;
public class Category : BaseEntity
{
    [StringLength(255)]
    public string? Image { get; set; }
    [StringLength(50)]
    public string Name { get; set; }
    [Display(Name = "Is a Main Category")]
    public bool IsMain { get; set; }

    [Display(Name = "Parent Category")]
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }

    public IEnumerable<Category>? Children { get; set; }
    public IEnumerable<Product>? Products { get; set; }

    [NotMapped]
    [Display(Name = "Image")]
    [MaxFileSize(30)]
    [FileTypes("image/jpeg", "image/png")]
    public IFormFile? File { get; set; }
}