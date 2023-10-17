using Sirb.CepBrasil.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Interfaces
{
    internal interface ICepServiceControl
    {
        /// <summary>
        /// Find location by zip code. Internal usage intended.
        /// </summary>
        /// <param name="cep"></param>
        /// <remarks>
        /// It will use a 30 seconds token. If requires more than that, use FindAsync instead passing a custom CancellationToken.
        /// </remarks>
        /// <returns></returns>
        [Obsolete("Use FindAsync instead.")]
        Task<CepContainer> Find(string cep);

        /// <summary>
        /// Find location by zip code. Internal usage intended.
        /// </summary>
        /// <param name="cep"></param>
        /// <param name="cancellationToken">With default value, will be considered a 30 seconds token.</param>
        /// <returns></returns>
        Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken);
    }
}
