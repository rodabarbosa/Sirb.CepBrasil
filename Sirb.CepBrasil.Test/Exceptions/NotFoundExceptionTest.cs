namespace Sirb.CepBrasil.Test.Exceptions;

/// <summary>
/// Testes unitários para a exceção NotFoundException
/// </summary>
public sealed class NotFoundExceptionTest
{
    /// <summary>
    /// Testa se o construtor sem parâmetros usa a mensagem padrão
    /// </summary>
    [Fact(DisplayName = "Deve usar mensagem padrão quando construtor sem parâmetros é chamado")]
    public void Construtor_SemParametros_DeveUsarMensagemPadrao()
    {
        // Arrange & Act
        var exception = new NotFoundException();

        // Assert
        Assert.Equal("Not found", exception.Message);
        Assert.Null(exception.InnerException);
    }

    /// <summary>
    /// Testa se o construtor com mensagem personalizada funciona corretamente
    /// </summary>
    [Fact(DisplayName = "Deve usar mensagem personalizada quando fornecida")]
    public void Construtor_ComMensagem_DeveUsarMensagemFornecida()
    {
        // Arrange
        const string mensagemEsperada = "CEP não encontrado";

        // Act
        var exception = new NotFoundException(mensagemEsperada);

        // Assert
        Assert.Equal(mensagemEsperada, exception.Message);
        Assert.Null(exception.InnerException);
    }

    /// <summary>
    /// Testa se o construtor usa mensagem padrão quando mensagem fornecida é nula ou vazia
    /// </summary>
    [Theory(DisplayName = "Deve usar mensagem padrão quando mensagem fornecida é nula ou vazia")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_ComMensagemNulaOuVazia_DeveUsarMensagemPadrao(string mensagem)
    {
        // Arrange & Act
        var exception = new NotFoundException(mensagem);

        // Assert
        Assert.Equal("Not found", exception.Message);
    }

    /// <summary>
    /// Testa se o construtor com exceção interna armazena corretamente a exceção
    /// </summary>
    [Fact(DisplayName = "Deve armazenar exceção interna quando fornecida")]
    public void Construtor_ComExcecaoInterna_DeveArmazenarExcecao()
    {
        // Arrange
        var innerException = new InvalidOperationException("Erro interno");

        // Act
        var exception = new NotFoundException(innerException);

        // Assert
        Assert.Equal("Not found", exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Same(innerException, exception.InnerException);
        Assert.Equal("Erro interno", exception.InnerException.Message);
    }

    /// <summary>
    /// Testa se o construtor com mensagem e exceção interna funciona corretamente
    /// </summary>
    [Fact(DisplayName = "Deve usar mensagem e exceção interna quando ambas são fornecidas")]
    public void Construtor_ComMensagemEExcecao_DeveUsarAmbos()
    {
        // Arrange
        const string mensagem = "Recurso não encontrado";
        var innerException = new InvalidOperationException("Erro de operação");

        // Act
        var exception = new NotFoundException(mensagem, innerException);

        // Assert
        Assert.Equal(mensagem, exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Same(innerException, exception.InnerException);
    }

    /// <summary>
    /// Testa se ThrowIf lança exceção quando condição é verdadeira
    /// </summary>
    [Fact(DisplayName = "Deve lançar NotFoundException quando ThrowIf recebe condição verdadeira")]
    public void ThrowIf_QuandoCondicaoVerdadeira_DeveLancarNotFoundException()
    {
        // Arrange
        const bool condicao = true;
        const string mensagem = "Item não existe";

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() =>
            NotFoundException.ThrowIf(condicao, mensagem));

        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    /// <summary>
    /// Testa se ThrowIf não lança exceção quando condição é falsa
    /// </summary>
    [Fact(DisplayName = "Não deve lançar exceção quando ThrowIf recebe condição falsa")]
    public void ThrowIf_QuandoCondicaoFalsa_NaoDeveLancarExcecao()
    {
        // Arrange
        const bool condicao = false;
        const string mensagem = "Esta mensagem não deve ser usada";

        // Act & Assert - Não deve lançar exceção
        var exception = Record.Exception(() =>
            NotFoundException.ThrowIf(condicao, mensagem));

        Assert.Null(exception);
    }

    /// <summary>
    /// Testa se ThrowIf lança exceção com mensagem e exceção interna
    /// </summary>
    [Fact(DisplayName = "Deve lançar NotFoundException com mensagem e exceção interna quando fornecidas")]
    public void ThrowIf_ComMensagemEExcecaoInterna_DeveLancarComAmbos()
    {
        // Arrange
        const bool condicao = true;
        const string mensagem = "Registro não localizado";
        var innerException = new ArgumentException("Argumento inválido");

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() =>
            NotFoundException.ThrowIf(condicao, mensagem, innerException));

        Assert.Equal(mensagem, exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Same(innerException, exception.InnerException);
    }

    /// <summary>
    /// Testa se ThrowIf usa mensagem padrão quando mensagem é nula
    /// </summary>
    [Fact(DisplayName = "Deve usar mensagem padrão quando ThrowIf recebe mensagem nula")]
    public void ThrowIf_ComMensagemNula_DeveUsarMensagemPadrao()
    {
        // Arrange
        const bool condicao = true;
        string mensagem = null;

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() =>
            NotFoundException.ThrowIf(condicao, mensagem));

        Assert.Equal("Not found", exception.Message);
    }

    /// <summary>
    /// Testa se ThrowIf funciona com exceção interna nula
    /// </summary>
    [Fact(DisplayName = "Deve funcionar quando ThrowIf recebe exceção interna nula")]
    public void ThrowIf_ComExcecaoInternaNula_DeveFuncionar()
    {
        // Arrange
        const bool condicao = true;
        const string mensagem = "Erro ocorreu";

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() =>
            NotFoundException.ThrowIf(condicao, mensagem));

        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    /// <summary>
    /// Testa se a exceção é serializável
    /// </summary>
    [Fact(DisplayName = "Deve ser marcada como serializável")]
    public void Classe_DeveSerSerializavel()
    {
        // Arrange & Act
        var tipo = typeof(NotFoundException);

        // Assert
#pragma warning disable SYSLIB0050 // Type.IsSerializable is obsolete
        Assert.True(tipo.IsSerializable, "NotFoundException deve ser serializável");
#pragma warning restore SYSLIB0050
    }

    /// <summary>
    /// Testa se a exceção herda de Exception
    /// </summary>
    [Fact(DisplayName = "Deve herdar de Exception")]
    public void Classe_DeveHerdarDeException()
    {
        // Arrange
        var exception = new NotFoundException();

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    /// <summary>
    /// Testa se ThrowIf funciona em cenários reais com validações
    /// </summary>
    [Theory(DisplayName = "Deve validar corretamente cenários reais com ThrowIf")]
    [InlineData(true)]
    [InlineData(false)]
    public void ThrowIf_CenariosReais_DeveValidarCorretamente(bool condicao)
    {
        // Arrange
        const string mensagem = "Validação falhou";

        // Act & Assert
        if (condicao)
        {
            var exception = Assert.Throws<NotFoundException>(() =>
                NotFoundException.ThrowIf(condicao, mensagem));
            Assert.Equal(mensagem, exception.Message);
        }
        else
        {
            var exception = Record.Exception(() =>
                NotFoundException.ThrowIf(condicao, mensagem));
            Assert.Null(exception);
        }
    }

    /// <summary>
    /// Testa se a mensagem é preservada através da cadeia de construtores
    /// </summary>
    [Fact(DisplayName = "Deve preservar mensagem através da cadeia de construtores")]
    public void Construtores_DevePreservarMensagemNaCadeia()
    {
        // Arrange
        const string mensagem = "Mensagem de teste";
        var innerException = new Exception("Inner");

        // Act
        var ex1 = new NotFoundException(mensagem);
        var ex2 = new NotFoundException(mensagem, null);
        var ex3 = new NotFoundException(mensagem, innerException);

        // Assert
        Assert.Equal(mensagem, ex1.Message);
        Assert.Equal(mensagem, ex2.Message);
        Assert.Equal(mensagem, ex3.Message);
    }
}
