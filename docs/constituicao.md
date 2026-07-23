# Constituição do Projeto — Sirb.CepBrasil

> **Propósito:** Princípios de governança que servem como filtro de aceite para todos os requisitos e mudanças no projeto.

| Campo           | Valor                    |
|-----------------|--------------------------|
| **Versão**      | v1.0                     |
| **Data**        | 23/07/2026               |
| **Autor**       | Rodrigo Araujo Barbosa   |
| **Status**      | `aprovado`               |

---

## Histórico de Alterações

| Data       | Versão | Descrição                    | Autor                 |
|------------|--------|------------------------------|-----------------------|
| 23/07/2026 | v1.0   | Criação da constituição      | Rodrigo Araujo Barbosa |

---

## 1. Segurança

### 1.1 Validação de Entrada

- **Toda entrada externa deve ser validada antes do processamento.** Nulos, strings vazias e espaços em branco devem ser rejeitados com `ArgumentNullException`.
- O CEP deve conter exatamente **8 dígitos numéricos** após remoção de máscara. Caracteres não numéricos (exceto hífen posicional) devem ser rejeitados com `ServiceException`.
- A validação deve ser **fail-fast**: a primeira violação encontrada interrompe o fluxo e retorna o erro imediatamente.
- **Responsabilidade:** a classe `CepValidation` (`Sirb.CepBrasil/Validations/CepValidation.cs`) é o ponto único de validação de CEP.

### 1.2 Tratamento de Exceções

- Exceções personalizadas (`ServiceException`, `NotFoundException`) devem ser usadas para erros de domínio.
- Exceções de sistema (`ArgumentNullException`, `ArgumentException`) devem ser usadas para erros de validação de entrada.
- `ServiceException.ThrowIf(condição, mensagem)` é o padrão para validações condicionais.
- Erros de serviços externos (HTTP, timeout) são capturados no `CepService` e consolidados em uma única mensagem de erro.
- Nenhuma exceção deve ser suprimida silenciosamente; sempre registrar a causa raiz.

### 1.3 Chamadas HTTPS

- **Todas as requisições a provedores externos devem usar HTTPS.** URLs inseguras (HTTP) são proibidas.
- Certificados SSL devem ser validados; não ignorar erros de certificado.

---

## 2. Performance

### 2.1 Chamadas Assíncronas

- **Toda operação de I/O deve ser assíncrona** usando `async/await`.
- `ConfigureAwait(false)` **deve ser usado em todos os awaits** em código de biblioteca para evitar captura desnecessária do `SynchronizationContext` e prevenir deadlocks.

### 2.2 Reuso de HttpClient

- `HttpClient` deve ser **reutilizado** sempre que possível para evitar esgotamento de sockets.
- A classe `CepService` oferece dois modos:
  - **Gerenciado internamente:** construtor sem parâmetros cria e gerencia o próprio `HttpClient` (disposto ao final do ciclo de vida).
  - **Injeção de dependência:** construtor aceita `HttpClient` externo, sem gerenciar seu ciclo de vida.
- Cada serviço provedor (`BrasilApiService`, `ViaCepService`, `AwesomeApiService`, `OpenCepService`) recebe o `HttpClient` via construtor injetado pelo `CepService`.

### 2.3 Timeout Configurável

- Timeout padrão: **30 segundos** por tentativa.
- Timeout é gerenciado via `CancellationTokenSource` com `using` para garantir liberação de recursos.
- O `CancellationToken.None` aciona o timeout padrão; se o usuário fornecer um token próprio, seus timeouts e cancelamento têm prioridade.

### 2.4 Gerenciamento de Recursos

- `CancellationTokenSource` deve ser **sempre disposto** via `using` para evitar vazamento de memória (timers, handles, callbacks).
- `CepService` implementa `IDisposable` e libera o `HttpClient` apenas se o gerenciou internamente.

---

## 3. Qualidade

### 3.1 Cobertura de Testes

- **100% de cobertura de código** obrigatória em todos os componentes de produção.
- Testes devem usar **xUnit** com assertions nativas (sem FluentAssertions ou bibliotecas de terceiros).
- **Estrutura AAA** (Arrange-Act-Assert) obrigatória em todos os testes.
- **Nomenclatura padronizada:** `MetodoTestado_Condicao_ResultadoEsperado`.
- **`[Fact(DisplayName = "...")]` ou `[Theory(DisplayName = "...")]` obrigatório** em todos os testes, com descrição em português.

### 3.2 CI/CD

- **Todo commit deve ser verificado** por pipeline de CI (GitHub Actions) que executa:
  - Build multi-target (.NET 8, 9, 10)
  - Testes unitários completos
  - Verificação de cobertura de código
- **Publicação no NuGet** apenas após aprovação da pipeline e criação de release no GitHub.
- A pipeline de publicação executa testes antes de publicar; se falhar, o pacote não é publicado.

### 3.3 Documentação XML

- **100% de documentação XML** em todas as classes, métodos e propriedades públicas.
- A documentação deve ser **em inglês** para compatibilidade internacional da biblioteca.
- Tags obrigatórias: `<summary>`, `<param>`, `<returns>`, `<exception>`, `<example>`.
- Nenhum método público deve ter apenas `<inheritdoc />`.
- A documentação deve ser clara, concisa e incluir exemplos de uso.

### 3.4 Build sem Warnings

- O projeto deve compilar **sem warnings** em todas as plataformas alvo.
- Regras de estilo de código devem ser respeitadas (`EnforceCodeStyleInBuild = true` no `.csproj`).

---

## 4. UX da API

### 4.1 API Fluente

- A interface pública `ICepService` deve ser simples e intuitiva: `FindAsync(cep, cancellationToken)`.
- O método retorna `Task<CepResult>`, que expõe `Success`, `CepContainer`, `Message` e `Exceptions`.
- **Não retornar tipos complexos ou genéricos expostos** — o contrato público deve ser autoexplicativo.

### 4.2 Suporte a DI

- A biblioteca deve funcionar tanto com **instanciação direta** quanto com **injeção de dependência**.
- `services.AddHttpClient<ICepService, CepService>()` deve funcionar sem configuração adicional.
- O construtor de `CepService` aceita `HttpClient` externo para cenários DI.

### 4.3 Exceções Customizadas

- `ServiceException`: usada para erros de serviço (HTTP, timeout, falha geral).
- `NotFoundException`: usada para recursos não encontrados (manter para compatibilidade, mas não usada no fluxo de fallback atual).
- Mensagens de erro em português para o domínio brasileiro.

### 4.4 Modelos de Resposta

- `CepResult`: container de resultado com flag de sucesso, dados e mensagens de erro.
- `CepContainer`: modelo imutável com os 7 campos do endereço brasileiro (CEP, logradouro, bairro, cidade, UF, complemento, IBGE).

---

## 5. Dados

### 5.1 Validação de Formato do CEP

- Formato aceito: `00000000` (8 dígitos) ou `00000-000` (com hífen opcional).
- O CEP é normalizado removendo a máscara antes da validação.
- A validação rejeita: valores nulos/vazios, comprimento diferente de 8 dígitos, caracteres não numéricos, hífens em posições incorretas, hífens duplicados.

### 5.2 Tratamento de Respostas Externas

- Cada provedor tem um modelo de resposta específico (`BrasilApiResponse`, `AwesomeApiResponse`, `CepContainer` para ViaCEP e OpenCEP).
- As respostas são **normalizadas** para o modelo unificado `CepContainer` antes de retornar ao usuário.
- Tratamento especial para o campo `erro` do ViaCEP (pode ser booleano ou string) usando `BooleanOrStringConverter`.
- Respostas nulas ou vazias de provedores são tratadas como "não encontrado".

### 5.3 Modelo de Dados Unificado

| Campo        | Tipo     | Origem                     | Obrigatório |
|--------------|----------|----------------------------|-------------|
| `Cep`        | `string` | Formatado (00000-000)      | Sim         |
| `Logradouro` | `string` | Street/address/logradouro  | Não         |
| `Bairro`     | `string` | Neighborhood/district      | Não         |
| `Cidade`     | `string` | City/localidade/cidade     | Sim         |
| `Uf`         | `string` | State/uf                   | Sim         |
| `Complemento`| `string` | Complemento/additional     | Não         |
| `Ibge`       | `string` | Código IBGE da cidade      | Não         |

---

## 6. Arquitetura

### 6.1 Fallback Pattern

- A biblioteca implementa **fallback chain pattern**: tenta provedores em sequência até obter sucesso.
- Ordem de tentativa: **BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP**.
- Comportamentos:
  - **Sucesso:** retorna imediatamente (não tenta próximo).
  - **Não encontrado (null):** tenta próximo provedor.
  - **Erro HTTP/timeout:** captura exceção, registra, tenta próximo.
  - **Todos com erro:** retorna `CepResult { Success = false }` com mensagem consolidada.
  - **Nenhum encontrou:** retorna `null`.

### 6.2 Separação por Serviços

- Cada provedor externo é uma classe separada que implementa `ICepServiceControl` (interface interna).
- `ICepService` (interface pública) é implementada por `CepService`, que orquestra o fallback.
- `BaseService` (classe abstrata interna) contém a lógica comum de HTTP (criação de requisição, execução, deserialização).

### 6.3 Interfaces

| Interface           | Visibilidade | Método                          | Retorno         | Finalidade                  |
|---------------------|--------------|---------------------------------|-----------------|-----------------------------|
| `ICepService`       | `public`     | `FindAsync(cep, cancellation)`  | `Task<CepResult>`| Interface pública da lib   |
| `ICepServiceControl`| `internal`   | `FindAsync(cep, cancellation)`  | `Task<CepContainer>` | Interface interna para serviços |

### 6.4 Gerenciamento de Ciclo de Vida

- `BaseService` usa **primary constructor** para receber `HttpClient`.
- Serviços provedores usam primary constructor com `HttpClient` herdando de `BaseService`.
- `CepService` é o ponto único de entrada pública e gerencia a lista de serviços.

---

## 7. Processo

### 7.1 Convenções .NET

- **Target frameworks:** .NET 8.0, 9.0, 10.0 (multi-target).
- **Linguagem:** C# 12+ com `LangVersion = latest`.
- **Nullable:** habilitado (tratar nulos adequadamente).
- **Ordem de modificadores C#:** `public static` e `internal static` (não `static public` / `static internal`).
- **Nomenclatura:**
  - Classes/métodos públicos: **PascalCase**
  - Parâmetros/variáveis locais: **camelCase**
  - Constantes: **PascalCase** (ex.: `DefaultTimeoutMilliseconds`)
  - Propriedades estáticas: **PascalCase** (ex.: `SerializerOptions`)

### 7.2 Multi-target

- A biblioteca deve compilar e testar em **.NET 8, 9 e 10** simultaneamente.
- Não usar APIs exclusivas de uma versão sem fallback.
- `InternalsVisibleTo` configurado para permitir testes de componentes internos.

### 7.3 Documentação em Inglês

- Documentação XML de código (summary, param, returns, exception) deve ser em **inglês** para alcance internacional.
- Mensagens de erro e DisplayName de testes podem ser em português.
- Documentação de projeto e requisitos (arquivos `.md`) podem ser em português.

### 7.4 Versionamento Semântico

- Seguir [SemVer](https://semver.org/):
  - **MAJOR:** breaking changes
  - **MINOR:** novas features backward-compatible
  - **PATCH:** bug fixes backward-compatible

---

## 8. Filtro de Aceite

Nenhum requisito, mudança ou contribuição será aceita se violar qualquer um dos princípios acima. Em caso de conflito entre princípios, a ordem de precedência é:

1. **Segurança** (validação de entrada, HTTPS)
2. **Qualidade** (cobertura de testes, CI/CD)
3. **Performance** (async, HttpClient, resources)
4. **Arquitetura** (fallback pattern, interfaces)
5. **Processo** (convenções, multi-target, docs)
6. **UX da API** (fluência, DI, exceptions)
7. **Dados** (formato, normalização)

---

## 9. Clarificações e Notas

| Data       | Nota                                                                                |
|------------|--------------------------------------------------------------------------------------|
| 23/07/2026 | Documento inicial criado com base no código-fonte e README.md do projeto v1.4.0.    |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
