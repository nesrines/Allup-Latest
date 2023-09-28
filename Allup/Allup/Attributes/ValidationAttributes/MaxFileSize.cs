using System.ComponentModel.DataAnnotations;

namespace Allup.Attributes.ValidationAttributes;
public class MaxFileSize : ValidationAttribute
{
    private readonly int _maxFileSize;
    public MaxFileSize(int maxFileSize) { _maxFileSize = maxFileSize; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        List<IFormFile> files = new List<IFormFile>();

        if (value is List<IFormFile>) files = (List<IFormFile>)value;
        else if (value is IFormFile) files.Add((IFormFile)value);
        foreach (IFormFile file in files)
        {
            if (file.Length > _maxFileSize * 1024)  return new ValidationResult("File size cannot be larger than" + _maxFileSize + "KB.");
        }
        return ValidationResult.Success;
    }
}