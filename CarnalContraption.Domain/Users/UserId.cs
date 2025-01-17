using ErrorOr;

namespace CarnalContraption.Domain.Users;

public class UserId : SingleValueObject<UserId, ulong>
{
    public static ErrorOr<UserId> Create(ulong value)
        => new UserId(value);

    protected UserId(ulong value) : base(value)
    {
    }
}