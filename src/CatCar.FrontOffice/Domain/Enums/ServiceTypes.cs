namespace CatCar.FrontOffice.Domain.Enums;

/// <summary>
/// Represents the priority level of a service request
/// </summary>
public enum ServicePriority
{
    /// <summary>
    /// Normal service request
    /// </summary>
    Normal = 0,

    /// <summary>
    /// High priority service
    /// </summary>
    High = 1,

    /// <summary>
    /// Emergency repair required
    /// </summary>
    Emergency = 2
}

/// <summary>
/// Represents the type of service being requested
/// </summary>
public enum ServiceType
{
    /// <summary>
    /// Regular maintenance service
    /// </summary>
    Maintenance = 0,

    /// <summary>
    /// Repair of existing problem
    /// </summary>
    Repair = 1,

    /// <summary>
    /// Diagnostic service to identify issues
    /// </summary>
    Diagnostic = 2,

    /// <summary>
    /// Pre-purchase inspection
    /// </summary>
    Inspection = 3,

    /// <summary>
    /// Warranty repair work
    /// </summary>
    Warranty = 4
}
