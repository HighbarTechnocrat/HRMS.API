using HRMS.API.DTOs;
using HRMS.API.Models;

namespace HRMS.API.Repository.Interfaces
{
    public interface ILeaveRepository
    {
        Task<List<Leave>?> GetLeaveInbox(string empcode);
        Task<LeaveApproveResponse?> GetApproveLeave(int leaveReqId);
    }
}
