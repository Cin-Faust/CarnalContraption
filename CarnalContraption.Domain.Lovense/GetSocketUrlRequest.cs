using System.Text.Json.Serialization;

namespace CarnalContraption.Bot.Discord.Lovense.API;

internal record GetSocketUrlRequest : IRequest<GetSocketUrlResponse, GetSocketUrlResponse.SocketUrlData>
{
    public GetSocketUrlRequest(string platform, string authToken)
    {
        Platform = platform;
        AuthToken = authToken;
    }

    [JsonPropertyName("platform")] public string Platform { get; init; }
    [JsonPropertyName("authToken")] public string AuthToken { get; init; }
}