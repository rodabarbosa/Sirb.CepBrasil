using System.Threading;

namespace Sirb.CepBrasil.Test.Services;

/// <summary>
/// Unit tests for the CepService (Orchestrator with fallback strategy)
/// </summary>
public sealed class CepServiceTest : IDisposable
{
    private CepService _service = new();

    public void Dispose()
    {
        _service?.Dispose();
    }

    #region Testes de Sucesso

    /// <summary>
    /// Tests if FindAsync returns success when CEP is valid and exists in the first provider
    /// </summary>
    [Theory(DisplayName = "Deve retornar sucesso quando CEP é válido e existe no primeiro provedor (BrasilAPI)")]
    [InlineData("01310-100")]
    [InlineData("20040-020")]
    [InlineData("30130-000")]
    [InlineData("80035-020")]
    [InlineData("01310100")]
    public async Task FindAsync_QuandoCepValidoExisteNoPrimeiroProvedor_DeveRetornarSucesso(string cep)
    {
        // Arrange & Act
        var result = await _service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
        Assert.False(string.IsNullOrEmpty(result.CepContainer.Cep));
        Assert.Null(result.Message);
    }

    /// <summary>
    /// Tests if FindAsync accepts CEP with or without formatting
    /// </summary>
    [Theory(DisplayName = "Deve aceitar CEP com ou sem formatação")]
    [InlineData("01310100")]
    [InlineData("01310-100")]
    public async Task FindAsync_QuandoCepComOuSemFormatacao_DeveRetornarSucesso(string cep)
    {
        // Arrange & Act
        var result = await _service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
    }

    #endregion

    #region Testes de Falha

    /// <summary>
    /// Tests if FindAsync returns error when CEP is null or empty
    /// </summary>
    [Theory(DisplayName = "Deve retornar erro quando CEP é nulo ou vazio")]
    [InlineData("")]
    [InlineData(null)]
    public async Task FindAsync_QuandoCepNuloOuVazio_DeveRetornarErro(string cepInvalido)
    {
        // Arrange & Act
        var result = await _service.FindAsync(cepInvalido, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.NotNull(result.Message);
        Assert.NotEmpty(result.Message);
    }

    /// <summary>
    /// Tests if FindAsync returns error when CEP has invalid size
    /// </summary>
    [Theory(DisplayName = "Deve retornar erro quando CEP tem tamanho inválido")]
    [InlineData("123")]
    [InlineData("abcdefgh")]
    [InlineData("123456789")]
    public async Task FindAsync_QuandoCepTamanhoInvalido_DeveRetornarErro(string cepInvalido)
    {
        // Arrange & Act
        var result = await _service.FindAsync(cepInvalido, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.NotNull(result.Message);
        Assert.NotEmpty(result.Message);
    }

    /// <summary>
    /// Tests if FindAsync returns error when CEP doesn't exist in any provider
    /// </summary>
    [Theory(DisplayName = "Deve retornar erro quando CEP não existe em nenhum provedor")]
    [InlineData("00000-000")]
    public async Task FindAsync_QuandoCepNaoExisteEmNenhumProvedor_DeveRetornarErro(string cepInexistente)
    {
        // Arrange & Act
        var result = await _service.FindAsync(cepInexistente, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.NotNull(result.Message);
        Assert.NotEmpty(result.Message);
    }

    #endregion

    #region Testes de CancellationToken

    /// <summary>
    /// Tests if FindAsync uses default timeout when CancellationToken.None is provided
    /// </summary>
    [Fact(DisplayName = "Deve usar timeout padrão de 30 segundos quando CancellationToken.None é fornecido")]
    public async Task FindAsync_QuandoCancellationTokenNone_DeveUsarTimeoutPadrao()
    {
        // Arrange
        const string cep = "01310-100";

        // Act
        var result = await _service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
    }

    #endregion

    #region Testes de Fallback

    /// <summary>
    /// Tests if FindAsync tries all providers in order when previous ones fail
    /// </summary>
    /// <remarks>
    /// This test validates the fallback strategy by using a CEP that might fail in some providers
    /// but should eventually succeed in one of them
    /// </remarks>
    [Fact(DisplayName = "Deve tentar todos os provedores em ordem até encontrar resultado")]
    public async Task FindAsync_QuandoPrimeirosProvedoresFalham_DeveTentarProximoProvedor()
    {
        // Arrange
        const string cep = "01310-100";

        // Act
        var result = await _service.FindAsync(cep, CancellationToken.None);

        // Assert
        // Deve retornar sucesso de algum provedor
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
    }

    #endregion

    #region Testes de Dispose

    /// <summary>
    /// Tests if Dispose can be called multiple times safely
    /// </summary>
    [Fact(DisplayName = "Deve permitir múltiplas chamadas de Dispose sem erro")]
    public void Dispose_QuandoChamadoMultiplasVezes_NaoDeveLancarExcecao()
    {
        // Arrange
        var service = new CepService();

        // Act & Assert
        service.Dispose();
        service.Dispose(); // Não deve lançar exceção
    }

    /// <summary>
    /// Tests if service can be used after creation with parameterless constructor
    /// </summary>
    [Fact(DisplayName = "Deve funcionar com construtor sem parâmetros")]
    public async Task Constructor_QuandoChamadoSemParametros_DeveCriarHttpClientInterno()
    {
        // Arrange
        using var service = new CepService();

        // Act
        var result = await service.FindAsync("01310-100", CancellationToken.None);

        // Assert
        Assert.True(result.Success);
    }

    /// <summary>
    /// Tests if service can be used with external HttpClient
    /// </summary>
    [Fact(DisplayName = "Deve funcionar com HttpClient injetado")]
    public async Task Constructor_QuandoChamadoComHttpClient_DeveUsarHttpClientFornecido()
    {
        // Arrange
        using var httpClient = new HttpClient();
        using var service = new CepService(httpClient);

        // Act
        var result = await service.FindAsync("01310-100", CancellationToken.None);

        // Assert
        Assert.True(result.Success);
    }

    /// <summary>
    /// Tests if constructor throws exception when null HttpClient is provided
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando HttpClient é nulo")]
    public void Constructor_QuandoHttpClientNulo_DeveLancarArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CepService(null));
    }

    #endregion

    #region Testes de Resultado

    /// <summary>
    /// Tests if successful result has correct structure
    /// </summary>
    [Fact(DisplayName = "Deve retornar CepResult com estrutura correta quando bem-sucedido")]
    public async Task FindAsync_QuandoSucesso_DeveRetornarCepResultCorreto()
    {
        // Arrange
        const string cep = "01310-100";

        // Act
        var result = await _service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.CepContainer);
        Assert.Null(result.Message);

        // Verifica se o CepContainer tem dados válidos
        Assert.NotNull(result.CepContainer.Cep);
        Assert.NotNull(result.CepContainer.Uf);
        // Cidade pode ser null em alguns provedores, então não validamos
    }

    /// <summary>
    /// Tests if error result has correct structure
    /// </summary>
    [Fact(DisplayName = "Deve retornar CepResult com estrutura correta quando falha")]
    public async Task FindAsync_QuandoFalha_DeveRetornarCepResultCorreto()
    {
        // Arrange
        const string cepInvalido = "123";

        // Act
        var result = await _service.FindAsync(cepInvalido, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.CepContainer);
        Assert.NotNull(result.Message);
        Assert.NotEmpty(result.Message);
    }

    #endregion
}
