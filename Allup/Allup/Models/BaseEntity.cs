namespace Allup.Models;
public class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public string CreatedBy { get; set; } = "System";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow.AddHours(4);
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}