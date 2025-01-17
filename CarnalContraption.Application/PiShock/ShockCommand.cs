using System.Text.Json;
using CarnalContraption.Application.Services.PiShock;
using CarnalContraption.Domain.PiShock;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Options;

namespace CarnalContraption.Application.PiShock;

public record ShockCommand(ulong TargetUserId, int Duration, int Intensity) : IRequest<ErrorOr<Success>>;

internal class ShockCommandHandler(IPiShockService piShockService) : IRequestHandler<ShockCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ShockCommand request, CancellationToken cancellationToken)
    {
        var durationResult = Duration.Create(request.Duration);
        if (durationResult.IsError) return durationResult.Errors;

        var intensityResult = Intensity.Create(request.Intensity);
        if (intensityResult.IsError) return intensityResult.Errors;

        var users = JsonSerializer.Deserialize<List<User>>(File.OpenRead("users.json"));
        var user = users.FirstOrDefault(x => x.Id == request.TargetUserId);

        if (user == null) return Error.NotFound(description: "User not registered.");

        return await piShockService.ShockAsync(user, durationResult.Value, intensityResult.Value);
    }
}