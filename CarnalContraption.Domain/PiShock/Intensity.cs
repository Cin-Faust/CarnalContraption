using ErrorOr;

namespace CarnalContraption.Domain.PiShock;

public class Intensity : SingleValueObject<Intensity, int>
{
    public static class Errors
    {
        public static readonly Error OutOfRange = Error.Validation("Intensity.OutOfRange", "Intensity must be between 1 and 15.");
    }

    public static ErrorOr<Intensity> Create(int value)
    {
        if (value is < 1 or > 100) return Errors.OutOfRange;
        return new Intensity(value);
    }

    protected Intensity(int value) : base(value)
    {
    }
}