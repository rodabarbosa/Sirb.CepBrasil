using System;
using System.Text.RegularExpressions;

namespace Sirb.CepBrasil.Extensions;

/// <summary>
/// Extensões para manipulação e validação de CEP (Código de Endereçamento Postal brasileiro).
/// </summary>
static public class CepExtension
{
    private const int CepLengthWithoutMask = 8;
    private const int CepLengthWithMask = 9;
    static private readonly Regex _removeNonDigitsRegex = new(@"[^\d]", RegexOptions.Compiled, TimeSpan.FromSeconds(15));
    static private readonly Regex _cepFormatRegex = new(@"^(\d{5})(\d{3})$", RegexOptions.Compiled, TimeSpan.FromSeconds(15));
    static private readonly Regex _cepValidationRegex = new(@"^\d{5}-?\d{3}$", RegexOptions.Compiled, TimeSpan.FromSeconds(15));

    /// <summary>
    /// Remove a máscara do CEP, mantendo apenas os dígitos numéricos.
    /// </summary>
    /// <param name="value">CEP com ou sem máscara (ex: "01310-100" ou "01310100").</param>
    /// <returns>CEP contendo apenas dígitos (ex: "01310100"), ou o valor original se for nulo/vazio.</returns>
    /// <example>
    /// <code>
    /// var cepFormatado = "01310-100";
    /// var cepLimpo = cepFormatado.RemoveMask();
    /// // Resultado: "01310100"
    ///
    /// var cepJaLimpo = "01310100";
    /// var resultado = cepJaLimpo.RemoveMask();
    /// // Resultado: "01310100"
    /// </code>
    /// </example>
    static public string RemoveMask(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return _removeNonDigitsRegex.Replace(value, string.Empty);
    }

    /// <summary>
    /// Aplica a máscara padrão brasileira no CEP (formato: 00000-000).
    /// </summary>
    /// <param name="value">CEP com ou sem máscara (ex: "01310100" ou "01310-100").</param>
    /// <returns>CEP formatado com máscara (ex: "01310-100"), ou o valor original se inválido.</returns>
    /// <exception cref="ArgumentException">Quando o CEP tem formato inválido (não tem 8 dígitos).</exception>
    /// <example>
    /// <code>
    /// var cepSemMascara = "01310100";
    /// var cepFormatado = cepSemMascara.CepMask();
    /// // Resultado: "01310-100"
    ///
    /// var cepJaFormatado = "01310-100";
    /// var resultado = cepJaFormatado.CepMask();
    /// // Resultado: "01310-100"
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
    /// Valida se o CEP está em formato válido brasileiro (8 dígitos com ou sem máscara).
    /// </summary>
    /// <param name="value">CEP a ser validado (ex: "01310-100" ou "01310100").</param>
    /// <returns>True se o CEP é válido, False caso contrário.</returns>
    /// <remarks>
    /// Este método apenas valida o formato, não verifica se o CEP existe nos Correios.
    /// CEPs válidos podem ter formato "00000-000" ou "00000000".
    /// </remarks>
    /// <example>
    /// <code>
    /// var cepValido = "01310-100".IsValidCep();    // True
    /// var cepValido2 = "01310100".IsValidCep();    // True
    /// var cepInvalido = "123".IsValidCep();        // False
    /// var cepInvalido2 = "abcde-fgh".IsValidCep(); // False
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
    /// Retorna apenas os dígitos do CEP, removendo qualquer caractere não numérico.
    /// Alias para RemoveMask() com nome mais descritivo.
    /// </summary>
    /// <param name="value">CEP com formatação (ex: "01310-100").</param>
    /// <returns>Apenas os dígitos do CEP (ex: "01310100").</returns>
    /// <example>
    /// <code>
    /// var cep = "01310-100";
    /// var digitos = cep.GetDigitsOnly();
    /// // Resultado: "01310100"
    /// </code>
    /// </example>
    static public string GetDigitsOnly(this string value)
    {
        return value.RemoveMask();
    }

    /// <summary>
    /// Formata o CEP no padrão brasileiro (00000-000), validando antes de aplicar a máscara.
    /// </summary>
    /// <param name="value">CEP a ser formatado.</param>
    /// <returns>CEP formatado se válido, ou string vazia se inválido.</returns>
    /// <remarks>
    /// Diferente de CepMask(), este método retorna string vazia para CEPs inválidos
    /// em vez de retornar o valor original.
    /// </remarks>
    /// <example>
    /// <code>
    /// var cep = "01310100";
    /// var formatado = cep.Format();
    /// // Resultado: "01310-100"
    ///
    /// var cepInvalido = "123";
    /// var resultado = cepInvalido.Format();
    /// // Resultado: "" (string vazia)
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
    /// Normaliza o CEP removendo a máscara e validando o formato.
    /// </summary>
    /// <param name="value">CEP a ser normalizado.</param>
    /// <returns>CEP sem máscara se válido, ou null se inválido.</returns>
    /// <example>
    /// <code>
    /// var cep = "01310-100";
    /// var normalizado = cep.Normalize();
    /// // Resultado: "01310100"
    ///
    /// var cepInvalido = "abc";
    /// var resultado = cepInvalido.Normalize();
    /// // Resultado: null
    /// </code>
    /// </example>
    static public string Normalize(this string value)
    {
        if (!value.IsValidCep())
            return null;

        return value.RemoveMask();
    }
}
