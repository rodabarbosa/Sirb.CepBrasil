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
        public List<Exception> Exceptions { get; private set; } = new List<Exception>();
    }
}
