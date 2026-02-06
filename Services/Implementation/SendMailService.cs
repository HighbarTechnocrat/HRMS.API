using HRMS.API.DTOs;
using HRMS.API.Models;
using HRMS.API.Repository;
using HRMS.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
namespace HRMS.API.Services.Implementation

{
    public class SendMailService : ISendMail
    {
        private readonly MailSettings _emailSettings;
        public SendMailService(IOptions<MailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmail_User(EmailTemplateDto obj)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                {
                    client.UseDefaultCredentials = false;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.FromPassword);
                    client.EnableSsl = _emailSettings.EnableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.FromEmail, _emailSettings.DisplayName),
                        Subject = obj.Subject,
                        Body = obj.mailBody,
                        IsBodyHtml = true
                    };

                    if (!string.IsNullOrWhiteSpace(obj.ToEmail))
                    {
                        string[] strtoEmail = Convert.ToString(obj.ToEmail).Trim().Split(';');
                        for (int i = 0; i < strtoEmail.Length; i++)
                        {
                            if (Convert.ToString(strtoEmail[i]).Trim() != "")
                            {
                                mailMessage.To.Add(strtoEmail[i]);
                            }
                        }
                    }

                    await client.SendMailAsync(mailMessage);
                }
                return true;
            }
            catch (Exception ex)    
            {
                Console.WriteLine($"Email sending error: {ex.Message}");
                return false;
            }
        }
    }
}