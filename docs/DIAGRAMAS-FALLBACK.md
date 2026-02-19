# Diagramas de Fluxo - Fallback Strategy v1.4.0

## 🔄 Diagrama de Sequência - Busca com Fallback

```mermaid
sequenceDiagram
    participant User as Usuário
    participant Orq as Orquestrador<br/>(Fallback Manager)
    participant B1 as BrasilAPI
    participant B2 as ViaCEP
    participant B3 as AwesomeAPI
    participant B4 as OpenCEP

    User->>Orq: FindAsync("01310100")
    
    rect rgb(200, 220, 255)
    Note over Orq,B1: Tentativa 1: BrasilAPI
    Orq->>B1: GET /address/v2/01310100
    alt Sucesso - CEP Encontrado
        B1-->>Orq: ✓ CepContainer
        Orq-->>User: ✓ CepResult { Success=true }
    else Sucesso - CEP Não Encontrado
        B1-->>Orq: null
        Note over Orq: CEP não existe em BrasilAPI<br/>→ Tentar próximo serviço
    else Erro (timeout, 503, etc)
        B1-->>Orq: ✗ HttpRequestException
        Note over Orq: Erro em BrasilAPI<br/>→ Tentar próximo serviço
    end
    end

    rect rgb(220, 240, 220)
    Note over Orq,B2: Tentativa 2: ViaCEP
    Orq->>B2: GET /ws/01310100/json
    alt Sucesso - CEP Encontrado
        B2-->>Orq: ✓ CepContainer
        Orq-->>User: ✓ CepResult { Success=true }
    else Sucesso - CEP Não Encontrado
        B2-->>Orq: null
        Note over Orq: CEP não existe em ViaCEP<br/>→ Tentar próximo serviço
    else Erro (timeout, 503, etc)
        B2-->>Orq: ✗ HttpRequestException
        Note over Orq: Erro em ViaCEP<br/>→ Tentar próximo serviço
    end
    end

    rect rgb(240, 230, 200)
    Note over Orq,B3: Tentativa 3: AwesomeAPI
    Orq->>B3: GET /api/cep/01310100
    alt Sucesso - CEP Encontrado
        B3-->>Orq: ✓ CepContainer
        Orq-->>User: ✓ CepResult { Success=true }
    else Sucesso - CEP Não Encontrado
        B3-->>Orq: null
        Note over Orq: CEP não existe em AwesomeAPI<br/>→ Tentar próximo serviço
    else Erro (timeout, 503, etc)
        B3-->>Orq: ✗ HttpRequestException
        Note over Orq: Erro em AwesomeAPI<br/>→ Tentar próximo serviço
    end
    end

    rect rgb(240, 200, 200)
    Note over Orq,B4: Tentativa 4: OpenCEP (Última)
    Orq->>B4: GET /01310100
    alt Sucesso - CEP Encontrado
        B4-->>Orq: ✓ CepContainer
        Orq-->>User: ✓ CepResult { Success=true }
    else Sucesso - CEP Não Encontrado
        B4-->>Orq: null
        Note over Orq: CEP não existe em nenhum serviço<br/>→ Retornar null
        Orq-->>User: null
    else Erro (timeout, 503, etc)
        B4-->>Orq: ✗ HttpRequestException
        Note over Orq: Erro em OpenCEP (última tentativa)<br/>→ Lançar ServiceException
        Orq-->>User: ✗ ServiceException
    end
    end
```

---

## 🌳 Diagrama de Decisão - Lógica de Fallback

```mermaid
graph TD
    Start([Usuário chama FindAsync]) --> Validate{CEP é<br/>válido?}
    
    Validate -->|Não| Error1["🚫 Lançar<br/>ArgumentException"]
    Error1 --> End1([Retorna erro])
    
    Validate -->|Sim| Try1["🔍 Tentativa 1:<br/>BrasilAPI"]
    
    Try1 --> Check1{Resultado?}
    Check1 -->|Sucesso| Success["✅ Encontrou!"]
    Success --> RetSuccess["Retorna CepResult<br/>com dados"]
    RetSuccess --> End2([Fim - Sucesso])
    
    Check1 -->|Erro| Try2["🔍 Tentativa 2:<br/>ViaCEP"]
    Check1 -->|Não encontrado| Try2
    
    Try2 --> Check2{Resultado?}
    Check2 -->|Sucesso| Success
    Check2 -->|Erro| Try3["🔍 Tentativa 3:<br/>AwesomeAPI"]
    Check2 -->|Não encontrado| Try3
    
    Try3 --> Check3{Resultado?}
    Check3 -->|Sucesso| Success
    Check3 -->|Erro| Try4["🔍 Tentativa 4:<br/>OpenCEP"]
    Check3 -->|Não encontrado| Try4
    
    Try4 --> Check4{Resultado?}
    Check4 -->|Sucesso| Success
    
    Check4 -->|Não encontrado| RetNull["Retorna null<br/>(CEP não existe)"]
    RetNull --> End3([Fim - Não encontrado])
    
    Check4 -->|Erro| CountErrors{Todos os<br/>serviços<br/>tiveram<br/>erro?}
    
    CountErrors -->|Sim| RetException["🚫 Lançar<br/>ServiceException"]
    RetException --> End4([Fim - Erro])
    
    CountErrors -->|Não| RetNull
    
    style Start fill:#e1f5ff
    style Success fill:#c8e6c9
    style RetSuccess fill:#a5d6a7
    style End2 fill:#81c784
    style RetNull fill:#fff9c4
    style End3 fill:#fbc02d
    style RetException fill:#ffcdd2
    style End4 fill:#ef5350
    style Error1 fill:#ffcdd2
    style End1 fill:#ef5350
```

---

## 📊 Diagrama de Estados - Máquina de Estados do Orquestrador

```mermaid
stateDiagram-v2
    [*] --> Validando
    
    Validando --> ValidoErro: CEP inválido
    ValidoErro --> [*]: Lança ArgumentException
    
    Validando --> BrasilAPI: CEP válido
    
    BrasilAPI --> BrasilSucesso: Encontrou
    BrasilAPI --> BrasilErro: Erro/Timeout
    BrasilAPI --> BrasilNaoEnc: Não encontrado
    
    BrasilSucesso --> Retorno: ✓ Sucesso
    BrasilErro --> ViaCEP: Próximo
    BrasilNaoEnc --> ViaCEP: Próximo
    
    ViaCEP --> ViaSucesso: Encontrou
    ViaCEP --> ViaErro: Erro/Timeout
    ViaCEP --> ViaNaoEnc: Não encontrado
    
    ViaSucesso --> Retorno: ✓ Sucesso
    ViaErro --> AwesomeAPI: Próximo
    ViaNaoEnc --> AwesomeAPI: Próximo
    
    AwesomeAPI --> AWSucesso: Encontrou
    AwesomeAPI --> AWErro: Erro/Timeout
    AwesomeAPI --> AWNaoEnc: Não encontrado
    
    AWSucesso --> Retorno: ✓ Sucesso
    AWErro --> OpenCEP: Próximo
    AWNaoEnc --> OpenCEP: Próximo
    
    OpenCEP --> OpenSucesso: Encontrou
    OpenCEP --> OpenErro: Erro/Timeout
    OpenCEP --> OpenNaoEnc: Não encontrado
    
    OpenSucesso --> Retorno: ✓ Sucesso
    OpenErro --> TodosErro: Verificar erros
    OpenNaoEnc --> RetornoNull: ✓ Retorna null
    
    TodosErro --> LancaExc: Todos tiveram erro
    TodosErro --> RetornoNull: Alguns tiveram erro
    
    Retorno --> [*]: CepResult { Success=true }
    RetornoNull --> [*]: null
    LancaExc --> [*]: ServiceException
    
    style Validando fill:#e3f2fd
    style BrasilAPI fill:#e8f5e9
    style ViaCEP fill:#f3e5f5
    style AwesomeAPI fill:#fff3e0
    style OpenCEP fill:#fce4ec
    style Retorno fill:#c8e6c9
    style RetornoNull fill:#fff9c4
    style LancaExc fill:#ffcdd2
    style ValidoErro fill:#ffcdd2
```

---

## 🔗 Diagrama de Dependências e Registro de DI

```mermaid
graph LR
    subgraph "Implementações de ICepServiceControl"
        BA["BrasilApiService"]
        VC["ViaCepService"]
        AA["AwesomeApiService"]
        OC["OpenCepService"]
    end
    
    subgraph "Interfaces"
        ICSC["ICepServiceControl"]
        ICS["ICepService"]
    end
    
    subgraph "Orquestrador"
        ORQ["CepServiceOrchestrator"]
    end
    
    subgraph "DI Container"
        HTTP["HttpClientFactory"]
        LOGGER["ILogger"]
        SERVICES["ICepServiceControl[]"]
    end
    
    BA --> ICSC
    VC --> ICSC
    AA --> ICSC
    OC --> ICSC
    
    ORQ --> ICS
    ORQ --> SERVICES
    ORQ --> LOGGER
    
    BA --> HTTP
    VC --> HTTP
    AA --> HTTP
    OC --> HTTP
    
    SERVICES -.-> BA
    SERVICES -.-> VC
    SERVICES -.-> AA
    SERVICES -.-> OC
    
    style BA fill:#e8f5e9
    style VC fill:#f3e5f5
    style AA fill:#fff3e0
    style OC fill:#fce4ec
    style ORQ fill:#e3f2fd
    style ICSC fill:#fff9c4
    style ICS fill:#fff9c4
    style HTTP fill:#f5f5f5
    style LOGGER fill:#f5f5f5
```

---

## 📈 Fluxo de Erro e Exceções

```mermaid
graph TD
    Start([Chamada FindAsync]) --> Valid{Validação<br/>CEP}
    
    Valid -->|Falha| ArgEx["ArgumentNullException<br/>ou<br/>ArgumentException"]
    ArgEx --> Client["Cliente recebe erro<br/>imediatamente"]
    
    Valid -->|OK| Loop["Loop de Tentativas<br/>em 4 serviços"]
    
    Loop --> Try1["Tenta BrasilAPI"]
    Try1 --> R1{Resultado}
    R1 -->|Sucesso| Success["✓ Retorna dado"]
    R1 -->|Null| Try2["Tenta ViaCEP"]
    R1 -->|Erro| E1["Registra erro"]
    
    E1 --> Try2
    Try2 --> R2{Resultado}
    R2 -->|Sucesso| Success
    R2 -->|Null| Try3["Tenta AwesomeAPI"]
    R2 -->|Erro| E2["Registra erro"]
    
    E2 --> Try3
    Try3 --> R3{Resultado}
    R3 -->|Sucesso| Success
    R3 -->|Null| Try4["Tenta OpenCEP"]
    R3 -->|Erro| E3["Registra erro"]
    
    E3 --> Try4
    Try4 --> R4{Resultado}
    R4 -->|Sucesso| Success
    R4 -->|Null| NaoEnc["✓ Retorna null<br/>(Não encontrado)"]
    R4 -->|Erro| E4["Registra erro"]
    
    E4 --> Check{Todos os<br/>serviços<br/>tiveram<br/>erro?}
    
    Check -->|Sim| ServiceEx["ServiceException<br/>com lista de erros"]
    ServiceEx --> ClientErr["Cliente recebe<br/>exceção com<br/>detalhes de todos<br/>os erros"]
    
    Check -->|Não| NaoEnc
    
    Success --> ClientOK["Cliente recebe<br/>CepResult com<br/>Success=true"]
    NaoEnc --> ClientNull["Cliente recebe<br/>null"]
    
    style ArgEx fill:#ffcdd2
    style ServiceEx fill:#ffcdd2
    style Success fill:#c8e6c9
    style NaoEnc fill:#fff9c4
    style Client fill:#ffcdd2
    style ClientErr fill:#ffcdd2
    style ClientOK fill:#c8e6c9
    style ClientNull fill:#fff9c4
```

---

## ⏱️ Timeline de Timeout

```
Usuário chama FindAsync("01310100")
│
├─→ Validação: ~1ms
│
├─→ Tentativa 1 (BrasilAPI): 0-30s
│   ├─→ Se sucesso: retorna imediatamente
│   ├─→ Se timeout: passa para próximo
│   └─→ Se erro: passa para próximo
│
├─→ Tentativa 2 (ViaCEP): 0-30s
│   ├─→ Se sucesso: retorna imediatamente
│   ├─→ Se timeout: passa para próximo
│   └─→ Se erro: passa para próximo
│
├─→ Tentativa 3 (AwesomeAPI): 0-30s
│   ├─→ Se sucesso: retorna imediatamente
│   ├─→ Se timeout: passa para próximo
│   └─→ Se erro: passa para próximo
│
└─→ Tentativa 4 (OpenCEP): 0-30s
    ├─→ Se sucesso: retorna imediatamente
    ├─→ Se timeout: lança exceção
    └─→ Se erro: lança exceção ou retorna null

Total máximo: ~120 segundos (4 tentativas × 30s timeout)
Tempo típico: 200-500ms (primeira tentativa bem-sucedida)
Melhor caso: ~100ms (BrasilAPI responde rápido)
Pior caso: ~120s (todos falharem com timeout)
```

---

## 🔐 Tratamento de Segurança

```mermaid
graph TD
    Input["Input: CEP"] --> Clean["Limpar formatação<br/>01310-100 → 01310100"]
    Clean --> Valid["Validar formato<br/>✓ 8 dígitos numéricos<br/>✓ Sem caracteres especiais"]
    
    Valid -->|Inválido| Throw["Lançar<br/>ArgumentException"]
    Valid -->|Válido| Proceed["Prosseguir com<br/>tentativas"]
    
    Proceed --> Log["Log: Iniciando busca<br/>(sem dados sensíveis)"]
    Log --> Call["Chamar serviço<br/>com timeout<br/>e CancellationToken"]
    
    Call --> Response["Receber resposta"]
    Response --> Sanitize["Sanitizar resposta<br/>✓ Validar campos<br/>✓ Remover dados inesperados<br/>✓ Mapear para CepContainer"]
    
    Sanitize --> Return["Retornar de forma<br/>segura ao cliente"]
    
    style Input fill:#e3f2fd
    style Clean fill:#e8f5e9
    style Valid fill:#f3e5f5
    style Throw fill:#ffcdd2
    style Proceed fill:#c8e6c9
    style Log fill:#fff9c4
    style Call fill:#e3f2fd
    style Response fill:#e8f5e9
    style Sanitize fill:#f3e5f5
    style Return fill:#c8e6c9
```

---

## 📋 Tabela de Resposta por Cenário

| Cenário               | HTTP Status             | Body               | Comportamento         | Resultado          |
|-----------------------|-------------------------|--------------------|-----------------------|--------------------|
| CEP válido existente  | 200 OK                  | `{ ... }`          | Retorna imediatamente | ✓ CepResult        |
| CEP válido não existe | 404 Not Found           | `{}` ou `null`     | Tenta próximo         | → Próximo          |
| CEP inválido          | 400 Bad Request         | `{ error: "..." }` | Tenta próximo         | → Próximo          |
| Servidor indisponível | 503 Service Unavailable | -                  | Tenta próximo         | → Próximo          |
| Timeout               | (timeout)               | -                  | Tenta próximo         | → Próximo          |
| Conexão recusada      | (erro de conexão)       | -                  | Tenta próximo         | → Próximo          |
| Rate limit atingido   | 429 Too Many Requests   | -                  | Tenta próximo         | → Próximo          |
| Todos com erro        | Todos falharam          | -                  | Lança exceção         | ✗ ServiceException |
| Nenhum encontrou      | Todos retornaram vazio  | -                  | Retorna null          | null               |

---

**Versão:** 1.4.0  
**Data:** 2026-02-18  
**Status:** Diagramas Completos
