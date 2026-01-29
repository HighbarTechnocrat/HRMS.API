using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models
{
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string ControllerName { get; set; }

        [Required]
        [MaxLength(200)]
        public string ActionName { get; set; }

        [Required]
        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }

        public string RequestPath { get; set; }

        public string RequestMethod { get; set; }

        public string UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [MaxLength(50)]
        public string ErrorLevel { get; set; } // Error, Warning, Information
    }
}
