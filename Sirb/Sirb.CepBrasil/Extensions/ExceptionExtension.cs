using System;
using System.Text;

namespace Sirb.CepBrasil.Extensions
{
	public static class ExceptionExtension
	{
		/// <summary>
		/// Return exception's message with inner exception if exists.
		/// </summary>
		/// <param name="e">Exception</param>
		/// <returns></returns>
		public static string AllMessages(this Exception e)
		{
			if (e == null)
				return "";

			var sb = new StringBuilder(e.Message);
			if (e.InnerException != null)
				sb.Append(' ')
					.Append(e.InnerException.AllMessages());

			return sb.ToString();
		}
	}
}