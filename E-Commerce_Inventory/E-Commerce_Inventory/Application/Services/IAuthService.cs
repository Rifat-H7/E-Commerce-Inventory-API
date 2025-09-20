using E_Commerce_Inventory.Application.DTOs;

namespace E_Commerce_Inventory.Application.Services
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponseDto<bool>> LogoutAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
    }
}
