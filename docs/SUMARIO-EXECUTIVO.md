# Sumário Executivo - Novo Fluxo de Fallback v1.4.0

## 📌 Resumo Executivo

A biblioteca **Sirb.CepBrasil** implementará uma **estratégia robusta de busca com fallback automático** entre 4 provedores de CEP públicos, garantindo alta disponibilidade, resiliência e melhor experiência do usuário.

---

## 🎯 Objetivo Principal

Permitir que a biblioteca funcione de forma confiável mesmo quando um ou mais serviços de CEP estão indisponíveis, fornecendo um mecanismo automático de fallback transparente ao usuário.

---

## 🔄 Estratégia de Fallback

### Ordem de Tentativas

1. **BrasilAPI** (https://brasilapi.com.br/) - Principal, melhor performance
2. **ViaCEP** (https://viacep.com.br/) - Fallback primário, mais antigo e confiável
3. **AwesomeAPI** (https://awesomeapi.com.br/) - Fallback secundário
4. **OpenCEP** (https://github.com/filipedeschamps/cep-promise) - Fallback final

### Comportamento por Resultado

| Resultado                          | Ação                                      |
|------------------------------------|-------------------------------------------|
| **Encontrado em qualquer serviço** | Retorna imediatamente (não tenta próximo) |
| **CEP não encontrado**             | Tenta o próximo serviço                   |
| **Erro no serviço**                | Tenta o próximo serviço                   |
| **Erro em todos os 4 serviços**    | Lança `ServiceException` com detalhes     |
| **Não encontrado em nenhum**       | Retorna `null`                            |

---

## 📊 Impacto e Benefícios

### Disponibilidade

- ✅ De 99.9% para ~99.99% (com 4 provedores)
- ✅ Falha apenas se todos os 4 serviços estiverem down simultânea/sequencialmente

### Resiliência

- ✅ Recuperação automática de falhas temporárias
- ✅ Balanceamento natural de carga (primeira tentativa bem-sucedida economiza requisições)
- ✅ Suporte a CancellationToken e timeouts

### Experiência do Usuário

- ✅ Transparência total (fallback automático)
- ✅ Mensagens de erro claras em caso de falha total
- ✅ Tempo de resposta típico: 100-500ms

### Segurança

- ✅ Validação rigorosa de entrada
- ✅ Sanitização de resposta
- ✅ Logging sem dados sensíveis
- ✅ Tratamento seguro de exceções

---

## 🏗️ Arquitetura

### Componentes Principais

```
┌─────────────────────────────────────────────────┐
│         CepServiceOrchestrator                  │
│      (Gerencia fallback automático)             │
└──────────┬──────────────────────────────────────┘
           │
    ┌──────┴──────┬──────────┬──────────┐
    │             │          │          │
    ▼             ▼          ▼          ▼
BrasilAPI    ViaCEP    AwesomeAPI   OpenCEP
Service      Service    Service     Service
```

### Classes a Implementar

1. **BrasilApiService** (NOVO)
    - Implementa busca via BrasilAPI
    - Herda de `ICepServiceControl`

2. **AwesomeApiService** (NOVO)
    - Implementa busca via AwesomeAPI
    - Herda de `ICepServiceControl`

3. **OpenCepService** (NOVO)
    - Implementa busca via OpenCEP
    - Herda de `ICepServiceControl`

4. **CepServiceOrchestrator** (NOVO)
    - Orquestra o fallback entre serviços
    - Implementa `ICepService` (interface pública)
    - Responsável pela estratégia de tentativas

5. **ViaCepService** (EXISTENTE)
    - Pode ser refatorado para reutilizar código comum
    - Mantém compatibilidade com versão anterior

---

## 🧪 Testes - 100% Cobertura Obrigatória

### Testes por Serviço

Cada um dos 4 serviços deve ter:

- ✅ Teste de sucesso (CEP encontrado)
- ✅ Teste de não encontrado (retorna null)
- ✅ Teste de erro HTTP
- ✅ Teste de timeout
- ✅ Teste de CancellationToken

### Testes do Orquestrador

Deve ter:

- ✅ Teste usando primeiro serviço se bem-sucedido
- ✅ Teste pulando para próximo se primeiro falhar
- ✅ Teste em sequência até encontrar
- ✅ Teste retornando null se nenhum encontrar
- ✅ Teste lançando exceção se todos falharem
- ✅ Teste de timeout total
- ✅ Teste de cancelamento via CancellationToken

### Framework e Padrões

- 🧪 **Framework:** xUnit
- ✅ **Assertions:** xUnit nativo (sem FluentAssertions)
- 📝 **Nomenclatura:** `Metodo_Quando_Deve`
- 🏷️ **DisplayName:** Obrigatório em TODOS os testes
- 🏗️ **Estrutura:** AAA (Arrange-Act-Assert)

---

## 📝 Documentação XML - 100% Obrigatória

Todos os métodos públicos devem ter:

```csharp
/// <summary>Descrição clara do que faz</summary>
/// <param name="...">Descrição de cada parâmetro</param>
/// <returns>Descrição do retorno</returns>
/// <exception cref="...">Exceção que pode ser lançada</exception>
/// <example>Exemplo de uso prático</example>
```

---

## ⏱️ Timeline e Timeouts

| Etapa                                | Tempo     |
|--------------------------------------|-----------|
| Validação de CEP                     | ~1ms      |
| Uma tentativa (timeout)              | 30s       |
| 4 tentativas sequenciais (pior caso) | ~120s     |
| Tempo típico (BrasilAPI sucesso)     | 100-500ms |

---

## 🔒 Comportamento Esperado

### Caso 1: CEP Válido Encontrado ✅

```
Entrada: "01310100"
↓
BrasilAPI: Encontra em 150ms
↓
Saída: CepResult { 
  Success: true,
  CepContainer: { Cep: "01310-100", ... }
}
Tempo total: 150ms
```

### Caso 2: BrasilAPI Falha, ViaCEP Encontra ✅

```
Entrada: "01310100"
↓
BrasilAPI: Timeout (30s)
ViaCEP: Encontra em 200ms
↓
Saída: CepResult { 
  Success: true,
  CepContainer: { Cep: "01310-100", ... }
}
Tempo total: 30.2s
```

### Caso 3: CEP Não Existe ℹ️

```
Entrada: "00000000"
↓
BrasilAPI: Não encontrado
ViaCEP: Não encontrado
AwesomeAPI: Não encontrado
OpenCEP: Não encontrado
↓
Saída: null
Tempo total: 300-500ms
```

### Caso 4: Todos os Serviços Down ❌

```
Entrada: "01310100"
↓
BrasilAPI: Erro (503)
ViaCEP: Erro (Timeout)
AwesomeAPI: Erro (503)
OpenCEP: Erro (Timeout)
↓
Saída: ServiceException
"Todos os serviços de CEP estão indisponíveis no momento"
Tempo total: ~120s
```

---

## 📋 Checklist de Entrega

### Código

- [ ] BrasilApiService.cs implementado
- [ ] AwesomeApiService.cs implementado
- [ ] OpenCepService.cs implementado
- [ ] CepServiceOrchestrator.cs implementado
- [ ] ViaCepService refatorado (se necessário)

### Testes

- [ ] 100% de cobertura para cada serviço
- [ ] 100% de cobertura para orquestrador
- [ ] Todos os testes com DisplayName obrigatório
- [ ] Nomenclatura padrão aplicada

### Documentação

- [ ] XML documentation em todos os métodos públicos
- [ ] README.md atualizado
- [ ] FLUXO-FALLBACK.md criado
- [ ] IMPLEMENTACAO-FALLBACK.md criado
- [ ] DIAGRAMAS-FALLBACK.md criado

### Qualidade

- [ ] `dotnet build` sem warnings
- [ ] `dotnet test` com 100% de sucesso
- [ ] Compatibilidade com .NET 8, 9, 10
- [ ] Código segue SOLID principles

---

## 📚 Documentação Criada

### Arquivo 1: FLUXO-FALLBACK.md

- Visão geral do fluxo
- Tabela de comportamento
- Exemplos de fluxo por cenário
- Arquitetura técnica
- Estratégia de testes
- Comportamento esperado

### Arquivo 2: IMPLEMENTACAO-FALLBACK.md

- Instruções técnicas detalhadas
- Código de exemplo para cada classe
- Padrões de DI Container
- Testes unitários completos
- Checklist de implementação
- Referências técnicas com URLs e exemplos de resposta

### Arquivo 3: DIAGRAMAS-FALLBACK.md

- Diagrama de sequência (Mermaid)
- Diagrama de decisão
- Diagrama de estados (máquina de estados)
- Diagrama de dependências e DI
- Fluxo de erro e exceções
- Timeline de timeout
- Tratamento de segurança
- Tabela de resposta por cenário

---

## 🚀 Próximos Passos

1. **Revisar Documentação**
    - Ler os 3 arquivos criados
    - Validar entendimento do fluxo
    - Esclarecer dúvidas

2. **Preparação**
    - Configurar ambiente de desenvolvimento
    - Instalar dependências necessárias

3. **Implementação**
    - Seguir guia de IMPLEMENTACAO-FALLBACK.md
    - Implementar em ordem: serviços → orquestrador → testes

4. **Validação**
    - Executar testes
    - Validar cobertura em 100%
    - Build sem warnings

5. **Entrega**
    - Atualizar README.md com novo fluxo
    - Criar pull request com toda documentação
    - Revisar e aprovar

---

## 📞 Referências Rápidas

### URLs das APIs

| Serviço    | URL                                           |
|------------|-----------------------------------------------|
| BrasilAPI  | https://brasilapi.com.br/api/address/v2/{cep} |
| ViaCEP     | https://viacep.com.br/ws/{cep}/json           |
| AwesomeAPI | https://awesomeapi.com.br/api/cep/{cep}       |
| OpenCEP    | https://cep.dev/{cep}                         |

### Exceções

- `ArgumentNullException` - CEP é null
- `ArgumentException` - CEP em formato inválido
- `ServiceException` - Todos os serviços falharam
- `OperationCanceledException` - CancellationToken foi acionado
- `HttpRequestException` - Erro HTTP genérico (capturado internamente)

### Timeouts

- Por tentativa: 30 segundos
- Total máximo: ~120 segundos
- Timeout esperado: 100-500ms (sucesso)

---

## ✅ Status

| Item                        | Status       |
|-----------------------------|--------------|
| Documentação                | ✅ Completa   |
| Diagramas                   | ✅ Criados    |
| Instruções de Implementação | ✅ Detalhadas |
| Pronto para Desenvolvimento | ✅ Sim        |

---

**Versão:** 1.4.0  
**Data:** 2026-02-18  
**Hora:** Conforme contexto  
**Documentação Completa e Pronta para Implementação** ✅
