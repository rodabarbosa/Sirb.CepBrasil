# Diagrama de Fluxo - Fallback de Busca de CEP

## Fluxo Principal com Fallback

```mermaid
flowchart TD
    A["🔍 Usuário: FindAsync(cep, token)"] --> B{"✓ CEP Válido?"}
    
    B -->|❌ Inválido| C["❌ Retorna erro<br/>CepResult { Success = false }"]
    B -->|✓ Válido| D["1️⃣ Tenta BrasilAPI"]
    
    D --> D1{"Resultado?"}
    D1 -->|✓ Encontrado| E["✅ Retorna sucesso<br/>CepResult { Success = true }"]
    D1 -->|⏱️ Erro/Timeout| F["2️⃣ Tenta ViaCEP"]
    D1 -->|❌ Não encontrado| F
    
    F --> F1{"Resultado?"}
    F1 -->|✓ Encontrado| E
    F1 -->|⏱️ Erro/Timeout| G["3️⃣ Tenta AwesomeAPI"]
    F1 -->|❌ Não encontrado| G
    
    G --> G1{"Resultado?"}
    G1 -->|✓ Encontrado| E
    G1 -->|⏱️ Erro/Timeout| H["4️⃣ Tenta OpenCEP"]
    G1 -->|❌ Não encontrado| H
    
    H --> H1{"Resultado?"}
    H1 -->|✓ Encontrado| E
    H1 -->|⏱️ Erro/Timeout| I{"Todos<br/>falharam?"}
    H1 -->|❌ Não encontrado| J["📭 Retorna null"]
    
    I -->|✓ Sim| K["🚨 Lança ServiceException"]
    I -->|❌ Não encontrado| J
    
    style A fill:#4CAF50,color:#fff
    style E fill:#2196F3,color:#fff
    style C fill:#f44336,color:#fff
    style J fill:#ff9800,color:#fff
    style K fill:#f44336,color:#fff
```

## Diagrama de Sequência - Caso de Sucesso

```mermaid
sequenceDiagram
    participant Cliente as 👤 Cliente
    participant Orquestrador as 🎯 Orquestrador
    participant BrasilAPI as 🌐 BrasilAPI
    participant ViaCEP as 🌐 ViaCEP
    
    Cliente ->> Orquestrador: FindAsync("01310100")
    
    Orquestrador ->> Orquestrador: Validar CEP ✓
    
    Orquestrador ->> BrasilAPI: GET /api/cep/v1/01310100
    BrasilAPI -->> Orquestrador: ✓ CepContainer
    
    Orquestrador -->> Cliente: ✓ CepResult { Success = true }
    
    Note over Orquestrador: ViaCEP NÃO foi tentado<br/>(BrasilAPI retornou sucesso)
```

## Diagrama de Sequência - Caso com Fallback

```mermaid
sequenceDiagram
    participant Cliente as 👤 Cliente
    participant Orquestrador as 🎯 Orquestrador
    participant BrasilAPI as 🌐 BrasilAPI
    participant ViaCEP as 🌐 ViaCEP
    participant AwesomeAPI as 🌐 AwesomeAPI
    
    Cliente ->> Orquestrador: FindAsync("01310100")
    
    Orquestrador ->> Orquestrador: Validar CEP ✓
    
    Orquestrador ->> BrasilAPI: GET /api/cep/v1/01310100
    BrasilAPI -->> Orquestrador: ⏱️ Timeout
    
    Note over Orquestrador: BrasilAPI falhou
    
    Orquestrador ->> ViaCEP: GET /ws/01310100/json
    ViaCEP -->> Orquestrador: ❌ Not Found
    
    Note over Orquestrador: ViaCEP não encontrou
    
    Orquestrador ->> AwesomeAPI: GET /api/cep/01310100
    AwesomeAPI -->> Orquestrador: ✓ CepContainer
    
    Orquestrador -->> Cliente: ✓ CepResult { Success = true }
    
    Note over Orquestrador: AwesomeAPI retornou sucesso
```

## Diagrama de Sequência - Falha Total

```mermaid
sequenceDiagram
    participant Cliente as 👤 Cliente
    participant Orquestrador as 🎯 Orquestrador
    participant BrasilAPI as 🌐 BrasilAPI
    participant ViaCEP as 🌐 ViaCEP
    participant AwesomeAPI as 🌐 AwesomeAPI
    participant OpenCEP as 🌐 OpenCEP
    
    Cliente ->> Orquestrador: FindAsync("01310100")
    
    Orquestrador ->> Orquestrador: Validar CEP ✓
    
    Orquestrador ->> BrasilAPI: GET /api/cep/v1/01310100
    BrasilAPI -->> Orquestrador: ⏱️ Timeout
    
    Orquestrador ->> ViaCEP: GET /ws/01310100/json
    ViaCEP -->> Orquestrador: 🔴 500 Error
    
    Orquestrador ->> AwesomeAPI: GET /api/cep/01310100
    AwesomeAPI -->> Orquestrador: ⏱️ Timeout
    
    Orquestrador ->> OpenCEP: GET /cep/01310100
    OpenCEP -->> Orquestrador: 🔴 500 Error
    
    Orquestrador -->> Cliente: 🚨 ServiceException
    
    Note over Orquestrador: Todos os 4 serviços falharam
```

## Arquitetura de Classes

```mermaid
classDiagram
    class ICepServiceControl {
        <<interface>>
        +FindAsync(cep: string, token: CancellationToken) Task~CepContainer~
    }
    
    class ICepService {
        <<interface>>
        +FindAsync(cep: string, token: CancellationToken) Task~CepResult~
    }
    
    class BrasilApiService {
        -HttpClient _httpClient
        +FindAsync(cep, token) Task~CepContainer~
        -BuildRequestUrl(cep) string
    }
    
    class ViaCepService {
        -HttpClient _httpClient
        +FindAsync(cep, token) Task~CepContainer~
        -BuildRequestUrl(cep) string
    }
    
    class AwesomeApiService {
        -HttpClient _httpClient
        +FindAsync(cep, token) Task~CepContainer~
        -BuildRequestUrl(cep) string
    }
    
    class OpenCepService {
        -HttpClient _httpClient
        +FindAsync(cep, token) Task~CepContainer~
        -BuildRequestUrl(cep) string
    }
    
    class CepServiceOrchestrator {
        -ICepServiceControl[] _services
        +FindAsync(cep, token) Task~CepResult~
        -TryService(service, cep, token) Task~CepContainer~
    }
    
    class CepResult {
        +Success: bool
        +CepContainer: CepContainer
        +Message: string
        +Exceptions: List~Exception~
    }
    
    class CepContainer {
        +Cep: string
        +Logradouro: string
        +Bairro: string
        +Cidade: string
        +Uf: string
    }
    
    ICepServiceControl <|.. BrasilApiService
    ICepServiceControl <|.. ViaCepService
    ICepServiceControl <|.. AwesomeApiService
    ICepServiceControl <|.. OpenCepService
    
    ICepService <|.. CepServiceOrchestrator
    
    CepServiceOrchestrator --> ICepServiceControl
    CepServiceOrchestrator --> CepResult
    
    CepResult --> CepContainer
```

## Estado Transitions - CepServiceOrchestrator

```mermaid
stateDiagram-v2
    [*] --> Validando
    
    Validando --> Erro: CEP Inválido
    Validando --> TentandoBrasilAPI: CEP Válido
    
    TentandoBrasilAPI --> Sucesso: Encontrado
    TentandoBrasilAPI --> TentandoViaCEP: Falha/Não Encontrado
    
    TentandoViaCEP --> Sucesso: Encontrado
    TentandoViaCEP --> TentandoAwesomeAPI: Falha/Não Encontrado
    
    TentandoAwesomeAPI --> Sucesso: Encontrado
    TentandoAwesomeAPI --> TentandoOpenCEP: Falha/Não Encontrado
    
    TentandoOpenCEP --> Sucesso: Encontrado
    TentandoOpenCEP --> Verificando: Falha/Não Encontrado
    
    Verificando --> FalhaTotal: Todos Falharam
    Verificando --> NaoEncontrado: Nenhum Encontrou
    
    Sucesso --> [*]
    Erro --> [*]
    FalhaTotal --> [*]
    NaoEncontrado --> [*]
```

## Matriz de Teste

```
┌─────────────────────────────────────────────────────────────────────┐
│ MATRIZ DE TESTE - FALLBACK DE CEP                                   │
├─────┬───────────┬──────────┬──────────┬──────────┬──────────────────┤
│ #   │ BrasilAPI │ ViaCEP   │ Awesome  │ OpenCEP  │ Resultado        │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 1   │ ✓ Sucesso │   -      │   -      │    -     │ ✅ Retorna       │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 2   │ ❌ Erro   │ ✓ Sucesso│   -      │    -     │ ✅ Retorna       │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 3   │ ❌ Erro   │ ❌ Erro  │ ✓ Sucesso│    -     │ ✅ Retorna       │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 4   │ ❌ Erro   │ ❌ Erro  │ ❌ Erro  │ ✓ Sucesso│ ✅ Retorna       │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 5   │ ❌ Erro   │ ❌ Erro  │ ❌ Erro  │ ❌ Erro  │ 🚨 ServiceExcept.│
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 6   │ ❌ N.Econ │ ❌ N.Econ│ ❌ N.Econ│ ❌ N.Econ│ 📭 Retorna null  │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 7   │ ⏱ Timeout │ ✓ Sucesso│   -      │    -     │ ✅ Retorna       │
├─────┼───────────┼──────────┼──────────┼──────────┼──────────────────┤
│ 8   │ ❌ 500    │ ❌ 503   │ ⏱ Timeout│ ✓ Sucesso│ ✅ Retorna       │
└─────┴───────────┴──────────┴──────────┴──────────┴──────────────────┘

Legenda:
✓ Sucesso      = Serviço retornou CepContainer
❌ Erro        = Serviço retornou erro HTTP (4xx/5xx)
❌ N.Econ      = Serviço retornou "não encontrado" (null)
⏱ Timeout      = Serviço expirou o timeout
-              = Serviço não foi tentado
✅ Retorna     = Retorna CepResult com sucesso
🚨 ServiceExcept. = Lança exceção
📭 Retorna null = Retorna null
```

## Otimizações e Considerações

### Performance

```mermaid
graph LR
    A["Requisição"] --> B["Valida CEP<br/>⚡ ~1ms"]
    B --> C["Tenta BrasilAPI<br/>⚡ ~100-500ms"]
    C -->|Sucesso| D["Retorna<br/>⚡ Imediato"]
    C -->|Falha| E["Tenta ViaCEP<br/>⚡ ~100-500ms"]
    E -->|Sucesso| D
    E -->|Falha| F["Próximas..."]
    
    style D fill:#2196F3,color:#fff
```

### Timeout Strategy

- **Default**: 30 segundos total
- **BrasilAPI**: 5 segundos
- **ViaCEP**: 5 segundos
- **AwesomeAPI**: 5 segundos
- **OpenCEP**: 5 segundos

Se um serviço não responde em tempo, passa para o próximo automaticamente.

---

**Versão**: 1.4.0  
**Data**: 2026-02-18
