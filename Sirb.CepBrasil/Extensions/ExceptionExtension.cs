using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirb.CepBrasil.Extensions;

/// <summary>
/// Extensões para facilitar manipulação e formatação de exceções.
/// </summary>
static public class ExceptionExtension
{
    /// <summary>
    /// Retorna todas as mensagens de erro da exceção e suas exceções internas concatenadas.
    /// </summary>
    /// <param name="exception">A exceção a ser processada.</param>
    /// <returns>String contendo todas as mensagens de erro separadas por " → ", ou string vazia se exception é null.</returns>
    /// <exception cref="ArgumentNullException">Quando exception é nulo.</exception>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // algum código que lança exceção
    /// }
    /// catch (Exception ex)
    /// {
    ///     var messages = ex.AllMessages();
    ///     // Resultado: "Erro principal → Erro interno → Erro mais profundo"
    /// }
    /// </code>
    /// </example>
    static public string AllMessages(this Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var messages = GetExceptionChain(exception)
            .Select(e => e.Message)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .ToList();

        return messages.Count == 0
            ? string.Empty
            : string.Join(" → ", messages);
    }

    /// <summary>
    /// Retorna a hierarquia completa de exceções (exceção principal + todas as exceções internas).
    /// </summary>
    /// <param name="exception">A exceção a ser processada.</param>
    /// <returns>Enumeração de todas as exceções da cadeia.</returns>
    /// <example>
    /// <code>
    /// var chain = ex.GetExceptionChain();
    /// foreach (var exc in chain)
    /// {
    ///     Console.WriteLine($"Tipo: {exc.GetType().Name}, Mensagem: {exc.Message}");
    /// }
    /// </code>
    /// </example>
    static private IEnumerable<Exception> GetExceptionChain(Exception exception)
    {
        var current = exception;
        while (current != null)
        {
            yield return current;

            current = current.InnerException;
        }
    }

    /// <summary>
    /// Retorna todas as mensagens de erro com opção de incluir informações adicionais como tipo e stack trace.
    /// </summary>
    /// <param name="exception">A exceção a ser processada.</param>
    /// <param name="includeStackTrace">Se true, inclui stack trace de cada exceção.</param>
    /// <returns>String formatada com informações detalhadas da exceção.</returns>
    /// <example>
    /// <code>
    /// var detailedInfo = ex.GetDetailedMessage(includeStackTrace: true);
    /// Console.WriteLine(detailedInfo);
    /// </code>
    /// </example>
    static public string GetDetailedMessage(this Exception exception, bool includeStackTrace = false)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var sb = new StringBuilder();
        var exceptions = GetExceptionChain(exception).ToList();

        for (var i = 0; i < exceptions.Count; i++)
        {
            var exc = exceptions[i];

            // Adiciona nível da exceção
            if (i > 0)
                sb.AppendLine();

            sb.Append($"[{exc.GetType().Name}] {exc.Message}");

            // Adiciona stack trace se solicitado
            if (includeStackTrace && !string.IsNullOrEmpty(exc.StackTrace))
                sb.AppendLine().Append($"StackTrace: {exc.StackTrace}");
        }

        return sb.ToString();
    }
}
