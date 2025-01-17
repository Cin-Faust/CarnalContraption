using CarnalContraption.Domain.Users;

namespace CarnalContraption.Domain.PiShock;

public class User(UserId id, Username name, ApiKey apiKey, ShareCode shareCode) : ExternalUser(id)
{
    public Username Name { get; } = name;
    public ApiKey ApiKey { get; } = apiKey;
    public ShareCode ShareCode { get; } = shareCode;
}