namespace Sirb.CepBrasil.Test.Models
{
    public class CepResultTest
    {
        [Fact]
        public void Constructor_Test()
        {
            CepResult result = new();

            Assert.NotNull(result);
            Assert.NotNull(result.Exceptions);
            Assert.Empty(result.Exceptions);
            Assert.False(result.Success);
            Assert.Null(result.Message);
            Assert.Null(result.CepContainer);
        }

        [Fact]
        public void Construct_With_Result()
        {
            CepContainer container = new(default, default, default, default, default, default);
            CepResult result = new(container);

            Assert.NotNull(result);
            Assert.NotNull(result.CepContainer);
            Assert.Same(container, result.CepContainer);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("TEST")]
        public void Construct_With_Message(string message)
        {
            CepResult result = new(message);

            Assert.NotNull(result);
            Assert.Null(result.CepContainer);
            Assert.False(result.Success);
            Assert.Equal(message, result.Message);
            Assert.NotNull(result.Exceptions);
            Assert.Empty(result.Exceptions);
        }

        [Theory]
        [InlineData("TEST")]
        public void Construct_With_Exception(string message)
        {
            CepResult result = new(message, new Exception(message));

            Assert.NotNull(result);
            Assert.Null(result.CepContainer);
            Assert.False(result.Success);
            Assert.Equal(message, result.Message);
            Assert.NotNull(result.Exceptions);
            Assert.NotEmpty(result.Exceptions);
        }

        [Fact]
        public void Inline_Test()
        {
            Exception test = new("Test");
            CepResult result = new(true, default, "TEST");
            result.Exceptions.Add(test);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Exceptions);
            Assert.Single(result.Exceptions);
            Assert.True(result.Success);
            Assert.Equal("TEST", result.Message);
            Assert.Null(result.CepContainer);
        }

        [Fact]
        public void InnerException_Test()
        {
            Exception ex = new("Test");
            CepResult result = new(true, default, "TEST", ex);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Exceptions);
            Assert.Single(result.Exceptions);
            Assert.Contains(ex, result.Exceptions);
            Assert.True(result.Success);
            Assert.Equal("TEST", result.Message);
            Assert.Null(result.CepContainer);
        }
    }
}
