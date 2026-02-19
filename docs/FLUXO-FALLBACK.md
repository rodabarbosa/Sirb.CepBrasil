# Novo Fluxo de Fallback - Sirb.CepBrasil v1.4.0

## 📋 Visão Geral

A versão 1.4.0 implementa uma **estratégia robusta de busca com fallback automático** entre 4 provedores de CEP públicos, garantindo alta disponibilidade e resiliência.

---

## 🔄 Fluxo de Fallback

```
Usuário busca CEP
  ↓
┌─────────────────────────────────┐
│ Tentativa 1: BrasilAPI          │
│ https://brasilapi.com.br/       │
└────────────┬────────────────────┘
             │
      ┌──────┴──────┐
      ↓             ↓
   Sucesso    Erro ou não encontrado
      │             │
      └────┬────────┘
           ↓
┌─────────────────────────────────┐
│ Tentativa 2: ViaCEP             │
│ https://viacep.com.br/          │
└────────────┬────────────────────┘
             │
      ┌──────┴──────┐
      ↓             ↓
   Sucesso    Erro ou não encontrado
      │             │
      └────┬────────┘
           ↓
┌─────────────────────────────────┐
│ Tentativa 3: AwesomeAPI         │
│ https://awesomeapi.com.br/      │
└────────────┬────────────────────┘
             │
      ┌──────┴──────┐
      ↓             ↓
   Sucesso    Erro ou não encontrado
      │             │
      └────┬────────┘
           ↓
┌─────────────────────────────────┐
│ Tentativa 4: OpenCEP            │
│ https://github.com/              │
│ filipedeschamps/cep-promise     │
└────────────┬────────────────────┘
             │
      ┌──────┴──────┐
      ↓             ↓
   Sucesso    Erro ou não encontrado
      │             │
      └────┬────────┘
           ↓
    Retorna null
    (CEP não existe)
```

---

## 📊 Tabela de Comportamento

| Cenário                              | Ação                                                |
|--------------------------------------|-----------------------------------------------------|
| **Encontrou em qualquer serviço**    | Retorna resultado imediatamente (não tenta próximo) |
| **CEP não encontrado no serviço**    | Tenta o próximo serviço                             |
| **Erro HTTP (timeout, 500, etc)**    | Tenta o próximo serviço                             |
| **Erro em todos os 4 serviços**      | Lança `ServiceException` com detalhes               |
| **Não encontrado em nenhum serviço** | Retorna `null`                                      |

---

## 🔧 Ordem de Prioridade

### Por quê essa ordem?

1. **BrasilAPI** 🥇 (Primeira)
    - Melhor custo-benefício
    - Boa disponibilidade
    - Resposta rápida

2. **ViaCEP** 🥈 (Segunda)
    - Serviço mais antigo e confiável
    - Mantido há muitos anos
    - Fallback seguro

3. **AwesomeAPI** 🥉 (Terceira)
    - Diversifica o provedor
    - Bom uptime

4. **OpenCEP** 🏅 (Quarta)
    - Última opção
    - Implementação alternativa
    - Sempre disponível como fallback final

---

## 📝 Exemplos de Fluxo

### Exemplo 1: Encontra em BrasilAPI ✅

```
Usuário busca: "01310100"
  ↓
BrasilAPI → Encontra → Retorna resultado
```

### Exemplo 2: BrasilAPI falha, ViaCEP encontra ✅

```
Usuário busca: "01310100"
  ↓
BrasilAPI → Falha (timeout)
  ↓
ViaCEP → Encontra → Retorna resultado
```

### Exemplo 3: CEP não existe em nenhum serviço

```
Usuário busca: "00000000"
  ↓
BrasilAPI → Não encontrado
  ↓
ViaCEP → Não encontrado
  ↓
AwesomeAPI → Não encontrado
  ↓
OpenCEP → Não encontrado
  ↓
Retorna null
```

### Exemplo 4: Todos os serviços estão down ❌

```
Usuário busca: "01310100"
  ↓
BrasilAPI → Erro (503 Service Unavailable)
  ↓
ViaCEP → Erro (Connection timeout)
  ↓
AwesomeAPI → Erro (503 Service Unavailable)
  ↓
OpenCEP → Erro (Connection timeout)
  ↓
Lança ServiceException com mensagem clara:
"Todos os serviços de CEP estão indisponíveis no momento"
```

---

## 🛠️ Arquitetura Técnica

### Classes a Serem Implementadas

```
Sirb.CepBrasil/
├── Services/
│   ├── BrasilApiService.cs          ← NOVO
│   ├── ViaCepService.cs             (existente, pode refatorar)
│   ├── AwesomeApiService.cs         ← NOVO
│   ├── OpenCepService.cs            ← NOVO
│   └── CepServiceOrchestrator.cs    ← NOVO (orquestra o fallback)
├── Interfaces/
│   ├── ICepService.cs               (existente)
│   └── ICepServiceControl.cs        (existente)
└── Models/
    ├── CepResult.cs                 (existente)
    └── CepContainer.cs              (existente)
```

### Interfaces

```csharp
// Interface que todos os serviços implementam
public interface ICepServiceControl
{
    /// <summary>
    /// Busca endereço pelo CEP de forma assíncrona.
    /// </summary>
    Task<CepContainer> FindAsync(string cep, CancellationToken cancellationToken);
}

// Interface pública da biblioteca
public interface ICepService
{
    /// <summary>
    /// Busca endereço pelo CEP com estratégia de fallback automático.
    /// </summary>
    Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken);
}
```

---

## 🧪 Estratégia de Testes

### Cobertura Esperada: 100%

Cada serviço deve ter testes para:

1. **Sucesso (Happy Path)**
    - CEP válido e encontrado
    - Resultado contém dados corretos

2. **Não Encontrado**
    - CEP válido mas não existe
    - Deve retornar `null`

3. **Erro HTTP**
    - Timeout
    - 503 Service Unavailable
    - 500 Internal Server Error
    - Deve lançar exceção apropriada

4. **CancellationToken**
    - Deve respeitar token de cancelamento
    - Deve lançar OperationCanceledException

5. **Edge Cases**
    - CEP vazio/nulo
    - CEP inválido
    - Timeout customizado

### Nomenclatura de Testes

```csharp
[Fact(DisplayName = "Deve retornar sucesso quando CEP é válido e existe")]
public async Task FindAsync_QuandoCepValido_DeveRetornarSucesso()

[Fact(DisplayName = "Deve retornar null quando CEP não é encontrado")]
public async Task FindAsync_QuandoCepNaoEncontrado_DeveRetornarNull()

[Fact(DisplayName = "Deve lançar exceção quando serviço falha")]
public async Task FindAsync_QuandoServicoFalha_DeveLancarExcecao()

[Fact(DisplayName = "Deve cancelar operação quando CancellationToken é acionado")]
public async Task FindAsync_QuandoCanceladoComToken_DeveLancarOperationCanceledException()
```

---

## 📚 Documentação XML Obrigatória

Todos os métodos públicos devem ter documentação completa:

```csharp
/// <summary>
/// Busca informações de endereço através do CEP fornecido.
/// </summary>
/// <param name="cep">CEP a ser consultado (formato: 00000000 ou 00000-000)</param>
/// <param name="cancellationToken">Token para cancelamento da operação. Padrão: 30 segundos</param>
/// <returns>
/// Retorna um objeto <see cref="CepResult"/> contendo:
/// - Success: true se encontrou o endereço
/// - CepContainer: dados do endereço encontrado
/// - Message: mensagem de erro (se houver)
/// </returns>
/// <exception cref="ArgumentNullException">Quando o CEP é nulo</exception>
/// <exception cref="ArgumentException">Quando o CEP está em formato inválido</exception>
/// <exception cref="ServiceException">Quando todos os serviços falham</exception>
/// <example>
/// <code>
/// var service = new CepService();
/// var result = await service.FindAsync("01310100", CancellationToken.None);
/// if (result.Success)
/// {
///     Console.WriteLine($"Endereço: {result.CepContainer.Logradouro}");
/// }
/// else if (result == null)
/// {
///     Console.WriteLine("CEP não encontrado");
/// }
/// </code>
/// </example>
public async Task<CepResult> FindAsync(string cep, CancellationToken cancellationToken)
```

---

## ⚡ Comportamento Esperado

### Sucesso

```csharp
var result = await cepService.FindAsync("01310100", CancellationToken.None);

// Result:
// {
//   Success: true,
//   CepContainer: {
//     Cep: "01310-100",
//     Logradouro: "Avenida Paulista",
//     Bairro: "Bela Vista",
//     Cidade: "São Paulo",
//     Estado: "SP"
//   },
//   Message: null
// }
```

### CEP Não Encontrado

```csharp
var result = await cepService.FindAsync("00000000", CancellationToken.None);

// Result: null
```

### Erro em Todos os Serviços

```csharp
var result = await cepService.FindAsync("01310100", CancellationToken.None);

// Lança: ServiceException
// Mensagem: "Todos os serviços de CEP estão indisponíveis no momento"
```

---

## 🔐 Segurança e Resiliência

### Timeouts

- Default: 30 segundos por tentativa
- Total da operação: ~120 segundos (4 tentativas × 30s)

### Rate Limiting

- Implementar rate limit na orquestração
- Respeitar limites de cada API

### Logging

- Log de cada tentativa
- Log de sucesso/falha
- Tempo de resposta

### Error Handling

- Tratamento específico por tipo de erro
- Mensagens amigáveis ao usuário
- Stacktrace apenas em logs

---

## ✅ Checklist de Implementação

- [ ] BrasilApiService implementado e testado
- [ ] ViaCepService refatorado (se necessário)
- [ ] AwesomeApiService implementado e testado
- [ ] OpenCepService implementado e testado
- [ ] CepServiceOrchestrator implementado com lógica de fallback
- [ ] 100% de cobertura de testes para cada serviço
- [ ] XML documentation completa em todos os métodos públicos
- [ ] Suporte a CancellationToken em todos os métodos async
- [ ] Testes com DisplayName obrigatório
- [ ] README.md atualizado com novo fluxo
- [ ] Compatibilidade com .NET 8, 9, 10

---

## 📞 Referências

- [BrasilAPI](https://brasilapi.com.br/)
- [ViaCEP](https://viacep.com.br/)
- [AwesomeAPI](https://awesomeapi.com.br/)
- [OpenCEP](https://github.com/filipedeschamps/cep-promise)

---

**Versão:** 1.4.0  
**Data:** 2026-02-18  
**Status:** Documentado e Pronto para Implementação
