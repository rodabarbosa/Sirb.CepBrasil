using Sirb.CepBrasil.Models;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Interfaces
{
    internal interface ICepServiceControl
    {
        /// <summary>
        /// Find location by zip code. Internal usage intended.
        /// </summary>
        /// <param name="cep"></param>
        /// <returns></returns>
        Task<CepContainer> Find(string cep);
    }
}
