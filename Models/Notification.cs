namespace HRMS.API.Models
{
    public class Notification
    {
        public int Srno { get; set; }
        public string? ProcessName { get; set; }

       public int PendingCount { get; set; }

        public string? displayName { get; set; }
    }
}
