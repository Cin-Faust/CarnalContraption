using CarnalContraption.Domain.PiShock;
using CarnalContraption.Domain.PiShock.Requests;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace CarnalContraption.Application.Services.PiShock;

internal class Client(IHttpApiClient httpApiClient, IOptions<Settings> settings) : IClient
{
    private static class Errors
    {
        public static readonly Error UnknownRequest = Error.Unexpected("PiShock.Client.UnknownRequest", "Unknown Request.");
    }

    public async Task<ErrorOr<Success>> SendRequest(User user, Request request)
        => request switch
        {
            ShockRequest shockRequest => await httpApiClient.PostAsync(settings.Value.Url, new
            {
                Username = user.Name.Value,
                ApiKey = user.ApiKey.Value,
                Code = user.ShareCode.Value,
                Name = settings.Value.Name,
                Op = shockRequest.Operation,
                Duration = shockRequest.Duration.Value,
                Intensity = shockRequest.Intensity.Value
            }),
            VibrateRequest vibrateRequest => await httpApiClient.PostAsync(settings.Value.Url, new
            {
                Username = user.Name.Value,
                ApiKey = user.ApiKey.Value,
                Code = user.ShareCode.Value,
                Name = settings.Value.Name,
                Op = vibrateRequest.Operation,
                Duration = vibrateRequest.Duration.Value,
                Intensity = vibrateRequest.Intensity.Value
            }),
            BeepRequest beepRequest => await httpApiClient.PostAsync(settings.Value.Url, new
            {
                Username = user.Name.Value,
                ApiKey = user.ApiKey.Value,
                Code = user.ShareCode.Value,
                Name = settings.Value.Name,
                Op = beepRequest.Operation,
                Duration = beepRequest.Duration.Value
            }),
            _ => Errors.UnknownRequest
        };
}