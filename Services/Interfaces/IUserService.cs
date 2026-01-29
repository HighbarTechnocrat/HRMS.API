using HRMS.API.DTOs;
using HRMS.API.Models;

namespace HRMS.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<User?>> ValidateUserLoginAsync(string email, string password);
        Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ApiResponse<User?>> GetUserByEmailAsync(string email);
    }
}
