using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Models;

/// <summary>
/// Container para o resultado da busca de endereço por CEP.
/// Representa um endereço completo brasileiro obtido através da consulta de CEP.
/// </summary>
/// <remarks>
/// Esta classe é imutável e contém todas as informações de endereço retornadas pelos serviços de consulta de CEP.
/// Os dados são normalizados de diferentes provedores (ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP) para um formato unificado.
/// </remarks>
/// <param name="uf">Sigla da Unidade Federativa (estado) no formato de 2 letras (ex: SP, RJ, MG)</param>
/// <param name="cidade">Nome completo da cidade (ex: São Paulo, Rio de Janeiro)</param>
/// <param name="bairro">Nome do bairro ou distrito (pode estar vazio para CEPs genéricos)</param>
/// <param name="complemento">Informações complementares do endereço (pode estar vazio)</param>
/// <param name="logradouro">Nome da rua, avenida ou logradouro público (pode estar vazio para CEPs genéricos)</param>
/// <param name="cep">Código de Endereçamento Postal formatado (ex: 01310-100)</param>
/// <param name="ibge">Código IBGE do município (7 dígitos)</param>
/// <example>
/// Exemplo de uso:
/// <code>
/// var container = new CepContainer(
///     uf: "SP",
///     cidade: "São Paulo",
///     bairro: "Consolação",
///     complemento: "lado ímpar",
///     logradouro: "Avenida Paulista",
///     cep: "01310-100",
///     ibge: "3550308"
/// );
/// Console.WriteLine($"Endereço: {container.Logradouro}, {container.Bairro} - {container.Cidade}/{container.Uf}");
/// </code>
/// </example>
public sealed class CepContainer(string uf, string cidade, string bairro, string complemento, string logradouro, string cep, string ibge)
{
    /// <summary>
    /// Obtém a sigla da Unidade Federativa (estado).
    /// </summary>
    /// <value>
    /// Sigla do estado no formato de 2 letras maiúsculas (ex: SP, RJ, MG, RS).
    /// </value>
    /// <example>
    /// "SP", "RJ", "MG"
    /// </example>
    [JsonPropertyName("uf")]
    public string Uf { get; } = uf;

    /// <summary>
    /// Obtém o nome completo da cidade.
    /// </summary>
    /// <value>
    /// Nome da cidade onde o CEP está localizado.
    /// </value>
    /// <example>
    /// "São Paulo", "Rio de Janeiro", "Belo Horizonte"
    /// </example>
    [JsonPropertyName("localidade")]
    public string Cidade { get; } = cidade;

    /// <summary>
    /// Obtém o nome do bairro ou distrito.
    /// </summary>
    /// <value>
    /// Nome do bairro. Pode estar vazio para CEPs genéricos que abrangem múltiplos bairros.
    /// </value>
    /// <remarks>
    /// Para CEPs genéricos de grandes avenidas ou áreas comerciais, este campo pode estar vazio.
    /// </remarks>
    /// <example>
    /// "Consolação", "Centro", "Copacabana"
    /// </example>
    [JsonPropertyName("bairro")]
    public string Bairro { get; } = bairro;

    /// <summary>
    /// Obtém informações complementares do endereço.
    /// </summary>
    /// <value>
    /// Informações adicionais como "lado par", "lado ímpar", "até X" ou outras observações.
    /// Pode estar vazio quando não há complemento disponível.
    /// </value>
    /// <example>
    /// "lado ímpar", "até 500", ""
    /// </example>
    [JsonPropertyName("complemento")]
    public string Complemento { get; } = complemento;

    /// <summary>
    /// Obtém o nome do logradouro (rua, avenida, etc.).
    /// </summary>
    /// <value>
    /// Nome completo do logradouro público. Pode estar vazio para CEPs de abrangência especial.
    /// </value>
    /// <remarks>
    /// Para alguns CEPs especiais (caixas postais, grandes empresas), este campo pode estar vazio.
    /// </remarks>
    /// <example>
    /// "Avenida Paulista", "Rua Augusta", "Praça da Sé"
    /// </example>
    [JsonPropertyName("logradouro")]
    public string Logradouro { get; } = logradouro;

    /// <summary>
    /// Obtém o Código de Endereçamento Postal formatado.
    /// </summary>
    /// <value>
    /// CEP no formato 00000-000 (com hífen).
    /// </value>
    /// <example>
    /// "01310-100", "20040-020", "30130-000"
    /// </example>
    [JsonPropertyName("cep")]
    public string Cep { get; } = cep;

    /// <summary>
    /// Obtém o código IBGE do município.
    /// </summary>
    /// <value>
    /// Código de 7 dígitos que identifica unicamente o município no sistema do IBGE.
    /// </value>
    /// <remarks>
    /// O código IBGE é utilizado para identificação oficial de municípios brasileiros.
    /// Os dois primeiros dígitos representam o estado.
    /// </remarks>
    /// <example>
    /// "3550308" (São Paulo/SP), "3304557" (Rio de Janeiro/RJ)
    /// </example>
    [JsonPropertyName("ibge")]
    public string Ibge { get; } = ibge;
}
