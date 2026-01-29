using Azure.Core;
using HRMS.API.DTOs;
using HRMS.API.Models;
using HRMS.API.Models.Response;
using HRMS.API.Repository.Interfaces;
using HRMS.API.Services.Interfaces;

namespace HRMS.API.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly ISecurityService _securityService;
        public UserService(IUserRepository userRepository, ILogger<UserService> logger, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _securityService = securityService;
        }

        public async Task<ApiResponse<User?>> ValidateUserLoginAsync(string email, string password)
        {
            try
            { 
                var user = await _userRepository.ValidateUserSaltAsync(email);

                if (user == null)
                {
                    return new ApiResponse<User?>
                    {
                        Success = false,
                        Message = "Invalid Login Credentials!",
                        Data = null
                    };
                }

                // Check if user is allowed to login based on ActualHrReleaseDate
                if (user.ActualHrReleaseDate == "true")
                {
                    return new ApiResponse<User?>
                    {
                        Success = false,
                        //Message = "Account is locked due to exit process",
                        Message="Invalid Login Credentials!",
                        Data = null
                    };
                }

                var haspassword = _securityService.HashSHA1(password + user.UserGuid);


                if (haspassword !=user.Pwd)
                {
                    return new ApiResponse<User?>
                    {
                        Success = false,
                        Message = "Invalid Login Credentials!",
                        Data = null
                    };
                }

                return new ApiResponse<User?>
                {
                    Success = true,
                    Message = "User validated successfully",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user login");
                return new ApiResponse<User?>
                {
                    Success = false,
                    Message = "An error occurred during login validation",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var result = await _userRepository.ChangePasswordAsync(request);

                var success = result.Contains("Successfully");

                return new ApiResponse<string>
                {
                    Success = success,
                    Message = result,
                    Data = success ? "Password changed successfully" : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while changing password",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<User?>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.ValidateUserSaltAsync(email);

                return new ApiResponse<User?>
                {
                    Success = user != null,
                    Message = user != null ? "User found" : "User not found",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email");
                return new ApiResponse<User?>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user",
                    Data = null
                };
            }
        }
    }
}
