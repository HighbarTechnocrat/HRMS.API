using HRMS.API.Data;
using HRMS.API.Models;
using HRMS.API.Repository.Interfaces;
using HRMS.API.Services.Interfaces;

namespace HRMS.API.Services.Implementation
{
    public class ErrorLogService : IErrorLogService
    { 
        private readonly IErrorlogRepository _errorlogRepository;

        public ErrorLogService(IErrorlogRepository errorlogRepository)
        {      
            _errorlogRepository = errorlogRepository;
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
                EmpCode = userId,
                ErrorLevel = errorLevel
            };

            //errorLog.CreatedAt = DateTime.UtcNow;            
            // await LogErrorAsync(errorLog);
            await _errorlogRepository.CreateLogErrorAsync(errorLog);
        }
    }
}
