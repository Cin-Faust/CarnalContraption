using ErrorOr;

namespace CarnalContraption.Domain.Lovense;

public class Duration : SingleValueObject<Duration, int>
{
    private const int Minimum = 0;

    public static class Errors
    {
        public static readonly Error OutOfRange = Error.Validation("Duration.OutOfRange", $"Duration must be greater than {Minimum}.");
    }

    public static ErrorOr<Duration> Create(int value)
    {
        if (value is < Minimum) return Errors.OutOfRange;
        return new Duration(value);
    }

    protected Duration(int value) : base(value)
    {
    }
}