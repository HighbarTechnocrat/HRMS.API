using Azure.Core;
using HRMS.API.DTOs;
using HRMS.API.Helper;
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
        private readonly ISendMail _emailSend;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, ISendMail emailSend,ISecurityService securityService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _emailSend = emailSend;
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
                if (user.ActualHrReleaseDate == "false")
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

        private async Task<bool> SendForgotPasswordEmail(ForgotPasswordEmailTemplate emailDetails, string toEmail, string tempPassword)
        {
            try
            {
                var sendEmailCredentials = new EmailTemplateDto();
                var emailTemplate = emailDetails.EmailTemplate;

                string emailBody = string.Empty;
                string emailSubject = string.Empty;

                if (emailTemplate != null && !string.IsNullOrEmpty(emailTemplate.mailBody))
                {
                    emailBody = emailTemplate.mailBody;
                    emailSubject = emailTemplate.Subject ?? string.Empty;
                }
                else
                {
                    _logger.LogWarning("Email template is missing or empty for forgot password email.");
                    return false;
                }

                emailBody = emailBody
                    .Replace("{employeeName}", emailDetails.EmployeeName)
                    .Replace("{tempPassword}", tempPassword);

                emailSubject = emailSubject
                    .Replace("{employeeName}", emailDetails.EmployeeName);

                sendEmailCredentials.ToEmail = toEmail;
                sendEmailCredentials.Subject = emailSubject;
                sendEmailCredentials.mailBody = emailBody;

                return await _emailSend.SendEmail_User(sendEmailCredentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send forgot password email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<ApiResponse<object>> ForgetPasswordAsync(string email)
        {
            try
            {
                var userResult = await GetUserByEmailAsync(email);

                var emailDetails = await _userRepository.GetForgotPasswordEmailDetails(email);


                if (!userResult.Success || userResult.Data == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email not found"
                    };
                }

                var newGuid = Guid.NewGuid();
                var plainPassword = PasswordHelper.RandomString(8);
                var hashedPassword = PasswordHelper.HashSHA1(plainPassword + newGuid);

                var resetResult = await _userRepository.ResetPasswordAsync( email, hashedPassword,newGuid);

                if (!resetResult)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Password reset failed"
                    };
                }

                var emailSent = await SendForgotPasswordEmail(emailDetails, email, plainPassword);


                return new ApiResponse<object>
                {
                    Success = true,
                    Message = emailSent ? "Password reset successful" : "Password reset successful but email could not be sent.",
                   Data = new { EmailSent = emailSent }

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password error");

                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Something went wrong"
                };
            }
        }

    }
}
