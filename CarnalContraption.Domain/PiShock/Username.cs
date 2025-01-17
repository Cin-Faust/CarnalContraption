using ErrorOr;

namespace CarnalContraption.Domain.PiShock;

public class Username : SingleValueObject<Username, string>
{
    private static class Errors
    {
        public static readonly Error IsEmpty = Error.Validation("PiShock.Username.IsEmpty", "Username cannot be empty");
    }

    public static ErrorOr<Username> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Errors.IsEmpty;
        return new Username(value);
    }

    protected Username(string value) : base(value)
    {
    }
}