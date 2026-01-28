using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application;

public class FileService(IWebHostEnvironment webHostEnvironment) : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        // 1. Get path to wwwroot/images/folderName
        var wwwrootPath = _webHostEnvironment.WebRootPath;
        var folderPath = Path.Combine(wwwrootPath, "images", folderName);

        // 2. Create directory if not exists
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // 3. Generate unique file name
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        // 4. Save file to disk
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        // 5. Return the accessible URL path
        return Path.Combine("images", folderName, fileName).Replace("\\", "/");
    }

    public void DeleteFile(string fileName, string folderName)
    {
        var wwwrootPath = _webHostEnvironment.WebRootPath;
        var filePath = Path.Combine(wwwrootPath, fileName); // fileName likely contains 'images/products/guid.jpg'

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
