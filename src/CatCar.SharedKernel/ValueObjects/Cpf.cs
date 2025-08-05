using System.Text.RegularExpressions;

namespace CatCar.SharedKernel.ValueObjects;

public sealed record Cpf
{
    private string _value;

    private Cpf(string? value)
    {
        if (value is null)
                ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        var digits = OnlyDigits(_value);

        if (digits.Length != 11 || !IsValidCpf(digits))
            ArgumentException.ThrowIfNullOrEmpty("Invalid CPF.", nameof(value));

        _value = digits;
    }

    private static string OnlyDigits(string input)
        => Regex.Replace(input, @"\D", "");

    private static bool IsValidCpf(string cpf)
    {
        if (Regex.IsMatch(cpf, @"^(.)\1{10}$"))
            return false;

        var numbers = new int[11];
        for (int i = 0; i < 11; i++)
            numbers[i] = cpf[i] - '0';

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += numbers[i] * (10 - i);
        var firstCheck = sum % 11;
        firstCheck = firstCheck < 2 ? 0 : 11 - firstCheck;
        if (numbers[9] != firstCheck)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += numbers[i] * (11 - i);
        var secondCheck = sum % 11;
        secondCheck = secondCheck < 2 ? 0 : 11 - secondCheck;
        return numbers[10] == secondCheck;
    }

    public override string ToString() => _value;

    public static explicit operator Cpf(string value)
        => new(value);
    public static implicit operator string(Cpf cpf)
        => cpf._value;
}