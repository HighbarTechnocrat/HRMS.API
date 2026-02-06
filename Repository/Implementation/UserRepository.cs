using Dapper;
using HRMS.API.DTOs;
using HRMS.API.Models;
using HRMS.API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace HRMS.API.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("HRMSConnection");
        }

        public async Task<User?> ValidateUserAsync(ValidateUserRequest request)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@SearchString", request.SearchString);
            parameters.Add("@Pwd", request.Pwd);
            parameters.Add("@OldPwd", request.OldPwd);
            parameters.Add("@qtype", request.Qtype);
            parameters.Add("@Guid", request.Guid);
            parameters.Add("@SearchId", request.SearchId);

            using var multi = await connection.QueryMultipleAsync(
                "[dbo].[API_SP_Admin_Validate_User]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var result = await multi.ReadFirstOrDefaultAsync<dynamic>();

            if (result == null) return null;

            var user = new User
            { 
                Emp_Code = result.Emp_Code,
                Emp_Name = result.Emp_Name,
                Emp_Emailaddress = result.Emp_Emailaddress,
                Pwd = result.Pwd,
                emp_status = result.emp_status,
                // Handle GUID conversion
                UserGuid = result.UserGuid?.ToString() ??
                           result.userGuid?.ToString() ??
                           result.Guid?.ToString(),
                PMS_Pwd = result.PMS_Pwd,
                ActualHrReleaseDate = result.ActualHrReleaseDate?.ToString()
            };

            var logParams = new DynamicParameters();
            logParams.Add("@qtype", "CreateErrorLog");
            logParams.Add("@ControllerName", "AuthController");
            logParams.Add("@ActionName", "Login");
            logParams.Add("@ErrorMessage", null);
            logParams.Add("@StackTrace", null);
            logParams.Add("@RequestPath", "/api/v1/Auth/login");
            logParams.Add("@RequestMethod", "POST");
            logParams.Add("@EmpCode", user.Emp_Code);
            logParams.Add("@ErrorLevel", "Error");

            await connection.ExecuteAsync(
                    "[dbo].[API_SP_LogError]",
                    logParams,
                    commandType: CommandType.StoredProcedure
                );

            return user;
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var validateRequest = new ValidateUserRequest
            {
                SearchString = request.EmpCode,
                OldPwd = request.OldPassword,
                Pwd = request.NewPassword,
                Qtype = "ChangePassword",
                Guid = request.Guid
            };

            var result = await ValidateUserAsync(validateRequest);

            // For ChangePassword, we need to return the message from SP
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@SearchString", request.EmpCode);
            parameters.Add("@Pwd", request.NewPassword);
            parameters.Add("@OldPwd", request.OldPassword);
            parameters.Add("@qtype", "ChangePassword");
            parameters.Add("@Guid", request.Guid);
            parameters.Add("@SearchId", null);

            var multi = await connection.QueryMultipleAsync(
                "[dbo].[API_SP_Admin_Validate_User]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // First result is user (might be null)
            await multi.ReadFirstOrDefaultAsync<User>();

            // Second result is message
            var messageResult = await multi.ReadFirstOrDefaultAsync<dynamic>();

            return messageResult?.msg?.ToString() ?? "Password change failed";
        }

        public async Task<User?> ValidateUserSaltAsync(string email)
        {
            var request = new ValidateUserRequest
            {
                SearchString = email,
                Qtype = "Validate_Salt"
            };

            return await ValidateUserAsync(request);
        }

        public async Task<bool> ResetPasswordAsync(string email, string hashedPassword, Guid guid)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@SearchString", email);
            parameters.Add("@Pwd", hashedPassword);
            parameters.Add("@Guid", guid);
            parameters.Add("@qtype", "UpdatePassword");

            var isSuccess = await connection.ExecuteScalarAsync<int?>(
                "[dbo].[API_SP_Admin_Validate_User]",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return isSuccess == 1;
        }

        public async Task<ForgotPasswordEmailTemplate> GetForgotPasswordEmailDetails(string email)
        {
           
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@SearchString", email);
                parameters.Add("@qtype", "GetForgotPasswordEmail");
                using (var reader = await connection.QueryMultipleAsync(
                    "[dbo].[API_SP_Admin_Validate_User]",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    var employeeNameResult = await reader.ReadFirstOrDefaultAsync<dynamic>();
                    string employeeName = employeeNameResult?.Emp_Name ?? "User";

                    var emailTemplate = await reader.ReadFirstOrDefaultAsync<EmailTemplateDto>();

                    return new ForgotPasswordEmailTemplate
                    {
                        EmployeeName = employeeName,
                        EmailTemplate = emailTemplate
                    };
                }
        }

    }
}
