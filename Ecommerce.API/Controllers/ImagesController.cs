namespace Ecommerce.API;

public class ImagesController(IFileService fileService) : ApiBaseController
{
    private readonly IFileService _fileService = fileService;

    [Authorize(Roles = "Admin")]
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file, [FromQuery] string folder = "products")
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only JPG, PNG, and WebP are allowed.");

        try
        {
            var result = await _fileService.UploadFileAsync(file, folder);
            return Ok(new { url = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete")]
    public IActionResult DeleteImage([FromQuery] string path)
    {
        if (string.IsNullOrEmpty(path))
            return BadRequest("Path is required");

        try
        {
            _fileService.DeleteFile(path, ""); 
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
