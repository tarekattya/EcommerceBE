using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.Shared;
using Microsoft.Extensions.Options;

namespace Ecommerce.Application;

public class PaymobPaymentService(
    IHttpClientFactory httpClientFactory,
    IUnitOfWork unitOfWork,
    IOptions<PaymobSettings> options) : IPaymentService
{
    private readonly PaymobSettings _paymob = options.Value;

    public async Task<Result<PaymobPaymentResponse>> CreatePaymobPaymentAsync(int orderId, string? buyerEmail = null)
    {
        if (string.IsNullOrEmpty(_paymob.ApiKey))
            return Result<PaymobPaymentResponse>.Failure(new Error("PaymobNotConfigured", "Paymob is not configured.", 502));

        var order = await unitOfWork.Repository<Order>().GetByIdWithSpecAsync(new OrderSpecification(orderId, buyerEmail));
        if (order == null)
            return Result<PaymobPaymentResponse>.Failure(OrderErrors.NotFoundOrder);
        if (order.Status != OrderStatus.Pending)
            return Result<PaymobPaymentResponse>.Failure(new Error("InvalidOrder", "Order is not pending payment.", 400));
        if (order.IsCOD)
            return Result<PaymobPaymentResponse>.Failure(new Error("InvalidOrder", "Cash on delivery orders do not use online payment.", 400));

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_paymob.BaseUrl.TrimEnd('/') + "/");

        var authResponse = await client.PostAsJsonAsync("auth/tokens", new { api_key = _paymob.ApiKey });
        if (!authResponse.IsSuccessStatusCode)
            return Result<PaymobPaymentResponse>.Failure(new Error("PaymobError", "Failed to get Paymob auth token.", 502));
        var authJson = await authResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = authJson.GetProperty("token").GetString();
        if (string.IsNullOrEmpty(token))
            return Result<PaymobPaymentResponse>.Failure(new Error("PaymobError", "Invalid auth response from Paymob.", 502));

        var amountCents = (int)Math.Round(order.GetTotal() * 100);
        var orderPayload = new
        {
            auth_token = token,
            delivery_needed = "false",
            amount_cents = amountCents,
            currency = _paymob.Currency,
            merchant_order_id = orderId.ToString(),
            items = Array.Empty<object>()
        };
        var orderResponse = await client.PostAsJsonAsync("ecommerce/orders", orderPayload);
        if (!orderResponse.IsSuccessStatusCode)
            return Result<PaymobPaymentResponse>.Failure(new Error("PaymobError", "Failed to register order with Paymob.", 502));
        var orderJson = await orderResponse.Content.ReadFromJsonAsync<JsonElement>();
        var paymobOrderId = orderJson.GetProperty("id").GetInt64();

        var keyPayload = new
        {
            auth_token = token,
            amount_cents = amountCents,
            expiration = 3600,
            order_id = paymobOrderId,
            integration_id = _paymob.IntegrationId,
            currency = _paymob.Currency,
            lock_order_when_paid = "false"
        };
        var keyResponse = await client.PostAsJsonAsync("acceptance/payment_keys", keyPayload);
        if (!keyResponse.IsSuccessStatusCode)
            return Result<PaymobPaymentResponse>.Failure(new Error("PaymobError", "Failed to get payment key from Paymob.", 502));
        var keyJson = await keyResponse.Content.ReadFromJsonAsync<JsonElement>();
        var paymentToken = keyJson.GetProperty("token").GetString() ?? "";

        order.SetPaymobOrderId(paymobOrderId);
        unitOfWork.Repository<Order>().Update(order);
        await unitOfWork.CompleteAsync();

        var iframeUrl = $"{_paymob.BaseUrl.TrimEnd('/')}/acceptance/iframes/{_paymob.IframeId}?payment_token={paymentToken}";
        return Result<PaymobPaymentResponse>.Success(new PaymobPaymentResponse(iframeUrl, paymentToken));
    }
}
