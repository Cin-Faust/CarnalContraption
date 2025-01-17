using CarnalContraption.Application.Services.Lovense;
using CarnalContraption.Domain.Lovense;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.Lovense;

public record ControlCommand(Actions Action, int Duration, int Intensity, ulong? TargetUserId = null, string? ToyName = null) : IRequest<ErrorOr<Success>>;

internal class ControlCommandHandler(ILovenseService lovenseService) : IRequestHandler<ControlCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ControlCommand request, CancellationToken cancellationToken)
    {
        var intensityResult = Intensity.Create(request.Action, request.Intensity);
        if (intensityResult.IsError) return intensityResult.Errors;

        var durationResult = Duration.Create(request.Duration);
        if (durationResult.IsError) return durationResult.Errors;

        switch (request)
        {
            case { TargetUserId: null, ToyName: null }:
                return await lovenseService.ControlAllToys(request.Action, durationResult.Value, intensityResult.Value);
            case { TargetUserId: null, ToyName: not null }:
                return Error.Validation(description: "Must supply a target user.");
            case { TargetUserId: not null, ToyName: null }:
                return await lovenseService.ControlToysByUserIdAsync(request.TargetUserId.Value, request.Action, durationResult.Value, intensityResult.Value);
            case { TargetUserId: not null, ToyName: not null }:
            {
                var toyNameResult = ToyName.Create(request.ToyName);
                if (toyNameResult.IsError) return toyNameResult.Errors;
                return await lovenseService.ControlToysByUserIdAndToyNameAsync(request.TargetUserId.Value, toyNameResult.Value, request.Action, durationResult.Value, intensityResult.Value);
            }
            default:
                return Error.Unexpected(description: "Command could not be understood.");
        }
    }
}