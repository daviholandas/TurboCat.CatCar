namespace CatCar.FrontOffice.Domain.ValueObjects;

public record VehicleIdentification(LicensePlate LicensePlate, 
    string Brand, 
    string Model, 
    int Year, 
    string Color = "Unknown");

public sealed record LicensePlate
{
    private string _plate;

    private LicensePlate(string plate)
    {
        _plate = IsValidBrazilianLicensePlate(plate);
    }


    private static string IsValidBrazilianLicensePlate(string plate)
    {
        ArgumentException.ThrowIfNullOrEmpty(plate, nameof(plate));

        plate = plate.ToUpper().Replace("-", "");

        var traditionalPattern = @"^[A-Z]{3}[0-9]{4}$";
        var mercosulPattern = @"^[A-Z]{3}[0-9]{1}[A-Z]{1}[0-9]{2}$";

        if (!(System.Text.RegularExpressions.Regex.IsMatch(plate, traditionalPattern) ||
               System.Text.RegularExpressions.Regex.IsMatch(plate, mercosulPattern)))
            throw new ArgumentException("Invalid Brazilian license plate format.", nameof(plate));

        return plate;
    }

    public override string ToString() => _plate;

    public static implicit operator string(LicensePlate licensePlate)
        => licensePlate._plate;
    public static explicit operator LicensePlate(string plate)
        => new(plate);
}
