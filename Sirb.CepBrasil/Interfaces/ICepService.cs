using Sirb.CepBrasil.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Interfaces;

/// <summary>
/// Interface for brazilian Zipcode service
/// </summary>
public interface ICepService
{
    /// <summary>
    /// Find location by zip code. Internal usage intended.
    ///
    /// Método para buscar Logradouro por CEP
    /// </summary>
    /// <param name="cep"></param>
    /// <remarks>
    /// It will use a 30 seconds token. If requires more than that, use FindAsync instead passing a custom CancellationToken.
    /// </remarks>
    /// <returns></returns>
    [Obsolete("This method is obsolete. Use FindAsync instead.")]
    Task<CepResult> Find(string cep);

    /// <summary>
    /// Find location by zip code. Internal usage intended.
    ///
    /// Método para buscar Logradouro por CEP
    /// </summary>
    /// <param name="cep"></param>
    /// <param name="cancellationToken">With default value, will be considered a 30 seconds token.</param>
    /// <returns></returns>
    Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken);
}
