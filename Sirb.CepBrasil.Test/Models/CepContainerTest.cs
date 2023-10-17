namespace Sirb.CepBrasil.Test.Models
{
    public class CepContainerTest
    {
        [Fact]
        public void Constructor_Test()
        {
            CepContainer result = new(default, default, default, default, default, default);

            result.Bairro
                .Should()
                .BeNull();

            result.Cep
                .Should()
                .BeNull();

            result.Cidade
                .Should()
                .BeNull();

            result.Complemento
                .Should()
                .BeNull();

            result.Logradouro
                .Should()
                .BeNull();

            result.Uf
                .Should()
                .BeNull();
        }

        [Theory]
        [InlineData("TEST", "00000-000")]
        public void Inline_Test(string text, string cep)
        {
            CepContainer result = new(text, text, text, text, text, cep);

            result.Bairro
                .Should()
                .Be(text);

            result.Cep
                .Should()
                .Be(cep);

            result.Cidade
                .Should()
                .Be(text);

            result.Complemento
                .Should()
                .Be(text);

            result.Logradouro
                .Should()
                .Be(text);

            result.Uf
                .Should()
                .Be(text);
        }
    }
}
