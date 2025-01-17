using CarnalContraption.Application.Services.Lovense;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.Lovense;

public record ConnectCommand(ulong userId) : IRequest<ErrorOr<Success>>;

internal class ConnectCommandHandler(ILovenseService lovenseService) : IRequestHandler<ConnectCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(ConnectCommand request, CancellationToken cancellationToken)
    {
        return lovenseService.ConnectAsync(request.userId);
    }
}