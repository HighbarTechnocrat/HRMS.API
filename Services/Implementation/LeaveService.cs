using Azure;
using HRMS.API.DTOs;
using HRMS.API.Models;
using HRMS.API.Repository.Implementation;
using HRMS.API.Repository.Interfaces;
using HRMS.API.Services.Interfaces;

namespace HRMS.API.Services.Implementation
{
    public class LeaveService : ILeaveServices
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILogger<LeaveService> _logger; 

        public LeaveService(
            ILeaveRepository leaveRepository,
            ILogger<LeaveService> logger )
        {
            _leaveRepository = leaveRepository;
            _logger = logger; 
        }

        public async Task<ApiResponse<List<Leave>>> GetLeaveInbox(string empcode)
        {
            try
            {

                var leaves = await _leaveRepository.GetLeaveInbox(empcode);

                if (leaves == null || !leaves.Any())
                {
                    return new ApiResponse<List<Leave>>
                    {
                        Success = true,
                        Message = "No pending leaves",
                        Data = new List<Leave>()
                    };
                }

                return new ApiResponse<List<Leave>>
                {
                    Success = true,
                    Message = $"Found {leaves.Count} leaves",
                    Data = leaves
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approver notifications for empcode: {EmpCode}", empcode);
                return new ApiResponse<List<Leave>>
                {
                    Success = false,
                    Message = "An error occurred while fetching notifications",
                    Data = new List<Leave>()
                };
            }
        }
        public async Task<ApiResponse<LeaveApproveResponse>> GetApproveLeave(int leaveReqId)
        {
            try
            {

                var leaves = await _leaveRepository.GetApproveLeave(leaveReqId);

                if (leaves == null)
                {
                    return new ApiResponse<LeaveApproveResponse>
                    {
                        Success = true,
                        Message = "Data not Found",
                        Data = new LeaveApproveResponse()
                    };
                }

                return new ApiResponse<LeaveApproveResponse>
                {
                    Success = true,
                    Message = $"Data Found Successfully",
                    Data = leaves
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approver notifications for empcode: {EmpCode}", leaveReqId);
                return new ApiResponse<LeaveApproveResponse>
                {
                    Success = false,
                    Message = "An error occurred while fetching notifications",
                    Data = new LeaveApproveResponse()
                };
            }
        }

    }
}

 

 