using System.Net.Http.Json;
using ErrorOr;

namespace CarnalContraption.Application.Services;

internal class HttpApiClient(HttpClient httpClient) : IHttpApiClient
{
    public static class Errors
    {
        public static class PostAsync
        {
            public static readonly Error UnknownResponse = Error.Failure("HttpApiClient.PostAsync.UnknownResponse", "Server did not return a response that could be understood.");
        }
    }

    public async Task<ErrorOr<Success>> PostAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await httpClient.PostAsJsonAsync(url, request, cancellationToken: cancellationToken);
        if (!httpResponseMessage.IsSuccessStatusCode)
            return httpResponseMessage.StatusCode switch
            {
                _ => Error.Unexpected("HttpApiClient.PostAsync.FailedRequest", $"{httpResponseMessage.StatusCode}")
            };

        return new Success();
    }

    public async Task<ErrorOr<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await httpClient.PostAsJsonAsync(url, request, cancellationToken: cancellationToken);
        if (!httpResponseMessage.IsSuccessStatusCode)
            return httpResponseMessage.StatusCode switch
            {
                _ => Error.Unexpected("HttpApiClient.PostAsync.FailedRequest", $"{httpResponseMessage.StatusCode}")
            };

        var response = await httpResponseMessage.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        if (response == null) return Errors.PostAsync.UnknownResponse;
        return response;
    }
}