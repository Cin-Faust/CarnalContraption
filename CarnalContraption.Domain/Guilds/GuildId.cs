using ErrorOr;

namespace CarnalContraption.Domain.Guilds;

public class GuildId : SingleValueObject<GuildId, ulong>
{
    public static ErrorOr<GuildId> Create(ulong value)
        => new GuildId(value);

    protected GuildId(ulong value) : base(value)
    {
    }
}