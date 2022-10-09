using Sirb.CepBrasil.Models;
using System;
using System.Linq;
using Xunit;

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
            var result = new CepResult
            {
                Success = true,
                CepContainer = null,
                Message = "TEST"
            };

            result.Exceptions.Add(new Exception("Test"));

            Assert.NotNull(result);
            Assert.NotEmpty(result.Exceptions);
            Assert.IsType<Exception>(result.Exceptions.First());
            Assert.True(result.Success);
            Assert.Equal("TEST", result.Message);
            Assert.Null(result.CepContainer);
        }

        [Fact]
        public void InnerException_Test()
        {
            var ex = new Exception("Test");

            var result = new CepResult
            {
                Success = true,
                CepContainer = null,
                Message = "TEST"
            };
            result.Exceptions.Add(ex);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Exceptions);
            Assert.IsType<Exception>(result.Exceptions.First());
            Assert.True(result.Success);
            Assert.Equal("TEST", result.Message);
            Assert.Null(result.CepContainer);
        }
    }
}
