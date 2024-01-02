﻿using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services
{
    /// <summary>
    /// Brazilian CEP service implementation
    /// </summary>
    public sealed class CepService : ICepService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _httpClientSelfCreated;

        private readonly List<ICepServiceControl> _services = new();

        private CepService(HttpClient httpClient, bool httpClientSelfCreated)
        {
            _httpClientSelfCreated = httpClientSelfCreated;
            _httpClient = httpClient;

            StartServices();
        }

        /// <inheritdoc />
        public CepService()
            : this(new HttpClient(), true)
        {
        }

        /// <inheritdoc />
        public CepService(HttpClient httpClient)
            : this(httpClient, false)
        {
        }

        /// <inheritdoc cref="ICepService"/>
        [Obsolete("This method is obsolete. Use FindAsync instead.")]
        public Task<CepResult> Find(string cep)
        {
            return FindAsync(cep, default);
        }

        /// <inheritdoc cref="ICepService"/>
        public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
        {
            if (cancellationToken == default)
                cancellationToken = GetDefaultCancellationToken();

            var message = string.Empty;
            foreach (var service in _services)
            {
                try
                {
                    var response = await service.FindAsync(cep, cancellationToken);

                    NotFoundException.ThrowIf(response is null, $"Nenhum resultado para o {cep}");

                    return new CepResult(true, response, default);
                }
                catch (Exception e)
                {
                    message += $"{e.AllMessages() ?? string.Empty} ";
                }
            }

            return new CepResult(false, default, message);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_httpClientSelfCreated)
                _httpClient?.Dispose();

            _services.Clear();
        }

        private static CancellationToken GetDefaultCancellationToken()
        {
            var cancelationToken = new CancellationTokenSource(30000);
            return cancelationToken.Token;
        }

        private void StartServices()
        {
            //_services.Add(new CorreiosService(_httpClient));
            _services.Add(new ViaCepService(_httpClient));
        }
    }
}
