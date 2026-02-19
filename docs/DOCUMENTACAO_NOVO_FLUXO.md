# 📚 Documentação do Novo Fluxo - Sirb.CepBrasil v1.4.0

## 🎯 Resumo Executivo

A versão 1.4.0 do **Sirb.CepBrasil** implementa uma estratégia robusta de fallback entre múltiplos provedores de serviços de CEP para aumentar a confiabilidade e disponibilidade da biblioteca.

**Versão**: 1.4.0  
**Data de Atualização**: 2026-02-18  
**Linguagem**: C# / .NET 8, 9, 10

---

## 📋 Documentos Principais

### 1. **README.md** (Atualizado)

- ✅ Características atualizadas com novo fluxo
- ✅ Novo diagrama de fluxo com fallback
- ✅ Estratégia de fallback documentada
- ✅ Comportamento por resultado detalhado
- ✅ Changelog com v1.4.0
- ✅ Links para todos os provedores

**Localização**: `/home/rodbarbosa/Projetos/CepBrasil/README.md`

### 2. **AGENTS.md** (Atualizado)

- ✅ Agentes especializados para desenvolvimento
- ✅ Workflow TDD recomendado
- ✅ Seção completa sobre implementação do novo fluxo (v1.4.0)
- ✅ Checklist de sucesso
- ✅ Exemplo de estrutura de testes

**Localização**: `/home/rodbarbosa/Projetos/CepBrasil/AGENTS.md`

### 3. **.github/copilot-instructions.md** (Atualizado)

- ✅ Contexto do projeto com novo fluxo
- ✅ Documentação detalhada do fallback
- ✅ Classes a implementar/modificar
- ✅ Fluxo de execução detalhado
- ✅ Testes esperados
- ✅ Requisitos de Assert nativo do xUnit (sem FluentAssertions)
- ✅ Estrutura de testes com DisplayName obrigatório

**Localização**: `/home/rodbarbosa/Projetos/CepBrasil/.github/copilot-instructions.md`

### 4. **.github/FALLBACK_IMPLEMENTATION.md** (Novo)

- ✅ Fluxo de execução detalhado
- ✅ Padrão de design (Strategy + Facade)
- ✅ Arquitetura de implementação
- ✅ Todas as classes a implementar
- ✅ Estratégia de testes completa
- ✅ Exemplos de fluxo para cada cenário
- ✅ Tratamento de erros
- ✅ Passo a passo de implementação (RED → GREEN → REFACTOR)
- ✅ Checklist de conclusão

**Localização**: `/home/rodbarbosa/Projetos/CepBrasil/.github/FALLBACK_IMPLEMENTATION.md`

### 5. **.github/FALLBACK_DIAGRAMS.md** (Novo)

- ✅ Diagrama Mermaid de fluxo principal com fallback
- ✅ Diagrama de sequência - Caso de sucesso
- ✅ Diagrama de sequência - Caso com fallback
- ✅ Diagrama de sequência - Falha total
- ✅ Arquitetura de classes (diagrama de classes)
- ✅ State transitions
- ✅ Matriz de teste completa
- ✅ Otimizações e considerações de performance

**Localização**: `/home/rodbarbosa/Projetos/CepBrasil/.github/FALLBACK_DIAGRAMS.md`

---

## 🔄 Novo Fluxo de Fallback

### Ordem de Tentativas

```
BrasilAPI (1º) → ViaCEP (2º) → AwesomeAPI (3º) → OpenCEP (4º)
```

### Comportamento por Resultado

| Resultado                        | Ação                          |
|----------------------------------|-------------------------------|
| ✓ Encontrado em qualquer serviço | Retorna sucesso imediatamente |
| ❌ Não encontrado                 | Tenta próximo serviço         |
| ⏱️ Erro/Timeout                  | Tenta próximo serviço         |
| 🚨 Erro em todos os 4 serviços   | Lança `ServiceException`      |
| 📭 Não encontrado em nenhum      | Retorna `null`                |

---

## 🏗️ Arquitetura

### Padrão de Design

- **Strategy Pattern**: Cada provedor é uma estratégia
- **Facade Pattern**: Orquestrador encapsula complexidade

### Classes a Implementar

1. **BrasilApiService** (novo) - Implementa ICepServiceControl
2. **AwesomeApiService** (novo) - Implementa ICepServiceControl
3. **OpenCepService** (novo) - Implementa ICepServiceControl
4. **CepServiceOrchestrator** (novo) - Implementa ICepService (Pública)
5. **ViaCepService** (existente) - Pode ser refatorado

---

## 🧪 Testes

### Requisitos

- ✅ 100% de cobertura de testes
- ✅ xUnit como framework
- ✅ **Assert nativo do xUnit** (SEM FluentAssertions)
- ✅ Nomenclatura: `MetodoTestado_Condicao_ResultadoEsperado`
- ✅ **`[Fact(DisplayName = "...")]` obrigatório** em TODOS os testes

### Exemplo de Teste

```csharp
/// <summary>
/// Testa se BrasilAPI retorna sucesso quando CEP é encontrado
/// </summary>
[Fact(DisplayName = "Deve retornar CepContainer quando BrasilAPI encontra o CEP")]
public async Task FindAsync_QuandoBrasilAPIEncontra_DeveRetornarCepContainer()
{
    // Arrange
    var service = new BrasilApiService(_httpClient);
    var cep = "01310100";

    // Act
    var result = await service.FindAsync(cep, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("01310-100", result.Cep);
}
```

---

## 📝 Documentação

### Requisitos

- ✅ 100% XML documentation em português
- ✅ Toda classe, método, propriedade pública
- ✅ Tags: `<summary>`, `<param>`, `<returns>`, `<exception>`, `<example>`

### Exemplo

```csharp
/// <summary>
/// Busca informações de endereço via BrasilAPI
/// </summary>
/// <param name="cep">CEP formatado ou não (8 dígitos)</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>CepContainer se encontrado, null se não encontrado</returns>
/// <exception cref="ServiceException">Se houver erro na requisição</exception>
public async Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken)
{
    // Implementação
}
```

---

## ✅ Checklist de Implementação

### Fase 1: Planejamento

- [ ] Revisar toda a documentação
- [ ] Entender o fluxo completo
- [ ] Planejar a implementação

### Fase 2: Testes (RED)

- [ ] Criar testes para BrasilApiService
- [ ] Criar testes para AwesomeApiService
- [ ] Criar testes para OpenCepService
- [ ] Criar testes para CepServiceOrchestrator
- [ ] Todos os testes devem falhar inicialmente

### Fase 3: Implementação (GREEN)

- [ ] Implementar BrasilApiService
- [ ] Implementar AwesomeApiService
- [ ] Implementar OpenCepService
- [ ] Implementar CepServiceOrchestrator
- [ ] Todos os testes devem passar

### Fase 4: Refatoração (REFACTOR)

- [ ] Extrair código comum
- [ ] Aplicar SOLID principles
- [ ] Otimizar tratamento de erro
- [ ] Melhorar código

### Fase 5: Documentação

- [ ] Criar XML documentation completa
- [ ] Atualizar README.md (já feito)
- [ ] Adicionar exemplos de uso
- [ ] Documentar exceções

### Fase 6: Validação

- [ ] Verificar 100% de cobertura
- [ ] Validar segurança (HTTPS, validação)
- [ ] Testar com CancellationToken
- [ ] Build sem warnings
- [ ] Compatibilidade .NET 8, 9, 10

---

## 📚 Recursos de Implementação

### Documentos de Referência

1. **FALLBACK_IMPLEMENTATION.md** - Detalhes técnicos completos
2. **FALLBACK_DIAGRAMS.md** - Diagramas Mermaid do fluxo
3. **copilot-instructions.md** - Instruções técnicas detalhadas
4. **AGENTS.md** - Como usar agentes especializados

### Provedores de CEP

- [BrasilAPI](https://brasilapi.com.br/)
- [ViaCEP](https://viacep.com.br/)
- [AwesomeAPI](https://awesomeapi.com.br/)
- [OpenCEP](https://github.com/filipedeschamps/cep-promise)

### Padrões de Design

- [Strategy Pattern](https://refactoring.guru/design-patterns/strategy)
- [Facade Pattern](https://refactoring.guru/design-patterns/facade)
- [xUnit Documentation](https://xunit.net/)

---

## 🚀 Próximos Passos

### Recomendação de Workflow

```
1. Ler toda documentação (15 min)
   ↓
2. Usar @plan para planejar (10 min)
   ↓
3. Usar @tdd-red para criar testes (2-3 horas)
   ↓
4. Usar @tdd-green para implementar (2-3 horas)
   ↓
5. Usar @tdd-refactor para melhorar (1-2 horas)
   ↓
6. Usar @se-technical-writer para documentar (1 hora)
   ↓
7. Usar @se-security-reviewer para validar segurança (30 min)
   ↓
8. Usar @principal-software-engineer para revisão final (1 hora)
```

### Agentes Recomendados

- **@tdd-red** - Criar testes que falham
- **@tdd-green** - Implementar código mínimo
- **@tdd-refactor** - Melhorar qualidade
- **@se-technical-writer** - Documentar
- **@se-security-reviewer** - Revisar segurança
- **@CSharpExpert** - Suporte técnico

---

## 📊 Métricas de Sucesso

✅ **Implementação Completa Quando**:

- 100% de cobertura de testes
- Todos os testes com DisplayName descritivo
- XML documentation 100% em português
- README.md atualizado
- Compatibilidade .NET 8, 9, 10
- Sem vulnerabilidades de segurança
- Build sem warnings
- Testes passando 100%
- Performance validada

---

## 🎓 Princípios Aplicados

### SOLID

- ✅ **S**ingle Responsibility: Cada serviço tem uma responsabilidade
- ✅ **O**pen/Closed: Fácil adicionar novos serviços
- ✅ **L**iskov Substitution: Todos implementam ICepServiceControl
- ✅ **I**nterface Segregation: Interfaces específicas
- ✅ **D**ependency Inversion: Depende de abstrações

### Clean Code

- ✅ Nomes claros e descritivos
- ✅ Métodos pequenos e focados
- ✅ Sem comentários óbvios
- ✅ DRY (Don't Repeat Yourself)
- ✅ KISS (Keep It Simple, Stupid)

### Best Practices

- ✅ 100% de cobertura de testes
- ✅ 100% de documentação XML
- ✅ Async/await com CancellationToken
- ✅ Tratamento robusto de erros
- ✅ HTTPS para todas as requisições
- ✅ Validação de entrada

---

## 💬 Dúvidas Frequentes

### P: Por que 4 serviços?

**R**: Aumenta a confiabilidade. Se um falhar, há 3 alternativas. Estatisticamente improvável que todos falhem simultaneamente.

### P: O que acontece se usuário cancela (CancellationToken)?

**R**: Todas as tentativas são canceladas imediatamente. Nenhuma requisição é iniciada após cancelamento.

### P: Qual é o timeout?

**R**: 30 segundos total de timeout padrão. Cada serviço tem aproximadamente 5 segundos.

### P: Por que retorna null e não exceção quando não encontrado?

**R**: CEP existente mas sem dados é diferente de erro de serviço. null indica "não encontrado", exceção indica "erro".

### P: Como adicionar novo serviço no futuro?

**R**: Criar novo serviço implementando `ICepServiceControl`, adicionar ao `CepServiceOrchestrator`, criar testes. Simples!

---

## 📞 Suporte

Para questões sobre implementação:

1. Consulte os documentos listados acima
2. Revise os diagramas no FALLBACK_DIAGRAMS.md
3. Analise exemplos no FALLBACK_IMPLEMENTATION.md
4. Use agentes especializados do AGENTS.md

---

**Documentação Criada em**: 2026-02-18  
**Versão do Projeto**: 1.4.0  
**Status**: ✅ Pronto para Implementação
