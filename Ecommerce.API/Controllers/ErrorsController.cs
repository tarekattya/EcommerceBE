namespace Ecommerce.API;
[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
public class ErrorsController : ApiBaseController
{
    public ActionResult Error(int code)
    {
        if (code == 401)
            return Unauthorized(new Error("Unauthorized" , "Unauthorized" , code ));
        else if (code == 404)
            return NotFound(new Error("NotFound", "NotFound", code));
        return StatusCode(code);
    }
}
