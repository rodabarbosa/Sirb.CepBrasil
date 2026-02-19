using System.Text.Json;

namespace Sirb.CepBrasil.Test.Models;

/// <summary>
/// Testes unitários para a classe CepContainer
/// </summary>
public sealed class CepContainerTest
{
    #region Imutabilidade

    /// <summary>
    /// Testa se a classe é imutável (propriedades são somente leitura)
    /// </summary>
    [Fact(DisplayName = "Deve ter todas as propriedades somente leitura")]
    public void CepContainer_QuandoCriado_DeveSerImutavel()
    {
        // Arrange
        var container = new CepContainer("SP", "São Paulo", "Centro", "complemento", "Rua A", "01000-000", "3550308");

        // Assert - Verifica que as propriedades não têm setter público
        var ufProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Uf));
        var cidadeProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Cidade));
        var bairroProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Bairro));
        var complementoProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Complemento));
        var logradouroProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Logradouro));
        var cepProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Cep));
        var ibgeProperty = typeof(CepContainer).GetProperty(nameof(CepContainer.Ibge));

        Assert.NotNull(ufProperty);
        Assert.NotNull(cidadeProperty);
        Assert.NotNull(bairroProperty);
        Assert.NotNull(complementoProperty);
        Assert.NotNull(logradouroProperty);
        Assert.NotNull(cepProperty);
        Assert.NotNull(ibgeProperty);

        Assert.Null(ufProperty.SetMethod);
        Assert.Null(cidadeProperty.SetMethod);
        Assert.Null(bairroProperty.SetMethod);
        Assert.Null(complementoProperty.SetMethod);
        Assert.Null(logradouroProperty.SetMethod);
        Assert.Null(cepProperty.SetMethod);
        Assert.Null(ibgeProperty.SetMethod);
    }

    #endregion

    #region Construtores

    /// <summary>
    /// Testa se o construtor cria uma instância com todos os parâmetros
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com todos os parâmetros quando fornecidos")]
    public void CepContainer_QuandoFornecerTodosParametros_DeveCriarInstancia()
    {
        // Arrange
        const string uf = "SP";
        const string cidade = "São Paulo";
        const string bairro = "Consolação";
        const string complemento = "lado ímpar";
        const string logradouro = "Avenida Paulista";
        const string cep = "01310-100";
        const string ibge = "3550308";

        // Act
        var container = new CepContainer(uf, cidade, bairro, complemento, logradouro, cep, ibge);

        // Assert
        Assert.NotNull(container);
        Assert.Equal(uf, container.Uf);
        Assert.Equal(cidade, container.Cidade);
        Assert.Equal(bairro, container.Bairro);
        Assert.Equal(complemento, container.Complemento);
        Assert.Equal(logradouro, container.Logradouro);
        Assert.Equal(cep, container.Cep);
        Assert.Equal(ibge, container.Ibge);
    }

    /// <summary>
    /// Testa se o construtor aceita valores null em todos os parâmetros
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com valores null quando fornecidos")]
    public void CepContainer_QuandoFornecerValoresNull_DeveCriarInstancia()
    {
        // Act
        var container = new CepContainer(null, null, null, null, null, null, null);

        // Assert
        Assert.NotNull(container);
        Assert.Null(container.Uf);
        Assert.Null(container.Cidade);
        Assert.Null(container.Bairro);
        Assert.Null(container.Complemento);
        Assert.Null(container.Logradouro);
        Assert.Null(container.Cep);
        Assert.Null(container.Ibge);
    }

    /// <summary>
    /// Testa se o construtor aceita strings vazias em todos os parâmetros
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com strings vazias quando fornecidas")]
    public void CepContainer_QuandoFornecerStringsVazias_DeveCriarInstancia()
    {
        // Act
        var container = new CepContainer("", "", "", "", "", "", "");

        // Assert
        Assert.NotNull(container);
        Assert.Equal("", container.Uf);
        Assert.Equal("", container.Cidade);
        Assert.Equal("", container.Bairro);
        Assert.Equal("", container.Complemento);
        Assert.Equal("", container.Logradouro);
        Assert.Equal("", container.Cep);
        Assert.Equal("", container.Ibge);
    }

    /// <summary>
    /// Testa se o construtor aceita valores mistos (null, vazio e preenchido)
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com valores mistos quando fornecidos")]
    public void CepContainer_QuandoFornecerValoresMistos_DeveCriarInstancia()
    {
        // Arrange
        const string uf = "RJ";
        const string cidade = "";
        const string logradouro = "Avenida Atlântica";
        const string cep = "22070-000";

        // Act
        var container = new CepContainer(uf, cidade, null, "", logradouro, cep, null);

        // Assert
        Assert.NotNull(container);
        Assert.Equal(uf, container.Uf);
        Assert.Equal(cidade, container.Cidade);
        Assert.Null(container.Bairro);
        Assert.Equal("", container.Complemento);
        Assert.Equal(logradouro, container.Logradouro);
        Assert.Equal(cep, container.Cep);
        Assert.Null(container.Ibge);
    }

    #endregion

    #region Propriedades

    /// <summary>
    /// Testa se a propriedade Uf retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Uf quando fornecida")]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("MG")]
    [InlineData("RS")]
    [InlineData("PR")]
    public void Uf_QuandoFornecida_DeveRetornarValor(string uf)
    {
        // Arrange & Act
        var container = new CepContainer(uf, "Cidade", "Bairro", "", "Rua", "00000-000", "0000000");

        // Assert
        Assert.Equal(uf, container.Uf);
    }

    /// <summary>
    /// Testa se a propriedade Cidade retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Cidade quando fornecida")]
    [InlineData("São Paulo")]
    [InlineData("Rio de Janeiro")]
    [InlineData("Belo Horizonte")]
    [InlineData("Porto Alegre")]
    [InlineData("Curitiba")]
    public void Cidade_QuandoFornecida_DeveRetornarValor(string cidade)
    {
        // Arrange & Act
        var container = new CepContainer("SP", cidade, "Bairro", "", "Rua", "00000-000", "0000000");

        // Assert
        Assert.Equal(cidade, container.Cidade);
    }

    /// <summary>
    /// Testa se a propriedade Bairro retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Bairro quando fornecido")]
    [InlineData("Consolação")]
    [InlineData("Centro")]
    [InlineData("Copacabana")]
    [InlineData("")]
    [InlineData(null)]
    public void Bairro_QuandoFornecido_DeveRetornarValor(string bairro)
    {
        // Arrange & Act
        var container = new CepContainer("SP", "Cidade", bairro, "", "Rua", "00000-000", "0000000");

        // Assert
        Assert.Equal(bairro, container.Bairro);
    }

    /// <summary>
    /// Testa se a propriedade Complemento retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Complemento quando fornecido")]
    [InlineData("lado ímpar")]
    [InlineData("lado par")]
    [InlineData("até 500")]
    [InlineData("")]
    [InlineData(null)]
    public void Complemento_QuandoFornecido_DeveRetornarValor(string complemento)
    {
        // Arrange & Act
        var container = new CepContainer("SP", "Cidade", "Bairro", complemento, "Rua", "00000-000", "0000000");

        // Assert
        Assert.Equal(complemento, container.Complemento);
    }

    /// <summary>
    /// Testa se a propriedade Logradouro retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Logradouro quando fornecido")]
    [InlineData("Avenida Paulista")]
    [InlineData("Rua Augusta")]
    [InlineData("Praça da Sé")]
    [InlineData("")]
    [InlineData(null)]
    public void Logradouro_QuandoFornecido_DeveRetornarValor(string logradouro)
    {
        // Arrange & Act
        var container = new CepContainer("SP", "Cidade", "Bairro", "", logradouro, "00000-000", "0000000");

        // Assert
        Assert.Equal(logradouro, container.Logradouro);
    }

    /// <summary>
    /// Testa se a propriedade Cep retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Cep quando fornecido")]
    [InlineData("01310-100")]
    [InlineData("20040-020")]
    [InlineData("30130-000")]
    [InlineData("00000-000")]
    public void Cep_QuandoFornecido_DeveRetornarValor(string cep)
    {
        // Arrange & Act
        var container = new CepContainer("SP", "Cidade", "Bairro", "", "Rua", cep, "0000000");

        // Assert
        Assert.Equal(cep, container.Cep);
    }

    /// <summary>
    /// Testa se a propriedade Ibge retorna o valor correto
    /// </summary>
    [Theory(DisplayName = "Deve retornar Ibge quando fornecido")]
    [InlineData("3550308")]
    [InlineData("3304557")]
    [InlineData("3106200")]
    [InlineData("4314902")]
    [InlineData("4106902")]
    public void Ibge_QuandoFornecido_DeveRetornarValor(string ibge)
    {
        // Arrange & Act
        var container = new CepContainer("SP", "Cidade", "Bairro", "", "Rua", "00000-000", ibge);

        // Assert
        Assert.Equal(ibge, container.Ibge);
    }

    #endregion

    #region Serialização JSON

    /// <summary>
    /// Testa se a serialização JSON funciona corretamente
    /// </summary>
    [Fact(DisplayName = "Deve serializar para JSON corretamente")]
    public void CepContainer_QuandoSerializado_DeveGerarJsonCorreto()
    {
        // Arrange
        var container = new CepContainer(
            "SP",
            "Sao Paulo",
            "Consolacao",
            "lado impar",
            "Avenida Paulista",
            "01310-100",
            "3550308"
        );

        // Act
        var json = JsonSerializer.Serialize(container);

        // Assert
        Assert.Contains("\"uf\":\"SP\"", json);
        Assert.Contains("\"localidade\":\"Sao Paulo\"", json);
        Assert.Contains("\"bairro\":\"Consolacao\"", json);
        Assert.Contains("\"complemento\":\"lado impar\"", json);
        Assert.Contains("\"logradouro\":\"Avenida Paulista\"", json);
        Assert.Contains("\"cep\":\"01310-100\"", json);
        Assert.Contains("\"ibge\":\"3550308\"", json);
    }

    /// <summary>
    /// Testa se a desserialização JSON funciona corretamente
    /// </summary>
    [Fact(DisplayName = "Deve desserializar de JSON corretamente")]
    public void CepContainer_QuandoDesserializado_DeveRecriarObjeto()
    {
        // Arrange
        const string json = """
                            {
                                "uf": "RJ",
                                "localidade": "Rio de Janeiro",
                                "bairro": "Copacabana",
                                "complemento": "",
                                "logradouro": "Avenida Atlântica",
                                "cep": "22070-000",
                                "ibge": "3304557"
                            }
                            """;

        // Act
        var container = JsonSerializer.Deserialize<CepContainer>(json);

        // Assert
        Assert.NotNull(container);
        Assert.Equal("RJ", container.Uf);
        Assert.Equal("Rio de Janeiro", container.Cidade);
        Assert.Equal("Copacabana", container.Bairro);
        Assert.Equal("", container.Complemento);
        Assert.Equal("Avenida Atlântica", container.Logradouro);
        Assert.Equal("22070-000", container.Cep);
        Assert.Equal("3304557", container.Ibge);
    }

    /// <summary>
    /// Testa serialização e desserialização com valores null
    /// </summary>
    [Fact(DisplayName = "Deve serializar e desserializar corretamente com valores null")]
    public void CepContainer_QuandoValoresNull_DeveSerializarEDesserializar()
    {
        // Arrange
        var container = new CepContainer(null, null, null, null, null, null, null);

        // Act
        var json = JsonSerializer.Serialize(container);
        var deserialized = JsonSerializer.Deserialize<CepContainer>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Uf);
        Assert.Null(deserialized.Cidade);
        Assert.Null(deserialized.Bairro);
        Assert.Null(deserialized.Complemento);
        Assert.Null(deserialized.Logradouro);
        Assert.Null(deserialized.Cep);
        Assert.Null(deserialized.Ibge);
    }

    /// <summary>
    /// Testa se os nomes das propriedades JSON estão corretos conforme atributos
    /// </summary>
    [Fact(DisplayName = "Deve usar nomes corretos de propriedades JSON conforme atributos")]
    public void CepContainer_QuandoSerializado_DeveUsarNomesJsonCorretos()
    {
        // Arrange
        var container = new CepContainer("MG", "Belo Horizonte", "Centro", "", "Av. Afonso Pena", "30130-000", "3106200");

        // Act
        var json = JsonSerializer.Serialize(container);

        // Assert
        Assert.Contains("\"uf\":", json);
        Assert.Contains("\"localidade\":", json);
        Assert.Contains("\"bairro\":", json);
        Assert.Contains("\"complemento\":", json);
        Assert.Contains("\"logradouro\":", json);
        Assert.Contains("\"cep\":", json);
        Assert.Contains("\"ibge\":", json);
    }

    #endregion

    #region Cenários Reais

    /// <summary>
    /// Testa cenário completo com endereço real da Avenida Paulista
    /// </summary>
    [Fact(DisplayName = "Deve representar corretamente endereço da Avenida Paulista")]
    public void CepContainer_CenarioAvenidaPaulista_DeveConterDadosCompletos()
    {
        // Arrange & Act
        var container = new CepContainer(
            "SP",
            "São Paulo",
            "Consolação",
            "lado ímpar",
            "Avenida Paulista",
            "01310-100",
            "3550308"
        );

        // Assert
        Assert.Equal("SP", container.Uf);
        Assert.Equal("São Paulo", container.Cidade);
        Assert.Equal("Consolação", container.Bairro);
        Assert.Equal("lado ímpar", container.Complemento);
        Assert.Equal("Avenida Paulista", container.Logradouro);
        Assert.Equal("01310-100", container.Cep);
        Assert.Equal("3550308", container.Ibge);
    }

    /// <summary>
    /// Testa cenário de CEP genérico sem bairro e complemento
    /// </summary>
    [Fact(DisplayName = "Deve representar corretamente CEP genérico sem bairro e complemento")]
    public void CepContainer_CenarioCepGenerico_DeveAceitarCamposVazios()
    {
        // Arrange & Act
        var container = new CepContainer(
            "PR",
            "Curitiba",
            "",
            "",
            "Rua XV de Novembro",
            "80020-000",
            "4106902"
        );

        // Assert
        Assert.Equal("PR", container.Uf);
        Assert.Equal("Curitiba", container.Cidade);
        Assert.Equal("", container.Bairro);
        Assert.Equal("", container.Complemento);
        Assert.Equal("Rua XV de Novembro", container.Logradouro);
        Assert.Equal("80020-000", container.Cep);
        Assert.Equal("4106902", container.Ibge);
    }

    /// <summary>
    /// Testa cenário de CEP especial sem logradouro
    /// </summary>
    [Fact(DisplayName = "Deve representar corretamente CEP especial sem logradouro")]
    public void CepContainer_CenarioCepEspecial_DeveAceitarLogradouroVazio()
    {
        // Arrange & Act
        var container = new CepContainer(
            "RS",
            "Porto Alegre",
            "Centro",
            "Caixa Postal",
            "",
            "90000-000",
            "4314902"
        );

        // Assert
        Assert.Equal("RS", container.Uf);
        Assert.Equal("Porto Alegre", container.Cidade);
        Assert.Equal("Centro", container.Bairro);
        Assert.Equal("Caixa Postal", container.Complemento);
        Assert.Equal("", container.Logradouro);
        Assert.Equal("90000-000", container.Cep);
        Assert.Equal("4314902", container.Ibge);
    }

    /// <summary>
    /// Testa se dois objetos com mesmos valores são iguais em referência quando criados separadamente
    /// </summary>
    [Fact(DisplayName = "Deve criar instâncias independentes com mesmos valores")]
    public void CepContainer_QuandoCriarDuasInstanciasComMesmosValores_DevemSerIndependentes()
    {
        // Arrange
        var container1 = new CepContainer("SP", "São Paulo", "Centro", "", "Rua A", "01000-000", "3550308");
        var container2 = new CepContainer("SP", "São Paulo", "Centro", "", "Rua A", "01000-000", "3550308");

        // Assert
        Assert.NotSame(container1, container2);
        Assert.Equal(container1.Uf, container2.Uf);
        Assert.Equal(container1.Cidade, container2.Cidade);
        Assert.Equal(container1.Bairro, container2.Bairro);
        Assert.Equal(container1.Complemento, container2.Complemento);
        Assert.Equal(container1.Logradouro, container2.Logradouro);
        Assert.Equal(container1.Cep, container2.Cep);
        Assert.Equal(container1.Ibge, container2.Ibge);
    }

    #endregion
}
