using System.ComponentModel.DataAnnotations;

namespace Allup.Attributes.ValidationAttributes;
public class FileTypes : ValidationAttribute
{
    private readonly string[] _fileTypes;
    public FileTypes(params string[] fileTypes) { _fileTypes = fileTypes; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        List<IFormFile> files = new List<IFormFile>();

        if (value is List<IFormFile>) files = (List<IFormFile>)value;
        else if (value is IFormFile) files.Add((IFormFile)value);
        foreach (IFormFile file in files)
        {
            if (!_fileTypes.Contains(file.ContentType)) return new ValidationResult("File type must be one of: " + string.Join(", ", _fileTypes));
        }
        return ValidationResult.Success;
    }
}