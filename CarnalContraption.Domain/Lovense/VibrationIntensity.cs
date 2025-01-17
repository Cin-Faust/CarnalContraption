using ErrorOr;

namespace CarnalContraption.Domain.Lovense;

public class VibrationIntensity : Intensity
{
    private const int Minimum = 1;
    private const int Maximum = 20;

    public static class Errors
    {
        public static readonly Error OutOfRange = Error.Validation("VibrationIntensity.OutOfRange", $"Intensity must be between {Minimum} and {Maximum}.");
    }

    public static ErrorOr<VibrationIntensity> Create(int value)
    {
        if (value is < Minimum or > Maximum) return Errors.OutOfRange;
        return new VibrationIntensity(value);
    }

    protected VibrationIntensity(int value) : base(value)
    {
    }
}