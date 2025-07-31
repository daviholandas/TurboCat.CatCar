using CatCar.FrontOffice.Domain.Entities;
using CatCar.FrontOffice.Domain.ValueObjects;
using CatCar.FrontOffice.Domain.Repositories;

namespace CatCar.FrontOffice.Domain.Services;

/// <summary>
/// Domain service for pricing calculations and quote generation
/// </summary>
public class QuotePricingService
{
    private readonly Dictionary<string, decimal> _laborRates;
    private readonly Dictionary<string, decimal> _markupRates;

    public QuotePricingService()
    {
        // Default labor rates (could be configured externally)
        _laborRates = new Dictionary<string, decimal>
        {
            { "Standard", 120m },
            { "Diagnostic", 150m },
            { "Specialist", 180m },
            { "Emergency", 220m }
        };

        // Default markup rates for parts
        _markupRates = new Dictionary<string, decimal>
        {
            { "Standard", 1.25m }, // 25% markup
            { "OEM", 1.15m },      // 15% markup
            { "Aftermarket", 1.35m } // 35% markup
        };
    }

    /// <summary>
    /// Calculates labor cost based on service type and estimated hours
    /// </summary>
    public Money CalculateLaborCost(string serviceCategory, decimal hours, string currency = "BRL")
    {
        var rate = _laborRates.GetValueOrDefault(serviceCategory, _laborRates["Standard"]);
        return new Money(rate * hours, currency);
    }

    /// <summary>
    /// Calculates part cost with appropriate markup
    /// </summary>
    public Money CalculatePartCost(Money baseCost, string partCategory)
    {
        var markup = _markupRates.GetValueOrDefault(partCategory, _markupRates["Standard"]);
        return baseCost * markup;
    }

    /// <summary>
    /// Generates a comprehensive quote with labor and parts
    /// </summary>
    public Quote GenerateQuote(IEnumerable<ServiceItem> serviceItems, decimal estimatedHours, 
        string serviceCategory = "Standard", int validityDays = 30)
    {
        var lineItems = new List<QuoteLineItem>();

        // Add labor line item if there are estimated hours
        if (estimatedHours > 0)
        {
            var laborCost = CalculateLaborCost(serviceCategory, estimatedHours);
            lineItems.Add(new QuoteLineItem(
                $"Labor - {serviceCategory} ({estimatedHours:F1} hours)",
                1,
                laborCost,
                isLabor: true));
        }

        // Add parts line items
        foreach (var item in serviceItems)
        {
            var partCost = CalculatePartCost(item.UnitCost, item.Category);
            lineItems.Add(new QuoteLineItem(
                item.Description,
                item.Quantity,
                partCost,
                item.PartNumber));
        }

        return new Quote(lineItems, estimatedHours, 
            _laborRates.GetValueOrDefault(serviceCategory, _laborRates["Standard"]), 
            validityDays);
    }

    /// <summary>
    /// Calculates discount amount for approved customers or bulk services
    /// </summary>
    public Money CalculateDiscount(Money totalAmount, string discountType, decimal discountValue)
    {
        return discountType.ToLowerInvariant() switch
        {
            "percentage" => totalAmount * (discountValue / 100m),
            "fixed" => new Money(discountValue, totalAmount.Currency),
            _ => Money.Zero(totalAmount.Currency)
        };
    }
}

/// <summary>
/// Represents a service item for quote generation
/// </summary>
public record ServiceItem
{
    public string Description { get; init; }
    public string Category { get; init; }
    public int Quantity { get; init; }
    public Money UnitCost { get; init; }
    public string? PartNumber { get; init; }

    public ServiceItem(string description, string category, int quantity, Money unitCost, string? partNumber = null)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive");
        UnitCost = unitCost ?? throw new ArgumentNullException(nameof(unitCost));
        PartNumber = partNumber;
    }
}
