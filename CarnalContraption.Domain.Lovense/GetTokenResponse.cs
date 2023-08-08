using System.Text.Json.Serialization;

namespace CarnalContraption.Bot.Discord.Lovense.API;

internal record GetTokenResponse : Response<GetTokenResponse.TokenData>
{
    public record TokenData
    {
        [JsonPropertyName("authToken")] public string AuthToken { get; set; }
    }
}