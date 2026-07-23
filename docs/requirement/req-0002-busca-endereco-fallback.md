# req-0002 — Busca de Endereço com Fallback Multi-Provedor

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | req-0002                 |
| **Título**           | Busca de Endereço com Fallback Multi-Provedor |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |
| **Bundle**           | Não (requisito isolado)  |

---

## Histórico de Alterações

| Data       | Autor                 | Versão | Descrição                        |
|------------|-----------------------|--------|----------------------------------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0   | Criação do documento de requisito |

---

## 1. Introdução

### 1.1 Objetivo

Este documento especifica o requisito de **Busca de Endereço com Fallback Multi-Provedor** da biblioteca Sirb.CepBrasil. Esta é a funcionalidade central da biblioteca: consultar endereços brasileiros através de um CEP, utilizando uma estratégia inteligente de fallback automático entre 4 provedores públicos para garantir máxima disponibilidade e resiliência.

### 1.2 Escopo

**In scope:**

- Consulta de endereço brasileiro a partir de um CEP (8 dígitos numéricos, com ou sem máscara)
- Estratégia de fallback automático entre provedores na ordem: **BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP**
- Retorno do primeiro resultado bem-sucedido (não tentar provedores subsequentes)
- Tratamento de erros de serviço (timeout, HTTP error) com tentativa ao próximo provedor
- Consolidação de erros de todos os provedores em mensagem de erro no `CepResult`
- Retorno de resultado nulo quando CEP não é encontrado em nenhum provedor
- Suporte a `CancellationToken` com timeout padrão de 30 segundos por tentativa
- Reuso de `HttpClient` (gerenciamento interno ou via injeção de dependência)
- Implementação de `IDisposable` para liberação de recursos
- Documentação XML completa em inglês para todos os membros públicos

**Out of scope:**

- Cache local de CEPs consultados
- Logging interno (a ser considerado em versão futura)
- Métricas de tempo de resposta por provedor
- Retry com backoff exponencial
- Cache de resultados em memória
- Interface de usuário (biblioteca .NET, sem UI)
- Geolocalização ou coordenadas
- Consulta paralela a múltiplos provedores (sempre sequencial)
- Validação de existência de CEP (a validação de formato já é coberta pelo req-0001)

### 1.3 Glossário

| Termo | Definição |
|-------|-----------|
| CEP | Código de Endereçamento Postal brasileiro, composto por 8 dígitos numéricos |
| Fallback | Estratégia de tentar provedores alternativos quando o primário falha |
| Provedor | Serviço externo que retorna dados de endereço a partir de um CEP |
| `CepResult` | Classe que encapsula o resultado da busca (Success, CepContainer, Message, Exceptions) |
| `CepContainer` | Modelo imutável com os dados do endereço (Cep, Logradouro, Bairro, Cidade, Uf, Complemento, Ibge) |
| `ServiceException` | Exceção lançada para erros de serviço externo |
| `CancellationToken` | Mecanismo .NET para cancelamento cooperativo de operações assíncronas |
| Timeout | Tempo máximo de espera por uma resposta HTTP (padrão: 30s por tentativa) |

---

## 2. Intenção / Motivação de Negócio (Por quê)

### Problema de negócio real sendo resolvido

A biblioteca Sirb.CepBrasil precisa consultar endereços brasileiros a partir de CEPs para seus consumidores. Os provedores públicos disponíveis (BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP) são serviços gratuitos sem SLA, que podem ficar indisponíveis, lentos ou retornar erros a qualquer momento. Um único provedor como ponto único de falha tornaria a biblioteca frágil e não confiável.

### Por que importa (impacto mensurável)

- **Disponibilidade:** Com 4 provedores em cadeia de fallback, a probabilidade de todos estarem simultaneamente indisponíveis é extremamente baixa, garantindo disponibilidade próxima de 100% para os consumidores da biblioteca.
- **Latência:** Ao retornar no primeiro sucesso sem tentar provedores desnecessários, o tempo médio de resposta é o do provedor primário (tipicamente 100-500ms). Cenários de falha têm latência maior (até 120s no pior caso com 4 timeouts).
- **Resiliência:** A falha de qualquer provedor individual não impede a consulta — o sistema degrada graciosamente tentando alternativas.
- **Experiência do desenvolvedor:** API pública simples (`ICepService.FindAsync`) que abstrai toda a complexidade do fallback.

### Quem é afetado

- **Desenvolvedores clientes** da biblioteca que consomem `ICepService` para consultar CEPs.
- **Sistemas integrados** que dependem da biblioteca para obter dados de endereço.
- **Usuários finais** indiretamente — maior disponibilidade significa menos falhas em sistemas que usam a biblioteca.

### Critério de sucesso mensurável

- **100% dos provedores individuais são testados** com cobertura de testes unitários.
- **A cadeia de fallback completa é testada** em todos os cenários: sucesso no 1º, 2º, 3º, 4º provedor, falha total e não encontrado total.
- **Todas as exceções HTTP/timeout** são capturadas e tratadas sem propagação para o consumidor.
- **Tempo de resposta no cenário feliz** (BrasilAPI sucesso) não excede 500ms em 95% dos casos.

---

## 3. Descrição Geral

### 3.1 Missão da Funcionalidade

Prover uma interface pública simples e robusta para consulta de endereços brasileiros por CEP, com fallback automático entre 4 provedores, garantindo máxima disponibilidade sem exigir conhecimento do consumidor sobre a estratégia de fallback.

### 3.2 Atores

| Ator | Descrição |
|------|-----------|
| Sistema consumidor | Aplicação cliente que utiliza a biblioteca para consultar CEPs via `ICepService` |
| Desenvolvedor | Profissional que integra a biblioteca em um sistema, consumindo a API pública |

### 3.3 Contexto de Uso

A busca de endereço é o caso de uso principal da biblioteca. O fluxo típico é:

1. O consumidor instancia `CepService` (diretamente ou via DI)
2. Chama `FindAsync(cep, cancellationToken)`
3. O `CepService` valida o CEP (via `CepValidation`) e inicia o loop de fallback
4. Cada provedor é tentado em ordem: BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP
5. Ao primeiro sucesso, retorna `CepResult { Success = true }` com o `CepContainer`
6. Se todos falharem com erro, retorna `CepResult { Success = false }` com mensagem consolidada
7. Se nenhum encontrar (todos retornam null), retorna `null`

---

## 4. Artefatos Relacionados

### Documentos que impactam este artefato

- `docs/constituicao.md` (segurança, performance, qualidade, arquitetura)
- `docs/architecture-tech-stack.md` (stack .NET, provedores externos)
- `docs/system-mapping.md` (estrutura do sistema, fluxo de chamada)
- `docs/requirement/req-0001-validacao-cep.md` (validação de CEP usada internamente)
- `docs/system-risk-matrix.md` (matriz de risco global)
- `docs/extra/FLUXO-FALLBACK.md` (documento de fluxo de fallback original)
- `docs/extra/DIAGRAMAS-FALLBACK.md` (diagramas originais)
- `docs/extra/FALLBACK_DIAGRAMS.md` (diagramas adicionais)
- `docs/extra/FALLBACK_IMPLEMENTATION.md` (implementação original)
- `docs/extra/IMPLEMENTACAO-FALLBACK.md` (guia de implementação)
- `docs/extra/DOCUMENTACAO_NOVO_FLUXO.md` (documentação do novo fluxo)
- `docs/extra/MELHORIAS-CEPSERVICE.md` (melhorias do CepService)

### Documentos impactados por este artefato

- `docs/requirement/tec/tec-req-0002-busca-endereco-fallback.md`
- `docs/requirement/api/api-brasilapi.md`
- `docs/requirement/api/api-viacep.md`
- `docs/requirement/api/api-awesomeapi.md`
- `docs/requirement/api/api-opencep.md`
- `docs/diagrams/dcl-estrutura-servicos.md`
- `docs/diagrams/min-fluxo-fallback.md`
- `docs/requirement/apf/apf-req-0002.md`
- `docs/requirement/analises/analise-req-0002.md`
- `docs/requirement/checklist/checklist-req-0002.md`
- `docs/system-risk-matrix.md`
- `docs/system-mapping.md`
- `docs/architecture-tech-stack.md`

### Componentes técnicos relacionados

| Componente | Caminho | Função |
|------------|---------|--------|
| `ICepService` | `Sirb.CepBrasil/Interfaces/ICepService.cs` | Interface pública da biblioteca |
| `ICepServiceControl` | `Sirb.CepBrasil/Interfaces/ICepServiceControl.cs` | Interface interna para serviços provedores |
| `CepService` | `Sirb.CepBrasil/Services/CepService.cs` | Orquestrador público do fallback |
| `BaseService` | `Sirb.CepBrasil/Services/BaseService.cs` | Classe abstrata com operações HTTP comuns |
| `BrasilApiService` | `Sirb.CepBrasil/Services/BrasilApiService.cs` | Provedor BrasilAPI |
| `ViaCepService` | `Sirb.CepBrasil/Services/ViaCepService.cs` | Provedor ViaCEP |
| `AwesomeApiService` | `Sirb.CepBrasil/Services/AwesomeApiService.cs` | Provedor AwesomeAPI |
| `OpenCepService` | `Sirb.CepBrasil/Services/OpenCepService.cs` | Provedor OpenCEP |
| `CepContainer` | `Sirb.CepBrasil/Models/CepContainer.cs` | Modelo unificado de endereço |
| `CepResult` | `Sirb.CepBrasil/Models/CepResult.cs` | Container de resultado |
| `BrasilApiResponse` | `Sirb.CepBrasil/Models/BrasilApiResponse.cs` | DTO de resposta da BrasilAPI |
| `AwesomeApiResponse` | `Sirb.CepBrasil/Models/AwesomeApiResponse.cs` | DTO de resposta da AwesomeAPI |
| `ServiceException` | `Sirb.CepBrasil/Exceptions/ServiceException.cs` | Exceção para erros de serviço |
| `CepServiceOrchestratorTest` | `Sirb.CepBrasil.Test/Services/CepServiceOrchestratorTest.cs` | Testes do orquestrador |
| Testes individuais | `Sirb.CepBrasil.Test/Services/*ServiceTest.cs` | Testes de cada provedor |

---

## 5. Requisitos Funcionais

| ID | Descrição | Prioridade | Regra de negócio associada |
|----|-----------|------------|----------------------------|
| RF-08 | Buscar endereço com fallback automático entre 4 provedores | Alta | RN-011, RN-012 |
| RF-09 | Retornar resultado no primeiro sucesso sem tentar próximos | Alta | RN-012 |
| RF-10 | Consolidar erros de todos os provedores em caso de falha geral | Alta | RN-013 |
| RF-11 | Retornar null quando CEP não é encontrado em nenhum provedor | Alta | RN-014 |
| RF-12 | Prover construtor com gerenciamento interno de HttpClient | Alta | RN-015 |
| RF-13 | Prover construtor com injeção de HttpClient externo (DI) | Alta | RN-015, RN-016 |
| RF-14 | Implementar IDisposable para liberação de recursos | Média | RN-016 |
| RF-15 | Suportar CancellationToken com timeout padrão de 30s | Alta | RN-017 |
| RF-16 | Validar entrada do CEP antes de consultar provedores | Alta | RN-003 (herdado do req-0001) |

---

## 6. Regras de Negócio

| ID | Regra | Origem | Impacto |
|----|-------|--------|---------|
| RN-011 | A ordem de tentativa dos provedores deve ser: BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP | Decisão arquitetural (BrasilAPI é mais rápido/confiável) | Define a sequência do fallback |
| RN-012 | Ao obter o primeiro resultado bem-sucedido (CepContainer não nulo), retornar imediatamente sem tentar os demais provedores | Eficiência (evita chamadas desnecessárias) | Fluxo de fallback |
| RN-013 | Se todos os provedores retornarem erro (exceção HTTP/timeout), retornar CepResult com Success = false e mensagem consolidada de todos os erros | Resiliência (informa o consumidor sobre a falha geral) | Tratamento de erro |
| RN-014 | Se nenhum provedor encontrar o CEP (todos retornam null), retornar null ao consumidor | Semântica clara (diferencia "não encontrado" de "erro") | Comportamento de saída |
| RN-015 | O HttpClient pode ser gerenciado internamente (construtor sem parâmetros) ou injetado externamente (construtor com HttpClient) | Flexibilidade (suporte a DI e uso simples) | Design da API |
| RN-016 | HttpClient criado internamente deve ser descartado no Dispose; HttpClient injetado não deve ser descartado | Prevenção de ObjectDisposedException em cenários DI | Gerenciamento de ciclo de vida |
| RN-017 | Timeout padrão de 30 segundos por tentativa quando nenhum CancellationToken é fornecido; CancellationToken do usuário tem prioridade | Performance (evita espera indefinida) | Timeout |
| RN-018 | Toda exceção capturada durante a chamada a um provedor deve ser registrada na mensagem consolidada, mas não propagada | Resilência (falha de um provedor não quebra a operação) | Tratamento de erro |

---

## 7. Critérios de Aceitação (Gherkin)

```gherkin
Funcionalidade: Busca de Endereço com Fallback Multi-Provedor
  Como um sistema consumidor da biblioteca
  Quero consultar um CEP com fallback automático entre provedores
  Para obter o endereço com máxima disponibilidade

  Cenário: BrasilAPI retorna sucesso no primeiro CEP
    Dado que o CEP informado é "01310100"
    E que a BrasilAPI retorna dados de endereço válidos
    Quando o método FindAsync() é executado
    Então o resultado deve ter Success = true
    E o CepContainer deve conter os dados do endereço
    E nenhum outro provedor deve ser consultado

  Cenário: BrasilAPI falha, ViaCEP retorna sucesso
    Dado que o CEP informado é "01310100"
    E que a BrasilAPI lança uma exceção de timeout
    E que a ViaCEP retorna dados de endereço válidos
    Quando o método FindAsync() é executado
    Então o resultado deve ter Success = true
    E os dados devem ser provenientes da ViaCEP
    E os provedores AwesomeAPI e OpenCEP não devem ser consultados

  Cenário: BrasilAPI e ViaCEP falham, AwesomeAPI retorna sucesso
    Dado que o CEP informado é "01310100"
    E que a BrasilAPI lança uma exceção de serviço
    E que a ViaCEP retorna null (não encontrado)
    E que a AwesomeAPI retorna dados de endereço válidos
    Quando o método FindAsync() é executado
    Então o resultado deve ter Success = true
    E os dados devem ser provenientes da AwesomeAPI
    E o OpenCEP não deve ser consultado

  Cenário: BrasilAPI, ViaCEP e AwesomeAPI falham, OpenCEP retorna sucesso
    Dado que o CEP informado é "01310100"
    E que a BrasilAPI lança uma exceção de timeout
    E que a ViaCEP lança uma exceção HTTP 500
    E que a AwesomeAPI retorna null (não encontrado)
    E que a OpenCEP retorna dados de endereço válidos
    Quando o método FindAsync() é executado
    Então o resultado deve ter Success = true
    E os dados devem ser provenientes do OpenCEP

  Cenário: Todos os provedores falham com erro
    Dado que o CEP informado é "01310100"
    E que todos os 4 provedores lançam exceções
    Quando o método FindAsync() é executado
    Então o resultado deve ter Success = false
    E a mensagem de erro deve conter detalhes consolidados de todas as falhas

  Cenário: CEP não encontrado em nenhum provedor
    Dado que o CEP informado é "00000000"
    E que todos os 4 provedores retornam null
    Quando o método FindAsync() é executado
    Então o retorno deve ser null

  Cenário: Timeout no primeiro provedor, sucesso no segundo
    Dado que o CEP informado é "01310100"
    E que a BrasilAPI excede o timeout de 30 segundos
    E que a ViaCEP retorna dados de endereço válidos
    Quando o método FindAsync() é executado
    Então o resultado deve ter Success = true
    E a ViaCEP deve ter sido consultada

  Cenário: CancellationToken é acionado durante a busca
    Dado que o CEP informado é "01310100"
    E que um CancellationToken é acionado antes do término da primeira tentativa
    Quando o método FindAsync() é executado
    Então deve ser lançada OperationCanceledException
```

---

## 8. Requisitos Não Funcionais (NFRs)

| Categoria | Requisito | Métrica/Critério |
|-----------|-----------|------------------|
| Desempenho | Tempo de resposta no cenário feliz (BrasilAPI sucesso) | < 500ms em 95% das requisições |
| Desempenho | Timeout máximo por tentativa | 30 segundos (configurável via CancellationToken) |
| Confiabilidade | Fallback entre 4 provedores | 4 provedores em cadeia sequencial |
| Confiabilidade | Tratamento de erros HTTP/timeout | 100% das exceções capturadas sem propagação |
| Segurança | Todas as requisições devem usar HTTPS | URLs iniciando com https:// |
| Segurança | Validação de entrada antes de qualquer requisição | CEP validado via CepValidation antes de chamar provedores |
| Manutenibilidade | 100% de cobertura de testes | Cobertura medida via Coverlet |
| Manutenibilidade | 100% de documentação XML em inglês | Tags summary, param, returns, exception, example |
| Compatibilidade | Funcionar em .NET 8, 9 e 10 | Build multi-target no CI |
| Disponibilidade | HttpClient reutilizado | Mesma instância para todas as requisições |
| Performance | ConfigureAwait(false) em todos os awaits | Evita captura de SynchronizationContext |

---

## 9. Wireframe

**Não aplicável.** Projeto é uma biblioteca .NET sem interface de usuário. Nenhum wireframe ou padrão visual é necessário.

```
⏳ PENDENTE [Fase 3 — Design] — N/A: Biblioteca .NET sem interface de usuário.
```

---

## 10. Integração de API / Banco de Dados

Os 4 provedores externos de CEP (BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP) são consumidos via HTTP GET. A documentação detalhada de cada endpoint está nos artefatos específicos:

| Provedor | Documentação da API | Status |
|----------|---------------------|--------|
| BrasilAPI | `docs/requirement/api/api-brasilapi.md` | `⏳ PENDENTE [Fase 3 — Dados]` |
| ViaCEP | `docs/requirement/api/api-viacep.md` | `⏳ PENDENTE [Fase 3 — Dados]` |
| AwesomeAPI | `docs/requirement/api/api-awesomeapi.md` | `⏳ PENDENTE [Fase 3 — Dados]` |
| OpenCEP | `docs/requirement/api/api-opencep.md` | `⏳ PENDENTE [Fase 3 — Dados]` |

---

## 11. Matriz de Risco do Requisito (Recorte Local)

| ID | Requisito | Descrição do Risco | Probabilidade | Impacto | Score | Nível | Mitigação | Status |
|----|-----------|-------------------|---------------|---------|-------|-------|-----------|--------|
| RSK-001 | req-0002 | Dependência de APIs externas — provedores podem ficar indisponíveis | A (4) — Alta | A (4) — Alta | 16 | 🔴 Crítico | Estratégia de fallback entre 4 provedores; se um falha, próximo é tentado | 🟡 Mitigado parcialmente |
| RSK-002 | req-0002 | Mudança em formato de resposta dos provedores | M (3) — Média | A (4) — Alta | 12 | 🟠 Alto | DTOs separados por provedor; fallback tenta outros se um quebrar | 🟡 Mitigado parcialmente |
| RSK-003 | req-0002 | Rate limiting / bloqueio por IP | M (3) — Média | M (3) — Médio | 9 | 🟡 Médio | Fallback distribui carga entre provedores | 🟢 Aceito |
| RSK-004 | req-0002 | Timeout em serviços externos (latência acumulada até 120s) | A (4) — Alta | M (3) — Médio | 12 | 🟠 Alto | Timeout de 30s por tentativa via CancellationTokenSource | 🟡 Mitigado |
| RSK-009 | req-0002 | Dependência de terceiros sem SLA | M (3) — Média | MA (5) — Muito Alto | 15 | 🟠 Alto | Fallback com 4 provedores reduz risco de ponto único | 🟡 Mitigado parcialmente |
| RSK-013 | req-0002 | CepResult com Success=false pode ser confundido com erro de validação pelo consumidor | B (2) — Baixa | B (2) — Baixo | 4 | 🟢 Baixo | Documentação XML e exemplos esclarecem a semântica | 🟢 Aceito |
| RSK-014 | req-0002 | HttpClient não reutilizado causa esgotamento de sockets | B (2) — Baixa | M (3) — Médio | 6 | 🟡 Médio | HttpClient reutilizado via construtor com/sem DI | ✅ Mitigado |

---

## 12. Análise de Pontos de Função (APF) Vinculada

- **Documento APF:** `docs/requirement/apf/apf-req-0002.md`
- **Total estimado:** ver documento APF específico.

---

## 13. Impactos Técnicos

### Backend / Código

- `Sirb.CepBrasil/Interfaces/ICepService.cs` — interface pública existente
- `Sirb.CepBrasil/Interfaces/ICepServiceControl.cs` — interface interna existente
- `Sirb.CepBrasil/Services/BaseService.cs` — classe abstrata com HTTP comum
- `Sirb.CepBrasil/Services/CepService.cs` — orquestrador de fallback
- `Sirb.CepBrasil/Services/BrasilApiService.cs` — provedor BrasilAPI
- `Sirb.CepBrasil/Services/ViaCepService.cs` — provedor ViaCEP
- `Sirb.CepBrasil/Services/AwesomeApiService.cs` — provedor AwesomeAPI
- `Sirb.CepBrasil/Services/OpenCepService.cs` — provedor OpenCEP
- `Sirb.CepBrasil/Models/CepContainer.cs` — modelo de endereço
- `Sirb.CepBrasil/Models/CepResult.cs` — container de resultado
- `Sirb.CepBrasil/Models/BrasilApiResponse.cs` — DTO BrasilAPI
- `Sirb.CepBrasil/Models/AwesomeApiResponse.cs` — DTO AwesomeAPI

### Frontend

- N/A — biblioteca .NET sem UI

### Banco de Dados

- N/A — sem persistência

### Integrações Externas

| Provedor | URL Base | Protocolo |
|----------|----------|-----------|
| BrasilAPI | `https://brasilapi.com.br/api/cep/v1/{cep}` | HTTPS/GET |
| ViaCEP | `https://viacep.com.br/ws/{cep}/json` | HTTPS/GET |
| AwesomeAPI | `https://cep.awesomeapi.com.br/json/{cep}` | HTTPS/GET |
| OpenCEP | `https://opencep.com/v1/{cep}` | HTTPS/GET |

### Design / UX

- N/A — sem interface de usuário

---

## 14. Rastreabilidade

| Item | Referência |
|------|------------|
| Código — Interfaces | `Sirb.CepBrasil/Interfaces/ICepService.cs`, `ICepServiceControl.cs` |
| Código — Orquestrador | `Sirb.CepBrasil/Services/CepService.cs` |
| Código — Base service | `Sirb.CepBrasil/Services/BaseService.cs` |
| Código — BrasilAPI | `Sirb.CepBrasil/Services/BrasilApiService.cs` |
| Código — ViaCEP | `Sirb.CepBrasil/Services/ViaCepService.cs` |
| Código — AwesomeAPI | `Sirb.CepBrasil/Services/AwesomeApiService.cs` |
| Código — OpenCEP | `Sirb.CepBrasil/Services/OpenCepService.cs` |
| Código — Modelos | `Sirb.CepBrasil/Models/CepContainer.cs`, `CepResult.cs` |
| Código — DTOs | `Sirb.CepBrasil/Models/BrasilApiResponse.cs`, `AwesomeApiResponse.cs` |
| Código — Exceções | `Sirb.CepBrasil/Exceptions/ServiceException.cs` |
| Testes — Orquestrador | `Sirb.CepBrasil.Test/Services/CepServiceOrchestratorTest.cs` |
| Testes — Provedores | `Sirb.CepBrasil.Test/Services/*ServiceTest.cs` |
| Documento técnico | `docs/requirement/tec/tec-req-0002-busca-endereco-fallback.md` |
| APIs | `docs/requirement/api/api-brasilapi.md`, `api-viacep.md`, `api-awesomeapi.md`, `api-opencep.md` |
| Diagramas | `docs/diagrams/dcl-estrutura-servicos.md`, `min-fluxo-fallback.md` |
| APF | `docs/requirement/apf/apf-req-0002.md` |
| Análise cross-artifact | `docs/requirement/analises/analise-req-0002.md` |
| Quality Checklist | `docs/requirement/checklist/checklist-req-0002.md` |
| Matriz de risco global | `docs/system-risk-matrix.md` |
| Constituição | `docs/constituicao.md` |
| Documentos de apoio | `docs/extra/FLUXO-FALLBACK.md`, `docs/extra/FALLBACK_IMPLEMENTATION.md`, `docs/extra/DIAGRAMAS-FALLBACK.md`, `docs/extra/FALLBACK_DIAGRAMS.md`, `docs/extra/IMPLEMENTACAO-FALLBACK.md`, `docs/extra/DOCUMENTACAO_NOVO_FLUXO.md` |

---

## 15. Validações Realizadas para esta Documentação

- [x] README do projeto analisado
- [x] Código principal analisado (todas as classes de serviço, interfaces, modelos)
- [x] Testes unitários analisados (CepServiceOrchestratorTest, provedores)
- [x] Documentos de apoio consultados (FLUXO-FALLBACK, DIAGRAMAS-FALLBACK, FALLBACK_DIAGRAMS, FALLBACK_IMPLEMENTATION, IMPLEMENTACAO-FALLBACK, DOCUMENTACAO_NOVO_FLUXO, MELHORIAS-CEPSERVICE)
- [x] Constituição do projeto revisada
- [x] Arquitetura e tech stack revisados
- [x] Artefatos relacionados revisados (req-0001, tec-req-0001, system-mapping, system-risk-matrix)

---

## 16. Clarification Log

| Data | Pergunta | Resposta | Status (Resolvido/Pendente) | Origem (Criação/Implementação) | Impacto no requisito |
|------|----------|----------|----------------------------|-------------------------------|----------------------|
| 23/07/2026 | Qual nome de usuário deve constar no campo `Autor` e nas entradas de histórico deste documento? | Rodrigo Araujo Barbosa | Resolvido | Criação | Define autor do documento |
| 23/07/2026 | O comportamento quando todos os serviços falham é lançar ServiceException ou retornar CepResult com Success=false? | Retornar CepResult com Success=false e mensagem consolidada (não lançar exceção) | Resolvido | Criação | Alinha RN-013 com o código real (CepService.cs linha 124) |
| 23/07/2026 | A ordem de fallback documentada (BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP) corresponde à implementação real? | Sim, confirmado no método StartServices() do CepService.cs | Resolvido | Criação | RN-011 confirmada |
| 23/07/2026 | O retorno quando nenhum provedor encontra o CEP é null (CepContainer nulo) ou CepResult com Success=false? | O CepService retorna new CepResult(false, null, message) — NÃO retorna null | Resolvido | Criação | Alinha RN-014 com o código real (linha 124) |

### Cobertura do Clarification (8 áreas)

| # | Área | Status (Resolvido \| N/A) | Ref. entrada no Log | Justificativa (se N/A) |
|---|------|---------------------------|---------------------|------------------------|
| 1 | Atores e personas | Resolvido | 23/07/2026 — Autor definido | — |
| 2 | Fluxos principais e alternativos | Resolvido | 23/07/2026 — Ordem fallback confirmada | — |
| 3 | Exceções e erros | Resolvido | 23/07/2026 — Comportamento erro vs exceção | — |
| 4 | Integrações externas | Resolvido | 23/07/2026 — 4 provedores confirmados | — |
| 5 | Requisitos não-funcionais | Resolvido | Coberto na seção NFR (desempenho, segurança) | — |
| 6 | Dados e privacidade | N/A | — | Requisito não processa dados pessoais; apenas dados públicos de endereço |
| 7 | Validações e regras de negócio | Resolvido | Coberto nas RN-011 a RN-018 | — |
| 8 | Critérios de aceite e mensurabilidade | Resolvido | Coberto nos 8 cenários Gherkin | — |

**Cobertura atual:** 7/8 (Resolvido). **Meta:** 8/8.

---

## 17. Esclarecimentos Adicionais

| Data | Nota |
|------|------|
| 23/07/2026 | Documento criado com base no código-fonte v1.4.0 (Sirb.CepBrasil) e documentos de apoio em docs/extra |
| 23/07/2026 | O comportamento real do CepService.FindAsync difere da documentação legada: retorna CepResult(false, null, message) quando todos os serviços falham, NÃO lança ServiceException e NÃO retorna null |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
