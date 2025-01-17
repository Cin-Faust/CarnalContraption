using CarnalContraption.Domain.Guilds;
using CarnalContraption.Domain.Users;
using ErrorOr;

namespace CarnalContraption.Application.Storage.PiShock;

public interface IGuildUserRepository
{
    Task<ErrorOr<Success>> AddUserToGuildAsync(UserId userId, GuildId guildId);
    Task<ErrorOr<Success>> RemoveUserFromGuildAsync(UserId userId, GuildId guildId);
    Task<ErrorOr<bool>> IsUserInGuildAsync(UserId userId, GuildId guildId);
}