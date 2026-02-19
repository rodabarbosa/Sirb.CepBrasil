using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Messages;
using System;
using System.Linq;

namespace Sirb.CepBrasil.Validations;

/// <summary>
/// Provides validation methods for Brazilian postal codes (CEP).
/// Responsible for validating CEP format and length according to the Brazilian standard.
/// </summary>
static internal class CepValidation
{
    /// <summary>
    /// Expected length for an unformatted Brazilian CEP (8 digits).
    /// </summary>
    private const int ExpectedCepLength = 8;

    /// <summary>
    /// Validates a Brazilian postal code (CEP) according to the national standard.
    /// </summary>
    /// <remarks>
    /// The CEP is validated after removing any formatting mask (hyphen or spaces).
    /// A valid CEP must contain exactly 8 numeric digits.
    /// </remarks>
    /// <param name="cep">CEP to validate. May be formatted (00000-000) or unformatted (00000000).</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="cep"/> is null or whitespace after trimming.
    /// </exception>
    /// <exception cref="ServiceException">
    /// Thrown when the CEP does not contain exactly 8 digits after removing formatting.
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
                "CEP cannot be null, empty, or contain only whitespace.");

        // Trim input for format checks but preserve original for normalization
        var trimmed = cep.Trim();

        // Only digits and optional hyphens are allowed at this stage
        foreach (var ch in trimmed)
        {
            if (!char.IsDigit(ch) && ch != '-')
                throw new ServiceException(CepMessages.ZipCodeInvalidMessage);
        }

        // Reject leading or trailing hyphen
        if (trimmed.StartsWith('-') || trimmed.EndsWith('-'))
            throw new ServiceException(CepMessages.ZipCodeInvalidMessage);

        // Reject consecutive hyphens
        if (trimmed.Contains("--"))
            throw new ServiceException(CepMessages.ZipCodeInvalidMessage);

        // If there's exactly one hyphen, it must be in the standard position (after 5 digits)
        var hyphenCount = trimmed.Count(c => c == '-');
        if (hyphenCount == 1 && (trimmed.Length <= 5 || trimmed[5] != '-'))
            throw new ServiceException(CepMessages.ZipCodeInvalidMessage);

        var normalizedCep = cep.RemoveMask();

        ServiceException.ThrowIf(
            normalizedCep.Length != ExpectedCepLength,
            CepMessages.ZipCodeInvalidMessage);
    }
}
