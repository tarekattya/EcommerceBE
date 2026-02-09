using Ecommerce.Core;

namespace Ecommerce.Services;

/// <summary>Sends invoice email when a COD order is confirmed (status → Processing).</summary>
public class OrderProcessingStartedInvoiceHandler(IEmailService emailService) : IDomainEventHandler<OrderProcessingStartedEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(OrderProcessingStartedEvent domainEvent)
    {
        var order = domainEvent.Order;
        if (!order.IsCOD || string.IsNullOrWhiteSpace(order.BuyerEmail)) return;

        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "PaymentReceivedInvoice.html");
        if (!File.Exists(templatePath)) return;
        var emailBody = await File.ReadAllTextAsync(templatePath);

        var fullName = $"{order.ShipingAddress.FirstName} {order.ShipingAddress.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(fullName)) fullName = order.BuyerEmail;

        var shippingAddress = $"{order.ShipingAddress.FirstName} {order.ShipingAddress.LastName}, {order.ShipingAddress.Street}, {order.ShipingAddress.City}, {order.ShipingAddress.Country}";
        var deliveryCost = order.DeliveryMethod?.Cost ?? 0m;

        var itemsRows = new System.Text.StringBuilder();
        foreach (var item in order.Items)
        {
            var lineTotal = item.Price * item.Quantity;
            itemsRows.AppendLine($"<tr style=\"border-bottom:1px solid #e5e7eb;\"><td style=\"padding:8px 0;\">{System.Net.WebUtility.HtmlEncode(item.ProductItemOrderd?.Name ?? "Item")}</td><td style=\"padding:8px 0; text-align:right;\">{item.Quantity}</td><td style=\"padding:8px 0; text-align:right;\">{item.Price:N2}</td><td style=\"padding:8px 0; text-align:right;\">{lineTotal:N2}</td></tr>");
        }

        var discountRow = order.Discount > 0
            ? $"<tr><td style=\"padding:4px 0;\">Discount" + (string.IsNullOrEmpty(order.CouponCode) ? "" : $" ({order.CouponCode})") + "</td><td style=\"padding:4px 0; text-align:right;\">-" + order.Discount.ToString("N2") + "</td></tr>"
            : "";

        emailBody = emailBody
            .Replace("{{CustomerName}}", fullName)
            .Replace("{{InvoiceNumber}}", $"INV-{order.Id}")
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{OrderDate}}", order.OrderDate.ToString("u"))
            .Replace("{{ShippingAddress}}", System.Net.WebUtility.HtmlEncode(shippingAddress))
            .Replace("{{ItemsRows}}", itemsRows.ToString())
            .Replace("{{SubTotal}}", order.SubTotal.ToString("N2"))
            .Replace("{{DeliveryCost}}", deliveryCost.ToString("N2"))
            .Replace("{{DiscountRow}}", discountRow)
            .Replace("{{Total}}", order.GetTotal().ToString("N2"));

        await _emailService.SendEmailAsync(
            order.BuyerEmail,
            $"Invoice INV-{order.Id} – Order confirmed (Cash on Delivery)",
            emailBody);
    }
}
