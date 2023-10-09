using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Shared.Exceptions;
using Sirb.CepBrasil.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services
{
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

        public CepService()
            : this(new HttpClient(), true)
        {
        }

        public CepService(HttpClient httpClient)
            : this(httpClient, false)
        {
        }

        /// <inheritdoc cref="ICepService"/>
        [Obsolete("This method is obsolete. Use FindAsync instead.")]
        public Task<CepResult> Find(string cep)
        {
            return FindAsync(cep, CancellationToken.None);
        }

        /// <inheritdoc cref="ICepService"/>
        public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
        {
            var result = new CepResult();
            foreach (var service in _services)
                try
                {
                    result.CepContainer = await service.FindAsync(cep, cancellationToken);

                    NotFoundException.ThrowIf(result.CepContainer is null, $"Nenhum resultado para o {cep}");

                    result.Success = true;
                    break;
                }
                catch (Exception e)
                {
                    result.Exceptions.Add(e);

                    var value = result.Message ?? string.Empty;
                    result.Message = $"{value}{e.AllMessages() ?? string.Empty} ";
                }

            return result;
        }

        public void Dispose()
        {
            if (_httpClientSelfCreated)
                _httpClient?.Dispose();
        }

        private void StartServices()
        {
            _services.Add(new CorreiosService(_httpClient));
            _services.Add(new ViaCepService(_httpClient));
        }
    }
}
