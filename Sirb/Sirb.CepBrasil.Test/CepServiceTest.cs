using System.Net.Http;
using System.Threading.Tasks;
using Sirb.CepBrasil.Models;
using Sirb.CepBrasil.Services;
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
