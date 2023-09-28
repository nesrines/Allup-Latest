using Allup.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup.Models;
public class Product : BaseEntity
{
    [StringLength(50)]
    public string Title { get; set; }
    [Column(TypeName = "smallmoney")]
    public double Price { get; set; }
    [Column(TypeName = "smallmoney")]
    public double DiscountedPrice { get; set; }
    [Column(TypeName = "smallmoney")]
    public double ExTax { get; set; }
    public int Count { get; set; }
    [StringLength(255)]
    public string? SmallDescription { get; set; }
    [StringLength(1500)]
    public string? Description { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsFeatured { get; set; }
    [StringLength(255)]
    public string? MainImage { get; set; }
    [StringLength(255)]
    public string? HoverImage { get; set; }
    [StringLength(5)]
    public string? Seria { get; set; }
    public int? Number { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    [Display(Name = "Brand")]
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public List<ProductImage>? ProductImages { get; set; }
    public List<ProductTag>? ProductTags { get; set; }

    [NotMapped]
    [Display(Name = "Main Image")]
    [MaxFileSize(100)]
    [FileTypes("image/jpeg", "image/png")]
    public IFormFile? MainFile { get; set; }
    [NotMapped]
    [Display(Name = "Second Image")]
    [MaxFileSize(100)]
    [FileTypes("image/jpeg", "image/png")]
    public IFormFile? HoverFile { get; set; }
    [NotMapped]
    [Display(Name = "Slider Images")]
    [MaxFileSize(100)]
    [FileTypes("image/jpeg", "image/png")]
    public IEnumerable<IFormFile>? Files { get; set; }
    [NotMapped]
    [Display(Name = "Tags")]
    public IEnumerable<int>? TagIds { get; set; }
}