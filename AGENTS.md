# Agents Configuration

Este documento define os agentes especializados disponíveis para o projeto **Sirb.CepBrasil**.

## 📋 Visão Geral

O projeto utiliza agentes especializados do GitHub Copilot para auxiliar no desenvolvimento, testes, documentação e manutenção da biblioteca de consulta de CEP.

## 🤖 Agentes Disponíveis

### 1. **CSharpExpert** (Principal)
**Descrição:** Agente especializado em desenvolvimento .NET e C#.

**Quando usar:**
- Desenvolvimento de novas funcionalidades
- Refatoração de código existente
- Implementação de patterns e best practices
- Otimização de performance

**Responsabilidades:**
- Garantir código C# limpo e eficiente
- Aplicar SOLID principles
- Implementar design patterns adequados
- Seguir convenções .NET

**Comando:**
```
@CSharpExpert [sua solicitação]
```

---

### 2. **tdd-red** (Testes - Fase RED)
**Descrição:** Guia o desenvolvimento test-first criando testes que falham antes da implementação.

**Quando usar:**
- Início de nova funcionalidade
- Definição de comportamento esperado
- Criação de testes para requisitos

**Responsabilidades:**
- Criar testes xUnit que falham
- Documentar comportamento esperado
- Definir cenários de teste completos
- Garantir cobertura de edge cases

**Comando:**
```
@tdd-red Criar testes para [funcionalidade]
```

---

### 3. **tdd-green** (Testes - Fase GREEN)
**Descrição:** Implementa código mínimo para satisfazer os testes criados na fase RED.

**Quando usar:**
- Após criar testes na fase RED
- Implementação de funcionalidade mínima
- Fazer testes passarem

**Responsabilidades:**
- Implementar código que faz testes passarem
- Manter simplicidade
- Evitar over-engineering
- Garantir todos os testes passam

**Comando:**
```
@tdd-green Implementar código para passar nos testes
```

---

### 4. **tdd-refactor** (Testes - Fase REFACTOR)
**Descrição:** Melhora qualidade do código mantendo testes verdes.

**Quando usar:**
- Após testes passarem (fase GREEN)
- Melhorias de qualidade de código
- Aplicação de best practices
- Otimizações

**Responsabilidades:**
- Refatorar mantendo testes verdes
- Aplicar SOLID e Clean Code
- Melhorar performance quando necessário
- Atualizar documentação XML

**Comando:**
```
@tdd-refactor Refatorar [classe/método]
```

---

### 5. **se-technical-writer** (Documentação)
**Descrição:** Especialista em documentação técnica e XML documentation.

**Quando usar:**
- Criação/atualização de documentação XML
- Atualização de README.md
- Documentação de APIs públicas
- Exemplos de código

**Responsabilidades:**
- Criar documentação XML completa e clara
- Manter README.md atualizado
- Documentar todos os métodos públicos
- Criar exemplos de uso

**Comando:**
```
@se-technical-writer Documentar [classe/método]
```

---

### 6. **se-security-reviewer** (Segurança)
**Descrição:** Revisor focado em segurança e OWASP.

**Quando usar:**
- Code review de segurança
- Validação de entrada de dados
- Análise de vulnerabilidades
- Auditoria de código

**Responsabilidades:**
- Identificar vulnerabilidades
- Validar tratamento de exceções
- Verificar validação de entrada
- Garantir práticas seguras

**Comando:**
```
@se-security-reviewer Revisar segurança de [código]
```

---

### 7. **janitor** (Manutenção)
**Descrição:** Realiza tarefas de limpeza e manutenção do código.

**Quando usar:**
- Limpeza de código legado
- Remoção de código morto
- Simplificação de código complexo
- Organização de namespaces

**Responsabilidades:**
- Remover código não utilizado
- Simplificar código complexo
- Organizar estrutura de arquivos
- Atualizar dependências

**Comando:**
```
@janitor Limpar e organizar [área do código]
```

---

### 8. **dotnet-upgrade** (Modernização)
**Descrição:** Especialista em atualização e modernização de código .NET.

**Quando usar:**
- Atualização de versão do .NET
- Migração de APIs obsoletas
- Modernização de código legado
- Aplicação de novos recursos da linguagem

**Responsabilidades:**
- Atualizar código para versões recentes
- Substituir APIs obsoletas
- Aplicar novos recursos do C#
- Manter compatibilidade multi-target

**Comando:**
```
@dotnet-upgrade Modernizar [código/projeto]
```

---

### 9. **plan** (Planejamento)
**Descrição:** Assistente de planejamento estratégico e análise arquitetural.

**Quando usar:**
- Planejamento de novas features
- Análise de impacto de mudanças
- Definição de estratégia de implementação
- Revisão arquitetural

**Responsabilidades:**
- Criar planos de implementação
- Analisar impacto de mudanças
- Sugerir arquitetura adequada
- Documentar decisões técnicas

**Comando:**
```
@plan Planejar implementação de [feature]
```

---

## 🔄 Workflow TDD Recomendado

Para desenvolvimento de novas funcionalidades, siga este workflow:

### 1. **Planejamento**
```bash
@plan Planejar implementação de busca de CEP por múltiplos provedores
```

### 2. **RED - Criar Testes**
```bash
@tdd-red Criar testes para validação de formato de CEP
```

### 3. **GREEN - Implementar**
```bash
@tdd-green Implementar validação de CEP
```

### 4. **REFACTOR - Melhorar**
```bash
@tdd-refactor Refatorar validação de CEP aplicando best practices
```

### 5. **Documentar**
```bash
@se-technical-writer Documentar classe CepValidation
```

### 6. **Revisar Segurança**
```bash
@se-security-reviewer Revisar tratamento de entrada na validação de CEP
```

---

## 📊 Cobertura de Testes

### Requisito Obrigatório
- **100% de cobertura de código**
- Todos os métodos públicos devem ter testes
- Todos os edge cases devem ser testados
- Testes devem usar xUnit + FluentAssertions
- **Nomenclatura clara e descritiva** para métodos de teste
- **Atributo `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]` obrigatório** para descrever o teste em português

### Padrão de Nomenclatura de Testes

#### Estrutura do Nome do Método
```
MetodoTestado_Condicao_ResultadoEsperado
```

#### Exemplos:
```csharp
// ✅ BOM: Nome claro e descritivo
[Fact(DisplayName = "Deve retornar sucesso quando CEP é válido")]
public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()

[Fact(DisplayName = "Deve retornar erro quando CEP está vazio")]
public async Task FindAsync_QuandoCepVazio_DeveRetornarErro()

[Theory(DisplayName = "Deve retornar erro quando CEP é nulo ou vazio")]
[InlineData("")]
[InlineData(null)]
public async Task FindAsync_QuandoCepNuloOuVazio_DeveRetornarErro(string cep)

// ❌ RUIM: Nome genérico, sem DisplayName
[Fact]
public async Task Test1()

// ❌ RUIM: Nome vago, sem contexto
[Fact]
public async Task TestaCep()
```

### Verificação de Cobertura
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

---

## 📝 Documentação XML

### Requisito Obrigatório
- **100% de documentação XML**
- Todas as classes, métodos e propriedades públicas devem ter XML documentation
- Documentação deve ser clara e em português
- Incluir exemplos quando apropriado

### Template de Documentação

#### Código de Produção
```csharp
/// <summary>
/// Descrição clara e concisa do método/classe
/// </summary>
/// <param name="parametro">Descrição do parâmetro</param>
/// <returns>Descrição do retorno</returns>
/// <exception cref="ExceptionType">Quando a exceção é lançada</exception>
/// <example>
/// Exemplo de uso:
/// <code>
/// var resultado = Metodo("valor");
/// </code>
/// </example>
```

#### Testes Unitários
```csharp
/// <summary>
/// Testa se o método retorna sucesso quando recebe entrada válida
/// </summary>
[Fact(DisplayName = "Deve retornar sucesso quando entrada é válida")]
public async Task Metodo_QuandoEntradaValida_DeveRetornarSucesso()
{
    // Arrange
    var parametro = "valor válido";
    
    // Act
    var resultado = await Metodo(parametro);
    
    // Assert
    resultado.Success.Should().BeTrue();
}
```

---

## 🎯 Best Practices

### 1. **SOLID Principles**
- Single Responsibility Principle
- Open/Closed Principle
- Liskov Substitution Principle
- Interface Segregation Principle
- Dependency Inversion Principle

### 2. **Clean Code**
- Nomes descritivos
- Métodos pequenos e focados
- Baixo acoplamento
- Alta coesão
- DRY (Don't Repeat Yourself)

### 3. **Async/Await**
- Usar `async/await` para operações I/O
- Suportar `CancellationToken`
- Evitar blocking calls
- Tratar exceções em código assíncrono

### 4. **Tratamento de Erros**
- Criar exceções customizadas quando apropriado
- Documentar exceções que podem ser lançadas
- Não suprimir exceções sem justificativa
- Logging adequado de erros

### 5. **Performance**
- Reutilizar `HttpClient`
- Evitar alocações desnecessárias
- Usar `Span<T>` quando apropriado
- Considerar pool de objetos para objetos grandes

---

## 🔍 Code Review Checklist

Antes de aprovar qualquer mudança, verificar:

- [ ] Testes unitários criados/atualizados
- [ ] Cobertura de 100% mantida
- [ ] **Nomenclatura de testes clara e descritiva** (Metodo_Quando_Deve)
- [ ] **Todos os testes têm atributo `[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]`**
- [ ] Documentação XML completa e atualizada
- [ ] README.md atualizado (se necessário)
- [ ] Código segue SOLID principles
- [ ] Tratamento de erros adequado
- [ ] Performance considerada
- [ ] Sem código morto ou comentado
- [ ] Nomes claros e descritivos
- [ ] Compatibilidade multi-target mantida (.NET 5, 6, 7, 8)

---

## 🚀 Comandos Úteis

### Executar Testes
```bash
dotnet test
```

### Executar Testes com Cobertura
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Build do Projeto
```bash
dotnet build
```

### Criar Pacote NuGet
```bash
dotnet pack
```

### Verificar Código
```bash
dotnet format --verify-no-changes
```

---

## 📞 Suporte

Para questões sobre uso de agentes ou configuração do projeto:
- Consulte as instruções em `.github/copilot-instructions.md`
- Revise o `README.md` para contexto do projeto
- Verifique exemplos de código existentes

---

**Última atualização:** 2026-02-17
