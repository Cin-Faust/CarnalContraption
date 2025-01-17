using ErrorOr;

namespace CarnalContraption.Domain.PiShock;

public class ApiKey : SingleValueObject<ApiKey, string>
{
    private static class Errors
    {
        public static readonly Error IsEmpty = Error.Validation("PiShock.ApiKey.IsEmpty", "ApiKey cannot be empty");
    }

    public static ErrorOr<ApiKey> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return ApiKey.Errors.IsEmpty;
        return new ApiKey(value);
    }

    protected ApiKey(string value) : base(value)
    {
    }
}