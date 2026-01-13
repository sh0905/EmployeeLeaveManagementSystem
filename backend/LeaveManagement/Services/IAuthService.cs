using LeaveManagement.DTOs;

namespace LeaveManagement.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}