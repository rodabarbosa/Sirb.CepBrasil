using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Shared.Exceptions;
using Sirb.CepBrasil.Shared.Extensions;
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

        public Task<CepContainer> Find(string cep)
        {
            return FindAsync(cep);
        }

        public async Task<CepContainer> FindAsync(string cep)
        {
            CepValidation.Validate(cep);

            var response = await GetFromService(cep.RemoveMask());

            return ConverterCepResult(response);
        }

        async private Task<string> GetFromService(string cep)
        {
            using var request = CreateRequestMessage(cep);

            using var response = await ExecuteRequest(request);

            return await GetResponseString(response);
        }

        static private HttpRequestMessage CreateRequestMessage(string cep)
        {
            var url = BuildRequestUrl(cep);

            return new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        }

        static private string BuildRequestUrl(string cep)
        {
            return $"https://viacep.com.br/ws/{cep}/json";
        }

        async private Task<HttpResponseMessage> ExecuteRequest(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);

            ServiceException.ThrowIf(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

            return response;
        }

        static async private Task<string> GetResponseString(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            ServiceException.ThrowIf(string.IsNullOrEmpty(responseString), CepMessages.ExceptionEmptyResponse);

            return responseString;
        }

        static private CepContainer ConverterCepResult(string response)
        {
            return response.FromJson<CepContainer>();
        }
    }
}
