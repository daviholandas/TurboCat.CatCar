using CatCar.SharedKernel.Common;
using CatCar.FrontOffice.Domain.ValueObjects;

namespace CatCar.FrontOffice.Domain.Entities;

/// <summary>
/// Entity representing a repair quote within a WorkOrder
/// </summary>
public class Quote : Entity
{
    private readonly List<QuoteLineItem> _lineItems = new();

    public Money TotalAmount { get; private set; }
    public Money LaborCost { get; private set; }
    public Money PartsCost { get; private set; }
    public decimal EstimatedHours { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? CustomerSignature { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the line items that make up this quote
    /// </summary>
    public IReadOnlyList<QuoteLineItem> LineItems => _lineItems.AsReadOnly();

    // Private constructor for EF Core
    private Quote() { }

    /// <summary>
    /// Creates a new quote
    /// </summary>
    public Quote(IEnumerable<QuoteLineItem> lineItems, decimal estimatedHours, 
        decimal laborRatePerHour = 150m, int validityDays = 30, string? notes = null)
    {
        var items = lineItems?.ToList() ?? throw new ArgumentNullException(nameof(lineItems));
        
        if (!items.Any())
            throw new ArgumentException("Quote must have at least one line item", nameof(lineItems));

        if (estimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative", nameof(estimatedHours));

        if (laborRatePerHour < 0)
            throw new ArgumentException("Labor rate cannot be negative", nameof(laborRatePerHour));

        _lineItems.AddRange(items);
        EstimatedHours = estimatedHours;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddDays(validityDays);
        Notes = notes;

        // Calculate costs
        PartsCost = CalculatePartsCost();
        LaborCost = new Money(estimatedHours * laborRatePerHour);
        TotalAmount = PartsCost + LaborCost;

        IsApproved = false;
    }

    /// <summary>
    /// Approves the quote with customer signature
    /// </summary>
    public void Approve(string customerSignature, DateTime approvalDate)
    {
        if (string.IsNullOrWhiteSpace(customerSignature))
            throw new ArgumentException("Customer signature is required", nameof(customerSignature));

        if (IsApproved)
            throw new InvalidOperationException("Quote is already approved");

        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationException("Quote has expired and cannot be approved");

        IsApproved = true;
        ApprovedAt = approvalDate;
        CustomerSignature = customerSignature;
        Update();
    }

    /// <summary>
    /// Adds a line item to the quote (only if not approved)
    /// </summary>
    public void AddLineItem(QuoteLineItem lineItem)
    {
        if (lineItem is null)
            throw new ArgumentNullException(nameof(lineItem));

        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved quote");

        _lineItems.Add(lineItem);
        RecalculateTotals();
        Update();
    }

    /// <summary>
    /// Removes a line item from the quote (only if not approved)
    /// </summary>
    public void RemoveLineItem(Guid lineItemId)
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved quote");

        var item = _lineItems.FirstOrDefault(li => li.Id == lineItemId);
        if (item != null)
        {
            _lineItems.Remove(item);
            RecalculateTotals();
            Update();
        }
    }

    /// <summary>
    /// Updates the estimated labor hours (only if not approved)
    /// </summary>
    public void UpdateEstimatedHours(decimal newEstimatedHours, decimal laborRatePerHour = 150m)
    {
        if (newEstimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative", nameof(newEstimatedHours));

        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved quote");

        EstimatedHours = newEstimatedHours;
        LaborCost = new Money(newEstimatedHours * laborRatePerHour);
        TotalAmount = PartsCost + LaborCost;
        Update();
    }

    /// <summary>
    /// Extends the quote expiration date (only if not approved and not expired)
    /// </summary>
    public void ExtendExpiration(int additionalDays)
    {
        if (additionalDays <= 0)
            throw new ArgumentException("Additional days must be positive", nameof(additionalDays));

        if (IsApproved)
            throw new InvalidOperationException("Cannot extend expiration of approved quote");

        ExpiresAt = ExpiresAt.AddDays(additionalDays);
        Update();
    }

    /// <summary>
    /// Checks if the quote has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt && !IsApproved;

    /// <summary>
    /// Gets the number of days until expiration
    /// </summary>
    public int DaysUntilExpiration => (ExpiresAt.Date - DateTime.UtcNow.Date).Days;

    private Money CalculatePartsCost()
    {
        return _lineItems.Aggregate(Money.Zero(), (total, item) => total + item.TotalPrice);
    }

    private void RecalculateTotals()
    {
        PartsCost = CalculatePartsCost();
        TotalAmount = PartsCost + LaborCost;
    }
}

/// <summary>
/// Value object representing a line item in a quote
/// </summary>
public record QuoteLineItem
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Description { get; init; }
    public int Quantity { get; init; }
    public Money UnitPrice { get; init; }
    public Money TotalPrice { get; init; }
    public string? PartNumber { get; init; }
    public bool IsLabor { get; init; }

    public QuoteLineItem(string description, int quantity, Money unitPrice, string? partNumber = null, bool isLabor = false)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        TotalPrice = unitPrice * quantity;
        PartNumber = partNumber?.Trim();
        IsLabor = isLabor;
    }
}
