using HRMS.API.Services.Interfaces;
using System.Net.Mail;

namespace HRMS.API.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendErrorNotificationAsync(string controllerName, string actionName,
                                                   string errorMessage, string stackTrace = null)
        {
            var adminEmail = _configuration["ErrorNotification:AdminEmail"];

            if (string.IsNullOrEmpty(adminEmail))
            {
                _logger.LogWarning("Admin email not configured for error notifications");
                return;
            }

            var subject = $"[Error] {controllerName}.{actionName} - {DateTime.UtcNow:yyyy-MM-dd HH:mm}";

            var body = $@"
            <h3>Application Error Notification</h3>
            <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
            <p><strong>Controller:</strong> {controllerName}</p>
            <p><strong>Action:</strong> {actionName}</p>
            <p><strong>Error Message:</strong> {errorMessage}</p>
            
            {(string.IsNullOrEmpty(stackTrace) ? "" : $@"
            <p><strong>Stack Trace:</strong></p>
            <pre>{stackTrace}</pre>")}
            
            <hr>
            <p>This is an automated error notification from HRMS API.</p>";

            await SendEmailAsync(adminEmail, subject, body);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort", 587);
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];
                var enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl", true);

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new System.Net.NetworkCredential(fromEmail, fromPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);

                    _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                throw;
            }
        }
    }
}
