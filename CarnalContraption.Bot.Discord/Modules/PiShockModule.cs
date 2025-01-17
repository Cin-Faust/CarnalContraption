using CarnalContraption.Application.PiShock;
using CarnalContraption.Domain.PiShock;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;

namespace CarnalContraption.Bot.Discord.Modules;

[Group("pishock", "Controls pishock devices.")]
public class PiShockModule(ISender sender) : BaseModule(sender)
{
    [SlashCommand("register", "Register your account.")]
    public Task RegisterAsync(string username, string apiKey, string code)
        => ExecuteAsync(new RegisterCommand(Context.User.Id, username, apiKey, code));

    [SlashCommand("connect", "Allows users of the guild to control your device.")]
    public Task ConnectAsync()
        => ExecuteAsync(new ConnectCommand(Context.User.Id, Context.Guild.Id));

    [SlashCommand("disconnect", "Prevents users of the guild from controlling your device.")]
    public Task DisconnectAsync()
        => ExecuteAsync(new DisconnectCommand(Context.User.Id, Context.Guild.Id));

    [SlashCommand("shock", "Shocks a pishock.")]
    public Task ShockAsync(SocketUser targetUser, int duration = 1, int intensity = 1)
        => ExecuteAsync(new ControlCommand(Context.User.Id, Context.Guild?.Id, targetUser.Id, Operations.Shock, duration, intensity));

    [SlashCommand("vibrate", "Vibrates a pishock.")]
    public Task VibrateAsync(SocketUser targetUser, int duration = 1, int intensity = 1)
        => ExecuteAsync(new ControlCommand(Context.User.Id, Context.Guild?.Id, targetUser.Id, Operations.Vibrate, duration, intensity));

    [SlashCommand("beep", "Beeps a pishock.")]
    public Task BeepAsync(SocketUser targetUser, int duration = 1)
        => ExecuteAsync(new ControlCommand(Context.User.Id, Context.Guild?.Id, targetUser.Id, Operations.Beep, duration));
}