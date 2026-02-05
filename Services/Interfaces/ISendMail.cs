using HRMS.API.DTOs;

namespace HRMS.API.Services.Interfaces
{
    public interface ISendMail
    {
        Task<bool> SendEmail_User(EmailTemplateDto obj);
    }
}
