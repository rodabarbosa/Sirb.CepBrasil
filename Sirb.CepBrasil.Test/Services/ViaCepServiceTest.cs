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
                var result = await service.Find(cep);
                Assert.NotNull(result);
            }
        }
    }
}
