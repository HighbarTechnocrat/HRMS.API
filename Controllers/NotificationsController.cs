using AutoMapper;
using HRMS.API.DTOs;
using HRMS.API.Models.Response;
using HRMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper; 
        private readonly INotificationService _notificationService;

        public NotificationsController(ILogger<AuthController> logger, IMapper mapper,INotificationService notificationService)
        {
            _logger = logger;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationList(string empcode)
        {
            if (string.IsNullOrEmpty(empcode))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "employee code are required"
                });
            }
            var result = await _notificationService.GetApproverNotificationsAsync(empcode);


            if (!result.Success)
            {
                _logger.LogWarning("Failed to attempt for Pending Count : {empcode}", empcode);
                return Unauthorized(result);
            }



            //var userResponse = _mapper.Map<UserResponse>(result.Data);
            return Ok(new
            {
                Success = true,
                Message = "Pending Count",
                PendingCount = result.Data

            });
        }
    }
}
