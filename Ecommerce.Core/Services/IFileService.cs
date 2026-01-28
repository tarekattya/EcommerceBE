using Microsoft.AspNetCore.Http;

namespace Ecommerce.Core;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    void DeleteFile(string fileName, string folderName);
}
