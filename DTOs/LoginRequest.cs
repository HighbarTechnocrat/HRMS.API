using System.ComponentModel.DataAnnotations;

namespace HRMS.API.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password
        {
            get; set;
        }
    }
}
