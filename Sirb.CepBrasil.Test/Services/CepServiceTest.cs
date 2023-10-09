namespace Sirb.CepBrasil.Test.Services
{
    public sealed class CepServiceTest
    {
        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        [InlineData("00000-000")]
        public async Task GetCepSuccess_NoHttp(string cep)
        {
            using (var service = new CepService())
            {
                var result = await service.Find(cep);
                Assert.True(result.Success);
            }
        }

        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        public async Task GetCepSuccess_WithHttp(string cep)
        {
            using (var httpClient = new HttpClient())
            {
                using (var service = new CepService(httpClient))
                {
                    var result = await service.Find(cep);
                    Assert.True(result.Success);
                }
            }
        }
    }
}
