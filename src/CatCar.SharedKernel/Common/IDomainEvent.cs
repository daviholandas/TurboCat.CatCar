using System.Diagnostics.CodeAnalysis;

namespace CatCar.SharedKernel.Common;


public abstract class DomainEvent : IEqualityComparer<DomainEvent>
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public double Version { get; init; } = 1.0;

    public bool Equals(DomainEvent? x, DomainEvent? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Id.Equals(y.Id);
    }

    public int GetHashCode([DisallowNull] DomainEvent obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.Id.GetHashCode();
    }
}
