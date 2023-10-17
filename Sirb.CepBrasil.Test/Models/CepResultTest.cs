using System;

namespace Sirb.CepBrasil.Test.Models
{
    public class CepResultTest
    {
        [Fact]
        public void Constructor_Test()
        {
            CepResult result = new();

            result.Should()
                .NotBeNull();

            result.Exceptions
                .Should()
                .NotBeNull()
                .And
                .BeEmpty();

            result.Success
                .Should()
                .BeFalse();

            result.Message
                .Should()
                .BeNull();

            result.CepContainer
                .Should()
                .BeNull();
        }

        [Fact]
        public void Construct_With_Result()
        {
            CepContainer container = new(default, default, default, default, default, default);
            CepResult result = new(container);

            result.Should()
                .NotBeNull();

            result.CepContainer
                .Should()
                .NotBeNull()
                .And
                .BeSameAs(container);

            result.Success
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("TEST")]
        public void Construct_With_Message(string message)
        {
            CepResult result = new(message);

            result.Should()
                .NotBeNull();

            result.CepContainer
                .Should()
                .BeNull();

            result.Success
                .Should()
                .BeFalse();

            result.Message
                .Should()
                .Be(message);

            result.Exceptions
                .Should()
                .NotBeNull()
                .And
                .BeEmpty();
        }

        [Theory]
        [InlineData("TEST")]
        public void Construct_With_Exception(string message)
        {
            CepResult result = new(message, new Exception(message));

            result.Should()
                .NotBeNull();

            result.CepContainer
                .Should()
                .BeNull();

            result.Success
                .Should()
                .BeFalse();

            result.Message
                .Should()
                .Be(message);

            result.Exceptions
                .Should()
                .NotBeNull()
                .And
                .NotBeEmpty();
        }

        [Fact]
        public void Inline_Test()
        {
            Exception test = new("Test");
            CepResult result = new(true, default, "TEST");
            result.Exceptions.Add(test);

            result.Should()
                .NotBeNull();

            result.Exceptions
                .Should()
                .NotBeEmpty()
                .And
                .ContainSingle();

            result.Success
                .Should()
                .BeTrue();

            result.Message
                .Should()
                .Be("TEST");

            result.CepContainer
                .Should()
                .BeNull();
        }

        [Fact]
        public void InnerException_Test()
        {
            Exception ex = new("Test");
            CepResult result = new(true, default, "TEST", ex);

            result.Should()
                .NotBeNull();

            result.Exceptions
                .Should()
                .NotBeEmpty()
                .And
                .ContainSingle()
                .And
                .Contain(ex);

            result.Success
                .Should()
                .BeTrue();

            result.Message
                .Should()
                .Be("TEST");

            result.CepContainer
                .Should()
                .BeNull();
        }
    }
}
