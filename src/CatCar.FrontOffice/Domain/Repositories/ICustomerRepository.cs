using CatCar.FrontOffice.Domain.Entities;

namespace CatCar.FrontOffice.Domain.Repositories;

/// <summary>
/// Repository interface for Customer aggregate
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Gets a customer by ID
    /// </summary>
    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a customer by email address
    /// </summary>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active customers
    /// </summary>
    Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name or email
    /// </summary>
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new customer
    /// </summary>
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a customer with the given email already exists
    /// </summary>
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customers with vehicles due for service
    /// </summary>
    Task<IEnumerable<Customer>> GetCustomersWithVehiclesDueForServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of active customers
    /// </summary>
    Task<int> GetActiveCustomerCountAsync(CancellationToken cancellationToken = default);
}
