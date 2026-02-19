using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sirb.CepBrasil.Extensions;

/// <summary>
/// Provides extension methods for JSON serialization and deserialization.
/// Uses System.Text.Json with project-standard settings.
/// </summary>
static public class JsonExtension
{
    /// <summary>
    /// Default JSON serialization settings for the project.
    /// It ignores null properties, is case-insensitive for property names and uses camelCase naming.
    /// </summary>
    static private readonly JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes an object to a JSON string using the project's default settings.
    /// </summary>
    /// <param name="value">Object to serialize to JSON.</param>
    /// <returns>
    /// JSON string representing the object. Null properties are ignored and property names are camelCased.
    /// </returns>
    /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
    /// <exception cref="JsonException">When a serialization error occurs.</exception>
    /// <example>
    /// <code>
    /// var cepResult = new CepResult { Logradouro = "Rua A", Cidade = "São Paulo" };
    /// var json = cepResult.ToJson();
    /// // Result: {"logradouro":"Rua A","cidade":"São Paulo"}
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
    /// Deserializes a JSON string into the specified generic type using the project's default settings.
    /// </summary>
    /// <typeparam name="T">Type to deserialize the JSON string into.</typeparam>
    /// <param name="value">JSON string to be deserialized.</param>
    /// <returns>
    /// An instance of <typeparamref name="T"/> containing deserialized data, or null if the JSON represents a null value.
    /// </returns>
    /// <exception cref="ArgumentNullException">When <paramref name="value"/> is null.</exception>
    /// <exception cref="JsonException">When the JSON is invalid or cannot be converted to the specified type.</exception>
    /// <example>
    /// <code>
    /// var json = "{\"logradouro\":\"Rua A\",\"cidade\":\"São Paulo\"}";
    /// var cepResult = json.FromJson&lt;CepResult&gt;();
    /// // Result: CepResult { Logradouro = "Rua A", Cidade = "São Paulo" }
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
