using Azure.Core;
using Dapper;
using System.Data;
using HRMS.API.DTOs;
using HRMS.API.Models;
using HRMS.API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace HRMS.API.Repository.Implementation
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public LeaveRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("HRMSConnection");
        }

        public async Task<List<Leave>?> GetLeaveInbox(string empcode)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@empCode", empcode);
            parameters.Add("@qtype", "getAllRequest");

            using var multi = await connection.QueryMultipleAsync(
                "[dbo].[API_SP_GETLEAVEINBOX]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var leaves = (await multi.ReadAsync<Leave>()).ToList();

            return leaves;

        }

        public async Task<LeaveApproveResponse?> GetApproveLeave(int leaveReqId)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@Req_id", leaveReqId);
            parameters.Add("@stype", "LeaveRequest_Mng_Edit");

            using var multi = await connection.QueryMultipleAsync(
                "[dbo].[API_Usp_getDetails_leaves]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var leaveDetails = (await multi.ReadAsync<ApproveLeaveDetails>()).ToList();
            var leaveBalance = (await multi.ReadAsync<LeaveBalance>()).ToList(); 
            var approvers = (await multi.ReadAsync<LeaveApprover>()).ToList();

            return new LeaveApproveResponse
            {
                LeaveDetails = leaveDetails,
                LeaveBalances = leaveBalance,
                Approvers = approvers
            };

        }

    }
}
