using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ecommerce.Services;

public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string mailTo, string subject, string body)
    {
        // Validate configuration
        if (string.IsNullOrWhiteSpace(_mailSettings.Host))
        {
            _logger.LogWarning("Email service is not configured. MailSettings.Host is empty. Email will not be sent.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_mailSettings.Email))
        {
            _logger.LogWarning("Email service is not configured. MailSettings.Email is empty. Email will not be sent.");
            return;
        }

        try
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailTo));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            
            _logger.LogInformation("Email sent successfully to {Recipient}", mailTo);
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            _logger.LogError(ex, "Failed to connect to SMTP server {Host}:{Port}. Check your MailSettings configuration.", 
                _mailSettings.Host, _mailSettings.Port);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", mailTo);
            throw;
        }
    }
}
