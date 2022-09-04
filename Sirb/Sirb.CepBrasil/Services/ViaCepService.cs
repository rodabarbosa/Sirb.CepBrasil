using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Validations;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services
{
    internal sealed class ViaCepService : ICepServiceControl
    {
        private readonly HttpClient _httpClient;

        public ViaCepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CepContainer> Find(string cep)
        {
            CepValidation.Validate(cep);

            var response = await GetFromService(cep.RemoveMask()).ConfigureAwait(false);
            ServiceException.ThrowIf(string.IsNullOrEmpty(response), CepMessages.ExceptionEmptyResponse);

            return ConverterCepResult(response);
        }

        private async Task<string> GetFromService(string cep)
        {
            var url = BuildRequestUrl(cep);
            using var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ServiceException.ThrowIf(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

            return responseString;
        }

        private static string BuildRequestUrl(string cep)
        {
            return $"https://viacep.com.br/ws/{cep}/json";
        }

        private static CepContainer ConverterCepResult(string response)
        {
            return response.FromJson<CepContainer>();
        }
    }
}
