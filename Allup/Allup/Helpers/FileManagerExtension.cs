namespace Allup.Helpers;
public static class FileManagerExtension
{
    public static async Task<string> SaveAsync(this IFormFile file, string rootPath, params string[] folders)
    {
        string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + file.FileName.Substring(file.FileName.LastIndexOf('.'));
        string folderPath = Path.Combine(folders);
        string filePath = Path.Combine(rootPath, folderPath, fileName);
        
        using (FileStream fileStream = new(filePath, FileMode.Create))
            await file.CopyToAsync(fileStream);
        
        return fileName;
    }
}