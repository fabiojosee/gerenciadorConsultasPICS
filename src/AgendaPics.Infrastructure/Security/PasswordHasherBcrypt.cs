using AgendaPics.Application.Common.Interfaces;

namespace AgendaPics.Infrastructure.Security;

public class PasswordHasherBcrypt : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    public bool IsLegacyHash(string hash)
    {
        // BCrypt hashes always start with "$2" and have a specific format
        // SHA-256 hashes are 64 character hex strings
        if (string.IsNullOrEmpty(hash))
            return false;

        // BCrypt hash starts with $2a$, $2b$, or $2y$
        if (hash.StartsWith("$2"))
            return false;

        // SHA-256 hashes are exactly 64 hex characters
        return hash.Length == 64 && hash.All(c => "0123456789abcdef".Contains(c));
    }
}
