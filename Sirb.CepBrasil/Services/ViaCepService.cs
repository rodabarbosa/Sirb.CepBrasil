using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Messages;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Validations;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services;

/// <summary>
/// Serviço de consulta de CEP utilizando a API ViaCEP (https://viacep.com.br/).
/// </summary>
/// <remarks>
/// Este serviço implementa a interface <see cref="ICepServiceControl"/> e fornece
/// acesso à base de dados de CEPs brasileiros através do serviço público ViaCEP.
/// Inclui timeout padrão de 30 segundos e validação automática de formato do CEP.
/// </remarks>
/// <remarks>
/// Inicializa uma nova instância do serviço ViaCEP.
/// </remarks>
/// <param name="httpClient">Cliente HTTP para realizar as requisições ao serviço ViaCEP.</param>
/// <exception cref="ArgumentNullException">Quando <paramref name="httpClient"/> é nulo.</exception>
internal sealed class ViaCepService(HttpClient httpClient) : ICepServiceControl
{
    private const int DefaultTimeoutMilliseconds = 30000;
    private const string ViaCepBaseUrl = "https://viacep.com.br/ws";

    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Busca assincronamente informações de endereço através do CEP fornecido.
    /// </summary>
    /// <param name="cep">CEP a ser consultado (formato: 00000000 ou 00000-000).</param>
    /// <param name="cancellationToken">Token para cancelamento da operação. Se não informado, utiliza timeout padrão de 30 segundos.</param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona, contendo um objeto <see cref="CepContainer"/>
    /// com os dados do endereço encontrado (logradouro, bairro, cidade, estado, etc.).
    /// </returns>
    /// <exception cref="ArgumentException">Quando o CEP está em formato inválido ou contém caracteres não numéricos.</exception>
    /// <exception cref="ServiceException">
    /// Quando ocorre erro na comunicação com o serviço ViaCEP:
    /// - Serviço indisponível (erro HTTP)
    /// - Resposta vazia do servidor
    /// - Timeout da requisição
    /// </exception>
    /// <exception cref="NotFoundException">Quando o CEP não é encontrado na base de dados do ViaCEP.</exception>
    /// <example>
    /// Exemplo de uso:
    /// <code>
    /// var service = new ViaCepService(httpClient);
    /// var resultado = await service.FindAsync("83040-040", CancellationToken.None);
    /// Console.WriteLine($"Logradouro: {resultado.Logradouro}");
    /// Console.WriteLine($"Cidade: {resultado.Localidade}/{resultado.Uf}");
    /// </code>
    /// </example>
    async public Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        CepValidation.Validate(cep);

        if (cancellationToken == CancellationToken.None)
            cancellationToken = GetDefaultCancellationToken();

        var response = await GetFromServiceAsync(cep.RemoveMask(), cancellationToken);

        return ConvertCepResult(response);
    }

    /// <summary>
    /// Cria um token de cancelamento com timeout padrão de 30 segundos.
    /// </summary>
    /// <returns>Token de cancelamento configurado com timeout padrão.</returns>
    static private CancellationToken GetDefaultCancellationToken()
    {
        var cancellationTokenSource = new CancellationTokenSource(DefaultTimeoutMilliseconds);
        return cancellationTokenSource.Token;
    }

    /// <summary>
    /// Executa a requisição completa ao serviço ViaCEP e retorna a resposta em formato JSON.
    /// </summary>
    /// <param name="cep">CEP sem formatação (apenas números).</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>String JSON contendo os dados do endereço retornados pelo ViaCEP.</returns>
    /// <exception cref="ServiceException">Quando ocorre erro na requisição ou resposta inválida.</exception>
    async private Task<string> GetFromServiceAsync(string cep, CancellationToken cancellationToken)
    {
        var request = CreateRequestMessage(cep);

        var response = await ExecuteRequestAsync(request, cancellationToken);

        return await GetResponseStringAsync(response, cancellationToken);
    }

    /// <summary>
    /// Cria a mensagem de requisição HTTP para consulta ao ViaCEP.
    /// </summary>
    /// <param name="cep">CEP sem formatação (apenas números).</param>
    /// <returns>Mensagem HTTP configurada para consulta ao ViaCEP.</returns>
    static private HttpRequestMessage CreateRequestMessage(string cep)
    {
        var url = BuildRequestUrl(cep);
        var uri = new Uri(url);

        return new HttpRequestMessage(HttpMethod.Get, uri);
    }

    /// <summary>
    /// Constrói a URL completa para consulta ao serviço ViaCEP.
    /// </summary>
    /// <param name="cep">CEP sem formatação (apenas números).</param>
    /// <returns>URL completa do endpoint ViaCEP para o CEP informado.</returns>
    /// <example>
    /// Para CEP "83040040", retorna: "https://viacep.com.br/ws/83040040/json"
    /// </example>
    static private string BuildRequestUrl(string cep)
    {
        return $"{ViaCepBaseUrl}/{cep}/json";
    }

    /// <summary>
    /// Executa a requisição HTTP ao serviço ViaCEP e valida o código de status da resposta.
    /// </summary>
    /// <param name="request">Mensagem de requisição HTTP configurada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Resposta HTTP do serviço ViaCEP.</returns>
    /// <exception cref="ServiceException">Quando a requisição retorna código de status de erro.</exception>
    async private Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);

        ServiceException.ThrowIf(!response.IsSuccessStatusCode, CepMessages.ExceptionServiceError);

        return response;
    }

    /// <summary>
    /// Extrai o conteúdo JSON da resposta HTTP do ViaCEP.
    /// </summary>
    /// <param name="response">Resposta HTTP do serviço ViaCEP.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>String JSON contendo os dados do endereço.</returns>
    /// <exception cref="ServiceException">Quando a resposta está vazia ou é nula.</exception>
    static async private Task<string> GetResponseStringAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        ServiceException.ThrowIf(string.IsNullOrEmpty(responseString), CepMessages.ExceptionEmptyResponse);

        return responseString;
    }

    /// <summary>
    /// Converte a resposta JSON do ViaCEP em um objeto <see cref="CepContainer"/>.
    /// </summary>
    /// <param name="response">String JSON contendo os dados do endereço.</param>
    /// <returns>Objeto <see cref="CepContainer"/> com os dados deserializados.</returns>
    /// <exception cref="NotFoundException">Quando o JSON indica que o CEP não foi encontrado.</exception>
    static private CepContainer ConvertCepResult(string response)
    {
        return response.FromJson<CepContainer>();
    }
}
