
namespace Ecommerce.Shared.Abstraction
{
    public class ApiValidationResponse : Error
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationResponse() : base("Not Valid", "This action not valid to execute", 400)
        {
            Errors = new List<string>();
        }
    }
}
