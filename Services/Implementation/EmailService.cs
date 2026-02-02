using HRMS.API.Services.Interfaces;
using System.Net.Mail;
using System.Text;

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

            await SendEmailErrorLogAsync(adminEmail, subject, body);
        }
              

        public async Task SendEmailAsync(string toMailIDs, string subject, string strbody,string ccMailIDs)
        {
            try
            {

                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort", 587);
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];
                var enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl", true);

                string stoemailid = "";
                string sccemailid = "";

                MailMessage mail = new MailMessage();
                HashSet<string> uniqueEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                string[] strtoEmail = Convert.ToString(toMailIDs).Trim().Split(';');
                foreach (string email in strtoEmail)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        if (Convert.ToString(email).Trim() != "")
                        {
                            uniqueEmails.Add(email.Trim());
                        }
                    }
                }

                string[] strCCEmail = Convert.ToString(ccMailIDs).Trim().Split(';');
                foreach (string email in strCCEmail)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        if (Convert.ToString(email).Trim() != "")
                            uniqueEmails.Add(email.Trim());
                    }
                }

                foreach (string email in strtoEmail)
                {
                    if (uniqueEmails.Contains(email))
                    {
                        // mail.To.Add(email);
                        uniqueEmails.Remove(email); // Remove it from uniqueEmails after adding to To
                        stoemailid = stoemailid + email + ";";
                    }
                }

                foreach (string email in strCCEmail)
                {
                    if (uniqueEmails.Contains(email))
                    {
                        //  mail.CC.Add(email); 
                        sccemailid = sccemailid + email + ";";
                    }
                }

                mail.To.Add("sanjay.patil@highbartech.com");

                StringBuilder strsignature = new StringBuilder();

                mail.From = new MailAddress(fromEmail, "OneHRAPI");

                mail.Subject = subject;
                //mail.Body = Convert.ToString(strbody) + Convert.ToString(strsignature);
                mail.Body = "toMailIDs =" + stoemailid + "<br> ccMailIDs =" + sccemailid + "<br><br>" + Convert.ToString(strbody) + Convert.ToString(strsignature);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
               

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.office365.com"; //Highbar SMTP

                    smtp.Port = 587;
                    smtp.TargetName = "STARTTLS/smtp.office365.com";
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(fromEmail, fromPassword);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = SMTPUserInfo;
                    smtp.EnableSsl = true;

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                    smtp.Send(mail);
                }
                mail.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendEmailErrorLogAsync(string toMailIDs, string subject, string strbody, string ccMailIDs = "")
        {

            try
            {

                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort", 587);
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];
                var enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl", true);

                string stoemailid = "";
                string sccemailid = "";

                MailMessage mail = new MailMessage();
                HashSet<string> uniqueEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                string[] strtoEmail = Convert.ToString(toMailIDs).Trim().Split(';');
                foreach (string email in strtoEmail)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        if (Convert.ToString(email).Trim() != "")
                        {
                            uniqueEmails.Add(email.Trim());
                        }
                    }
                }

                string[] strCCEmail = Convert.ToString(ccMailIDs).Trim().Split(';');
                foreach (string email in strCCEmail)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        if (Convert.ToString(email).Trim() != "")
                            uniqueEmails.Add(email.Trim());
                    }
                }

                foreach (string email in strtoEmail)
                {
                    if (uniqueEmails.Contains(email))
                    {
                        // mail.To.Add(email);
                        uniqueEmails.Remove(email); // Remove it from uniqueEmails after adding to To
                        stoemailid = stoemailid + email + ";";
                    }
                }

                foreach (string email in strCCEmail)
                {
                    if (uniqueEmails.Contains(email))
                    {
                        //  mail.CC.Add(email); 
                        sccemailid = sccemailid + email + ";";
                    }
                }

                mail.To.Add("sanjay.patil@highbartech.com");
                StringBuilder strsignature = new StringBuilder();
                mail.From = new MailAddress(fromEmail, "OneHRAPI");

                mail.Subject = subject;
                //mail.Body = Convert.ToString(strbody) + Convert.ToString(strsignature);
                mail.Body = "toMailIDs =" + stoemailid + "<br> ccMailIDs =" + sccemailid + "<br><br>" + Convert.ToString(strbody) + Convert.ToString(strsignature);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;


                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.office365.com";  
                    smtp.Port = 587;
                    smtp.TargetName = "STARTTLS/smtp.office365.com";
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(fromEmail, fromPassword);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = SMTPUserInfo;
                    smtp.EnableSsl = enableSsl;
                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;                    
                    await smtp.SendMailAsync(mail);
                }
                mail.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }
               

        Task IEmailService.SendEmailAsync(string toEmail, string subject, string body)
        {
            throw new NotImplementedException();
        }

        Task IEmailService.SendEmailErrorLogAsync(string toEmail, string subject, string body, string ccMailIDs)
        {
            throw new NotImplementedException();
        }
    }
}
