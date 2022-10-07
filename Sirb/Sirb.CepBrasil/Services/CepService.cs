using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;

namespace Sirb.CepBrasil.Services
{
    public sealed class CepService : ICepService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _httpClientSelfCreated;

        private readonly List<ICepServiceControl> _services = new List<ICepServiceControl>();

        private CepService(HttpClient httpClient, bool httpClientSelfCreated)
        {
            _httpClientSelfCreated = httpClientSelfCreated;
            _httpClient = httpClient;

            StartServices();
        }

        public CepService() : this(new HttpClient(), true)
        {
        }

        public CepService(HttpClient httpClient) : this(httpClient, false)
        {
        }

        public async Task<CepResult> Find(string cep)
        {
            var result = new CepResult();
            foreach (var service in _services)
                try
                {
                    result.CepContainer = await service.Find(cep);

                    NotFoundException.ThrowIf(result.CepContainer == null, $"Nenhum resultado para o {cep}");

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