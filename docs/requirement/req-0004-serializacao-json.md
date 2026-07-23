# req-0004 — Serialização/Desserialização JSON

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | req-0004                 |
| **Título**           | Serialização/Desserialização JSON |
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

Este documento especifica o requisito de **Serialização e Desserialização JSON** da biblioteca Sirb.CepBrasil. Define os métodos de extensão `ToJson()` e `FromJson()` da classe `JsonExtension`, bem como os modelos de resposta (`BrasilApiResponse`, `AwesomeApiResponse`) e o conversor customizado `BooleanOrStringConverter` para o campo `erro` do ViaCEP. Garante que toda comunicação com provedores externos utilize serialização JSON confiável, eficiente e consistente.

### 1.2 Escopo

**In scope:**

- `JsonExtension.ToJson()` — serialização de objetos para JSON com configurações padronizadas
- `JsonExtension.FromJson<T>()` — desserialização de JSON para tipos específicos com configurações padronizadas
- Configurações globais de serialização (`JsonSerializerOptions`): `PropertyNameCaseInsensitive`, `DefaultIgnoreCondition`, `PropertyNamingPolicy`
- `BrasilApiResponse` — modelo de resposta para BrasilAPI com mapeamento `JsonPropertyName`
- `AwesomeApiResponse` — modelo de resposta para AwesomeAPI com mapeamento `JsonPropertyName`
- `BooleanOrStringConverter` — conversor JSON customizado para o campo `erro` do ViaCEP (booleano ou string)
- Mapeamento dos campos dos provedores para o modelo unificado `CepContainer`
- Tratamento de erros de serialização/desserialização com `InvalidOperationException`
- Validação de entrada: `ArgumentNullException` para parâmetros nulos, `ArgumentException` para strings vazias

**Out of scope:**

- Serialização de tipos complexos com referências circulares
- Suporte a formatos alternativos (XML, YAML, MessagePack)
- Cache de instâncias `JsonSerializerOptions` em memória compartilhada cross-AppDomain
- Serialização com formatação indentada (`WriteIndented`)
- Contratos de serialização via `System.Runtime.Serialization`
- Internacionalização de mensagens de erro de serialização
- Serialização de exceções ou cenários cross-AppDomain (já coberto por `[Serializable]` no req-0003)
- Documentação dos endpoints de API (já coberta pelo req-0002)

### 1.3 Glossário

| Termo | Definição |
|-------|-----------|
| `JsonExtension` | Classe estática com métodos de extensão `ToJson()` e `FromJson<T>()` |
| `ToJson()` | Método de extensão que serializa qualquer objeto para string JSON |
| `FromJson<T>()` | Método de extensão que desserializa string JSON para o tipo especificado |
| `JsonSerializerOptions` | Configurações de serialização do `System.Text.Json` |
| `PropertyNameCaseInsensitive` | Configuração que ignora diferenças maiúsculas/minúsculas nos nomes das propriedades JSON |
| `DefaultIgnoreCondition` | Configuração que ignora propriedades nulas durante serialização |
| `PropertyNamingPolicy` | Política de nomenclatura (camelCase) para nomes de propriedades no JSON |
| `JsonPropertyName` | Atributo que mapeia o nome da propriedade JSON para a propriedade C# |
| `BrasilApiResponse` | Modelo de resposta específico para o provedor BrasilAPI |
| `AwesomeApiResponse` | Modelo de resposta específico para o provedor AwesomeAPI |
| `BooleanOrStringConverter` | Conversor JSON customizado que aceita valores booleanos ou string para campo `erro` |
| `CepContainer` | Modelo unificado de endereço utilizado por todos os provedores |
| Round-trip | Ciclo completo de serialização seguido de desserialização |

---

## 2. Intenção / Motivação de Negócio (Por quê)

### Problema de negócio real sendo resolvido

A biblioteca Sirb.CepBrasil comunica-se com 4 provedores públicos de CEP (BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP), cada um com formato JSON próprio — nomes de campos diferentes (`street` vs. `address` vs. `logradouro`), estruturas distintas e comportamentos de erro variados (ViaCEP retorna `{"erro": true}` enquanto outros retornam HTTP 404). Sem um sistema padronizado e robusto de serialização JSON, cada provedor exigiria tratamento ad hoc, aumentando a complexidade do código, o risco de falhas de desserialização e o custo de manutenção.

### Por que importa (impacto mensurável)

- **Consistência entre provedores:** `JsonExtension` unifica as configurações de serialização (`camelCase`, case-insensitive, ignorar nulos) garantindo que todos os provedores usem o mesmo comportamento, independentemente do formato de resposta.
- **Resiliência a mudanças:** `PropertyNameCaseInsensitive = true` protege contra variações de capitalização nos nomes de campos retornados pelos provedores — um provedor que mude de `"Street"` para `"street"` não quebrará a desserialização.
- **Diagnóstico:** Mensagens de erro contextualizadas (`"Erro ao serializar objeto do tipo 'X' para JSON."`) permitem identificar rapidamente qual modelo falhou, sem necessidade de depuração profunda.
- **Eficiência:** Instância única e reutilizável de `JsonSerializerOptions` elimina overhead de recriação a cada operação de serialização.
- **Segurança:** `ArgumentNullException.ThrowIfNull()` bloqueia entradas nulas antes do processamento, evitando `NullReferenceException` em tempo de execução.

### Quem é afetado

- **Desenvolvedores clientes** da biblioteca que serializam/desserializam modelos de CEP (ex.: converter `CepContainer` para JSON para enviar a uma API própria).
- **Equipe de manutenção** que precisa adicionar novos provedores ou atualizar modelos existentes quando os contratos JSON dos provedores mudam.
- **Sistemas consumidores** que dependem da estabilidade dos modelos de resposta e do comportamento de serialização.

### Critério de sucesso mensurável

- **2 métodos de extensão** (`ToJson()`, `FromJson<T>()`) com validação de entrada e tratamento de erro.
- **3 modelos de resposta** mapeados corretamente (`BrasilApiResponse`, `AwesomeApiResponse`, `CepContainer` com `BooleanOrStringConverter`).
- **22+ testes unitários** com 100% de cobertura na classe `JsonExtension`.
- **0 falhas de desserialização** com variações de capitalização (case-insensitive) testadas.
- **Instância única** de `JsonSerializerOptions` reutilizada em toda a biblioteca.

---

## 3. Descrição Geral

### 3.1 Missão da Funcionalidade

Prover um sistema de serialização JSON padronizado, robusto e eficiente que garanta a comunicação confiável entre a biblioteca e os 4 provedores externos de CEP, com modelos de resposta específicos por provedor e um formato unificado de endereço (`CepContainer`).

### 3.2 Atores

| Ator | Descrição |
|------|-----------|
| Sistema consumidor | Aplicação cliente que utiliza a biblioteca e chama métodos de serialização |
| Provedor externo | API pública (BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP) que retorna JSON |
| Desenvolvedor | Profissional que integra a biblioteca e precisa entender os modelos de dados |
| Equipe de manutenção | Responsável por adicionar/atualizar provedores e modelos de resposta |

### 3.3 Contexto de Uso

A serialização JSON é utilizada nos seguintes contextos:

1. **Requisição HTTP (leitura):** `BaseService.GetResponseAsync<T>()` utiliza `ReadFromJsonAsync<T>()` com as configurações do `System.Text.Json` para desserializar respostas dos provedores.
2. **Conversão de modelos:** Cada serviço converte a resposta específica do provedor (`BrasilApiResponse`, `AwesomeApiResponse`) para o modelo unificado `CepContainer`.
3. **Uso público (cliente):** O consumidor da biblioteca pode serializar `CepContainer` ou `CepResult` para JSON via `ToJson()` para integrar com seus próprios sistemas.
4. **Tratamento de erro ViaCEP:** `BooleanOrStringConverter` lida com o campo `erro` do ViaCEP, que pode vir como booleano (`true`) ou string (`"true"`).

---

## 4. Artefatos Relacionados

### Documentos que impactam este artefato

- `docs/constituicao.md` (qualidade, segurança, tratamento de erros)
- `docs/architecture-tech-stack.md` (stack .NET, `System.Text.Json`)
- `docs/system-mapping.md` (estrutura do sistema)
- `docs/requirement/req-0002-busca-endereco-fallback.md` (APIs dos provedores que consomem os modelos)
- `docs/requirement/req-0003-excecoes-servico.md` (exceções usadas no tratamento de erros de serialização)
- `docs/extra/MELHORIAS-JSONEXTENSION.md` (documento de melhorias do JsonExtension)
- `docs/system-risk-matrix.md` (matriz de risco global)

### Documentos impactados por este artefato

- `docs/requirement/tec/tec-req-0004-serializacao-json.md`
- `docs/requirement/apf/apf-req-0004.md`
- `docs/requirement/analises/analise-req-0004.md`
- `docs/requirement/checklist/checklist-req-0004.md`
- `docs/requirement/did/did-models-resposta.md`
- `docs/system-risk-matrix.md`

### Componentes técnicos relacionados

| Componente | Caminho | Função |
|------------|---------|--------|
| `JsonExtension` | `Sirb.CepBrasil/Extensions/JsonExtension.cs` | Métodos de extensão `ToJson()` e `FromJson<T>()` |
| `BrasilApiResponse` | `Sirb.CepBrasil/Models/BrasilApiResponse.cs` | Modelo de resposta da BrasilAPI |
| `AwesomeApiResponse` | `Sirb.CepBrasil/Models/AwesomeApiResponse.cs` | Modelo de resposta da AwesomeAPI |
| `CepContainer` | `Sirb.CepBrasil/Models/CepContainer.cs` | Modelo unificado com `BooleanOrStringConverter` |
| `CepResult` | `Sirb.CepBrasil/Models/CepResult.cs` | Container de resultado (serializável via `ToJson()`) |
| `BaseService` | `Sirb.CepBrasil/Services/BaseService.cs` | Consome `ReadFromJsonAsync<T>()` para desserialização |
| `JsonExtensionTest` | `Sirb.CepBrasil.Test/Extensions/JsonExtensionTest.cs` | Testes unitários (22+ testes) |

---

## 5. Requisitos Funcionais

| ID | Descrição | Prioridade | Regra de negócio associada |
|----|-----------|------------|----------------------------|
| RF-26 | Implementar `JsonExtension.ToJson()` para serialização de objetos | Alta | RN-026, RN-027 |
| RF-27 | Implementar `JsonExtension.FromJson<T>()` para desserialização de strings JSON | Alta | RN-026, RN-028 |
| RF-28 | Configurar `JsonSerializerOptions` com `PropertyNameCaseInsensitive = true`, `DefaultIgnoreCondition.WhenWritingNull` e `PropertyNamingPolicy.CamelCase` | Alta | RN-026 |
| RF-29 | Definir `BrasilApiResponse` com mapeamento `JsonPropertyName` para campos da BrasilAPI | Alta | RN-029 |
| RF-30 | Definir `AwesomeApiResponse` com mapeamento `JsonPropertyName` para campos da AwesomeAPI | Alta | RN-029 |
| RF-31 | Implementar `BooleanOrStringConverter` para suportar campo `erro` como booleano ou string | Alta | RN-030 |
| RF-32 | Validar entrada nula em `ToJson()` e `FromJson<T>()` com `ArgumentNullException` | Alta | RN-027, RN-028 |
| RF-33 | Validar string vazia ou espaços em branco em `FromJson<T>()` com `ArgumentException` | Alta | RN-028 |
| RF-34 | Tratar erros de serialização com `InvalidOperationException` e mensagem contextualizada | Média | RN-031 |
| RF-35 | Reutilizar instância única de `JsonSerializerOptions` para toda a biblioteca | Alta | RN-032 |

---

## 6. Regras de Negócio

| ID | Regra | Origem | Impacto |
|----|-------|--------|---------|
| RN-026 | Toda serialização/desserialização deve usar as mesmas configurações padronizadas: camelCase, case-insensitive e ignorar nulos | Consistência entre provedores | Comportamento uniforme em toda a biblioteca |
| RN-027 | `ToJson()` deve lançar `ArgumentNullException` se o objeto for nulo e `InvalidOperationException` se a serialização falhar | Validação de entrada | Proteção contra NullReferenceException |
| RN-028 | `FromJson<T>()` deve lançar `ArgumentNullException` se a string for nula, `ArgumentException` se for vazia, e `InvalidOperationException` se a desserialização falhar | Validação de entrada | Rejeição de dados inválidos antes do processamento |
| RN-029 | Cada provedor deve ter seu próprio modelo de resposta com `JsonPropertyName` mapeando os campos exatos do JSON retornado | Isolamento por provedor | Modelos independentes facilitam manutenção |
| RN-030 | O campo `erro` do ViaCEP deve aceitar tanto booleano (`true`/`false`) quanto string (`"true"`/`"false"`) via `BooleanOrStringConverter` | Compatibilidade com formato variável do provedor | Robustez contra variações de tipo |
| RN-031 | Erros de serialização/desserialização devem ser encapsulados em `InvalidOperationException` com o tipo envolvido na mensagem | Diagnóstico | Mensagens contextualizadas facilitam depuração |
| RN-032 | A instância de `JsonSerializerOptions` deve ser única, estática e readonly para toda a biblioteca | Performance | Elimina overhead de recriação |

---

## 7. Critérios de Aceitação (Gherkin)

```gherkin
Funcionalidade: Serialização/Desserialização JSON
  Como um sistema consumidor da biblioteca
  Quero serializar e desserializar objetos JSON de forma confiável
  Para integrar dados de CEP com meus sistemas

  Cenário: Serialização de CepContainer para JSON
    Dado que um CepContainer válido com CEP "01310-100", UF "SP", cidade "São Paulo" é fornecido
    Quando o método ToJson() é executado
    Então o resultado deve ser uma string JSON válida
    E os nomes das propriedades devem estar em camelCase (ex.: "localidade", "logradouro")
    E propriedades nulas devem ser omitidas

  Cenário: Serialização com objeto nulo lança exceção
    Dado que o objeto é null
    Quando o método ToJson() é executado
    Então deve ser lançada ArgumentNullException

  Cenário: Serialização com erro interno lança InvalidOperationException
    Dado que um objeto que causa erro de serialização é fornecido
    Quando o método ToJson() é executado
    Então deve ser lançada InvalidOperationException
    E a mensagem deve conter o nome do tipo do objeto

  Cenário: Desserialização de JSON válido para CepContainer
    Dado que uma string JSON válida é fornecida
    Quando o método FromJson<CepContainer>() é executado
    Então o resultado deve ser um CepContainer com os dados corretos

  Cenário: Desserialização case-insensitive
    Dado que uma string JSON com nomes de propriedades em maiúsculas (ex.: "LOGRADOURO") é fornecida
    Quando o método FromJson<CepContainer>() é executado
    Então a desserialização deve ser bem-sucedida
    E os valores devem ser atribuídos corretamente

  Cenário: Desserialização com null ignora propriedades ausentes
    Dado que uma string JSON sem algumas propriedades opcionais é fornecida
    Quando o método FromJson<CepContainer>() é executado
    Então as propriedades ausentes devem ficar como null
    E a desserialização não deve lançar exceção

  Cenário: Desserialização com string nula lança exceção
    Dado que a string JSON é null
    Quando o método FromJson<CepContainer>() é executado
    Então deve ser lançada ArgumentNullException

  Cenário: Desserialização com string vazia lança exceção
    Dado que a string JSON é vazia ("")
    Quando o método FromJson<CepContainer>() é executado
    Então deve ser lançada ArgumentException

  Cenário: Desserialização de JSON inválido lança exceção
    Dado que a string JSON é malformada
    Quando o método FromJson<CepContainer>() é executado
    Então deve ser lançada InvalidOperationException
    E a mensagem deve conter o nome do tipo destino

  Cenário: Round-trip CepContainer (serializar e desserializar)
    Dado que um CepContainer com dados completos é criado
    Quando ele é serializado com ToJson() e depois desserializado com FromJson<CepContainer>()
    Então o resultado deve ser um CepContainer com os mesmos valores originais

  Cenário: BrasilApiResponse com mapeamento correto
    Dado que um JSON da BrasilAPI é recebido com campos "cep", "state", "city", "neighborhood", "street"
    Quando o método FromJson<BrasilApiResponse>() é executado
    Então as propriedades Cep, Uf, Cidade, Bairro, Logradouro devem ser preenchidas corretamente

  Cenário: AwesomeApiResponse com mapeamento correto
    Dado que um JSON da AwesomeAPI é recebido com campos "cep", "state", "city", "district", "address"
    Quando o método FromJson<AwesomeApiResponse>() é executado
    Então as propriedades Cep, Uf, Cidade, Bairro, Logradouro devem ser preenchidas corretamente

  Cenário: BooleanOrStringConverter aceita booleano verdadeiro
    Dado que o JSON contém "erro": true
    Quando o FromJson<CepContainer>() é executado
    Então a propriedade Erro deve ser true

  Cenário: BooleanOrStringConverter aceita string "true"
    Dado que o JSON contém "erro": "true"
    Quando o FromJson<CepContainer>() é executado
    Então a propriedade Erro deve ser true

  Cenário: BooleanOrStringConverter retorna null para valor inválido
    Dado que o JSON contém "erro": "invalido"
    Quando o FromJson<CepContainer>() é executado
    Então a propriedade Erro deve ser null
```

---

## 8. Requisitos Não Funcionais (NFRs)

| Categoria | Requisito | Métrica/Critério |
|-----------|-----------|------------------|
| Desempenho | `ToJson()` deve executar em < 10ms para objetos típicos (CepContainer, CepResult) | Tempo de execução < 10ms |
| Desempenho | Instância de `JsonSerializerOptions` deve ser reutilizada (não recriada por chamada) | Uma única instância estática |
| Segurança | Entradas nulas/vazias devem ser rejeitadas antes do processamento | `ArgumentNullException` ou `ArgumentException` |
| Manutenibilidade | 100% de cobertura de testes no `JsonExtension` | Cobertura medida via Coverlet |
| Manutenibilidade | 100% de documentação XML em inglês nos modelos e extensões | Tags summary, param, returns, exception, example |
| Compatibilidade | Funcionar em .NET 8, 9 e 10 | Build multi-target no CI |
| Confiabilidade | Case-insensitive: desserialização deve funcionar independente da capitalização dos campos JSON | `PropertyNameCaseInsensitive = true` |
| Confiabilidade | Propriedades nulas no objeto de origem não devem aparecer no JSON de saída | `DefaultIgnoreCondition.WhenWritingNull` |

---

## 9. Wireframe

**Não aplicável.** Projeto é uma biblioteca .NET sem interface de usuário. Nenhum wireframe ou padrão visual é necessário.

```
⏳ PENDENTE [Fase 3 — Design] — N/A: Biblioteca .NET sem interface de usuário.
```

---

## 10. Integração de API / Banco de Dados

**Não aplicável diretamente.** A serialização JSON é utilizada **durante** a comunicação com APIs externas (documentadas no req-0002). Os modelos de resposta (`BrasilApiResponse`, `AwesomeApiResponse`) são consumidos pelos serviços `BrasilApiService`, `AwesomeApiService`, `ViaCepService` e `OpenCepService`.

```
⏳ PENDENTE [Fase 3 — Dados] — Ver req-0002 para documentação das APIs dos provedores.
```

---

## 11. Matriz de Risco do Requisito (Recorte Local)

| ID | Requisito | Descrição do Risco | Probabilidade | Impacto | Score | Nível | Mitigação | Status |
|----|-----------|-------------------|---------------|---------|-------|-------|-----------|--------|
| RSK-019 | req-0004 | Provedor externo altera nomes de campos JSON sem aviso, quebrando desserialização (ex.: "street" → "logradouro") | M (3) — Média | A (4) — Alto | 12 | 🟠 Alto | Modelos separados por provedor; `PropertyNameCaseInsensitive`; fallback tenta outros provedores se um quebrar | 🟡 Mitigado |
| RSK-020 | req-0004 | ViaCEP retorna campo "erro" em formato não suportado (número, objeto), causando falha de desserialização | B (2) — Baixa | M (3) — Médio | 6 | 🟡 Médio | `BooleanOrStringConverter` trata booleano e string; valores não suportados retornam null | 🟢 Aceito |
| RSK-021 | req-0004 | String JSON muito longa causa estouro de memória ou timeout na desserialização | B (2) — Baixa | B (2) — Baixo | 4 | 🟢 Baixo | `System.Text.Json` tem limites internos de profundidade e tamanho; entrada vem de HTTP com timeout | 🟢 Aceito |
| RSK-022 | req-0004 | `JsonSerializerOptions` editado acidentalmente em runtime por código cliente (referência compartilhada) | MB (1) — Muito Baixa | M (3) — Médio | 3 | 🟢 Baixo | Instância é `private static readonly` — imutável externamente; código cliente não tem acesso ao campo | 🟢 Aceito |

---

## 12. Análise de Pontos de Função (APF) Vinculada

- **Documento APF:** `docs/requirement/apf/apf-req-0004.md`
- **Total estimado:** ver documento APF específico.

---

## 13. Impactos Técnicos

### Backend / Código

- `Sirb.CepBrasil/Extensions/JsonExtension.cs` — métodos `ToJson()` e `FromJson<T>()`
- `Sirb.CepBrasil/Models/BrasilApiResponse.cs` — modelo de resposta BrasilAPI
- `Sirb.CepBrasil/Models/AwesomeApiResponse.cs` — modelo de resposta AwesomeAPI
- `Sirb.CepBrasil/Models/CepContainer.cs` — modelo unificado com `BooleanOrStringConverter`
- `Sirb.CepBrasil/Services/BaseService.cs` — consome `ReadFromJsonAsync<T>()`
- `Sirb.CepBrasil/Services/BrasilApiService.cs` — converte `BrasilApiResponse` → `CepContainer`
- `Sirb.CepBrasil/Services/AwesomeApiService.cs` — converte `AwesomeApiResponse` → `CepContainer`
- `Sirb.CepBrasil/Services/ViaCepService.cs` — desserializa diretamente `CepContainer` (usa campo `erro`)
- `Sirb.CepBrasil/Services/OpenCepService.cs` — desserializa diretamente `CepContainer`

### Frontend

- N/A — biblioteca .NET sem UI

### Banco de Dados

- N/A — sem persistência

### Integrações Externas

- `BrasilAPI` — JSON mapeado via `BrasilApiResponse`
- `AwesomeAPI` — JSON mapeado via `AwesomeApiResponse`
- `ViaCEP` — JSON mapeado diretamente para `CepContainer` (usa `BooleanOrStringConverter`)
- `OpenCEP` — JSON mapeado diretamente para `CepContainer`

### Design / UX

- N/A — sem interface de usuário

---

## 14. Rastreabilidade

| Item | Referência |
|------|------------|
| Código — JsonExtension | `Sirb.CepBrasil/Extensions/JsonExtension.cs` (linhas 1-94) |
| Código — BrasilApiResponse | `Sirb.CepBrasil/Models/BrasilApiResponse.cs` (linhas 1-61) |
| Código — AwesomeApiResponse | `Sirb.CepBrasil/Models/AwesomeApiResponse.cs` (linhas 1-60) |
| Código — CepContainer | `Sirb.CepBrasil/Models/CepContainer.cs` (linhas 1-184) |
| Código — CepResult | `Sirb.CepBrasil/Models/CepResult.cs` (linhas 1-77) |
| Código — BaseService | `Sirb.CepBrasil/Services/BaseService.cs` (linhas 74-103) |
| Código — BrasilApiService | `Sirb.CepBrasil/Services/BrasilApiService.cs` (linhas 44-86) |
| Código — AwesomeApiService | `Sirb.CepBrasil/Services/AwesomeApiService.cs` (linhas 44-86) |
| Código — ViaCepService | `Sirb.CepBrasil/Services/ViaCepService.cs` (linhas 58-91) |
| Código — OpenCepService | `Sirb.CepBrasil/Services/OpenCepService.cs` (linhas 44-76) |
| Testes — JsonExtension | `Sirb.CepBrasil.Test/Extensions/JsonExtensionTest.cs` (22+ testes) |
| Documento técnico | `docs/requirement/tec/tec-req-0004-serializacao-json.md` |
| APF | `docs/requirement/apf/apf-req-0004.md` |
| Análise cross-artifact | `docs/requirement/analises/analise-req-0004.md` |
| Quality Checklist | `docs/requirement/checklist/checklist-req-0004.md` |
| Dicionário de dados | `docs/requirement/did/did-models-resposta.md` |
| Matriz de risco global | `docs/system-risk-matrix.md` |
| Constituição | `docs/constituicao.md` |
| Documentos de melhoria | `docs/extra/MELHORIAS-JSONEXTENSION.md` |

---

## 15. Validações Realizadas para esta Documentação

- [x] README do projeto analisado
- [x] Código principal analisado (`JsonExtension.cs`, `BrasilApiResponse.cs`, `AwesomeApiResponse.cs`, `CepContainer.cs`, `CepResult.cs`, `BaseService.cs`, `BrasilApiService.cs`, `AwesomeApiService.cs`, `ViaCepService.cs`, `OpenCepService.cs`)
- [x] Testes unitários analisados (`JsonExtensionTest.cs`)
- [x] Documentos de melhoria consultados (`MELHORIAS-JSONEXTENSION.md`)
- [x] Constituição do projeto revisada
- [x] Arquitetura e tech stack revisados
- [x] Artefatos relacionados revisados (req-0001, req-0002, req-0003, tec-req-0001, tec-req-0002, tec-req-0003)

---

## 16. Clarification Log

| Data | Pergunta | Resposta | Status (Resolvido/Pendente) | Origem (Criação/Implementação) | Impacto no requisito |
|------|----------|----------|----------------------------|-------------------------------|----------------------|
| 23/07/2026 | Qual nome de usuário deve constar no campo `Autor` e nas entradas de histórico deste documento? | Rodrigo Araujo Barbosa | Resolvido | Criação | Define autor do documento |
| 23/07/2026 | O BaseService.GetResponseAsync usa ReadFromJsonAsync (System.Text.Json) ou FromJson do JsonExtension? | Usa `ReadFromJsonAsync<T>()` do `System.Net.Http.Json`, não o `FromJson` do `JsonExtension` | Resolvido | Criação | Confirma que `JsonExtension` é usado pelo consumidor, não internamente no BaseService |
| 23/07/2026 | O BooleanOrStringConverter está em qual arquivo? | Está em `CepContainer.cs` como classe interna | Resolvido | Criação | Localização confirmada |

### Cobertura do Clarification (8 áreas)

| # | Área | Status (Resolvido \| N/A) | Ref. entrada no Log | Justificativa (se N/A) |
|---|------|---------------------------|---------------------|------------------------|
| 1 | Atores e personas | Resolvido | 23/07/2026 — Autor definido | — |
| 2 | Fluxos principais e alternativos | N/A | — | Fluxo de serialização é coberto pelos cenários Gherkin |
| 3 | Exceções e erros | Resolvido | 23/07/2026 — ReadFromJsonAsync vs FromJson, BooleanOrStringConverter | — |
| 4 | Integrações externas | Resolvido | 23/07/2026 — Provedores consomem JsonExtension via ReadFromJsonAsync | — |
| 5 | Requisitos não-funcionais | Resolvido | Coberto na seção NFR (case-insensitive, desempenho < 10ms) | — |
| 6 | Dados e privacidade | N/A | — | Modelos de resposta não contêm dados sensíveis (apenas endereço público) |
| 7 | Validações e regras de negócio | Resolvido | Coberto nas RN-026 a RN-032 | — |
| 8 | Critérios de aceite e mensurabilidade | Resolvido | Coberto nos 15 cenários Gherkin | — |

**Cobertura atual:** 6/8 (Resolvido). **Meta:** 8/8.

---

## 17. Esclarecimentos Adicionais

| Data | Nota |
|------|------|
| 23/07/2026 | Documento criado com base no código-fonte v1.4.0 e documentos de melhoria em docs/extra |
| 23/07/2026 | O `JsonExtension` já está implementado e com 22+ testes. Este requisito documenta o comportamento existente e formaliza as regras de negócio |
| 23/07/2026 | O `BooleanOrStringConverter` está embutido no arquivo `CepContainer.cs`, não em arquivo separado |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
