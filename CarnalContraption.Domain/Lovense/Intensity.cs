using ErrorOr;

namespace CarnalContraption.Domain.Lovense;

public abstract class Intensity(int value) : SingleValueObject<Intensity, int>(value)
{
    public static ErrorOr<Intensity> Create(Actions action, int value)
        => action switch
        {
            Actions.Stop => new NoIntensity(),
            Actions.Vibrate => Convert(VibrationIntensity.Create(value)),
            _ => Error.Failure(description: "Action not currently supported.")
        };

    protected static ErrorOr<Intensity> Convert<T>(ErrorOr<T> result) where T : Intensity
        => result.IsError
            ? result.Errors
            : result.Value;
}