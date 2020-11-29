using System.Text.RegularExpressions;

namespace Sirb.CepBrasil.Extensions
{
	public static class CepExtension
	{
		/// <summary>
		/// Remove Mask, keeping alpha numeric chars.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveMask(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return Regex.Replace(value, @"[^\d]", string.Empty);
		}

		/// <summary>
		/// Place Brazilian Zip Code mask.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string CepMask(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return Regex.Replace(RemoveMask(value), @"(\d{5})(\d{3})", "$1-$2");
		}
	}
}