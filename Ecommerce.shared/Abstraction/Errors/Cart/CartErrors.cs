namespace Ecommerce.Shared;

public static class CartErrors
{
    public static Error NotFoundCart = new Error("NotFoundCart", "Not Found Cart Cart With this key", StatusCodes.Status404NotFound);
    public static Error CantCreateCart = new Error("CantCreateCart", "Cant Create this cart now , try later ", StatusCodes.Status405MethodNotAllowed);
    public static Error CantUpdateCart = new Error("CantUpdateCart", "Cant Update this cart now, try later ", StatusCodes.Status405MethodNotAllowed);
    public static Error EmptyCart = new Error("EmaptyCart", "The Cart Is Empty Not Have Products , try later ", StatusCodes.Status404NotFound);
    public static Error InvalidCartId = new Error("InvalidCartId", "Invalid Cart Id  ", StatusCodes.Status406NotAcceptable);
    public static Error InvalidItemsIds = new Error("InvalidItemsIds", "Invalid Items Ids", StatusCodes.Status406NotAcceptable);
}
