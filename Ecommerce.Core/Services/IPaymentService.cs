namespace Ecommerce.Core;

public interface IPaymentService
{
    Task<Result<PaymobPaymentResponse>> CreatePaymobPaymentAsync(int orderId, string? buyerEmail = null);
}
