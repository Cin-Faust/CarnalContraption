using CarnalContraption.Application.Storage.PiShock;
using CarnalContraption.Domain.PiShock;
using ErrorOr;
using MediatR;
using CarnalContraption.Domain.Users;

namespace CarnalContraption.Application.PiShock;

public record RegisterCommand(ulong UserId, string Username, string ApiKey, string Code) : UserRequest<Success>(UserId);

internal class RegisterCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    private const int None = 0;

    public Task<ErrorOr<Success>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => RegisterCommandToUser(request)
            .ThenAsync(userRepository.CreateAsync);

    private static ErrorOr<User> RegisterCommandToUser(RegisterCommand request)
    {
        var errors = new List<Error>();
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsError) errors.AddRange(userIdResult.Errors);

        var usernameResult = Username.Create(request.Username);
        if (usernameResult.IsError) errors.AddRange(usernameResult.Errors);

        var apiKeyResult = ApiKey.Create(request.ApiKey);
        if (apiKeyResult.IsError) errors.AddRange(apiKeyResult.Errors);

        var shareCodeResult = ShareCode.Create(request.Code);
        if (shareCodeResult.IsError) errors.AddRange(shareCodeResult.Errors);

        return errors.Count == None
            ? new User(userIdResult.Value, usernameResult.Value, apiKeyResult.Value, shareCodeResult.Value)
            : errors;
    }
}