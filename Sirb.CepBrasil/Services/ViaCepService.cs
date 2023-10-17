using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Shared.Exceptions;
using Sirb.CepBrasil.Shared.Extensions;
using Sirb.CepBrasil.Validations;
using System;
using System.Net.Http;
using System.Threading;
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

        /// <inheritdoc cref="ICepServiceControl"/>
        [Obsolete("This method is obsolete. Use FindAsync instead.")]
        public Task<CepContainer> Find(string cep)
        {
            return FindAsync(cep, default);
        }

        /// <inheritdoc cref="ICepServiceControl"/>
        public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
        {
            CepValidation.Validate(cep);

            if (cancellationToken == default)
                cancellationToken = getDefaultCancellationToken();

            var response = await GetFromService(cep.RemoveMask(), cancellationToken);

            return ConverterCepResult(response);
        }

        private static CancellationToken getDefaultCancellationToken()
        {
            var cancelationToken = new CancellationTokenSource(30000);
            return cancelationToken.Token;
        }

        async private Task<string> GetFromService(string cep, CancellationToken cancellationToken)
        {
            using var request = CreateRequestMessage(cep);

            using var response = await ExecuteRequest(request, cancellationToken);

            return await GetResponseString(response, cancellationToken);
        }

        static private HttpRequestMessage CreateRequestMessage(string cep)
        {
            var url = BuildRequestUrl(cep);
            Uri uri = new(url);

            HttpRequestMessage message = new(HttpMethod.Get, uri);

            return message;
        }

        static private string BuildRequestUrl(string cep)
        {
            return $"https://viacep.com.br/ws/{cep}/json";
        }

        async private Task<HttpResponseMessage> ExecuteRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            ServiceException.ThrowIf(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

            return response;
        }

        static async private Task<string> GetResponseString(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            ServiceException.ThrowIf(string.IsNullOrEmpty(responseString), CepMessages.ExceptionEmptyResponse);

            return responseString;
        }

        static private CepContainer ConverterCepResult(string response)
        {
            return response.FromJson<CepContainer>();
        }
    }
}
