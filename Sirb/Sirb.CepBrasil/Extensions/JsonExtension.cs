﻿using System.Text.Json;

namespace Sirb.CepBrasil.Extensions
{
    public static class JsonExtension
    {
        private static readonly JsonSerializerOptions Settings = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Convert object to JSON formatted.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value, Settings);
        }

        /// <summary>
        /// Convert JSON string to especified class type.
        /// </summary>
        /// <typeparam name="T">Convert to</typeparam>
        /// <param name="value">JSON string</param>
        /// <returns></returns>
        public static T FromJson<T>(this string value) where T : class
        {
            return JsonSerializer.Deserialize<T>(value, Settings);
        }
    }
}
