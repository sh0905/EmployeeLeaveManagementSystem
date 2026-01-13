using LeaveManagement.DTOs;

namespace LeaveManagement.Services
{
    public interface ILeaveService
    {
        Task ApplyLeaveAsync(int userId, ApplyLeaveDto dto);
        Task<List<LeaveResponseDto>> GetMyLeavesAsync(int userId);
        Task<List<LeaveResponseDto>> GetPendingLeavesAsync();
        Task<List<LeaveResponseDto>> GetAllLeavesAsync(); 
        Task ApproveLeaveAsync(int leaveId, string comments);
        Task RejectLeaveAsync(int leaveId, string comments);
        Task<LeaveBalanceDto> GetLeaveBalanceAsync(int userId);
    }
}