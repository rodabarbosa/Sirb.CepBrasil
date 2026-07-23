# Diagrama de Integração — Fluxo de Fallback Multi-Provedor

> **Propósito:** Documentar o fluxo de integração entre o orquestrador `CepService` e os 4 provedores externos, demonstrando a sequência de tentativas e as decisões de fallback.

| Campo           | Valor                    |
|-----------------|--------------------------|
| **Código**      | min-fluxo-fallback       |
| **Versão**      | v1.0                     |
| **Data**        | 23/07/2026               |
| **Autor**       | Rodrigo Araujo Barbosa   |
| **Status**      | `rascunho`               |

---

## Histórico de Alterações

| Data       | Autor                 | Versão | Descrição                        |
|------------|-----------------------|--------|----------------------------------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0   | Criação do diagrama de integração|

---

## Diagrama de Sequência — Fallback Completo

```mermaid
sequenceDiagram
    participant App as Aplicação Cliente
    participant CepSvc as CepService
    participant BrasilAPI as BrasilApiService
    participant ViaCEP as ViaCepService
    participant Awesome as AwesomeApiService
    participant OpenCEP as OpenCepService
    participant HTTP as HTTP Client

    App->>CepSvc: FindAsync("01310100", CancellationToken)

    Note over CepSvc: timeout 30s se token for None

    %% Tentativa 1: BrasilAPI
    CepSvc->>BrasilAPI: FindAsync(cep, effectiveToken)
    BrasilAPI->>BrasilAPI: CepValidation.Validate(cep)
    BrasilAPI->>BrasilAPI: cep.RemoveMask() → "01310100"
    BrasilAPI->>HTTP: GET https://brasilapi.com.br/api/cep/v1/01310100

    alt BrasilAPI → Sucesso
        HTTP-->>BrasilAPI: 200 OK + JSON
        BrasilAPI->>BrasilAPI: GetResponseAsync → BrasilApiResponse
        BrasilAPI->>BrasilAPI: Mapeia → CepContainer
        BrasilAPI-->>CepSvc: CepContainer (não nulo)
        CepSvc-->>App: CepResult { Success = true }
    else BrasilAPI → Falha
        HTTP--xBrasilAPI: Timeout / HTTP Error
        BrasilAPI--xCepSvc: Exception
        Note over CepSvc: Captura erro, continua loop
    end

    %% Tentativa 2: ViaCEP (se BrasilAPI falhou)
    alt BrasilAPI falhou
        CepSvc->>ViaCEP: FindAsync(cep, effectiveToken)
        ViaCEP->>ViaCEP: CepValidation.Validate(cep)
        ViaCEP->>ViaCEP: cep.RemoveMask() → "01310100"
        ViaCEP->>HTTP: GET https://viacep.com.br/ws/01310100/json

        alt ViaCEP → Sucesso
            HTTP-->>ViaCEP: 200 OK + JSON
            ViaCEP->>ViaCEP: GetResponseAsync → CepContainer
            ViaCEP->>ViaCEP: Verifica Erro != true
            ViaCEP-->>CepSvc: CepContainer
            CepSvc-->>App: CepResult { Success = true }
        else ViaCEP → Não encontrado
            HTTP-->>ViaCEP: 200 + {"erro": true}
            ViaCEP-->>CepSvc: null (Erro == true)
            Note over CepSvc: null → continua loop
        else ViaCEP → Erro
            HTTP--xViaCEP: Timeout / HTTP Error
            ViaCEP--xCepSvc: Exception
        end
    end

    %% Tentativa 3: AwesomeAPI
    alt ViaCEP falhou
        CepSvc->>Awesome: FindAsync(cep, effectiveToken)
        Awesome->>Awesome: CepValidation.Validate(cep)
        Awesome->>Awesome: cep.RemoveMask() → "01310100"
        Awesome->>HTTP: GET https://cep.awesomeapi.com.br/json/01310100

        alt AwesomeAPI → Sucesso
            HTTP-->>Awesome: 200 OK + JSON
            Awesome->>Awesome: GetResponseAsync → AwesomeApiResponse
            Awesome->>Awesome: Mapeia → CepContainer
            Awesome-->>CepSvc: CepContainer
            CepSvc-->>App: CepResult { Success = true }
        else AwesomeAPI → Falha
            HTTP--xAwesome: 404 / Timeout
            Awesome--xCepSvc: null / Exception
        end
    end

    %% Tentativa 4: OpenCEP
    alt AwesomeAPI falhou
        CepSvc->>OpenCEP: FindAsync(cep, effectiveToken)
        OpenCEP->>OpenCEP: CepValidation.Validate(cep)
        OpenCEP->>OpenCEP: cep.RemoveMask() → "01310100"
        OpenCEP->>HTTP: GET https://opencep.com/v1/01310100

        alt OpenCEP → Sucesso
            HTTP-->>OpenCEP: 200 OK + JSON
            OpenCEP->>OpenCEP: GetResponseAsync → CepContainer
            OpenCEP-->>CepSvc: CepContainer
            CepSvc-->>App: CepResult { Success = true }
        else OpenCEP → Falha
            HTTP--xOpenCEP: 404 / Timeout
            OpenCEP--xCepSvc: null / Exception
            Note over CepSvc: Última tentativa
        end
    end

    %% Resultado final
    Note over CepSvc: Loop encerrado
    alt Encontrou em algum provedor
        CepSvc-->>App: CepResult { Success = true, CepContainer }
    else Todos com erro (exception)
        CepSvc-->>App: CepResult { Success = false, Message = erros }
    else Nenhum encontrou (todos null)
        CepSvc-->>App: CepResult { Success = false, Message = "" }
    end
```

---

## Diagrama de Fluxo — Decisão de Fallback

```mermaid
flowchart TD
    Start(["FindAsync(cep, token)"]) --> Timeout{token == None?}
    Timeout -->|Sim| CriaCTS[Cria CTS timeout 30s]
    Timeout -->|Não| UsaToken[Usa token do usuário]
    CriaCTS --> LoopInicio
    UsaToken --> LoopInicio

    LoopInicio["Loop: _services[0..3]"] --> ProxServ[Próximo serviço]

    ProxServ --> Try[Tenta FindAsync]
    Try --> Check{Resultado}
    Check -->|CepContainer != null| Success["Retorna CepResult(true, data, null)"]
    Check -->|null| ContinueLoop{continue}
    Check -->|Exception| Catch[Catch + AllMessages]
    Catch --> ContinueLoop

    ContinueLoop --> HasNext{Ainda há<br/>serviços?}
    HasNext -->|Sim| ProxServ
    HasNext -->|Não| Fail["Retorna CepResult(false, null, message)"]

    Success --> End["Fim"]
    Fail --> End

    style Success fill:#a5d6a7,stroke:#388e3c
    style Fail fill:#ffab91,stroke:#d84315
    style LoopInicio fill:#e3f2fd,stroke:#1565c0
    style ProxServ fill:#f3e5f5,stroke:#7b1fa2
```

---

## Tabela de Comportamento por Cenário

| Cenário | BrasilAPI | ViaCEP | AwesomeAPI | OpenCEP | Resultado Final |
|---------|-----------|--------|------------|---------|-----------------|
| 1 — Sucesso imediato | ✅ Encontrou | — | — | — | `CepResult { Success = true }` |
| 2 — Fallback 1→2 | ❌ Erro | ✅ Encontrou | — | — | `CepResult { Success = true }` |
| 3 — Fallback 1→2→3 | ❌ Erro | ❌ Erro | ✅ Encontrou | — | `CepResult { Success = true }` |
| 4 — Fallback completo | ❌ Erro | ❌ Erro | ❌ Erro | ✅ Encontrou | `CepResult { Success = true }` |
| 5 — Todos com erro | ❌ Erro | ❌ Erro | ❌ Erro | ❌ Erro | `CepResult { Success = false, Message = erros }` |
| 6 — Nenhum encontrou | ❌ Null | ❌ Null | ❌ Null | ❌ Null | `CepResult { Success = false, Message = "" }` |
| 7 — Timeout no 1º | ⏱ Timeout | ✅ Encontrou | — | — | `CepResult { Success = true }` |

**Legenda:** ✅ = Sucesso | ❌ = Erro/Falha | ⏱ = Timeout | — = Não tentado

---

## Timeline de Timeout

```
Usuário chama FindAsync("01310100")
│
├─→ Tentativa 1 (BrasilAPI): 0-30s
│   ├─→ Sucesso → retorna imediatamente (~100-500ms)
│   ├─→ Timeout → passa para próximo
│   └─→ Erro → passa para próximo
│
├─→ Tentativa 2 (ViaCEP): 0-30s
│   ├─→ Sucesso → retorna imediatamente
│   ├─→ Timeout → passa para próximo
│   └─→ Erro → passa para próximo
│
├─→ Tentativa 3 (AwesomeAPI): 0-30s
│   ├─→ Sucesso → retorna imediatamente
│   ├─→ Timeout → passa para próximo
│   └─→ Erro → passa para próximo
│
└─→ Tentativa 4 (OpenCEP): 0-30s
    ├─→ Sucesso → retorna imediatamente
    ├─→ Erro → loop encerra
    └─→ Timeout → loop encerra

Tempo típico (sucesso):    100-500ms (apenas BrasilAPI)
Pior caso (4 timeouts):    ~120s
Cenário médio com fallback: 500ms-5s
```

---

## Descrição textual do fluxo

1. A aplicação cliente chama `CepService.FindAsync(cep, cancellationToken)`.
2. O `CepService` verifica se o `CancellationToken` é `CancellationToken.None`. Se sim, cria um `CancellationTokenSource` com timeout de 30 segundos para limitar a duração total da tentativa.
3. Inicia o loop sobre a lista de serviços `_services` na ordem: BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP.
4. Para cada serviço:
   - O serviço valida o CEP (`CepValidation.Validate`).
   - Remove a máscara do CEP (`RemoveMask`).
   - Constrói a URL específica do provedor.
   - Executa a requisição HTTP GET.
   - Se a resposta for um `CepContainer` não nulo: retorna imediatamente ao cliente com `CepResult { Success = true }`.
   - Se a resposta for nula: continua para o próximo serviço.
   - Se ocorrer uma exceção (timeout, HTTP error): captura a mensagem de erro, concatena e continua para o próximo.
5. Após o loop:
   - Se encontrou → `CepResult { Success = true }` com dados.
   - Se todos com erro → `CepResult { Success = false }` com mensagens consolidadas.
   - Se todos retornaram null → `CepResult { Success = false }` com mensagem vazia.

---

## Referências no código

| Componente | Arquivo | Linhas |
|------------|---------|--------|
| Orquestrador | `Sirb.CepBrasil/Services/CepService.cs` | 101-125 |
| Registro de serviços | `Sirb.CepBrasil/Services/CepService.cs` | 167-173 |
| BaseService HTTP | `Sirb.CepBrasil/Services/BaseService.cs` | 61-103 |
| Validação de CEP | `Sirb.CepBrasil/Validations/CepValidation.cs` | 45-80 |

---

## Rastreabilidade

| Item | Referência |
|------|------------|
| Requisito de negócio | `docs/requirement/req-0002-busca-endereco-fallback.md` |
| Requisito técnico | `docs/requirement/tec/tec-req-0002-busca-endereco-fallback.md` |
| Diagrama de classes | `docs/diagrams/dcl-estrutura-servicos.md` |

---

## Clarification Log

| Data | Pergunta | Resposta | Status | Origem | Impacto |
|------|----------|----------|--------|--------|---------|
| 23/07/2026 | N/A — Diagrama gerado a partir do código-fonte | — | N/A | Criação | Sem impacto |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
