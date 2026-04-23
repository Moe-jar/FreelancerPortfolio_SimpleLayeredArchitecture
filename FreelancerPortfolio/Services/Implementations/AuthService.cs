using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FreelancerPortfolio.DTOs;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using FreelancerPortfolio.Services.Interfaces;
using FreelancerPortfolio.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FreelancerPortfolio.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IAdminUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IConfiguration _config;

    public AuthService(
        IAdminUserRepository userRepo,
        IRefreshTokenRepository refreshTokenRepo,
        IConfiguration config)
    {
        _userRepo = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _config = config;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepo.GetByUsernameAsync(dto.Username)
            ?? throw new UnauthorizedAccessException("Invalid username or password.");

        if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password.");

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (storedToken.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token has been revoked.");

        if (storedToken.IsUsed)
            throw new UnauthorizedAccessException("Refresh token has already been used.");

        if (storedToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        storedToken.IsUsed = true;
        await _refreshTokenRepo.UpdateAsync(storedToken);

        return await GenerateTokensAsync(storedToken.AdminUser);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
        if (storedToken is not null)
        {
            storedToken.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(storedToken);
        }
    }

    private async Task<LoginResponseDto> GenerateTokensAsync(AdminUser user)
    {
        var jwtSettings = _config.GetSection("JWT");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"] ?? "FreelancerPortfolioAPI";
        var audience = jwtSettings["Audience"] ?? "FreelancerPortfolioAPI";
        var accessMinutes = int.TryParse(jwtSettings["AccessTokenExpireMinutes"], out var m) ? m : 15;
        var refreshDays = int.TryParse(jwtSettings["RefreshTokenExpireDays"], out var d) ? d : 7;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtId = Guid.NewGuid().ToString();
        var accessExpiry = DateTime.UtcNow.AddMinutes(accessMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: accessExpiry,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshTokenBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(refreshTokenBytes);
        var refreshTokenValue = Convert.ToBase64String(refreshTokenBytes);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshTokenValue,
            JwtId = jwtId,
            AdminUserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(refreshDays),
            IsUsed = false,
            IsRevoked = false,
        };
        await _refreshTokenRepo.CreateAsync(refreshTokenEntity);

        return new LoginResponseDto(accessToken, refreshTokenValue, accessExpiry, user.Username);
    }
}
