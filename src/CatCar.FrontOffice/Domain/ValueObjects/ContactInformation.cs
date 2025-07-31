namespace CatCar.FrontOffice.Domain.ValueObjects;

/// <summary>
/// Value Object representing customer contact information
/// </summary>
public record ContactInformation
{
    public string FullName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public Address Address { get; init; }

    public ContactInformation(string fullName, string email, string phoneNumber, Address address)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required", nameof(phoneNumber));

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phoneNumber.Trim();
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    /// <summary>
    /// Creates a new ContactInformation with updated email
    /// </summary>
    public ContactInformation UpdateEmail(string newEmail)
    {
        return this with { Email = newEmail.Trim().ToLowerInvariant() };
    }

    /// <summary>
    /// Creates a new ContactInformation with updated phone number
    /// </summary>
    public ContactInformation UpdatePhoneNumber(string newPhoneNumber)
    {
        return this with { PhoneNumber = newPhoneNumber.Trim() };
    }
}
