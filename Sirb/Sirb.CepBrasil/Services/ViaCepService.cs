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
            CepValition.Validate(cep);

            string response = await GetFromService(cep.RemoveMask());
            ServiceException.When(string.IsNullOrEmpty(response), CepMessages.ExceptionEmptyResponse);
            return ConverterCepResult(response);
        }

        private async Task<string> GetFromService(string cep)
        {
            string url = BuildRequestUrl(cep);
            using var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};
            using HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ServiceException.When(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

            return responseString;
        }

        private static string BuildRequestUrl(string cep) => $"https://viacep.com.br/ws/{cep}/json";

        private static CepContainer ConverterCepResult(string response) => response.FromJson<CepContainer>();
    }
}
