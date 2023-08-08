using CarnalContraption.Bot.Discord.Lovense.API;
using SocketIOClient;
using SocketIOClient.Transport;
using System.Text.Json;

namespace CarnalContraption.Domain.Lovense;

public class Session
{
    private readonly SemaphoreSlim _semaphore = new(0);
    private readonly string _socketUrl;
    private readonly string _socketPath;
    private SocketIO? _socket;

    public Session(ulong userId, string socketUrl, string socketPath)
    {
        UserId = userId;
        _socketUrl = socketUrl;
        _socketPath = socketPath;
    }

    public ulong UserId { get; }
    public string? QRCodeUrl { get; private set; } = null;

    public async Task StartAsync()
    {
        if (_socket != null) return;

        var data = _socketUrl.Split("ntoken=");

        _socket = new SocketIO(_socketUrl, new SocketIOOptions
        {
            Path = _socketPath,
            Transport = TransportProtocol.WebSocket,
            Reconnection = false,
            ReconnectionAttempts = 1,
            EIO = EngineIO.V3,
            AutoUpgrade = false,
            Query = new[] { new KeyValuePair<string, string>("ntoken", data[1]) }
        });

        _socket.On("basicapi_get_qrcode_tc", response =>
        {
            var getQRCodeResponse = JsonSerializer.Deserialize<GetQRCodeResponse>(response.GetValue<string>());
            QRCodeUrl = getQRCodeResponse.Data.QRCodeUrl;
            _semaphore.Release(1);
        });

        _socket.OnConnected += (sender, args) => { _socket.EmitAsync("basicapi_get_qrcode_ts", new { ackId = UserId.ToString() }).Wait(); };

        await _socket.ConnectAsync();
        await _semaphore.WaitAsync();
    }

    public Task StopAsync()
        => _socket?.DisconnectAsync() ?? Task.CompletedTask;

    public Task VibrateAsync(int intensity, int duration)
        => _socket?.EmitAsync("basicapi_send_toy_command_ts", new { command = "Function", action = $"Vibrate:{intensity}", timeSec = duration, apiVer = 1 }) ?? Task.CompletedTask;
}