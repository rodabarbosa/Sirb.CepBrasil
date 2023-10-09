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
        public bool Success { get; set; }

        /// <summary>
        /// Cotainer com o resultado da pesquisa
        /// </summary>
        public CepContainer CepContainer { get; set; }

        /// <summary>
        /// Mensagem de erro
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Lista de exceções geradas
        /// </summary>
        public List<Exception> Exceptions { get; } = new();

        public CepResult()
            : this(default, default, default)
        {
        }

        public CepResult(CepContainer cepContainer)
            : this(true, cepContainer, default)
        {
        }

        public CepResult(string message, Exception exception = default)
            : this(false, default, message, exception)
        {
        }

        public CepResult(bool success, CepContainer cepContainer, string message)
        {
            Success = success;
            CepContainer = cepContainer;
            Message = message;
        }

        public CepResult(bool success, CepContainer cepContainer, string message, Exception exception)
            : this(success, cepContainer, message)
        {
            if (exception != null)
                Exceptions.Add(exception);
        }
    }
}
