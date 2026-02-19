using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Service adapter for BrasilAPI (https://brasilapi.com.br/) used to lookup CEP information.
/// This class implements <see cref="ICepServiceControl"/> and performs the HTTP calls to the external provider.
/// </summary>
/// <param name="httpClient">HTTP client used to call BrasilAPI endpoints. It should be injected by DI.</param>
public class BrasilApiService(HttpClient httpClient) : ICepServiceControl
{
    /// <inheritdoc />
    public Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
