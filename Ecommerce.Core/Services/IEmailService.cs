namespace Ecommerce.Core;

public interface IEmailService
{
    Task SendEmailAsync(string mailTo, string subject, string body);
}
