using CatCar.SharedKernel.Common;
using CatCar.FrontOffice.Domain.ValueObjects;
using CatCar.FrontOffice.Domain.Enums;
using CatCar.FrontOffice.Domain.Events;

namespace CatCar.FrontOffice.Domain.Entities;

/// <summary>
/// WorkOrder aggregate root - the central business transaction for automotive repair services
/// </summary>
public class WorkOrder : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }
    public string ServiceDescription { get; private set; }
    public ServiceType ServiceType { get; private set; }
    public ServicePriority Priority { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public DateTime? ScheduledDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public Quote? Quote { get; private set; }
    public string? CustomerNotes { get; private set; }
    public string? InternalNotes { get; private set; }
    public string CreatedBy { get; private set; }
    public string? AssignedTechnician { get; private set; }

    // Private constructor for EF Core
    private WorkOrder() { }

    /// <summary>
    /// Creates a new work order
    /// </summary>
    public WorkOrder(Guid customerId, Guid vehicleId, string serviceDescription, 
        ServiceType serviceType, ServicePriority priority, DateTime requestedDate, 
        string createdBy, string? customerNotes = null)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(vehicleId));

        if (string.IsNullOrWhiteSpace(serviceDescription))
            throw new ArgumentException("Service description is required", nameof(serviceDescription));

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by is required", nameof(createdBy));

        CustomerId = customerId;
        VehicleId = vehicleId;
        ServiceDescription = serviceDescription.Trim();
        ServiceType = serviceType;
        Priority = priority;
        RequestedDate = requestedDate;
        Status = WorkOrderStatus.Draft;
        CustomerNotes = customerNotes?.Trim();
        CreatedBy = createdBy.Trim();

        AddDomainEvent(new WorkOrderCreatedEvent(Id, customerId, vehicleId, serviceDescription, createdBy));
    }

    /// <summary>
    /// Moves the work order to pending diagnosis status
    /// </summary>
    public void StartDiagnosis()
    {
        if (Status != WorkOrderStatus.Draft)
            throw new InvalidOperationException($"Cannot start diagnosis from status: {Status}");

        Status = WorkOrderStatus.PendingDiagnosis;
        Update();
    }

    /// <summary>
    /// Creates and proposes a quote for the work order
    /// </summary>
    public void ProposeQuote(IEnumerable<QuoteLineItem> lineItems, decimal estimatedHours, 
        decimal laborRatePerHour = 150m, int validityDays = 30, string? notes = null)
    {
        if (Status != WorkOrderStatus.PendingDiagnosis && Status != WorkOrderStatus.QuoteInPreparation)
            throw new InvalidOperationException($"Cannot propose quote from status: {Status}");

        Quote = new Quote(lineItems, estimatedHours, laborRatePerHour, validityDays, notes);
        Status = WorkOrderStatus.AwaitingApproval;
        Update();

        AddDomainEvent(new QuoteProposedEvent(Id, Quote.TotalAmount.Amount, Quote.TotalAmount.Currency, validityDays));
    }

    /// <summary>
    /// Approves the current quote
    /// </summary>
    public void ApproveQuote(string customerSignature, DateTime approvalDate)
    {
        if (Quote is null)
            throw new InvalidOperationException("No quote available to approve");

        if (Status != WorkOrderStatus.AwaitingApproval)
            throw new InvalidOperationException($"Cannot approve quote from status: {Status}");

        Quote.Approve(customerSignature, approvalDate);
        Status = WorkOrderStatus.Approved;
        Update();

        AddDomainEvent(new QuoteApprovedEvent(Id, Quote.TotalAmount.Amount, Quote.TotalAmount.Currency, approvalDate, customerSignature));
    }

    /// <summary>
    /// Rejects the current quote
    /// </summary>
    public void RejectQuote(string rejectionReason)
    {
        if (Quote is null)
            throw new InvalidOperationException("No quote available to reject");

        if (Status != WorkOrderStatus.AwaitingApproval)
            throw new InvalidOperationException($"Cannot reject quote from status: {Status}");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required", nameof(rejectionReason));

        Status = WorkOrderStatus.Rejected;
        Update();

        AddDomainEvent(new QuoteRejectedEvent(Id, rejectionReason));
    }

    /// <summary>
    /// Schedules the work order for a specific date
    /// </summary>
    public void Schedule(DateTime scheduledDate, string? assignedTechnician = null)
    {
        if (Status != WorkOrderStatus.Approved)
            throw new InvalidOperationException($"Can only schedule approved work orders. Current status: {Status}");

        if (scheduledDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Scheduled date cannot be in the past", nameof(scheduledDate));

        ScheduledDate = scheduledDate;
        AssignedTechnician = assignedTechnician?.Trim();
        Update();
    }

    /// <summary>
    /// Starts the work on this order
    /// </summary>
    public void StartWork()
    {
        if (Status != WorkOrderStatus.Approved)
            throw new InvalidOperationException($"Cannot start work from status: {Status}");

        Status = WorkOrderStatus.InProgress;
        Update();
    }

    /// <summary>
    /// Completes the work order
    /// </summary>
    public void CompleteWork(DateTime completedDate)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete work from status: {Status}");

        CompletedDate = completedDate;
        Status = WorkOrderStatus.Completed;
        Update();
    }

    /// <summary>
    /// Marks the work order as delivered to customer
    /// </summary>
    public void MarkAsDelivered()
    {
        if (Status != WorkOrderStatus.Completed)
            throw new InvalidOperationException($"Cannot deliver from status: {Status}");

        Status = WorkOrderStatus.Delivered;
        Update();
    }

    /// <summary>
    /// Cancels the work order
    /// </summary>
    public void Cancel(string cancellationReason)
    {
        if (Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Delivered)
            throw new InvalidOperationException($"Cannot cancel work order with status: {Status}");

        if (string.IsNullOrWhiteSpace(cancellationReason))
            throw new ArgumentException("Cancellation reason is required", nameof(cancellationReason));

        Status = WorkOrderStatus.Cancelled;
        InternalNotes = $"Cancelled: {cancellationReason}";
        Update();
    }

    /// <summary>
    /// Updates customer notes
    /// </summary>
    public void UpdateCustomerNotes(string? notes)
    {
        CustomerNotes = notes?.Trim();
        Update();
    }

    /// <summary>
    /// Updates internal notes (staff only)
    /// </summary>
    public void UpdateInternalNotes(string? notes)
    {
        InternalNotes = notes?.Trim();
        Update();
    }

    /// <summary>
    /// Updates the service description
    /// </summary>
    public void UpdateServiceDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("Service description is required", nameof(newDescription));

        if (Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Delivered)
            throw new InvalidOperationException("Cannot update description for completed/delivered work orders");

        ServiceDescription = newDescription.Trim();
        Update();
    }

    /// <summary>
    /// Updates the priority of the work order
    /// </summary>
    public void UpdatePriority(ServicePriority newPriority)
    {
        if (Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Delivered)
            throw new InvalidOperationException("Cannot update priority for completed/delivered work orders");

        Priority = newPriority;
        Update();
    }

    /// <summary>
    /// Assigns or reassigns a technician to this work order
    /// </summary>
    public void AssignTechnician(string technicianName)
    {
        if (string.IsNullOrWhiteSpace(technicianName))
            throw new ArgumentException("Technician name is required", nameof(technicianName));

        if (Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Delivered || Status == WorkOrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot assign technician to work order with status: {Status}");

        AssignedTechnician = technicianName.Trim();
        Update();
    }

    /// <summary>
    /// Checks if the work order is overdue based on requested date
    /// </summary>
    public bool IsOverdue => RequestedDate < DateTime.UtcNow.Date && 
                             Status != WorkOrderStatus.Completed && 
                             Status != WorkOrderStatus.Delivered && 
                             Status != WorkOrderStatus.Cancelled;

    /// <summary>
    /// Gets the number of days since the work order was created
    /// </summary>
    public int DaysOpen => (DateTime.UtcNow.Date - CreatedAt.Date).Days;

    /// <summary>
    /// Checks if the work order has an approved quote
    /// </summary>
    public bool HasApprovedQuote => Quote?.IsApproved == true;

    /// <summary>
    /// Gets the total approved amount (if quote is approved)
    /// </summary>
    public Money? ApprovedAmount => HasApprovedQuote ? Quote?.TotalAmount : null;

    /// <summary>
    /// Checks if the work order can be started
    /// </summary>
    public bool CanStartWork => Status == WorkOrderStatus.Approved && HasApprovedQuote;

    /// <summary>
    /// Checks if the work order is in a final state
    /// </summary>
    public bool IsFinal => Status is WorkOrderStatus.Delivered 
                                or WorkOrderStatus.Cancelled 
                                or WorkOrderStatus.Rejected;
}
