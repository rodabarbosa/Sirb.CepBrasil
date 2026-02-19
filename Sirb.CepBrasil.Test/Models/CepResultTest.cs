namespace Sirb.CepBrasil.Test.Models;

/// <summary>
/// Testes unitários para a classe CepResult
/// </summary>
public sealed class CepResultTest
{
    #region Construtores

    /// <summary>
    /// Testa se o construtor padrão cria uma instância com valores padrão
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com valores padrão quando usar construtor sem parâmetros")]
    public void CepResult_QuandoUsarConstrutorPadrao_DeveCriarComValoresPadrao()
    {
        // Act
        var result = new CepResult();

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.Null(result.Message);
        Assert.NotNull(result.Exceptions);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor com CepContainer cria uma instância de sucesso
    /// </summary>
    [Fact(DisplayName = "Deve criar instância de sucesso quando fornecer CepContainer")]
    public void CepResult_QuandoFornecerCepContainer_DeveCriarComSucesso()
    {
        // Arrange
        var container = new CepContainer(
            "SP",
            "São Paulo",
            "Consolação",
            "lado ímpar",
            "Avenida Paulista",
            "01310-100",
            "3550308"
        );

        // Act
        var result = new CepResult(container);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
        Assert.Same(container, result.CepContainer);
        Assert.Null(result.Message);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor com CepContainer null funciona corretamente
    /// </summary>
    [Fact(DisplayName = "Deve criar instância de sucesso mesmo com CepContainer null")]
    public void CepResult_QuandoFornecerCepContainerNull_DeveCriarComSucesso()
    {
        // Arrange & Act
        var result = new CepResult(null);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Null(result.CepContainer);
        Assert.Null(result.Message);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor com mensagem de erro cria uma instância de falha
    /// </summary>
    [Theory(DisplayName = "Deve criar instância de falha quando fornecer mensagem de erro")]
    [InlineData("CEP não encontrado")]
    [InlineData("Erro ao consultar serviço")]
    [InlineData("CEP inválido")]
    public void CepResult_QuandoFornecerMensagemErro_DeveCriarComFalha(string mensagemErro)
    {
        // Act
        var result = new CepResult(mensagemErro);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.Equal(mensagemErro, result.Message);
        Assert.NotNull(result.Exceptions);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor adiciona exceção à lista quando fornecida
    /// </summary>
    [Fact(DisplayName = "Deve adicionar exceção à lista quando fornecida junto com mensagem")]
    public void CepResult_QuandoFornecerExcecao_DeveAdicionarNaLista()
    {
        // Arrange
        const string mensagemErro = "Erro ao consultar CEP";
        var exception = new InvalidOperationException("Serviço indisponível");

        // Act
        var result = new CepResult(mensagemErro, exception);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.Equal(mensagemErro, result.Message);
        Assert.NotNull(result.Exceptions);
        Assert.NotEmpty(result.Exceptions);
        Assert.Single(result.Exceptions);
        Assert.Contains(exception, result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor não adiciona exceção quando null
    /// </summary>
    [Fact(DisplayName = "Deve manter lista de exceções vazia quando exceção for null")]
    public void CepResult_QuandoExcecaoNull_DeveManterListaVazia()
    {
        // Arrange
        const string mensagemErro = "Erro ao consultar CEP";

        // Act
        var result = new CepResult(mensagemErro);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(mensagemErro, result.Message);
        Assert.NotNull(result.Exceptions);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor completo cria instância com todos os parâmetros
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com todos os parâmetros quando usar construtor completo")]
    public void CepResult_QuandoUsarConstrutorCompleto_DeveCriarComTodosParametros()
    {
        // Arrange
        var container = new CepContainer(
            "RJ",
            "Rio de Janeiro",
            "Copacabana",
            "",
            "Avenida Atlântica",
            "22070-000",
            "3304557"
        );
        const string mensagem = "Consulta realizada com sucesso";
        var exception = new Exception("Aviso");

        // Act
        var result = new CepResult(true, container, mensagem, exception);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
        Assert.Same(container, result.CepContainer);
        Assert.Equal(mensagem, result.Message);
        Assert.NotNull(result.Exceptions);
        Assert.NotEmpty(result.Exceptions);
        Assert.Single(result.Exceptions);
        Assert.Contains(exception, result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor completo não adiciona exceção quando null
    /// </summary>
    [Fact(DisplayName = "Deve manter lista vazia quando construtor completo receber exceção null")]
    public void CepResult_QuandoConstrutorCompletoComExcecaoNull_DeveManterListaVazia()
    {
        // Arrange
        var container = new CepContainer(
            "MG",
            "Belo Horizonte",
            "Centro",
            "",
            "Avenida Afonso Pena",
            "30130-000",
            "3106200"
        );
        const string mensagem = "Sucesso";

        // Act
        var result = new CepResult(false, container, mensagem, null);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Same(container, result.CepContainer);
        Assert.Equal(mensagem, result.Message);
        Assert.NotNull(result.Exceptions);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se o construtor intermediário (bool, CepContainer, string) funciona corretamente
    /// </summary>
    [Fact(DisplayName = "Deve criar instância com construtor intermediário sem exceção")]
    public void CepResult_QuandoUsarConstrutorIntermediario_DeveCriarSemExcecao()
    {
        // Arrange
        var container = new CepContainer("PR", "Curitiba", "Centro", "", "Rua XV", "80000-000", "4106902");
        const string mensagem = "Teste";

        // Act
        var result = new CepResult(true, container, mensagem);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Same(container, result.CepContainer);
        Assert.Equal(mensagem, result.Message);
        Assert.Empty(result.Exceptions);
    }

    #endregion

    #region Propriedades

    /// <summary>
    /// Testa se a propriedade Success retorna o valor correto para sucesso
    /// </summary>
    [Fact(DisplayName = "Deve retornar Success true quando resultado for bem-sucedido")]
    public void Success_QuandoResultadoBemSucedido_DeveRetornarTrue()
    {
        // Arrange
        var container = new CepContainer("SP", "São Paulo", "Centro", "", "Rua A", "01000-000", "3550308");

        // Act
        var result = new CepResult(container);

        // Assert
        Assert.True(result.Success);
    }

    /// <summary>
    /// Testa se a propriedade Success retorna o valor correto para falha
    /// </summary>
    [Fact(DisplayName = "Deve retornar Success false quando resultado falhar")]
    public void Success_QuandoResultadoFalhar_DeveRetornarFalse()
    {
        // Arrange & Act
        var result = new CepResult("Erro");

        // Assert
        Assert.False(result.Success);
    }

    /// <summary>
    /// Testa se a propriedade CepContainer retorna o container correto
    /// </summary>
    [Fact(DisplayName = "Deve retornar CepContainer quando fornecido")]
    public void CepContainer_QuandoFornecido_DeveRetornarContainer()
    {
        // Arrange
        var container = new CepContainer("RS", "Porto Alegre", "Centro", "", "Rua B", "90000-000", "4314902");

        // Act
        var result = new CepResult(container);

        // Assert
        Assert.NotNull(result.CepContainer);
        Assert.Same(container, result.CepContainer);
        Assert.Equal("RS", result.CepContainer.Uf);
        Assert.Equal("Porto Alegre", result.CepContainer.Cidade);
    }

    /// <summary>
    /// Testa se a propriedade CepContainer é null quando não fornecida
    /// </summary>
    [Fact(DisplayName = "Deve retornar CepContainer null quando não fornecido")]
    public void CepContainer_QuandoNaoFornecido_DeveRetornarNull()
    {
        // Arrange & Act
        var result = new CepResult("Erro");

        // Assert
        Assert.Null(result.CepContainer);
    }

    /// <summary>
    /// Testa se a propriedade Message retorna a mensagem correta
    /// </summary>
    [Theory(DisplayName = "Deve retornar Message quando fornecida")]
    [InlineData("CEP inválido")]
    [InlineData("Serviço indisponível")]
    [InlineData("")]
    public void Message_QuandoFornecida_DeveRetornarMensagem(string mensagem)
    {
        // Act
        var result = new CepResult(mensagem);

        // Assert
        Assert.Equal(mensagem, result.Message);
    }

    /// <summary>
    /// Testa se a propriedade Message é null quando não fornecida
    /// </summary>
    [Fact(DisplayName = "Deve retornar Message null quando não fornecida")]
    public void Message_QuandoNaoFornecida_DeveRetornarNull()
    {
        // Arrange
        var container = new CepContainer("SP", "São Paulo", "", "", "", "01000-000", "3550308");

        // Act
        var result = new CepResult(container);

        // Assert
        Assert.Null(result.Message);
    }

    /// <summary>
    /// Testa se a propriedade Exceptions contém as exceções adicionadas
    /// </summary>
    [Fact(DisplayName = "Deve retornar lista de exceções quando exceções forem adicionadas")]
    public void Exceptions_QuandoExcecoesAdicionadas_DeveRetornarLista()
    {
        // Arrange
        var exception1 = new InvalidOperationException("Erro 1");
        var exception2 = new ArgumentException("Erro 2");

        // Act
        var result = new CepResult("Múltiplos erros", exception1);
        result.Exceptions.Add(exception2);

        // Assert
        Assert.Equal(2, result.Exceptions.Count);
        Assert.Contains(exception1, result.Exceptions);
        Assert.Contains(exception2, result.Exceptions);
    }

    /// <summary>
    /// Testa se a propriedade Exceptions está vazia quando nenhuma exceção é adicionada
    /// </summary>
    [Fact(DisplayName = "Deve retornar lista vazia quando nenhuma exceção for adicionada")]
    public void Exceptions_QuandoNenhumaExcecao_DeveRetornarListaVazia()
    {
        // Arrange & Act
        var result = new CepResult();

        // Assert
        Assert.NotNull(result.Exceptions);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa se é possível adicionar exceções manualmente à lista
    /// </summary>
    [Fact(DisplayName = "Deve permitir adicionar exceções manualmente à lista")]
    public void Exceptions_QuandoAdicionarManualmente_DeveConterExcecao()
    {
        // Arrange
        var result = new CepResult();
        var exception = new Exception("Erro manual");

        // Act
        result.Exceptions.Add(exception);

        // Assert
        Assert.Single(result.Exceptions);
        Assert.Contains(exception, result.Exceptions);
    }

    #endregion

    #region Cenários de Uso Real

    /// <summary>
    /// Testa cenário de sucesso completo com todos os dados
    /// </summary>
    [Fact(DisplayName = "Deve criar resultado de sucesso completo com CEP válido")]
    public void CepResult_CenarioSucessoCompleto_DeveTerTodosDados()
    {
        // Arrange
        var container = new CepContainer(
            "SP",
            "São Paulo",
            "Consolação",
            "lado ímpar",
            "Avenida Paulista",
            "01310-100",
            "3550308"
        );

        // Act
        var result = new CepResult(container);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
        Assert.Equal("01310-100", result.CepContainer.Cep);
        Assert.Equal("São Paulo", result.CepContainer.Cidade);
        Assert.Equal("SP", result.CepContainer.Uf);
        Assert.Equal("Avenida Paulista", result.CepContainer.Logradouro);
        Assert.Equal("Consolação", result.CepContainer.Bairro);
        Assert.Null(result.Message);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa cenário de falha com mensagem de erro específica
    /// </summary>
    [Fact(DisplayName = "Deve criar resultado de falha com mensagem de CEP não encontrado")]
    public void CepResult_CenarioFalhaCepNaoEncontrado_DeveTerMensagemErro()
    {
        // Arrange
        const string mensagem = "CEP não encontrado em nenhum serviço";

        // Act
        var result = new CepResult(mensagem);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.Equal(mensagem, result.Message);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa cenário de falha com exceção de serviço indisponível
    /// </summary>
    [Fact(DisplayName = "Deve criar resultado de falha com exceção de serviço indisponível")]
    public void CepResult_CenarioServicoIndisponivel_DeveTerExcecao()
    {
        // Arrange
        const string mensagem = "Todos os serviços estão indisponíveis";
        var exception = new InvalidOperationException("Timeout ao consultar API");

        // Act
        var result = new CepResult(mensagem, exception);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.Equal(mensagem, result.Message);
        Assert.Single(result.Exceptions);
        Assert.IsType<InvalidOperationException>(result.Exceptions[0]);
    }

    /// <summary>
    /// Testa cenário de múltiplas falhas em diferentes serviços
    /// </summary>
    [Fact(DisplayName = "Deve acumular múltiplas exceções de diferentes serviços")]
    public void CepResult_CenarioMultiplasExcecoes_DeveAcumularTodas()
    {
        // Arrange
        var exception1 = new InvalidOperationException("BrasilAPI falhou");
        var exception2 = new TimeoutException("ViaCEP timeout");
        var exception3 = new HttpRequestException("AwesomeAPI não disponível");

        // Act
        var result = new CepResult("Falha em todos os serviços", exception1);
        result.Exceptions.Add(exception2);
        result.Exceptions.Add(exception3);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(3, result.Exceptions.Count);
        Assert.Contains(result.Exceptions, e => e is InvalidOperationException);
        Assert.Contains(result.Exceptions, e => e is TimeoutException);
        Assert.Contains(result.Exceptions, e => e is HttpRequestException);
    }

    /// <summary>
    /// Testa cenário onde resultado tem sucesso mas também tem mensagem informativa
    /// </summary>
    [Fact(DisplayName = "Deve permitir sucesso com mensagem informativa")]
    public void CepResult_CenarioSucessoComMensagem_DeveTerSucessoEMensagem()
    {
        // Arrange
        var container = new CepContainer("PR", "Curitiba", "Centro", "", "Rua XV", "80000-000", "4106902");
        const string mensagem = "Consulta realizada com sucesso via BrasilAPI";

        // Act
        var result = new CepResult(true, container, mensagem, null);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
        Assert.Equal(mensagem, result.Message);
        Assert.Empty(result.Exceptions);
    }

    /// <summary>
    /// Testa cenário de adição manual de exceção em resultado de sucesso
    /// </summary>
    [Fact(DisplayName = "Deve permitir adicionar exceção manualmente mesmo em resultado de sucesso")]
    public void CepResult_CenarioSucessoComExcecaoManual_DevePermitirAdicao()
    {
        // Arrange
        var exception = new Exception("Test");
        var result = new CepResult(true, null, "TEST");

        // Act
        result.Exceptions.Add(exception);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Exceptions);
        Assert.Single(result.Exceptions);
        Assert.True(result.Success);
        Assert.Equal("TEST", result.Message);
        Assert.Null(result.CepContainer);
    }

    #endregion
}
