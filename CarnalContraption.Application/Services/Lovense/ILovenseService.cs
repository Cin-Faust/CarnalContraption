using CarnalContraption.Domain.Lovense;
using ErrorOr;

namespace CarnalContraption.Application.Services.Lovense;

public interface ILovenseService
{
    event Func<(ulong UserId, string QuickReadCodeUrl), Task> OnConnect;
    Task<ErrorOr<Success>> ConnectAsync(ulong userId);
    Task<ErrorOr<Success>> ControlAllToys(Actions action, Duration duration, Intensity intensity);
    Task<ErrorOr<Success>> ControlToysByUserIdAsync(ulong userId, Actions action, Duration duration, Intensity intensity);
    Task<ErrorOr<Success>> ControlToysByUserIdAndToyNameAsync(ulong userId, ToyName toyName, Actions action, Duration duration, Intensity intensity);
    Task<ErrorOr<IEnumerable<Toy>>> GetToysByUserId(ulong userId);
}