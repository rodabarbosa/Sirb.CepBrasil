﻿namespace Sirb.CepBrasil.Test.Exceptions
{
    public class NotFoundExceptionTest
    {
        private const string FallbackMessage = "Not found";

        [Fact]
        public void Constructor_Valid()
        {
            var exception = new NotFoundException();
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
            var exception = new NotFoundException(message);
            if (!string.IsNullOrEmpty(message))
            {
                exception.Message
                    .Should()
                    .NotBeNullOrEmpty()
                    .And
                    .Be(message);
            }

            exception.InnerException
                .Should()
                .BeNull();
        }

        [Theory]
        [InlineData("Message 1")]
        public void Constructor_InnerException(string message)
        {
            var inner = new Exception(message);
            var exception = new NotFoundException(inner);

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
            var exception = new NotFoundException("Test 1", inner);

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
            var action = () => NotFoundException.ThrowIf(isThrowing, message);
            if (isThrowing)
            {
                action.Should()
                    .Throw<NotFoundException>();
            }
            else
            {
                action.Should()
                    .NotThrow<NotFoundException>();
            }
        }
    }
}
