using CarnalContraption.Application.Storage.PiShock;
using CarnalContraption.Domain.Guilds;
using CarnalContraption.Domain.Users;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.PiShock;

public record ConnectCommand(ulong UserId, ulong GuildId) : GuildUserRequest<Success>(UserId, GuildId);

internal class ConnectCommandHandler(IGuildUserRepository repository) : IRequestHandler<ConnectCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ConnectCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsError) return userIdResult.Errors;

        var guildIdResult = GuildId.Create(request.GuildId);
        if (guildIdResult.IsError) return guildIdResult.Errors;

        return await repository.AddUserToGuildAsync(userIdResult.Value, guildIdResult.Value);
    }
}