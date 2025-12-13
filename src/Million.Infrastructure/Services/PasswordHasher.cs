using Million.Application.Common.Interfaces;
using System.Security.Cryptography;

namespace Million.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

        var result = new byte[SaltSize + KeySize];
        salt.CopyTo(result, 0);
        key.CopyTo(result, SaltSize);

        return Convert.ToBase64String(result);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            
            if (hashBytes.Length != SaltSize + KeySize)
                return false;

            var salt = hashBytes[..SaltSize];
            var key = hashBytes[SaltSize..];

            var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

            return CryptographicOperations.FixedTimeEquals(key, keyToCheck);
        }
        catch
        {
            return false;
        }
    }
}
