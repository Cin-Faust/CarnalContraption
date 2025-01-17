using CarnalContraption.Application.Services.PiShock;
using CarnalContraption.Domain.PiShock;
using ErrorOr;
using MediatR;
using System.Text.Json;

namespace CarnalContraption.Application.PiShock;

public record VibrateCommand(ulong TargetUserId, int Duration, int Intensity) : IRequest<ErrorOr<Success>>;

internal class VibrateCommandHandler(IPiShockService piShockService) : IRequestHandler<VibrateCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(VibrateCommand request, CancellationToken cancellationToken)
    {
        var durationResult = Duration.Create(request.Duration);
        if (durationResult.IsError) return durationResult.Errors;

        var intensityResult = Intensity.Create(request.Intensity);
        if (intensityResult.IsError) return intensityResult.Errors;

        var users = JsonSerializer.Deserialize<List<User>>(File.OpenRead("users.json"));
        var user = users.FirstOrDefault(x => x.Id == request.TargetUserId);

        if (user == null) return Error.NotFound(description: "User not registered.");

        return await piShockService.VibrateAsync(user, durationResult.Value, intensityResult.Value);
    }
}