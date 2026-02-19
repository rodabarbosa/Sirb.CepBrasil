namespace Sirb.CepBrasil.Test.Extensions;

/// <summary>
/// Testes unitários para a classe ExceptionExtension.
/// Validam todos os métodos AllMessages() e GetDetailedMessage() com 100% de cobertura.
/// </summary>
public class ExceptionExtensionTest
{
    #region AllMessages - Testes com Diferentes Tipos de Exceção

    /// <summary>
    /// Verifica comportamento com diferentes tipos de exceção.
    /// </summary>
    [Theory(DisplayName = "Deve lidar com diferentes tipos de exceção")]
    [InlineData(typeof(InvalidOperationException), "Operação inválida")]
    [InlineData(typeof(NotImplementedException), "Não implementado")]
    [InlineData(typeof(TimeoutException), "Timeout da operação")]
    public void AllMessages_ComDiferentesTiposExcecao_DeveRetornarMensagem(Type exceptionType, string mensagem)
    {
        // Arrange
        var exception = (Exception)Activator.CreateInstance(exceptionType, mensagem);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Contains(mensagem, result);
    }

    #endregion

    #region AllMessages - Testes Básicos

    /// <summary>
    /// Verifica se AllMessages retorna a mensagem quando há apenas uma exceção.
    /// </summary>
    [Theory(DisplayName = "Deve retornar a mensagem original quando exceção não tem exceções internas")]
    [InlineData("Mensagem de erro")]
    [InlineData("CEP inválido")]
    [InlineData("Serviço indisponível")]
    public void AllMessages_QuandoExcecaoSemInternas_DeveRetornarMensagem(string mensagem)
    {
        // Arrange
        var exception = new InvalidOperationException(mensagem);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Equal(mensagem, result);
    }

    /// <summary>
    /// Verifica se AllMessages concatena corretamente com separador " → ".
    /// </summary>
    [Fact(DisplayName = "Deve concatenar mensagens com separador quando tem exceções internas")]
    public void AllMessages_QuandoTemExcecoesInternas_DeveRetornarTodas()
    {
        // Arrange
        var innerException = new InvalidOperationException("Erro interno");
        var outerException = new ArgumentNullException("paramName", new Exception("Erro principal", innerException));

        // Act
        var result = outerException.AllMessages();

        // Assert
        Assert.Contains("Erro principal", result);
        Assert.Contains("Erro interno", result);
        Assert.Contains(" → ", result);
    }

    /// <summary>
    /// Verifica se AllMessages concatena múltiplas exceções internas em ordem.
    /// </summary>
    [Fact(DisplayName = "Deve concatenar múltiplas exceções internas mantendo a ordem")]
    public void AllMessages_QuandoTemMultiplasExcecoesInternas_DeveRetornarTodas()
    {
        // Arrange
        var innerInner = new NotImplementedException("Erro 3");
        var inner = new InvalidOperationException("Erro 2", innerInner);
        var exception = new InvalidOperationException("Erro 1", inner);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Equal("Erro 1 → Erro 2 → Erro 3", result);
    }

    /// <summary>
    /// Verifica se AllMessages lança ArgumentNullException quando exception é nula.
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando exception é nula")]
    public void AllMessages_QuandoExceptionNula_DeveLancarArgumentNullException()
    {
        // Arrange
        Exception exc = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => exc.AllMessages());
        Assert.Contains("exception", ex.Message);
    }

    /// <summary>
    /// Verifica se AllMessages filtra mensagens vazias ou contendo apenas whitespace.
    /// </summary>
    [Theory(DisplayName = "Deve filtrar mensagens vazias ou com apenas espaço em branco")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void AllMessages_QuandoTemMensagensVazias_DeveFiltralas(string mensagemVazia)
    {
        // Arrange
        var innerEmpty = new InvalidOperationException(mensagemVazia);
        var outerException = new InvalidOperationException("Mensagem válida", innerEmpty);

        // Act
        var result = outerException.AllMessages();

        // Assert
        Assert.Equal("Mensagem válida", result);
    }

    /// <summary>
    /// Verifica se AllMessages retorna string vazia quando todas as mensagens são vazias.
    /// </summary>
    [Fact(DisplayName = "Deve retornar string vazia quando todas as mensagens são vazias")]
    public void AllMessages_QuandoTodasMensagensVazias_DeveRetornarStringVazia()
    {
        // Arrange
        var innerEmpty = new InvalidOperationException(string.Empty);
        var outerException = new InvalidOperationException("   ", innerEmpty);

        // Act
        var result = outerException.AllMessages();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region AllMessages - Testes de Edge Cases

    /// <summary>
    /// Verifica comportamento com mensagens muito longas.
    /// </summary>
    [Fact(DisplayName = "Deve lidar com mensagens muito longas")]
    public void AllMessages_ComMensagensLongas_DeveRetornarCompleta()
    {
        // Arrange
        var longMessage = new string('x', 10000);
        var exception = new InvalidOperationException(longMessage);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Equal(longMessage, result);
    }

    /// <summary>
    /// Verifica comportamento com caracteres especiais em mensagens.
    /// </summary>
    [Theory(DisplayName = "Deve lidar com caracteres especiais em mensagens")]
    [InlineData("Erro com @#$%^&*()")]
    [InlineData("Erro com []{};':\\|,./<>?")]
    [InlineData("Erro com \"aspas\" e 'apóstrofos'")]
    public void AllMessages_ComCaracteresEspeciais_DeveRetornarCorreto(string mensagem)
    {
        // Arrange
        var exception = new InvalidOperationException(mensagem);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Equal(mensagem, result);
    }

    /// <summary>
    /// Verifica comportamento com exceções contendo caracteres unicode.
    /// </summary>
    [Theory(DisplayName = "Deve lidar com caracteres unicode nas mensagens")]
    [InlineData("Erro com acentuação: áéíóú")]
    [InlineData("Erro com cedilha: ç")]
    [InlineData("Erro com tilde: ñ")]
    public void AllMessages_ComUnicode_DeveRetornarCorreto(string mensagem)
    {
        // Arrange
        var exception = new InvalidOperationException(mensagem);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Equal(mensagem, result);
    }

    /// <summary>
    /// Verifica comportamento com newlines nas mensagens.
    /// </summary>
    [Fact(DisplayName = "Deve lidar com quebras de linha nas mensagens")]
    public void AllMessages_ComNewlines_DevePreservarFormatacao()
    {
        // Arrange
        var mensagemComNewline = "Linha 1\nLinha 2\nLinha 3";
        var exception = new InvalidOperationException(mensagemComNewline);

        // Act
        var result = exception.AllMessages();

        // Assert
        Assert.Equal(mensagemComNewline, result);
    }

    #endregion

    #region GetDetailedMessage - Testes Básicos

    /// <summary>
    /// Verifica se GetDetailedMessage inclui tipo e mensagem de exceção.
    /// </summary>
    [Fact(DisplayName = "Deve incluir tipo da exceção e mensagem")]
    public void GetDetailedMessage_DeveIncluirTipoDaExcecao()
    {
        // Arrange
        var exception = new ArgumentNullException("paramName", "Teste");

        // Act
        var result = exception.GetDetailedMessage();

        // Assert
        Assert.Contains("[ArgumentNullException]", result);
        Assert.Contains("Teste", result);
    }

    /// <summary>
    /// Verifica se GetDetailedMessage formata múltiplas exceções corretamente com quebras de linha.
    /// </summary>
    [Fact(DisplayName = "Deve formatar múltiplas exceções com quebras de linha")]
    public void GetDetailedMessage_QuandoTemMultiplasExcecoes_DeveFormatarComQuebras()
    {
        // Arrange
        var inner = new InvalidOperationException("Erro interno");
        var exception = new InvalidOperationException("Erro principal", inner);

        // Act
        var result = exception.GetDetailedMessage();

        // Assert
        Assert.Contains("[InvalidOperationException]", result);
        Assert.Contains("Erro principal", result);
        Assert.Contains("Erro interno", result);
    }

    /// <summary>
    /// Verifica se GetDetailedMessage não inclui StackTrace quando parâmetro é false.
    /// </summary>
    [Fact(DisplayName = "Deve não incluir StackTrace quando parâmetro é false")]
    public void GetDetailedMessage_ComIncludeStackTraceFalse_NaoDeveIncluirStackTrace()
    {
        // Arrange
        var exception = new InvalidOperationException("Teste");

        // Act
        var result = exception.GetDetailedMessage();

        // Assert
        Assert.DoesNotContain("StackTrace:", result);
    }

    /// <summary>
    /// Verifica se GetDetailedMessage lança ArgumentNullException quando exception é nula.
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando exception é nula")]
    public void GetDetailedMessage_QuandoExceptionNula_DeveLancarArgumentNullException()
    {
        // Arrange
        Exception exc = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => exc.GetDetailedMessage());
        Assert.Contains("exception", ex.Message);
    }

    /// <summary>
    /// Verifica se GetDetailedMessage retorna mensagem válida com StackTrace quando exceção foi lançada.
    /// </summary>
    [Fact(DisplayName = "Deve incluir StackTrace quando exceção foi lançada e parâmetro é true")]
    public void GetDetailedMessage_QuandoExceptionFoiLancadaComStackTrace_DeveRetornarComStackTrace()
    {
        // Arrange
        Exception exception = null;
        try
        {
            throw new DivideByZeroException("Divisão por zero");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Act
        var result = exception.GetDetailedMessage(true);

        // Assert
        Assert.Contains("[DivideByZeroException]", result);
        Assert.Contains("Divisão por zero", result);
        Assert.Contains("StackTrace:", result);
    }

    #endregion

    #region Testes de Integração com Cenários Reais

    /// <summary>
    /// Verifica cenário completo de exceção real em aplicação de busca de CEP.
    /// </summary>
    [Fact(DisplayName = "Deve lidar com exceção real de cenário de busca de CEP com fallback")]
    public void CenarioReal_BuscaCepFallback_DeveRetornarMensagensCompletas()
    {
        // Arrange
        Exception serviceException = null;
        try
        {
            try
            {
                throw new HttpRequestException("BrasilAPI indisponível");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Erro ao buscar CEP em BrasilAPI", ex);
            }
        }
        catch (Exception ex)
        {
            serviceException = ex;
        }

        // Act
        var allMessages = serviceException.AllMessages();
        var detailed = serviceException.GetDetailedMessage();

        // Assert
        Assert.Contains("Erro ao buscar CEP em BrasilAPI", allMessages);
        Assert.Contains("BrasilAPI indisponível", allMessages);
        Assert.Contains("[ServiceException]", detailed);
        Assert.Contains("[HttpRequestException]", detailed);
    }

    /// <summary>
    /// Verifica comportamento com cadeia profunda de exceções (mais de 3 níveis).
    /// </summary>
    [Fact(DisplayName = "Deve lidar com cadeia profunda de exceções")]
    public void AllMessages_ComCadeiaProfundaExcecoes_DeveRetornarTodas()
    {
        // Arrange - Cria cadeia com 5 níveis
        var ex = new Exception("Nível 5");
        ex = new Exception("Nível 4", ex);
        ex = new Exception("Nível 3", ex);
        ex = new Exception("Nível 2", ex);
        ex = new Exception("Nível 1", ex);

        // Act
        var result = ex.AllMessages();

        // Assert
        var expected = "Nível 1 → Nível 2 → Nível 3 → Nível 4 → Nível 5";
        Assert.Equal(expected, result);
    }

    #endregion
}
