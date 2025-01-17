using CarnalContraption.Domain.PiShock;
using ErrorOr;

namespace CarnalContraption.Application.Services.PiShock;

public interface IPiShockService
{
    Task<ErrorOr<Success>> ShockAsync(User user, Duration duration, Intensity intensity);
    Task<ErrorOr<Success>> VibrateAsync(User user, Duration duration, Intensity intensity);
    Task<ErrorOr<Success>> BeepAsync(User user, Duration duration);
}