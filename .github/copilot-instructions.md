# GitHub Copilot Instructions - Sirb.CepBrasil

## 🎯 Contexto do Projeto

**Sirb.CepBrasil** é uma biblioteca .NET para consulta de endereços brasileiros através do CEP (Código de Endereçamento Postal) com estratégia inteligente de fallback entre múltiplos serviços públicos.

### Informações Técnicas

- **Linguagem:** C# (latest)
- **Frameworks:** .NET 8.0, 9.0, 10.0 (multi-target)
- **Tipo:** Class Library / NuGet Package
- **Licença:** MIT
- **Repositório:** https://github.com/rodabarbosa/CepBrasil
- **Idioma:** Português Brasileiro (pt-BR)
- **Versão Atual:** 1.4.0
- **⚠️ CRITICAL: XML Documentation Language:** **English only** - All XML documentation must be written in English for international compatibility

### 🔄 Novo Fluxo de Fallback (v1.4.0)

A versão 1.4.0 implementa uma estratégia robusta de busca com fallback automático entre múltiplos provedores:

#### Ordem de Tentativas

1. **BrasilAPI** (https://brasilapi.com.br/) - Primeira tentativa
2. **ViaCEP** (https://viacep.com.br/) - Se BrasilAPI falhar ou não encontrar
3. **AwesomeAPI** (https://awesomeapi.com.br/) - Se ViaCEP falhar ou não encontrar
4. **OpenCEP** (https://github.com/filipedeschamps/cep-promise) - Última tentativa

#### Comportamento por Resultado

| Cenário                         | Ação                                                |
|---------------------------------|-----------------------------------------------------|
| **Sucesso em qualquer serviço** | Retorna resultado imediatamente (não tenta próximo) |
| **CEP não encontrado**          | Tenta o próximo serviço na fila                     |
| **Falha/Erro HTTP**             | Tenta o próximo serviço na fila                     |
| **Erro em todos os 4 serviços** | Lança `ServiceException` com detalhes               |
| **Não encontrado em nenhum**    | Retorna `null`                                      |

#### Exemplo de Fluxo

```
Usuário busca CEP: "01310100"
  ↓
Tenta BrasilAPI → Encontra → ✅ Retorna resultado
  
Usuário busca CEP: "00000000" (inválido)
  ↓
Tenta BrasilAPI → Não encontra
  ↓
Tenta ViaCEP → Não encontra
  ↓
Tenta AwesomeAPI → Não encontra
  ↓
Tenta OpenCEP → Não encontra
  ↓
Retorna null

Usuário busca CEP "01310100", BrasilAPI está down
  ↓
Tenta BrasilAPI → Falha (timeout/erro)
  ↓
Tenta ViaCEP → Encontra → ✅ Retorna resultado

Todos os serviços estão down
  ↓
Lança ServiceException com mensagem clara
```

---

## 📋 Implementação do Novo Fluxo de Fallback

### Classes Envolvidas

#### Serviços a Implementar/Modificar

1. **BrasilApiService** (novo)
    - Implementa busca via BrasilAPI
    - Herda de `ICepServiceControl`
    - Retorna `CepContainer` ou null

2. **ViaCepService** (existente - manter/refatorar)
    - Serviço original já existente
    - Pode ser refatorado para reutilizar código comum

3. **AwesomeApiService** (novo)
    - Implementa busca via AwesomeAPI
    - Herda de `ICepServiceControl`
    - Retorna `CepContainer` ou null

4. **OpenCepService** (novo)
    - Implementa busca via OpenCEP
    - Herda de `ICepServiceControl`
    - Retorna `CepContainer` ou null

5. **CepServiceOrchestrator** ou **CepServiceFacade** (novo)
    - Orquestra o fallback entre serviços
    - Implementa `ICepService`
    - Responsável pela estratégia de tentativas

#### Interface Base

```csharp
// Existente - manter compatibilidade
public interface ICepServiceControl
{
    Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken);
}

// Público - manter compatibilidade
public interface ICepService
{
    Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken);
}
```

### Estrutura de Diretórios

```
Sirb.CepBrasil/
├── Services/
│   ├── BrasilApiService.cs          (novo)
│   ├── ViaCepService.cs             (existente)
│   ├── AwesomeApiService.cs         (novo)
│   ├── OpenCepService.cs            (novo)
│   └── CepServiceOrchestrator.cs    (novo - orquestra fallback)
├── Interfaces/
│   ├── ICepService.cs               (existente)
│   └── ICepServiceControl.cs        (existente)
├── Models/
│   ├── CepResult.cs                 (existente)
│   └── CepContainer.cs              (existente)
└── Exceptions/
    └── ServiceException.cs          (existente - pode precisar atualizar)
```

### Fluxo de Execução

```csharp
// Cliente chama
await cepService.FindAsync("01310100", cancellationToken);

// Orquestrador tenta em ordem
try
{
    var result = await brasilApiService.FindAsync(cep, cancellationToken);
    if (result != null) return new CepResult(result);  // Sucesso
}
catch { /* continua */ }

try
{
    var result = await viaCepService.FindAsync(cep, cancellationToken);
    if (result != null) return new CepResult(result);  // Sucesso
}
catch { /* continua */ }

try
{
    var result = await awesomeApiService.FindAsync(cep, cancellationToken);
    if (result != null) return new CepResult(result);  // Sucesso
}
catch { /* continua */ }

try
{
    var result = await openCepService.FindAsync(cep, cancellationToken);
    if (result != null) return new CepResult(result);  // Sucesso
}
catch { /* continua */ }

// Se chegou aqui, todos falharam
if (todasAsTentativasResultaramEmErro)
    throw new ServiceException("Todos os serviços falharam");  // Erro em todos

return null;  // Não encontrado em nenhum
```

### Testes Esperados

```
BrasilApiServiceTest.cs
- Deve retornar CepContainer quando encontrado
- Deve retornar null quando não encontrado
- Deve lançar exceção quando serviço falha
- Deve respeitar CancellationToken
- Deve fazer chamada HTTPS

ViaCepServiceTest.cs
- (testes existentes + novos para refatoração)

AwesomeApiServiceTest.cs
- Deve retornar CepContainer quando encontrado
- Deve retornar null quando não encontrado
- Deve lançar exceção quando serviço falha

OpenCepServiceTest.cs
- Deve retornar CepContainer quando encontrado
- Deve retornar null quando não encontrado
- Deve lançar exceção quando serviço falha

CepServiceOrchestratorTest.cs
- Deve tentar BrasilAPI primeiro
- Deve tentar ViaCEP se BrasilAPI falhar
- Deve tentar AwesomeAPI se ViaCEP falhar
- Deve tentar OpenCEP se AwesomeAPI falhar
- Deve retornar primeira resposta bem-sucedida
- Deve lançar exceção se todos falharem
- Deve retornar null se nenhum encontrar
- Deve respeitar timeout
- Deve cancelar todas as tentativas se CancellationToken sinalizar
```

---

### 1. **Testes Unitários - 100% de Cobertura**

**OBRIGATÓRIO:** Todo código deve ter 100% de cobertura de testes.

#### Requisitos:

- ✅ Usar **xUnit** como framework de testes
- ✅ Usar **Assert nativo do xUnit** para assertions (sem FluentAssertions)
- ✅ **Nomenclatura clara e descritiva** dos métodos de teste
- ✅ **Atributo `[Fact(DisplayName = "...")]` obrigatório** em todos os testes
- ✅ Testar **todos** os métodos públicos
- ✅ Testar **todos** os edge cases
- ✅ Testar tratamento de exceções
- ✅ Testar comportamentos assíncronos
- ✅ Usar `CancellationToken` nos testes async

#### Estrutura de Nomenclatura:

```
MetodoTestado_Condicao_ResultadoEsperado
```

#### Estrutura de Testes:

```csharp
public class NomeDaClasseTest
{
    /// <summary>
    /// Testa se FindAsync retorna sucesso quando o CEP é válido
    /// </summary>
    [Fact(DisplayName = "Deve retornar sucesso quando CEP é válido e existe no serviço")]
    public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()
    {
        // Arrange
        var servico = new CepService();
        var cep = "01310100";

        // Act
        var resultado = await servico.FindAsync(cep, CancellationToken.None);

        // Assert
        Assert.True(resultado.Success);
        Assert.NotNull(resultado.CepContainer);
        Assert.Equal("01310-100", resultado.CepContainer.Cep);
    }

    /// <summary>
    /// Testa se FindAsync retorna erro quando o CEP é inválido
    /// </summary>
    [Theory(DisplayName = "Deve retornar erro quando CEP é inválido ou vazio")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("abcdefgh")]
    public async Task FindAsync_QuandoCepInvalido_DeveRetornarErro(string cepInvalido)
    {
        // Arrange & Act
        var servico = new CepService();
        var resultado = await servico.FindAsync(cepInvalido, CancellationToken.None);
        
        // Assert
        Assert.False(resultado.Success);
        Assert.NotNullOrEmpty(resultado.Message);
    }
}
```

#### Exemplos de Nomenclatura:

**✅ CORRETO:**

```csharp
[Fact(DisplayName = "Deve retornar sucesso quando CEP é válido")]
public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()

[Fact(DisplayName = "Deve lançar ArgumentNullException quando CEP é nulo")]
public async Task FindAsync_QuandoCepNulo_DeveLancarArgumentNullException()

[Theory(DisplayName = "Deve aceitar CEP com ou sem formatação")]
[InlineData("01310100")]
[InlineData("01310-100")]
public async Task FindAsync_QuandoCepComOuSemFormatacao_DeveRetornarSucesso(string cep)
```

**❌ INCORRETO:**

```csharp
// ❌ Nome genérico, sem DisplayName
[Fact]
public async Task Test1()

// ❌ Nome vago, sem contexto
[Fact]
public async Task TestaCep()

// ❌ Tem DisplayName mas nome do método ruim
[Fact(DisplayName = "Testa CEP")]
public async Task Test()

// ❌ Nome bom mas falta DisplayName
[Fact]
public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()
```

#### Verificação de Cobertura:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

#### Ação Automática:

- **SE** um novo método/classe for criado **SEM** testes
- **ENTÃO** criar automaticamente os testes necessários com nomenclatura adequada e DisplayName
- **AÇÃO:** Não perguntar, apenas criar

---

### 2. **Documentação XML - 100% Obrigatória**

**OBRIGATÓRIO:** Toda classe, método, propriedade e parâmetro público deve ter documentação XML completa e clara.

#### Requisitos:

- ✅ Documentação em **inglês (English)** - OBRIGATÓRIO para compatibilidade internacional
- ✅ Descrição clara e concisa
- ✅ Documentar **todos** os parâmetros
- ✅ Documentar **todos** os retornos
- ✅ Documentar **todas** as exceções que podem ser lançadas
- ✅ Incluir exemplos quando apropriado
- ✅ Usar `<summary>`, `<param>`, `<returns>`, `<exception>`, `<example>`

#### Template Padrão:

```csharp
/// <summary>
/// Searches for address information using the provided postal code (CEP).
/// </summary>
/// <param name="cep">Postal code to query (format: 00000000 or 00000-000)</param>
/// <param name="cancellationToken">Cancellation token for the operation. Default: 30 seconds</param>
/// <returns>
/// Returns a <see cref="CepResult"/> object containing:
/// - Success: true if address was found
/// - CepContainer: found address data
/// - Message: error message (if any)
/// </returns>
/// <exception cref="ArgumentNullException">When the postal code is null or empty</exception>
/// <exception cref="ArgumentException">When the postal code format is invalid</exception>
/// <example>
/// Usage example:
/// <code>
/// var service = new CepService();
/// var result = await service.FindAsync("01310100", CancellationToken.None);
/// if (result.Success)
/// {
///     Console.WriteLine($"Address: {result.CepContainer.Logradouro}");
/// }
/// </code>
/// </example>
public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
{
    // Implementation
}
```

#### Ação Automática:

- **SE** documentação XML está faltando ou incompleta
- **ENTÃO** criar ou atualizar automaticamente
- **AÇÃO:** Não perguntar aprovação, apenas criar/atualizar

---

### 3. **Best Practices - Sempre Aplicar**

#### SOLID Principles

- **S**ingle Responsibility: Uma classe, uma responsabilidade
- **O**pen/Closed: Aberto para extensão, fechado para modificação
- **L**iskov Substitution: Subtipos devem ser substituíveis
- **I**nterface Segregation: Interfaces específicas, não genéricas
- **D**ependency Inversion: Dependa de abstrações, não implementações

#### Clean Code

- ✅ Nomes descritivos e significativos
- ✅ Métodos pequenos (máx 20-30 linhas)
- ✅ Sem comentários óbvios (código auto-explicativo)
- ✅ DRY (Don't Repeat Yourself)
- ✅ KISS (Keep It Simple, Stupid)
- ✅ YAGNI (You Aren't Gonna Need It)

#### Async/Await

- ✅ Sempre suportar `CancellationToken`
- ✅ Usar `ConfigureAwait(false)` em bibliotecas
- ✅ Nunca bloquear com `.Result` ou `.Wait()`
- ✅ Nomear métodos async com sufixo `Async`
- ✅ Retornar `Task` ou `Task<T>`

#### Tratamento de Erros

- ✅ Criar exceções customizadas quando necessário
- ✅ Documentar exceções com `<exception>`
- ✅ Não suprimir exceções sem motivo
- ✅ Usar `try-catch` apenas onde faz sentido
- ✅ Logar erros apropriadamente

---

## 🏗️ Arquitetura do Projeto

### Estrutura de Pastas

```
Sirb.CepBrasil/
├── Exceptions/          # Exceções customizadas
├── Extensions/          # Extension methods
├── Interfaces/          # Interfaces públicas
├── Messages/            # Mensagens de erro/validação
├── Models/              # DTOs e modelos de dados
├── Services/            # Implementações de serviços
└── Validations/         # Validações de entrada

Sirb.CepBrasil.Test/
├── Exceptions/          # Testes de exceções
├── Extensions/          # Testes de extensions
├── Models/              # Testes de models
└── Services/            # Testes de services
```

### Namespaces

- Use `Sirb.CepBrasil` como namespace base
- Sub-namespaces devem seguir estrutura de pastas
- Exemplo: `Sirb.CepBrasil.Services`, `Sirb.CepBrasil.Models`

---

## 💻 Padrões de Código

### Nomenclatura

#### Classes e Interfaces

```csharp
// Classes: PascalCase
public class CepService { }

// Interfaces: I + PascalCase
public interface ICepService { }

// Exceções: Nome + Exception
public class NotFoundException : Exception { }
```

#### Métodos e Propriedades

```csharp
// Métodos: PascalCase
public async Task<CepResult> FindAsync(string cep) { }

// Propriedades: PascalCase
public string Logradouro { get; set; }

// Campos privados: _camelCase
private readonly HttpClient _httpClient;

// Constantes: UPPER_SNAKE_CASE ou PascalCase
private const int DEFAULT_TIMEOUT_SECONDS = 30;
```

#### Parâmetros e Variáveis

```csharp
// camelCase
public void ProcessarCep(string cepFormatado)
{
    var resultado = await _service.FindAsync(cepFormatado);
}
```

### Modificadores de Acesso

```csharp
// Ordem recomendada:
public class ExemploClasse
{
    // 1. Campos privados
    private readonly IService _service;
    private const int Timeout = 30;

    // 2. Construtores
    public ExemploClasse(IService service)
    {
        _service = service;
    }

    // 3. Propriedades públicas
    public string Propriedade { get; set; }

    // 4. Métodos públicos
    public async Task MetodoPublicoAsync() { }

    // 5. Métodos privados
    private void MetodoPrivado() { }
}
```

### Uso de `sealed`

```csharp
// Classes que não devem ser herdadas
public sealed class CepResult { }
public sealed class CepContainer { }
```

---

## 🧪 Padrões de Testes

### Nomenclatura de Testes

#### Estrutura Obrigatória:

```
MetodoTestado_Condicao_ResultadoEsperado
```

#### Atributo DisplayName Obrigatório:

**TODOS** os testes devem ter o atributo `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]` com descrição clara em português.

#### Exemplos Corretos:

```csharp
/// <summary>
/// Verifica se FindAsync retorna sucesso quando CEP é válido
/// </summary>
[Fact(DisplayName = "Deve retornar sucesso quando CEP é válido")]
public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso() { }

/// <summary>
/// Verifica se FindAsync retorna erro quando CEP é inválido
/// </summary>
[Fact(DisplayName = "Deve retornar erro quando CEP é inválido")]
public async Task FindAsync_QuandoCepInvalido_DeveRetornarErro() { }

/// <summary>
/// Verifica se FindAsync lança exceção quando serviço está indisponível
/// </summary>
[Fact(DisplayName = "Deve lançar ServiceException quando serviço está indisponível")]
public async Task FindAsync_QuandoServicoIndisponivel_DeveLancarServiceException() { }
```

### Estrutura AAA (Arrange-Act-Assert)

```csharp
/// <summary>
/// Testa se FindAsync retorna sucesso quando CEP é válido
/// </summary>
[Fact(DisplayName = "Deve retornar sucesso quando CEP é válido e existir no serviço")]
public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()
{
    // Arrange - Preparar
    var service = new CepService();
    var cep = "01310100";

    // Act - Executar
    var result = await service.FindAsync(cep, CancellationToken.None);

    // Assert - Verificar
    Assert.NotNull(result);
    Assert.True(result.Success);
    Assert.NotNull(result.CepContainer);
    Assert.Equal("01310-100", result.CepContainer.Cep);
}
```

### Assert Nativo do xUnit

```csharp
// Verdade/Falsidade
Assert.True(condicao);
Assert.False(condicao);

// Nulidade
Assert.Null(objeto);
Assert.NotNull(objeto);

// Igualdade
Assert.Equal(esperado, atual);
Assert.NotEqual(naoEsperado, atual);

// Strings
Assert.Empty(string.Empty);
Assert.NotEmpty("conteudo");
Assert.Contains("substring", "string com substring");
Assert.StartsWith("início", "início do texto");
Assert.EndsWith("fim", "texto com fim");

// Coleções
Assert.Empty(colecaoVazia);
Assert.NotEmpty(colecaoComItens);
Assert.Single(colecaoComUmItem);
Assert.Contains(item, colecao);

// Exceções
Assert.Throws<ArgumentNullException>(() => metodo(null));
await Assert.ThrowsAsync<ServiceException>(() => metodoAsync());

// Tipo
Assert.IsType<TipoEsperado>(objeto);
Assert.IsNotType<TipoNaoEsperado>(objeto);

// Verificação de múltiplas condições
Assert.Multiple(
    () => Assert.True(resultado1),
    () => Assert.False(resultado2),
    () => Assert.Equal(esperado, atual)
);
```

### Testes Parametrizados

```csharp
/// <summary>
/// Testa se FindAsync valida corretamente diferentes formatos de CEP
/// </summary>
[Theory(DisplayName = "Deve validar corretamente diferentes formatos de CEP")]
[InlineData("01310100", true)]
[InlineData("01310-100", true)]
[InlineData("", false)]
[InlineData(null, false)]
[InlineData("123", false)]
[InlineData("abcdefgh", false)]
public async Task FindAsync_ComDiferentesCeps_DeveValidarCorretamente(
    string cep, 
    bool esperaSucesso)
{
    // Arrange
    var service = new CepService();
    
    // Act
    var result = await service.FindAsync(cep, CancellationToken.None);
    
    // Assert
    Assert.Equal(esperaSucesso, result.Success);
}
```

---

## 🔒 Segurança

### Validação de Entrada

```csharp
public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
{
    // Sempre validar entradas
    if (string.IsNullOrWhiteSpace(cep))
    {
        return new CepResult("CEP não pode ser vazio");
    }

    // Validar formato
    if (!CepValidation.IsValid(cep))
    {
        return new CepResult($"CEP '{cep}' está em formato inválido");
    }

    // Processar...
}
```

### HttpClient

```csharp
// NUNCA criar HttpClient em métodos
// ❌ ERRADO
public async Task MetodoErrado()
{
    using var client = new HttpClient(); // ❌ Causa socket exhaustion
}

// ✅ CORRETO: Injetar HttpClient ou reutilizar instância
private readonly HttpClient _httpClient;

public CepService(HttpClient httpClient)
{
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
}
```

### Timeout

```csharp
// Sempre suportar CancellationToken com timeout padrão
private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken = default)
{
    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    cts.CancelAfter(DefaultTimeout);
    
    // Usar cts.Token nas chamadas
}
```

---

## 📦 Compatibilidade Multi-Target

### Diretivas de Compilação

```csharp
#if NET5_0_OR_GREATER
    // Código específico para .NET 5+
#elif NETSTANDARD2_0
    // Código para .NET Standard 2.0
#endif
```

### APIs Condicionais

```csharp
// Usar APIs disponíveis em todas as versões target
// Evitar APIs específicas de versão a menos que necessário
```

---

## 🎨 Formatação de Código

### EditorConfig

O projeto usa `.editorconfig` para garantir consistência.

### Convenções:

- **Indentação:** 4 espaços
- **Line endings:** LF (Unix)
- **Encoding:** UTF-8
- **Chaves:** Nova linha (Allman style)
- **Espaços:** Após palavras-chave, não após nome de método

```csharp
// ✅ CORRETO
public async Task<CepResult> FindAsync(string cep)
{
    if (string.IsNullOrEmpty(cep))
    {
        return new CepResult("Erro");
    }
    
    var result = await ProcessarAsync(cep);
    return result;
}

// ❌ ERRADO
public async Task<CepResult> FindAsync(string cep){
    if(string.IsNullOrEmpty(cep)){
        return new CepResult("Erro");}
    var result=await ProcessarAsync(cep);return result;}
```

---

## 🔄 Workflow de Desenvolvimento

### 1. Nova Funcionalidade

#### Passo 1: Planejar

```bash
@plan Planejar implementação de [feature]
```

#### Passo 2: Criar Testes (RED)

```bash
@tdd-red Criar testes para [feature]
```

- Criar testes que falham
- Documentar comportamento esperado
- Cobrir edge cases

#### Passo 3: Implementar (GREEN)

```bash
@tdd-green Implementar [feature]
```

- Código mínimo para passar testes
- Não over-engineer
- Focar em funcionalidade

#### Passo 4: Refatorar (REFACTOR)

```bash
@tdd-refactor Refatorar [código]
```

- Melhorar qualidade
- Aplicar SOLID
- Otimizar se necessário
- Manter testes verdes

#### Passo 5: Documentar

```bash
@se-technical-writer Documentar [classe/método]
```

- Criar/atualizar XML documentation
- Atualizar README se necessário
- Adicionar exemplos

#### Passo 6: Revisar

```bash
@se-security-reviewer Revisar segurança
@CSharpExpert Revisar implementação
```

---

## 📝 Checklist de Pull Request

Antes de criar PR, verificar:

### Código

- [ ] Todo código está coberto por testes (100%)
- [ ] Todos os testes passam
- [ ] Cobertura verificada com `dotnet test /p:CollectCoverage=true`
- [ ] Código segue SOLID principles
- [ ] Código segue Clean Code
- [ ] Sem código comentado ou morto
- [ ] Nomes são claros e descritivos

### Documentação

- [ ] Toda classe/método público tem XML documentation
- [ ] **Documentação XML está em INGLÊS (English only)**
- [ ] Documentação está completa (`<summary>`, `<param>`, `<returns>`, `<exception>`)
- [ ] README.md atualizado (se necessário)
- [ ] Exemplos de uso incluídos quando apropriado

### Testes

- [ ] Testes unitários criados para novo código
- [ ] Testes existentes atualizados (se necessário)
- [ ] **Nomenclatura de testes clara** (Metodo_Quando_Deve)
- [ ] **Todos os testes têm `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]`**
- [ ] Edge cases cobertos
- [ ] Testes de exceções incluídos
- [ ] FluentAssertions usado para assertions

### Build

- [ ] `dotnet build` executa sem warnings
- [ ] `dotnet test` passa 100%
- [ ] `dotnet pack` cria pacote NuGet sem erros
- [ ] Compatível com .NET 8, 9 e 10

### Qualidade

- [ ] Sem vulnerabilidades de segurança
- [ ] Sem code smells
- [ ] Performance considerada
- [ ] Sem breaking changes (ou documentado)

---

## 🚫 O Que NÃO Fazer

### ❌ NUNCA:

1. Criar código sem testes
2. Criar métodos/classes públicos sem XML documentation
3. **Escrever documentação XML em português (deve ser INGLÊS)**
4. **Criar testes sem o atributo `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]`**
5. **Usar nomenclatura genérica em testes** (Test1, TestaCep, etc.)
6. Suprimir exceções silenciosamente
7. Usar `Thread.Sleep()` em código assíncrono
8. Criar `HttpClient` em métodos (usar DI ou singleton)
9. Ignorar `CancellationToken`
10. Usar `.Result` ou `.Wait()` em código async
11. Deixar código comentado no commit
12. Ter warnings de compilação
13. Ter testes que passam "por sorte"

### ⚠️ EVITAR:

1. Métodos com mais de 30 linhas
2. Classes com mais de 300 linhas
3. Aninhamento maior que 3 níveis
4. Magic numbers sem constantes
5. Comentários óbvios
6. Nomenclatura genérica (ex: `Manager`, `Helper`, `Util`)

---

## 🛠️ Ferramentas e Comandos

### Build e Testes

```bash
# Build completo
dotnet build

# Executar testes
dotnet test

# Testes com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov

# Criar pacote NuGet
dotnet pack

# Restaurar dependências
dotnet restore
```

### Análise de Código

```bash
# Formatar código
dotnet format

# Verificar formatação
dotnet format --verify-no-changes

# Analisar código (se tiver analyzers configurados)
dotnet build /p:TreatWarningsAsErrors=true
```

---

## 📚 Referências

### Documentação Oficial

- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET API Guidelines](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)

### SOLID e Clean Code

- [Clean Code by Robert C. Martin](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

### Async/Await

- [Async/Await Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)

---

## 🎯 Objetivos do Projeto

1. **Qualidade:** Código limpo, testado e documentado
2. **Performance:** Rápido e eficiente
3. **Confiabilidade:** Tratamento robusto de erros
4. **Manutenibilidade:** Fácil de entender e modificar
5. **Usabilidade:** API simples e intuitiva
6. **Compatibilidade:** Multi-target para diferentes versões .NET

---

## 💡 Dicas para GitHub Copilot

### Quando Criar Código

- Sempre incluir XML documentation
- Sempre criar testes correspondentes
- Aplicar SOLID principles
- Usar async/await quando apropriado
- Suportar CancellationToken

### Quando Criar Testes

- Usar nomenclatura: `Metodo_Quando_Deve`
- Estruturar com AAA (Arrange-Act-Assert)
- Usar Assert nativo do xUnit
- Adicionar `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]`
- Cobrir casos de sucesso e erro
- Cobrir edge cases

### Quando Documentar

- Ser claro e conciso
- Incluir exemplos quando útil
- Documentar exceções que podem ser lançadas
- Usar português brasileiro
- Evitar comentários óbvios

---

## 📞 Suporte

Para questões ou dúvidas:

1. Consulte `AGENTS.md` para ver agentes disponíveis
2. Revise `README.md` para contexto do projeto
3. Analise código existente como referência
4. Use `@plan` para planejar mudanças complexas

---

**Última atualização:** 2026-02-17

**Versão:** 1.4.0
