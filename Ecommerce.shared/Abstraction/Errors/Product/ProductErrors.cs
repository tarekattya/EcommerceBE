
namespace Ecommerce.Shared;
public static class ProductErrors
{
    public static Error NotFoundProduct => new Error("NotFoundProduct", "Product with this id not found", statusCode: 404);
    public static Error ProductNameAlreadyExists => new Error("ProductNameExists", "A product with this name already exists. Please choose a different name.", StatusCodes.Status409Conflict);
    public static Error InValidInputs => new Error("Invalid Inputs", "this inputs not match any product not found", statusCode: 404);










}
