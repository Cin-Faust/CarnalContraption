namespace CarnalContraption.Bot.Discord;

internal record DiscordSettings
{
    public string Token { get; init; } = string.Empty;
    public ulong[]? GuildIds { get; init; }
}