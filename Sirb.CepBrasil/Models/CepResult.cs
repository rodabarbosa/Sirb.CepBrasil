using System;
using System.Collections.Generic;

namespace Sirb.CepBrasil.Models;

/// <summary>
/// Container com resultado da busca de logradouro por CEP
/// </summary>
/// <remarks>
/// Construtor
/// </remarks>
/// <param name="success"></param>
/// <param name="cepContainer"></param>
/// <param name="message"></param>
public sealed class CepResult(bool success, CepContainer cepContainer, string message)
{
    /// <summary>
    /// Construtor
    /// </summary>
    public CepResult()
        : this(false, null, null)
    {
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="cepContainer"></param>
    public CepResult(CepContainer cepContainer)
        : this(true, cepContainer, null)
    {
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public CepResult(string message, Exception exception = null)
        : this(false, null, message, exception)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="success"></param>
    /// <param name="cepContainer"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public CepResult(bool success, CepContainer cepContainer, string message, Exception exception)
        : this(success, cepContainer, message)
    {
        if (exception != null)
            Exceptions.Add(exception);
    }

    /// <summary>
    /// Houve sucesso na pesquisa
    /// </summary>
    public bool Success { get; } = success;

    /// <summary>
    /// Cotainer com o resultado da pesquisa
    /// </summary>
    public CepContainer CepContainer { get; } = cepContainer;

    /// <summary>
    /// Mensagem de erro
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    /// Lista de exceções geradas
    /// </summary>
    public List<Exception> Exceptions { get; } = new();
}
