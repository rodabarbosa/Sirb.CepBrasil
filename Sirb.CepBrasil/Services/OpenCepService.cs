using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Validations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Service adapter for OpenCEP API used to lookup Brazilian postal code (CEP) information.
/// </summary>
/// <remarks>
/// OpenCEP is a free, open-source Brazilian postal code lookup service.
/// API documentation: https://opencep.com/
/// </remarks>
/// <param name="httpClient">Injected HttpClient instance used to perform HTTP requests.</param>
internal sealed class OpenCepService(HttpClient httpClient) : BaseService(httpClient), ICepServiceControl
{
    private const string OpenCepBaseUrl = "https://opencep.com/v1";

    /// <summary>
    /// Searches for address information using the provided postal code (CEP) via OpenCEP API.
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
    /// var service = new OpenCepService(httpClient);
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

        var result = await GetResponseAsync<CepContainer>(response, effectiveToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Builds the full URL to query the OpenCEP service.
    /// </summary>
    /// <param name="cep">CEP without formatting (digits only).</param>
    /// <returns>Full endpoint URL for the provided CEP.</returns>
    /// <example>
    /// For CEP "83040040", returns: "https://opencep.com/v1/83040040"
    /// </example>
    private static string BuildRequestUrl(string cep)
    {
        return $"{OpenCepBaseUrl}/{cep}";
    }
}
