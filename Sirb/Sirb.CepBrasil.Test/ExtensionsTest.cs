using Sirb.CepBrasil.Exceptions;
using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Validations;
using Xunit;

namespace Sirb.CepBrasil.Test
{
    public sealed class ExtensionsTest
    {
        [Theory]
        [InlineData("83040-040", false)]
        [InlineData("80035-020", false)]
        [InlineData("83040-040000", true)]
        [InlineData("800350", true)]
        public void ValidationTest(string cep, bool expectException)
        {
            if (expectException)
                Assert.Throws<ServiceException>(() => CepValidation.Validate(cep));
            else
                Assert.False(expectException);
        }

        [Theory]
        [InlineData("83040040", "83040-040")]
        [InlineData("80035020", "80035-020")]
        public void MaskTest(string value, string exceptedResult)
        {
            string maskedValue = value.CepMask();
            Assert.Equal(maskedValue, exceptedResult);
        }
    }
}
