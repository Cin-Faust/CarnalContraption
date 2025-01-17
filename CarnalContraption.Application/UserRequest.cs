using ErrorOr;
using MediatR;

namespace CarnalContraption.Application;

public abstract record UserRequest<TResult>(ulong UserId) : IRequest<ErrorOr<TResult>>;