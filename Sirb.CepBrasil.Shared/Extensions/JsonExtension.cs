﻿using System.Text.Json;

namespace Sirb.CepBrasil.Shared.Extensions
{
    static public class JsonExtension
    {
        static private readonly JsonSerializerOptions _serializerOptions = new()
        {
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        /// <summary>
        /// Convert object to JSON formatted.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value, _serializerOptions);
        }

        /// <summary>
        /// Convert JSON string to specified class type.
        /// </summary>
        /// <typeparam name="T">Convert to</typeparam>
        /// <param name="value">JSON string</param>
        /// <returns></returns>
        static public T FromJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value, _serializerOptions);
        }
    }
}
