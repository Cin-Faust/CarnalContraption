using Discord.Interactions;
using CarnalContraption.Application.Lovense;
using CarnalContraption.Domain.Lovense;
using Discord;
using Discord.WebSocket;
using MediatR;

namespace CarnalContraption.Bot.Discord.Modules;

[Group("lovense", "Controls lovense toys.")]
public class LovenseModule(ISender sender) : BaseModule(sender)
{
    [SlashCommand("connect", "Connects your lovense toys.")]
    public Task ConnectAsync()
        => ExecuteAsync(new ConnectCommand(Context.User.Id));

    [SlashCommand("control", "Controls lovense toy(s).")]
    public Task ControlAsync(Actions action, SocketUser? targetUser = null, [Autocomplete(typeof(ToyNameAutoCompleteHandler))] string? toyName = null, int duration = 1, int intensity = 1)
        => ExecuteAsync(new ControlCommand(action, duration, intensity, targetUser?.Id, toyName));

    [SlashCommand("vibrate", "Vibrates the user toy(s).")]
    public Task VibrateAsync(SocketUser? targetUser = null, [Autocomplete(typeof(ToyNameAutoCompleteHandler))] string? toyName = null, int duration = 1, int intensity = 1)
        => ExecuteAsync(new ControlCommand(Actions.Vibrate, duration, intensity, targetUser?.Id, toyName));

    [SlashCommand("toys", "Lists the user's toy(s).")]
    public Task ToysAsync(SocketUser targetUser)
        => ExecuteAsync(new GetToysByUserIdQuery(targetUser.Id), toys => string.Join("\n", toys.Select(toy => toy.Name.Value)));
}

public class ToyNameAutoCompleteHandler(ISender sender) : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var data = context.Interaction.Data as SocketAutocompleteInteractionData;
        if (data == null) return AutocompletionResult.FromSuccess();

        var option = data.Options.FirstOrDefault(x => string.Equals("target-user", x.Name));
        if (option == null) return AutocompletionResult.FromSuccess();

        var result = await sender.Send(new GetToysByUserIdQuery(ulong.Parse(option.Value.ToString())));
        return result.IsError
            ? AutocompletionResult.FromSuccess()
            : AutocompletionResult.FromSuccess(result.Value.Select(toy => new AutocompleteResult(toy.Name.Value, toy.Name.Value)));
    }
}