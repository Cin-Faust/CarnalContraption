using CarnalContraption.Application.PiShock;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;

namespace CarnalContraption.Bot.Discord.Modules;

[Group("pishock", "Controls pishock devices.")]
public class PiShockModule(ISender sender) : BaseModule(sender)
{
    [SlashCommand("shock", "Shocks a pishock.")]
    public Task ShockAsync(SocketUser targetUser, int duration = 1, int intensity = 1)
        => ExecuteAsync(new ShockCommand(targetUser.Id, duration, intensity));

    [SlashCommand("vibrate", "Vibrates a pishock.")]
    public Task VibrateAsync(SocketUser targetUser, int duration = 1, int intensity = 1)
        => ExecuteAsync(new VibrateCommand(targetUser.Id, duration, intensity));

    [SlashCommand("beep", "Beeps a pishock.")]
    public Task BeepAsync(SocketUser targetUser, int duration = 1)
        => ExecuteAsync(new BeepCommand(targetUser.Id, duration));

    [SlashCommand("register", "Register your account.")]
    public Task RegisterAsync(string username, string apiKey, string code)
        => ExecuteAsync(new RegisterCommand(Context.User.Id, username, apiKey, code));
}