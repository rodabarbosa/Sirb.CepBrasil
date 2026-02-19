using System;
using System.Collections.Generic;

namespace Sirb.CepBrasil.Models;

/// <summary>
/// Result container for address lookup by CEP.
/// </summary>
/// <remarks>
/// Constructors provide several convenient ways to create a result object for success/failure scenarios.
/// </remarks>
/// <param name="success">Indicates whether the lookup was successful.</param>
/// <param name="cepContainer">The <see cref="CepContainer"/> with address details when successful.</param>
/// <param name="message">Optional message describing errors or status.</param>
public sealed class CepResult(bool success, CepContainer cepContainer, string message)
{
    /// <summary>
    /// Default constructor creating an empty unsuccessful result.
    /// </summary>
    public CepResult()
        : this(false, null, null)
    {
    }

    /// <summary>
    /// Creates a successful result from a <see cref="CepContainer"/>.
    /// </summary>
    /// <param name="cepContainer">The address container with lookup results.</param>
    public CepResult(CepContainer cepContainer)
        : this(true, cepContainer, null)
    {
    }

    /// <summary>
    /// Creates an unsuccessful result with a message and an optional exception.
    /// </summary>
    /// <param name="message">Error or status message.</param>
    /// <param name="exception">Optional exception related to the failure.</param>
    public CepResult(string message, Exception exception = null)
        : this(false, null, message, exception)
    {
    }

    /// <summary>
    /// Primary constructor that accepts all result details.
    /// </summary>
    /// <param name="success">True when lookup succeeded.</param>
    /// <param name="cepContainer">Address information when successful.</param>
    /// <param name="message">Optional message describing the result.</param>
    /// <param name="exception">Optional exception captured during processing.</param>
    public CepResult(bool success, CepContainer cepContainer, string message, Exception exception)
        : this(success, cepContainer, message)
    {
        if (exception != null)
            Exceptions.Add(exception);
    }

    /// <summary>
    /// Indicates whether the lookup succeeded.
    /// </summary>
    public bool Success { get; } = success;

    /// <summary>
    /// Container with the lookup result.
    /// </summary>
    public CepContainer CepContainer { get; } = cepContainer;

    /// <summary>
    /// Message describing error or status of the lookup.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    /// List of exceptions captured during processing.
    /// </summary>
    public List<Exception> Exceptions { get; } = new();
}
