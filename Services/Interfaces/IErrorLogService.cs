using HRMS.API.Models;

namespace HRMS.API.Services.Interfaces
{
    
        public interface IErrorLogService
        {
            Task LogErrorAsync(ErrorLog errorLog);
            Task LogErrorAsync(string controllerName, string actionName,
                              string errorMessage, string stackTrace = null,
                              string requestPath = null, string requestMethod = null,
                              string userId = null, string errorLevel = "Error");
        }
     
}
