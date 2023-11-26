using System;
using System.Collections.Generic;

namespace Sirb.CepBrasil.Models
{
    /// <summary>
    /// Container com resultado da busca de logradouro por CEP
    /// </summary>
    public sealed class CepResult
    {
        /// <summary>
        /// Houve sucesso na pesquisa
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Cotainer com o resultado da pesquisa
        /// </summary>
        public CepContainer CepContainer { get; }

        /// <summary>
        /// Mensagem de erro
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Lista de exceções geradas
        /// </summary>
        public List<Exception> Exceptions { get; } = new();

        /// <summary>
        /// Construtor
        /// </summary>
        public CepResult()
            : this(default, default, default)
        {
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="cepContainer"></param>
        public CepResult(CepContainer cepContainer)
            : this(true, cepContainer, default)
        {
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public CepResult(string message, Exception exception = default)
            : this(false, default, message, exception)
        {
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="success"></param>
        /// <param name="cepContainer"></param>
        /// <param name="message"></param>
        public CepResult(bool success, CepContainer cepContainer, string message)
        {
            Success = success;
            CepContainer = cepContainer;
            Message = message;
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
    }
}
