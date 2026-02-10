using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ecommerce.Core;
using OrderEntity = Ecommerce.Core.Order;

namespace Ecommerce.API;

[ApiController]
[Route("api/webhooks/paymob")]
public class PaymobWebhookController(IUnitOfWork unitOfWork, IConfiguration configuration) : ControllerBase
{
    private readonly string _hmacSecret = configuration["Paymob:HmacSecret"] ?? "";

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> HandlePaymobCallback(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_hmacSecret))
            return BadRequest("Paymob not configured");

        var obj = Request.Form["obj"].FirstOrDefault();
        var hmac = Request.Form["hmac"].FirstOrDefault();
        if (string.IsNullOrEmpty(obj) || string.IsNullOrEmpty(hmac))
            return BadRequest("Missing obj or hmac");

        var computed = ComputeHmac(obj);
        if (!string.Equals(computed, hmac, StringComparison.OrdinalIgnoreCase))
            return BadRequest("Invalid HMAC");

        using var doc = JsonDocument.Parse(obj);
        var root = doc.RootElement;
        var success = root.TryGetProperty("success", out var successProp) && successProp.GetBoolean();
        if (!root.TryGetProperty("order", out var orderProp))
            return BadRequest("Missing order in payload");
        if (!orderProp.TryGetProperty("merchant_order_id", out var midProp))
            return BadRequest("Missing merchant_order_id");
        var merchantOrderIdStr = midProp.GetString();
        if (string.IsNullOrEmpty(merchantOrderIdStr) || !int.TryParse(merchantOrderIdStr, out var orderId))
            return BadRequest("Invalid merchant_order_id");

        var order = await unitOfWork.Repository<OrderEntity>().GetByIdWithSpecAsync(new OrderSpecification(orderId, null));
        if (order == null)
            return Ok();

        var newStatus = success ? OrderStatus.PaymentSucceeded : OrderStatus.PaymentFailed;
        order.UpdateStatus(newStatus);
        unitOfWork.Repository<OrderEntity>().Update(order);
        await unitOfWork.CompleteAsync();

        return Ok();
    }

    private string ComputeHmac(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        using var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
        var hash = hmacSha512.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
