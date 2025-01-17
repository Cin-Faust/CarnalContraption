using System.Text.Json;
using System.Text.Json.Serialization;
using CarnalContraption.Domain.Lovense;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocketIO.Core;
using SocketIOClient;
using SocketIOClient.Transport;

namespace CarnalContraption.Application.Services.Lovense;

internal class LovenseService(ILogger<LovenseService> logger, IHttpApiClient httpApiClient, IOptions<Settings> settings) : ILovenseService
{
    private const int SuccessCode = 0;
    private const string SendCommandName = "basicapi_send_toy_command_ts";

    public event Func<(ulong UserId, string QuickReadCodeUrl), Task>? OnConnect;
    public event Func<ulong, Task>? OnDisconnect;

    public async Task<ErrorOr<Success>> ConnectAsync(ulong userId)
    {
        try
        {
            if (_sockets.TryGetValue(userId, out var socket) && socket.Connected) return new Success();

            var authorisationResponse = await httpApiClient.PostAsync<AuthorisationRequestDto, ResponseDto<AuthorisationDto>>(settings.Value.UrlGetToken, new AuthorisationRequestDto(settings.Value.Token, userId.ToString()));
            if (authorisationResponse.IsError) return authorisationResponse.Errors;
            if (authorisationResponse.Value.Code != SuccessCode) return Error.Failure("LovenseService.ConnectAsync.AuthorisationFailed", "Failed to Authorise with Lovense.");

            var socketResponse = await httpApiClient.PostAsync<SocketAddressRequestDto, ResponseDto<SocketAddressDto>>(settings.Value.UrlGetSocket, new SocketAddressRequestDto(settings.Value.Platform, authorisationResponse.Value.Data.Token));
            if (socketResponse.IsError) return socketResponse.Errors;
            if (socketResponse.Value.Code != SuccessCode) return Error.Failure("LovenseService.ConnectAsync", "Failed to get Socket Information.");

            var data = socketResponse.Value.Data.Url.Split("ntoken=");

            socket = new SocketIOClient.SocketIO(socketResponse.Value.Data.Url, new SocketIOOptions
            {
                Path = socketResponse.Value.Data.Path,
                Transport = TransportProtocol.WebSocket,
                Reconnection = false,
                ReconnectionAttempts = 1,
                EIO = EngineIO.V3,
                AutoUpgrade = false,
                Query = new[] { new KeyValuePair<string, string>("ntoken", data[1]) }
            });

            socket.On("basicapi_get_qrcode_tc", response =>
            {
                _sockets[userId] = socket;
                var responseData = response.GetValue<string>();
                var quickReadCodeResponse = JsonSerializer.Deserialize<ResponseDto<QuickReadCodeDto>>(responseData);
                OnConnect?.Invoke((userId, quickReadCodeResponse.Data.Url));
            });

            socket.On("basicapi_update_device_info_tc", response =>
            {
                var responseData = response.GetValue<string>();
                var deviceUpdate = JsonSerializer.Deserialize<DeviceUpdateDto>(responseData);
                _toys[userId] = deviceUpdate.Toys.Select(x =>
                {
                    var toyId = ToyId.Create(x.Id).Value;
                    var toyName = ToyName.Create(x.Name).Value;
                    return new Toy(toyId, toyName);
                }).ToList();
            });

            socket.OnConnected += (sender, args) => { socket.EmitAsync("basicapi_get_qrcode_ts", new { ackId = userId.ToString() }).Wait(); };

            socket.OnDisconnected += (sender, s) =>
            {
                _sockets.Remove(userId);
                OnDisconnect?.Invoke(userId);
            };

            await socket.ConnectAsync();
            return new Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message, exception);
            return Error.Unexpected("LovenseService.ConnectAsync.Unexpected", exception.Message);
        }
    }

    public async Task<ErrorOr<IEnumerable<Toy>>> GetToysByUserId(ulong userId)
    {
        if (!_toys.TryGetValue(userId, out var toys) || toys?.Count < 1) Error.Failure(description: "No toys available.");
        return toys ?? [];
    }

    public async Task<ErrorOr<Success>> ControlAllToys(Actions action, Duration duration, Intensity intensity)
    {
        var toyCommand = CreateToyCommand(action, duration, intensity);
        foreach (var entry in _sockets)
        {
            await SendAsync(entry.Value, SendCommandName, toyCommand);
        }

        return new Success();
    }

    public async Task<ErrorOr<Success>> ControlToysByUserIdAsync(ulong userId, Actions action, Duration duration, Intensity intensity)
    {
        if (!_sockets.TryGetValue(userId, out var socket) || !socket.Connected) return Error.Failure(description: "Not connected to lovense.");
        if (!_toys.TryGetValue(userId, out var toys) || toys?.Count < 1) return Error.Failure(description: "No toys available.");
        var toyCommand = CreateToyCommand(action, duration, intensity);
        return await SendAsync(socket, SendCommandName, toyCommand);
    }

    public async Task<ErrorOr<Success>> ControlToysByUserIdAndToyNameAsync(ulong userId, ToyName toyName, Actions action, Duration duration, Intensity intensity)
    {
        if (!_sockets.TryGetValue(userId, out var socket) || !socket.Connected) return Error.Failure(description: "Not connected to lovense.");
        if (!_toys.TryGetValue(userId, out var toys) || toys?.Count < 1) return Error.Failure(description: "No toys available.");
        var toy = toys.FirstOrDefault(toy => toy.Name == toyName);
        if (toy == null) return Error.Failure(description: "No toy with that name.");
        var toyCommand = CreateToyCommand(action, duration, intensity, toy.Id);
        return await SendAsync(socket, SendCommandName, toyCommand);
    }

    private async Task<ErrorOr<Success>> SendAsync<T>(SocketIOClient.SocketIO socket, string name, T request)
    {
        try
        {
            await socket.EmitAsync(name, request);
            return new Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            return Error.Unexpected();
        }
    }

    private static ToyCommandDto CreateToyCommand(Actions action, Duration duration, Intensity intensity, ToyId? toyId = null)
        => new($"{Enum.GetName(action)}:{intensity.Value}", duration.Value, toyId?.Value);

    private readonly Dictionary<ulong, SocketIOClient.SocketIO> _sockets = [];
    private readonly Dictionary<ulong, List<Toy>> _toys = [];

    private record ToyCommandDto(
        [property: JsonPropertyName("action")]
        string Actions,
        [property: JsonPropertyName("timeSec")]
        int Duration,
        [property: JsonPropertyName("toy")]
        string? ToyId)
    {
        [property: JsonPropertyName("command")]
        public string Command => "Function";

        [property: JsonPropertyName("apiVer")]
        public int ApiVersion => 1;
    }

    private record ToyDto(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("toyType")]
        string ToyType,
        [property: JsonPropertyName("nickname")]
        string Alias,
        [property: JsonPropertyName("battery")]
        int Battery
    );

    private record DeviceUpdateDto(
        [property: JsonPropertyName("toyList")]
        ToyDto[] Toys
    );

    private record QuickReadCodeDto(
        [property: JsonPropertyName("qrcode")] string Data,
        [property: JsonPropertyName("qrcodeUrl")]
        string Url);

    private record AuthorisationDto(
        [property: JsonPropertyName("authToken")]
        string Token);

    private record AuthorisationRequestDto(
        [property: JsonPropertyName("token")]
        string Token,
        [property: JsonPropertyName("uid")]
        string UserId);

    private record SocketAddressDto(
        [property: JsonPropertyName("socketIoPath")]
        string Path,
        [property: JsonPropertyName("socketIoUrl")]
        string Url);

    private record SocketAddressRequestDto(
        [property: JsonPropertyName("platform")]
        string Platform,
        [property: JsonPropertyName("authToken")]
        string AuthToken);

    private record ResponseDto<TData>(
        [property: JsonPropertyName("code")]
        int Code,
        [property: JsonPropertyName("message")]
        string Message,
        [property: JsonPropertyName("data")]
        TData Data);
}