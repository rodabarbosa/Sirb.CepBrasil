using System;

namespace Sirb.CepBrasil.Exceptions
{
    public sealed class ServiceException : Exception
    {
        private const string DefaultMessage = "Error running service.";

        public ServiceException() : this(DefaultMessage)
        {
        }

        public ServiceException(string message) : this(message, null)
        {
        }

        public ServiceException(string message, Exception innerException) : base(message ?? DefaultMessage, innerException)
        {
        }

        /// <summary>
        /// Throws ServiceException when condition are met.
        /// </summary>
        /// <param name="condition">Condition for exception</param>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        [Obsolete("Use ThrowIf instead.")]
        public static void When(bool condition, string message, Exception innerException = null)
        {
            ThrowIf(condition, message, innerException);
        }

<<<<<<< HEAD
        /// <summary>
=======
        // <summary>
>>>>>>> a22339ff84e9c636f7f89fe430499bedad3ce9eb
        /// Throws ServiceException when condition are met.
        /// </summary>
        /// <param name="condition">Condition for exception</param>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public static void ThrowIf(bool condition, string message, Exception innerException = null)
        {
            if (condition)
                throw new ServiceException(message, innerException);
        }
    }
}
