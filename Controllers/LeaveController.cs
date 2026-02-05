using AutoMapper;
using HRMS.API.DTOs;
using HRMS.API.Models.Response;
using HRMS.API.Services.Implementation;
using HRMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly ILeaveServices _leaveServices;
        public LeaveController(ILogger<AuthController> logger, IMapper mapper, ILeaveServices leaveServices)
        {
            _logger = logger;
            _mapper = mapper;
            _leaveServices = leaveServices;
        }


        [HttpGet("inbox-leave")]
        public async Task<IActionResult> GetLeaveInbox(string empcode)
        {
            if (string.IsNullOrEmpty(empcode))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "employee code are required"
                });
            }
            var result = await _leaveServices.GetLeaveInbox(empcode);


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

        [HttpGet("approve-leave")]
        public async Task<IActionResult> GetApproveLeave(int leaveReqId)
        {
            if (leaveReqId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Request Id are required"
                });
            }
            var result = await _leaveServices.GetApproveLeave(leaveReqId);


            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
