using System.Net.Http;
using System.Threading.Tasks;
using Sirb.CepBrasil.Services;
using Xunit;

namespace Sirb.CepBrasil.Test.Services
{
    public class ViaCepServiceTest
    {
        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        public async Task ViaCepService_Test(string cep)
        {
            using (var httpClient = new HttpClient())
            {
                var service = new ViaCepService(httpClient);
                var result = await service.Find(cep).ConfigureAwait(false);
                Assert.NotNull(result);
            }
        }
    }
}