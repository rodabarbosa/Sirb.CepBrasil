using System.Net.Http;
using System.Threading.Tasks;
<<<<<<< HEAD
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Services;
=======
using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Validations;
>>>>>>> a22339ff84e9c636f7f89fe430499bedad3ce9eb
using Xunit;

namespace Sirb.CepBrasil.Test
{
    public sealed class CepServiceTest
    {
        private readonly HttpClient _httpClient;

        public CepServiceTest()
        {
            _httpClient = new HttpClient();
        }

        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        public async Task GetCepSuccess(string cep)
        {
            var cepService = new CepService(_httpClient);

            CepResult result = await cepService.Find(cep).ConfigureAwait(false);

            Assert.True(result.Success);
        }
    }
}
