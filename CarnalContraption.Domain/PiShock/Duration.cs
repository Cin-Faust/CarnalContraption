using ErrorOr;

namespace CarnalContraption.Domain.PiShock;

public class Duration : SingleValueObject<Duration, int>
{
    public static class Errors
    {
        public static readonly Error OutOfRange = Error.Validation("Duration.OutOfRange", "Duration must be between 1 and 15.");
    }

    public static ErrorOr<Duration> Create(int value)
    {
        if (value is < 1 or > 15) return Errors.OutOfRange;
        return new Duration(value);
    }

    protected Duration(int value) : base(value)
    {
    }
}