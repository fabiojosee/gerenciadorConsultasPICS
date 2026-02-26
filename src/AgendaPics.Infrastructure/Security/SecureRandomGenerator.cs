using System.Security.Cryptography;
using AgendaPics.Application.Common.Interfaces;

namespace AgendaPics.Infrastructure.Security;

public class SecureRandomGenerator : ISecureRandomGenerator
{
    private const string AlphanumericChars = "abcdefghijklmnopqrstuvwxyz0123456789";

    public string GenerateCode(int length)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(length));

        var chars = new char[length];
        var randomBytes = new byte[length];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        for (int i = 0; i < length; i++)
        {
            chars[i] = AlphanumericChars[randomBytes[i] % AlphanumericChars.Length];
        }

        return new string(chars);
    }

    public string GenerateToken(int byteLength = 32)
    {
        if (byteLength <= 0)
            throw new ArgumentException("Byte length must be greater than 0", nameof(byteLength));

        var bytes = new byte[byteLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public byte[] GenerateBytes(int length)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(length));

        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        return bytes;
    }
}
