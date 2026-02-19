# Fluxo de Fallback - Documentação de Implementação

## 📋 Resumo Executivo

A versão 1.4.0 do Sirb.CepBrasil implementa uma estratégia robusta de fallback entre múltiplos provedores de serviços de CEP para aumentar a confiabilidade e disponibilidade da biblioteca.

**Objetivo:** Garantir que o usuário consiga buscar um CEP mesmo que um ou mais serviços estejam indisponíveis.

---

## 🔄 Fluxo de Execução Detalhado

### Entrada

```
FindAsync(cep: "01310100", cancellationToken)
```

### Processamento

```
1. Validação
   ├─ CEP não é nulo/vazio?
   └─ CEP tem 8 dígitos numéricos?
   
   ✓ Sim → Continua
   ✗ Não → Retorna CepResult com erro

2. Tentativa 1: BrasilAPI
   ├─ Fazer requisição HTTP GET
   ├─ Respeitar timeout (padrão 30s)
   ├─ Respeitar CancellationToken
   └─ Se encontrado → Retorna CepContainer
      Se não encontrado → Tenta próximo
      Se erro → Tenta próximo

3. Tentativa 2: ViaCEP
   └─ Mesma lógica da BrasilAPI
      Se encontrado → Retorna CepContainer
      Se não encontrado → Tenta próximo
      Se erro → Tenta próximo

4. Tentativa 3: AwesomeAPI
   └─ Mesma lógica das anteriores
      Se encontrado → Retorna CepContainer
      Se não encontrado → Tenta próximo
      Se erro → Tenta próximo

5. Tentativa 4: OpenCEP
   └─ Última tentativa
      Se encontrado → Retorna CepContainer
      Se não encontrado → Retorna null
      Se erro → Coleta erro

6. Tratamento Final
   ├─ Se encontrado em qualquer serviço → Retorna sucesso
   ├─ Se não encontrado em nenhum → Retorna null
   └─ Se erro em TODOS os serviços → Lança ServiceException
```

### Saída

**Cenário 1: Sucesso**

```csharp
CepResult {
    Success = true,
    CepContainer = {
        Cep = "01310-100",
        Logradouro = "Avenida Paulista",
        Bairro = "Bela Vista",
        Cidade = "São Paulo",
        Uf = "SP"
    },
    Message = null,
    Exceptions = []
}
```

**Cenário 2: Não Encontrado**

```csharp
null  // Retorna null, não CepResult
```

**Cenário 3: Todos os Serviços Falharam**

```csharp
throw new ServiceException("Todos os serviços de busca de CEP falharam");
```

---

## 🏗️ Arquitetura de Implementação

### Padrão de Design: Strategy + Facade

```
Interface ICepServiceControl
        ↑
        ├─ BrasilApiService (Strategy 1)
        ├─ ViaCepService (Strategy 2)
        ├─ AwesomeApiService (Strategy 3)
        └─ OpenCepService (Strategy 4)

Interface ICepService (Pública)
        ↑
        └─ CepServiceFacade (Orquestra fallback)
```

### Classes a Implementar/Modificar

#### 1. Interface ICepServiceControl (Existente)

```csharp
namespace Sirb.CepBrasil.Interfaces
{
    /// <summary>
    /// Interface para controle de serviços de busca de CEP
    /// </summary>
    public interface ICepServiceControl
    {
        /// <summary>
        /// Busca o CEP em um provedor específico
        /// </summary>
        /// <param name="cep">CEP formatado ou não</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>CepContainer se encontrado, null se não encontrado</returns>
        /// <exception cref="ServiceException">Se houver erro na requisição</exception>
        Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken);
    }
}
```

#### 2. BrasilApiService (Novo)

```csharp
namespace Sirb.CepBrasil.Services
{
    /// <summary>
    /// Serviço de busca de CEP via BrasilAPI
    /// </summary>
    internal sealed class BrasilApiService : ICepServiceControl
    {
        private readonly HttpClient _httpClient;

        public BrasilApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
        {
            // Implementação...
        }

        private string BuildRequestUrl(string cep) => $"https://brasilapi.com.br/api/cep/v1/{cep}";
    }
}
```

#### 3. AwesomeApiService (Novo)

```csharp
namespace Sirb.CepBrasil.Services
{
    /// <summary>
    /// Serviço de busca de CEP via AwesomeAPI
    /// </summary>
    internal sealed class AwesomeApiService : ICepServiceControl
    {
        // Implementação similar à BrasilApiService
    }
}
```

#### 4. OpenCepService (Novo)

```csharp
namespace Sirb.CepBrasil.Services
{
    /// <summary>
    /// Serviço de busca de CEP via OpenCEP
    /// </summary>
    internal sealed class OpenCepService : ICepServiceControl
    {
        // Implementação similar à BrasilApiService
    }
}
```

#### 5. CepServiceOrchestrator (Novo)

```csharp
namespace Sirb.CepBrasil.Services
{
    /// <summary>
    /// Orquestra a busca de CEP com fallback entre múltiplos provedores
    /// </summary>
    public sealed class CepServiceOrchestrator : ICepService
    {
        private readonly ICepServiceControl[] _services;

        public CepServiceOrchestrator(
            BrasilApiService brasilApi,
            ViaCepService viaCep,
            AwesomeApiService awesomeApi,
            OpenCepService openCep)
        {
            _services = new ICepServiceControl[] { brasilApi, viaCep, awesomeApi, openCep };
        }

        /// <summary>
        /// Busca CEP com fallback automático entre provedores
        /// </summary>
        public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
        {
            // 1. Validar CEP
            // 2. Tentar cada serviço em ordem
            // 3. Retornar primeiro sucesso
            // 4. Se todos falharem, lançar exceção
            // 5. Se não encontrado em nenhum, retornar null
        }
    }
}
```

---

## 🧪 Estratégia de Testes

### Estrutura de Testes

```
Sirb.CepBrasil.Test/
├── Services/
│   ├── BrasilApiServiceTest.cs
│   ├── AwesomeApiServiceTest.cs
│   ├── OpenCepServiceTest.cs
│   ├── ViaCepServiceTest.cs (atualizar)
│   └── CepServiceOrchestratorTest.cs
└── ...
```

### Cenários de Teste Principais

#### BrasilApiServiceTest

```csharp
[Fact(DisplayName = "Deve retornar CepContainer quando encontrado")]
public async Task FindAsync_QuandoEncontrado_DeveRetornarCepContainer()

[Fact(DisplayName = "Deve retornar null quando não encontrado")]
public async Task FindAsync_QuandoNaoEncontrado_DeveRetornarNull()

[Fact(DisplayName = "Deve lançar ServiceException quando serviço retorna erro")]
public async Task FindAsync_QuandoServicoFalha_DeveLancarServiceException()

[Fact(DisplayName = "Deve respeitar CancellationToken")]
public async Task FindAsync_QuandoCancelado_DeveRespeitar()

[Fact(DisplayName = "Deve usar HTTPS")]
public async Task FindAsync_DeveUsarHttps()
```

#### CepServiceOrchestratorTest

```csharp
[Fact(DisplayName = "Deve tentar BrasilAPI primeiro")]
public async Task FindAsync_DeveAtentarBrasilApiPrimeiro()

[Fact(DisplayName = "Deve retornar sucesso de BrasilAPI sem tentar outros")]
public async Task FindAsync_BrasilApiSucesso_NaoTentaOutros()

[Fact(DisplayName = "Deve tentar ViaCEP se BrasilAPI falhar")]
public async Task FindAsync_BrasilApiFalha_DeveTentarViaCEP()

[Fact(DisplayName = "Deve tentar AwesomeAPI se ViaCEP falhar")]
public async Task FindAsync_ViaCepFalha_DeveTentarAwesomeApi()

[Fact(DisplayName = "Deve tentar OpenCEP se AwesomeAPI falhar")]
public async Task FindAsync_AwesomeApiFalha_DeveTentarOpenCep()

[Fact(DisplayName = "Deve lançar ServiceException se todos falharem")]
public async Task FindAsync_TodosFalham_DeveLancarServiceException()

[Fact(DisplayName = "Deve retornar null se nenhum encontrar")]
public async Task FindAsync_NenhunEncontra_DeveRetornarNull()

[Fact(DisplayName = "Deve cancelar todas as tentativas se CancellationToken sinalizar")]
public async Task FindAsync_CancelementToken_DeveCancelarTodas()

[Theory(DisplayName = "Deve tentar próximo se serviço retornar null")]
[InlineData(0)]  // BrasilAPI retorna null
[InlineData(1)]  // ViaCEP retorna null
[InlineData(2)]  // AwesomeAPI retorna null
public async Task FindAsync_ServicoRetornaNul_DeveTentarProximo(int indiceQueFalha)
```

---

## 📊 Fluxo de Fallback Detalhado

### Exemplo 1: BrasilAPI Encontra

```
Usuario: FindAsync("01310100")
  ↓
Valida CEP ✓
  ↓
Tenta BrasilAPI ✓ (encontrado)
  ↓
Retorna: CepResult { Success = true, CepContainer = {...} }
```

### Exemplo 2: BrasilAPI Falha, ViaCEP Encontra

```
Usuario: FindAsync("01310100")
  ↓
Valida CEP ✓
  ↓
Tenta BrasilAPI ✗ (timeout/erro)
  ↓
Tenta ViaCEP ✓ (encontrado)
  ↓
Retorna: CepResult { Success = true, CepContainer = {...} }
```

### Exemplo 3: Todos Falham

```
Usuario: FindAsync("01310100")
  ↓
Valida CEP ✓
  ↓
Tenta BrasilAPI ✗ (erro)
  ↓
Tenta ViaCEP ✗ (erro)
  ↓
Tenta AwesomeAPI ✗ (erro)
  ↓
Tenta OpenCEP ✗ (erro)
  ↓
Lança: ServiceException("Todos os serviços falharam")
```

### Exemplo 4: Nenhum Encontra

```
Usuario: FindAsync("99999999")
  ↓
Valida CEP ✓
  ↓
Tenta BrasilAPI (não encontrado)
  ↓
Tenta ViaCEP (não encontrado)
  ↓
Tenta AwesomeAPI (não encontrado)
  ↓
Tenta OpenCEP (não encontrado)
  ↓
Retorna: null
```

---

## 📝 Tratamento de Erros

### Exceções Esperadas

#### ServiceException

- Lançada quando TODOS os serviços falham
- Mensagem clara indicando o motivo
- Inclui detalhes dos erros de cada tentativa

```csharp
try
{
    // todas as 4 tentativas falharam
    throw new ServiceException(
        "Falha ao buscar CEP em todos os provedores: " +
        "BrasilAPI: timeout, " +
        "ViaCEP: erro 500, " +
        "AwesomeAPI: timeout, " +
        "OpenCEP: erro 503"
    );
}
catch (ServiceException ex)
{
    // Tratar erro
}
```

### Casos de "Não Encontrado"

Quando um CEP não é encontrado em nenhum serviço, retorna `null`:

```csharp
var result = await cepService.FindAsync("00000000", cancellationToken);
Assert.Null(result);  // Nenhum serviço tem este CEP
```

---

## 🚀 Implementação Passo a Passo

### Fase 1: Testes (RED)

1. Criar testes para BrasilApiService
2. Criar testes para AwesomeApiService
3. Criar testes para OpenCepService
4. Criar testes para CepServiceOrchestrator
5. Todos os testes devem falhar inicialmente

### Fase 2: Implementação (GREEN)

1. Implementar BrasilApiService
2. Implementar AwesomeApiService
3. Implementar OpenCepService
4. Refatorar ViaCepService se necessário
5. Implementar CepServiceOrchestrator
6. Todos os testes devem passar

### Fase 3: Refatoração (REFACTOR)

1. Extrair código comum em classe base
2. Aplicar padrão Strategy
3. Otimizar tratamento de erro
4. Melhorar logging

### Fase 4: Documentação

1. Atualizar README.md
2. Criar documentação XML completa
3. Documentar cada serviço
4. Adicionar exemplos de uso

### Fase 5: Validação

1. Verificar 100% de cobertura de testes
2. Validar segurança (HTTPS, validação de entrada)
3. Testar com CancellationToken
4. Validar timeout padrão

---

## ✅ Checklist de Conclusão

- [ ] 4 novos serviços implementados (BrasilAPI, AwesomeAPI, OpenCEP, Orquestrador)
- [ ] 100% de cobertura de testes
- [ ] Todos os testes com DisplayName descritivo
- [ ] XML documentation completa
- [ ] README.md atualizado com novo fluxo
- [ ] Compatibilidade .NET 8, 9, 10
- [ ] Sem vulnerabilidades de segurança
- [ ] Build sem warnings
- [ ] Testes passando
- [ ] Performance validada

---

## 📚 Referências

- [BrasilAPI](https://brasilapi.com.br/)
- [ViaCEP](https://viacep.com.br/)
- [AwesomeAPI](https://awesomeapi.com.br/)
- [OpenCEP](https://github.com/filipedeschamps/cep-promise)
- [Strategy Pattern](https://refactoring.guru/design-patterns/strategy)
- [Facade Pattern](https://refactoring.guru/design-patterns/facade)

---

**Versão**: 1.4.0  
**Data**: 2026-02-18
