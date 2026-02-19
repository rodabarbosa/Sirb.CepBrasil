# 🚀 Próximos Passos - Melhorias ExceptionExtension.cs

## Resumo do Que Foi Feito

✅ **ExceptionExtension.cs** refatorado com:

- Documentação XML 100%
- 3 métodos (AllMessages, GetDetailedMessage, GetExceptionChain)
- 29 testes aprovados em net8/net9/net10
- SOLID Principles aplicados
- DisplayName em todos os testes
- Pronto para produção

---

## 📋 Próximas Ações Recomendadas

### 1. **Atualizar Outras Extensions** (2-3 horas)

#### Files a revisar:

```
✅ CepExtension.cs
✅ JsonExtension.cs
```

#### Aplicar mesmas melhorias:

- [ ] Documentação XML 100%
- [ ] Nomenclatura clara
- [ ] SOLID Principles
- [ ] Testes com DisplayName
- [ ] 100% de cobertura

---

### 2. **Implementar Novo Fluxo de Fallback** (6-8 horas)

Conforme documentado em `FLUXO-FALLBACK.md`:

#### Serviços a criar/modificar:

```
📝 Services/
  ├── BrasilApiService.cs (novo)
  ├── ViaCepService.cs (modificar)
  ├── AwesomeApiService.cs (novo)
  ├── OpenCepService.cs (novo)
  └── CepServiceOrchestrator.cs (novo - orquestra fallback)
```

#### Testes a criar:

```
📝 Test/Services/
  ├── BrasilApiServiceTest.cs (novo)
  ├── AwesomeApiServiceTest.cs (novo)
  ├── OpenCepServiceTest.cs (novo)
  └── CepServiceOrchestratorTest.cs (novo)
```

**Ordem de Tentativas:**

1. BrasilAPI
2. ViaCEP
3. AwesomeAPI
4. OpenCEP
5. ServiceException ou null

---

### 3. **Atualizar README.md** (1 hora)

#### Seções a adicionar:

```markdown
## 🆕 v1.4.0 - Fluxo de Fallback Inteligente

### Recursos
- Busca inteligente em múltiplos provedores
- Fallback automático entre BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP
- 100% de cobertura de testes
- Multi-target: .NET 8, 9, 10

### Como Usar
...código de exemplo...

### Fluxo de Busca
(diagrama ASCII ou Mermaid)
```

---

### 4. **Remover Serviço dos Correios** (30 min)

#### Remoção de:

- [ ] `CorreiosService.cs` (se existir)
- [ ] Referências em injeção de dependência
- [ ] Testes relacionados
- [ ] Documentação

---

### 5. **Remover FluentAssertions** (1 hora)

#### Verificar projeto:

```bash
# Procurar por FluentAssertions
grep -r "FluentAssertions" /home/rodbarbosa/Projetos/CepBrasil/
```

#### Se encontrado:

- [ ] Remover do .csproj
- [ ] Converter Assert.That() → Assert nativo
- [ ] Verificar all tests

---

### 6. **Corrigir Erros de Testes** (varia)

#### Erros conhecidos:

```
❌ Testes com nomenclatura antiga
❌ Testes sem DisplayName
❌ Testes com FluentAssertions
```

#### Ações:

- [ ] Auditar todos os testes
- [ ] Atualizar nomenclatura
- [ ] Adicionar DisplayName
- [ ] Substituir FluentAssertions

---

## 📊 Priorização

### Priority 1 - CRÍTICO (Hoje)

- [ ] Implementar novo fluxo de fallback
- [ ] Testes para novo fluxo (29+ testes)
- [ ] Remover serviço dos Correios
- [ ] Atualizar README.md

**Tempo estimado:** 8-10 horas

### Priority 2 - IMPORTANTE (Semana 1)

- [ ] Atualizar outras Extensions
- [ ] Remover FluentAssertions
- [ ] Corrigir testes com problemas
- [ ] Validação completa

**Tempo estimado:** 4-6 horas

### Priority 3 - MELHORIAS (Semana 2)

- [ ] Adicionar mais exemplos
- [ ] Otimizar performance
- [ ] Documentação estendida
- [ ] Casos de uso

**Tempo estimado:** 3-4 horas

---

## 🧪 Comando para Validar

```bash
# Build completo
cd /home/rodbarbosa/Projetos/CepBrasil
dotnet build

# Executar todos os testes
dotnet test

# Verificar cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info

# Verificar formatação
dotnet format --verify-no-changes
```

---

## 📚 Documentação de Referência

Criada neste workspace:

1. **MELHORIAS-EXCEPTIONEXTENSION.md**
    - Análise completa das melhorias
    - SOLID Principles
    - Exemplos práticos

2. **SUMARIO-FINAL-EXCEPTIONEXTENSION.md**
    - Status final
    - Métricas
    - Checklist

3. **FLUXO-FALLBACK.md** (anterior)
    - Novo fluxo de fallback
    - 4 serviços
    - Exemplos e testes

---

## 💡 Dicas para Próximas Melhorias

### Padrão para Novas Services

```csharp
/// <summary>
/// Busca CEP via [Serviço]
/// </summary>
public sealed class [Serviço]Service : ICepServiceControl
{
    private readonly HttpClient _httpClient;
    private const int TimeoutSeconds = 10;
    
    public [Serviço]Service(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cep, nameof(cep));
        
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSeconds));
            
            // Implementação...
        }
        catch (Exception ex)
        {
            // Logging e re-throw como ServiceException
            throw new ServiceException($"Erro ao buscar CEP em [Serviço]", ex);
        }
    }
}
```

### Padrão para Testes de Services

```csharp
public class [Serviço]ServiceTest
{
    [Fact(DisplayName = "Deve retornar CepContainer quando CEP é válido")]
    public async Task FindAsync_QuandoCepValido_DeveRetornarCepContainer() { }
    
    [Fact(DisplayName = "Deve retornar null quando CEP não é encontrado")]
    public async Task FindAsync_QuandoCepNaoEncontrado_DeveRetornarNull() { }
    
    [Fact(DisplayName = "Deve lançar ServiceException quando serviço falha")]
    public async Task FindAsync_QuandoServicoFalha_DeveLancarServiceException() { }
}
```

---

## 📞 Suporte

### Dúvidas sobre implementação:

- Consulte `FLUXO-FALLBACK.md` para detalhes do novo fluxo
- Consulte `MELHORIAS-EXCEPTIONEXTENSION.cs` para padrões de código
- Verifique testes existentes como referência

### Erros ao compilar:

1. Verificar se todas as Services estão registradas na DI
2. Verificar se interfaces são implementadas corretamente
3. Executar `dotnet clean` e `dotnet build`

---

## ✅ Checklist para Próximas Sessões

### Session 1 - Novo Fluxo de Fallback

- [ ] Criar BrasilApiService
- [ ] Criar AwesomeApiService
- [ ] Criar OpenCepService
- [ ] Criar CepServiceOrchestrator
- [ ] Modificar ViaCepService (se necessário)
- [ ] Criar testes para cada serviço (29+ testes)
- [ ] Testar fallback completo

### Session 2 - Limpeza e Documentação

- [ ] Remover serviço dos Correios
- [ ] Remover FluentAssertions
- [ ] Atualizar README.md com novo fluxo
- [ ] Atualizar outras Extensions
- [ ] Corrigir nomenclatura de testes antigos

### Session 3 - Validação

- [ ] Build sucesso em net8/net9/net10
- [ ] 100% testes passando
- [ ] Cobertura >= 85%
- [ ] Sem warnings
- [ ] Review final
- [ ] Pronto para release 1.4.0

---

## 🎯 Objetivo Final

**Versão 1.4.0 pronta para NuGet com:**
✅ Fluxo de fallback inteligente  
✅ 100% de cobertura de testes  
✅ Documentação XML 100%  
✅ Multi-target .NET 8/9/10  
✅ DisplayName em todos os testes  
✅ Sem FluentAssertions  
✅ Sem serviço dos Correios  
✅ README.md atualizado

---

**Status:** Pronto para começar próximas melhorias  
**Data:** 2026-02-18  
**Última atualização:** ExceptionExtension refatorado com sucesso
