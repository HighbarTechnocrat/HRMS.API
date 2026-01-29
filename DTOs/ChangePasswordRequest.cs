namespace HRMS.API.DTOs
{
    public class ChangePasswordRequest
    {
        public string EmpCode { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public Guid? Guid { get; set; }
    }
}
