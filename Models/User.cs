using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class User
    { 
        public string? Emp_Code { get; set; }
        public string? Emp_Name { get; set; }
        public string? Emp_Emailaddress { get; set; }
        public string? Pwd { get; set; }
        public string? emp_status { get; set; }
        public string? UserGuid { get; set; }                
        public string? PMS_Pwd { get; set; }
        public string? ActualHrReleaseDate { get; set; }
    }
}
