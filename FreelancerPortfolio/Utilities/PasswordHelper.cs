using System.Security.Cryptography;

namespace FreelancerPortfolio.Utilities;

public static class PasswordHelper
{
    public static string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public static bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
