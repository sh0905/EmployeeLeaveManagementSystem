//using LeaveManagement.Data;
//using LeaveManagement.DTOs;
//using LeaveManagement.Models;
//using Microsoft.EntityFrameworkCore;

//namespace LeaveManagement.Services
//{
//    public class LeaveService : ILeaveService
//    {
//        private readonly ApplicationDbContext _context;

//        public LeaveService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task ApplyLeaveAsync(int userId, ApplyLeaveDto dto)
//        {
//            var leave = new LeaveRequest
//            {
//                UserId = userId,
//                LeaveType = dto.LeaveType,
//                StartDate = dto.StartDate,
//                EndDate = dto.EndDate,
//                Status = "PENDING", // Set initial status
//                AppliedOn = DateTime.Now,
//                ManagerComments = string.Empty
//            };

//            _context.LeaveRequests.Add(leave);
//            await _context.SaveChangesAsync();
//        }

//        public async Task<List<LeaveResponseDto>> GetMyLeavesAsync(int userId)
//        {
//            var leaves = await _context.LeaveRequests
//                .Where(l => l.UserId == userId)
//                .Select(l => new LeaveResponseDto
//                {
//                    LeaveRequestId = l.LeaveRequestId,
//                    LeaveType = l.LeaveType,
//                    StartDate = l.StartDate,
//                    EndDate = l.EndDate,
//                    Status = l.Status
//                })
//                .ToListAsync();

//            return leaves;
//        }

//        public async Task ApproveLeaveAsync(int leaveId, string comments)
//        {
//            var leave = await _context.LeaveRequests.FindAsync(leaveId);

//            if (leave == null)
//            {
//                throw new Exception($"Leave request with ID {leaveId} not found");
//            }

//            leave.Status = "APPROVED";
//            leave.ManagerComments = comments ?? string.Empty;
//            await _context.SaveChangesAsync();
//        }

//        public async Task<List<LeaveResponseDto>> GetPendingLeavesAsync()
//        {
//            var leaves = await _context.LeaveRequests
//                .Where(l => l.Status == "PENDING")
//                .Select(l => new LeaveResponseDto
//                {
//                    LeaveRequestId = l.LeaveRequestId,
//                    LeaveType = l.LeaveType,
//                    StartDate = l.StartDate,
//                    EndDate = l.EndDate,
//                    Status = l.Status
//                })
//                .ToListAsync();

//            return leaves;
//        }

//        public async Task RejectLeaveAsync(int leaveId, string comments)
//        {
//            var leave = await _context.LeaveRequests.FindAsync(leaveId);

//            if (leave == null)
//            {
//                throw new Exception($"Leave request with ID {leaveId} not found");
//            }

//            leave.Status = "REJECTED";
//            leave.ManagerComments = comments ?? string.Empty;
//            await _context.SaveChangesAsync();
//        }
//        public async Task<LeaveBalanceDto> GetLeaveBalanceAsync(int userId)
//        {
//            // Default balances (reset every time app restarts)
//            int vacation = 10;
//            int sick = 5;
//            int casual = 3;

//            // Get approved leaves for this user
//            var approvedLeaves = await _context.LeaveRequests
//                .Where(l => l.UserId == userId && l.Status == "APPROVED")
//                .ToListAsync();

//            // Subtract approved leaves from defaults
//            foreach (var leave in approvedLeaves)
//            {
//                if (leave.StartDate == null || leave.EndDate == null) continue;

//                int days = (leave.EndDate - leave.StartDate).Days + 1;

//                switch (leave.LeaveType?.ToUpper())
//                {
//                    case "VACATION":
//                        vacation -= days;
//                        break;
//                    case "SICK":
//                        sick -= days;
//                        break;
//                    case "CASUAL":
//                        casual -= days;
//                        break;
//                }
//            }

//            return new LeaveBalanceDto
//            {
//                Vacation = vacation,
//                Sick = sick,
//                Casual = casual
//            };
//        }
//        public async Task<List<LeaveResponseDto>> GetAllLeavesAsync()
//        {
//            var leaves = await _context.LeaveRequests
//                .Include(l => l.User) // Include user information
//                .OrderByDescending(l => l.AppliedOn)
//                .Select(l => new LeaveResponseDto
//                {
//                    LeaveRequestId = l.LeaveRequestId,
//                    LeaveType = l.LeaveType,
//                    StartDate = l.StartDate,
//                    EndDate = l.EndDate,
//                    Status = l.Status,
//                    EmployeeName = l.User.Name, 
//                    AppliedOn = l.AppliedOn
//                })
//                .ToListAsync();

//            return leaves;
//        }

//    }
//}

using LeaveManagement.Data;
using LeaveManagement.DTOs;
using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _context;

        // Default leave balances
        private const int DEFAULT_VACATION_DAYS = 10;
        private const int DEFAULT_SICK_DAYS = 5;
        private const int DEFAULT_CASUAL_DAYS = 3;

        public LeaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ApplyLeaveAsync(int userId, ApplyLeaveDto dto)
        {
            // Calculate days between start and end date
            int requestedDays = (dto.EndDate - dto.StartDate).Days + 1;

            // Validate days is positive
            if (requestedDays <= 0)
            {
                throw new InvalidOperationException("End date must be after start date");
            }

            // Get current leave balance
            var balance = await GetLeaveBalanceAsync(userId);

            // Validate sufficient balance based on leave type
            switch (dto.LeaveType?.ToUpper())
            {
                case "VACATION":
                    if (balance.Vacation < requestedDays)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient vacation leave balance. Available: {balance.Vacation} days, Requested: {requestedDays} days"
                        );
                    }
                    break;

                case "SICK":
                    if (balance.Sick < requestedDays)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient sick leave balance. Available: {balance.Sick} days, Requested: {requestedDays} days"
                        );
                    }
                    break;

                case "CASUAL":
                    if (balance.Casual < requestedDays)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient casual leave balance. Available: {balance.Casual} days, Requested: {requestedDays} days"
                        );
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Invalid leave type: {dto.LeaveType}");
            }

            // Check for overlapping leave requests (pending or approved)
            var overlapping = await _context.LeaveRequests
                .Where(l => l.UserId == userId &&
                           (l.Status == "PENDING" || l.Status == "APPROVED") &&
                           l.StartDate <= dto.EndDate &&
                           l.EndDate >= dto.StartDate)
                .AnyAsync();

            if (overlapping)
            {
                throw new InvalidOperationException("You have overlapping leave requests for the selected dates");
            }

            // Create leave request
            var leave = new LeaveRequest
            {
                UserId = userId,
                LeaveType = dto.LeaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "PENDING",
                AppliedOn = DateTime.Now,
                ManagerComments = string.Empty
            };

            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaveResponseDto>> GetMyLeavesAsync(int userId)
        {
            var leaves = await _context.LeaveRequests
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.AppliedOn)
                .Select(l => new LeaveResponseDto
                {
                    LeaveRequestId = l.LeaveRequestId,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status,
                    AppliedOn = l.AppliedOn,
                    ManagerComments = l.ManagerComments
                })
                .ToListAsync();

            return leaves;
        }

        public async Task ApproveLeaveAsync(int leaveId, string comments)
        {
            var leave = await _context.LeaveRequests.FindAsync(leaveId);
            if (leave == null)
            {
                throw new Exception($"Leave request with ID {leaveId} not found");
            }

            if (leave.Status != "PENDING")
            {
                throw new InvalidOperationException($"Cannot approve leave with status: {leave.Status}");
            }

            // Double-check balance before approval (in case multiple managers approve simultaneously)
            int requestedDays = (leave.EndDate - leave.StartDate).Days + 1;
            var balance = await GetLeaveBalanceAsync(leave.UserId);

            switch (leave.LeaveType?.ToUpper())
            {
                case "VACATION":
                    if (balance.Vacation < requestedDays)
                    {
                        throw new InvalidOperationException(
                            $"Cannot approve: Insufficient vacation leave balance. Available: {balance.Vacation} days"
                        );
                    }
                    break;

                case "SICK":
                    if (balance.Sick < requestedDays)
                    {
                        throw new InvalidOperationException(
                            $"Cannot approve: Insufficient sick leave balance. Available: {balance.Sick} days"
                        );
                    }
                    break;

                case "CASUAL":
                    if (balance.Casual < requestedDays)
                    {
                        throw new InvalidOperationException(
                            $"Cannot approve: Insufficient casual leave balance. Available: {balance.Casual} days"
                        );
                    }
                    break;
            }

            leave.Status = "APPROVED";
            leave.ManagerComments = comments ?? string.Empty;
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaveResponseDto>> GetPendingLeavesAsync()
        {
            var leaves = await _context.LeaveRequests
                .Where(l => l.Status == "PENDING")
                .Include(l => l.User)
                .OrderBy(l => l.AppliedOn)
                .Select(l => new LeaveResponseDto
                {
                    LeaveRequestId = l.LeaveRequestId,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status,
                    EmployeeName = l.User.Name,
                    AppliedOn = l.AppliedOn
                })
                .ToListAsync();

            return leaves;
        }

        public async Task RejectLeaveAsync(int leaveId, string comments)
        {
            var leave = await _context.LeaveRequests.FindAsync(leaveId);
            if (leave == null)
            {
                throw new Exception($"Leave request with ID {leaveId} not found");
            }

            if (leave.Status != "PENDING")
            {
                throw new InvalidOperationException($"Cannot reject leave with status: {leave.Status}");
            }

            leave.Status = "REJECTED";
            leave.ManagerComments = comments ?? string.Empty;
            await _context.SaveChangesAsync();
        }

        public async Task<LeaveBalanceDto> GetLeaveBalanceAsync(int userId)
        {
            // Start with default balances
            int vacation = DEFAULT_VACATION_DAYS;
            int sick = DEFAULT_SICK_DAYS;
            int casual = DEFAULT_CASUAL_DAYS;

            // Get approved leaves for this user
            var approvedLeaves = await _context.LeaveRequests
                .Where(l => l.UserId == userId && l.Status == "APPROVED")
                .ToListAsync();

            // Subtract approved leaves from defaults
            foreach (var leave in approvedLeaves)
            {
                if (leave.StartDate == null || leave.EndDate == null) continue;

                int days = (leave.EndDate - leave.StartDate).Days + 1;

                switch (leave.LeaveType?.ToUpper())
                {
                    case "VACATION":
                        vacation -= days;
                        break;
                    case "SICK":
                        sick -= days;
                        break;
                    case "CASUAL":
                        casual -= days;
                        break;
                }
            }

            // Ensure balances don't go negative (safety check)
            vacation = Math.Max(0, vacation);
            sick = Math.Max(0, sick);
            casual = Math.Max(0, casual);

            return new LeaveBalanceDto
            {
                Vacation = vacation,
                Sick = sick,
                Casual = casual
            };
        }

        public async Task<List<LeaveResponseDto>> GetAllLeavesAsync()
        {
            var leaves = await _context.LeaveRequests
                .Include(l => l.User)
                .OrderByDescending(l => l.AppliedOn)
                .Select(l => new LeaveResponseDto
                {
                    LeaveRequestId = l.LeaveRequestId,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status,
                    EmployeeName = l.User.Name,
                    AppliedOn = l.AppliedOn,
                    ManagerComments = l.ManagerComments
                })
                .ToListAsync();

            return leaves;
        }
    }
}
