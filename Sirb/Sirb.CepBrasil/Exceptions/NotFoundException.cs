using System;

namespace Sirb.CepBrasil.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        [NonSerialized] private const string DefaultMessage = "Not found";

        public NotFoundException() : this(DefaultMessage)
        {
        }

        public NotFoundException(string message) : this(message, null)
        {
        }

        public NotFoundException(Exception innerException) : this(DefaultMessage, innerException)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(DefineMessage(message, DefaultMessage), innerException)
        {
        }

        private static string DefineMessage(string message, string fallbackMessage)
        {
            if (string.IsNullOrEmpty(message?.Trim()))
                return fallbackMessage;

            return message;
        }

        public static void ThrowIf(bool condition, string message, Exception inner = null)
        {
            if (condition)
                throw new NotFoundException(message, inner);
        }
    }
}