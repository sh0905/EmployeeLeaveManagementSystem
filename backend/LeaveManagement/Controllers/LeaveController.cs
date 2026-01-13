using LeaveManagement.DTOs;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly ILogger<LeaveController> _logger;

        public LeaveController(ILeaveService leaveService, ILogger<LeaveController> logger)
        {
            _leaveService = leaveService;
            _logger = logger;
        }

        /// <summary>
        /// Employee applies for leave
        /// </summary>
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveDto dto)
        {
            try
            {
                // For demo purposes, using hardcoded userId
                // In production, get from JWT token: User.FindFirst(ClaimTypes.NameIdentifier).Value
                int userId = 1;

                await _leaveService.ApplyLeaveAsync(userId, dto);
                return Ok(new { message = "Leave applied successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave");
                return StatusCode(500, new { message = "Error applying leave", error = ex.Message });
            }
        }

        /// <summary>
        /// Get employee's own leave requests
        /// </summary>
        [HttpGet("my-leaves")]
        public async Task<IActionResult> GetMyLeaves()
        {
            try
            {
                int userId = 1; // Hardcoded for demo
                var leaves = await _leaveService.GetMyLeavesAsync(userId);
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my leaves");
                return StatusCode(500, new { message = "Error fetching leaves", error = ex.Message });
            }
        }

        /// <summary>
        /// Manager gets all pending leave requests
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            try
            {
                var leaves = await _leaveService.GetPendingLeavesAsync();
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending leaves");
                return StatusCode(500, new { message = "Error fetching pending leaves", error = ex.Message });
            }
        }

        /// <summary>
        /// Manager approves a leave request
        /// </summary>
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveLeave(int id, [FromBody] ApproveLeaveDto dto)
        {
            try
            {
                await _leaveService.ApproveLeaveAsync(id, dto.ManagerComments ?? string.Empty);
                return Ok(new { message = "Leave approved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave {LeaveId}", id);
                return StatusCode(500, new { message = "Error approving leave", error = ex.Message });
            }
        }

        /// <summary>
        /// Manager rejects a leave request
        /// </summary>
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectLeave(int id, [FromBody] ApproveLeaveDto dto)
        {
            try
            {
                await _leaveService.RejectLeaveAsync(id, dto.ManagerComments ?? string.Empty);
                return Ok(new { message = "Leave rejected successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave {LeaveId}", id);
                return StatusCode(500, new { message = "Error rejecting leave", error = ex.Message });
            }
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetLeaveBalance()
        {
            int userId = 1; // replace with actual user ID from token
            var balance = await _leaveService.GetLeaveBalanceAsync(userId);
            return Ok(new
            {
                vacation = balance.Vacation,
                sick = balance.Sick,
                casual = balance.Casual
            });
        }

        /// <summary>
        /// Manager gets all leave requests (complete history)
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLeaves()
        {
            try
            {
                var leaves = await _leaveService.GetAllLeavesAsync();
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all leaves");
                return StatusCode(500, new { message = "Error fetching all leaves", error = ex.Message });
            }
        }

    }
}
