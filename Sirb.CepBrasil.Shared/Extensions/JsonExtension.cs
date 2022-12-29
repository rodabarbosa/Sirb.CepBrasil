using System.Text.Json;

namespace Sirb.CepBrasil.Shared.Extensions
{
    static public class JsonExtension
    {
        static private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true
        };

        static private readonly JsonSerializerOptions _deserializerOptions = new JsonSerializerOptions
        {
#pragma warning disable SYSLIB0020
            IgnoreNullValues = true,
#pragma warning restore SYSLIB0020
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        ///     Convert object to JSON formatted.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value, _serializerOptions);
        }

        /// <summary>
        ///     Convert JSON string to especified class type.
        /// </summary>
        /// <typeparam name="T">Convert to</typeparam>
        /// <param name="value">JSON string</param>
        /// <returns></returns>
        static public T FromJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value, _deserializerOptions);
        }
    }
}
