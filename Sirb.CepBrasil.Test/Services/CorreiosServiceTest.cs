﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sirb.CepBrasil.Test.Services
{
    public class CorreiosServiceTest : IDisposable
    {
        private readonly HttpClient _httpClient = new();

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        [InlineData("81670-010")]
        public async Task Cep_Should_Return_Result(string cep)
        {
            var service = new CorreiosService(_httpClient);

            var result = await service.Find(cep);

            Assert.NotNull(result);
        }
    }
}
