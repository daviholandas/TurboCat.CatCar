namespace CatCar.FrontOffice.Domain.ValueObjects;


public sealed record ContactInformation(string FullName, 
    string Email, 
    string PhoneNumber, 
    Address Address)
{
    public ContactInformation UpdateEmail(string newEmail)
        => this with { Email = newEmail.Trim().ToLowerInvariant() };

    public ContactInformation UpdatePhoneNumber(string newPhoneNumber)
        => this with { PhoneNumber = newPhoneNumber.Trim() };
}