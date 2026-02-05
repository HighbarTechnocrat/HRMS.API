namespace HRMS.API.DTOs
{
    public class EmailTemplateDto
    {
        public string ToEmail { get; set; }

        public string Subject { get; set; }
        public string mailBody { get; set; }
    }
}
