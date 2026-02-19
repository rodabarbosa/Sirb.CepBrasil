using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Validations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Service adapter for BrasilAPI used to lookup Brazilian postal code (CEP) information.
/// </summary>
/// <remarks>
/// BrasilAPI is a free and open Brazilian API that provides various data services.
/// API documentation: https://brasilapi.com.br/docs
/// </remarks>
/// <param name="httpClient">Injected HttpClient instance used to perform HTTP requests.</param>
internal sealed class BrasilApiService(HttpClient httpClient) : BaseService(httpClient), ICepServiceControl
{
    private const string BrasilApiBaseUrl = "https://brasilapi.com.br/api/cep/v1";

    /// <summary>
    /// Searches for address information using the provided postal code (CEP) via BrasilAPI.
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
    /// var service = new BrasilApiService(httpClient);
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

        var brasilApiResponse = await GetResponseAsync<BrasilApiResponse>(response, effectiveToken)
            .ConfigureAwait(false);

        if (brasilApiResponse is null)
            return null;

        return new CepContainer(
            brasilApiResponse.Uf,
            brasilApiResponse.Cidade,
            brasilApiResponse.Bairro,
            null,
            brasilApiResponse.Logradouro,
            brasilApiResponse.Cep,
            null);
    }

    /// <summary>
    /// Builds the full URL to query the BrasilAPI service.
    /// </summary>
    /// <param name="cep">CEP without formatting (digits only).</param>
    /// <returns>Full endpoint URL for the provided CEP.</returns>
    /// <example>
    /// For CEP "01310100", returns: "https://brasilapi.com.br/api/cep/v1/01310100"
    /// </example>
    private static string BuildRequestUrl(string cep)
    {
        return $"{BrasilApiBaseUrl}/{cep}";
    }
}
