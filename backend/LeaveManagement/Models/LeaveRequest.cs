namespace LeaveManagement.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int UserId { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string ManagerComments { get; set; }
        public DateTime AppliedOn { get; set; }

        public User User { get; set; }
    }
}
