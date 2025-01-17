using ErrorOr;

namespace CarnalContraption.Domain.Lovense;
public class ToyId : SingleValueObject<ToyId, string>
{
    public static ErrorOr<ToyId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Error.Validation();
        return new ToyId(value);
    }

    protected ToyId(string value) : base(value)
    {

    }
}