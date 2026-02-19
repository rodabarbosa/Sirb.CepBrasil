using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Validations;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// CEP lookup service using the ViaCEP API (https://viacep.com.br/).
/// </summary>
/// <remarks>
/// This service implements <see cref="ICepServiceControl"/> and provides access to Brazilian postal data via the public ViaCEP service.
/// It uses a default timeout of 30 seconds and automatically validates CEP format.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the ViaCepService.
/// </remarks>
/// <param name="httpClient">HttpClient used to perform requests to ViaCEP.</param>
/// <exception cref="ArgumentNullException">When <paramref name="httpClient"/> is null.</exception>
internal sealed class ViaCepService(HttpClient httpClient) : ICepServiceControl
{
    private const int DefaultTimeoutMilliseconds = 30000;
    private const string ViaCepBaseUrl = "https://viacep.com.br/ws";
    private const string JsonFormat = "json";

    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Asynchronously fetches address information for the provided CEP.
    /// </summary>
    /// <param name="cep">CEP to lookup (format: 00000000 or 00000-000).</param>
    /// <param name="cancellationToken">Cancellation token for the operation. If not provided, a default 30-second timeout is applied.</param>
    /// <returns>
    /// A task that yields a <see cref="CepContainer"/> containing address data (street, neighborhood, city, state, etc.).
    /// </returns>
    /// <exception cref="ArgumentException">When the CEP has an invalid format or contains non-numeric characters.</exception>
    /// <exception cref="ServiceException">
    /// When a communication error with ViaCEP occurs:
    /// - Service unavailable (HTTP error)
    /// - Empty response from server
    /// - Request timeout
    /// </exception>
    /// <exception cref="NotFoundException">When the CEP is not found by ViaCEP.</exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// var service = new ViaCepService(httpClient);
    /// var result = await service.FindAsync("83040-040", CancellationToken.None);
    /// Console.WriteLine($"Street: {result.Logradouro}");
    /// Console.WriteLine($"City: {result.Localidade}/{result.Uf}");
    /// </code>
    /// </example>
    async public Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        CepValidation.Validate(cep);

        using var cts = GetCancellationTokenSource(cancellationToken);
        var effectiveToken = cts?.Token ?? cancellationToken;

        var response = await GetFromServiceAsync(cep.RemoveMask(), effectiveToken).ConfigureAwait(false);

        return ConvertCepResult(response);
    }

    /// <summary>
    /// Creates a <see cref="CancellationTokenSource"/> with a default timeout if the provided token has no timeout.
    /// </summary>
    /// <param name="cancellationToken">The original cancellation token.</param>
    /// <returns>
    /// A <see cref="CancellationTokenSource"/> with a default 30-second timeout if the token is <see cref="CancellationToken.None"/>,
    /// otherwise returns <c>null</c> to use the original token.
    /// </returns>
    static private CancellationTokenSource GetCancellationTokenSource(CancellationToken cancellationToken)
    {
        return cancellationToken == CancellationToken.None
            ? new CancellationTokenSource(DefaultTimeoutMilliseconds)
            : null;
    }

    /// <summary>
    /// Executes the full request against ViaCEP and returns the response as JSON.
    /// </summary>
    /// <param name="cep">CEP without formatting (digits only).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>JSON string containing the address data returned by ViaCEP.</returns>
    /// <exception cref="ServiceException">When an error occurs during the request or the response is invalid.</exception>
    async private Task<string> GetFromServiceAsync(string cep, CancellationToken cancellationToken)
    {
        var request = CreateRequestMessage(cep);

        var response = await ExecuteRequestAsync(request, cancellationToken).ConfigureAwait(false);

        return await GetResponseStringAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the HTTP request message to query ViaCEP.
    /// </summary>
    /// <param name="cep">CEP without formatting (digits only).</param>
    /// <returns>Configured HTTP request message for querying ViaCEP.</returns>
    static private HttpRequestMessage CreateRequestMessage(string cep)
    {
        var url = BuildRequestUrl(cep);
        var uri = new Uri(url);

        return new HttpRequestMessage(HttpMethod.Get, uri);
    }

    /// <summary>
    /// Builds the full URL to query the ViaCEP service.
    /// </summary>
    /// <param name="cep">CEP without formatting (digits only).</param>
    /// <returns>Full endpoint URL for the provided CEP.</returns>
    /// <example>
    /// For CEP "83040040", returns: "https://viacep.com.br/ws/83040040/json"
    /// </example>
    static private string BuildRequestUrl(string cep)
    {
        return $"{ViaCepBaseUrl}/{cep}/{JsonFormat}";
    }

    /// <summary>
    /// Executes the HTTP request to ViaCEP and validates the response status code.
    /// </summary>
    /// <param name="request">Configured HTTP request message.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>HTTP response from the ViaCEP service.</returns>
    /// <exception cref="ServiceException">When the request returns an error status code.</exception>
    async private Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        ServiceException.ThrowIf(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

        return response;
    }

    /// <summary>
    /// Extracts the JSON content from the ViaCEP HTTP response.
    /// </summary>
    /// <param name="response">HTTP response from the ViaCEP service.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>JSON string with the address data.</returns>
    /// <exception cref="ServiceException">When the response is empty or null.</exception>
    static async private Task<string> GetResponseStringAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        ServiceException.ThrowIf(string.IsNullOrEmpty(responseString), CepMessages.ExceptionEmptyResponse);

        return responseString;
    }

    /// <summary>
    /// Converts the ViaCEP JSON response into a <see cref="CepContainer"/> object.
    /// </summary>
    /// <param name="response">JSON string containing the address data.</param>
    /// <returns>A <see cref="CepContainer"/> instance with deserialized data.</returns>
    /// <remarks>
    /// ViaCEP returns a JSON with the property "erro": true when the CEP is not found.
    /// In that case deserialization results in a <see cref="CepContainer"/> with null properties.
    /// </remarks>
    static private CepContainer ConvertCepResult(string response)
    {
        return response.FromJson<CepContainer>();
    }
}
