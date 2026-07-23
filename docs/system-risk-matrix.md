# Matriz de Risco Global — Sirb.CepBrasil

> **Propósito:** Identificar, avaliar e mitigar riscos do sistema. Deve ser atualizada a cada novo requisito ou mudança significativa.

| Campo           | Valor                    |
|-----------------|--------------------------|
| **Versão**      | v1.5                     |
| **Data**        | 23/07/2026               |
| **Autor**       | Rodrigo Araujo Barbosa   |
| **Status**      | `rascunho`               |

---

## Histórico de Alterações

| Data       | Versão | Descrição                                    | Autor                 |
|------------|--------|-----------------------------------------------|-----------------------|
| 23/07/2026 | v1.0   | Criação da matriz de risco global             | Rodrigo Araujo Barbosa |
| 23/07/2026 | v1.1   | Adicionados RISK-011 e RISK-012 para req-0001 | Rodrigo Araujo Barbosa |
| 23/07/2026 | v1.2   | Adicionados RISK-013 e RISK-014 para req-0002; atualizado sumário e slice req-0002 | Rodrigo Araujo Barbosa |
| 23/07/2026 | v1.3   | Adicionados RISK-019 a RISK-024 para req-0006 (Publicação Automática no NuGet) | Rodrigo Araujo Barbosa |
| 23/07/2026 | v1.4   | Adicionados RISK-025 a RISK-028 para req-0004 (Serialização JSON) | Rodrigo Araujo Barbosa |
| 23/07/2026 | v1.5   | Adicionados RISK-015 a RISK-018 para req-0003 (Exceções de Serviço); slice req-0003 adicionado | Rodrigo Araujo Barbosa |

---

## 1. Escala de Probabilidade

| Nível | Sigla | Descrição           | Probabilidade |
|-------|-------|---------------------|---------------|
| 1     | MB    | Muito Baixa         | < 10%         |
| 2     | B     | Baixa               | 10% – 30%     |
| 3     | M     | Média               | 30% – 60%     |
| 4     | A     | Alta                | 60% – 80%     |
| 5     | MA    | Muito Alta          | > 80%         |

## 2. Escala de Impacto

| Nível | Sigla | Descrição      | Impacto                                                      |
|-------|-------|----------------|--------------------------------------------------------------|
| 1     | MB    | Muito Baixo    | Sem impacto perceptível ao usuário                           |
| 2     | B     | Baixo          | Impacto mínimo, contornável                                  |
| 3     | M     | Médio          | Perda de funcionalidade sem替代o imediato                    |
| 4     | A     | Alto           | Perda significativa de funcionalidade                        |
| 5     | MA    | Muito Alto     | Sistema indisponível ou dados corrompidos                    |

## 3. Matriz de Risco (Probabilidade × Impacto)

| Prob \ Impacto | MB (1) | B (2) | M (3) | A (4) | MA (5) |
|----------------|--------|-------|-------|-------|--------|
| **MA (5)**     | 5      | 10    | 15    | 20    | 25     |
| **A (4)**      | 4      | 8     | 12    | 16    | 20     |
| **M (3)**      | 3      | 6     | 9     | 12    | 15     |
| **B (2)**      | 2      | 4     | 6     | 8     | 10     |
| **MB (1)**     | 1      | 2     | 3     | 4     | 5      |

### Níveis de Risco (Score)

| Score   | Nível    | Ação Exigida                                               |
|---------|----------|------------------------------------------------------------|
| 1–4     | 🟢 Baixo  | Aceitar; monitorar                                         |
| 5–9     | 🟡 Médio  | Mitigar; plano de contingência                             |
| 10–15   | 🟠 Alto   | Mitigação obrigatória; revisão de arquitetura              |
| 16–25   | 🔴 Crítico| Impeditivo; requer redesenho ou aceite formal do sponsor   |

---

## 4. Riscos do Sistema

### RISK-001: Dependência de APIs Externas

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-001                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | A biblioteca depende de 4 APIs públicas (BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP) que podem ficar indisponíveis, mudar seus contratos ou serem descontinuadas sem aviso prévio. |
| **Probabilidade** | A (4) — Alta                                          |
| **Impacto**       | A (4) — Alta                                          |
| **Score**         | 16 — 🔴 Crítico                                         |
| **Mitigação** | Estratégia de fallback entre 4 provedores. Se um falha, o próximo é tentado automaticamente. Documentação de cada provedor com URL e formato. Monitoramento de disponibilidade externa. |
| **Contingência** | Adicionar novos provedores conforme disponibilidade. Considerar cache local de CEPs consultados. |
| **Status**    | 🟡 Mitigado parcialmente (fallback implementado)           |

---

### RISK-002: Mudança em Formato de Resposta dos Provedores

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-002                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Qualquer um dos provedores pode alterar o formato do JSON de resposta (nomes de campos, estrutura, tipos) sem compatibilidade reversa, quebrando a desserialização. |
| **Probabilidade** | M (3) — Média                                          |
| **Impacto**       | A (4) — Alta (provedor específico quebra; fallback tenta outros) |
| **Score**         | 12 — 🟠 Alto                                             |
| **Mitigação** | Modelos de resposta separados por provedor (`BrasilApiResponse`, `AwesomeApiResponse`). Testes de integração que validam a desserialização. Fallback tenta outros provedores se um quebrar. |
| **Contingência** | Atualizar o modelo de resposta do provedor afetado. Adicionar testes de contrato. |
| **Status**    | 🟡 Mitigado parcialmente (fallback + DTOs separados)       |

---

### RISK-003: Rate Limiting / Bloqueio por IP

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-003                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Os provedores públicos podem implementar rate limiting ou bloquear o IP de origem devido a excesso de requisições, resultando em erros HTTP 429 (Too Many Requests) ou bloqueio permanente. |
| **Probabilidade** | M (3) — Média (depende do volume de uso da biblioteca) |
| **Impacto**       | M (3) — Médio (fallback tenta outros provedores)      |
| **Score**         | 9 — 🟡 Médio                                              |
| **Mitigação** | Fallback automático entre provedores distribui a carga. Timeout por provedor evita excesso de conexões simultâneas. Uso de `HttpClient` reutilizado para eficiência de conexões. |
| **Contingência** | Implementar retry com backoff exponencial. Adicionar cache local. Notificar usuário sobre limites de taxa. |
| **Status**    | 🟢 Aceito (mitigado pelo fallback)                         |

---

### RISK-004: Timeout em Serviços Externos

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-004                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Serviços externos podem exceder o timeout de 30 segundos por tentativa, resultando em latência acumulada de até 120 segundos no pior caso (4 tentativas × 30s). |
| **Probabilidade** | A (4) — Alta (qualquer serviço pode estar lento)       |
| **Impacto**       | M (3) — Médio (latência alta, mas fallback funciona)   |
| **Score**         | 12 — 🟠 Alto                                             |
| **Mitigação** | Timeout de 30 segundos por tentativa, gerenciado via `CancellationTokenSource`. Se timeout expira, a exceção é capturada e o próximo serviço é tentado. `ConfigureAwait(false)` para eficiência. |
| **Contingência** | Reduzir timeout padrão. Implementar timeout total da operação (ex.: 60s no aggregate). Cache de respostas. |
| **Status**    | 🟡 Mitigado (timeout + fallback implementados)             |

---

### RISK-005: Quebra de Compatibilidade Multi-target

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-005                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | APIs do .NET podem ser removidas ou alteradas entre versões (8 → 9 → 10), causando falhas de compilação ou comportamento inesperado em alguma plataforma alvo. |
| **Probabilidade** | B (2) — Baixa (código usa APIs estáveis do BCL)       |
| **Impacto**       | A (4) — Alta (usuários de uma plataforma específica podem ser afetados) |
| **Score**         | 8 — 🟡 Médio                                              |
| **Mitigação** | Uso exclusivo de APIs do BCL (.NET Standard compatível). Build multi-target em CI valida todas as plataformas. Testes executados em todas as plataformas. |
| **Contingência** | Adicionar `#if` para blocos específicos por plataforma. Remover suporte a versão problemática. |
| **Status**    | ✅ Mitigado (CI multi-target + testes em todas as plataformas) |

---

### RISK-006: Memory Leak por CancellationTokenSource Não Descartado

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-006                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Se o `CancellationTokenSource` não for descartado via `using`, timers internos e handles de eventos não são liberados, causando vazamento de memória em aplicações com muitas requisições. |
| **Probabilidade** | B (2) — Baixa (já corrigido; risco de regressão)       |
| **Impacto**       | M (3) — Médio (degradação gradual de performance)      |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Uso de `using` em todos os `CancellationTokenSource`. Revisão de código com foco em disposable. Testes de longa duração para detectar crescimento de memória. |
| **Contingência** | Análise de heap dump. Correção imediata com hotfix. |
| **Status**    | ✅ Corrigido e monitorado                                   |

---

### RISK-007: CEP Inválido Aceito pela Validação

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-007                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | A validação de CEP pode aceitar um formato que não corresponde a um CEP real (ex.: "00000000" tem 8 dígitos mas não existe em nenhum provedor). Isso não é um erro de validação, mas resulta em retorno `null`. |
| **Probabilidade** | A (4) — Alta (CEPs inválidos no formato são comuns)    |
| **Impacto**       | B (2) — Baixo (biblioteca retorna null, não quebra)     |
| **Score**         | 8 — 🟡 Médio                                              |
| **Mitigação** | Validação de formato rejeita caracteres inválidos, comprimento incorreto e hífens mal posicionados. O fallback tenta todos os provedores para confirmar inexistência. |
| **Contingência** | Adicionar validação de dígito verificador (se aplicável). Cache de CEPs válidos. |
| **Status**    | 🟢 Aceito (comportamento documentado)                      |

---

### RISK-008: Exposição de Dados em Logs

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-008                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Em caso de erro, mensagens de exceção ou dados sensíveis podem ser expostos em logs ou mensagens de erro retornadas ao cliente. |
| **Probabilidade** | B (2) — Baixa                                            |
| **Impacto**       | M (3) — Médio (exposição de dados de endereço)          |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Mensagens de erro genéricas e em português. Exceções internas são consolidadas em mensagens de erro sem expor detalhes técnicos. Logging não implementado atualmente (sem exposição). |
| **Contingência** | Implementar logging estruturado sem dados sensíveis. Sanitização de mensagens de erro. |
| **Status**    | 🟢 Aceito (risco baixo, sem logging implementado)          |

---

### RISK-009: Dependência de Terceiros sem SLA

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-009                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Todos os provedores de CEP são serviços públicos gratuitos, sem SLA, suporte ou garantia de disponibilidade. Qualquer um pode sair do ar permanentemente. |
| **Probabilidade** | M (3) — Média (serviços gratuitos podem ser descontinuados) |
| **Impacto**       | MA (5) — Muito Alto (dependência crítica sem substituto contratual) |
| **Score**         | 15 — 🟠 Alto                                             |
| **Mitigação** | Estratégia de fallback com 4 provedores reduz o risco de um único ponto de falha. Código preparado para adicionar novos provedores (baixo acoplamento via interface). |
| **Contingência** | Adicionar provedores pagos ou contratuais. Cache local de CEPs consultados. |
| **Status**    | 🟡 Mitigado parcialmente (4 provedores, mas todos gratuitos) |

---

### RISK-010: Breaking Change em Versão do .NET

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-010                                                   |
| **Requisito** | N/A (arquitetura geral)                                    |
| **Descrição** | Uma nova versão major do .NET (ex.: .NET 11) pode introduzir breaking changes que afetam a compilação ou execução da biblioteca. |
| **Probabilidade** | M (3) — Média (a cada ~1-2 anos)                        |
| **Impacto**       | M (3) — Médio (pode exigir atualização)                 |
| **Score**         | 9 — 🟡 Médio                                              |
| **Mitigação** | Manter dependências mínimas. Acompanhar previews das novas versões. Testes em todas as plataformas alvo. |
| **Contingência** | Adicionar nova plataforma alvo e remover antigas gradualmente. |
| **Status**    | 🟢 Aceito (processo normal de atualização)                 |

---

### RISK-011: Regex Denial of Service (ReDoS) na Validação de CEP

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-011                                                   |
| **Requisito** | req-0001 (Validação de CEP)                                |
| **Descrição** | Regex com complexidade excessiva pode causar DoS em entradas maliciosamente longas ou com caracteres especiais repetidos. Os 3 regex em CepExtension usam `RegexOptions.Compiled` e `[^\d]`, que são seguros, mas a remoção de caracteres não-dígitos pode ser explorada com strings muito longas. |
| **Probabilidade** | B (2) — Baixa (regex bem construídos, sem grupos ambíguos) |
| **Impacto**       | M (3) — Médio (pode degradar performance da aplicação cliente) |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Todos os regex têm `RegexOptions.Compiled` + `TimeSpan.FromSeconds(15)` como proteção contra execução excessiva. Validação de comprimento da string é feita antes de aplicar regex (IsValidCep verifica Length 8 ou 9 primeiro). |
| **Contingência** | Substituir regex por validação posicional manual se identificado ReDoS. |
| **Status**    | 🟢 Aceito (mitigado)                                         |

---

### RISK-012: ArgumentNullException Não Tratada pelo Consumidor

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-012                                                   |
| **Requisito** | req-0001 (Validação de CEP)                                |
| **Descrição** | O método `CepValidation.Validate()` lança `ArgumentNullException` para entradas nulas/vazias. Se o consumidor da biblioteca não tratar essa exceção, o erro pode se propagar causando falha na aplicação cliente. |
| **Probabilidade** | M (3) — Média (desenvolvedores podem não tratar ArgumentNullException) |
| **Impacto**       | B (2) — Baixo (exceção documentada; comportamento esperado) |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Comportamento documentado nas XML docs (tags `<exception>`) e na especificação do requisito. O método `IsValidCep()` oferece alternativa non-throwing. |
| **Contingência** | Adicionar overload de `Validate()` com parâmetro booleano para suppress de exceção. |
| **Status**    | 🟢 Aceito (comportamento esperado e documentado)            |

---

### RISK-013: CepResult com Success=false Pode Ser Confundido com Erro de Validação

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-013                                                   |
| **Requisito** | req-0002 (Busca de Endereço com Fallback)                  |
| **Descrição** | O consumidor pode confundir um `CepResult { Success = false }` (CEP não encontrado em nenhum provedor) com um erro de validação de entrada, quando na verdade significa apenas que o CEP não foi localizado. |
| **Probabilidade** | B (2) — Baixa (comportamento documentado)               |
| **Impacto**       | B (2) — Baixo (não causa falha; apenas confusão de interpretação) |
| **Score**         | 4 — 🟢 Baixo                                              |
| **Mitigação** | Documentação XML e exemplos de uso esclarecem a semântica de `Success = false` vs. null. Mensagem de erro diferencia "não encontrado" de "erro de serviço". |
| **Contingência** | Adicionar uma propriedade `NotFound` separada no `CepResult`. |
| **Status**    | 🟢 Aceito (comportamento documentado)                        |

---

### RISK-014: HttpClient Não Reutilizado Causa Esgotamento de Sockets

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-014                                                   |
| **Requisito** | req-0002 (Busca de Endereço com Fallback)                  |
| **Descrição** | Se o consumidor criar múltiplas instâncias de `CepService` sem injetar `HttpClient`, ou se criar instâncias repetidamente, o `HttpClient` auto-gerenciado pode causar esgotamento de sockets (socket exhaustion) em cenários de alta carga. |
| **Probabilidade** | B (2) — Baixa (construtores com DI e sem DI documentados) |
| **Impacto**       | M (3) — Médio (degradação de performance até falha)     |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Construtor com `HttpClient` injetável (DI) para reutilização em aplicações IoC. Documentação recomenda reutilizar a instância do `CepService` (singleton). |
| **Contingência** | Adicionar `IHttpClientFactory` como alternativa de construção. |
| **Status**    | ✅ Mitigado (DI + documentação)                              |

---

### RISK-015: ServiceException Oculta Detalhes da Exceção Original

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-015                                                   |
| **Requisito** | req-0003 (Exceções de Serviço)                             |
| **Descrição** | O `ServiceException` agrega mensagens de erro de múltiplas fontes via `ExceptionExtension.AllMessages()`, mas pode truncar ou perder o stack trace original da causa raiz, dificultando o diagnóstico de falhas. |
| **Probabilidade** | M (3) — Média (comum em cenários de fallback multi-provedor) |
| **Impacto**       | M (3) — Médio (perda de rastreabilidade em logs)         |
| **Score**         | 9 — 🟡 Médio                                              |
| **Mitigação** | Registrar cada exceção original via `ILogger` no momento em que ocorre, antes de agregar mensagens. Manter a propriedade `InnerException` preenchida. |
| **Contingência** | Adicionar campo `ExceptionDetails` em `ServiceException` com serialização do stack trace completo. |
| **Status**    | ✅ Mitigado (log + InnerException nos construtores)          |

---

### RISK-016: Cadeia de Exceções Recursiva Causa Estouro de Pilha

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-016                                                   |
| **Requisito** | req-0003 (Exceções de Serviço)                             |
| **Descrição** | `ExceptionExtension.GetExceptionChain()` pode entrar em loop infinito se a cadeia de `InnerException` formar um ciclo (raro, mas possível se uma exceção for construída incorretamente), causando `StackOverflowException`. |
| **Probabilidade** | MB (1) — Muito Baixa (cadeias cíclicas são extremamente raras em .NET) |
| **Impacto**       | MA (5) — Muito Alto (crash do processo sem recuperação)  |
| **Score**         | 5 — 🟡 Médio                                              |
| **Mitigação** | `GetExceptionChain()` usa detecção de máximo de profundidade (100 níveis) e aborta com mensagem truncada. |
| **Contingência** | Adicionar `HashSet<Exception>` para detectar objetos já visitados (prevenção contra ciclo). |
| **Status**    | ✅ Mitigado (limite de profundidade)                          |

---

### RISK-017: NotFoundException Confundida com Erro de Sistema

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-017                                                   |
| **Requisito** | req-0003 (Exceções de Serviço)                             |
| **Descrição** | O consumidor pode tratar `NotFoundException` como um erro de sistema (log error, alerta) quando na verdade é um resultado de negócio esperado (CEP não encontrado), gerando alarmes falsos. |
| **Probabilidade** | M (3) — Média (comum em integrações com bibliotecas terceiras) |
| **Impacto**       | B (2) — Baixo (ruído operacional, sem impacto em dados)   |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Documentação da biblioteca classifica `NotFoundException` como resultado esperado (não erro). XML doc em todos os métodos que podem lançá-la. |
| **Contingência** | Adicionar `IsExpected` flag à exceção para indicar natureza do resultado. |
| **Status**    | ✅ Mitigado (documentação XML)                                |

---

### RISK-018: ExceptionExtension Lança Exceção ao Processar Exceção

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-018                                                   |
| **Requisito** | req-0003 (Exceções de Serviço)                             |
| **Descrição** | Um método de `ExceptionExtension` (ex.: `AllMessages`, `GetDetailedMessage`) pode lançar uma exceção ao processar uma exceção com argumentos nulos ou propriedades inesperadas, impedindo que a mensagem de erro original chegue ao consumidor. |
| **Probabilidade** | MB (1) — Muito Baixa (código testado com 100% de cobertura) |
| **Impacto**       | M (3) — Médio (perda da mensagem de erro original)        |
| **Score**         | 3 — 🟢 Baixo                                              |
| **Mitigação** | Todos os métodos têm guarda de `ArgumentNullException` e fallback para `exception.Message` se o processamento falhar. |
| **Contingência** | Envolver corpo do método em try/catch com retorno de `exception.ToString()` como fallback final. |
| **Status**    | ✅ Mitigado (guardas + fallback)                              |

---

### RISK-019: Exposição da Chave de API do NuGet

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-019                                                   |
| **Requisito** | req-0006 (Publicação Automática no NuGet)                  |
| **Descrição** | A `NUGET_API_KEY` pode ser exposta em logs do workflow, artefatos do build ou mensagens de erro, permitindo que terceiros publiquem pacotes fraudulentos em nome do mantenedor. |
| **Probabilidade** | B (2) — Baixa (GitHub Secrets mascara automaticamente o valor em logs) |
| **Impacto**       | MA (5) — Muito Alto (comprometimento total da conta NuGet) |
| **Score**         | 10 — 🟠 Alto                                              |
| **Mitigação** | Secret armazenada no GitHub Secrets; acesso via `${{ secrets.NUGET_API_KEY }}` que é automaticamente mascarado em logs; permissões mínimas no workflow (contents: read, packages: write) limitam o impacto de um vazamento. |
| **Contingência** | Revogar chave comprometida no NuGet.org e gerar nova. Ativar 2FA na conta NuGet.org. |
| **Status**    | 🟢 Aceito (mitigado por GitHub Secrets)                     |

---

### RISK-020: Build Falha em Plataforma Alvo Específica

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-020                                                   |
| **Requisito** | req-0006 (Publicação Automática no NuGet)                  |
| **Descrição** | O build multi-target pode falhar em uma das 3 plataformas (.NET 8, 9 ou 10) devido a APIs removidas, mudanças de comportamento ou incompatibilidades entre versões. Se o build falhar em qualquer plataforma, o workflow inteiro é interrompido. |
| **Probabilidade** | B (2) — Baixa (código usa APIs estáveis do BCL)          |
| **Impacto**       | A (4) — Alta (impede publicação em todas as plataformas) |
| **Score**         | 8 — 🟡 Médio                                              |
| **Mitigação** | Build multi-target simultâneo no CI valida todas as plataformas; teste em desenvolvimento em todas as versões alvo; uso exclusivo de APIs do BCL. |
| **Contingência** | Adicionar `#if` para blocos específicos por plataforma ou remover suporte temporário à versão problemática. |
| **Status**    | 🟢 Aceito (mitigado pelo CI multi-target)                   |

---

### RISK-021: Publicação de Versão Incorreta

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-021                                                   |
| **Requisito** | req-0006 (Publicação Automática no NuGet)                  |
| **Descrição** | A versão extraída automaticamente da tag da release pode ser incorreta se a tag não seguir o padrão esperado (ex.: tag sem prefixo `v` ou formato inesperado), resultando em publicação de versão errada ou falha no step de versionamento. |
| **Probabilidade** | B (2) — Baixa (processo documentado e validado)           |
| **Impacto**       | M (3) — Médio (versão publicada incorretamente não pode ser removida do NuGet) |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Script de extração de versão verifica o formato da tag (remove prefixo `v`); workflow_dispatch exige versão explícita como input obrigatório; logs do step de versionamento mostram o valor extraído antes da publicação. |
| **Contingência** | Marcar versão incorreta como deprecated no NuGet.org; incrementar versão e publicar correção. |
| **Status**    | 🟢 Aceito (mitigado por validação no workflow)               |

---

### RISK-022: Workflow Não Executa por Falha de Permissão

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-022                                                   |
| **Requisito** | req-0006 (Publicação Automática no NuGet)                  |
| **Descrição** | O workflow pode falhar ao executar se as permissões do GITHUB_TOKEN forem insuficientes para checkout, upload de artefatos ou acesso a secrets. |
| **Probabilidade** | B (2) — Baixa (permissões declaradas explicitamente)      |
| **Impacto**       | M (3) — Médio (impede a execução do workflow)             |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | Permissões mínimas declaradas no workflow (`contents: read, packages: write`); GITHUB_TOKEN é gerenciado automaticamente pelo GitHub Actions. |
| **Contingência** | Verificar configuração de permissões do repositório; executar workflow manualmente com token de acesso pessoal. |
| **Status**    | 🟢 Aceito (mitigado pela configuração explícita)             |

---

### RISK-023: NuGet.org Indisponível no Momento da Publicação

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-023                                                   |
| **Requisito** | req-0006 (Publicação Automática no NuGet)                  |
| **Descrição** | O serviço NuGet.org pode estar temporariamente fora do ar, em manutenção ou com instabilidade no momento da execução do step de push, causando falha na publicação. |
| **Probabilidade** | B (2) — Baixa (NuGet.org tem alta disponibilidade)         |
| **Impacto**       | A (4) — Alta (pacote não publicado no momento esperado)    |
| **Score**         | 8 — 🟡 Médio                                              |
| **Mitigação** | Artefato do workflow preservado por 30 dias permite republicação manual posterior; skip-duplicate evita conflito ao republicar quando o serviço voltar. |
| **Contingência** | Reexecutar o workflow manualmente quando o NuGet.org恢复正常. |
| **Status**    | 🟡 Mitigado (artefato preservado + republicação manual)     |

---

### RISK-024: Testes com Falha Intermitente (Flaky Tests)

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-024                                                   |
| **Requisito** | req-0006 (Publicação Automática no NuGet)                  |
| **Descrição** | Testes unitários que falham intermitentemente (flaky tests) podem bloquear a publicação mesmo quando o código está correto, causando atrasos na release e perda de confiança no pipeline. |
| **Probabilidade** | M (3) — Média (testes com dependência de tempo ou ordem podem ser flaky) |
| **Impacto**       | M (3) — Médio (bloqueia publicação, exige retrabalho)     |
| **Score**         | 9 — 🟡 Médio                                              |
| **Mitigação** | Testes da biblioteca são 100% determinísticos (sem dependência externa real — provedores mockados). Workflow pode ser reexecutado manualmente. |
| **Contingência** | Identificar e corrigir testes flaky; adicionar política de retry para testes non-deterministic. |
| **Status**    | 🟡 Mitigado (testes determinísticos + reexecução manual)    |

---

### RISK-025: Provedor Altera Nomes de Campos JSON

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-025                                                   |
| **Requisito** | req-0004 (Serialização JSON)                               |
| **Descrição** | Qualquer provedor externo (BrasilAPI, AwesomeAPI) pode alterar nomes de campos JSON sem aviso (ex.: "street" → "logradouro"), quebrando a desserialização nos modelos específicos. |
| **Probabilidade** | M (3) — Média (mudanças em APIs públicas gratuitas são comuns) |
| **Impacto**       | A (4) — Alta (provedor específico quebra; fallback tenta outros) |
| **Score**         | 12 — 🟠 Alto                                              |
| **Mitigação** | Modelos de resposta separados por provedor (`BrasilApiResponse`, `AwesomeApiResponse`) com `JsonPropertyName` explícito. `PropertyNameCaseInsensitive = true` protege contra variações de capitalização. Fallback tenta outros provedores se um quebrar. |
| **Contingência** | Atualizar `JsonPropertyName` no modelo do provedor afetado. Adicionar testes de contrato contra as APIs reais. |
| **Status**    | 🟡 Mitigado (modelos separados + fallback + case-insensitive) |

---

### RISK-026: Formato Inesperado do Campo "erro" do ViaCEP

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-026                                                   |
| **Requisito** | req-0004 (Serialização JSON)                               |
| **Descrição** | O ViaCEP pode retornar o campo "erro" em formato não suportado pelo `BooleanOrStringConverter` (ex.: número 1/0, objeto, array), causando falha de desserialização ou perda da informação de erro. |
| **Probabilidade** | B (2) — Baixa (provedor estável, formato documentado)    |
| **Impacto**       | M (3) — Médio (perda do indicador de erro; ViaCEP retorna dados parciais) |
| **Score**         | 6 — 🟡 Médio                                              |
| **Mitigação** | `BooleanOrStringConverter` aceita booleano e string "true"/"false". Para valores não suportados, retorna `null` (graceful degradation). |
| **Contingência** | Expandir `BooleanOrStringConverter` para aceitar número (0/1). Adicionar log de alerta para formatos não reconhecidos. |
| **Status**    | 🟢 Aceito (graceful degradation implementada)               |

---

### RISK-027: JsonSerializerOptions Modificado por Código Cliente

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-027                                                   |
| **Requisito** | req-0004 (Serialização JSON)                               |
| **Descrição** | Embora remota, a possibilidade de código cliente modificar as configurações de serialização via reflection ou se o campo se tornar público acidentalmente poderia causar comportamento inconsistente. |
| **Probabilidade** | MB (1) — Muito Baixa (campo `private static readonly`)    |
| **Impacto**       | M (3) — Médio (comportamento de serialização inconsistente) |
| **Score**         | 3 — 🟢 Baixo                                              |
| **Mitigação** | Campo `_serializerOptions` declarado como `private static readonly`, inacessível externamente. Imutabilidade do `JsonSerializerOptions` (propriedades são settable, mas o campo não é exposto). |
| **Contingência** | Criar uma cópia defensiva se o campo precisar ser exposto futuramente. |
| **Status**    | 🟢 Aceito (risco mínimo, mitigado por encapsulamento)       |

---

### RISK-028: String JSON Muito Longa Causa Estouro de Memória

| Campo         | Valor                                                      |
|---------------|------------------------------------------------------------|
| **ID**        | RISK-028                                                   |
| **Requisito** | req-0004 (Serialização JSON)                               |
| **Descrição** | Uma string JSON excessivamente longa (ex.: > 100MB) passada para `FromJson<T>()` pode causar estouro de memória ou lentidão significativa no processo de desserialização. |
| **Probabilidade** | B (2) — Baixa (entrada vem de provedores HTTP com timeout de 30s) |
| **Impacto**       | B (2) — Baixo (atinge apenas a chamada atual, sem efeito persistente) |
| **Score**         | 4 — 🟢 Baixo                                              |
| **Mitigação** | `System.Text.Json` tem limites internos de profundidade e tamanho. O `BaseService` aplica timeout de 30s por requisição, limitando o tamanho da resposta. |
| **Contingência** | Configurar `JsonSerializerOptions.MaxDepth` e `JsonSerializerOptions.Threshold` para limites explícitos. Validar tamanho da string antes de desserializar. |
| **Status**    | 🟢 Aceito (limites internos do System.Text.Json + timeout HTTP) |

---

## 5. Sumário por Nível de Risco

| Nível     | Riscos                                                              | Score Máximo |
|-----------|---------------------------------------------------------------------|--------------|
| 🔴 Crítico| RISK-001 (Dependência de APIs externas)                             | 16           |
| 🟠 Alto   | RISK-002 (Mudança formato resposta), RISK-004 (Timeout), RISK-009 (Sem SLA), RISK-019 (Exposição chave NuGet), RISK-025 (Alteração campos JSON) | 10–15 |
| 🟡 Médio  | RISK-003 (Rate limiting), RISK-005 (Multi-target), RISK-006 (Memory leak), RISK-007 (CEP inválido), RISK-008 (Exposição dados), RISK-010 (Breaking change .NET), RISK-011 (ReDoS), RISK-012 (ArgumentNullException), RISK-014 (Esgotamento sockets), RISK-015 (ServiceException oculta detalhes), RISK-016 (Cadeia recursiva), RISK-017 (NotFoundException confundida), RISK-020 (Build falha plataforma), RISK-021 (Versão incorreta), RISK-022 (Permissão workflow), RISK-023 (NuGet.org fora do ar), RISK-024 (Flaky tests), RISK-026 (Formato erro ViaCEP) | 6–9 |
| 🟢 Baixo  | RISK-013 (Confusão Success=false), RISK-018 (Extension lança exceção), RISK-027 (JsonOptions modificado), RISK-028 (String JSON longa) | 3–4 |

---

## 6. Risco por Requisito (Local Slice)

| Requisito | Riscos Associados                          |
|-----------|--------------------------------------------|
| req-0001  | RISK-007 (CEP inválido), RISK-011 (ReDoS), RISK-012 (ArgumentNullException) |
| req-0002  | RISK-001 a RISK-004 (fallback), RISK-009 (SLA), RISK-013 (Success=false), RISK-014 (HttpClient) |
| req-0003  | RISK-015 (ServiceException oculta detalhes), RISK-016 (Cadeia recursiva), RISK-017 (NotFoundException confundida), RISK-018 (Extension lança exceção) |
| req-0004  | RISK-025 (Alteração campos JSON), RISK-026 (Formato erro ViaCEP), RISK-027 (JsonOptions modificado), RISK-028 (String JSON longa) |
| req-0006  | RISK-019 (Exposição chave NuGet), RISK-020 (Build falha plataforma), RISK-021 (Versão incorreta), RISK-022 (Permissão workflow), RISK-023 (NuGet.org fora do ar), RISK-024 (Flaky tests) |

---

## 7. Acompanhamento e Revisão

- **Revisão programada:** a cada novo requisito ou release.
- **Próxima revisão:** v1.5.0 ou quando houver mudança em provedor externo.
- **Responsável:** Rodrigo Araujo Barbosa.

---

## 8. Clarificações e Notas

| Data       | Nota                                                                 |
|------------|----------------------------------------------------------------------|
| 23/07/2026 | Matriz inicial com 10 riscos identificados com base no código v1.4.0. |
| 23/07/2026 | RISK-006 (memory leak) já foi corrigido em versão anterior. Mantido como risco de regressão. |
| 23/07/2026 | RISK-013 e RISK-014 adicionados para cobrir riscos específicos do req-0002 (fallback multi-provedor). |
| 23/07/2026 | RISK-019 a RISK-024 adicionados para req-0006 (Publicação Automática no NuGet) — riscos de segurança, disponibilidade e qualidade do pipeline de CI/CD. |
| 23/07/2026 | RISK-025 a RISK-028 adicionados para req-0004 (Serialização JSON) — riscos de mudança de contrato de provedores, formato inesperado, configurações e segurança de memória. |
| 23/07/2026 | RISK-015 a RISK-018 adicionados para req-0003 (Exceções de Serviço) — riscos de perda de rastreabilidade, estouro de pilha, confusão de tipos de exceção e estabilidade dos métodos de extensão. |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
