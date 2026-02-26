using AgendaPics.Domain.Common;

namespace AgendaPics.Domain.ValueObjects;

public sealed class CNPJ : ValueObject
{
    public string Value { get; }

    private CNPJ(string value)
    {
        Value = value;
    }

    public static Result<CNPJ> Create(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return Result<CNPJ>.Failure("CNPJ é obrigatório");

        var cleanCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cleanCnpj.Length != 14)
            return Result<CNPJ>.Failure("CNPJ deve conter 14 dígitos");

        if (!IsValid(cleanCnpj))
            return Result<CNPJ>.Failure("CNPJ inválido");

        return Result<CNPJ>.Success(new CNPJ(cleanCnpj));
    }

    public static bool IsValid(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var cleanCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cleanCnpj.Length != 14)
            return false;

        // Check if all digits are the same (invalid CNPJ)
        if (cleanCnpj.All(c => c == cleanCnpj[0]))
            return false;

        // First check digit multipliers
        int[] multiplier1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        // Second check digit multipliers
        int[] multiplier2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        // Calculate first check digit
        var sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (cleanCnpj[i] - '0') * multiplier1[i];

        var remainder = sum % 11;
        var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        if ((cleanCnpj[12] - '0') != firstCheckDigit)
            return false;

        // Calculate second check digit
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += (cleanCnpj[i] - '0') * multiplier2[i];

        remainder = sum % 11;
        var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        return (cleanCnpj[13] - '0') == secondCheckDigit;
    }

    public string ToFormattedString()
    {
        return $"{Value[..2]}.{Value[2..5]}.{Value[5..8]}/{Value[8..12]}-{Value[12..]}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CNPJ cnpj) => cnpj.Value;
}
