namespace CatCar.SharedKernel.Common;


public abstract class AggregateRoot : Entity, IAggregateRoot
{
    private readonly HashSet<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));

        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public bool HasDomainEvents => _domainEvents.Count > 0;
}
