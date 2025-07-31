namespace CatCar.FrontOffice.Domain.Enums;

/// <summary>
/// Represents the various states of a WorkOrder in the repair process
/// </summary>
public enum WorkOrderStatus
{
    /// <summary>
    /// Initial state when service request is created
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Work order is pending initial assessment
    /// </summary>
    PendingDiagnosis = 1,

    /// <summary>
    /// Diagnosis completed, quote is being prepared
    /// </summary>
    QuoteInPreparation = 2,

    /// <summary>
    /// Quote sent to customer, awaiting approval
    /// </summary>
    AwaitingApproval = 3,

    /// <summary>
    /// Customer approved the quote, ready for workshop
    /// </summary>
    Approved = 4,

    /// <summary>
    /// Work is in progress in the workshop
    /// </summary>
    InProgress = 5,

    /// <summary>
    /// Work completed, vehicle ready for pickup
    /// </summary>
    Completed = 6,

    /// <summary>
    /// Vehicle delivered to customer
    /// </summary>
    Delivered = 7,

    /// <summary>
    /// Customer rejected the quote
    /// </summary>
    Rejected = 8,

    /// <summary>
    /// Work order was cancelled
    /// </summary>
    Cancelled = 9
}
