using HRMS.API.DTOs;
using HRMS.API.Models;

namespace HRMS.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponse<List<Notification>>> GetApproverNotificationsAsync(string empcode);
    }
}

