using System.Net.Http.Json;
using CarnalContraption.Bot.Discord.Lovense.API;
using Microsoft.Extensions.Options;

namespace CarnalContraption.Domain.Lovense;

public class LovenseClient
{
    private const int SuccessCode = 0;

    private readonly Settings _settings;
    private readonly HttpClient _httpClient;

    public List<Session> Sessions { get; } = new();

    public LovenseClient(HttpClient httpClient, IOptions<Settings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<Session> ConnectUser(ulong userId)
    {
        var session = Sessions.FirstOrDefault(s => s.UserId == userId);
        if (session != null) return session;

        var uniqueIdentifier = userId.ToString();

        var getTokenResponse = await PostAsync<GetTokenRequest, GetTokenResponse, GetTokenResponse.TokenData>(
            "https://api.lovense-api.com/api/basicApi/getToken",
            new GetTokenRequest(
                _settings.Token,
                uniqueIdentifier,
                uniqueIdentifier,
                uniqueIdentifier
            ));

        if (getTokenResponse.Code != SuccessCode)
        {
            throw new Exception($"Error getting token. {getTokenResponse.Message}");
        }

        var getSocketUrlResponse = await PostAsync<GetSocketUrlRequest, GetSocketUrlResponse, GetSocketUrlResponse.SocketUrlData>("https://api.lovense-api.com/api/basicApi/getSocketUrl",
            new GetSocketUrlRequest(_settings.Platform, getTokenResponse.Data.AuthToken));

        if (getSocketUrlResponse.Code != SuccessCode)
        {
            throw new Exception($"Error getting token. {getSocketUrlResponse.Message}");
        }

        session = new Session(userId, getSocketUrlResponse.Data.SocketIOUrl, getSocketUrlResponse.Data.SocketIOPath);
        Sessions.Add(session);
        await session.StartAsync();

        return session;
    }

    public async Task DisconnectUser(ulong userId)
    {
        var userSession = Sessions.SingleOrDefault(session => session.UserId == userId);
        if (userSession != null)
        {
            await userSession.StopAsync();
        }
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse, TData>(string uri, TRequest request) where TRequest : IRequest<TResponse, TData> where TResponse : Response<TData>
    {
        var httpResponse = await _httpClient.PostAsJsonAsync(uri, request);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }
}