using System.Data;
using CarnalContraption.Application.Extensions;
using CarnalContraption.Bot.Discord.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using CarnalContraption.Application.Services.Lovense;
using MySql.Data.MySqlClient;

namespace CarnalContraption.Bot.Discord;

internal class Program(
    ILogger<Program> logger,
    DiscordSocketClient discordSocketClient,
    InteractionService interactionService,
    ILovenseService lovenseService,
    IServiceProvider serviceProvider,
    IOptions<DiscordSettings> settings
)
{
    private readonly DiscordSettings _settings = settings.Value;
    private const string ConfigurationFilePath = "appsettings.json";
    private const string ConnectionStringDefault = "Default";
    private const string ConfigurationSectionPiShock = "PiShock";
    private const string ConfigurationSectionLovense = "Lovense";
    private const string ConfigurationSectionDiscord = "Discord";

    private static async Task Main()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile(ConfigurationFilePath);
        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        serviceCollection.AddSingleton<IDbConnection>(new MySqlConnection(configuration.GetConnectionString(ConnectionStringDefault)));
        serviceCollection.AddSingleton<HttpClient>();
        serviceCollection.Configure<Application.Services.PiShock.Settings>(configuration.GetSection(ConfigurationSectionPiShock));
        serviceCollection.Configure<Application.Services.Lovense.Settings>(configuration.GetSection(ConfigurationSectionLovense));
        serviceCollection.AddApplication();

        //Bot
        serviceCollection.Configure<DiscordSettings>(configuration.GetSection(ConfigurationSectionDiscord));
        serviceCollection.AddSingleton<DiscordSocketClient>();
        serviceCollection.AddSingleton<IRestClientProvider>(provider => provider.GetRequiredService<DiscordSocketClient>());
        serviceCollection.AddSingleton<InteractionService>();
        serviceCollection.AddSingleton<Program>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var program = serviceProvider.GetRequiredService<Program>();

        await program.RunAsync();
    }

    public async Task RunAsync()
    {
        try
        {
            discordSocketClient.Log += OnLog;
            discordSocketClient.Ready += OnReady;
            discordSocketClient.InteractionCreated += OnInteractionCreated;

            await discordSocketClient.LoginAsync(TokenType.Bot, _settings.Token);
            await discordSocketClient.StartAsync();

            interactionService.Log += OnLog;
            interactionService.SlashCommandExecuted += OnSlashCommandExecuted;
            lovenseService.OnConnect += OnConnect;

            await interactionService.AddModulesAsync(typeof(Program).Assembly, serviceProvider);

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
        }
    }

    private Task OnSlashCommandExecuted(SlashCommandInfo slashCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        var location = interactionContext.Guild is not null
            ? $"{interactionContext.Guild.Name} ({interactionContext.Guild.Id}) #{interactionContext.Channel.Name} ({interactionContext.Channel.Id})"
            : "private";

        var commandName = slashCommandInfo.Module.IsSlashGroup
            ? $"{slashCommandInfo.Module.SlashGroupName} {slashCommandInfo.Name}"
            : slashCommandInfo.Name;

        var parameters = string.Join(", ", slashCommandInfo.Parameters.Select(x => x.Name));
        logger.LogInformation($"{interactionContext.User.Username} ({interactionContext.User.Id}) in {location} ran /{commandName} {parameters}");
        return Task.CompletedTask;
    }

    private async Task OnConnect((ulong UserId, string QuickReadCodeUrl) arg)
    {
        var user = discordSocketClient.GetUser(arg.UserId);
        await user.SendMessageAsync(embed: new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle("Lovense QR Code")
            .WithDescription("Steps:\n1) Open the lovense app.\n2) Press the + icon in the top right corner.\n3) Select Scan QR.")
            .WithImageUrl(arg.QuickReadCodeUrl)
            .Build());
    }

    private async Task OnInteractionCreated(SocketInteraction socketInteraction)
    {
        var context = new SocketInteractionContext(discordSocketClient, socketInteraction);
        await interactionService.ExecuteCommandAsync(context, serviceProvider);
    }

    private Task OnLog(LogMessage logMessage)
    {
        logger.Log(logMessage.Severity.ToLogLevel(), logMessage.Exception, logMessage.Message);
        return Task.CompletedTask;
    }

    public async Task OnReady()
    {
        if (settings.Value.GuildIds == null)
        {
            await interactionService.RegisterCommandsGloballyAsync();
        }
        else
        {
            foreach (var guildId in settings.Value.GuildIds)
            {
                await interactionService.RegisterCommandsToGuildAsync(guildId);
            }
        }
    }
}