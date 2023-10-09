using Sirb.CepBrasil.Validations;

namespace Sirb.CepBrasil.Shared.Test
{
    public sealed class ExtensionsTest
    {
        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        public void ValidationTest_Should_Not_Throw(string cep)
        {
            var action = () => CepValidation.Validate(cep);

            action.Should()
                .NotThrow<ServiceException>();
        }

        [Theory]
        [InlineData("83040-040000")]
        [InlineData("800350")]
        public void ValidationTest_Show_Throw(string cep)
        {
            var action = () => CepValidation.Validate(cep);

            action.Should()
                .Throw<ServiceException>();
        }

        [Theory]
        [InlineData("83040040", "83040-040")]
        [InlineData("80035020", "80035-020")]
        public void MaskTest(string value, string exceptedResult)
        {
            var maskedValue = value.CepMask();

            maskedValue.Should()
                .Be(exceptedResult);
        }
    }
}
