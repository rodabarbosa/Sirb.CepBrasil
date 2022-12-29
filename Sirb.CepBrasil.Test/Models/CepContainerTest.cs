using Sirb.CepBrasil.Models;
using Xunit;

namespace Sirb.CepBrasil.Test.Models
{
    public class CepContainerTest
    {
        [Fact]
        public void Constructor_Test()
        {
            var result = new CepContainer();

            Assert.Null(result.Bairro);
            Assert.Null(result.Cep);
            Assert.Null(result.Cidade);
            Assert.Null(result.Complemento);
            Assert.Null(result.Logradouro);
            Assert.Null(result.Uf);
        }

        [Theory]
        [InlineData("TEST", "00000-000")]
        public void Inline_Test(string text, string cep)
        {
            var result = new CepContainer
            {
                Uf = text,
                Cidade = text,
                Bairro = text,
                Complemento = text,
                Logradouro = text,
                Cep = cep
            };

            Assert.Equal(text, result.Bairro);
            Assert.Equal(cep, result.Cep);
            Assert.Equal(text, result.Cidade);
            Assert.Equal(text, result.Complemento);
            Assert.Equal(text, result.Logradouro);
            Assert.Equal(text, result.Uf);
        }
    }
}