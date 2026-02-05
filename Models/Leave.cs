using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.API.Models
{
    public class LeaveApproveResponse
    {
        public List<ApproveLeaveDetails> LeaveDetails { get; set; }
        public List<LeaveBalance> LeaveBalances { get; set; }
        public List<LeaveApprover> Approvers { get; set; }
    }
    public class Leave
    {
        public string RequestedDate { get; set; }
        public string? LeaveType { get; set; }

       public string Period { get; set; }

        public string? Applicant { get; set; }
        public string? Status { get; set; }
        public int? LeaveDays { get; set; }
        public int? Req_id { get; set; }
        public int? LeaveTypeid { get; set; }
        public string? Emp_Code { get; set; }
        public string? Wrk_schedule { get; set; }
        public string? LRType { get; set; }
    }

    public class ApproveLeaveDetails
        {
            public int leaveTypeid { get; set; }
            public int Req_id { get; set; }
            public string Leave_Type { get; set; }
            public string reqest_dt { get; set; }
            public string Period { get; set; }
            public decimal LeaveDays { get; set; }

            public string Emp_Name { get; set; }
            public string Emp_Code { get; set; }
            public string DesginationName { get; set; }
            public string Department_Name { get; set; }

            public string fromdate { get; set; }
            public string todate { get; set; }

            public string Reason { get; set; }
            public string approverCommnets { get; set; }
            public string Leave_Type_Description { get; set; }

            public string Leave_FromDate { get; set; }
            public string Leave_ToDate { get; set; }

            public string Emp_Emailaddress { get; set; }
            public int Status_id { get; set; }
            public int LeaveConditionTypeid { get; set; }

            public string For_From { get; set; }
            public string For_To { get; set; }

            public string UploadFile { get; set; }
            public string grade { get; set; }
            public string reason_req_cancel { get; set; }

            public string frmdate_email { get; set; }
            public string todate_email { get; set; }

            public string lAppr_status { get; set; }
            public string ismodify { get; set; }
            public string IsExport { get; set; }
        }

        public class LeaveBalance
    {
        public string LeaveType { get; set; }
        public string OpeningBalance { get; set; }
        public string Earned { get; set; }
        public string Availed { get; set; }
        public string Pending { get; set; }
        public string Future { get; set; }
        public string Balance { get; set; }
        public decimal AccruedLeaves { get; set; }
    }

    public class LeaveApprover
    {
        public int APPR_ID { get; set; }
        public string A_EMP_CODE { get; set; }
        public string Emp_Name { get; set; }
        public string Emp_Emailaddress { get; set; }
        public string Status { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ApproverRemark { get; set; }
    }
}
