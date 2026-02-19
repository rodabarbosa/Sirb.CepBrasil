namespace Sirb.CepBrasil.Test;

public sealed class ExtensionsTest
{
    [Theory(DisplayName = "Validação de CEP - Sucesso")]
    [InlineData("83040-040")]
    [InlineData("80035-020")]
    // ReSharper disable once HeapView.ClosureAllocation
    public void ValidationTest_Should_Not_Throw(string cep)
    {
        var exception = Record.Exception(() => CepValidation.Validate(cep));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("83040-040000")]
    [InlineData("800350")]
    // ReSharper disable once HeapView.ClosureAllocation
    public void ValidationTest_Show_Throw(string cep)
    {
        Assert.Throws<ServiceException>(() => CepValidation.Validate(cep));
    }

    [Theory]
    [InlineData("83040040", "83040-040")]
    [InlineData("80035020", "80035-020")]
    public void MaskTest(string value, string exceptedResult)
    {
        var maskedValue = value.CepMask();
        Assert.Equal(exceptedResult, maskedValue);
    }
}
