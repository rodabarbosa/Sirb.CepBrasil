using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
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
internal sealed class ViaCepService(HttpClient httpClient) : BaseService(httpClient), ICepServiceControl
{
    private const string ViaCepBaseUrl = "https://viacep.com.br/ws";
    private const string JsonFormat = "json";

    /// <summary>
    /// Asynchronously fetches address information for the provided CEP.
    /// </summary>
    /// <param name="cep">CEP to lookup (format: 00000000 or 00000-000).</param>
    /// <param name="cancellationToken">Cancellation token for the operation. If not provided, a default 30-second timeout is applied.</param>
    /// <returns>
    /// A task that yields a <see cref="CepContainer"/> containing address data (street, neighborhood, city, state, etc.),
    /// or <c>null</c> if the CEP is not found.
    /// </returns>
    /// <exception cref="ArgumentException">When the CEP has an invalid format or contains non-numeric characters.</exception>
    /// <exception cref="ServiceException">
    /// When a communication error with ViaCEP occurs:
    /// - Service unavailable (HTTP error)
    /// - Empty response from server
    /// - Request timeout
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// var service = new ViaCepService(httpClient);
    /// var result = await service.FindAsync("83040-040", CancellationToken.None);
    /// if (result != null)
    /// {
    ///     Console.WriteLine($"Street: {result.Logradouro}");
    ///     Console.WriteLine($"City: {result.Localidade}/{result.Uf}");
    /// }
    /// </code>
    /// </example>
    async public Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        CepValidation.Validate(cep);

        using var cts = GetCancellationTokenSource(cancellationToken);
        var effectiveToken = cts?.Token ?? cancellationToken;

        var cleanCep = cep.RemoveMask();
        var url = BuildRequestUrl(cleanCep);
        var request = CreateRequestMessage(url);

        var response = await ExecuteRequestAsync(request, effectiveToken)
            .ConfigureAwait(false);

        var result = await GetResponseAsync<CepContainer>(response, effectiveToken)
            .ConfigureAwait(false);

        // ViaCEP returns an object with "erro": true when CEP is not found
        return result?.Erro == true ? null : result;
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
}
