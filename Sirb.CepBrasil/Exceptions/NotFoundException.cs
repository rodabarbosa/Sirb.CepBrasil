using System;

namespace Sirb.CepBrasil.Exceptions
{
    /// <summary>
    /// Not found exception
    /// </summary>
    [Serializable]
    public sealed class NotFoundException : Exception
    {
        [NonSerialized] private const string DefaultMessage = "Not found";

        /// <inheritdoc />
        public NotFoundException() : this(DefaultMessage)
        {
        }

        /// <inheritdoc />
        public NotFoundException(Exception innerException) : this(DefaultMessage, innerException)
        {
        }

        /// <inheritdoc />
        public NotFoundException(string message, Exception innerException = default) : base(DefineMessage(message, DefaultMessage), innerException)
        {
        }

        static private string DefineMessage(string message, string fallbackMessage)
        {
            return string.IsNullOrEmpty(message?.Trim()) ? fallbackMessage : message;
        }

        /// <summary>
        /// Throws a NotFoundException if condition is met
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <exception cref="NotFoundException"></exception>
        static public void ThrowIf(bool condition, string message, Exception innerException = default)
        {
            if (condition)
                throw new NotFoundException(message, innerException);
        }
    }
}
