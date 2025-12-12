
namespace Ecommerce.Shared;

public class Error
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    public int statusCode { get; set; }
    public Error(string code, string message, int statusCode)
    {
        Code = code;
        Message = message;
        this.statusCode = statusCode;
    }
}
