using System.ComponentModel.DataAnnotations;

namespace Allup.Models;
public class Tag : BaseEntity
{
    [StringLength(50)]
    public string Name { get; set; }
}