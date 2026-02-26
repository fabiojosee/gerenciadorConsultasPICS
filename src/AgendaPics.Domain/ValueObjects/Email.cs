using System.Text.RegularExpressions;
using AgendaPics.Domain.Common;

namespace AgendaPics.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    private static readonly Regex EmailRegex = MyEmailRegex();

    public string Value { get; }

    private Email(string value)
    {
        Value = value.ToLowerInvariant();
    }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Email>.Failure("E-mail é obrigatório");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > 254)
            return Result<Email>.Failure("E-mail deve ter no máximo 254 caracteres");

        if (!EmailRegex.IsMatch(trimmedEmail))
            return Result<Email>.Failure("Formato de e-mail inválido");

        return Result<Email>.Success(new Email(trimmedEmail));
    }

    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmedEmail = email.Trim();
        return trimmedEmail.Length <= 254 && EmailRegex.IsMatch(trimmedEmail);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex MyEmailRegex();
}
