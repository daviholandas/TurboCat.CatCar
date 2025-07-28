namespace CatCar.SharedKernel.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    public virtual void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.Now;
    }

    public virtual void Update()
        => UpdatedAt = DateTime.Now;

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Entity) obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}