using ErrorOr;
using MediatR;

namespace CarnalContraption.Application.Lovense;

public record GetConnectedQuery : IRequest<ErrorOr<IEnumerable<ulong>>>;