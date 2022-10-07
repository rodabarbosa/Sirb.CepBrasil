using Sirb.CepBrasil.Extensions;
using Xunit;

namespace Sirb.CepBrasil.Test.Extensions
{
    public class CepExtensionTest
    {
        [Theory]
        [InlineData("00000-000", "00000000")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void RemoveMask_Test(string value, string expected)
        {
            var result = value.RemoveMask();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("00000000", "00000-000")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void PlaceMask_Test(string value, string expected)
        {
            var result = value.CepMask();
            Assert.Equal(expected, result);
        }
    }
}