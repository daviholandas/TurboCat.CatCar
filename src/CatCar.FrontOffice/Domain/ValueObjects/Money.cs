namespace CatCar.FrontOffice.Domain.ValueObjects;

/// <summary>
/// Value Object representing monetary values in the automotive repair context
/// </summary>
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Money(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required", nameof(currency));

        Amount = Math.Round(amount, 2); // Always round to 2 decimal places for currency
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Adds two Money objects (must be same currency)
    /// </summary>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts two Money objects (must be same currency)
    /// </summary>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies money by a factor
    /// </summary>
    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    /// <summary>
    /// Formats money for display
    /// </summary>
    public string ToDisplayString()
    {
        return Currency switch
        {
            "BRL" => $"R$ {Amount:N2}",
            "USD" => $"$ {Amount:N2}",
            "EUR" => $"€ {Amount:N2}",
            _ => $"{Amount:N2} {Currency}"
        };
    }

    public static Money Zero(string currency = "BRL") => new Money(0, currency);

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
}
