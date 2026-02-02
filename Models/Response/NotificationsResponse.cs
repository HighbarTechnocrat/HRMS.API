namespace HRMS.API.Models.Response
{
    public class NotificationsResponse
    { 
            public int Srno { get; set; }
            public string? ProcessName { get; set; }

            public int PendingCount { get; set; }

            public string? displayName { get; set; }
       
    }
}
