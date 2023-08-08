using System.Text.Json.Serialization;

namespace CarnalContraption.Bot.Discord.Lovense.API;

internal record GetSocketUrlResponse : Response<GetSocketUrlResponse.SocketUrlData>
{
    public class SocketUrlData
    {
        [JsonPropertyName("socketIoPath")] public string SocketIOPath { get; set; }
        [JsonPropertyName("socketIoUrl")] public string SocketIOUrl { get; set; }
    }
}