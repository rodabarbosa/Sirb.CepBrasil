using System;
using System.Text;

namespace Sirb.CepBrasil.Extensions
{
    /// <summary>
    /// Exception Extension
    /// </summary>
    static public class ExceptionExtension
    {
        /// <summary>
        ///     Return exception's message with inner exception if exists.
        /// </summary>
        /// <param name="e">Exception</param>
        /// <returns></returns>
        static public string AllMessages(this Exception e)
        {
            if (e is null)
                return string.Empty;

            var sb = new StringBuilder(e.Message);
            var inner = e.InnerException;
            while (inner != null)
            {
                sb.Append(' ')
                    .Append(inner.Message);

                inner = inner.InnerException;
            }

            return sb.ToString();
        }
    }
}
