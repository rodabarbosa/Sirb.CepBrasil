using Sirb.CepBrasil.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Interfaces;

internal interface ICepServiceControl
{
    /// <summary>
    /// Find location by zip code. Internal usage intended.
    /// </summary>
    /// <param name="cep"></param>
    /// <param name="cancellationToken">With default value, will be considered a 30 seconds token.</param>
    /// <returns></returns>
    Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken);
}
