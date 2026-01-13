namespace LeaveManagement.DTOs
{
    public class LeaveResponseDto
    {
        public int LeaveRequestId { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? AppliedOn { get; set; }
        public string? ManagerComments { get; set; }
    }
}