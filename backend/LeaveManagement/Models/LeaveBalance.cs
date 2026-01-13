namespace LeaveManagement.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Vacation { get; set; }
        public int Sick { get; set; }
        public int Casual { get; set; }
    }
}
