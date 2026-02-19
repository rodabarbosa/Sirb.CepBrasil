using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sirb.CepBrasil.Extensions;

/// <summary>
/// Extensions to assist with exception handling and formatting.
/// </summary>
static public class ExceptionExtension
{
    /// <summary>
    /// Returns all error messages from the exception and its inner exceptions concatenated.
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    /// <returns>String with all error messages separated by " → ", or empty string if there are no messages.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="exception"/> is null.</exception>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // some code that may throw
    /// }
    /// catch (Exception ex)
    /// {
    ///     var messages = ex.AllMessages();
    ///     // Result: "Main error → Inner error → Deepest error"
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
    /// Returns the full exception hierarchy (the root exception plus all inner exceptions).
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    /// <returns>An enumeration of all exceptions in the chain.</returns>
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
    /// Returns all error messages with the option to include additional information such as type and stack trace.
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    /// <param name="includeStackTrace">If true, includes the stack trace for each exception.</param>
    /// <returns>Formatted string with detailed exception information.</returns>
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
