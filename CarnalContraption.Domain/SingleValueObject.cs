namespace CarnalContraption.Domain;

public abstract class SingleValueObject<TValueObject, TValue>(TValue value) : ValueObject<TValueObject> where TValueObject : ValueObject<TValueObject>
{
    public TValue Value { get; } = value;

    public static implicit operator TValue(SingleValueObject<TValueObject, TValue> valueObject)
        => valueObject.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}