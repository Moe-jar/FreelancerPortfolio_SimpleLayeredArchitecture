using FreelancerPortfolio.DTOs;

namespace FreelancerPortfolio.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
