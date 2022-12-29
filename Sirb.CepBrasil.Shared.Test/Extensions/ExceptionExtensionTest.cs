namespace Sirb.CepBrasil.Shared.Test.Extensions;

public class ExceptionExtensionTest
{
    [Fact]
    public void NullException_Test()
    {
        Exception exception = null;

        var message = exception.AllMessages();

        Assert.Empty(message);
    }

    [Fact]
    public void NoInnerException_Test()
    {
        var exception = new Exception("TEST");

        var message = exception.AllMessages();

        Assert.NotEmpty(message);
        Assert.Equal("TEST", message);
    }

    [Fact]
    public void WithInnerException_Test()
    {
        var innerException = new Exception("TEST");
        var exception = new Exception("TEST", innerException);

        var message = exception.AllMessages();

        Assert.NotEmpty(message);
        Assert.Equal("TEST TEST", message);
    }
}
