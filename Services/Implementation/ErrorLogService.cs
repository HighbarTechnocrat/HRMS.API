using HRMS.API.Data;
using HRMS.API.Models;
using HRMS.API.Services.Interfaces;

namespace HRMS.API.Services.Implementation
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly ApplicationDbContext _context;

        public ErrorLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogErrorAsync(ErrorLog errorLog)
        {
            try
            {
                errorLog.CreatedAt = DateTime.UtcNow;
                _context.ErrorLogs.Add(errorLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Fallback to file logging if database fails
                Console.WriteLine($"Failed to log error to database: {ex.Message}");
                // You can also log to file or other logging providers here
            }
        }

        public async Task LogErrorAsync(string controllerName, string actionName,
                                       string errorMessage, string stackTrace = null,
                                       string requestPath = null, string requestMethod = null,
                                       string userId = null, string errorLevel = "Error")
        {
            var errorLog = new ErrorLog
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ErrorMessage = errorMessage,
                StackTrace = stackTrace,
                RequestPath = requestPath,
                RequestMethod = requestMethod,
                UserId = userId,
                ErrorLevel = errorLevel
            };

            await LogErrorAsync(errorLog);
        }
    }
}
