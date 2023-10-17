using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Shared.Exceptions;
using Sirb.CepBrasil.Shared.Extensions;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services
{
    internal sealed class CorreiosService : ICepServiceControl
    {
        private const string MediaType = "application/xml";
        private const string CorreiosUrl = "https://apphom.correios.com.br/SigepMasterJPA/AtendeClienteService/AtendeCliente";
        private readonly HttpClient _httpClient;

        public CorreiosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc cref="ICepServiceControl"/>
        [Obsolete("This method is obsolete. Use FindAsync instead.")]
        public Task<CepContainer> Find(string cep)
        {
            return FindAsync(cep, CancellationToken.None);
        }

        /// <inheritdoc cref="ICepServiceControl"/>
        public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
        {
            var response = await GetFromServiceAsync(cep.RemoveMask(), cancellationToken);

            ServiceException.ThrowIf(string.IsNullOrEmpty(response), CepMessages.ExceptionEmptyResponse);

            return ConvertResult(response);
        }

        private async Task<string> GetFromServiceAsync(string cep, CancellationToken cancellationToken)
        {
            using var request = CreateRequest(cep);
            return await ExecuteRequestAsync(request, cancellationToken);
        }

        static private HttpRequestMessage CreateRequest(string cep)
        {
            var uri = new Uri(CorreiosUrl);
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = GetRequestContent(cep)
            };
        }

        static private HttpContent GetRequestContent(string cep)
        {
            return new StringContent(BuildSoapBody(cep), Encoding.UTF8, MediaType);
        }

        static private string BuildSoapBody(string cep)
        {
            var sb = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cli=\"http://cliente.bean.master.sigep.bsb.correios.com.br/\">")
                .Append("<soapenv:Header/>")
                .Append("<soapenv:Body>")
                .Append("<cli:consultaCEP>")
                .Append($"<cep>{cep}</cep>")
                .Append("</cli:consultaCEP>")
                .Append("</soapenv:Body>")
                .Append("</soapenv:Envelope>");

            return sb.ToString();
        }

        async private Task<string> ExecuteRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return await GetResponseString(response, cancellationToken);
        }

        static async private Task<string> GetResponseString(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            ServiceException.ThrowIf(!response.IsSuccessStatusCode, GetFaultString(responseString));

            return responseString;
        }

        static private string GetTagValue(string rawValue, string tagName, string tagNotFoundMessage = default)
        {
            var regex = new Regex($"<{tagName}>(.*?)</{tagName}>", RegexOptions.None, TimeSpan.FromSeconds(10));
            var result = regex.Matches(rawValue);

            if (result.Count == 0)
                return tagNotFoundMessage;

            return result[0].Value
                .Replace($"</{tagName}>", string.Empty)
                .Replace($"<{tagName}>", string.Empty);
        }

        static private string GetFaultString(string response)
        {
            return GetTagValue(response, "faultstring", CepMessages.ExceptionServiceError);
        }

        static private CepContainer ConvertResult(string result)
        {
            return new CepContainer(GetUfValue(result),
                GetCidadeValue(result),
                GetBairroValue(result),
                GetEnderecoValue(result),
                GetComplementoValue(result),
                GetCepValue(result));
        }

        static private string GetBairroValue(string result)
        {
            return GetTagValue(result, "bairro");
        }

        static private string GetCepValue(string result)
        {
            return GetTagValue(result, "cep");
        }

        static private string GetCidadeValue(string result)
        {
            return GetTagValue(result, "cidade");
        }

        static private string GetComplementoValue(string result)
        {
            return GetTagValue(result, "complemento2");
        }

        static private string GetEnderecoValue(string result)
        {
            return GetTagValue(result, "end");
        }

        static private string GetUfValue(string result)
        {
            return GetTagValue(result, "uf");
        }
    }
}
