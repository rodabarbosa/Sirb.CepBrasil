using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

    /// <summary>
    /// Primary service to query Brazilian postal codes (CEP) using an automatic fallback strategy.
    /// </summary>
    /// <remarks>
    /// Implements fallback between multiple providers in the following order:
    /// 1. BrasilAPI
    /// 2. ViaCEP
    /// 3. AwesomeAPI
    /// 4. OpenCEP
    ///
    /// Returns the first successful result. If no provider finds the CEP, the method returns a
    /// <see cref="CepResult"/> with Success = false and a consolidated error message.
    /// </remarks>
public sealed class CepService : ICepService, IDisposable
{
    private const int DefaultTimeoutMilliseconds = 30000;

    private readonly HttpClient _httpClient;
    private readonly bool _httpClientSelfCreated;

    private readonly List<ICepServiceControl> _services = new();

    private CepService(HttpClient httpClient, bool httpClientSelfCreated)
    {
        _httpClientSelfCreated = httpClientSelfCreated;
        _httpClient = httpClient;

        StartServices();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CepService"/> with an internally managed <see cref="HttpClient"/>.
    /// </summary>
    /// <remarks>
    /// This constructor creates and manages its own <see cref="HttpClient"/>, which will be disposed when
    /// <see cref="Dispose"/> is called. Use this constructor for simple scenarios where sharing an
    /// <see cref="HttpClient"/> instance is not required.
    /// </remarks>
    public CepService()
        : this(new HttpClient(), true)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CepService"/> using an externally provided <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">Instance of <see cref="HttpClient"/> to be used for HTTP requests.</param>
    /// <remarks>
    /// This constructor supports dependency injection of an <see cref="HttpClient"/>, which is ideal when used
    /// with DI containers. The provided <see cref="HttpClient"/> will NOT be disposed by this class because its
    /// lifecycle is expected to be managed externally.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
    public CepService(HttpClient httpClient)
        : this(httpClient ?? throw new ArgumentNullException(nameof(httpClient)), false)
    {
    }

    /// <summary>
    /// Retrieves address information for the provided CEP using an automatic fallback strategy.
    /// </summary>
    /// <param name="cep">The CEP to query (format: 00000000 or 00000-000).</param>
    /// <param name="cancellationToken">Token to cancel the operation. Defaults to a 30-second timeout when <see cref="CancellationToken.None"/> is passed.</param>
    /// <returns>
    /// A <see cref="CepResult"/> containing:
    /// - Success: true if an address was found by any provider
    /// - CepContainer: the address data found
    /// - Message: consolidated error message (if all providers fail)
    /// </returns>
    /// <remarks>
    /// The method tries providers in the following order:
    /// 1. BrasilAPI
    /// 2. ViaCEP
    /// 3. AwesomeAPI
    /// 4. OpenCEP
    ///
    /// The method returns immediately when the first successful response is obtained. If all providers
    /// fail, a <see cref="CepResult"/> with Success = false and a consolidated error message is returned.
    /// </remarks>
    /// <example>
    /// <code>
    /// using var service = new CepService();
    /// var result = await service.FindAsync("01310100", CancellationToken.None);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Address: {result.CepContainer.Logradouro}");
    /// }
    /// </code>
    /// </example>
    public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
    {
        using var cts = GetCancellationTokenSource(cancellationToken);
        var effectiveToken = cts?.Token ?? cancellationToken;

        var message = string.Empty;
        foreach (var service in _services)
        {
            try
            {
                var response = await service.FindAsync(cep, effectiveToken).ConfigureAwait(false);

                if (response is null)
                    continue;

                return new CepResult(true, response, null);
            }
            catch (Exception e)
            {
                message += $"{e.AllMessages() ?? string.Empty} ";
            }
        }

        return new CepResult(false, null, message.TrimEnd());
    }

    /// <summary>
    /// Releases resources used by this <see cref="CepService"/> instance.
    /// </summary>
    /// <remarks>
    /// If the <see cref="HttpClient"/> was created internally (via the parameterless constructor), it will be disposed.
    /// Otherwise, the client lifecycle is assumed to be managed externally and will not be disposed.
    /// </remarks>
    public void Dispose()
    {
        if (_httpClientSelfCreated)
            _httpClient?.Dispose();

        _services.Clear();
    }

    /// <summary>
    /// Creates a <see cref="CancellationTokenSource"/> with a default timeout when the provided token does not have one.
    /// </summary>
    /// <param name="cancellationToken">The original cancellation token.</param>
    /// <returns>
    /// A <see cref="CancellationTokenSource"/> with a default 30-second timeout when <paramref name="cancellationToken"/>
    /// equals <see cref="CancellationToken.None"/>; otherwise, returns <c>null</c> to indicate the caller's token should be used.
    /// </returns>
    private static CancellationTokenSource GetCancellationTokenSource(CancellationToken cancellationToken)
    {
        return cancellationToken == CancellationToken.None
            ? new CancellationTokenSource(DefaultTimeoutMilliseconds)
            : null;
    }

    /// <summary>
    /// Initializes the list of CEP providers in the fallback priority order.
    /// </summary>
    /// <remarks>
    /// Attempt order:
    /// 1. BrasilAPI - first attempt
    /// 2. ViaCEP - second attempt
    /// 3. AwesomeAPI - third attempt
    /// 4. OpenCEP - last attempt
    /// </remarks>
    private void StartServices()
    {
        _services.Add(new BrasilApiService(_httpClient));
        _services.Add(new ViaCepService(_httpClient));
        _services.Add(new AwesomeApiService(_httpClient));
        _services.Add(new OpenCepService(_httpClient));
    }
}
