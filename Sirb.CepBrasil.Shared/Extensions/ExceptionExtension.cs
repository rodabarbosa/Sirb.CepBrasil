using System.Text;

namespace Sirb.CepBrasil.Shared.Extensions
{
    static public class ExceptionExtension
    {
        /// <summary>
        ///     Return exception's message with inner exception if exists.
        /// </summary>
        /// <param name="e">Exception</param>
        /// <returns></returns>
        static public string AllMessages(this Exception e)
        {
            if (e == null)
                return string.Empty;

            var sb = new StringBuilder();

            sb.AppendException(e);

            return sb.ToString();
        }

        static private void AppendException(this StringBuilder sb, Exception e)
        {
            if (e == null)
                return;

            sb.Append(' ')
                .Append(e.Message);

            sb.AppendException(e.InnerException);
        }
    }
}
