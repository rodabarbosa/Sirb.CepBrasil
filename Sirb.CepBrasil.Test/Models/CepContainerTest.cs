namespace Sirb.CepBrasil.Test.Models;

public class CepContainerTest
{
    [Fact]
    public void Constructor_Test()
    {
        CepContainer result = new(default, default, default, default, default, default);

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
        CepContainer result = new(text, text, text, text, text, cep);

        Assert.Equal(text, result.Bairro);
        Assert.Equal(cep, result.Cep);
        Assert.Equal(text, result.Cidade);
        Assert.Equal(text, result.Complemento);
        Assert.Equal(text, result.Logradouro);
        Assert.Equal(text, result.Uf);
    }
}
