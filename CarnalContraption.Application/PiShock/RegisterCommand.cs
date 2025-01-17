using CarnalContraption.Application.Storage.PiShock;
using CarnalContraption.Domain.PiShock;
using ErrorOr;
using MediatR;
using CarnalContraption.Domain.Users;

namespace CarnalContraption.Application.PiShock;

public record RegisterCommand(ulong TargetUserId, string Username, string ApiKey, string Code) : IRequest<ErrorOr<Success>>;

internal class RegisterCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = UserId.Create(request.TargetUserId);
        if (userIdResult.IsError) return userIdResult.Errors;

        var usernameResult = Username.Create(request.Username);
        if(usernameResult.IsError) return usernameResult.Errors;

        var apiKeyResult = ApiKey.Create(request.ApiKey);
        if(apiKeyResult.IsError) return apiKeyResult.Errors;

        var shareCodeResult = ShareCode.Create(request.Code);
        if(shareCodeResult.IsError) return shareCodeResult.Errors;

        var user = new User(userIdResult.Value, usernameResult.Value, apiKeyResult.Value, shareCodeResult.Value);
        return await userRepository.CreateAsync(user);
    }
}