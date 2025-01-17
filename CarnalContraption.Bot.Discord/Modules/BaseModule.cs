using Discord;
using Discord.Interactions;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Bot.Discord.Modules;

public abstract class BaseModule(ISender sender) : InteractionModuleBase
{
    protected async Task ExecuteAsync<TResult>(IRequest<ErrorOr<TResult>> request, Func<TResult, Embed>? output = null)
    {
        var result = await sender.Send(request);
        if (result.IsError)
        {
            await RespondAsync(embed: new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("Error")
                .WithDescription(string.Join('\n', result.Errors.Select(error => error.Description)))
                .Build());
            return;
        }

        var embed = output?.Invoke(result.Value) ?? new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle("Success")
            .Build();
        await RespondAsync(embed: embed);
    }

    protected async Task ExecuteAsync<TResult>(IRequest<ErrorOr<TResult>> request, Func<TResult, string> toDescription)
    {
        var result = await sender.Send(request);
        if (result.IsError)
        {
            await RespondAsync(embed: new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("Error")
                .WithDescription(string.Join('\n', result.Errors.Select(error => error.Description)))
                .Build());
            return;
        }

        var description = toDescription.Invoke(result.Value);
        await RespondAsync(embed: new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle("Success")
            .WithDescription(description)
            .Build());
    }
}