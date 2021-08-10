using Sirb.CepBrasil.Extensions;
using Sirb.CepBrasil.Interfaces;
using Sirb.CepBrasil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sirb.CepBrasil.Services
{
	public sealed class CepService : ICepService, IDisposable
	{
		private readonly bool _httpClientSelfCreated;
		private readonly HttpClient _httpClient;

		private readonly List<ICepServiceControl> _services = new List<ICepServiceControl>();

		private CepService(HttpClient httpClient, bool httpClientSelfCreated)
		{
			_httpClientSelfCreated = httpClientSelfCreated;
			_httpClient = httpClient;

			StartServices();
		}

		public CepService() : this(new HttpClient(), true)
		{
		}

		public CepService(HttpClient httpClient) : this(httpClient, false)
		{
		}

		private void StartServices()
		{
			_services.Add(new CorreiosService(_httpClient));
			_services.Add(new ViaCepService(_httpClient));
		}

		public async Task<CepResult> Find(string cep)
		{
			var result = new CepResult();
			foreach (ICepServiceControl service in _services)
			{
				try
				{
					result.CepContainer = await service.Find(cep);
					result.Success = true;

					break;
				}
				catch (Exception e)
				{
					if (result.Exceptions == null)
						result.Exceptions = new List<Exception>();

					result.Exceptions.Add(e);

					string value = result?.Message ?? "";
					result.Message = $"{value}{e?.AllMessages()} ";
				}
			}

			return result;
		}

		public void Dispose()
		{
			if (_httpClientSelfCreated)
				_httpClient?.Dispose();
		}
	}
}