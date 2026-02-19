using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Service adapter for OpenCEP used to lookup CEP information.
/// Implements <see cref="ICepServiceControl"/> and encapsulates calls to the provider API.
/// </summary>
/// <param name="httpClient">Injected HttpClient used to perform HTTP requests.</param>
public class OpenCepService(HttpClient httpClient) : ICepServiceControl
{
    /// <inheritdoc />
    public Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
