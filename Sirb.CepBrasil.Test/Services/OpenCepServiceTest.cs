using System.Threading;

namespace Sirb.CepBrasil.Test.Services;

/// <summary>
/// Unit tests for the OpenCepService
/// </summary>
public sealed class OpenCepServiceTest : IDisposable
{
    private readonly HttpClient _httpClient = new();

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    /// <summary>
    /// Tests if FindAsync returns success when valid CEPs are provided
    /// </summary>
    [Theory(DisplayName = "Deve retornar CepContainer quando CEP é válido e existe no OpenCEP")]
    [InlineData("83040-040")]
    [InlineData("80035-020")]
    [InlineData("81670-010")]
    [InlineData("01310-100")]
    [InlineData("20040020")]
    public async Task FindAsync_QuandoCepValido_DeveRetornarCepContainer(string cep)
    {
        // Arrange
        var service = new OpenCepService(_httpClient);

        // Act
        var result = await service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Cep));
    }

    /// <summary>
    /// Tests if FindAsync throws ArgumentNullException when CEP is null or empty
    /// </summary>
    [Theory(DisplayName = "Deve lançar ArgumentNullException quando CEP é nulo ou vazio")]
    [InlineData("")]
    [InlineData(null)]
    public async Task FindAsync_QuandoCepNuloOuVazio_DeveLancarArgumentNullException(string cepInvalido)
    {
        // Arrange
        var service = new OpenCepService(_httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.FindAsync(cepInvalido, CancellationToken.None));
    }

    /// <summary>
    /// Tests if FindAsync throws ServiceException when CEP has invalid size
    /// </summary>
    [Theory(DisplayName = "Deve lançar ServiceException quando CEP tem tamanho inválido")]
    [InlineData("123")]
    [InlineData("abcdefgh")]
    [InlineData("123456789")]
    public async Task FindAsync_QuandoCepTamanhoInvalido_DeveLancarServiceException(string cepInvalido)
    {
        // Arrange
        var service = new OpenCepService(_httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceException>(() => service.FindAsync(cepInvalido, CancellationToken.None));
    }

    /// <summary>
    /// Tests if FindAsync respects provided CancellationToken
    /// </summary>
    [Fact(DisplayName = "Deve respeitar CancellationToken quando fornecido")]
    public async Task FindAsync_QuandoCancellationTokenCancelado_DeveLancarTaskCanceledException()
    {
        // Arrange
        var service = new OpenCepService(_httpClient);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => service.FindAsync("83040-040", cts.Token));
    }

    /// <summary>
    /// Tests if FindAsync uses default timeout when CancellationToken is not provided
    /// </summary>
    [Fact(DisplayName = "Deve usar timeout padrão de 30 segundos quando CancellationToken não é fornecido")]
    public async Task FindAsync_QuandoCancellationTokenDefault_DeveUsarTimeoutPadrao()
    {
        // Arrange
        var service = new OpenCepService(_httpClient);
        const string cep = "83040-040";

        // Act
        var result = await service.FindAsync(cep, default);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// Tests if FindAsync works with formatted and unformatted CEP
    /// </summary>
    [Theory(DisplayName = "Deve aceitar CEP com ou sem formatação")]
    [InlineData("83040040", "83040-040")]
    [InlineData("83040-040", "83040-040")]
    public async Task FindAsync_QuandoCepComOuSemFormatacao_DeveRetornarMesmoCep(string cepEntrada, string cepEsperado)
    {
        // Arrange
        var service = new OpenCepService(_httpClient);

        // Act
        var result = await service.FindAsync(cepEntrada, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cepEsperado, result.Cep);
    }
}
