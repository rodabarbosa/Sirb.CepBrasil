using System;

namespace Sirb.CepBrasil.Exceptions
{
    /// <summary>
    /// Service Exception
    /// </summary>
    [Serializable]
    public sealed class ServiceException : Exception
    {
        [NonSerialized] private const string DefaultMessage = "Ocorreu um erro ao tentar acessar o serviço.";

        /// <inheritdoc />
        public ServiceException() : this(DefaultMessage)
        {
        }

        /// <inheritdoc />
        public ServiceException(Exception innerException) : this(DefaultMessage, innerException)
        {
        }

        /// <inheritdoc />
        public ServiceException(string message, Exception innerException = default) : base(DefineMessage(message, DefaultMessage), innerException)
        {
        }

        static private string DefineMessage(string message, string fallbackMessage)
        {
            return string.IsNullOrEmpty(message?.Trim()) ? fallbackMessage : message;
        }

        /// <summary>
        /// Throws ServiceException when condition are met.
        /// </summary>
        /// <param name="condition">Condition for exception</param>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        static public void ThrowIf(bool condition, string message, Exception innerException = default)
        {
            if (condition)
                throw new ServiceException(message, innerException);
        }
    }
}
