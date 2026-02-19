using System;
using System.Text.RegularExpressions;

namespace Sirb.CepBrasil.Extensions;

/// <summary>
/// Extensions for manipulating and validating Brazilian postal codes (CEP).
/// </summary>
static public class CepExtension
{
    private const int CepLengthWithoutMask = 8;
    private const int CepLengthWithMask = 9;
    static private readonly Regex _removeNonDigitsRegex = new(@"[^\d]", RegexOptions.Compiled, TimeSpan.FromSeconds(15));
    static private readonly Regex _cepFormatRegex = new(@"^(\d{5})(\d{3})$", RegexOptions.Compiled, TimeSpan.FromSeconds(15));
    static private readonly Regex _cepValidationRegex = new(@"^\d{5}-?\d{3}$", RegexOptions.Compiled, TimeSpan.FromSeconds(15));

    /// <summary>
    /// Removes the CEP mask, keeping only numeric digits.
    /// </summary>
    /// <param name="value">CEP with or without mask (e.g. "01310-100" or "01310100").</param>
    /// <returns>CEP containing only digits (e.g. "01310100"), or the original value if null/whitespace.</returns>
    /// <example>
    /// <code>
    /// var formatted = "01310-100";
    /// var cleaned = formatted.RemoveMask();
    /// // Result: "01310100"
    ///
    /// var alreadyClean = "01310100";
    /// var result = alreadyClean.RemoveMask();
    /// // Result: "01310100"
    /// </code>
    /// </example>
    static public string RemoveMask(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return _removeNonDigitsRegex.Replace(value, string.Empty);
    }

    /// <summary>
    /// Applies the standard Brazilian CEP mask (format: 00000-000).
    /// </summary>
    /// <param name="value">CEP with or without mask (e.g. "01310100" or "01310-100").</param>
    /// <returns>CEP formatted with mask (e.g. "01310-100"), or the original value if invalid.</returns>
    /// <exception cref="ArgumentException">When the CEP has an invalid format (does not have 8 digits).</exception>
    /// <example>
    /// <code>
    /// var noMask = "01310100";
    /// var formatted = noMask.CepMask();
    /// // Result: "01310-100"
    ///
    /// var alreadyFormatted = "01310-100";
    /// var result = alreadyFormatted.CepMask();
    /// // Result: "01310-100"
    /// </code>
    /// </example>
    static public string CepMask(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var cleanValue = value.RemoveMask();

        if (cleanValue.Length != CepLengthWithoutMask)
            return value;

        return _cepFormatRegex.Replace(cleanValue, "$1-$2");
    }

    /// <summary>
    /// Validates whether the CEP has a valid Brazilian format (8 digits with or without mask).
    /// </summary>
    /// <param name="value">CEP to validate (e.g. "01310-100" or "01310100").</param>
    /// <returns>True if the CEP is valid, False otherwise.</returns>
    /// <remarks>
    /// This method only validates the format; it does not check if the CEP exists in postal services.
    /// Valid CEPs may have the format "00000-000" or "00000000".
    /// </remarks>
    /// <example>
    /// <code>
    /// var valid = "01310-100".IsValidCep();    // True
    /// var valid2 = "01310100".IsValidCep();    // True
    /// var invalid = "123".IsValidCep();        // False
    /// var invalid2 = "abcde-fgh".IsValidCep(); // False
    /// </code>
    /// </example>
    static public bool IsValidCep(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmedValue = value.Trim();

        // Verifica comprimento básico primeiro
        if (trimmedValue.Length != CepLengthWithoutMask && trimmedValue.Length != CepLengthWithMask)
            return false;

        return _cepValidationRegex.IsMatch(trimmedValue);
    }

    /// <summary>
    /// Returns only the digits of the CEP, removing any non-numeric characters.
    /// Alias for <see cref="RemoveMask"/> with a more descriptive name.
    /// </summary>
    /// <param name="value">Formatted CEP (e.g. "01310-100").</param>
    /// <returns>Only the CEP digits (e.g. "01310100").</returns>
    /// <example>
    /// <code>
    /// var cep = "01310-100";
    /// var digits = cep.GetDigitsOnly();
    /// // Result: "01310100"
    /// </code>
    /// </example>
    static public string GetDigitsOnly(this string value)
    {
        return value.RemoveMask();
    }

    /// <summary>
    /// Formats the CEP using the Brazilian pattern (00000-000), validating before applying the mask.
    /// </summary>
    /// <param name="value">CEP to be formatted.</param>
    /// <returns>Formatted CEP when valid, or an empty string when invalid.</returns>
    /// <remarks>
    /// Unlike <see cref="CepMask"/>, this method returns an empty string for invalid CEPs
    /// instead of returning the original value.
    /// </remarks>
    /// <example>
    /// <code>
    /// var cep = "01310100";
    /// var formatted = cep.Format();
    /// // Result: "01310-100"
    ///
    /// var invalidCep = "123";
    /// var result = invalidCep.Format();
    /// // Result: "" (empty string)
    /// </code>
    /// </example>
    static public string Format(this string value)
    {
        if (!value.IsValidCep())
            return string.Empty;

        var cleanValue = value.RemoveMask();
        return _cepFormatRegex.Replace(cleanValue, "$1-$2");
    }

    /// <summary>
    /// Normalizes the CEP by removing the mask and validating the format.
    /// </summary>
    /// <param name="value">CEP to be normalized.</param>
    /// <returns>CEP without mask when valid, or null if invalid.</returns>
    /// <example>
    /// <code>
    /// var cep = "01310-100";
    /// var normalized = cep.Normalize();
    /// // Result: "01310100"
    ///
    /// var invalidCep = "abc";
    /// var result = invalidCep.Normalize();
    /// // Result: null
    /// </code>
    /// </example>
    static public string Normalize(this string value)
    {
        if (!value.IsValidCep())
            return null;

        return value.RemoveMask();
    }
}
