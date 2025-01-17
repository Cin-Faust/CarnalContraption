using System.Text.Json;
using CarnalContraption.Application.Services.PiShock;
using CarnalContraption.Domain.PiShock;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.PiShock;

public record BeepCommand(ulong TargetUserId, int Duration) : IRequest<ErrorOr<Success>>;

internal class BeepCommandHandler(IPiShockService piShockService) : IRequestHandler<BeepCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(BeepCommand request, CancellationToken cancellationToken)
    {
        var durationResult = Duration.Create(request.Duration);
        if (durationResult.IsError) return durationResult.Errors;

        var users = JsonSerializer.Deserialize<List<User>>(File.OpenRead("users.json"));
        var user = users.FirstOrDefault(x => x.Id == request.TargetUserId);

        if (user == null) return Error.NotFound(description: "User not registered.");

        return await piShockService.BeepAsync(user, durationResult.Value);
    }
}