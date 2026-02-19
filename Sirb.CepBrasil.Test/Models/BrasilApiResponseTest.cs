using System.Text.Json;

namespace Sirb.CepBrasil.Test.Models;

/// <summary>
/// Testes unitários para a classe BrasilApiResponse
/// </summary>
public sealed class BrasilApiResponseTest
{
    #region Construtor e Instanciação

    [Fact(DisplayName = "Deve criar instância da classe BrasilApiResponse")]
    public void BrasilApiResponse_QuandoCriar_DeveCriarInstancia()
    {
        // Arrange & Act
        var response = new BrasilApiResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact(DisplayName = "Deve criar instância com valores padrão null")]
    public void BrasilApiResponse_QuandoCriarSemInicializar_DeveConterValoresPadraoNull()
    {
        // Arrange & Act
        var response = new BrasilApiResponse();

        // Assert
        Assert.Null(response.Cep);
        Assert.Null(response.Uf);
        Assert.Null(response.Cidade);
        Assert.Null(response.Bairro);
        Assert.Null(response.Logradouro);
        Assert.Null(response.Service);
    }

    #endregion

    #region Propriedades

    [Theory(DisplayName = "Deve definir e retornar Cep corretamente")]
    [InlineData("01310-100")]
    [InlineData("20040-020")]
    [InlineData("30130-000")]
    [InlineData("")]
    [InlineData(null)]
    public void Cep_QuandoDefinido_DeveRetornarValor(string cep)
    {
        // Arrange
        var response = new BrasilApiResponse();

        // Act
        response.Cep = cep;

        // Assert
        Assert.Equal(cep, response.Cep);
    }

    [Theory(DisplayName = "Deve definir e retornar Uf corretamente")]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("MG")]
    [InlineData("RS")]
    [InlineData("")]
    [InlineData(null)]
    public void Uf_QuandoDefinido_DeveRetornarValor(string uf)
    {
        // Arrange
        var response = new BrasilApiResponse();

        // Act
        response.Uf = uf;

        // Assert
        Assert.Equal(uf, response.Uf);
    }

    [Theory(DisplayName = "Deve definir e retornar Cidade corretamente")]
    [InlineData("Sao Paulo")]
    [InlineData("Rio de Janeiro")]
    [InlineData("Belo Horizonte")]
    [InlineData("")]
    [InlineData(null)]
    public void Cidade_QuandoDefinida_DeveRetornarValor(string cidade)
    {
        // Arrange
        var response = new BrasilApiResponse();

        // Act
        response.Cidade = cidade;

        // Assert
        Assert.Equal(cidade, response.Cidade);
    }

    [Theory(DisplayName = "Deve definir e retornar Bairro corretamente")]
    [InlineData("Consolacao")]
    [InlineData("Centro")]
    [InlineData("Copacabana")]
    [InlineData("")]
    [InlineData(null)]
    public void Bairro_QuandoDefinido_DeveRetornarValor(string bairro)
    {
        // Arrange
        var response = new BrasilApiResponse();

        // Act
        response.Bairro = bairro;

        // Assert
        Assert.Equal(bairro, response.Bairro);
    }

    [Theory(DisplayName = "Deve definir e retornar Logradouro corretamente")]
    [InlineData("Avenida Paulista")]
    [InlineData("Rua Augusta")]
    [InlineData("Praca da Se")]
    [InlineData("")]
    [InlineData(null)]
    public void Logradouro_QuandoDefinido_DeveRetornarValor(string logradouro)
    {
        // Arrange
        var response = new BrasilApiResponse();

        // Act
        response.Logradouro = logradouro;

        // Assert
        Assert.Equal(logradouro, response.Logradouro);
    }

    [Theory(DisplayName = "Deve definir e retornar Service corretamente")]
    [InlineData("viacep")]
    [InlineData("correios")]
    [InlineData("brasilapi")]
    [InlineData("")]
    [InlineData(null)]
    public void Service_QuandoDefinido_DeveRetornarValor(string service)
    {
        // Arrange
        var response = new BrasilApiResponse();

        // Act
        response.Service = service;

        // Assert
        Assert.Equal(service, response.Service);
    }

    [Fact(DisplayName = "Deve permitir definir todas as propriedades simultaneamente")]
    public void BrasilApiResponse_QuandoDefinirTodasPropriedades_DeveRetornarTodosValores()
    {
        // Arrange
        var response = new BrasilApiResponse
        {
            Cep = "01310-100",
            Uf = "SP",
            Cidade = "Sao Paulo",
            Bairro = "Consolacao",
            Logradouro = "Avenida Paulista",
            Service = "brasilapi"
        };

        // Act & Assert
        Assert.Equal("01310-100", response.Cep);
        Assert.Equal("SP", response.Uf);
        Assert.Equal("Sao Paulo", response.Cidade);
        Assert.Equal("Consolacao", response.Bairro);
        Assert.Equal("Avenida Paulista", response.Logradouro);
        Assert.Equal("brasilapi", response.Service);
    }

    #endregion

    #region Serialização JSON

    [Fact(DisplayName = "Deve desserializar de JSON corretamente")]
    public void BrasilApiResponse_QuandoDesserializar_DeveRecriarObjeto()
    {
        // Arrange
        const string json = """
            {
                "cep": "01310-100",
                "uf": "SP",
                "city": "Sao Paulo",
                "neighborhood": "Consolacao",
                "street": "Avenida Paulista",
                "service": "brasilapi"
            }
            """;

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("01310-100", response.Cep);
        Assert.Equal("SP", response.Uf);
        Assert.Equal("Sao Paulo", response.Cidade);
        Assert.Equal("Consolacao", response.Bairro);
        Assert.Equal("Avenida Paulista", response.Logradouro);
        Assert.Equal("brasilapi", response.Service);
    }

    [Fact(DisplayName = "Deve serializar para JSON corretamente")]
    public void BrasilApiResponse_QuandoSerializar_DeveGerarJsonCorreto()
    {
        // Arrange
        var response = new BrasilApiResponse
        {
            Cep = "20040-020",
            Uf = "RJ",
            Cidade = "Rio de Janeiro",
            Bairro = "Centro",
            Logradouro = "Praca XV",
            Service = "viacep"
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.Contains("\"cep\":\"20040-020\"", json);
        Assert.Contains("\"uf\":\"RJ\"", json);
        Assert.Contains("\"city\":\"Rio de Janeiro\"", json);
        Assert.Contains("\"neighborhood\":\"Centro\"", json);
        Assert.Contains("\"street\":\"Praca XV\"", json);
        Assert.Contains("\"service\":\"viacep\"", json);
    }

    [Fact(DisplayName = "Deve desserializar JSON com campos ausentes")]
    public void BrasilApiResponse_QuandoDesserializarComCamposAusentes_DeveRetornarNull()
    {
        // Arrange
        const string json = """
            {
                "cep": "01310-100",
                "uf": "SP"
            }
            """;

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("01310-100", response.Cep);
        Assert.Equal("SP", response.Uf);
        Assert.Null(response.Cidade);
        Assert.Null(response.Bairro);
        Assert.Null(response.Logradouro);
        Assert.Null(response.Service);
    }

    [Fact(DisplayName = "Deve serializar corretamente com valores null")]
    public void BrasilApiResponse_QuandoSerializarComNull_DeveGerarJsonComNull()
    {
        // Arrange
        var response = new BrasilApiResponse
        {
            Cep = "01310-100",
            Uf = "SP",
            Cidade = null,
            Bairro = null,
            Logradouro = null,
            Service = null
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.Contains("\"cep\":\"01310-100\"", json);
        Assert.Contains("\"uf\":\"SP\"", json);
        Assert.Contains("\"city\":null", json);
        Assert.Contains("\"neighborhood\":null", json);
        Assert.Contains("\"street\":null", json);
        Assert.Contains("\"service\":null", json);
    }

    [Fact(DisplayName = "Deve usar nomes corretos de propriedades JSON conforme atributos")]
    public void BrasilApiResponse_QuandoSerializar_DeveUsarNomesJsonCorretos()
    {
        // Arrange
        var response = new BrasilApiResponse
        {
            Cep = "30130-000",
            Uf = "MG",
            Cidade = "Belo Horizonte",
            Bairro = "Centro",
            Logradouro = "Av. Afonso Pena",
            Service = "brasilapi"
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.Contains("\"cep\":", json);
        Assert.Contains("\"uf\":", json);
        Assert.Contains("\"city\":", json);
        Assert.Contains("\"neighborhood\":", json);
        Assert.Contains("\"street\":", json);
        Assert.Contains("\"service\":", json);
    }

    [Fact(DisplayName = "Deve manter dados após serialização e desserialização")]
    public void BrasilApiResponse_QuandoSerializarEDesserializar_DeveManterDados()
    {
        // Arrange
        var original = new BrasilApiResponse
        {
            Cep = "90000-000",
            Uf = "RS",
            Cidade = "Porto Alegre",
            Bairro = "Centro",
            Logradouro = "Rua dos Andradas",
            Service = "brasilapi"
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Cep, deserialized.Cep);
        Assert.Equal(original.Uf, deserialized.Uf);
        Assert.Equal(original.Cidade, deserialized.Cidade);
        Assert.Equal(original.Bairro, deserialized.Bairro);
        Assert.Equal(original.Logradouro, deserialized.Logradouro);
        Assert.Equal(original.Service, deserialized.Service);
    }

    #endregion

    #region Cenários Reais

    [Fact(DisplayName = "Deve representar resposta completa da BrasilAPI")]
    public void BrasilApiResponse_CenarioRespostaCompleta_DeveConterTodosDados()
    {
        // Arrange
        const string json = """
            {
                "cep": "01310-100",
                "uf": "SP",
                "city": "Sao Paulo",
                "neighborhood": "Bela Vista",
                "street": "Avenida Paulista",
                "service": "viacep"
            }
            """;

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("01310-100", response.Cep);
        Assert.Equal("SP", response.Uf);
        Assert.Equal("Sao Paulo", response.Cidade);
        Assert.Equal("Bela Vista", response.Bairro);
        Assert.Equal("Avenida Paulista", response.Logradouro);
        Assert.Equal("viacep", response.Service);
    }

    [Fact(DisplayName = "Deve representar resposta de CEP não encontrado")]
    public void BrasilApiResponse_CenarioCepNaoEncontrado_DeveConterCamposVazios()
    {
        // Arrange
        const string json = """
            {
                "cep": "",
                "uf": "",
                "city": "",
                "neighborhood": "",
                "street": "",
                "service": ""
            }
            """;

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("", response.Cep);
        Assert.Equal("", response.Uf);
        Assert.Equal("", response.Cidade);
        Assert.Equal("", response.Bairro);
        Assert.Equal("", response.Logradouro);
        Assert.Equal("", response.Service);
    }

    [Fact(DisplayName = "Deve representar resposta com dados parciais")]
    public void BrasilApiResponse_CenarioDadosParciais_DevePreencherApenasDisponiveis()
    {
        // Arrange
        const string json = """
            {
                "cep": "80000-000",
                "uf": "PR",
                "city": "Curitiba"
            }
            """;

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("80000-000", response.Cep);
        Assert.Equal("PR", response.Uf);
        Assert.Equal("Curitiba", response.Cidade);
        Assert.Null(response.Bairro);
        Assert.Null(response.Logradouro);
        Assert.Null(response.Service);
    }

    [Fact(DisplayName = "Deve criar instâncias independentes com mesmos valores")]
    public void BrasilApiResponse_QuandoCriarDuasInstancias_DevemSerIndependentes()
    {
        // Arrange
        var response1 = new BrasilApiResponse
        {
            Cep = "01310-100",
            Uf = "SP",
            Cidade = "Sao Paulo",
            Bairro = "Consolacao",
            Logradouro = "Av Paulista",
            Service = "brasilapi"
        };

        var response2 = new BrasilApiResponse
        {
            Cep = "01310-100",
            Uf = "SP",
            Cidade = "Sao Paulo",
            Bairro = "Consolacao",
            Logradouro = "Av Paulista",
            Service = "brasilapi"
        };

        // Act
        response2.Cep = "20040-020";

        // Assert
        Assert.NotSame(response1, response2);
        Assert.Equal("01310-100", response1.Cep);
        Assert.Equal("20040-020", response2.Cep);
    }

    [Fact(DisplayName = "Deve processar resposta real da BrasilAPI para Avenida Paulista")]
    public void BrasilApiResponse_CenarioAvenidaPaulista_DeveProcessarCorretamente()
    {
        // Arrange
        var response = new BrasilApiResponse
        {
            Cep = "01310-100",
            Uf = "SP",
            Cidade = "Sao Paulo",
            Bairro = "Bela Vista",
            Logradouro = "Avenida Paulista",
            Service = "viacep"
        };

        // Act & Assert
        Assert.Equal("01310-100", response.Cep);
        Assert.Equal("SP", response.Uf);
        Assert.Equal("Sao Paulo", response.Cidade);
        Assert.Equal("Bela Vista", response.Bairro);
        Assert.Equal("Avenida Paulista", response.Logradouro);
        Assert.Equal("viacep", response.Service);
    }

    #endregion

    #region Mapeamento de Propriedades

    [Fact(DisplayName = "Deve mapear corretamente propriedade 'uf' para 'Uf'")]
    public void BrasilApiResponse_MapeamentoUf_DeveMapeParaUf()
    {
        // Arrange
        const string json = """{"uf": "SP"}""";

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("SP", response.Uf);
    }

    [Fact(DisplayName = "Deve mapear corretamente propriedade 'city' para 'Cidade'")]
    public void BrasilApiResponse_MapeamentoCity_DeveMapeParaCidade()
    {
        // Arrange
        const string json = """{"city": "Sao Paulo"}""";

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Sao Paulo", response.Cidade);
    }

    [Fact(DisplayName = "Deve mapear corretamente propriedade 'neighborhood' para 'Bairro'")]
    public void BrasilApiResponse_MapeamentoNeighborhood_DeveMapeParaBairro()
    {
        // Arrange
        const string json = """{"neighborhood": "Centro"}""";

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Centro", response.Bairro);
    }

    [Fact(DisplayName = "Deve mapear corretamente propriedade 'street' para 'Logradouro'")]
    public void BrasilApiResponse_MapeamentoStreet_DeveMapeParaLogradouro()
    {
        // Arrange
        const string json = """{"street": "Rua Augusta"}""";

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Rua Augusta", response.Logradouro);
    }

    [Fact(DisplayName = "Deve mapear corretamente propriedade 'cep' para 'Cep'")]
    public void BrasilApiResponse_MapeamentoCep_DeveMapeParaCep()
    {
        // Arrange
        const string json = """{"cep": "01310-100"}""";

        // Act
        var response = JsonSerializer.Deserialize<BrasilApiResponse>(json);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("01310-100", response.Cep);
    }

    #endregion
}
