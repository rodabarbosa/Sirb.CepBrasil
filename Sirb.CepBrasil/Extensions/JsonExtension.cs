using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Extensions;

/// <summary>
/// Fornece métodos de extensão para serialização e desserialização de JSON.
/// Utiliza System.Text.Json com configurações padronizadas para o projeto.
/// </summary>
static public class JsonExtension
{
    /// <summary>
    /// Configuração padrão de serialização JSON para todo o projeto.
    /// Ignora propriedades nulas, é insensível a maiúsculas/minúsculas e usa nomenclatura camelCase.
    /// </summary>
    static private readonly JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Converte um objeto para uma string JSON formatada utilizando as configurações padrão do projeto.
    /// </summary>
    /// <param name="value">Objeto a ser serializado para JSON.</param>
    /// <returns>
    /// String contendo a representação JSON do objeto.
    /// Propriedades nulas são ignoradas e a nomenclatura utiliza camelCase.
    /// </returns>
    /// <exception cref="ArgumentNullException">Quando <paramref name="value"/> é nulo.</exception>
    /// <exception cref="JsonException">Quando ocorre um erro durante a serialização.</exception>
    /// <example>
    /// <code>
    /// var cepResult = new CepResult { Logradouro = "Rua A", Cidade = "São Paulo" };
    /// var json = cepResult.ToJson();
    /// // Resultado: {"logradouro":"Rua A","cidade":"São Paulo"}
    /// </code>
    /// </example>
    static public string ToJson(this object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        try
        {
            return JsonSerializer.Serialize(value, _serializerOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Erro ao serializar objeto do tipo '{value.GetType().Name}' para JSON.",
                ex);
        }
    }

    /// <summary>
    /// Converte uma string JSON para o tipo genérico especificado utilizando as configurações padrão do projeto.
    /// </summary>
    /// <typeparam name="T">Tipo para o qual desserializar a string JSON.</typeparam>
    /// <param name="value">String contendo o JSON a ser desserializado.</param>
    /// <returns>
    /// Objeto do tipo <typeparamref name="T"/> contendo os dados desserializados,
    /// ou null se a string JSON representar um valor nulo válido.
    /// </returns>
    /// <exception cref="ArgumentNullException">Quando <paramref name="value"/> é nulo.</exception>
    /// <exception cref="JsonException">Quando o JSON é inválido ou não pode ser convertido para o tipo especificado.</exception>
    /// <example>
    /// <code>
    /// var json = "{\"logradouro\":\"Rua A\",\"cidade\":\"São Paulo\"}";
    /// var cepResult = json.FromJson&lt;CepResult&gt;();
    /// // Resultado: CepResult { Logradouro = "Rua A", Cidade = "São Paulo" }
    /// </code>
    /// </example>
    static public T FromJson<T>(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "A string JSON não pode estar vazia ou conter apenas espaços em branco.",
                nameof(value));

        try
        {
            return JsonSerializer.Deserialize<T>(value, _serializerOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Erro ao desserializar JSON para o tipo '{typeof(T).Name}'.",
                ex);
        }
    }
}
