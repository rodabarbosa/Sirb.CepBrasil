using System.Runtime.Serialization;

namespace Sirb.CepBrasil.Shared.Exceptions
{
    [Serializable]
    public sealed class NotFoundException : Exception
    {
        [NonSerialized] private const string DefaultMessage = "Not found";

        public NotFoundException() : this(DefaultMessage)
        {
        }

        public NotFoundException(string message) : this(message, default)
        {
        }

        public NotFoundException(Exception innerException) : this(DefaultMessage, innerException)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(DefineMessage(message, DefaultMessage), innerException)
        {
        }

        private NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        static private string DefineMessage(string message, string fallbackMessage)
        {
            return string.IsNullOrEmpty(message?.Trim()) ? fallbackMessage : message;
        }

        static public void ThrowIf(bool condition, string message, Exception innerException = default)
        {
            if (condition)
                throw new NotFoundException(message, innerException);
        }
    }
}
