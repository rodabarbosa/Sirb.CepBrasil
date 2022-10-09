using Sirb.CepBrasil.Exceptions;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace Sirb.CepBrasil.Test.Exceptions
{
    public class ServiceExceptionTest
    {
        private const string FallbackMessage = "Ocorreu um erro ao tentar acessar o serviço.";

        [Fact]
        public void Constructor_Valid()
        {
            var exception = new ServiceException();
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
            var exception = new ServiceException(message);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.Null(exception.InnerException);

            var expected = string.IsNullOrEmpty(message?.Trim()) ? FallbackMessage : message;
            Assert.Equal(expected, exception.Message);
        }

        [Fact]
        public void Constructor_InnerException()
        {
            var inner = new Exception("Test");
            var exception = new ServiceException(inner);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.NotNull(exception.InnerException);
        }

        [Fact]
        public void Constructor_MessageInnerException()
        {
            var inner = new Exception("Test 2");
            var exception = new ServiceException("Test 1", inner);
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
                Assert.Throws<ServiceException>(() => ServiceException.ThrowIf(isThrowing, message));
            }
            else
            {
#pragma warning disable CS0618
                ServiceException.When(isThrowing, message);
#pragma warning restore CS0618
                Assert.False(isThrowing);
            }
        }

        [Fact]
        public void Serialization_Test()
        {
            // Arrange
            const string expectedMessage = "Serialization test";
            var e = new ServiceException(expectedMessage);

            // Act
            using (Stream s = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                formatter.Serialize(s, e);
                s.Position = 0; // Reset stream position
                e = (ServiceException)formatter.Deserialize(s);
#pragma warning restore SYSLIB0011
            }

            // Assert
            Assert.Equal(expectedMessage, e.Message);
        }
    }
}
