using System;

namespace Sirb.CepBrasil.Exceptions;

/// <summary>
/// Service Exception
/// </summary>
[Serializable]
public sealed class ServiceException(string message, Exception exception)
    : Exception(DefineMessage(message, DefaultMessage), exception)
{
    [NonSerialized]
    private const string DefaultMessage = "Ocorreu um erro ao tentar acessar o serviço.";

    /// <inheritdoc />
    public ServiceException() : this(DefaultMessage)
    {
    }

    /// <inheritdoc />
    public ServiceException(Exception innerException) : this(DefaultMessage, innerException)
    {
    }

    /// <inheritdoc />
    public ServiceException(string message) : this(message, null)
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
    static public void ThrowIf(bool condition, string message, Exception innerException = null)
    {
        if (condition)
            throw new ServiceException(message, innerException);
    }
}
