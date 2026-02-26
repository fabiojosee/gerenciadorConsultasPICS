using AgendaPics.Domain.Common;

namespace AgendaPics.Domain.ValueObjects;

public sealed class CPF : ValueObject
{
    public string Value { get; }

    private CPF(string value)
    {
        Value = value;
    }

    public static Result<CPF> Create(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return Result<CPF>.Failure("CPF é obrigatório");

        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cleanCpf.Length != 11)
            return Result<CPF>.Failure("CPF deve conter 11 dígitos");

        if (!IsValid(cleanCpf))
            return Result<CPF>.Failure("CPF inválido");

        return Result<CPF>.Success(new CPF(cleanCpf));
    }

    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cleanCpf.Length != 11)
            return false;

        // Check if all digits are the same (invalid CPF)
        if (cleanCpf.All(c => c == cleanCpf[0]))
            return false;

        // Validate first check digit
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cleanCpf[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        if ((cleanCpf[9] - '0') != firstCheckDigit)
            return false;

        // Validate second check digit
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cleanCpf[i] - '0') * (11 - i);

        remainder = sum % 11;
        var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        return (cleanCpf[10] - '0') == secondCheckDigit;
    }

    public string ToFormattedString()
    {
        return $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CPF cpf) => cpf.Value;
}
