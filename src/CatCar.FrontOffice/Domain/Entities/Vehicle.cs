using CatCar.SharedKernel.Common;
using CatCar.FrontOffice.Domain.ValueObjects;

namespace CatCar.FrontOffice.Domain.Entities;

public class Vehicle : AggregateRoot
{
    private readonly List<string> _serviceHistory = new();

    public Guid CustomerId { get; private set; }
    public VehicleIdentification Identification { get; private set; }
    public int Mileage { get; private set; }
    public DateTime LastServiceDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the service history notes for this vehicle
    /// </summary>
    public IReadOnlyList<string> ServiceHistory => _serviceHistory.AsReadOnly();

    // Private constructor for EF Core
    private Vehicle() { }

    /// <summary>
    /// Creates a new vehicle
    /// </summary>
    public Vehicle(Guid customerId, VehicleIdentification identification, int mileage = 0, string? notes = null)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        CustomerId = customerId;
        Identification = identification ?? throw new ArgumentNullException(nameof(identification));
        Mileage = mileage >= 0 ? mileage : throw new ArgumentException("Mileage cannot be negative", nameof(mileage));
        Notes = notes;
        LastServiceDate = DateTime.MinValue; // No service yet
        IsActive = true;
    }

    /// <summary>
    /// Updates the vehicle's mileage
    /// </summary>
    public void UpdateMileage(int newMileage)
    {
        if (newMileage < 0)
            throw new ArgumentException("Mileage cannot be negative", nameof(newMileage));

        if (newMileage < Mileage)
            throw new ArgumentException("New mileage cannot be less than current mileage", nameof(newMileage));

        if (!IsActive)
            throw new InvalidOperationException("Cannot update mileage for inactive vehicle");

        Mileage = newMileage;
        Update();
    }

    /// <summary>
    /// Adds a service record to the vehicle's history
    /// </summary>
    public void AddServiceRecord(string serviceDescription, DateTime serviceDate, int? mileageAtService = null)
    {
        if (string.IsNullOrWhiteSpace(serviceDescription))
            throw new ArgumentException("Service description is required", nameof(serviceDescription));

        if (!IsActive)
            throw new InvalidOperationException("Cannot add service record to inactive vehicle");

        var serviceRecord = $"{serviceDate:yyyy-MM-dd}: {serviceDescription}";
        if (mileageAtService.HasValue)
        {
            serviceRecord += $" (Mileage: {mileageAtService:N0})";
            
            // Update vehicle mileage if provided and higher than current
            if (mileageAtService.Value > Mileage)
            {
                Mileage = mileageAtService.Value;
            }
        }

        _serviceHistory.Add(serviceRecord);
        LastServiceDate = serviceDate > LastServiceDate ? serviceDate : LastServiceDate;
        Update();
    }

    /// <summary>
    /// Updates the vehicle identification information
    /// </summary>
    public void UpdateIdentification(VehicleIdentification newIdentification)
    {
        if (newIdentification is null)
            throw new ArgumentNullException(nameof(newIdentification));

        if (!IsActive)
            throw new InvalidOperationException("Cannot update identification for inactive vehicle");

        Identification = newIdentification;
        Update();
    }

    /// <summary>
    /// Updates or adds notes about the vehicle
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update notes for inactive vehicle");

        Notes = notes;
        Update();
    }

    /// <summary>
    /// Transfers the vehicle to a different customer
    /// </summary>
    public void TransferToCustomer(Guid newCustomerId)
    {
        if (newCustomerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(newCustomerId));

        if (!IsActive)
            throw new InvalidOperationException("Cannot transfer inactive vehicle");

        if (CustomerId == newCustomerId)
            return; // No change needed

        CustomerId = newCustomerId;
        AddServiceRecord($"Vehicle transferred to new customer", DateTime.UtcNow);
        Update();
    }

    /// <summary>
    /// Deactivates the vehicle (e.g., sold, totaled, etc.)
    /// </summary>
    public void Deactivate(string reason)
    {
        if (!IsActive)
            return;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            AddServiceRecord($"Vehicle deactivated: {reason}", DateTime.UtcNow);
        }

        IsActive = false;
        Update();
    }

    /// <summary>
    /// Reactivates the vehicle
    /// </summary>
    public void Reactivate()
    {
        if (IsActive)
            return;

        IsActive = true;
        AddServiceRecord("Vehicle reactivated", DateTime.UtcNow);
        Update();
    }

    /// <summary>
    /// Gets the number of days since the last service
    /// </summary>
    public int DaysSinceLastService
    {
        get
        {
            if (LastServiceDate == DateTime.MinValue)
                return int.MaxValue; // Never serviced

            return (DateTime.UtcNow.Date - LastServiceDate.Date).Days;
        }
    }

    /// <summary>
    /// Checks if the vehicle is due for service (based on days or mileage)
    /// </summary>
    public bool IsDueForService(int maxDaysBetweenService = 180, int maxMilesBetweenService = 5000)
    {
        return DaysSinceLastService > maxDaysBetweenService;
        // Note: More sophisticated logic would consider mileage-based intervals too
    }
}
