using CatCar.FrontOffice.Domain.Entities;
using CatCar.FrontOffice.Domain.Enums;

namespace CatCar.FrontOffice.Domain.Repositories;

/// <summary>
/// Repository interface for WorkOrder aggregate
/// </summary>
public interface IWorkOrderRepository
{
    /// <summary>
    /// Gets a work order by ID
    /// </summary>
    Task<WorkOrder?> GetByIdAsync(Guid workOrderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all work orders for a specific customer
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all work orders for a specific vehicle
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders by status
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByStatusAsync(WorkOrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders by multiple statuses
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByStatusesAsync(IEnumerable<WorkOrderStatus> statuses, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders assigned to a specific technician
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByTechnicianAsync(string technicianName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders by priority
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByPriorityAsync(ServicePriority priority, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders by service type
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByServiceTypeAsync(ServiceType serviceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overdue work orders
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetOverdueWorkOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders created within a date range
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders scheduled for a specific date
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetScheduledForDateAsync(DateTime scheduledDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active work orders (not completed, delivered, or cancelled)
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetActiveWorkOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders awaiting quote approval
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetAwaitingApprovalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches work orders by service description or customer name
    /// </summary>
    Task<IEnumerable<WorkOrder>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new work order
    /// </summary>
    Task AddAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing work order
    /// </summary>
    Task UpdateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work order statistics for reporting
    /// </summary>
    Task<WorkOrderStatistics> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of work orders by status
    /// </summary>
    Task<Dictionary<WorkOrderStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work orders with expired quotes
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetWithExpiredQuotesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Statistics about work orders for reporting purposes
/// </summary>
public record WorkOrderStatistics
{
    public int TotalWorkOrders { get; init; }
    public int CompletedWorkOrders { get; init; }
    public int ActiveWorkOrders { get; init; }
    public int OverdueWorkOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal AverageCompletionDays { get; init; }
    public Dictionary<ServiceType, int> ServiceTypeBreakdown { get; init; } = new();
    public Dictionary<ServicePriority, int> PriorityBreakdown { get; init; } = new();
    public Dictionary<WorkOrderStatus, int> StatusBreakdown { get; init; } = new();
}
