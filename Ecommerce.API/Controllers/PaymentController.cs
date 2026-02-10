using Ecommerce.Shared;

namespace Ecommerce.API;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentController(IPaymentService paymentService) : ApiBaseController
{
    private readonly IPaymentService _paymentService = paymentService;

    [HttpPost("paymob/create")]
    public async Task<IActionResult> CreatePaymobPayment([FromBody] PaymobPaymentRequest request, CancellationToken cancellationToken)
    {
        var email = User.IsInRole("Admin") ? null : UserEmail;
        if (email == null && !User.IsInRole("Admin")) return Unauthorized();
        var result = await _paymentService.CreatePaymobPaymentAsync(request.OrderId, email);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
