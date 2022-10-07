using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;

namespace Sirb.CepBrasil.Services
{
    internal sealed class CorreiosService : ICepServiceControl
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string CorreiosUrl = "https://apphom.correios.com.br/SigepMasterJPA/AtendeClienteService/AtendeCliente";
#pragma warning restore S1075 // URIs should not be hardcoded
        private readonly HttpClient _httpClient;

        public CorreiosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CepContainer> Find(string cep)
        {
            var response = await GetFromService(cep.RemoveMask());
            ServiceException.ThrowIf(string.IsNullOrEmpty(response), CepMessages.ExceptionEmptyResponse);
            return ConvertResult(response);
        }

        private async Task<string> GetFromService(string cep)
        {
            using var request = new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri(CorreiosUrl) };
            request.Content = GetRequestContent(cep);

            using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ServiceException.ThrowIf(!response.IsSuccessStatusCode, GetFaultString(responseString));

            return responseString;
        }

        private HttpContent GetRequestContent(string cep)
        {
            return new StringContent(BuildSoapBody(cep), Encoding.UTF8, "application/xml");
        }

        private string BuildSoapBody(string cep)
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

        private string GetTagValue(string rawValue, string tagName, string tagNotFoundMessage = null)
        {
            var result = Regex.Matches(rawValue, $"<{tagName}>(.*?)</{tagName}>");
            if (result.Count == 0)
                return tagNotFoundMessage;

            return result[0].Value.Replace($"</{tagName}>", "").Replace($"<{tagName}>", "");
        }

        private string GetFaultString(string response)
        {
            return GetTagValue(response, "faultstring", CepMessages.ExceptionServiceError);
        }

        private CepContainer ConvertResult(string result)
        {
            return new CepContainer
            {
                Bairro = GetBairroValue(result),
                Cep = GetCepValue(result),
                Cidade = GetCidadeValue(result),
                Complemento = GetComplementoValue(result),
                Logradouro = GetEnderecoValue(result),
                Uf = GetUfValue(result)
            };
        }

        private string GetBairroValue(string result)
        {
            return GetTagValue(result, "bairro");
        }

        private string GetCepValue(string result)
        {
            return GetTagValue(result, "cep");
        }

        private string GetCidadeValue(string result)
        {
            return GetTagValue(result, "cidade");
        }

        private string GetComplementoValue(string result)
        {
            return GetTagValue(result, "complemento2");
        }

        private string GetEnderecoValue(string result)
        {
            return GetTagValue(result, "end");
        }

        private string GetUfValue(string result)
        {
            return GetTagValue(result, "uf");
        }
    }
}