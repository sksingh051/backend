using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var emailSettings = _config.GetSection("EmailSettings");
                var host = emailSettings["Host"];
                var port = int.Parse(emailSettings["Port"] ?? "2525");
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];
                var fromAddress = emailSettings["FromAddress"] ?? "noreply@ecommerce.com";

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("E-Commerce System", fromAddress));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlMessage };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                // Connect to Mailtrap
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    await smtp.AuthenticateAsync(username, password);
                }

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                _logger.LogInformation("Email successfully sent to {ToEmail} regarding {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to send email to {ToEmail}", toEmail);
                // Depending on requirements, we might throw here or swallow it so the main transaction doesn't fail
            }
        }
    }
}
