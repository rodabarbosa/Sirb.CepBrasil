# Instruções de Implementação - Fluxo de Fallback v1.4.0

## 🎯 Objetivo

Implementar estratégia robusta de busca de CEP com fallback automático entre 4 serviços públicos, garantindo resiliência e alta disponibilidade.

---

## 📐 Arquitetura de Implementação

### 1. Interface Base (ICepServiceControl)

Todos os 4 serviços devem implementar essa interface:

```csharp
namespace Sirb.CepBrasil.Interfaces;

/// <summary>
/// Interface para serviços de busca de CEP.
/// Define contrato que todos os provedores devem seguir.
/// </summary>
public interface ICepServiceControl
{
    /// <summary>
    /// Busca endereço pelo CEP de forma assíncrona.
    /// </summary>
    /// <param name="cep">CEP a ser consultado (formato: 00000000 ou 00000-000)</param>
    /// <param name="cancellationToken">Token para cancelamento da operação</param>
    /// <returns>CepContainer com dados do endereço ou null se não encontrado</returns>
    /// <exception cref="HttpRequestException">Quando há erro na requisição HTTP</exception>
    Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken);
}
```

### 2. Orquestrador (CepServiceOrchestrator)

Classe que gerencia o fallback entre serviços:

```csharp
namespace Sirb.CepBrasil.Services;

/// <summary>
/// Orquestra a busca de CEP com estratégia de fallback automático.
/// Tenta múltiplos serviços em ordem pré-definida.
/// </summary>
public sealed class CepServiceOrchestrator : ICepService
{
    private readonly ICepServiceControl[] _services;
    private readonly ILogger<CepServiceOrchestrator> _logger;

    /// <summary>
    /// Inicializa orquestrador com lista de serviços em ordem de prioridade.
    /// </summary>
    /// <param name="services">Array de serviços em ordem: BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP</param>
    /// <param name="logger">Logger para rastrear tentativas</param>
    public CepServiceOrchestrator(ICepServiceControl[] services, ILogger<CepServiceOrchestrator> logger)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Busca endereço com fallback automático entre serviços.
    /// </summary>
    /// <remarks>
    /// Fluxo:
    /// 1. Tenta BrasilAPI
    /// 2. Se falhar/não encontrar, tenta ViaCEP
    /// 3. Se falhar/não encontrar, tenta AwesomeAPI
    /// 4. Se falhar/não encontrar, tenta OpenCEP
    /// 5. Se todos falharem, lança ServiceException
    /// 6. Se nenhum encontrar, retorna null
    /// </remarks>
    public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(cep))
        {
            throw new ArgumentNullException(nameof(cep), "CEP não pode ser vazio");
        }

        // Limpar CEP de formatação
        var cleanCep = CepExtension.RemoveFormatting(cep);

        if (!CepValidation.IsValid(cleanCep))
        {
            throw new ArgumentException($"CEP '{cep}' está em formato inválido", nameof(cep));
        }

        var exceptions = new List<Exception>();

        // Tentar cada serviço em ordem
        foreach (var service in _services)
        {
            try
            {
                _logger.LogInformation($"Tentando buscar CEP {cep} no serviço {service.GetType().Name}");
                var result = await service.FindAsync(cleanCep, cancellationToken);

                if (result != null)
                {
                    _logger.LogInformation($"CEP {cep} encontrado em {service.GetType().Name}");
                    return new CepResult(true, result, null);
                }

                _logger.LogInformation($"CEP {cep} não encontrado em {service.GetType().Name}, tentando próximo...");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao buscar em {service.GetType().Name}: {ex.Message}");
                exceptions.Add(ex);
            }
        }

        // Se chegou aqui, todos falharam ou nenhum encontrou
        if (exceptions.Count == _services.Length)
        {
            // Todos os serviços tiveram erro
            var message = $"Todos os serviços de CEP estão indisponíveis no momento. Erros: {string.Join("; ", exceptions.Select(e => e.Message))}";
            _logger.LogError(message);
            throw new ServiceException(message);
        }

        // Nenhum serviço encontrou o CEP
        _logger.LogInformation($"CEP {cep} não encontrado em nenhum serviço");
        return null;
    }
}
```

### 3. Registrar Serviços no DI Container

```csharp
// Program.cs ou IServiceCollection extension
services.AddHttpClient<BrasilApiService>()
    .ConfigureHttpClient(client => 
    {
        client.BaseAddress = new Uri("https://brasilapi.com.br/api/address/");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

services.AddHttpClient<ViaCepService>()
    .ConfigureHttpClient(client => 
    {
        client.BaseAddress = new Uri("https://viacep.com.br/ws/");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

services.AddHttpClient<AwesomeApiService>()
    .ConfigureHttpClient(client => 
    {
        client.BaseAddress = new Uri("https://awesomeapi.com.br/address/");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

services.AddHttpClient<OpenCepService>()
    .ConfigureHttpClient(client => 
    {
        client.BaseAddress = new Uri("https://cep.dev/");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

// Registrar serviços como ICepServiceControl
services.AddScoped<ICepServiceControl>(sp => sp.GetRequiredService<BrasilApiService>());
services.AddScoped<ICepServiceControl>(sp => sp.GetRequiredService<ViaCepService>());
services.AddScoped<ICepServiceControl>(sp => sp.GetRequiredService<AwesomeApiService>());
services.AddScoped<ICepServiceControl>(sp => sp.GetRequiredService<OpenCepService>());

// Registrar orquestrador
services.AddScoped<ICepService>(sp =>
{
    var brasilApi = sp.GetRequiredService<BrasilApiService>();
    var viaCep = sp.GetRequiredService<ViaCepService>();
    var awesomeApi = sp.GetRequiredService<AwesomeApiService>();
    var openCep = sp.GetRequiredService<OpenCepService>();
    var logger = sp.GetRequiredService<ILogger<CepServiceOrchestrator>>();

    var services = new ICepServiceControl[] { brasilApi, viaCep, awesomeApi, openCep };
    return new CepServiceOrchestrator(services, logger);
});
```

---

## 🔧 Implementação de Cada Serviço

### BrasilApiService

```csharp
namespace Sirb.CepBrasil.Services;

/// <summary>
/// Serviço de busca de CEP via BrasilAPI.
/// Primeira opção de fallback.
/// </summary>
public sealed class BrasilApiService : ICepServiceControl
{
    private readonly HttpClient _httpClient;

    public BrasilApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Busca endereço no BrasilAPI pelo CEP.
    /// </summary>
    public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        // Implementação...
        // URL: GET https://brasilapi.com.br/api/address/v2/{cep}
    }
}
```

### AwesomeApiService

```csharp
namespace Sirb.CepBrasil.Services;

/// <summary>
/// Serviço de busca de CEP via AwesomeAPI.
/// Terceira opção de fallback.
/// </summary>
public sealed class AwesomeApiService : ICepServiceControl
{
    private readonly HttpClient _httpClient;

    public AwesomeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Busca endereço no AwesomeAPI pelo CEP.
    /// </summary>
    public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        // Implementação...
        // URL: GET https://awesomeapi.com.br/api/cep/{cep}
    }
}
```

### OpenCepService

```csharp
namespace Sirb.CepBrasil.Services;

/// <summary>
/// Serviço de busca de CEP via OpenCEP.
/// Quarta e última opção de fallback.
/// </summary>
public sealed class OpenCepService : ICepServiceControl
{
    private readonly HttpClient _httpClient;

    public OpenCepService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Busca endereço no OpenCEP pelo CEP.
    /// </summary>
    public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        // Implementação...
        // URL: GET https://cep.dev/{cep}
    }
}
```

---

## 🧪 Estratégia de Testes

### Para Cada Serviço (BrasilApiService, AwesomeApiService, etc.)

```csharp
namespace Sirb.CepBrasil.Test.Services;

public class BrasilApiServiceTest
{
    [Fact(DisplayName = "Deve retornar sucesso quando CEP é válido e existe")]
    public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new BrasilApiService(httpClient);
        var cep = "01310100";

        // Act
        var result = await service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("01310-100", result.Cep);
        Assert.NotNull(result.Logradouro);
    }

    [Fact(DisplayName = "Deve retornar null quando CEP não é encontrado")]
    public async Task FindAsync_QuandoCepNaoEncontrado_DeveRetornarNull()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new BrasilApiService(httpClient);
        var cep = "00000000";

        // Act
        var result = await service.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Deve lançar exceção quando serviço falha")]
    public async Task FindAsync_QuandoServicoFalha_DeveLancarExcecao()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new BrasilApiService(httpClient);
        var cep = "01310100";

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            service.FindAsync(cep, CancellationToken.None));
    }

    [Fact(DisplayName = "Deve cancelar operação quando CancellationToken é acionado")]
    public async Task FindAsync_QuandoCanceladoComToken_DeveLancarOperationCanceledException()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new BrasilApiService(httpClient);
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            service.FindAsync("01310100", cts.Token));
    }

    [Fact(DisplayName = "Deve validar CEP antes de buscar")]
    public async Task FindAsync_QuandoCepInvalido_DeveLancarArgumentException()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new BrasilApiService(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.FindAsync("123", CancellationToken.None));
    }
}
```

### Para CepServiceOrchestrator

```csharp
namespace Sirb.CepBrasil.Test.Services;

public class CepServiceOrchestratorTest
{
    [Fact(DisplayName = "Deve tentar BrasilAPI primeiro")]
    public async Task FindAsync_DeveUsarBrasilApiPrimeiro()
    {
        // Arrange
        var mockBrasilApi = new Mock<ICepServiceControl>();
        var mockViaCep = new Mock<ICepServiceControl>();
        var mockAwesomeApi = new Mock<ICepServiceControl>();
        var mockOpenCep = new Mock<ICepServiceControl>();
        var mockLogger = new Mock<ILogger<CepServiceOrchestrator>>();

        var result = new CepContainer { Cep = "01310-100", Logradouro = "Teste" };
        mockBrasilApi.Setup(s => s.FindAsync("01310100", It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var orchestrator = new CepServiceOrchestrator(
            new[] { mockBrasilApi.Object, mockViaCep.Object, mockAwesomeApi.Object, mockOpenCep.Object },
            mockLogger.Object);

        // Act
        var response = await orchestrator.FindAsync("01310100", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockBrasilApi.Verify(s => s.FindAsync("01310100", It.IsAny<CancellationToken>()), Times.Once);
        mockViaCep.Verify(s => s.FindAsync("01310100", It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve tentar ViaCEP se BrasilAPI falhar")]
    public async Task FindAsync_DeveUsarViaCepSeBrasilApiFalhar()
    {
        // Arrange
        var mockBrasilApi = new Mock<ICepServiceControl>();
        var mockViaCep = new Mock<ICepServiceControl>();
        var mockAwesomeApi = new Mock<ICepServiceControl>();
        var mockOpenCep = new Mock<ICepServiceControl>();
        var mockLogger = new Mock<ILogger<CepServiceOrchestrator>>();

        mockBrasilApi.Setup(s => s.FindAsync("01310100", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Timeout"));

        var result = new CepContainer { Cep = "01310-100", Logradouro = "Teste" };
        mockViaCep.Setup(s => s.FindAsync("01310100", It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var orchestrator = new CepServiceOrchestrator(
            new[] { mockBrasilApi.Object, mockViaCep.Object, mockAwesomeApi.Object, mockOpenCep.Object },
            mockLogger.Object);

        // Act
        var response = await orchestrator.FindAsync("01310100", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockViaCep.Verify(s => s.FindAsync("01310100", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar exceção se todos os serviços falharem")]
    public async Task FindAsync_SeTodosServicosFalharem_DeveLancarServiceException()
    {
        // Arrange
        var mockBrasilApi = new Mock<ICepServiceControl>();
        var mockViaCep = new Mock<ICepServiceControl>();
        var mockAwesomeApi = new Mock<ICepServiceControl>();
        var mockOpenCep = new Mock<ICepServiceControl>();
        var mockLogger = new Mock<ILogger<CepServiceOrchestrator>>();

        mockBrasilApi.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Erro"));
        mockViaCep.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Erro"));
        mockAwesomeApi.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Erro"));
        mockOpenCep.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Erro"));

        var orchestrator = new CepServiceOrchestrator(
            new[] { mockBrasilApi.Object, mockViaCep.Object, mockAwesomeApi.Object, mockOpenCep.Object },
            mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceException>(() => 
            orchestrator.FindAsync("01310100", CancellationToken.None));
    }

    [Fact(DisplayName = "Deve retornar null se nenhum serviço encontrar CEP")]
    public async Task FindAsync_SeNenhumEncontrar_DeveRetornarNull()
    {
        // Arrange
        var mockBrasilApi = new Mock<ICepServiceControl>();
        var mockViaCep = new Mock<ICepServiceControl>();
        var mockAwesomeApi = new Mock<ICepServiceControl>();
        var mockOpenCep = new Mock<ICepServiceControl>();
        var mockLogger = new Mock<ILogger<CepServiceOrchestrator>>();

        mockBrasilApi.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CepContainer)null);
        mockViaCep.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CepContainer)null);
        mockAwesomeApi.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CepContainer)null);
        mockOpenCep.Setup(s => s.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CepContainer)null);

        var orchestrator = new CepServiceOrchestrator(
            new[] { mockBrasilApi.Object, mockViaCep.Object, mockAwesomeApi.Object, mockOpenCep.Object },
            mockLogger.Object);

        // Act
        var response = await orchestrator.FindAsync("00000000", CancellationToken.None);

        // Assert
        Assert.Null(response);
    }
}
```

---

## 📋 Checklist de Implementação

### Fase 1: Preparação

- [ ] Ler e entender toda documentação
- [ ] Revisar estrutura existente
- [ ] Preparar ambiente de desenvolvimento

### Fase 2: Implementação dos Serviços

- [ ] BrasilApiService.cs criado e implementado
- [ ] AwesomeApiService.cs criado e implementado
- [ ] OpenCepService.cs criado e implementado
- [ ] ViaCepService refatorado (se necessário)

### Fase 3: Orquestrador

- [ ] CepServiceOrchestrator.cs criado
- [ ] Lógica de fallback implementada
- [ ] Tratamento de exceções configurado

### Fase 4: Testes

- [ ] BrasilApiServiceTest.cs com 100% cobertura
- [ ] AwesomeApiServiceTest.cs com 100% cobertura
- [ ] OpenCepServiceTest.cs com 100% cobertura
- [ ] CepServiceOrchestratorTest.cs com 100% cobertura
- [ ] Todos os testes com DisplayName obrigatório
- [ ] Assertions usando xUnit nativo (sem FluentAssertions)

### Fase 5: Documentação

- [ ] XML documentation em todos os métodos públicos
- [ ] README.md atualizado
- [ ] Comentários em lógica complexa

### Fase 6: Validação

- [ ] `dotnet build` sem warnings
- [ ] `dotnet test` com 100% de sucesso
- [ ] Cobertura de código em 100%
- [ ] Compatibilidade com .NET 8, 9, 10

---

## 📚 Referências Técnicas

### URLs das APIs

| Serviço        | Base URL                  | Endpoint              |
|----------------|---------------------------|-----------------------|
| **BrasilAPI**  | https://brasilapi.com.br  | /api/address/v2/{cep} |
| **ViaCEP**     | https://viacep.com.br     | /ws/{cep}/json        |
| **AwesomeAPI** | https://awesomeapi.com.br | /api/cep/{cep}        |
| **OpenCEP**    | https://cep.dev           | /{cep}                |

### Exemplos de Resposta

#### BrasilAPI

```json
{
  "cep": "01310100",
  "state": "SP",
  "city": "São Paulo",
  "neighborhood": "Bela Vista",
  "street": "Avenida Paulista"
}
```

#### ViaCEP

```json
{
  "cep": "01310-100",
  "logradouro": "Avenida Paulista",
  "complemento": "",
  "bairro": "Bela Vista",
  "localidade": "São Paulo",
  "uf": "SP",
  "ibge": "3550308",
  "gia": "",
  "ddd": "11",
  "siafi": "7107"
}
```

#### AwesomeAPI

```json
{
  "status": 200,
  "ok": true,
  "cep": "01310-100",
  "state": "SP",
  "city": "São Paulo",
  "district": "Bela Vista",
  "address": "Avenida Paulista"
}
```

#### OpenCEP

```json
{
  "status": 200,
  "ok": true,
  "cep": "01310100",
  "state": "SP",
  "city": "São Paulo",
  "neighborhood": "Bela Vista",
  "street": "Avenida Paulista"
}
```

---

**Versão:** 1.4.0  
**Data:** 2026-02-18  
**Status:** Guia de Implementação Completo
