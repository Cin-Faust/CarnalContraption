using CarnalContraption.Domain.Lovense;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Discord;
using Discord.WebSocket;
using System.Net.Sockets;

namespace CarnalContraption.Bot.Discord;

internal class Program
{
    private readonly ILogger<Program> _logger;
    private readonly Settings _settings;
    private readonly DiscordSocketClient _discordClient;
    private readonly LovenseClient _lovenseClient;

    private static void Main()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

        //Bot
        serviceCollection.Configure<Settings>(configuration.GetSection("Discord"));
        serviceCollection.AddSingleton<DiscordSocketClient>();
        serviceCollection.AddSingleton<Program>();

        //Lovense
        serviceCollection.Configure<Domain.Lovense.Settings>(configuration.GetSection("Lovense"));
        serviceCollection.AddSingleton<HttpClient>();
        serviceCollection.AddSingleton<LovenseClient>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var program = serviceProvider.GetService<Program>();

        program.RunAsync().Wait();
    }

    public Program(ILogger<Program> logger, DiscordSocketClient discordClient, LovenseClient lovenseClient, IOptions<Settings> settings)
    {
        _logger = logger;

        _discordClient = discordClient;
        _lovenseClient = lovenseClient;

        _settings = settings.Value;
    }

    public async Task RunAsync()
    {
        try
        {
            _discordClient.Connected += Connected;
            _discordClient.Disconnected += Disconnected;
            _discordClient.Ready += Ready;
            _discordClient.SlashCommandExecuted += SlashCommandHandler;
            await _discordClient.LoginAsync(TokenType.Bot, _settings.Token);
            await _discordClient.StartAsync();

            await Task.Delay(-1);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }

    public async Task Connected()
    {
        _logger.LogInformation("Connected.");
    }

    public async Task Disconnected(Exception exception)
    {
        _logger.LogError(exception, "Disconnected.");
    }

    public async Task Ready()
    {
        _logger.LogInformation("Ready.");
        foreach (var clientGuild in _discordClient.Guilds)
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("connect");
            guildCommand.WithDescription($"Connect lovense.");
            try
            {
                await clientGuild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("vibrate");
            guildCommand.WithDescription($"Vibrates all connected toys.");
            guildCommand.AddOption("user", ApplicationCommandOptionType.User, "The specific user.");
            guildCommand.AddOption("intensity", ApplicationCommandOptionType.Integer, "How intense the vibration from 0-20.");
            guildCommand.AddOption("duration", ApplicationCommandOptionType.Integer, "How long the vibration in seconds from 1-60.");

            try
            {
                await clientGuild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        _logger.LogInformation($"Command '{command.Data.Name}' executed by {(command.User.IsBot ? "Bot" : "User")} '{command.User.GlobalName}' ({command.User.Id}).");

        try
        {
            if (command.Data.Name.Equals("connect", StringComparison.OrdinalIgnoreCase))
            {
                await command.RespondAsync($"Sending you your unique QR Code.");
                var session = await _lovenseClient.ConnectUser(command.User.Id);
                await command.User.SendMessageAsync($"To connect, scan your unique QR code in lovense. {session.QRCodeUrl}");
            }
            else if (command.Data.Name.Equals("vibrate", StringComparison.OrdinalIgnoreCase))
            {
                var intensity = 10;
                var duration = 1;
                SocketGuildUser? user = null;

                foreach (var option in command.Data.Options)
                {
                    switch (option.Name)
                    {
                        case "intensity":
                            int.TryParse(option.Value.ToString(), out intensity);
                            break;
                        case "duration":
                            int.TryParse(option.Value.ToString(), out duration);
                            break;
                        case "user":
                            user = option.Value as SocketGuildUser;
                            break;
                    }
                }

                if (intensity < 0) intensity = 0;
                if (intensity > 20) intensity = 20;

                if (duration < 1) duration = 1;
                if (duration > 60) duration = 60;

                if (user == null)
                {
                    await command.RespondAsync($"Vibrating all toys at intensity {intensity} for {duration} seconds.");
                    foreach (var session in _lovenseClient.Sessions)
                    {
                        await session.VibrateAsync(intensity, duration);
                    }
                }
                else
                {
                    var userSession = _lovenseClient.Sessions.FirstOrDefault(session => session.UserId == user.Id);
                    if (userSession == null)
                    {
                        await command.RespondAsync($"User {user.DisplayName} is not connected.");
                    }
                    else
                    {
                        await command.RespondAsync($"Vibrating all {user.DisplayName} toys at intensity {intensity} for {duration} seconds.");
                        await userSession.VibrateAsync(intensity, duration);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }
}