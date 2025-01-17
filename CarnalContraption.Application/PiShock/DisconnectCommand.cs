using CarnalContraption.Application.Storage.PiShock;
using CarnalContraption.Domain.Guilds;
using CarnalContraption.Domain.Users;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.PiShock;

public record DisconnectCommand(ulong UserId, ulong GuildId) : GuildUserRequest<Success>(UserId, GuildId);

internal class DisconnectCommandHandler(IGuildUserRepository repository) : IRequestHandler<DisconnectCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DisconnectCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsError) return userIdResult.Errors;

        var guildIdResult = GuildId.Create(request.GuildId);
        if (guildIdResult.IsError) return guildIdResult.Errors;

        return await repository.RemoveUserFromGuildAsync(userIdResult.Value, guildIdResult.Value);
    }
}