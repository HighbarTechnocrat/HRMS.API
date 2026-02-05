using HRMS.API.DTOs;
using HRMS.API.Models;

namespace HRMS.API.Services.Interfaces
{
    public interface ILeaveServices
    {
        Task<ApiResponse<List<Leave>>> GetLeaveInbox(string empcode);
        Task<ApiResponse<LeaveApproveResponse>> GetApproveLeave(int leaveReqId);
    }
}
