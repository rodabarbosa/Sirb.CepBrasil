using System.Text.Json;

namespace Sirb.CepBrasil.Extensions
{
    public static class JsonExtension
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions DeserializerOptions = new JsonSerializerOptions
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
        public static string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value, SerializerOptions);
        }

        /// <summary>
        ///     Convert JSON string to especified class type.
        /// </summary>
        /// <typeparam name="T">Convert to</typeparam>
        /// <param name="value">JSON string</param>
        /// <returns></returns>
        public static T FromJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value, DeserializerOptions);
        }
    }
}