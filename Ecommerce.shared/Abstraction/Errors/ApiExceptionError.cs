namespace Ecommerce.Shared;
public class ApiExceptionError : Error
{
    public string ErrorId { get; }

    public ApiExceptionError(string message, string details, int statusCode, string errorId) : base(message, details, statusCode)
    {
        ErrorId = errorId;
    }
}
