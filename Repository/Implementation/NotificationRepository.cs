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
    public class NotificationRepository : INotificationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public NotificationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("HRMSConnection");
        }
        public async Task<List<Notification>?> GetApproverNotificationAsync(string empcode)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@empCode", empcode); 

            using var multi = await connection.QueryMultipleAsync(
                "[dbo].[API_SP_Notification]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var notifications = (await multi.ReadAsync<Notification>()).ToList();

            return notifications;

        }
    }
}
