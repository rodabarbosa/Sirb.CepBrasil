using System.Threading;

namespace Sirb.CepBrasil.Test.Services;

/// <summary>
/// Testes unitários para o serviço ViaCepService
/// </summary>
public sealed class ViaCepServiceTest : IDisposable
{
    private readonly HttpClient _httpClient = new();

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    /// <summary>
    /// Testa se FindAsync retorna sucesso quando CEPs válidos são fornecidos
    /// </summary>
    [Theory(DisplayName = "Deve retornar CepContainer quando CEP é válido e existe no ViaCEP")]
    [InlineData("83040-040")]
    [InlineData("80035-020")]
    [InlineData("81670-010")]
    [InlineData("01310-100")]
    [InlineData("20040020")]
    public async Task FindAsync_QuandoCepValido_DeveRetornarCepContainer(string cep)
    {
        // Arrange
        var service = new ViaCepService(_httpClient);

        // Act
        var result = await service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Cep));
    }

    /// <summary>
    /// Testa se Find (método obsoleto) retorna sucesso quando CEP é válido
    /// </summary>
    [Fact(DisplayName = "Deve retornar CepContainer quando usar método obsoleto Find com CEP válido")]
    public async Task Find_QuandoCepValido_DeveRetornarCepContainer()
    {
        // Arrange
        var service = new ViaCepService(_httpClient);
        const string cep = "83040-040";

        // Act
#pragma warning disable CS0618 // Tipo ou membro obsoleto
        var result = await service.Find(cep);
#pragma warning restore CS0618 // Tipo ou membro obsoleto

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Cep));
    }

    /// <summary>
    /// Testa se FindAsync lança ArgumentNullException quando CEP é nulo ou vazio
    /// </summary>
    [Theory(DisplayName = "Deve lançar ArgumentNullException quando CEP é nulo ou vazio")]
    [InlineData("")]
    [InlineData(null)]
    public async Task FindAsync_QuandoCepNuloOuVazio_DeveLancarArgumentNullException(string cepInvalido)
    {
        // Arrange
        var service = new ViaCepService(_httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.FindAsync(cepInvalido, CancellationToken.None));
    }

    /// <summary>
    /// Testa se FindAsync lança ServiceException quando CEP tem tamanho inválido
    /// </summary>
    [Theory(DisplayName = "Deve lançar ServiceException quando CEP tem tamanho inválido")]
    [InlineData("123")]
    [InlineData("abcdefgh")]
    [InlineData("123456789")]
    public async Task FindAsync_QuandoCepTamanhoInvalido_DeveLancarServiceException(string cepInvalido)
    {
        // Arrange
        var service = new ViaCepService(_httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceException>(() => service.FindAsync(cepInvalido, CancellationToken.None));
    }

    /// <summary>
    /// Testa se FindAsync retorna CepContainer com propriedades nulas quando CEP não é encontrado
    /// </summary>
    [Theory(DisplayName = "Deve retornar CepContainer com propriedades nulas quando CEP não é encontrado no ViaCEP")]
    [InlineData("00000-000")]
    [InlineData("99999-999")]
    public async Task FindAsync_QuandoCepNaoEncontrado_DeveRetornarCepContainerComPropriedadesNulas(string cepNaoEncontrado)
    {
        // Arrange
        var service = new ViaCepService(_httpClient);

        // Act
        var result = await service.FindAsync(cepNaoEncontrado, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Cep);
        Assert.Null(result.Logradouro);
    }

    /// <summary>
    /// Testa se FindAsync lança ArgumentNullException quando HttpClient é nulo no construtor
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando HttpClient do construtor é nulo")]
    public void Construtor_QuandoHttpClientNulo_DeveLancarArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ViaCepService(null));
    }

    /// <summary>
    /// Testa se FindAsync respeita CancellationToken fornecido
    /// </summary>
    [Fact(DisplayName = "Deve respeitar CancellationToken quando fornecido")]
    public async Task FindAsync_QuandoCancellationTokenCancelado_DeveLancarTaskCanceledException()
    {
        // Arrange
        var service = new ViaCepService(_httpClient);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - TaskCanceledException herda de OperationCanceledException
        await Assert.ThrowsAsync<TaskCanceledException>(() => service.FindAsync("83040-040", cts.Token));
    }

    /// <summary>
    /// Testa se FindAsync usa timeout padrão quando CancellationToken não é fornecido
    /// </summary>
    [Fact(DisplayName = "Deve usar timeout padrão de 30 segundos quando CancellationToken não é fornecido")]
    public async Task FindAsync_QuandoCancellationTokenDefault_DeveUsarTimeoutPadrao()
    {
        // Arrange
        var service = new ViaCepService(_httpClient);
        const string cep = "83040-040";

        // Act
        var result = await service.FindAsync(cep, default);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// Testa se FindAsync funciona com CEP formatado e sem formatação
    /// </summary>
    [Theory(DisplayName = "Deve aceitar CEP com ou sem formatação")]
    [InlineData("83040040", "83040-040")]
    [InlineData("83040-040", "83040-040")]
    public async Task FindAsync_QuandoCepComOuSemFormatacao_DeveRetornarMesmoCep(string cepEntrada, string cepEsperado)
    {
        // Arrange
        var service = new ViaCepService(_httpClient);

        // Act
        var result = await service.FindAsync(cepEntrada, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cepEsperado, result.Cep);
    }
}
