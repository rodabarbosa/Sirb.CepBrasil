namespace Sirb.CepBrasil.Test.Exceptions
{
    public class ServiceExceptionTest
    {
        private const string FallbackMessage = "Ocorreu um erro ao tentar acessar o serviço.";

        [Fact]
        public void Constructor_Valid()
        {
            var exception = new ServiceException();
            exception.InnerException
                .Should()
                .BeNull();

            exception.Message
                .Should()
                .NotBeNullOrEmpty()
                .And
                .Be(FallbackMessage);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_Case(string message)
        {
            var exception = new ServiceException(message);
            var expected = string.IsNullOrEmpty(message?.Trim()) ? FallbackMessage : message;

            exception.Message
                .Should()
                .NotBeNullOrEmpty()
                .And
                .Be(expected);

            exception.InnerException
                .Should()
                .BeNull();
        }

        [Fact]
        public void Constructor_InnerException()
        {
            var inner = new Exception("Test");
            var exception = new ServiceException(inner);

            exception.InnerException
                .Should()
                .NotBeNull()
                .And
                .Be(inner);

            exception.Message
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public void Constructor_MessageInnerException()
        {
            var inner = new Exception("Test 2");
            var exception = new ServiceException("Test 1", inner);

            exception.InnerException
                .Should()
                .NotBeNull()
                .And
                .Be(inner);

            exception.Message
                .Should()
                .NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("message", true)]
        [InlineData("message", false)]
        public void ThrowIf_Test(string message, bool isThrowing)
        {
            var action = () => ServiceException.ThrowIf(isThrowing, message);
            if (isThrowing)
            {
                action.Should()
                    .Throw<ServiceException>();
            }
            else
            {
                action.Should()
                    .NotThrow<ServiceException>();
            }
        }

//         [Fact]
//         public void Serialization_Test()
//         {
//             // Arrange
//             const string expectedMessage = "Serialization test";
//             var e = new ServiceException(expectedMessage);
//
//             // Act
//             using (Stream s = new MemoryStream())
//             {
//                 var formatter = new BinaryFormatter();
// #pragma warning disable SYSLIB0011
//                 formatter.Serialize(s, e);
//                 s.Position = 0; // Reset stream position
//                 e = (ServiceException)formatter.Deserialize(s);
// #pragma warning restore SYSLIB0011
//             }
//
//             // Assert
//             Assert.Equal(expectedMessage, e.Message);
//         }
    }
}
