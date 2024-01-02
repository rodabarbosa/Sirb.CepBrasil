using System;
using System.Text.RegularExpressions;

namespace Sirb.CepBrasil.Extensions
{
    /// <summary>
    /// Cep Extension
    /// </summary>
    static public class CepExtension
    {
        static private readonly Regex _regexOnlyNumber = new(@"[^\d]", RegexOptions.None, TimeSpan.FromSeconds(15));
        static private readonly Regex _regexCep = new(@"(\d{5})(\d{3})", RegexOptions.None, TimeSpan.FromSeconds(15));

        /// <summary>
        ///     Remove Mask, keeping alpha numeric chars.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string RemoveMask(this string value)
        {
            return string.IsNullOrEmpty(value?.Trim())
                ? value
                : _regexOnlyNumber.Replace(value, string.Empty);
        }

        /// <summary>
        ///     Place Brazilian Zip Code mask.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string CepMask(this string value)
        {
            if (string.IsNullOrEmpty(value?.Trim()))
                return value;

            var cleanValue = value.RemoveMask();

            return _regexCep.Replace(cleanValue, "$1-$2");
        }
    }
}
