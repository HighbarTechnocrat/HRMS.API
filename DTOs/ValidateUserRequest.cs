namespace HRMS.API.DTOs
{
    public class ValidateUserRequest
    {
        public string? SearchString { get; set; }
        public string? Pwd { get; set; }
        public string? OldPwd { get; set; }
        public string? Qtype { get; set; }
        public Guid? Guid { get; set; }
        public long? SearchId { get; set; }
    }
}
