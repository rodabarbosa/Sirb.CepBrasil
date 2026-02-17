# GitHub Copilot Instructions - Sirb.CepBrasil

## 🎯 Contexto do Projeto

**Sirb.CepBrasil** é uma biblioteca .NET para consulta de endereços brasileiros através do CEP (Código de Endereçamento Postal), utilizando o serviço público ViaCEP.

### Informações Técnicas
- **Linguagem:** C# (latest)
- **Frameworks:** .NET 8.0, 9.0, 10.0 (multi-target)
- **Tipo:** Class Library / NuGet Package
- **Licença:** MIT
- **Repositório:** https://github.com/rodabarbosa/CepBrasil
- **Idioma:** Português Brasileiro (pt-BR)
- **Versão Atual:** 1.4.0

---

## 🚨 REGRAS OBRIGATÓRIAS E NÃO NEGOCIÁVEIS

### 1. **Testes Unitários - 100% de Cobertura**

**OBRIGATÓRIO:** Todo código deve ter 100% de cobertura de testes.

#### Requisitos:
- ✅ Usar **xUnit** como framework de testes
- ✅ Usar **FluentAssertions** para assertions
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
        resultado.Success.Should().BeTrue();
        resultado.CepContainer.Should().NotBeNull();
        resultado.CepContainer.Cep.Should().Be("01310-100");
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
        // Arrange & Act & Assert
        var servico = new CepService();
        var resultado = await servico.FindAsync(cepInvalido, CancellationToken.None);
        
        resultado.Success.Should().BeFalse();
        resultado.Message.Should().NotBeNullOrEmpty();
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
- ✅ Documentação em **português brasileiro**
- ✅ Descrição clara e concisa
- ✅ Documentar **todos** os parâmetros
- ✅ Documentar **todos** os retornos
- ✅ Documentar **todas** as exceções que podem ser lançadas
- ✅ Incluir exemplos quando apropriado
- ✅ Usar `<summary>`, `<param>`, `<returns>`, `<exception>`, `<example>`

#### Template Padrão:
```csharp
/// <summary>
/// Busca informações de endereço através do CEP fornecido.
/// </summary>
/// <param name="cep">CEP a ser consultado (formato: 00000000 ou 00000-000)</param>
/// <param name="cancellationToken">Token para cancelamento da operação. Padrão: 30 segundos</param>
/// <returns>
/// Retorna um objeto <see cref="CepResult"/> contendo:
/// - Success: true se encontrou o endereço
/// - CepContainer: dados do endereço encontrado
/// - Message: mensagem de erro (se houver)
/// </returns>
/// <exception cref="ArgumentNullException">Quando o CEP é nulo ou vazio</exception>
/// <exception cref="ArgumentException">Quando o CEP está em formato inválido</exception>
/// <example>
/// Exemplo de uso:
/// <code>
/// var service = new CepService();
/// var result = await service.FindAsync("01310100", CancellationToken.None);
/// if (result.Success)
/// {
///     Console.WriteLine($"Endereço: {result.CepContainer.Logradouro}");
/// }
/// </code>
/// </example>
public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
{
    // Implementação
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
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.CepContainer.Should().NotBeNull();
    result.CepContainer.Cep.Should().Be("01310-100");
}
```

### FluentAssertions
```csharp
// Usar FluentAssertions ao invés de Assert do xUnit
result.Success.Should().BeTrue();
result.Message.Should().NotBeNullOrEmpty();
result.CepContainer.Should().NotBeNull();
result.Exceptions.Should().BeEmpty();

// Coleções
lista.Should().HaveCount(3);
lista.Should().Contain(x => x.Cep == "01310100");
lista.Should().NotContainNulls();

// Exceções
var act = () => servico.Metodo(null);
await act.Should().ThrowAsync<ArgumentNullException>();
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
    result.Success.Should().Be(esperaSucesso);
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
3. **Criar testes sem o atributo `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]`**
4. **Usar nomenclatura genérica em testes** (Test1, TestaCep, etc.)
5. Suprimir exceções silenciosamente
6. Usar `Thread.Sleep()` em código assíncrono
7. Criar `HttpClient` em métodos (usar DI ou singleton)
8. Ignorar `CancellationToken`
9. Usar `.Result` ou `.Wait()` em código async
10. Deixar código comentado no commit
11. Ter warnings de compilação
12. Ter testes que passam "por sorte"

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
- Usar FluentAssertions
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
