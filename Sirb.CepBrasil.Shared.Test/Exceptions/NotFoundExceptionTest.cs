using System.Runtime.Serialization.Formatters.Binary;

namespace Sirb.CepBrasil.Test.Exceptions
{
    public class NotFoundExceptionTest
    {
        private const string FallbackMessage = "Not found";

        [Fact]
        public void Constructor_Valid()
        {
            var exception = new NotFoundException();
            Assert.Null(exception.InnerException);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.Equal(FallbackMessage, exception.Message);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_Case(string message)
        {
            var exception = new NotFoundException(message);
            if (!string.IsNullOrEmpty(message))
            {
                Assert.Equal(message, exception.Message);
                Assert.NotNull(exception.Message);
            }

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor_InnerException()
        {
            var inner = new Exception("Test");
            var exception = new NotFoundException(inner);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.NotNull(exception.InnerException);
        }

        [Fact]
        public void Constructor_MessageInnerException()
        {
            var inner = new Exception("Test 2");
            var exception = new NotFoundException("Test 1", inner);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.NotNull(exception.InnerException);
        }

        [Theory]
        [InlineData("message", true)]
        [InlineData("message", false)]
        public void ThrowIf_Test(string message, bool isThrowing)
        {
            if (isThrowing)
            {
                Assert.Throws<NotFoundException>(() => NotFoundException.ThrowIf(isThrowing, message));
            }
            else
            {
                NotFoundException.ThrowIf(isThrowing, message);
                Assert.False(isThrowing);
            }
        }

        [Fact]
        public void Serialization_Test()
        {
            // Arrange
            const string expectedMessage = "Serialization test";
            var e = new NotFoundException(expectedMessage);

            // Act
            using (Stream s = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                formatter.Serialize(s, e);
                s.Position = 0; // Reset stream position
                e = (NotFoundException)formatter.Deserialize(s);
#pragma warning restore SYSLIB0011
            }

            // Assert
            Assert.Equal(expectedMessage, e.Message);
        }
    }
}
