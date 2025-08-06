namespace CatCar.FrontOffice.Domain.ValueObjects;

public sealed record Address(string Street, 
    string City, 
    string State, 
    string PostalCode, 
    string Country = "Brasil")
{
    public string ToFullAddress
        => $"{Street}, {City}, {State} {PostalCode}, {Country}";
}
