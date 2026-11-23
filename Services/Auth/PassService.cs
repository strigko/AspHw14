using System.Security.Cryptography;

namespace p48_lessons_asp.Services.Auth;

public static class PassService
{
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32
        );

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');

        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] originalHash = Convert.FromBase64String(parts[1]);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32
        );

        return CryptographicOperations.FixedTimeEquals(hash, originalHash);
    }
}
