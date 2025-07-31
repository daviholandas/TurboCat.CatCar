using CatCar.FrontOffice.Domain.Entities;

namespace CatCar.FrontOffice.Domain.Repositories;

/// <summary>
/// Repository interface for Vehicle aggregate
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Gets a vehicle by ID
    /// </summary>
    Task<Vehicle?> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a vehicle by VIN
    /// </summary>
    Task<Vehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a vehicle by license plate
    /// </summary>
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all vehicles for a specific customer
    /// </summary>
    Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets vehicles due for service
    /// </summary>
    Task<IEnumerable<Vehicle>> GetVehiclesDueForServiceAsync(int maxDaysBetweenService = 180, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches vehicles by make, model, or license plate
    /// </summary>
    Task<IEnumerable<Vehicle>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active vehicles
    /// </summary>
    Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new vehicle
    /// </summary>
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing vehicle
    /// </summary>
    Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a vehicle with the given VIN already exists
    /// </summary>
    Task<bool> ExistsWithVinAsync(string vin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a vehicle with the given license plate already exists
    /// </summary>
    Task<bool> ExistsWithLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of active vehicles
    /// </summary>
    Task<int> GetActiveVehicleCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets vehicles by make and model
    /// </summary>
    Task<IEnumerable<Vehicle>> GetByMakeAndModelAsync(string make, string model, CancellationToken cancellationToken = default);
}
