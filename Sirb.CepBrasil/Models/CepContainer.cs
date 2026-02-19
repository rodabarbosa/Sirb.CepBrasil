using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Models;

/// <summary>
/// Custom JSON converter for handling ViaCEP's "erro" field which can be boolean or string.
/// </summary>
internal sealed class BooleanOrStringConverter : JsonConverter<bool?>
{
    /// <summary>
    /// Reads and converts the JSON to type bool?.
    /// </summary>
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True)
            return true;

        if (reader.TokenType == JsonTokenType.False)
            return false;

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (bool.TryParse(stringValue, out var boolValue))
                return boolValue;
        }

        return null;
    }

    /// <summary>
    /// Writes a specified value as JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

/// <summary>
/// Container for address lookup results by CEP.
/// Represents a full Brazilian address obtained from a CEP lookup.
/// </summary>
/// <remarks>
/// This class is immutable and contains all address information returned by CEP lookup services.
/// Data from different providers (ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP) is normalized into a unified format.
/// </remarks>
/// <param name="uf">State abbreviation (2 letters), e.g. "SP", "RJ", "MG".</param>
/// <param name="cidade">Full city name, e.g. "São Paulo".</param>
/// <param name="bairro">Neighborhood or district name (may be empty for generic ZIPs).</param>
/// <param name="complemento">Additional address information (may be empty).</param>
/// <param name="logradouro">Street, avenue or public place name (may be empty for generic ZIPs).</param>
/// <param name="cep">Formatted postal code (e.g. "01310-100").</param>
/// <param name="ibge">IBGE municipality code (7 digits).</param>
/// <example>
/// Example usage:
/// <code>
/// var container = new CepContainer(
///     uf: "SP",
///     cidade: "São Paulo",
///     bairro: "Consolação",
///     complemento: "odd side",
///     logradouro: "Avenida Paulista",
///     cep: "01310-100",
///     ibge: "3550308"
/// );
/// Console.WriteLine($"Address: {container.Logradouro}, {container.Bairro} - {container.Cidade}/{container.Uf}");
/// </code>
/// </example>
public sealed class CepContainer(string uf, string cidade, string bairro, string complemento, string logradouro, string cep, string ibge)
{
    /// <summary>
    /// Gets the state abbreviation (UF).
    /// </summary>
    /// <value>
    /// Two-letter uppercase state code (e.g. SP, RJ, MG, RS).
    /// </value>
    /// <example>
    /// "SP", "RJ", "MG"
    /// </example>
    [JsonPropertyName("uf")]
    public string Uf { get; } = uf;

    /// <summary>
    /// Gets the full city name.
    /// </summary>
    /// <value>
    /// City where the CEP is located.
    /// </value>
    /// <example>
    /// "São Paulo", "Rio de Janeiro", "Belo Horizonte"
    /// </example>
    [JsonPropertyName("localidade")]
    public string Cidade { get; } = cidade;

    /// <summary>
    /// Gets the neighborhood or district name.
    /// </summary>
    /// <value>
    /// Neighborhood name; may be empty for generic CEPs that span multiple neighborhoods.
    /// </value>
    /// <remarks>
    /// For generic CEPs (large avenues or commercial areas) this field can be empty.
    /// </remarks>
    /// <example>
    /// "Consolação", "Centro", "Copacabana"
    /// </example>
    [JsonPropertyName("bairro")]
    public string Bairro { get; } = bairro;

    /// <summary>
    /// Gets additional address information.
    /// </summary>
    /// <value>
    /// Additional notes such as "odd side", "even side", "up to 500" or other observations.
    /// Can be empty when no complement is available.
    /// </value>
    /// <example>
    /// "odd side", "up to 500", ""
    /// </example>
    [JsonPropertyName("complemento")]
    public string Complemento { get; } = complemento;

    /// <summary>
    /// Gets the street (logradouro) name.
    /// </summary>
    /// <value>
    /// Full name of the public place; may be empty for special CEPs.
    /// </value>
    /// <remarks>
    /// For some special CEPs (PO boxes, large companies), this field may be empty.
    /// </remarks>
    /// <example>
    /// "Avenida Paulista", "Rua Augusta", "Praça da Sé"
    /// </example>
    [JsonPropertyName("logradouro")]
    public string Logradouro { get; } = logradouro;

    /// <summary>
    /// Gets the formatted postal code (CEP).
    /// </summary>
    /// <value>
    /// CEP in the format 00000-000 (with hyphen).
    /// </value>
    /// <example>
    /// "01310-100", "20040-020", "30130-000"
    /// </example>
    [JsonPropertyName("cep")]
    public string Cep { get; } = cep;

    /// <summary>
    /// Gets the IBGE code for the municipality.
    /// </summary>
    /// <value>
    /// Seven-digit code uniquely identifying the municipality in the IBGE system.
    /// </value>
    /// <remarks>
    /// The IBGE code is used for official municipality identification in Brazil. The first two digits represent the state.
    /// </remarks>
    /// <example>
    /// "3550308" (São Paulo/SP), "3304557" (Rio de Janeiro/RJ)
    /// </example>
    [JsonPropertyName("ibge")]
    public string Ibge { get; } = ibge;

    /// <summary>
    /// Gets a value indicating whether the CEP was not found (ViaCEP specific).
    /// </summary>
    /// <value>
    /// <c>true</c> if the CEP was not found in the database; otherwise, <c>false</c> or <c>null</c>.
    /// </value>
    /// <remarks>
    /// This property is specific to the ViaCEP API, which returns {"erro": true} when a CEP is not found.
    /// Other providers handle not found differently.
    /// </remarks>
    [JsonPropertyName("erro")]
    [JsonConverter(typeof(BooleanOrStringConverter))]
    public bool? Erro { get; init; }
}
