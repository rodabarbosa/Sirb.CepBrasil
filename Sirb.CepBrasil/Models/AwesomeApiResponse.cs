using Sirb.CepBrasil.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Representa a resposta da API AwesomeAPI para consulta de CEP.
/// </summary>
/// <remarks>
/// Esta classe é utilizada internamente para deserializar a resposta JSON do serviço AwesomeAPI.
/// Os dados são então mapeados para o formato unificado <see cref="CepContainer"/>.
///
/// URL da API: https://cep.awesomeapi.com.br/json/{cep}
/// </remarks>
internal sealed class AwesomeApiResponse
{
    /// <summary>
    /// Obtém ou define o código de endereçamento postal.
    /// </summary>
    /// <value>CEP no formato 00000-000 retornado pela AwesomeAPI.</value>
    [JsonPropertyName("cep")]
    public string Cep { get; set; }

    /// <summary>
    /// Obtém ou define a sigla da Unidade Federativa.
    /// </summary>
    /// <value>Sigla do estado retornado pela AwesomeAPI usando a propriedade "state".</value>
    [JsonPropertyName("state")]
    public string Uf { get; set; }

    /// <summary>
    /// Obtém ou define o nome da cidade.
    /// </summary>
    /// <value>Nome da cidade retornado pela AwesomeAPI usando a propriedade "city".</value>
    [JsonPropertyName("city")]
    public string Cidade { get; set; }

    /// <summary>
    /// Obtém ou define o nome do bairro.
    /// </summary>
    /// <value>Nome do bairro retornado pela AwesomeAPI usando a propriedade "district".</value>
    [JsonPropertyName("district")]
    public string Bairro { get; set; }

    /// <summary>
    /// Obtém ou define o nome do logradouro.
    /// </summary>
    /// <value>Nome da rua/avenida retornado pela AwesomeAPI usando a propriedade "address".</value>
    [JsonPropertyName("address")]
    public string Logradouro { get; set; }

    /// <summary>
    /// Obtém ou define o nome do serviço que originou os dados.
    /// </summary>
    /// <value>Identificação do provedor de dados utilizado pela AwesomeAPI.</value>
    /// <remarks>
    /// Este campo pode conter informações sobre a fonte original dos dados de CEP.
    /// </remarks>
    [JsonPropertyName("service")]
    public string Service { get; set; }
}
