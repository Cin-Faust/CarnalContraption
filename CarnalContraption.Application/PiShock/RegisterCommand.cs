using ErrorOr;
using MediatR;
using System.Text.Json;
using CarnalContraption.Domain.PiShock;

namespace CarnalContraption.Application.PiShock;

public record RegisterCommand(ulong TargetUserId, string Username, string ApiKey, string Code) : IRequest<ErrorOr<Success>>;

internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username)) return Error.Validation(description: "Username cannot be empty.");
        if (string.IsNullOrWhiteSpace(request.ApiKey)) return Error.Validation(description: "ApiKey cannot be empty.");
        if (string.IsNullOrWhiteSpace(request.Code)) return Error.Validation(description: "Code cannot be empty.");

        var users = JsonSerializer.Deserialize<List<User>>(await File.ReadAllTextAsync("users.json", cancellationToken)) ?? [];
        users.Add(new User(request.TargetUserId, request.Username, request.ApiKey, request.Code));
        await File.WriteAllTextAsync("users.json",JsonSerializer.Serialize(users), cancellationToken);
        return new Success();
    }
}