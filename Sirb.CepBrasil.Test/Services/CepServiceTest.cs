﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sirb.CepBrasil.Test.Services
{
    public sealed class CepServiceTest : IDisposable
    {
        private readonly HttpClient httpClient = new();

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        [InlineData("81670-010")]
        public async Task GetCepSuccess_NoHttp(string cep)
        {
            using var service = new CepService();
            var result = await service.Find(cep);

            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("83040-040")]
        [InlineData("80035-020")]
        [InlineData("81670-010")]
        public async Task GetCepSuccess_WithHttp(string cep)
        {
            using var service = new CepService(httpClient);
            var result = await service.Find(cep);

            Assert.True(result.Success);
        }
    }
}
