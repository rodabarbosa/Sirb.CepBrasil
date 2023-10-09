namespace Sirb.CepBrasil.Shared.Test.Extensions
{
    public class ExceptionExtensionTest
    {
        [Fact]
        public void NullException_Test()
        {
            Exception exception = default;

            // ReSharper disable once ExpressionIsAlwaysNull
            var message = exception.AllMessages();

            message.Should()
                .BeEmpty();
        }

        [Theory]
        [InlineData("Message Test")]
        public void NoInnerException_Test(string param)
        {
            var exception = new Exception(param);

            var message = exception.AllMessages();

            message.Should()
                .NotBeNullOrEmpty()
                .And
                .Be(param);
        }

        [Theory]
        [InlineData("Message Test")]
        public void WithInnerException_Test(string param)
        {
            var innerException = new Exception(param);
            var exception = new Exception(param, innerException);

            var message = exception.AllMessages();
            var expected = $"{param} {param}";

            message.Should()
                .NotBeNullOrEmpty()
                .And
                .Be(expected);
        }
    }
}
