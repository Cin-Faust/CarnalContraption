using CarnalContraption.Domain.PiShock;
using ErrorOr;

namespace CarnalContraption.Application.Services.PiShock;

public interface IClient
{
    Task<ErrorOr<Success>> SendRequest(User user, Request request);
}