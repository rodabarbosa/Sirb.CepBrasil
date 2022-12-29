using System.Runtime.Serialization;

namespace Sirb.CepBrasil.Shared.Exceptions
{
    [Serializable]
    public sealed class ServiceException : Exception
    {
        [NonSerialized] private const string DefaultMessage = "Ocorreu um erro ao tentar acessar o serviço.";

        public ServiceException() : this(DefaultMessage)
        {
        }

        public ServiceException(string message) : this(message, default)
        {
        }

        public ServiceException(Exception innerException) : this(DefaultMessage, innerException)
        {
        }

        public ServiceException(string message, Exception innerException) : base(DefineMessage(message, DefaultMessage), innerException)
        {
        }

        private ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
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
