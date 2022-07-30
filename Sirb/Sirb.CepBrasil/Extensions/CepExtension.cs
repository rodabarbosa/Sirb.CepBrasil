using System.Text.RegularExpressions;

namespace Sirb.CepBrasil.Extensions
{
    public static class CepExtension
    {
        private const string RemoveMaskPattern = @"[^\d]";
        private const string RemoveMaskReplacement = "";

        private const string CepMaskPattern = @"(\d{5})(\d{3})";
        private const string CepMaskReplacement = "$1-$2";

        /// <summary>
        /// Remove Mask, keeping alpha numeric chars.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveMask(this string value)
        {
            if (string.IsNullOrEmpty(value?.Trim()))
                return default;

            return Regex.Replace(value, RemoveMaskPattern, RemoveMaskReplacement);
        }

        /// <summary>
        /// Place Brazilian Zip Code mask.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CepMask(this string value)
        {
            if (string.IsNullOrEmpty(value?.Trim()))
                return default;

            string input = value.RemoveMask();
            return Regex.Replace(input, CepMaskPattern, CepMaskReplacement);
        }
    }
}
