using ErrorOr;

namespace CarnalContraption.Application.Services;

public interface IHttpApiClient
{
    Task<ErrorOr<Success>> PostAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default);
}