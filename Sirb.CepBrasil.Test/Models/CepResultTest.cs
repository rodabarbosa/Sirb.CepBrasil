using System;

namespace Sirb.CepBrasil.Test.Models
{
    public class CepResultTest
    {
        [Fact]
        public void Constructor_Test()
        {
            var result = new CepResult();

            Assert.NotNull(result);
            Assert.NotNull(result.Exceptions);
            Assert.Empty(result.Exceptions);
            Assert.False(result.Success);
            Assert.Null(result.Message);
            Assert.Null(result.CepContainer);
        }

        [Fact]
        public void Inline_Test()
        {
            var result = new CepResult(true, default, "TEST");
            result.Exceptions.Add(new Exception("Test"));

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
            var ex = new Exception("Test");
            var result = new CepResult(true, default, "TEST", ex);

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
