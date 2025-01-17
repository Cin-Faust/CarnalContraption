using CarnalContraption.Application.Services.Lovense;
using CarnalContraption.Domain.Lovense;
using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.Lovense;

public record GetToysByUserIdQuery(ulong UserId) : IRequest<ErrorOr<IEnumerable<Toy>>>;

internal class GetToysByUserIdQueryHandler(ILovenseService lovenseService) : IRequestHandler<GetToysByUserIdQuery, ErrorOr<IEnumerable<Toy>>>
{
    public Task<ErrorOr<IEnumerable<Toy>>> Handle(GetToysByUserIdQuery request, CancellationToken cancellationToken)
        => lovenseService.GetToysByUserId(request.UserId);
}