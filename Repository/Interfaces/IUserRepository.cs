using HRMS.API.DTOs;
using HRMS.API.Models;

namespace HRMS.API.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> ValidateUserAsync(ValidateUserRequest request);
        Task<string> ChangePasswordAsync(ChangePasswordRequest request);
        Task<User?> ValidateUserSaltAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string hashedPassword, Guid guid);
        public  Task<ForgotPasswordEmailTemplate> GetForgotPasswordEmailDetails(string email);

    }
}
