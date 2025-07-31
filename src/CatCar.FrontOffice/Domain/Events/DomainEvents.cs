using CatCar.SharedKernel.Common;

namespace CatCar.FrontOffice.Domain.Events;

/// <summary>
/// Domain event raised when a work order is created
/// </summary>
public record WorkOrderCreatedEvent : IDomainEvent
{
    public Guid WorkOrderId { get; init; }
    public Guid CustomerId { get; init; }
    public Guid VehicleId { get; init; }
    public string ServiceDescription { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; }

    public WorkOrderCreatedEvent(Guid workOrderId, Guid customerId, Guid vehicleId, 
        string serviceDescription, string createdBy)
    {
        WorkOrderId = workOrderId;
        CustomerId = customerId;
        VehicleId = vehicleId;
        ServiceDescription = serviceDescription;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Domain event raised when a quote is proposed to a customer
/// </summary>
public record QuoteProposedEvent : IDomainEvent
{
    public Guid WorkOrderId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; }
    public DateTime ProposedAt { get; init; }
    public int QuoteValidityDays { get; init; }

    public QuoteProposedEvent(Guid workOrderId, decimal totalAmount, string currency, int quoteValidityDays = 30)
    {
        WorkOrderId = workOrderId;
        TotalAmount = totalAmount;
        Currency = currency;
        ProposedAt = DateTime.UtcNow;
        QuoteValidityDays = quoteValidityDays;
    }
}

/// <summary>
/// Domain event raised when a customer approves a quote
/// </summary>
public record QuoteApprovedEvent : IDomainEvent
{
    public Guid WorkOrderId { get; init; }
    public decimal ApprovedAmount { get; init; }
    public string Currency { get; init; }
    public DateTime ApprovedAt { get; init; }
    public string CustomerSignature { get; init; }

    public QuoteApprovedEvent(Guid workOrderId, decimal approvedAmount, string currency, 
        DateTime approvedAt, string customerSignature)
    {
        WorkOrderId = workOrderId;
        ApprovedAmount = approvedAmount;
        Currency = currency;
        ApprovedAt = approvedAt;
        CustomerSignature = customerSignature;
    }
}

/// <summary>
/// Domain event raised when a customer rejects a quote
/// </summary>
public record QuoteRejectedEvent : IDomainEvent
{
    public Guid WorkOrderId { get; init; }
    public string RejectionReason { get; init; }
    public DateTime RejectedAt { get; init; }

    public QuoteRejectedEvent(Guid workOrderId, string rejectionReason)
    {
        WorkOrderId = workOrderId;
        RejectionReason = rejectionReason;
        RejectedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when a customer is registered
/// </summary>
public record CustomerRegisteredEvent : IDomainEvent
{
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; }
    public string Email { get; init; }
    public DateTime RegisteredAt { get; init; }

    public CustomerRegisteredEvent(Guid customerId, string customerName, string email)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        Email = email;
        RegisteredAt = DateTime.UtcNow;
    }
}
