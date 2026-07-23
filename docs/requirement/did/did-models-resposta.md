# Dicionário de Dados — Modelos de Resposta dos Provedores

## Metadados

| Campo                | Valor                      |
|----------------------|----------------------------|
| **Código**           | did-models-resposta        |
| **Título**           | Dicionário de Dados — Modelos de Resposta |
| **Data de criação**  | 23/07/2026                 |
| **Última atualização**| 23/07/2026                 |
| **Autor**            | Rodrigo Araujo Barbosa     |
| **Versão**           | v1.0                       |
| **Status**           | Rascunho                   |

---

## Objetivo

Catalogar todos os campos, tipos, mapeamentos e regras dos modelos de resposta utilizados na serialização/desserialização JSON da biblioteca Sirb.CepBrasil. Abrange os modelos específicos por provedor (`BrasilApiResponse`, `AwesomeApiResponse`) e o modelo unificado (`CepContainer`).

## Escopo

- Modelos cobertos: `BrasilApiResponse`, `AwesomeApiResponse`, `CepContainer`
- Limites do documento: Não cobre `CepResult` (já documentado no req-0002) nem modelos de exceção (req-0003)
- Provedores relacionados: BrasilAPI, ViaCEP, AwesomeAPI, OpenCEP

## Artefatos relacionados

### Documentos/requisitos que impactam este artefato

- `docs/requirement/req-0004-serializacao-json.md` — requisito de serialização JSON
- `docs/requirement/req-0002-busca-endereco-fallback.md` — uso dos modelos nos serviços de fallback
- `Sirb.CepBrasil/Models/BrasilApiResponse.cs` — código do modelo BrasilAPI
- `Sirb.CepBrasil/Models/AwesomeApiResponse.cs` — código do modelo AwesomeAPI
- `Sirb.CepBrasil/Models/CepContainer.cs` — código do modelo unificado

### Documentos/requisitos impactados por este artefato

- `docs/requirement/tec/tec-req-0004-serializacao-json.md` — detalhamento técnico
- `docs/requirement/api/api-brasilapi.md` — documentação da API BrasilAPI
- `docs/requirement/api/api-awesomeapi.md` — documentação da API AwesomeAPI
- `docs/requirement/api/api-viacep.md` — documentação da API ViaCEP

### Componentes técnicos relacionados

- Classes C#: `BrasilApiResponse`, `AwesomeApiResponse`, `CepContainer`, `BooleanOrStringConverter`
- Namespace: `Sirb.CepBrasil.Models`
- Serviços consumidores: `BrasilApiService`, `AwesomeApiService`, `ViaCepService`, `OpenCepService`

---

## Dicionário de campos

### CepContainer (Modelo Unificado)

Classe pública (`public sealed class`) que representa um endereço brasileiro completo. Usada como saída padronizada de todos os provedores e diretamente na desserialização das respostas do ViaCEP e OpenCEP.

**Arquivo:** `Sirb.CepBrasil/Models/CepContainer.cs`
**Tipo:** `public sealed class` (primary constructor com 7 parâmetros)
**JSON:** Serializado/desserializado via `System.Text.Json` com `JsonPropertyName`

| Campo | Tipo | Tamanho | Obrigatório | JsonPropertyName | Regra/Domínio | Origem/Uso |
|-------|------|---------|------------|-----------------|---------------|------------|
| `Uf` | `string` | 2 caracteres | Sim | `"uf"` | Sigla do estado brasileiro (SP, RJ, MG, etc.) | ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP |
| `Cidade` | `string` | Variável | Sim | `"localidade"` | Nome completo da cidade | ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP |
| `Bairro` | `string` | Variável | Não | `"bairro"` | Nome do bairro; pode ser vazio para CEPs genéricos | ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP |
| `Complemento` | `string` | Variável | Não | `"complemento"` | Informações adicionais (ex.: "lado ímpar", "até 500") | ViaCEP (outros provedores não fornecem) |
| `Logradouro` | `string` | Variável | Não | `"logradouro"` | Nome da rua, avenida ou praça; pode ser vazio para CEPs especiais | ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP |
| `Cep` | `string` | 9 caracteres (formato 00000-000) | Sim | `"cep"` | CEP formatado com hífen (ex.: "01310-100") | ViaCEP, BrasilAPI, AwesomeAPI, OpenCEP |
| `Ibge` | `string` | 7 caracteres | Não | `"ibge"` | Código IBGE do município (ex.: "3550308") | ViaCEP (outros provedores não fornecem) |
| `Erro` | `bool?` | 1 bit (nullable) | Não | `"erro"` | Indica se o CEP não foi encontrado (específico ViaCEP) | ViaCEP (usa `BooleanOrStringConverter`) |

**Observações:**
- O `BooleanOrStringConverter` (classe interna em `CepContainer.cs`) permite que `Erro` seja `true`, `false` (booleano) ou `"true"`, `"false"` (string).
- Propriedades `{ get; }` (init-only via primary constructor), exceto `Erro` que é `{ get; init; }`.
- `Complemento` e `Ibge` são fornecidos apenas pelo ViaCEP; BrasilAPI e AwesomeAPI não retornam esses campos.
- Ao serializar com `ToJson()`, a propriedade `Erro` é serializada como booleano (graças ao `BooleanOrStringConverter.Write`).

---

### BrasilApiResponse (Modelo Específico BrasilAPI)

Classe interna (`internal sealed class`) para desserializar a resposta da API BrasilAPI.

**Arquivo:** `Sirb.CepBrasil/Models/BrasilApiResponse.cs`
**Tipo:** `internal sealed class` (auto properties)
**URL da API:** `https://brasilapi.com.br/api/cep/v1/{cep}`

| Campo | Tipo | Tamanho | Obrigatório | JsonPropertyName | Regra/Domínio | Origem/Uso |
|-------|------|---------|------------|-----------------|---------------|------------|
| `Cep` | `string` | 9 caracteres (00000-000) | Sim | `"cep"` | CEP formatado | Resposta JSON da BrasilAPI |
| `Uf` | `string` | 2 caracteres | Sim | `"state"` | Sigla do estado | Resposta JSON da BrasilAPI |
| `Cidade` | `string` | Variável | Sim | `"city"` | Nome da cidade | Resposta JSON da BrasilAPI |
| `Bairro` | `string` | Variável | Não | `"neighborhood"` | Nome do bairro | Resposta JSON da BrasilAPI |
| `Logradouro` | `string` | Variável | Não | `"street"` | Nome do logradouro | Resposta JSON da BrasilAPI |
| `Service` | `string` | Variável | Não | `"service"` | Identificador do provedor de dados usado pela BrasilAPI | Resposta JSON da BrasilAPI |

**Mapeamento para CepContainer:**

| BrasilApiResponse | CepContainer | Observação |
|-------------------|-------------|------------|
| `Cep` | `Cep` | Direto |
| `Uf` | `Uf` | Direto |
| `Cidade` | `Cidade` | Direto |
| `Bairro` | `Bairro` | Direto |
| `Logradouro` | `Logradouro` | Direto |
| — (ausente) | `Complemento` | BrasilAPI não retorna complemento → `null` |
| — (ausente) | `Ibge` | BrasilAPI não retorna código IBGE → `null` |
| — (ausente) | `Erro` | BrasilAPI usa HTTP 404, não campo `erro` → `null` |

---

### AwesomeApiResponse (Modelo Específico AwesomeAPI)

Classe interna (`internal sealed class`) para desserializar a resposta da API AwesomeAPI.

**Arquivo:** `Sirb.CepBrasil/Models/AwesomeApiResponse.cs`
**Tipo:** `internal sealed class` (auto properties)
**URL da API:** `https://cep.awesomeapi.com.br/json/{cep}`

| Campo | Tipo | Tamanho | Obrigatório | JsonPropertyName | Regra/Domínio | Origem/Uso |
|-------|------|---------|------------|-----------------|---------------|------------|
| `Cep` | `string` | 9 caracteres (00000-000) | Sim | `"cep"` | CEP formatado | Resposta JSON da AwesomeAPI |
| `Uf` | `string` | 2 caracteres | Sim | `"state"` | Sigla do estado | Resposta JSON da AwesomeAPI |
| `Cidade` | `string` | Variável | Sim | `"city"` | Nome da cidade | Resposta JSON da AwesomeAPI |
| `Bairro` | `string` | Variável | Não | `"district"` | Nome do bairro (campo `district` na AwesomeAPI) | Resposta JSON da AwesomeAPI |
| `Logradouro` | `string` | Variável | Não | `"address"` | Nome do logradouro (campo `address` na AwesomeAPI) | Resposta JSON da AwesomeAPI |
| `Service` | `string` | Variável | Não | `"service"` | Identificador do serviço/provedor | Resposta JSON da AwesomeAPI |

**Mapeamento para CepContainer:**

| AwesomeApiResponse | CepContainer | Observação |
|-------------------|-------------|------------|
| `Cep` | `Cep` | Direto |
| `Uf` | `Uf` | Direto |
| `Cidade` | `Cidade` | Direto |
| `Bairro` | `Bairro` | Mapeado de `district` |
| `Logradouro` | `Logradouro` | Mapeado de `address` |
| — (ausente) | `Complemento` | AwesomeAPI não retorna complemento → `null` |
| — (ausente) | `Ibge` | AwesomeAPI não retorna código IBGE → `null` |
| — (ausente) | `Erro` | AwesomeAPI usa HTTP 404/200 com array vazio, não campo `erro` → `null` |

---

## Regras complementares

### Valores default
- Todos os campos string: `null` quando não fornecidos pelo provedor
- `Erro` (bool?): `null` quando não fornecido ou valor não reconhecido
- `Complemento`: `null` padrão (BrasilAPI e AwesomeAPI não fornecem)
- `Ibge`: `null` padrão (BrasilAPI e AwesomeAPI não fornecem)

### Regras de consistência
- O campo `Erro` do `CepContainer` é tratado pelo `BooleanOrStringConverter` e nunca deve ser validado como string no código de negócio
- `BrasilApiResponse` e `AwesomeApiResponse` nunca devem ser expostos publicamente (classes `internal`)
- A conversão para `CepContainer` é feita exclusivamente nos serviços (`BrasilApiService`, `AwesomeApiService`)
- ViaCEP e OpenCEP desserializam diretamente para `CepContainer` (sem modelo intermediário)

### Máscaras e formatações
- `Cep` sempre no formato `00000-000` (com hífen) nos modelos de resposta
- `Uf` sempre em 2 caracteres maiúsculos
- `Ibge` sempre em 7 dígitos (sem zeros à esquerda)

### Sensibilidade/LGPD
- Nenhum campo contém dados pessoais sensíveis. Dados de endereço público (CEP, logradouro, bairro, cidade, UF) são informações de domínio público.

---

## Validações realizadas para esta documentação

- [x] Código dos modelos verificado (`BrasilApiResponse.cs`, `AwesomeApiResponse.cs`, `CepContainer.cs`)
- [x] Mapeamento de campos confirmado nos serviços (`BrasilApiService.cs`, `AwesomeApiService.cs`)
- [x] Comportamento do `BooleanOrStringConverter` verificado no código
- [x] Documentações relacionadas revisadas (req-0002, req-0004)
- [x] APIs dos provedores consultadas (BrasilAPI, AwesomeAPI, ViaCEP)

---

## Histórico de alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do dicionário de dados |

---

## Esclarecimentos

- **Premissas consideradas:** Os mapeamentos refletem o código-fonte atual v1.4.0. As APIs dos provedores podem mudar sem aviso — novos campos devem ser adicionados aos modelos correspondentes.
- **Dúvidas pendentes:** Nenhuma.
- **Decisões tomadas:** `BrasilApiResponse` e `AwesomeApiResponse` como classes `internal` para não poluir a API pública da biblioteca. Apenas `CepContainer` e `CepResult` são públicos.

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
