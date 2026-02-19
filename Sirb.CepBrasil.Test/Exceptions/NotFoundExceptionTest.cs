namespace Sirb.CepBrasil.Test.Exceptions;

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
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.Equal(message, exception.Message);
        }

        Assert.Null(exception.InnerException);
    }

    [Theory]
    [InlineData("Message 1")]
    public void Constructor_InnerException(string message)
    {
        var inner = new Exception(message);
        var exception = new NotFoundException(inner);

        Assert.NotNull(exception.InnerException);
        Assert.Same(inner, exception.InnerException);
        Assert.NotNull(exception.Message);
        Assert.NotEmpty(exception.Message);
    }

    [Fact]
    public void Constructor_MessageInnerException()
    {
        var inner = new Exception("Test 2");
        var exception = new NotFoundException("Test 1", inner);

        Assert.NotNull(exception.InnerException);
        Assert.Same(inner, exception.InnerException);
        Assert.NotNull(exception.Message);
        Assert.NotEmpty(exception.Message);
    }

    [Theory]
    [InlineData("message", true)]
    [InlineData("message", false)]
    public void ThrowIf_Test(string message, bool isThrowing)
    {
        if (isThrowing)
            Assert.Throws<NotFoundException>(() => NotFoundException.ThrowIf(isThrowing, message));
        else
        {
            var exception = Record.Exception(() => NotFoundException.ThrowIf(isThrowing, message));
            Assert.Null(exception);
        }
    }
}
