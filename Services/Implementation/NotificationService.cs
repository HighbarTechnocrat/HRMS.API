using HRMS.API.DTOs;
using HRMS.API.Models;
using HRMS.API.Repository.Implementation;
using HRMS.API.Repository.Interfaces;
using HRMS.API.Services.Interfaces;

namespace HRMS.API.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger; 

        public NotificationService(
            INotificationRepository notificationRepository,
            ILogger<NotificationService> logger )
        {
            _notificationRepository = notificationRepository;
            _logger = logger; 
        }

        public async Task<ApiResponse<List<Notification>>> GetApproverNotificationsAsync(string empcode)
        {
            try
            { 
                
                var notifications = await _notificationRepository.GetApproverNotificationAsync(empcode);

                if (notifications == null || !notifications.Any())
                {
                    return new ApiResponse<List<Notification>>
                    {
                        Success = true,
                        Message = "No pending notifications",
                        Data = new List<Notification>()
                    };
                }

                return new ApiResponse<List<Notification>>
                {
                    Success = true,
                    Message = $"Found {notifications.Count} notifications",
                    Data = notifications
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approver notifications for empcode: {EmpCode}", empcode);
                return new ApiResponse<List<Notification>>
                {
                    Success = false,
                    Message = "An error occurred while fetching notifications",
                    Data = new List<Notification>()
                };
            }
        }

    }
}

 

 