using System.Text.RegularExpressions;
using AgendaPics.Domain.Common;

namespace AgendaPics.Domain.ValueObjects;

public sealed partial class Telefone : ValueObject
{
    public string Value { get; }

    private Telefone(string value)
    {
        Value = value;
    }

    public static Result<Telefone> Create(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return Result<Telefone>.Failure("Telefone é obrigatório");

        var cleanTelefone = new string(telefone.Where(char.IsDigit).ToArray());

        if (cleanTelefone.Length < 10 || cleanTelefone.Length > 11)
            return Result<Telefone>.Failure("Telefone deve conter 10 ou 11 dígitos");

        return Result<Telefone>.Success(new Telefone(cleanTelefone));
    }

    public static bool IsValid(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return false;

        var cleanTelefone = new string(telefone.Where(char.IsDigit).ToArray());
        return cleanTelefone.Length >= 10 && cleanTelefone.Length <= 11;
    }

    public string ToFormattedString()
    {
        if (Value.Length == 11)
            return $"({Value[..2]}) {Value[2..7]}-{Value[7..]}";
        return $"({Value[..2]}) {Value[2..6]}-{Value[6..]}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Telefone telefone) => telefone.Value;
}
