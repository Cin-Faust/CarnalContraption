using CarnalContraption.Application.Services.PiShock;
using CarnalContraption.Application.Storage.PiShock;
using CarnalContraption.Domain.Guilds;
using CarnalContraption.Domain.PiShock;
using CarnalContraption.Domain.PiShock.Requests;
using CarnalContraption.Domain.Users;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.PiShock;

public record ControlCommand(ulong UserId, ulong? GuildId, ulong TargetUserId, Operations Operation, int Duration = 1, int Intensity = 1) : UserRequest<Success>(UserId);

internal class ControlCommandHandler(IClient client, IUserRepository userRepository, IGuildUserRepository guildUserRepository) : IRequestHandler<ControlCommand, ErrorOr<Success>>
{
    private const int None = 0;

    private static class Errors
    {
        public static readonly Error UnknownRequest = Error.Unexpected("PiShock.ControlCommand.UnknownRequest", "Unknown Request.");
    }

    public async Task<ErrorOr<Success>> Handle(ControlCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsError) return userIdResult.Errors;

        var targetUserIdResult = UserId.Create(request.TargetUserId);
        if (targetUserIdResult.IsError) return targetUserIdResult.Errors;

        if (userIdResult.Value == targetUserIdResult.Value)
        {
        }
        else if (request.GuildId.HasValue)
        {
            var guildIdResult = GuildId.Create(request.GuildId.Value);
            if (guildIdResult.IsError) return guildIdResult.Errors;

            var allowedResult = await guildUserRepository.IsUserInGuildAsync(targetUserIdResult.Value, guildIdResult.Value);
            if (allowedResult.IsError) return allowedResult.Errors;

            if (!allowedResult.Value) return Error.Forbidden(description: "User is not connected on this server.");
        }
        else
        {
            return Error.Failure(description: "Cannot run the command from here.");
        }

        var errors = new List<Error>();

        var durationResult = Duration.Create(request.Duration);
        if (durationResult.IsError) errors.AddRange(durationResult.Errors);

        var intensityResult = Intensity.Create(request.Intensity);
        if (intensityResult.IsError) errors.AddRange(intensityResult.Errors);

        if (errors.Count > None) return errors;

        var requestResult = CreateRequest(request.Operation, durationResult.Value, intensityResult.Value);
        if (requestResult.IsError) return requestResult.Errors;

        return await userRepository.RetrieveByIdAsync(targetUserIdResult.Value)
            .ThenAsync(user => client.SendRequest(user, requestResult.Value));
    }

    private static ErrorOr<Request> CreateRequest(Operations operation, Duration duration, Intensity intensity)
        => operation switch
        {
            Operations.Shock => new ShockRequest(duration, intensity),
            Operations.Vibrate => new VibrateRequest(duration, intensity),
            Operations.Beep => new BeepRequest(duration),
            _ => Errors.UnknownRequest
        };
}