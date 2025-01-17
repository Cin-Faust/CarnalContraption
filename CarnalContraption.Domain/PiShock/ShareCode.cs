using ErrorOr;

namespace CarnalContraption.Domain.PiShock;

public class ShareCode : SingleValueObject<ShareCode, string>
{
    private static class Errors
    {
        public static readonly Error IsEmpty = Error.Validation("PiShock.ShareCode.IsEmpty", "ShareCode cannot be empty");
    }

    public static ErrorOr<ShareCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Errors.IsEmpty;
        return new ShareCode(value);
    }

    protected ShareCode(string value) : base(value)
    {
    }
}