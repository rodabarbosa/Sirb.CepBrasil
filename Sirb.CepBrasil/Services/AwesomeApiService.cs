using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Validations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Service adapter for AwesomeAPI used to lookup Brazilian postal code (CEP) information.
/// </summary>
/// <remarks>
/// AwesomeAPI is a free Brazilian postal code lookup service.
/// API documentation: https://cep.awesomeapi.com.br/
/// </remarks>
/// <param name="httpClient">Injected HttpClient instance used to perform HTTP requests.</param>
internal sealed class AwesomeApiService(HttpClient httpClient) : BaseService(httpClient), ICepServiceControl
{
    private const string AwesomeApiBaseUrl = "https://cep.awesomeapi.com.br/json";

    /// <summary>
    /// Searches for address information using the provided postal code (CEP) via AwesomeAPI.
    /// </summary>
    /// <param name="cep">Postal code to query (format: 00000000 or 00000-000).</param>
    /// <param name="cancellationToken">Cancellation token for the operation. Default timeout: 30 seconds.</param>
    /// <returns>
    /// Returns a <see cref="CepContainer"/> with address data if found, or null if the CEP does not exist.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">When the postal code is null or empty.</exception>
    /// <exception cref="ServiceException">When the postal code format is invalid or the service request fails.</exception>
    /// <example>
    /// Usage example:
    /// <code>
    /// var service = new AwesomeApiService(httpClient);
    /// var result = await service.FindAsync("01310100", CancellationToken.None);
    /// if (result != null)
    /// {
    ///     Console.WriteLine($"Address: {result.Logradouro}");
    /// }
    /// </code>
    /// </example>
    public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        CepValidation.Validate(cep);

        var cleanCep = cep.RemoveMask();
        var url = BuildRequestUrl(cleanCep);
        var request = CreateRequestMessage(url);

        using var cts = GetCancellationTokenSource(cancellationToken);
        var effectiveToken = cts?.Token ?? cancellationToken;

        var response = await ExecuteRequestAsync(request, effectiveToken)
            .ConfigureAwait(false);

        var awesomeResponse = await GetResponseAsync<AwesomeApiResponse>(response, effectiveToken)
            .ConfigureAwait(false);

        if (awesomeResponse is null)
            return null;

        return new CepContainer(
            awesomeResponse.Uf,
            awesomeResponse.Cidade,
            awesomeResponse.Bairro,
            null,
            awesomeResponse.Logradouro,
            awesomeResponse.Cep,
            null);
    }

    /// <summary>
    /// Builds the full URL to query the AwesomeAPI service.
    /// </summary>
    /// <param name="cep">CEP without formatting (digits only).</param>
    /// <returns>Full endpoint URL for the provided CEP.</returns>
    /// <example>
    /// For CEP "01310100", returns: "https://cep.awesomeapi.com.br/json/01310100"
    /// </example>
    private static string BuildRequestUrl(string cep)
    {
        return $"{AwesomeApiBaseUrl}/{cep}";
    }
}
