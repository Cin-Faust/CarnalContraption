namespace CarnalContraption.Domain;

public abstract class ValueObject<TValueObject> : IEquatable<TValueObject> where TValueObject : ValueObject<TValueObject>, IEquatable<TValueObject>
{
    private const string Delimiter = ", ";

    public bool Equals(TValueObject? other)
        => other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj)
        => Equals(obj as TValueObject);

    public override int GetHashCode()
        => GetEqualityComponents().Select(x => x?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);

    public override string ToString()
        => string.Join(Delimiter, GetEqualityComponents());

    public static bool operator ==(ValueObject<TValueObject>? left, ValueObject<TValueObject> right)
        => Equals(left, right);

    public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
        => !Equals(left, right);

    protected abstract IEnumerable<object?> GetEqualityComponents();
}