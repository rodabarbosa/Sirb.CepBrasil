namespace Sirb.CepBrasil.Test.Extensions
{
    public class ExceptionExtensionTest
    {
        [Fact]
        public void NullException_Test()
        {
            Exception exception = default;

            // ReSharper disable once ExpressionIsAlwaysNull
            var message = exception.AllMessages();

            Assert.Empty(message);
        }

        [Theory]
        [InlineData("Message Test")]
        public void NoInnerException_Test(string param)
        {
            var exception = new Exception(param);

            var message = exception.AllMessages();

            Assert.NotNull(message);
            Assert.NotEmpty(message);
            Assert.Equal(param, message);
        }

        [Theory]
        [InlineData("Message Test")]
        public void WithInnerException_Test(string param)
        {
            var innerException = new Exception(param);
            var exception = new Exception(param, innerException);

            var message = exception.AllMessages();
            var expected = $"{param} {param}";

            Assert.NotNull(message);
            Assert.NotEmpty(message);
            Assert.Equal(expected, message);
        }
    }
}
