using HRMS.API.DTOs;
using HRMS.API.Models;

namespace HRMS.API.Repository.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>?> GetApproverNotificationAsync(string  empcode);
    }
}
