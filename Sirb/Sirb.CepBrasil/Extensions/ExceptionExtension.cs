using System;
using System.Collections;

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
                return string.Empty;

            var messages = new ArrayList
            {
                e.Message
            };

            var innerException = e.InnerException;
            while (innerException != null)
            {
                messages.Add(innerException.Message);
                innerException = innerException.InnerException;
            }

            return string.Join(" ", messages);
        }
    }
}
