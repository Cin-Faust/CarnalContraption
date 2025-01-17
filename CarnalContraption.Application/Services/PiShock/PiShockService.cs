using CarnalContraption.Domain.PiShock;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarnalContraption.Application.Services.PiShock;

internal class PiShockService(ILogger<PiShockService> logger, IHttpApiClient httpApiClient, IOptions<Settings> settings) : IPiShockService
{
    private const int OperationShock = 0;
    private const int OperationVibrate = 1;
    private const int OperationBeep = 2;

    abstract record Request(string Username, string ApiKey, string Code, string Name, int Op);

    record ShockRequest(string Username, string ApiKey, string Code, string Name, int Duration, int Intensity) : Request(Username, ApiKey, Code, Name, OperationShock);

    record VibrateRequest(string Username, string ApiKey, string Code, string Name, int Duration, int Intensity) : Request(Username, ApiKey, Code, Name, OperationVibrate);

    record BeepRequest(string Username, string ApiKey, string Code, string Name, int Duration) : Request(Username, ApiKey, Code, Name, OperationBeep);

    public Task<ErrorOr<Success>> ShockAsync(User user, Duration duration, Intensity intensity)
        => httpApiClient.PostAsync(settings.Value.Url, new ShockRequest(user.Name, user.ApiKey, user.Code, settings.Value.Name, duration, intensity));

    public Task<ErrorOr<Success>> VibrateAsync(User user, Duration duration, Intensity intensity)
        => httpApiClient.PostAsync(settings.Value.Url, new VibrateRequest(user.Name, user.ApiKey, user.Code, settings.Value.Name, duration, intensity));

    public Task<ErrorOr<Success>> BeepAsync(User user, Duration duration)
        => httpApiClient.PostAsync(settings.Value.Url, new BeepRequest(user.Name, user.ApiKey, user.Code, settings.Value.Name, duration));
}