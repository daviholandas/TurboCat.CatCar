using CatCar.SharedKernel.Common;
using CatCar.FrontOffice.Domain.ValueObjects;
using CatCar.FrontOffice.Domain.Events;

namespace CatCar.FrontOffice.Domain.Entities;

/// <summary>
/// Customer aggregate root representing a customer in the automotive repair business
/// </summary>
public class Customer : AggregateRoot
{
    private readonly HashSet<Guid> _vehicleIds = [];

    public ContactInformation ContactInformation { get; private set; }
    public DateTime DateRegistered { get; private set; }
    public bool IsActive { get; private set; }
    public string? PreferredContactMethod { get; private set; }

    /// <summary>
    /// Gets the list of vehicle IDs owned by this customer
    /// </summary>
    public IReadOnlyList<Guid> VehicleIds => _vehicleIds.AsReadOnly();

    // Private constructor for EF Core
    private Customer() { }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    public Customer(ContactInformation contactInformation, string? preferredContactMethod = null)
    {
        ContactInformation = contactInformation ?? throw new ArgumentNullException(nameof(contactInformation));
        DateRegistered = DateTime.UtcNow;
        IsActive = true;
        PreferredContactMethod = preferredContactMethod;

        AddDomainEvent(new CustomerRegisteredEvent(Id, contactInformation.FullName, contactInformation.Email));
    }

    /// <summary>
    /// Updates customer contact information
    /// </summary>
    public void UpdateContactInformation(ContactInformation newContactInformation)
    {
        if (newContactInformation is null)
            throw new ArgumentNullException(nameof(newContactInformation));

        if (!IsActive)
            throw new InvalidOperationException("Cannot update contact information for inactive customer");

        ContactInformation = newContactInformation;
        Update();
    }

    /// <summary>
    /// Associates a vehicle with this customer
    /// </summary>
    public void AddVehicle(Guid vehicleId)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(vehicleId));

        if (!IsActive)
            throw new InvalidOperationException("Cannot add vehicle to inactive customer");

        if (!_vehicleIds.Contains(vehicleId))
        {
            _vehicleIds.Add(vehicleId);
            Update();
        }
    }

    /// <summary>
    /// Removes a vehicle association from this customer
    /// </summary>
    public void RemoveVehicle(Guid vehicleId)
    {
        if (_vehicleIds.Remove(vehicleId))
        {
            Update();
        }
    }

    /// <summary>
    /// Updates the preferred contact method for this customer
    /// </summary>
    public void UpdatePreferredContactMethod(string preferredContactMethod)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update preferred contact method for inactive customer");

        PreferredContactMethod = preferredContactMethod;
        Update();
    }

    /// <summary>
    /// Deactivates the customer account
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        Update();
    }

    /// <summary>
    /// Reactivates the customer account
    /// </summary>
    public void Reactivate()
    {
        if (IsActive)
            return;

        IsActive = true;
        Update();
    }

    /// <summary>
    /// Checks if the customer owns a specific vehicle
    /// </summary>
    public bool OwnsVehicle(Guid vehicleId)
    {
        return _vehicleIds.Contains(vehicleId);
    }

    /// <summary>
    /// Gets the total number of vehicles owned by this customer
    /// </summary>
    public int VehicleCount => _vehicleIds.Count;
}
