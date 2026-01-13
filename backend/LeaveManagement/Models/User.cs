namespace LeaveManagement.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Employee / Manager

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
