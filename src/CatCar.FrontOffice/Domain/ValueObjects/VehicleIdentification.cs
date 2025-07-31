namespace CatCar.FrontOffice.Domain.ValueObjects;

/// <summary>
/// Value Object representing vehicle identification information
/// </summary>
public record VehicleIdentification
{
    public string Vin { get; init; }
    public string LicensePlate { get; init; }
    public string Make { get; init; }
    public string Model { get; init; }
    public int Year { get; init; }
    public string Color { get; init; }

    public VehicleIdentification(string vin, string licensePlate, string make, string model, int year, string color)
    {
        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN is required", nameof(vin));

        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new ArgumentException("License plate is required", nameof(licensePlate));

        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make is required", nameof(make));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required", nameof(model));

        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new ArgumentException("Year must be between 1900 and next year", nameof(year));

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color is required", nameof(color));

        Vin = vin.Trim().ToUpperInvariant();
        LicensePlate = licensePlate.Trim().ToUpperInvariant();
        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        Color = color.Trim();
    }

    /// <summary>
    /// Returns a display-friendly vehicle description
    /// </summary>
    public string ToDisplayString()
    {
        return $"{Year} {Make} {Model} ({Color}) - {LicensePlate}";
    }

    /// <summary>
    /// Validates VIN format (basic validation)
    /// </summary>
    public bool IsValidVin()
    {
        return Vin.Length == 17 && Vin.All(c => char.IsLetterOrDigit(c));
    }
}
