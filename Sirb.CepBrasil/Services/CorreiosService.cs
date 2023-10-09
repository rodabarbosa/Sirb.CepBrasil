using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Shared.Exceptions;
using Sirb.CepBrasil.Shared.Extensions;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services
{
    internal sealed class CorreiosService : ICepServiceControl
    {
        private const string MediaType = "application/xml";
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string CorreiosUrl = "https://apphom.correios.com.br/SigepMasterJPA/AtendeClienteService/AtendeCliente";
#pragma warning restore S1075 // URIs should not be hardcoded
        private readonly HttpClient _httpClient;

        public CorreiosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<CepContainer> Find(string cep)
        {
            return FindAsync(cep);
        }

        public async Task<CepContainer> FindAsync(string cep)
        {
            var response = await GetFromServiceAsync(cep.RemoveMask());

            ServiceException.ThrowIf(string.IsNullOrEmpty(response), CepMessages.ExceptionEmptyResponse);

            return ConvertResult(response);
        }

        private Task<string> GetFromServiceAsync(string cep)
        {
            using var request = CreateRequest(cep);

            return ExecuteRequest(request);
        }

        static private HttpRequestMessage CreateRequest(string cep)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(CorreiosUrl),
                Content = GetRequestContent(cep)
            };

            return request;
        }

        async private Task<string> ExecuteRequest(HttpRequestMessage request)
        {
            using var response = await _httpClient.SendAsync(request);

            var responseString = await GetResponseString(response);

            return responseString;
        }

        static async private Task<string> GetResponseString(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            ServiceException.ThrowIf(!response.IsSuccessStatusCode, GetFaultString(responseString));

            return responseString;
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
                .AppendFormat("<cep>{0}</cep>", cep)
                .Append("</cli:consultaCEP>")
                .Append("</soapenv:Body>")
                .Append("</soapenv:Envelope>");

            return sb.ToString();
        }

        static private string GetTagValue(string rawValue, string tagName, string tagNotFoundMessage = default)
        {
            if (string.IsNullOrEmpty(rawValue?.Trim()))
                return tagNotFoundMessage;

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
