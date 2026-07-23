# req-0001 — Validação de CEP

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | req-0001                 |
| **Título**           | Validação de CEP         |
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

Este documento especifica o requisito de **Validação de CEP** da biblioteca Sirb.CepBrasil. A validação de CEP é o ponto de entrada obrigatório para toda consulta de endereço postal brasileiro, garantindo que apenas CEPs com formato válido sejam processados pelos serviços provedores.

### 1.2 Escopo

**In scope:**

- Validação de formato do CEP brasileiro (8 dígitos numéricos, com ou sem máscara `00000-000`)
- Rejeição de valores nulos, vazios ou com apenas espaços em branco
- Rejeição de caracteres não numéricos (exceto hífen na posição padrão)
- Rejeição de hífens em posições incorretas, duplicados, no início ou no final
- Disponibilização de métodos de extensão para operações comuns de CEP:
  - Remoção de máscara (`RemoveMask`, `GetDigitsOnly`)
  - Aplicação de máscara (`CepMask`)
  - Validação booleana (`IsValidCep`)
  - Formatação condicional (`Format`)
  - Normalização (`Normalize`)
- Mensagens de erro padronizadas

**Out of scope:**

- Consulta a serviços externos para verificar se o CEP existe (validação de existência)
- Validação de dígito verificador do CEP
- Geolocalização ou coordenadas
- Cache de CEPs previamente validados
- Qualquer operação de banco de dados
- Interface de usuário (biblioteca .NET, sem UI)

### 1.3 Glossário

| Termo | Definição |
|-------|-----------|
| CEP | Código de Endereçamento Postal brasileiro, composto por 8 dígitos numéricos |
| Máscara | Formatação visual do CEP no padrão `00000-000` |
| Dígito | Caractere numérico (0-9) |
| Hífen posicional | Hífen opcional na posição padrão entre o 5º e 6º dígito (`_____-___`) |

---

## 2. Intenção / Motivação de Negócio (Por quê)

### Problema de negócio real sendo resolvido

A biblioteca Sirb.CepBrasil recebe entradas de CEP de diversas origens: formulários web, sistemas desktop, APIs, arquivos de importação. Cada origem pode enviar o CEP em formatos diferentes (com ou sem máscara, com espaços, com caracteres especiais). Sem uma validação robusta na entrada, dados malformados chegariam aos provedores externos, gerando requisições desnecessárias, aumento de latência, consumo de banda e exposição a erros HTTP evitáveis.

### Por que importa (impacto mensurável)

- **Prevenção de requisições inválidas:** aproximadamente 15-20% das chamadas a provedores externos seriam descartadas por formato inválido se a validação não existisse, consumindo recursos de rede e filas de timeout.
- **Fail-fast:** ao rejeitar entradas inválidas no primeiro milissegundo, evitam-se timeouts de até 30s por provedor (até 120s no pior caso com fallback).
- **Consistência de formato:** todos os provedores recebem o CEP normalizado (8 dígitos), eliminando erros de parse específicos de cada provedor.

### Quem é afetado

- **Desenvolvedores clientes** da biblioteca que fornecem CEPs para consulta.
- **Sistemas integrados** que consomem a biblioteca via DI ou instanciação direta.
- **Usuários finais** indiretamente — a validação robusta garante respostas mais rápidas e menor taxa de erro.

### Critério de sucesso mensurável

- **100% das entradas inválidas** (nulas, vazias, formato incorreto, caracteres inválidos) são rejeitadas antes de qualquer chamada a provedor externo.
- **0% de falsos positivos** na validação (nenhum CEP válido é rejeitado).
- **Cobertura de testes de 100%** mantida na classe `CepValidation` e `CepExtension`.

---

## 3. Descrição Geral

### 3.1 Missão da Funcionalidade

Garantir que toda consulta de CEP receba uma entrada validada e normalizada, eliminando ruídos e formatos inválidos antes da chamada aos provedores externos, maximizando a taxa de sucesso e reduzindo latência desnecessária.

### 3.2 Atores

| Ator | Descrição |
|------|-----------|
| Sistema consumidor | Aplicação cliente que utiliza a biblioteca para consultar CEPs |
| Desenvolvedor | Profissional que integra a biblioteca em um sistema |

### 3.3 Contexto de Uso

A validação de CEP é acionada:
1. Diretamente via `CepValidation.Validate(cep)` pelo `CepService` antes de cada consulta.
2. Pelos métodos de extensão `IsValidCep()`, `RemoveMask()`, `CepMask()`, `Format()` e `Normalize()` expostos como API pública.

---

## 4. Artefatos Relacionados

### Documentos que impactam este artefato

- `docs/constituicao.md` (segurança, qualidade, dados)
- `docs/architecture-tech-stack.md` (stack .NET, regex)
- `docs/system-mapping.md` (estrutura do sistema)
- `docs/extra/MELHORIAS-CEPVALIDATION.md` (documento de melhoria da validação)
- `docs/extra/RESUMO-TESTES-CEPEXTENSION.md` (resumo de testes da extensão)
- `docs/extra/TESTES-CEPEXTENSION.md` (detalhes de testes da extensão)
- `Sirb.CepBrasil/Validations/CepValidation.cs` (código fonte da validação)
- `Sirb.CepBrasil/Extensions/CepExtension.cs` (código fonte dos métodos de extensão)
- `Sirb.CepBrasil/Messages/CepMessages.cs` (mensagens de erro)

### Documentos impactados por este artefato

- `docs/system-risk-matrix.md` (atualização com recorte local)
- `docs/requirement/tec/tec-req-0001-validacao-cep.md`
- `docs/requirement/apf/apf-req-0001.md`
- `docs/requirement/analises/analise-req-0001.md`
- `docs/requirement/checklist/checklist-req-0001.md`

### Componentes técnicos relacionados

| Componente | Caminho | Função |
|------------|---------|--------|
| `CepValidation` | `Sirb.CepBrasil/Validations/CepValidation.cs` | Validação de formato e comprimento do CEP |
| `CepExtension` | `Sirb.CepBrasil/Extensions/CepExtension.cs` | Métodos de extensão (RemoveMask, CepMask, IsValidCep, etc.) |
| `CepMessages` | `Sirb.CepBrasil/Messages/CepMessages.cs` | Mensagens de erro padronizadas |
| `ServiceException` | `Sirb.CepBrasil/Exceptions/ServiceException.cs` | Exceção lançada para CEPs inválidos (formato) |
| `CepValidationTest` | `Sirb.CepBrasil.Test/Validations/CepValidationTest.cs` | Testes unitários da validação |
| `CepExtensionTest` | `Sirb.CepBrasil.Test/Extensions/CepExtensionTest.cs` | Testes unitários da extensão |

---

## 5. Requisitos Funcionais

| ID | Descrição | Prioridade | Regra de negócio associada |
|----|-----------|------------|----------------------------|
| RF-01 | Validar formato de CEP | Alta | RN-001, RN-002, RN-003, RN-004, RN-005, RN-006 |
| RF-02 | Remover máscara do CEP | Alta | RN-001, RN-010 |
| RF-03 | Aplicar máscara no CEP (formatação 00000-000) | Média | RN-001, RN-002, RN-008 |
| RF-04 | Validar CEP com retorno booleano | Alta | RN-003, RN-007 |
| RF-05 | Extrair apenas dígitos do CEP | Baixa | RN-001, RN-010 |
| RF-06 | Formatar CEP com segurança (vazio se inválido) | Média | RN-008 |
| RF-07 | Normalizar CEP (validar + remover máscara) | Média | RN-009 |

---

## 6. Regras de Negócio

| ID | Regra | Origem | Impacto |
|----|-------|--------|---------|
| RN-001 | O CEP deve conter exatamente 8 dígitos numéricos após remoção da máscara para ser considerado válido | Padrão brasileiro (Correios) | Validação de comprimento |
| RN-002 | O CEP pode ser informado com máscara (`00000-000`) ou sem máscara (`00000000`) | Requisito de usabilidade | Aceita ambos os formatos |
| RN-003 | Valores nulos (`null`), vazios (`""`) ou com apenas espaços em branco (`" "`) devem ser rejeitados | Boa prática de segurança | Lança `ArgumentNullException` ou retorna `false` |
| RN-004 | Caracteres não numéricos (exceto hífen) devem ser rejeitados | Segurança e formato | Lança `ServiceException` |
| RN-005 | Se presente, o hífen deve estar na posição padrão (após o 5º dígito) | Padrão brasileiro | Validação de posição do hífen |
| RN-006 | Hífens duplicados, no início ou no final da string são rejeitados | Prevenção de ambiguidade | Lança `ServiceException` |
| RN-007 | CEP inválido retorna `false` (não lança exceção) nos métodos de extensão `IsValidCep()` | Suavidade na validação (não-throwing) | Diferente do `Validate()` que lança exceção |
| RN-008 | CEP inválido retorna string vazia (`""`) no método `Format()` e valor original no `CepMask()` | Comportamento seguro | Retorno condicional conforme método |
| RN-009 | CEP inválido retorna `null` no método `Normalize()` | Comportamento seguro | Retorno nulo para inválidos |
| RN-010 | O método `RemoveMask()` preserva espaços em branco (o regex `[^\d]` remove apenas não-dígitos) | Comportamento do regex | Espaços não são removidos |

---

## 7. Critérios de Aceitação (Gherkin)

```gherkin
Funcionalidade: Validação de CEP
  Como um sistema consumidor da biblioteca
  Quero validar o formato do CEP antes de consultar
  Para garantir que apenas CEPs com formato válido sejam processados

  Cenário: CEP válido com máscara
    Dado que o CEP informado é "01310-100"
    Quando o método Validate() é executado
    Então nenhuma exceção deve ser lançada
    E IsValidCep() deve retornar true

  Cenário: CEP válido sem máscara
    Dado que o CEP informado é "01310100"
    Quando o método Validate() é executado
    Então nenhuma exceção deve ser lançada
    E IsValidCep() deve retornar true

  Cenário: CEP nulo
    Dado que o CEP informado é null
    Quando o método Validate() é executado
    Então deve ser lançada ArgumentNullException
    E IsValidCep() deve retornar false

  Cenário: CEP vazio
    Dado que o CEP informado é ""
    Quando o método Validate() é executado
    Então deve ser lançada ArgumentNullException
    E IsValidCep() deve retornar false

  Cenário: CEP com apenas espaços
    Dado que o CEP informado é "   "
    Quando o método Validate() é executado
    Então deve ser lançada ArgumentNullException
    E IsValidCep() deve retornar false

  Cenário: CEP com comprimento inválido (menos de 8 dígitos)
    Dado que o CEP informado é "123"
    Quando o método Validate() é executado
    Então deve ser lançada ServiceException
    E IsValidCep() deve retornar false

  Cenário: CEP com comprimento inválido (mais de 8 dígitos)
    Dado que o CEP informado é "123456789"
    Quando o método Validate() é executado
    Então deve ser lançada ServiceException
    E IsValidCep() deve retornar false

  Cenário: CEP com caracteres não numéricos
    Dado que o CEP informado é "0131a100"
    Quando o método Validate() é executado
    Então deve ser lançada ServiceException

  Cenário: CEP com hífen na posição incorreta
    Dado que o CEP informado é "013101-00"
    Quando o método Validate() é executado
    Então deve ser lançada ServiceException

  Cenário: CEP com hífen duplicado
    Dado que o CEP informado é "01310--100"
    Quando o método Validate() é executado
    Então deve ser lançada ServiceException

  Cenário: Remover máscara de CEP formatado
    Dado que o CEP informado é "01310-100"
    Quando o método RemoveMask() é executado
    Então o resultado deve ser "01310100"

  Cenário: Aplicar máscara em CEP sem formatação
    Dado que o CEP informado é "01310100"
    Quando o método CepMask() é executado
    Então o resultado deve ser "01310-100"

  Cenário: Formatar CEP inválido
    Dado que o CEP informado é "123"
    Quando o método Format() é executado
    Então o resultado deve ser ""

  Cenário: Normalizar CEP inválido
    Dado que o CEP informado é "123"
    Quando o método Normalize() é executado
    Então o resultado deve ser null
```

---

## 8. Requisitos Não Funcionais (NFRs)

| Categoria | Requisito | Métrica/Critério |
|-----------|-----------|------------------|
| Desempenho | A validação de CEP deve executar em menos de 10ms para qualquer entrada válida ou inválida | Tempo de execução < 10ms (regex compilado + validação de string) |
| Desempenho | Regex compilados devem ser estáticos e reutilizados entre chamadas | RegexOptions.Compiled + TimeSpan de 15s |
| Segurança | Toda entrada deve ser validada antes do processamento | Nulos, vazios e espaços rejeitados com ArgumentNullException |
| Segurança | Caracteres não numéricos rejeitados | ServiceException com mensagem padrão |
| Manutenibilidade | 100% de cobertura de testes | Cobertura medida via Coverlet |
| Manutenibilidade | 100% de documentação XML em inglês | Tags <summary>, <param>, <returns>, <exception>, <example> |
| Confiabilidade | Validação deve ser deterministic: mesma entrada → mesmo resultado | Função pura, sem estado externo |
| Compatibilidade | Funcionar em .NET 8, 9 e 10 | Build multi-target no CI |

---

## 9. Wireframe

**Não aplicável.** Projeto é uma biblioteca .NET sem interface de usuário. Nenhum wireframe ou padrão visual é necessário.

```
⏳ PENDENTE [Fase 3 — Design] — N/A: Biblioteca .NET sem interface de usuário.
```

---

## 10. Integração de API / Banco de Dados

**Não aplicável.** A validação de CEP é uma operação puramente local (validação de formato de string em memória) — não envolve chamadas de API, banco de dados ou integrações externas.

```
⏳ PENDENTE [Fase 3 — Dados] — N/A: Requisito não envolve banco de dados ou endpoints de API.
```

---

## 11. Matriz de Risco do Requisito (Recorte Local)

| ID | Requisito | Descrição do Risco | Probabilidade | Impacto | Score | Nível | Mitigação | Status |
|----|-----------|-------------------|---------------|---------|-------|-------|-----------|--------|
| RSK-007 | req-0001 | CEP inválido aceito pela validação (ex.: "00000000" tem 8 dígitos mas não existe em nenhum provedor) | A (4) — Alta | B (2) — Baixo | 8 | 🟡 Médio | Validação de formato rejeita caracteres inválidos, comprimento incorreto e hífens mal posicionados. O fallback tenta todos os provedores para confirmar inexistência. | 🟢 Aceito (comportamento documentado) |
| RSK-011 | req-0001 | Regex com complexidade excessiva causando DoS (ReDoS) em entradas maliciosamente longas | B (2) — Baixa | M (3) — Médio | 6 | 🟡 Médio | Regex compilados com `RegexOptions.Compiled` e timeout de 15s. Validação de comprimento antes do regex. | 🟢 Aceito (mitigado) |
| RSK-012 | req-0001 | ArgumentNullException não tratada pelo consumidor da biblioteca | M (3) — Média | B (2) — Baixo | 6 | 🟡 Médio | Comportamento documentado nas XML docs e nesta especificação. Exceção esperada e justificada. | 🟢 Aceito (comportamento esperado) |

---

## 12. Análise de Pontos de Função (APF) Vinculada

- **Documento APF:** `docs/requirement/apf/apf-req-0001.md`
- **Total estimado:** 20 PF (não ajustados)
- **Detalhamento:** ver documento APF específico.

---

## 13. Impactos Técnicos

### Backend / Código

- `Sirb.CepBrasil/Validations/CepValidation.cs` — classe `internal static` com método `Validate(string)`
- `Sirb.CepBrasil/Extensions/CepExtension.cs` — classe `public static` com 6 métodos de extensão
- `Sirb.CepBrasil/Messages/CepMessages.cs` — constantes de mensagens padronizadas

### Frontend

- N/A — biblioteca .NET sem UI

### Banco de Dados

- N/A — sem persistência

### Integrações Externas

- N/A — operação puramente local

### Design / UX

- N/A — sem interface de usuário

---

## 14. Rastreabilidade

| Item | Referência |
|------|------------|
| Código — Validação | `Sirb.CepBrasil/Validations/CepValidation.cs` |
| Código — Extensão | `Sirb.CepBrasil/Extensions/CepExtension.cs` |
| Código — Mensagens | `Sirb.CepBrasil/Messages/CepMessages.cs` |
| Testes — Validação | `Sirb.CepBrasil.Test/Validations/CepValidationTest.cs` (26 testes) |
| Testes — Extensão | `Sirb.CepBrasil.Test/Extensions/CepExtensionTest.cs` (52 testes) |
| Documento técnico | `docs/requirement/tec/tec-req-0001-validacao-cep.md` |
| APF | `docs/requirement/apf/apf-req-0001.md` |
| Análise cross-artifact | `docs/requirement/analises/analise-req-0001.md` |
| Quality Checklist | `docs/requirement/checklist/checklist-req-0001.md` |
| Matriz de risco global | `docs/system-risk-matrix.md` |
| Constituição | `docs/constituicao.md` |
| Documentos de melhoria | `docs/extra/MELHORIAS-CEPVALIDATION.md`, `docs/extra/RESUMO-TESTES-CEPEXTENSION.md` |

---

## 15. Validações Realizadas para esta Documentação

- [x] README do projeto analisado
- [x] Código principal analisado (`CepValidation.cs`, `CepExtension.cs`, `CepMessages.cs`)
- [x] Testes unitários analisados (`CepValidationTest.cs`, `CepExtensionTest.cs`)
- [x] Documentos de melhoria consultados (`MELHORIAS-CEPVALIDATION.md`, `RESUMO-TESTES-CEPEXTENSION.md`, `TESTES-CEPEXTENSION.md`)
- [x] Constituição do projeto revisada
- [x] Arquitetura e tech stack revisados
- [x] Artefatos relacionados revisados

---

## 16. Clarification Log

| Data | Pergunta | Resposta | Status (Resolvido/Pendente) | Origem (Criação/Implementação) | Impacto no requisito |
|------|----------|----------|----------------------------|-------------------------------|----------------------|
| 23/07/2026 | Qual nome de usuário deve constar no campo `Autor` e nas entradas de histórico deste documento? | Rodrigo Araujo Barbosa | Resolvido | Criação | Define autor do documento |
| 23/07/2026 | Os métodos de extensão de CEP (RemoveMask, CepMask, IsValidCep) devem ser documentados como parte da validação ou como requisito separado? | Consolidar em req-0001 (Validação de CEP) | Resolvido | Criação | req-0005 consolidado em req-0001 |
| 23/07/2026 | Qual é a motivação de negócio para a validação de CEP? | Prevenir requisições inválidas a provedores externos (~15-20% das chamadas), fail-fast no primeiro ms, consistência de formato | Resolvido | Criação | Preenche seção "Intenção / Motivação de Negócio" |

### Cobertura do Clarification (8 áreas)

| # | Área | Status (Resolvido \| N/A) | Ref. entrada no Log | Justificativa (se N/A) |
|---|------|---------------------------|---------------------|------------------------|
| 1 | Atores e personas | Resolvido | 23/07/2026 — Autor definido | — |
| 2 | Fluxos principais e alternativos | N/A | — | Fluxos triviais (validação de string única); documentado na seção de Gherkin |
| 3 | Exceções e erros | Resolvido | Coberto nos cenários Gherkin (6 cenários de exceção) | — |
| 4 | Integrações externas | N/A | — | Requisito não envolve integrações externas |
| 5 | Requisitos não-funcionais | Resolvido | Coberto na seção NFR (desempenho, segurança, manutenibilidade) | — |
| 6 | Dados e privacidade | N/A | — | Requisito não processa dados pessoais ou sensíveis |
| 7 | Validações e regras de negócio | Resolvido | Coberto nas RN-001 a RN-010 | — |
| 8 | Critérios de aceite e mensurabilidade | Resolvido | Coberto nos 14 cenários Gherkin | — |

**Cobertura atual:** 6/8 (Resolvido). **Meta:** 8/8.

---

## 17. Esclarecimentos Adicionais

| Data | Nota |
|------|------|
| 23/07/2026 | Documento consolidado a partir dos arquivos MELHORIAS-CEPVALIDATION.md, RESUMO-TESTES-CEPEXTENSION.md e TESTES-CEPEXTENSION.md |
| 23/07/2026 | O requisito original req-0005 (Métodos de Extensão de CEP) foi consolidado neste req-0001 conforme decisão do usuário |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
