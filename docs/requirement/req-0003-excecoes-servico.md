# req-0003 — Tratamento de Exceções de Serviço

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | req-0003                 |
| **Título**           | Tratamento de Exceções de Serviço |
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

Este documento especifica o requisito de **Tratamento de Exceções de Serviço** da biblioteca Sirb.CepBrasil. Define as exceções personalizadas do domínio — `NotFoundException` e `ServiceException` — bem como os métodos de extensão para formatação e consulta de mensagens de erro (`ExceptionExtension`), garantindo que falhas de serviço, CEPs não encontrados e argumentos inválidos sejam diferenciados de forma clara e tratados consistentemente em toda a biblioteca.

### 1.2 Escopo

**In scope:**

- `NotFoundException` — exceção específica para indicar que um CEP não foi encontrado em nenhum provedor
- `ServiceException` — exceção para erros de comunicação com provedores externos (HTTP, timeout, desserialização)
- `ExceptionExtension.AllMessages()` — concatenação de mensagens da cadeia de exceções com separador " → "
- `ExceptionExtension.GetDetailedMessage()` — formatação detalhada com tipo da exceção e stack trace opcional
- `ExceptionExtension.GetExceptionChain()` — iteração lazy pela cadeia de exceções internas
- Integração das exceções no `CepService` (captura e consolidação no `CepResult`)
- Integração das exceções no `BaseService` (lançamento de `ServiceException` para erros HTTP e desserialização)
- Método `ThrowIf` estático em ambas as exceções para validação condicional concisa
- Uso de `NotFoundException` no `CepService` para diferenciar "não encontrado" de "erro de serviço"

**Out of scope:**

- Logging interno (a ser considerado em versão futura)
- Retry com backoff exponencial
- Cache de exceções ou resultados
- Exceções de infraestrutura do .NET (exceto quando capturadas e relançadas como exceções de domínio)
- Interface de usuário (biblioteca .NET, sem UI)
- Internacionalização de mensagens de erro
- Métricas de frequência de exceções por provedor

### 1.3 Glossário

| Termo | Definição |
|-------|-----------|
| `NotFoundException` | Exceção de domínio que indica que o CEP não foi localizado em nenhum provedor |
| `ServiceException` | Exceção de domínio que indica falha na comunicação com provedor externo |
| `ExceptionExtension` | Classe estática com métodos de extensão para `System.Exception` |
| `AllMessages()` | Método de extensão que retorna todas as mensagens da cadeia de exceções concatenadas |
| `GetDetailedMessage()` | Método de extensão que retorna mensagem formatada com tipo e stack trace opcional |
| Cadeia de exceções | Sequência de exceções ligadas via `InnerException` |
| `ThrowIf` | Método estático para lançamento condicional de exceção |
| Fail-fast | Princípio de falhar imediatamente em entradas inválidas, sem processamento desnecessário |

---

## 2. Intenção / Motivação de Negócio (Por quê)

### Problema de negócio real sendo resolvido

A biblioteca Sirb.CepBrasil consulta 4 provedores externos que podem falhar de diferentes formas: timeout, HTTP 500, HTTP 404, resposta vazia, JSON malformado. Cada tipo de falha tem significado diferente — um timeout no BrasilAPI não é o mesmo que um CEP inexistente no ViaCEP. Sem uma hierarquia clara de exceções, o consumidor da biblioteca recebe exceções genéricas .NET (`Exception`, `HttpRequestException`, `TaskCanceledException`) sem conseguir distinguir semanticamente cada cenário de falha.

### Por que importa (impacto mensurável)

- **Clareza semântica:** o consumidor pode tratar cada cenário de forma específica — `NotFoundException` para CEP não encontrado (retornar 404 HTTP), `ServiceException` para erro de provedor (retornar 503), `ArgumentNullException` para argumento inválido (retornar 400).
- **Depuração:** `AllMessages()` consolida a cadeia completa de exceções em uma única string legível, eliminando a necessidade de navegar manualmente por `InnerException`.
- **Diagnóstico:** `GetDetailedMessage()` com stack trace permite logging estruturado em produção sem expor detalhes internos ao cliente.
- **Consistência:** todos os provedores usam o mesmo padrão de exceções (`ServiceException.ThrowIf`), garantindo comportamento uniforme independente do provedor que falhou.

### Quem é afetado

- **Desenvolvedores clientes** da biblioteca que precisam tratar erros de consulta de CEP de forma diferenciada.
- **Sistemas integrados** que dependem da biblioteca e precisam mapear exceções para códigos HTTP ou respostas de API.
- **Equipe de operações** que analisa logs de produção para diagnosticar falhas em provedores específicos.

### Critério de sucesso mensurável

- **3 exceções de domínio claramente diferenciadas:** `NotFoundException` (não encontrado), `ServiceException` (erro de serviço), `ArgumentNullException` (argumento inválido).
- **100% dos cenários de erro** nos provedores capturam e utilizam as exceções de domínio corretas.
- **100% de cobertura de testes** nas classes `NotFoundException`, `ServiceException` e `ExceptionExtension`.
- **Mensagens de erro consolidadas** via `AllMessages()` em todo ponto de captura de exceções no `CepService`.

---

## 3. Descrição Geral

### 3.1 Missão da Funcionalidade

Prover um sistema de exceções de domínio claro e consistente que permita ao consumidor da biblioteca distinguir semanticamente entre "CEP não encontrado", "erro de serviço externo" e "argumento inválido", com métodos utilitários para formatação e consulta da cadeia de exceções.

### 3.2 Atores

| Ator | Descrição |
|------|-----------|
| Sistema consumidor | Aplicação cliente que utiliza a biblioteca e precisa tratar exceções de forma diferenciada |
| Desenvolvedor | Profissional que integra a biblioteca e depende das exceções para fluxo de erro |
| Equipe de operações | Responsável por monitorar logs e diagnosticar falhas em produção |

### 3.3 Contexto de Uso

As exceções de domínio são acionadas nos seguintes contextos:

1. **Validação de entrada:** `CepValidation.Validate()` lança `ArgumentNullException` (nulo/vazio) ou `ServiceException` (formato inválido)
2. **Requisição HTTP:** `BaseService.ExecuteRequestAsync()` lança `ServiceException` quando o status HTTP não é sucesso
3. **Desserialização:** `BaseService.GetResponseAsync()` lança `ServiceException` quando a resposta é nula ou não pode ser desserializada
4. **Fallback:** `CepService.FindAsync()` captura todas as exceções dos provedores via `catch (Exception e)` e as consolida no `CepResult`
5. **CEP não encontrado:** `NotFoundException` é usada para sinalizar que o CEP não foi localizado (diferente de erro de serviço)

---

## 4. Artefatos Relacionados

### Documentos que impactam este artefato

- `docs/constituicao.md` (qualidade, segurança, tratamento de erros)
- `docs/architecture-tech-stack.md` (stack .NET, padrões de exceção)
- `docs/system-mapping.md` (estrutura do sistema)
- `docs/requirement/req-0001-validacao-cep.md` (validação de CEP — `ServiceException` usada na validação)
- `docs/requirement/req-0002-busca-endereco-fallback.md` (fallback — exceções capturadas no `CepService`)
- `docs/extra/MELHORIAS-EXCEPTIONEXTENSION.md` (documento de melhorias do ExceptionExtension)
- `docs/extra/SUMARIO-FINAL-EXCEPTIONEXTENSION.md` (resumo final das melhorias)
- `docs/system-risk-matrix.md` (matriz de risco global)

### Documentos impactados por este artefato

- `docs/requirement/tec/tec-req-0003-excecoes-servico.md`
- `docs/requirement/apf/apf-req-0003.md`
- `docs/requirement/analises/analise-req-0003.md`
- `docs/requirement/checklist/checklist-req-0003.md`
- `docs/system-risk-matrix.md`

### Componentes técnicos relacionados

| Componente | Caminho | Função |
|------------|---------|--------|
| `NotFoundException` | `Sirb.CepBrasil/Exceptions/NotFoundException.cs` | Exceção para CEP não encontrado |
| `ServiceException` | `Sirb.CepBrasil/Exceptions/ServiceException.cs` | Exceção para erro de serviço externo |
| `ExceptionExtension` | `Sirb.CepBrasil/Extensions/ExceptionExtension.cs` | Métodos de extensão para Exception |
| `CepResult` | `Sirb.CepBrasil/Models/CepResult.cs` | Container de resultado com lista de exceções |
| `CepService` | `Sirb.CepBrasil/Services/CepService.cs` | Orquestrador que captura exceções no fallback |
| `BaseService` | `Sirb.CepBrasil/Services/BaseService.cs` | Classe base que lança ServiceException |
| `CepValidation` | `Sirb.CepBrasil/Validations/CepValidation.cs` | Validação que lança exceções |
| `ExceptionExtensionTest` | `Sirb.CepBrasil.Test/Extensions/ExceptionExtensionTest.cs` | Testes do ExceptionExtension (29 testes) |

---

## 5. Requisitos Funcionais

| ID | Descrição | Prioridade | Regra de negócio associada |
|----|-----------|------------|----------------------------|
| RF-17 | Definir `NotFoundException` para CEP não encontrado | Alta | RN-019 |
| RF-18 | Definir `ServiceException` para erros de serviço externo | Alta | RN-020 |
| RF-19 | Implementar `ExceptionExtension.AllMessages()` para concatenação da cadeia de exceções | Alta | RN-021 |
| RF-20 | Implementar `ExceptionExtension.GetDetailedMessage()` com opção de stack trace | Média | RN-022 |
| RF-21 | Implementar `ExceptionExtension.GetExceptionChain()` para iteração pela cadeia | Alta | RN-021 |
| RF-22 | Integrar `ServiceException` no `BaseService` para erros HTTP e desserialização | Alta | RN-020, RN-023 |
| RF-23 | Capturar exceções no `CepService` e consolidar no `CepResult` | Alta | RN-013 (herdado), RN-024 |
| RF-24 | Prover método `ThrowIf` estático em `NotFoundException` e `ServiceException` | Média | RN-025 |
| RF-25 | Registrar exceções capturadas na propriedade `Exceptions` do `CepResult` | Média | RN-024 |

---

## 6. Regras de Negócio

| ID | Regra | Origem | Impacto |
|----|-------|--------|---------|
| RN-019 | `NotFoundException` deve ser usada exclusivamente para indicar que um CEP não foi encontrado em nenhum provedor (diferente de erro de serviço) | Semântica de domínio | Diferencia "não encontrado" de "erro" |
| RN-020 | `ServiceException` deve ser usada para erros de comunicação com provedores externos: HTTP não-sucedido, timeout, falha de desserialização, resposta vazia | Semântica de domínio | Exceção padronizada para todos os provedores |
| RN-021 | `AllMessages()` deve retornar todas as mensagens da cadeia de exceções concatenadas com separador " → ", ou string vazia se não houver mensagens | Formatação padronizada | Consistência nas mensagens de erro |
| RN-022 | `GetDetailedMessage()` deve incluir o tipo da exceção (`[NomeTipo]`) antes da mensagem, com opção de incluir stack trace | Diagnóstico | Logging estruturado |
| RN-023 | Toda requisição HTTP com status code não-sucedido deve lançar `ServiceException` via `ServiceException.ThrowIf()` | Consistência | Padrão único de erro HTTP |
| RN-024 | Toda exceção capturada durante o fallback deve ser adicionada à lista `Exceptions` do `CepResult` e ter sua mensagem consolidada via `AllMessages()` | Rastreabilidade | Rastreio completo de falhas |
| RN-025 | O método `ThrowIf(bool condition, string message, Exception innerException)` deve lançar a exceção apenas quando `condition` for `true` | Padrão de validação | Código conciso e legível |

---

## 7. Critérios de Aceitação (Gherkin)

```gherkin
Funcionalidade: Tratamento de Exceções de Serviço
  Como um sistema consumidor da biblioteca
  Quero receber exceções de domínio claras e diferenciadas
  Para tratar cada cenário de falha de forma específica

  Cenário: NotFoundException é lançada quando CEP não é encontrado
    Dado que o CEP informado é "00000000"
    E que todos os provedores retornam null (não encontrado)
    Quando o método FindAsync() é executado
    Então o CepResult deve ter Success = false
    E a propriedade Exceptions deve conter uma NotFoundException

  Cenário: ServiceException é lançada quando provedor falha
    Dado que o CEP informado é "01310100"
    E que a BrasilAPI retorna HTTP 500
    Quando o método FindAsync() é executado
    Então a exceção deve ser capturada como ServiceException
    E o próximo provedor deve ser tentado

  Cenário: Múltiplos erros são agregados no CepResult
    Dado que o CEP informado é "01310100"
    E que todos os 4 provedores lançam exceções
    Quando o método FindAsync() é executado
    Então o CepResult.Exceptions deve conter 4 exceções
    E o CepResult.Message deve conter mensagens de todos os provedores

  Cenário: AllMessages concatena cadeia de exceções
    Dado que uma exceção contém InnerException com mensagem "Erro interno"
    Quando o método AllMessages() é executado
    Então o resultado deve conter ambas as mensagens separadas por " → "

  Cenário: GetDetailedMessage inclui tipo da exceção
    Dado que uma exceção do tipo ServiceException com mensagem "Erro"
    Quando o método GetDetailedMessage() é executado
    Então o resultado deve conter "[ServiceException] Erro"

  Cenário: GetDetailedMessage com stack trace
    Dado que uma exceção foi lançada em tempo de execução
    Quando o método GetDetailedMessage(includeStackTrace: true) é executado
    Então o resultado deve incluir o stack trace da exceção

  Cenário: ArgumentException para CEP inválido
    Dado que o CEP informado é null
    Quando o método FindAsync() é executado
    Então deve ser propagada ArgumentNullException
    E não deve ser capturada pelo CepService

  Cenário: ThrowIf condicional
    Dado que uma condição é verdadeira
    Quando o método ThrowIf(true, "mensagem") é executado
    Então a exceção correspondente deve ser lançada

  Cenário: ThrowIf não lança quando condição é falsa
    Dado que uma condição é falsa
    Quando o método ThrowIf(false, "mensagem") é executado
    Então nenhuma exceção deve ser lançada
```

---

## 8. Requisitos Não Funcionais (NFRs)

| Categoria | Requisito | Métrica/Critério |
|-----------|-----------|------------------|
| Desempenho | `AllMessages()` deve executar em < 1ms para cadeias de até 10 exceções | Tempo de execução < 1ms |
| Desempenho | `GetExceptionChain()` deve usar lazy evaluation (yield return) | Sem materialização desnecessária da coleção |
| Segurança | Mensagens de erro não devem expor detalhes internos (stack trace, IPs internos) | Mensagens genéricas em português |
| Manutenibilidade | 100% de cobertura de testes | Cobertura medida via Coverlet |
| Manutenibilidade | 100% de documentação XML em inglês | Tags summary, param, returns, exception, example |
| Compatibilidade | Exceções serializáveis (`[Serializable]`) para cenários cross-AppDomain | Atributo `[Serializable]` presente |
| Confiabilidade | `ThrowIf` deve ser deterministic: mesma condição → mesmo resultado | Função pura, sem estado externo |
| Compatibilidade | Funcionar em .NET 8, 9 e 10 | Build multi-target no CI |

---

## 9. Wireframe

**Não aplicável.** Projeto é uma biblioteca .NET sem interface de usuário. Nenhum wireframe ou padrão visual é necessário.

```
⏳ PENDENTE [Fase 3 — Design] — N/A: Biblioteca .NET sem interface de usuário.
```

---

## 10. Integração de API / Banco de Dados

**Não aplicável.** O tratamento de exceções é uma operação puramente local (classes C# em memória) — não envolve chamadas de API, banco de dados ou integrações externas.

```
⏳ PENDENTE [Fase 3 — Dados] — N/A: Requisito não envolve banco de dados ou endpoints de API.
```

---

## 11. Matriz de Risco do Requisito (Recorte Local)

| ID | Requisito | Descrição do Risco | Probabilidade | Impacto | Score | Nível | Mitigação | Status |
|----|-----------|-------------------|---------------|---------|-------|-------|-----------|--------|
| RSK-015 | req-0003 | ServiceException genérica demais, consumidor não consegue distinguir entre "timeout", "HTTP 500", "resposta vazia" | M (3) — Média | M (3) — Médio | 9 | 🟡 Médio | Mensagens padronizadas via `CepMessages`; construtor aceita innerException para rastreio; `Exceptions` no `CepResult` preserva o tipo original | 🟢 Aceito |
| RSK-016 | req-0003 | NotFoundException não propagada corretamente para o CepResult (consumidor recebe CepResult com Success=false sem saber que foi "não encontrado" vs "erro") | B (2) — Baixa | M (3) — Médio | 6 | 🟡 Médio | NotFoundException adicionada à lista Exceptions do CepResult quando CEP não encontrado; documentação XML esclarece semântica | 🟢 Aceito |
| RSK-017 | req-0003 | ExceptionExtension.AllMessages() com ArgumentNullException para entrada nula pode quebrar código legado que esperava string vazia | B (2) — Baixa | B (2) — Baixo | 4 | 🟢 Baixo | Comportamento documentado; exceção é a prática correta para null | 🟢 Aceito |
| RSK-018 | req-0003 | Stack trace exposto em GetDetailedMessage pode vazar informações internas se logado sem sanitização | B (2) — Baixa | B (2) — Baixo | 4 | 🟢 Baixo | Parâmetro `includeStackTrace` é false por padrão; responsabilidade do consumidor de só habilitar em logging interno | 🟢 Aceito |

---

## 12. Análise de Pontos de Função (APF) Vinculada

- **Documento APF:** `docs/requirement/apf/apf-req-0003.md`
- **Total estimado:** ver documento APF específico.

---

## 13. Impactos Técnicos

### Backend / Código

- `Sirb.CepBrasil/Exceptions/NotFoundException.cs` — exceção para CEP não encontrado
- `Sirb.CepBrasil/Exceptions/ServiceException.cs` — exceção para erro de serviço
- `Sirb.CepBrasil/Extensions/ExceptionExtension.cs` — métodos de extensão (AllMessages, GetDetailedMessage)
- `Sirb.CepBrasil/Models/CepResult.cs` — container de resultado com lista Exceptions (já existente)
- `Sirb.CepBrasil/Services/CepService.cs` — captura e consolidação de exceções no fallback
- `Sirb.CepBrasil/Services/BaseService.cs` — lançamento de ServiceException em erros HTTP/desserialização

### Frontend

- N/A — biblioteca .NET sem UI

### Banco de Dados

- N/A — sem persistência

### Integrações Externas

- N/A — as exceções são disparadas *durante* integrações externas, mas o tratamento é local

### Design / UX

- N/A — sem interface de usuário

---

## 14. Rastreabilidade

| Item | Referência |
|------|------------|
| Código — NotFoundException | `Sirb.CepBrasil/Exceptions/NotFoundException.cs` |
| Código — ServiceException | `Sirb.CepBrasil/Exceptions/ServiceException.cs` |
| Código — ExceptionExtension | `Sirb.CepBrasil/Extensions/ExceptionExtension.cs` |
| Código — CepResult | `Sirb.CepBrasil/Models/CepResult.cs` |
| Código — CepService | `Sirb.CepBrasil/Services/CepService.cs` |
| Código — BaseService | `Sirb.CepBrasil/Services/BaseService.cs` |
| Testes — ExceptionExtension | `Sirb.CepBrasil.Test/Extensions/ExceptionExtensionTest.cs` (29 testes) |
| Documento técnico | `docs/requirement/tec/tec-req-0003-excecoes-servico.md` |
| APF | `docs/requirement/apf/apf-req-0003.md` |
| Análise cross-artifact | `docs/requirement/analises/analise-req-0003.md` |
| Quality Checklist | `docs/requirement/checklist/checklist-req-0003.md` |
| Matriz de risco global | `docs/system-risk-matrix.md` |
| Constituição | `docs/constituicao.md` |
| Documentos de melhoria | `docs/extra/MELHORIAS-EXCEPTIONEXTENSION.md`, `docs/extra/SUMARIO-FINAL-EXCEPTIONEXTENSION.md` |

---

## 15. Validações Realizadas para esta Documentação

- [x] README do projeto analisado
- [x] Código principal analisado (`NotFoundException.cs`, `ServiceException.cs`, `ExceptionExtension.cs`, `CepResult.cs`, `CepService.cs`, `BaseService.cs`)
- [x] Testes unitários analisados (`ExceptionExtensionTest.cs`)
- [x] Documentos de melhoria consultados (`MELHORIAS-EXCEPTIONEXTENSION.md`, `SUMARIO-FINAL-EXCEPTIONEXTENSION.md`)
- [x] Constituição do projeto revisada
- [x] Arquitetura e tech stack revisados
- [x] Artefatos relacionados revisados (req-0001, req-0002, tec-req-0001, tec-req-0002)

---

## 16. Clarification Log

| Data | Pergunta | Resposta | Status (Resolvido/Pendente) | Origem (Criação/Implementação) | Impacto no requisito |
|------|----------|----------|----------------------------|-------------------------------|----------------------|
| 23/07/2026 | Qual nome de usuário deve constar no campo `Autor` e nas entradas de histórico deste documento? | Rodrigo Araujo Barbosa | Resolvido | Criação | Define autor do documento |
| 23/07/2026 | As exceções NotFoundException e ServiceException têm método ThrowIf? | Sim, ambas têm `ThrowIf(bool condition, string message, Exception innerException = null)` | Resolvido | Criação | Confirma RF-24 e RN-025 |
| 23/07/2026 | O CepResult já tem uma propriedade Exceptions (List\<Exception\>)? | Sim, confirmado no código `CepResult.cs` linha 76 | Resolvido | Criação | Confirma RF-25 |
| 23/07/2026 | O ExceptionExtension.GetDetailedMessage() aceita parâmetro includeStackTrace? | Sim, `bool includeStackTrace = false` | Resolvido | Criação | Confirma RN-022 |

### Cobertura do Clarification (8 áreas)

| # | Área | Status (Resolvido \| N/A) | Ref. entrada no Log | Justificativa (se N/A) |
|---|------|---------------------------|---------------------|------------------------|
| 1 | Atores e personas | Resolvido | 23/07/2026 — Autor definido | — |
| 2 | Fluxos principais e alternativos | N/A | — | Fluxo de exceções é condicional e coberto pelos cenários Gherkin |
| 3 | Exceções e erros | Resolvido | 23/07/2026 — ThrowIf, Exceptions, GetDetailedMessage confirmados | — |
| 4 | Integrações externas | N/A | — | Exceções são disparadas *durante* integrações externas, mas o tratamento é local |
| 5 | Requisitos não-funcionais | Resolvido | Coberto na seção NFR (desempenho, segurança, manutenibilidade) | — |
| 6 | Dados e privacidade | N/A | — | Exceções não processam dados pessoais; stack trace é opcional e controlado |
| 7 | Validações e regras de negócio | Resolvido | Coberto nas RN-019 a RN-025 | — |
| 8 | Critérios de aceite e mensurabilidade | Resolvido | Coberto nos 9 cenários Gherkin | — |

**Cobertura atual:** 6/8 (Resolvido). **Meta:** 8/8.

---

## 17. Esclarecimentos Adicionais

| Data | Nota |
|------|------|
| 23/07/2026 | Documento criado com base no código-fonte v1.4.0 e documentos de melhoria em docs/extra |
| 23/07/2026 | As exceções `NotFoundException` e `ServiceException` já estão implementadas no código. Este requisito documenta o comportamento existente e formaliza as regras de negócio. |
| 23/07/2026 | O `ExceptionExtension` foi refatorado com base no documento MELHORIAS-EXCEPTIONEXTENSION.md, adicionando `GetDetailedMessage()`, `GetExceptionChain()` e melhorando `AllMessages()` com LINQ |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**