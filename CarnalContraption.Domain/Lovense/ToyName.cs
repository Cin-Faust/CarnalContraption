using ErrorOr;

namespace CarnalContraption.Domain.Lovense;

public class ToyName : SingleValueObject<ToyName, string>
{
    public static ErrorOr<ToyName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Error.Validation();
        return new ToyName(value);
    }

    protected ToyName(string value) : base(value)
    {

    }
}

