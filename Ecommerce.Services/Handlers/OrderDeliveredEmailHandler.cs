namespace Ecommerce.Services;

public class OrderDeliveredEmailHandler(IEmailService emailService) : IDomainEventHandler<OrderDeliveredEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(OrderDeliveredEvent domainEvent)
    {
        var order = domainEvent.Order;
        if (string.IsNullOrWhiteSpace(order.BuyerEmail)) return;

        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "OrderDelivered.html");
        var emailBody = await File.ReadAllTextAsync(templatePath);

        var fullName = $"{order.ShipingAddress.FirstName} {order.ShipingAddress.LastName}".Trim();

        emailBody = emailBody
            .Replace("{{CustomerName}}", fullName)
            .Replace("{{OrderId}}", order.Id.ToString())
            .Replace("{{OrderDate}}", order.OrderDate.ToString("u"))
            .Replace("{{OrderStatus}}", order.Status.ToString())
            .Replace("{{OrderTotal}}", order.GetTotal().ToString("0.00"));

        await _emailService.SendEmailAsync(order.BuyerEmail, $"Order #{order.Id} delivered", emailBody);
    }
}

