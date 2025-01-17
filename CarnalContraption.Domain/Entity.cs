namespace CarnalContraption.Domain;

public abstract class Entity<TId>(TId id) where TId : IEquatable<TId>
{
    public TId Id { get; } = id;

    public static IEqualityComparer<Entity<TId>> IdEqualityComparer =>
        EqualityComparer<Entity<TId>>.Create((left, right)
            => left is null
                ? right is null
                : right is not null && left.GetType() == right.GetType() && left.Id.Equals(right.Id));
}