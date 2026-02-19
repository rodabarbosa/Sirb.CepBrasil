using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Models;

/// <summary>
/// Representa a resposta da API BrasilAPI para consulta de CEP.
/// </summary>
/// <remarks>
/// Esta classe é utilizada internamente para deserializar a resposta JSON do serviço BrasilAPI.
/// Os dados são então mapeados para o formato unificado <see cref="CepContainer"/>.
///
/// URL da API: https://brasilapi.com.br/api/cep/v2/{cep}
/// </remarks>
internal sealed class BrasilApiResponse
{
    /// <summary>
    /// Obtém ou define o código de endereçamento postal.
    /// </summary>
    /// <value>CEP no formato 00000-000 retornado pela BrasilAPI.</value>
    [JsonPropertyName("cep")]
    public string Cep { get; set; }

    /// <summary>
    /// Obtém ou define a sigla da Unidade Federativa.
    /// </summary>
    /// <value>Sigla do estado (ex: SP, RJ).</value>
    [JsonPropertyName("uf")]
    public string Uf { get; set; }

    /// <summary>
    /// Obtém ou define o nome da cidade.
    /// </summary>
    /// <value>Nome da cidade retornado pela BrasilAPI usando a propriedade "city".</value>
    [JsonPropertyName("city")]
    public string Cidade { get; set; }

    /// <summary>
    /// Obtém ou define o nome do bairro.
    /// </summary>
    /// <value>Nome do bairro retornado pela BrasilAPI usando a propriedade "neighborhood".</value>
    [JsonPropertyName("neighborhood")]
    public string Bairro { get; set; }

    /// <summary>
    /// Obtém ou define o nome do logradouro.
    /// </summary>
    /// <value>Nome da rua/avenida retornado pela BrasilAPI usando a propriedade "street".</value>
    [JsonPropertyName("street")]
    public string Logradouro { get; set; }

    /// <summary>
    /// Obtém ou define o nome do serviço que originou os dados.
    /// </summary>
    /// <value>Identificação do provedor de dados utilizado pela BrasilAPI.</value>
    /// <remarks>
    /// A BrasilAPI pode utilizar diferentes provedores como fonte de dados.
    /// Este campo identifica qual provedor foi usado.
    /// </remarks>
    [JsonPropertyName("service")]
    public string Service { get; set; }
}
