using System.Text.RegularExpressions;

namespace Sirb.CepBrasil.Shared.Extensions
{
    static public class CepExtension
    {
        /// <summary>
        ///     Remove Mask, keeping alpha numeric chars.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string RemoveMask(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(value, @"[^\d]", string.Empty);
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

            return Regex.Replace(RemoveMask(value), @"(\d{5})(\d{3})", "$1-$2");
        }
    }
}
