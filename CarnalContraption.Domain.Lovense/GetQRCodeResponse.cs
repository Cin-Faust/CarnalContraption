using System.Text.Json.Serialization;

namespace CarnalContraption.Bot.Discord.Lovense.API;

internal record GetQRCodeResponse : Response<GetQRCodeResponse.QRData>
{
    public record QRData
    {
        [JsonPropertyName("qrcode")] public string QRCode { get; set; }

        [JsonPropertyName("qrcodeUrl")] public string QRCodeUrl { get; set; }

        [JsonPropertyName("ackId")] public string AckId { get; set; }
    }
}