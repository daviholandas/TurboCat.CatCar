using CatCar.FrontOffice.Domain.Entities;
using CatCar.FrontOffice.Domain.Repositories;
using CatCar.FrontOffice.Domain.ValueObjects;

namespace CatCar.FrontOffice.Domain.Services;

/// <summary>
/// Domain service for customer-related business logic
/// </summary>
public class CustomerDomainService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public CustomerDomainService(ICustomerRepository customerRepository, IVehicleRepository vehicleRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
    }

    /// <summary>
    /// Validates if a customer can be created with the given email
    /// </summary>
    public async Task<bool> CanCreateCustomerWithEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return !await _customerRepository.ExistsWithEmailAsync(email, cancellationToken);
    }

    /// <summary>
    /// Registers a new customer and their first vehicle if provided
    /// </summary>
    public async Task<Customer> RegisterNewCustomerAsync(ContactInformation contactInfo, 
        VehicleIdentification? firstVehicle = null, CancellationToken cancellationToken = default)
    {
        // Check if customer already exists
        var existingCustomer = await _customerRepository.GetByEmailAsync(contactInfo.Email, cancellationToken);
        if (existingCustomer is not null)
        {
            throw new InvalidOperationException($"Customer with email {contactInfo.Email} already exists");
        }

        // Create the customer
        var customer = new Customer(contactInfo);
        await _customerRepository.AddAsync(customer, cancellationToken);

        // Add first vehicle if provided
        if (firstVehicle is not null)
        {
            var vehicle = new Vehicle(customer.Id, firstVehicle);
            await _vehicleRepository.AddAsync(vehicle, cancellationToken);
            customer.AddVehicle(vehicle.Id);
            await _customerRepository.UpdateAsync(customer, cancellationToken);
        }

        return customer;
    }

    /// <summary>
    /// Transfers a vehicle from one customer to another
    /// </summary>
    public async Task TransferVehicleAsync(Guid vehicleId, Guid newCustomerId, CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
            throw new ArgumentException($"Vehicle with ID {vehicleId} not found");

        var oldCustomer = await _customerRepository.GetByIdAsync(vehicle.CustomerId, cancellationToken);
        var newCustomer = await _customerRepository.GetByIdAsync(newCustomerId, cancellationToken);
        
        if (newCustomer is null)
            throw new ArgumentException($"Customer with ID {newCustomerId} not found");

        // Remove vehicle from old customer
        oldCustomer?.RemoveVehicle(vehicleId);
        if (oldCustomer is not null)
            await _customerRepository.UpdateAsync(oldCustomer, cancellationToken);

        // Transfer vehicle
        vehicle.TransferToCustomer(newCustomerId);
        await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);

        // Add vehicle to new customer
        newCustomer.AddVehicle(vehicleId);
        await _customerRepository.UpdateAsync(newCustomer, cancellationToken);
    }

    /// <summary>
    /// Calculates customer loyalty score based on service history
    /// </summary>
    public CustomerLoyaltyScore CalculateLoyaltyScore(Customer customer, IEnumerable<WorkOrder> workOrderHistory)
    {
        var completedOrders = workOrderHistory.Where(wo => wo.Status == Domain.Enums.WorkOrderStatus.Delivered).ToList();
        var totalSpent = completedOrders.Where(wo => wo.ApprovedAmount is not null).Sum(wo => wo.ApprovedAmount!.Amount);
        var yearsAsCustomer = (DateTime.UtcNow - customer.DateRegistered).Days / 365.0;
        
        var loyaltyLevel = (completedOrders.Count, totalSpent) switch
        {
            (>= 10, >= 10000) => LoyaltyLevel.Platinum,
            (>= 5, >= 5000) => LoyaltyLevel.Gold,
            (>= 3, >= 2000) => LoyaltyLevel.Silver,
            _ => LoyaltyLevel.Bronze
        };

        return new CustomerLoyaltyScore(
            loyaltyLevel,
            completedOrders.Count,
            new Money(totalSpent),
            yearsAsCustomer,
            customer.VehicleCount
        );
    }

    /// <summary>
    /// Determines if a customer is eligible for VIP treatment
    /// </summary>
    public bool IsVipCustomer(CustomerLoyaltyScore loyaltyScore)
    {
        return loyaltyScore.Level is LoyaltyLevel.Gold or LoyaltyLevel.Platinum;
    }
}

/// <summary>
/// Represents a customer's loyalty score and status
/// </summary>
public record CustomerLoyaltyScore
{
    public LoyaltyLevel Level { get; init; }
    public int CompletedServices { get; init; }
    public Money TotalSpent { get; init; }
    public double YearsAsCustomer { get; init; }
    public int VehicleCount { get; init; }
    public decimal DiscountPercentage => Level switch
    {
        LoyaltyLevel.Platinum => 15m,
        LoyaltyLevel.Gold => 10m,
        LoyaltyLevel.Silver => 5m,
        _ => 0m
    };

    public CustomerLoyaltyScore(LoyaltyLevel level, int completedServices, Money totalSpent, 
        double yearsAsCustomer, int vehicleCount)
    {
        Level = level;
        CompletedServices = completedServices;
        TotalSpent = totalSpent;
        YearsAsCustomer = yearsAsCustomer;
        VehicleCount = vehicleCount;
    }
}

/// <summary>
/// Customer loyalty levels
/// </summary>
public enum LoyaltyLevel
{
    Bronze = 0,
    Silver = 1,
    Gold = 2,
    Platinum = 3
}
