namespace FreelancerPortfolio.DTOs;

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    string Username
);

public record RefreshTokenRequestDto(string RefreshToken);

public record LogoutRequestDto(string RefreshToken);
