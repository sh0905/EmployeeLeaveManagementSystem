namespace LeaveManagement.DTOs
{
    public class ApplyLeaveDto
    {
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; } 
    }
}
