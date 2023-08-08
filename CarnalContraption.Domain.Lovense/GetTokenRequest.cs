using System.Text.Json.Serialization;

namespace CarnalContraption.Bot.Discord.Lovense.API;

internal record GetTokenRequest : IRequest<GetTokenResponse, GetTokenResponse.TokenData>
{
    public GetTokenRequest(string token, string uId, string uName, string uToken)
    {
        Token = token;
        UId = uId;
        UName = uName;
        UToken = uToken;
    }

    [JsonPropertyName("token")] public string Token { get; init; }
    [JsonPropertyName("uid")] public string UId { get; init; }
    [JsonPropertyName("uname")] public string UName { get; init; }
    [JsonPropertyName("utoken")] public string UToken { get; init; }
}