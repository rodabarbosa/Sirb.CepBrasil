using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Messages;
using System;

namespace Sirb.CepBrasil.Validations;

/// <summary>
/// Fornece métodos de validação para códigos de endereçamento postal brasileiro (CEP).
/// Responsável por validar o formato e comprimento do CEP conforme padrão brasileiro.
/// </summary>
static internal class CepValidation
{
    /// <summary>
    /// Comprimento padrão esperado de um CEP brasileiro sem formatação (8 dígitos).
    /// </summary>
    private const int ExpectedCepLength = 8;

    /// <summary>
    /// Valida um código de endereçamento postal (CEP) brasileiro de acordo com o padrão nacional.
    /// </summary>
    /// <remarks>
    /// O CEP é validado após remover qualquer máscara de formatação (hífen ou espaços).
    /// Um CEP válido deve conter exatamente 8 dígitos numéricos.
    /// </remarks>
    /// <param name="cep">CEP a ser validado. Pode estar formatado (00000-000) ou sem formatação (00000000).</param>
    /// <exception cref="ArgumentNullException">
    /// Quando <paramref name="cep"/> é nulo ou vazio após limpeza de espaços.
    /// </exception>
    /// <exception cref="ServiceException">
    /// Quando o CEP não possui exatamente 8 dígitos após remover a formatação.
    /// </exception>
    /// <example>
    /// <code>
    /// // CEP formatado
    /// CepValidation.Validate("01310-100");
    /// // CEP sem formatação
    /// CepValidation.Validate("01310100");
    ///
    /// // CEP inválido - lança ServiceException
    /// CepValidation.Validate("123");
    /// </code>
    /// </example>
    static public void Validate(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentNullException(
                nameof(cep),
                "CEP não pode ser nulo, vazio ou conter apenas espaços em branco.");

        var normalizedCep = cep.RemoveMask();

        ServiceException.ThrowIf(
            normalizedCep.Length != ExpectedCepLength,
            CepMessages.ZipCodeInvalidMessage);
    }
}
