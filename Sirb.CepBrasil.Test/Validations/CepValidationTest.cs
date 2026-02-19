namespace Sirb.CepBrasil.Test.Validations;

/// <summary>
/// Testes unitários para a classe <see cref="CepValidation"/>.
/// Valida a lógica de validação de CEP brasileiro.
/// </summary>
public class CepValidationTest
{
    #region Valid CEP Tests

    /// <summary>
    /// Testa se Validate aceita CEP formatado válido.
    /// </summary>
    [Fact(DisplayName = "Deve aceitar CEP formatado válido")]
    public void Validate_WhenCepIsValidFormatted_ShouldNotThrow()
    {
        // Arrange
        var validCep = "01310-100";

        // Act & Assert
        var exception = Record.Exception(() => CepValidation.Validate(validCep));
        Assert.Null(exception);
    }

    /// <summary>
    /// Testa se Validate aceita CEP sem formatação válido.
    /// </summary>
    [Fact(DisplayName = "Deve aceitar CEP sem formatação válido")]
    public void Validate_WhenCepIsValidWithoutMask_ShouldNotThrow()
    {
        // Arrange
        var validCep = "01310100";

        // Act & Assert
        var exception = Record.Exception(() => CepValidation.Validate(validCep));
        Assert.Null(exception);
    }

    /// <summary>
    /// Testa se Validate aceita múltiplos CEPs válidos.
    /// </summary>
    [Theory(DisplayName = "Deve aceitar diversos CEPs válidos")]
    [InlineData("01310-100")]
    [InlineData("01310100")]
    [InlineData("20040-020")]
    [InlineData("20040020")]
    [InlineData("70040-902")]
    [InlineData("70040902")]
    [InlineData("88015-100")]
    [InlineData("88015100")]
    public void Validate_WhenCepIsValid_ShouldNotThrow(string validCep)
    {
        // Act & Assert
        var exception = Record.Exception(() => CepValidation.Validate(validCep));
        Assert.Null(exception);
    }

    #endregion

    #region Invalid CEP Tests

    /// <summary>
    /// Testa se Validate lança ArgumentNullException quando CEP é null.
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando CEP é null")]
    public void Validate_WhenCepIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string nullCep = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CepValidation.Validate(nullCep));
    }

    /// <summary>
    /// Testa se Validate lança ArgumentNullException quando CEP é vazio.
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando CEP é vazio")]
    public void Validate_WhenCepIsEmpty_ShouldThrowArgumentNullException()
    {
        // Arrange
        var emptyCep = "";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CepValidation.Validate(emptyCep));
    }

    /// <summary>
    /// Testa se Validate lança ArgumentNullException quando CEP contém apenas espaços.
    /// </summary>
    [Fact(DisplayName = "Deve lançar ArgumentNullException quando CEP é apenas espaços")]
    public void Validate_WhenCepIsOnlyWhitespace_ShouldThrowArgumentNullException()
    {
        // Arrange
        var whitespaceCep = "   ";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CepValidation.Validate(whitespaceCep));
    }

    /// <summary>
    /// Testa se Validate lança ServiceException quando CEP tem comprimento inválido.
    /// </summary>
    [Theory(DisplayName = "Deve lançar ServiceException quando CEP tem comprimento inválido")]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("12345678901")]
    public void Validate_WhenCepHasInvalidLength_ShouldThrowServiceException(string invalidCep)
    {
        // Act & Assert
        Assert.Throws<ServiceException>(() => CepValidation.Validate(invalidCep));
    }

    /// <summary>
    /// Testa se Validate lança ServiceException quando CEP contém caracteres não-numéricos.
    /// </summary>
    [Theory(DisplayName = "Deve lançar ServiceException quando CEP contém caracteres não-numéricos")]
    [InlineData("0131a100")]
    [InlineData("01310-abc")]
    [InlineData("abc-def")]
    [InlineData("01310@100")]
    public void Validate_WhenCepContainsNonNumericCharacters_ShouldThrowServiceException(string invalidCep)
    {
        // Act & Assert
        Assert.Throws<ServiceException>(() => CepValidation.Validate(invalidCep));
    }

    /// <summary>
    /// Testa se Validate lança ServiceException quando CEP tem formato inválido.
    /// </summary>
    [Theory(DisplayName = "Deve lançar ServiceException quando CEP tem formato inválido")]
    [InlineData("01310--100")]
    [InlineData("013101-00")]
    [InlineData("-01310100")]
    [InlineData("01310100-")]
    public void Validate_WhenCepHasInvalidFormat_ShouldThrowServiceException(string invalidCep)
    {
        // Act & Assert
        Assert.Throws<ServiceException>(() => CepValidation.Validate(invalidCep));
    }

    #endregion

    #region Edge Cases

    /// <summary>
    /// Testa se Validate aceita CEP com espaços ao redor quando têm 8 dígitos.
    /// </summary>
    [Fact(DisplayName = "Deve aceitar CEP com espaços ao redor")]
    public void Validate_WhenCepHasLeadingAndTrailingWhitespace_ShouldAccept()
    {
        // Arrange
        var cepWithSpaces = "  01310100  ";

        // Act & Assert
        var exception = Record.Exception(() => CepValidation.Validate(cepWithSpaces));
        Assert.Null(exception);
    }

    /// <summary>
    /// Testa se Validate aceita CEP com múltiplos hífens após remoção de máscara.
    /// </summary>
    [Fact(DisplayName = "Deve aceitar CEP com múltiplos hífens após normalização")]
    public void Validate_WhenCepHasMultipleHyphens_ShouldValidateByNumericDigits()
    {
        // Arrange - Após RemoveMask(), todos os hífens são removidos
        var cepWithMultipleHyphens = "01-3-10-100";

        // Act & Assert
        var exception = Record.Exception(() => CepValidation.Validate(cepWithMultipleHyphens));
        Assert.Null(exception);
    }

    /// <summary>
    /// Testa mensagem de erro ao validar CEP inválido.
    /// </summary>
    [Fact(DisplayName = "Deve conter mensagem de erro apropriada ao validar CEP inválido")]
    public void Validate_WhenCepIsInvalid_ShouldThrowWithAppropriateMessage()
    {
        // Arrange
        var invalidCep = "123";

        // Act & Assert
        var exception = Assert.Throws<ServiceException>(() => CepValidation.Validate(invalidCep));
        Assert.NotNull(exception);
        Assert.NotEmpty(exception.Message);
    }

    /// <summary>
    /// Testa mensagem de erro ao validar CEP null.
    /// </summary>
    [Fact(DisplayName = "Deve conter mensagem de erro apropriada para CEP null")]
    public void Validate_WhenCepIsNull_ShouldThrowWithAppropriateMessage()
    {
        // Arrange
        string nullCep = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => CepValidation.Validate(nullCep));
        Assert.NotNull(exception.Message);
        Assert.Contains("CEP não pode ser nulo", exception.Message);
    }

    #endregion
}
