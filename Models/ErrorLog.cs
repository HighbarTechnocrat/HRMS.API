using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class ErrorLog
    {
       
        public string ControllerName { get; set; }=string.Empty;
 
        public string ActionName { get; set; } = string.Empty;

        
        public string ErrorMessage { get; set; } = string.Empty;

        public string StackTrace { get; set; } = string.Empty;

        public string RequestPath { get; set; } = string.Empty;

        public string RequestMethod { get; set; } = string.Empty;

        public string EmpCode { get; set; } = string.Empty;
                
        public string ErrorLevel { get; set; } = string.Empty; // Error, Warning, Information
    }
}
