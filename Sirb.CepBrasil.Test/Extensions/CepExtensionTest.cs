namespace Sirb.CepBrasil.Test.Extensions;

/// <summary>
/// Testes unitários para a classe <see cref="CepExtension"/>.
/// Valida todas as operações de manipulação e validação de CEP.
/// </summary>
public class CepExtensionTest
{
    #region RemoveMask Tests

    /// <summary>
    /// Testa se RemoveMask remove corretamente a máscara de um CEP formatado.
    /// </summary>
    [Fact(DisplayName = "Deve remover a máscara de um CEP formatado")]
    public void RemoveMask_WhenCepIsFormatted_ShouldRemoveMask()
    {
        // Arrange
        var cepFormatado = "01310-100";

        // Act
        var result = cepFormatado.RemoveMask();

        // Assert
        Assert.Equal("01310100", result);
    }

    /// <summary>
    /// Testa se RemoveMask retorna CEP sem alterações quando já está sem máscara.
    /// </summary>
    [Fact(DisplayName = "Deve retornar CEP inalterado quando já está sem máscara")]
    public void RemoveMask_WhenCepHasNoMask_ShouldReturnUnchanged()
    {
        // Arrange
        var cepSemMascara = "01310100";

        // Act
        var result = cepSemMascara.RemoveMask();

        // Assert
        Assert.Equal("01310100", result);
    }

    /// <summary>
    /// Testa se RemoveMask retorna string vazia quando a entrada é vazia.
    /// </summary>
    [Fact(DisplayName = "Deve retornar string vazia quando entrada está vazia")]
    public void RemoveMask_WhenInputIsEmpty_ShouldReturnEmpty()
    {
        // Arrange
        var cepVazio = "";

        // Act
        var result = cepVazio.RemoveMask();

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Testa se RemoveMask retorna null quando a entrada é null.
    /// </summary>
    [Fact(DisplayName = "Deve retornar null quando entrada é null")]
    public void RemoveMask_WhenInputIsNull_ShouldReturnNull()
    {
        // Arrange
        string cepNulo = null;

        // Act
        var result = cepNulo.RemoveMask();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa se RemoveMask retorna espaços quando entrada é apenas espaços em branco.
    /// </summary>
    [Fact(DisplayName = "Deve retornar espaços quando entrada é apenas espaços")]
    public void RemoveMask_WhenInputIsOnlyWhitespace_ShouldReturnWhitespace()
    {
        // Arrange - RemoveMask apenas remove caracteres não-dígitos, espaços são mantidos
        var cepComEspacos = "   ";

        // Act
        var result = cepComEspacos.RemoveMask();

        // Assert
        Assert.Equal("   ", result);
    }

    /// <summary>
    /// Testa se RemoveMask remove múltiplos caracteres não numéricos.
    /// </summary>
    [Fact(DisplayName = "Deve remover múltiplos caracteres não numéricos")]
    public void RemoveMask_WhenCepHasMultipleNonDigitCharacters_ShouldRemoveAll()
    {
        // Arrange
        var cepComMultiplosCaracteres = "01-310.100";

        // Act
        var result = cepComMultiplosCaracteres.RemoveMask();

        // Assert
        Assert.Equal("01310100", result);
    }

    #endregion

    #region CepMask Tests

    /// <summary>
    /// Testa se CepMask aplica corretamente a máscara em um CEP sem formatação.
    /// </summary>
    [Fact(DisplayName = "Deve aplicar máscara em CEP sem formatação")]
    public void CepMask_WhenCepHasNoMask_ShouldApplyMask()
    {
        // Arrange
        var cepSemMascara = "01310100";

        // Act
        var result = cepSemMascara.CepMask();

        // Assert
        Assert.Equal("01310-100", result);
    }

    /// <summary>
    /// Testa se CepMask retorna CEP inalterado quando já está formatado.
    /// </summary>
    [Fact(DisplayName = "Deve retornar CEP inalterado quando já está formatado")]
    public void CepMask_WhenCepIsAlreadyFormatted_ShouldReturnUnchanged()
    {
        // Arrange
        var cepFormatado = "01310-100";

        // Act
        var result = cepFormatado.CepMask();

        // Assert
        Assert.Equal("01310-100", result);
    }

    /// <summary>
    /// Testa se CepMask retorna string vazia quando entrada é vazia.
    /// </summary>
    [Fact(DisplayName = "Deve retornar string vazia quando entrada está vazia")]
    public void CepMask_WhenInputIsEmpty_ShouldReturnEmpty()
    {
        // Arrange
        var cepVazio = "";

        // Act
        var result = cepVazio.CepMask();

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Testa se CepMask retorna null quando entrada é null.
    /// </summary>
    [Fact(DisplayName = "Deve retornar null quando entrada é null")]
    public void CepMask_WhenInputIsNull_ShouldReturnNull()
    {
        // Arrange
        string cepNulo = null;

        // Act
        var result = cepNulo.CepMask();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa se CepMask retorna o valor original quando CEP tem comprimento inválido.
    /// </summary>
    [Theory(DisplayName = "Deve retornar valor original quando CEP tem comprimento inválido")]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    public void CepMask_WhenCepHasInvalidLength_ShouldReturnOriginal(string cepInvalido)
    {
        // Arrange & Act
        var result = cepInvalido.CepMask();

        // Assert
        Assert.Equal(cepInvalido, result);
    }

    /// <summary>
    /// Testa se CepMask retorna espaços quando entrada é apenas espaços.
    /// </summary>
    [Fact(DisplayName = "Deve retornar espaços quando entrada é apenas espaços")]
    public void CepMask_WhenInputIsOnlyWhitespace_ShouldReturnWhitespace()
    {
        // Arrange - CepMask verifica IsNullOrWhiteSpace, então retorna a string original
        var cepComEspacos = "   ";

        // Act
        var result = cepComEspacos.CepMask();

        // Assert
        Assert.Equal("   ", result);
    }

    /// <summary>
    /// Testa se CepMask aplica máscara corretamente mesmo com caracteres não numéricos.
    /// </summary>
    [Fact(DisplayName = "Deve aplicar máscara mesmo com caracteres não numéricos")]
    public void CepMask_WhenCepHasNonDigitCharacters_ShouldApplyMask()
    {
        // Arrange
        var cepComCaracteres = "01-310.100";

        // Act
        var result = cepComCaracteres.CepMask();

        // Assert
        Assert.Equal("01310-100", result);
    }

    #endregion

    #region IsValidCep Tests

    /// <summary>
    /// Testa se IsValidCep retorna verdadeiro para CEP formatado válido.
    /// </summary>
    [Fact(DisplayName = "Deve validar CEP formatado corretamente")]
    public void IsValidCep_WhenCepIsFormattedCorrectly_ShouldReturnTrue()
    {
        // Arrange
        var cepValido = "01310-100";

        // Act
        var result = cepValido.IsValidCep();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Testa se IsValidCep retorna verdadeiro para CEP sem máscara válido.
    /// </summary>
    [Fact(DisplayName = "Deve validar CEP sem máscara corretamente")]
    public void IsValidCep_WhenCepIsWithoutMaskCorrectly_ShouldReturnTrue()
    {
        // Arrange
        var cepValido = "01310100";

        // Act
        var result = cepValido.IsValidCep();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Testa se IsValidCep retorna falso para CEP com formato inválido.
    /// </summary>
    [Theory(DisplayName = "Deve rejeitar CEP com formato inválido")]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("abcde-fgh")]
    [InlineData("abcdefgh")]
    public void IsValidCep_WhenCepHasInvalidFormat_ShouldReturnFalse(string cepInvalido)
    {
        // Arrange & Act
        var result = cepInvalido.IsValidCep();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Testa se IsValidCep retorna falso para null.
    /// </summary>
    [Fact(DisplayName = "Deve rejeitar CEP nulo")]
    public void IsValidCep_WhenCepIsNull_ShouldReturnFalse()
    {
        // Arrange
        string cepNulo = null;

        // Act
        var result = cepNulo.IsValidCep();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Testa se IsValidCep retorna falso para string vazia.
    /// </summary>
    [Fact(DisplayName = "Deve rejeitar CEP vazio")]
    public void IsValidCep_WhenCepIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var cepVazio = "";

        // Act
        var result = cepVazio.IsValidCep();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Testa se IsValidCep retorna falso para string com apenas espaços.
    /// </summary>
    [Fact(DisplayName = "Deve rejeitar CEP com apenas espaços")]
    public void IsValidCep_WhenCepIsOnlyWhitespace_ShouldReturnFalse()
    {
        // Arrange
        var cepComEspacos = "   ";

        // Act
        var result = cepComEspacos.IsValidCep();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Testa se IsValidCep retorna verdadeiro para vários CEPs válidos.
    /// </summary>
    [Theory(DisplayName = "Deve validar diversos CEPs válidos")]
    [InlineData("01310-100")]
    [InlineData("01310100")]
    [InlineData("20040020")]
    [InlineData("20040-020")]
    [InlineData("70040902")]
    [InlineData("70040-902")]
    public void IsValidCep_WhenCepIsValid_ShouldReturnTrue(string cepValido)
    {
        // Arrange & Act
        var result = cepValido.IsValidCep();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Testa se IsValidCep é sensível a espaços em branco no início/fim.
    /// </summary>
    [Fact(DisplayName = "Deve validar CEP com espaços em branco no início e fim")]
    public void IsValidCep_WhenCepHasWhitespaceAround_ShouldReturnTrue()
    {
        // Arrange
        var cepComEspacos = "  01310-100  ";

        // Act
        var result = cepComEspacos.IsValidCep();

        // Assert
        Assert.True(result);
    }

    #endregion

    #region GetDigitsOnly Tests

    /// <summary>
    /// Testa se GetDigitsOnly remove corretamente a máscara.
    /// </summary>
    [Fact(DisplayName = "Deve retornar apenas os dígitos do CEP")]
    public void GetDigitsOnly_WhenCepIsFormatted_ShouldReturnOnlyDigits()
    {
        // Arrange
        var cepFormatado = "01310-100";

        // Act
        var result = cepFormatado.GetDigitsOnly();

        // Assert
        Assert.Equal("01310100", result);
    }

    /// <summary>
    /// Testa se GetDigitsOnly retorna CEP inalterado quando já tem apenas dígitos.
    /// </summary>
    [Fact(DisplayName = "Deve retornar CEP inalterado quando já tem apenas dígitos")]
    public void GetDigitsOnly_WhenCepHasOnlyDigits_ShouldReturnUnchanged()
    {
        // Arrange
        var cepSemMascara = "01310100";

        // Act
        var result = cepSemMascara.GetDigitsOnly();

        // Assert
        Assert.Equal("01310100", result);
    }

    /// <summary>
    /// Testa se GetDigitsOnly é um alias para RemoveMask.
    /// </summary>
    [Fact(DisplayName = "Deve ser equivalente a RemoveMask")]
    public void GetDigitsOnly_ShouldBeEquivalentToRemoveMask()
    {
        // Arrange
        var cep = "01310-100";

        // Act
        var resultGetDigits = cep.GetDigitsOnly();
        var resultRemoveMask = cep.RemoveMask();

        // Assert
        Assert.Equal(resultRemoveMask, resultGetDigits);
    }

    #endregion

    #region Format Tests

    /// <summary>
    /// Testa se Format aplica máscara em CEP válido sem máscara.
    /// </summary>
    [Fact(DisplayName = "Deve formatar CEP válido sem máscara")]
    public void Format_WhenCepIsValidWithoutMask_ShouldApplyMask()
    {
        // Arrange
        var cepSemMascara = "01310100";

        // Act
        var result = cepSemMascara.Format();

        // Assert
        Assert.Equal("01310-100", result);
    }

    /// <summary>
    /// Testa se Format retorna string vazia para CEP inválido.
    /// </summary>
    [Fact(DisplayName = "Deve retornar string vazia para CEP inválido")]
    public void Format_WhenCepIsInvalid_ShouldReturnEmpty()
    {
        // Arrange
        var cepInvalido = "123";

        // Act
        var result = cepInvalido.Format();

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Testa se Format formata corretamente CEP já formatado.
    /// </summary>
    [Fact(DisplayName = "Deve manter formato de CEP já formatado")]
    public void Format_WhenCepIsAlreadyFormatted_ShouldReturnFormatted()
    {
        // Arrange
        var cepFormatado = "01310-100";

        // Act
        var result = cepFormatado.Format();

        // Assert
        Assert.Equal("01310-100", result);
    }

    /// <summary>
    /// Testa se Format retorna string vazia para CEP nulo.
    /// </summary>
    [Fact(DisplayName = "Deve retornar string vazia para CEP nulo")]
    public void Format_WhenCepIsNull_ShouldReturnEmpty()
    {
        // Arrange
        string cepNulo = null;

        // Act
        var result = cepNulo.Format();

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Testa se Format retorna string vazia para CEP vazio.
    /// </summary>
    [Fact(DisplayName = "Deve retornar string vazia para CEP vazio")]
    public void Format_WhenCepIsEmpty_ShouldReturnEmpty()
    {
        // Arrange
        var cepVazio = "";

        // Act
        var result = cepVazio.Format();

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Testa se Format retorna string vazia para múltiplos CEPs inválidos.
    /// </summary>
    [Theory(DisplayName = "Deve retornar string vazia para múltiplos CEPs inválidos")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("abcdefgh")]
    public void Format_WhenCepIsInvalidMultiple_ShouldReturnEmpty(string cepInvalido)
    {
        // Arrange & Act
        var result = cepInvalido.Format();

        // Assert
        Assert.Equal("", result);
    }

    #endregion

    #region Normalize Tests

    /// <summary>
    /// Testa se Normalize remove máscara de CEP válido.
    /// </summary>
    [Fact(DisplayName = "Deve normalizar CEP válido formatado")]
    public void Normalize_WhenCepIsValidFormatted_ShouldReturnWithoutMask()
    {
        // Arrange
        var cepFormatado = "01310-100";

        // Act
        var result = CepExtension.Normalize(cepFormatado);

        // Assert
        Assert.Equal("01310100", result);
    }

    /// <summary>
    /// Testa se Normalize retorna CEP inalterado quando já está sem máscara.
    /// </summary>
    [Fact(DisplayName = "Deve retornar CEP inalterado quando já está normalizado")]
    public void Normalize_WhenCepIsAlreadyNormalized_ShouldReturnUnchanged()
    {
        // Arrange
        var cepSemMascara = "01310100";

        // Act
        var result = CepExtension.Normalize(cepSemMascara);

        // Assert
        Assert.Equal("01310100", result);
    }

    /// <summary>
    /// Testa se Normalize retorna null para CEP inválido.
    /// </summary>
    [Fact(DisplayName = "Deve retornar null para CEP inválido")]
    public void Normalize_WhenCepIsInvalid_ShouldReturnNull()
    {
        // Arrange
        var cepInvalido = "123";

        // Act
        var result = CepExtension.Normalize(cepInvalido);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa se Normalize retorna null para CEP nulo.
    /// </summary>
    [Fact(DisplayName = "Deve retornar null para CEP nulo")]
    public void Normalize_WhenCepIsNull_ShouldReturnNull()
    {
        // Arrange
        string cepNulo = null;

        // Act
        var result = CepExtension.Normalize(cepNulo);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa se Normalize retorna null para CEP vazio.
    /// </summary>
    [Fact(DisplayName = "Deve retornar null para CEP vazio")]
    public void Normalize_WhenCepIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var cepVazio = "";

        // Act
        var result = CepExtension.Normalize(cepVazio);

        // Assert
        // IsValidCep retorna false para strings vazias, então Normalize retorna null
        Assert.Null(result);
    }

    /// <summary>
    /// Testa se Normalize retorna null para CEP com apenas espaços.
    /// </summary>
    [Fact(DisplayName = "Deve retornar null para CEP com apenas espaços")]
    public void Normalize_WhenCepIsOnlyWhitespace_ShouldReturnNull()
    {
        // Arrange
        var cepComEspacos = "   ";

        // Act
        var result = CepExtension.Normalize(cepComEspacos);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa se Normalize normaliza múltiplos CEPs válidos.
    /// </summary>
    [Theory(DisplayName = "Deve normalizar diversos CEPs válidos")]
    [InlineData("01310-100", "01310100")]
    [InlineData("20040-020", "20040020")]
    [InlineData("70040-902", "70040902")]
    public void Normalize_WhenCepIsValid_ShouldNormalize(string cepFormatado, string esperado)
    {
        // Arrange & Act
        var result = CepExtension.Normalize(cepFormatado);

        // Assert
        Assert.Equal(esperado, result);
    }

    #endregion
}
