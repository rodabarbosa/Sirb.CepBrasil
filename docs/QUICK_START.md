# 🚀 Quick Start - Implementação do Novo Fluxo v1.4.0

> Guia rápido para começar a implementação do fluxo de fallback

## ⚡ Em 5 Minutos

### 1. Entenda o Fluxo

O novo fluxo tenta buscar CEP em múltiplos serviços em sequência:

```
BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP
```

Se um encontra o CEP, retorna. Se todos falham, lança exceção. Se nenhum encontra, retorna null.

### 2. Conheça os Requisitos

- ✅ 100% de cobertura de testes
- ✅ xUnit + Assert nativo (sem FluentAssertions)
- ✅ `[Fact(DisplayName = "...")]` obrigatório
- ✅ 100% XML documentation em português
- ✅ .NET 8, 9, 10

### 3. Veja o Diagrama

Abra `.github/FALLBACK_DIAGRAMS.md` e veja os diagramas Mermaid do fluxo.

### 4. Leia a Especificação

Abra `.github/FALLBACK_IMPLEMENTATION.md` e entenda as classes.

### 5. Comece a Implementar

Use os agentes do GitHub Copilot (veja próxima seção).

---

## 🤖 Workflow com Agentes (Recomendado)

### Passo 1: Planejamento (10 min)

```bash
@plan Planejar implementação de fallback entre BrasilAPI, ViaCEP, AwesomeAPI e OpenCEP
```

### Passo 2: Testes (2-3 horas) - Fase RED

```bash
@tdd-red Criar testes para fallback entre múltiplos serviços de CEP

# Cenários a testar:
- BrasilAPI encontra → retorna sucesso
- BrasilAPI falha, ViaCEP encontra → retorna sucesso
- Todos falham → lança ServiceException
- Nenhum encontra → retorna null
- Respeita CancellationToken
```

### Passo 3: Implementação (2-3 horas) - Fase GREEN

```bash
@tdd-green Implementar fallback entre múltiplos serviços

# Classes a implementar:
- BrasilApiService
- AwesomeApiService
- OpenCepService
- CepServiceOrchestrator
```

### Passo 4: Refatoração (1-2 horas) - Fase REFACTOR

```bash
@tdd-refactor Refatorar implementação de fallback aplicando SOLID principles

# Focos:
- Extrair código comum
- Strategy Pattern
- Tratamento de erro robusto
```

### Passo 5: Documentação (1 hora)

```bash
@se-technical-writer Documentar novo fluxo e novos serviços

# Documentar:
- XML documentation completa
- README.md (já tem básico)
- Exemplos de uso
```

### Passo 6: Segurança (30 min)

```bash
@se-security-reviewer Revisar segurança da implementação de múltiplos serviços

# Validar:
- HTTPS em todas requisições
- Validação de entrada
- Tratamento de timeout
```

### Passo 7: Revisão Final (1 hora)

```bash
@principal-software-engineer Revisar implementação final de fallback

# Validar:
- 100% cobertura
- Performance
- Documentação
- Compatibilidade
```

---

## 📋 Classes a Implementar

### 1. BrasilApiService.cs (novo)

```csharp
// Namespace: Sirb.CepBrasil.Services
// Herança: ICepServiceControl
// Responsabilidade: Buscar CEP via BrasilAPI
// URL: https://brasilapi.com.br/api/cep/v1/{cep}
```

### 2. AwesomeApiService.cs (novo)

```csharp
// Namespace: Sirb.CepBrasil.Services
// Herança: ICepServiceControl
// Responsabilidade: Buscar CEP via AwesomeAPI
// URL: https://awesomeapi.com.br/api/cep/{cep}
```

### 3. OpenCepService.cs (novo)

```csharp
// Namespace: Sirb.CepBrasil.Services
// Herança: ICepServiceControl
// Responsabilidade: Buscar CEP via OpenCEP
// URL: https://cep.awesomeapi.com.br/json/{cep}
```

### 4. CepServiceOrchestrator.cs (novo)

```csharp
// Namespace: Sirb.CepBrasil.Services
// Herança: ICepService (PÚBLICA)
// Responsabilidade: Orquestar fallback entre serviços
// Método público: FindAsync(cep, token)
```

### 5. ViaCepService.cs (existente)

```csharp
// Pode manter como está
// Ou refatorar para extrair código comum
// URL: https://viacep.com.br/ws/{cep}/json
```

---

## 🧪 Exemplo de Teste

```csharp
/// <summary>
/// Testa se orquestrador tenta BrasilAPI primeiro
/// </summary>
[Fact(DisplayName = "Deve tentar BrasilAPI primeiro")]
public async Task FindAsync_DeveAtentarBrasilApiPrimeiro()
{
    // Arrange
    var mockBrasilApi = new Mock<ICepServiceControl>();
    var mockViaCep = new Mock<ICepServiceControl>();
    var mockAwesomeApi = new Mock<ICepServiceControl>();
    var mockOpenCep = new Mock<ICepServiceControl>();
    
    var cepContainer = new CepContainer { Cep = "01310-100" };
    mockBrasilApi.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(cepContainer);
    
    var orchestrator = new CepServiceOrchestrator(
        mockBrasilApi.Object,
        mockViaCep.Object,
        mockAwesomeApi.Object,
        mockOpenCep.Object);
    
    // Act
    var result = await orchestrator.FindAsync("01310100", CancellationToken.None);
    
    // Assert
    Assert.NotNull(result);
    Assert.True(result.Success);
    mockBrasilApi.Verify(x => x.FindAsync("01310100", It.IsAny<CancellationToken>()), Times.Once);
    mockViaCep.Verify(x => x.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
}
```

---

## 📚 Documentos de Referência

### Leitura Obrigatória

- [ ] `.github/DOCUMENTACAO_NOVO_FLUXO.md` - Visão geral e índice
- [ ] `.github/FALLBACK_DIAGRAMS.md` - Diagramas visuais
- [ ] `.github/FALLBACK_IMPLEMENTATION.md` - Especificação técnica
- [ ] `.github/copilot-instructions.md` - Instruções técnicas

### Leitura Recomendada

- [ ] `README.md` - Atualizado com novo fluxo
- [ ] `AGENTS.md` - Seção sobre implementação v1.4.0

---

## ✅ Checklist Rápido

### Antes de Começar

- [ ] Li toda documentação
- [ ] Entendi o fluxo
- [ ] Conheço as 4 classes a implementar
- [ ] Entendi os requisitos (100% testes, XML doc, Assert nativo)

### Durante a Implementação

- [ ] Criei testes que falham (RED)
- [ ] Implementei código mínimo (GREEN)
- [ ] Refatorei mantendo testes verdes (REFACTOR)
- [ ] Documentei com XML (100%)
- [ ] Validei segurança
- [ ] Verifico Build sem warnings
- [ ] Confiro 100% de cobertura

### Antes de Finalizar

- [ ] 100% cobertura de testes ✅
- [ ] DisplayName em TODOS os testes ✅
- [ ] XML documentation 100% ✅
- [ ] Assert nativo do xUnit (sem FluentAssertions) ✅
- [ ] Build sem warnings ✅
- [ ] Testes passando 100% ✅
- [ ] Compatível .NET 8, 9, 10 ✅
- [ ] Sem vulnerabilidades de segurança ✅
- [ ] README atualizado (já feito) ✅

---

## 🎯 Estrutura do Projeto Esperada

```
Sirb.CepBrasil/
├── Services/
│   ├── BrasilApiService.cs         ← novo
│   ├── AwesomeApiService.cs        ← novo
│   ├── OpenCepService.cs           ← novo
│   ├── CepServiceOrchestrator.cs   ← novo (PÚBLICO)
│   └── ViaCepService.cs            ← existente

Sirb.CepBrasil.Test/
└── Services/
    ├── BrasilApiServiceTest.cs     ← novo
    ├── AwesomeApiServiceTest.cs    ← novo
    ├── OpenCepServiceTest.cs       ← novo
    ├── CepServiceOrchestratorTest.cs ← novo
    └── ViaCepServiceTest.cs        ← existente
```

---

## 🚀 Dicas de Implementação

### ⚡ Use Mocks para Testes

```csharp
var mockService = new Mock<ICepServiceControl>();
mockService.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(new CepContainer { Cep = "01310-100" });
```

### 🔄 Padrão para Cada Serviço

1. Validar entrada
2. Fazer requisição HTTP
3. Parsear resposta
4. Retornar CepContainer ou null
5. Lançar ServiceException em erro

### 💾 Reutilize Código

Extraia método comum para fazer requisição HTTP em classe base.

### 🧪 Teste Todos os Cenários

- Sucesso
- Não encontrado
- Erro HTTP
- Timeout
- CancellationToken

---

## 🎓 Referências Rápidas

### Assert Nativo do xUnit

```csharp
Assert.True(condicao);
Assert.False(condicao);
Assert.Null(objeto);
Assert.NotNull(objeto);
Assert.Equal(esperado, atual);
Assert.NotEqual(naoEsperado, atual);
Assert.Empty(colecao);
Assert.NotEmpty(colecao);
Assert.Throws<Exception>(() => metodo());
await Assert.ThrowsAsync<Exception>(() => metodoAsync());
```

### XML Documentation Mínimo

```csharp
/// <summary>
/// O que faz
/// </summary>
/// <param name="parametro">Descrição</param>
/// <returns>O que retorna</returns>
/// <exception cref="Exception">Quando lança</exception>
```

### DisplayName Obrigatório

```csharp
[Fact(DisplayName = "Descrição clara em português")]
[Theory(DisplayName = "Descrição clara em português")]
```

---

## 📞 Próximas Ações

### Imediatamente

1. Leia `DOCUMENTACAO_NOVO_FLUXO.md`
2. Visualize `FALLBACK_DIAGRAMS.md`
3. Prepare-se para implementar

### Próximas 8 horas

1. Use @plan para planejar
2. Use @tdd-red para criar testes
3. Use @tdd-green para implementar
4. Use @tdd-refactor para melhorar
5. Use @se-technical-writer para documentar

---

## ✨ Você está Pronto!

Com esta documentação e os agentes especializados do GitHub Copilot, você tem tudo que precisa para implementar o novo fluxo de fallback da v1.4.0.

**Comece agora**: Leia `DOCUMENTACAO_NOVO_FLUXO.md` 👇

---

**Quick Start**: Este guia  
**Data**: 2026-02-18  
**Versão**: 1.4.0  
**Status**: ✅ Pronto para Implementação
