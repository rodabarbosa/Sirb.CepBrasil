using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Messages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Base service class providing common HTTP operations for CEP lookup services.
/// </summary>
internal abstract class BaseService(HttpClient httpClient)
{
    private const int DefaultTimeoutMilliseconds = 30000;

    /// <summary>
    /// Gets the base URL for the service endpoint.
    /// </summary>
    protected readonly string Url = string.Empty;

    /// <summary>
    /// Creates a <see cref="CancellationTokenSource"/> with a default timeout if the provided token has no timeout.
    /// </summary>
    /// <param name="cancellationToken">The original cancellation token.</param>
    /// <returns>
    /// A <see cref="CancellationTokenSource"/> with a default 30-second timeout if the token is <see cref="CancellationToken.None"/>,
    /// otherwise returns <c>null</c> to indicate the original token should be used.
    /// </returns>
    static protected CancellationTokenSource GetCancellationTokenSource(CancellationToken cancellationToken)
    {
        return cancellationToken == CancellationToken.None
            ? new CancellationTokenSource(DefaultTimeoutMilliseconds)
            : null;
    }

    /// <summary>
    /// Creates an HTTP GET request message for the specified URL.
    /// </summary>
    /// <param name="url">The target URL for the HTTP request.</param>
    /// <returns>Configured HTTP GET request message.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="url"/> is null or empty.</exception>
    /// <exception cref="UriFormatException">When <paramref name="url"/> is not a valid URI format.</exception>
    static protected HttpRequestMessage CreateRequestMessage(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var uri = new Uri(url);
        return new HttpRequestMessage(HttpMethod.Get, uri);
    }

    /// <summary>
    /// Executes an HTTP request and validates the response status code.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The HTTP response message from the service.</returns>
    /// <exception cref="ServiceException">When the request returns an unsuccessful status code.</exception>
    /// <exception cref="OperationCanceledException">When the operation is cancelled.</exception>
    async protected Task<HttpResponseMessage> ExecuteRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        ServiceException.ThrowIf(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

        return response;
    }

    /// <summary>
    /// Deserializes the JSON content from an HTTP response into the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content into.</typeparam>
    /// <param name="response">The HTTP response message containing JSON content.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ServiceException">When the response content is null, empty, or cannot be deserialized.</exception>
    /// <exception cref="OperationCanceledException">When the operation is cancelled.</exception>
    static async protected Task<T> GetResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var responseItem = await response.Content
            .ReadFromJsonAsync<T>(cancellationToken)
            .ConfigureAwait(false);

        if (responseItem is not null)
            return responseItem;

        var responseString = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        var errorMessage = string.IsNullOrWhiteSpace(responseString)
            ? "The service returned an empty or null response."
            : $"Failed to deserialize response: {responseString}";

        throw new ServiceException(errorMessage);
    }
}
