namespace HRMS.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendErrorNotificationAsync(string controllerName, string actionName,
                                     string errorMessage, string stackTrace = null);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
