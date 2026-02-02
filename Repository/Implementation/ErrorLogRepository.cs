using System.Data;
using System.Data.SqlClient;
using Dapper;
using HRMS.API.Models;
using HRMS.API.Repository.Interfaces;


namespace HRMS.API.Repository.Implementation
{
    public class ErrorLogRepository : IErrorlogRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public ErrorLogRepository(IConfiguration configuration) 
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("HRMSConnection");
        }

       public async Task CreateLogErrorAsync(ErrorLog errorLog)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@qtype", "CreateErrorLog");
                parameters.Add("@ControllerName", errorLog.ControllerName);
                parameters.Add("@ActionName", errorLog.ActionName);
                parameters.Add("@ErrorMessage", errorLog.ErrorMessage);
                parameters.Add("@StackTrace", errorLog.StackTrace);
                parameters.Add("@RequestPath", errorLog.RequestPath);
                parameters.Add("@RequestMethod", errorLog.RequestMethod);
                parameters.Add("@EmpCode", errorLog.EmpCode);
                parameters.Add("@ErrorLevel", errorLog.ErrorLevel);

                await connection.ExecuteAsync(
                    "[dbo].[API_SP_LogError]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }

        }
    }
}
