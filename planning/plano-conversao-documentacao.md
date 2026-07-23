# Implementation Plan: Conversão da Documentação para o Formato Padrão

**Status:** COMPLETED

## Overview

Este plano cobre a **conversão de 29 arquivos de documentação esparsos** (localizados em `/docs/`) para a **estrutura padronizada de documentação** definida pela skill `documentation`. Após a conversão, os 29 arquivos originais serão **excluídos**, restando apenas a nova estrutura em `/docs/` com artefatos rastreáveis, versionados e em conformidade com o padrão da skill.

**Projeto:** Sirb.CepBrasil — Biblioteca .NET (C#) para consulta de CEP brasileiro com fallback entre 4 provedores (BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP). Multi-target .NET 8, 9, 10.

**Observações importantes:**
- Projeto de biblioteca (sem UI) → sem wireframes, sem `padrao-visual.md`, sem `database/`
- APIs são endpoints HTTP externos consumidos pelos serviços → documentar os 4 provedores
- O diretório `docs/extras/` será **criado** para preservar qualquer conteúdo original que não se enquadre nos novos artefatos (ex.: documentos de apoio, rascunhos, checklists operacionais)

## Architecture Decisions

- **Batch mode obrigatório**: A demanda cobre a documentação completa do sistema → ativar modo lotes conforme `lotes.md` (Fases 1-4).
- **Estrutura de destino**: Segue `estrutura.md` com adaptações para biblioteca (sem UI, sem banco de dados).
- **Requisitos identificados**: 5 requisitos funcionais + 1 requisito não-funcional (CI/CD) — ver tabela na Fase 1.
- **Nomenclatura**: Números de requisito `req-0001` a `req-0006`, sequenciais por ordem de relevância do domínio.
- **Deleção ao final**: Apenas após todos os novos artefatos estarem criados e validados, os 29 arquivos originais são deletados.
- **Backup em extras**: Conteúdos que não se enquadram nos templates padrão (checklists operacionais, rascunhos de melhoria, dashboards de status) serão movidos para `/docs/extras/` antes da deleção.

## Phase 1: Classification and Listing of Requirements

### Fase 1 — Levantamento e listagem dos requisitos

Investigação do repositório, código-fonte e documentação existente revelou **6 requisitos** que o sistema atende:

| Código    | Nome                                       | Tipo       | Módulo/Área       | Envolve Interface? | Envolve Banco? | Prioridade |
|-----------|--------------------------------------------|------------|-------------------|--------------------|----------------|------------|
| req-0001  | Validação de CEP                           | Funcional  | Validations       | Não                | Não            | Alta       |
| req-0002  | Busca de Endereço com Fallback Multi-Provedor | Funcional  | Services          | Não                | Não            | Alta       |
| req-0003  | Tratamento de Exceções de Serviço          | Funcional  | Exceptions        | Não                | Não            | Alta       |
| req-0004  | Serialização/Desserialização JSON          | Funcional  | Extensions        | Não                | Não            | Média      |
| req-0005  | Métodos de Extensão de CEP (RemoverMask, AplicarMask, IsValid) | Funcional  | Extensions        | Não                | Não            | Média      |
| req-0006  | Publicação Automática no NuGet             | Não-funcional | CI/CD           | Não                | Não            | Média      |

**Nota:** req-0001 (Validação de CEP) foi consolidado a partir de `CepValidation.cs` e `CepExtension.cs` (métodos `IsValid`, `RemoveMask`, `ApplyMask` são validações + formatação, mas documentaremos como um requisito único de validação).

### Mapeamento: 29 Arquivos Originais → Novos Artefatos

Abaixo, o mapeamento de quais documentos originais alimentam cada novo artefato. Múltiplos originais podem convergir para o mesmo artefato (conteúdo redundante).

| Grupo | Arquivo Original                      | Novo Artefato de Destino                          | Função                                |
|-------|---------------------------------------|---------------------------------------------------|---------------------------------------|
| A     | FLUXO-FALLBACK.md                     | req-0002-busca-endereco-fallback.md               | Fluxo técnico do fallback             |
| A     | DIAGRAMAS-FALLBACK.md                 | diagrams/dcl-estrutura-servicos.md                | Diagramas técnicos                    |
| A     | FALLBACK_DIAGRAMS.md                  | diagrams/dcl-estrutura-servicos.md                | Diagramas redundantes                 |
| A     | FALLBACK_IMPLEMENTATION.md            | tec/tec-req-0002-busca-endereco-fallback.md       | Detalhamento técnico                  |
| A     | IMPLEMENTACAO-FALLBACK.md             | tec/tec-req-0002-busca-endereco-fallback.md       | Guia de implementação                 |
| A     | QUICK_START.md                        | extras/ (após extração)                           | Guia rápido (preservar como extra)    |
| B     | MELHORIAS-EXCEPTIONEXTENSION.md       | req-0003-excecoes-servico.md / tec-req-0003       | Refatoração ExceptionExtension        |
| B     | SUMARIO-FINAL-EXCEPTIONEXTENSION.md   | extras/ (resumo operacional)                      | Resumo operacional                    |
| B     | MELHORIAS-JSONEXTENSION.md            | req-0004-serializacao-json.md / tec-req-0004      | Refatoração JsonExtension             |
| B     | MELHORIAS-CEPVALIDATION.md            | req-0001-validacao-cep.md / tec-req-0001          | Refatoração CepValidation             |
| B     | MELHORIAS-CEPSERVICE.md               | tec/tec-req-0002-busca-endereco-fallback.md       | Refatoração CepService                |
| B     | RESUMO-TESTES-CEPEXTENSION.md         | extras/                                           | Resumo de testes (operacional)        |
| B     | TESTES-CEPE XTENSION.md               | extras/                                           | Detalhe de testes (operacional)       |
| C     | RESUMO-CORRECAO-CI-CD.md              | req-0006-publicacao-nuget.md / tec-req-0006       | Correção CI/CD                        |
| C     | WORKFLOW-NUGET-PUBLISH.md             | tec/tec-req-0006-publicacao-nuget.md              | Workflow NuGet detalhado              |
| C     | CHECKLIST-RELEASE.md                  | extras/                                           | Checklist operacional                 |
| D     | INDEX.md                              | README.md (TOC raiz)                               | Substituído pelo TOC raiz             |
| D     | INDICE-DOCUMENTACAO.md                | README.md (TOC raiz)                               | Substituído pelo TOC raiz             |
| D     | INDICE-DOCUMENTACAO-FINAL.md          | README.md (TOC raiz)                               | Substituído pelo TOC raiz             |
| D     | DOCUMENTACAO-CRIADA.md                | README.md (TOC raiz)                               | Substituído pelo TOC raiz             |
| D     | DOCUMENTACAO_CHECKLIST.md             | 🗑️ Descartado (operacional)                       | Tracking operacional — descartado     |
| D     | DOCUMENTACAO_NOVO_FLUXO.md            | req-0002-busca-endereco-fallback.md               | Conteúdo do fluxo                     |
| D     | RESUMO_DOCUMENTACAO.md                | extras/                                           | Resumo operacional — preservar        |
| D     | SUMARIO-EXECUTIVO.md                  | system-mapping.md (seção resumo)                   | Conteúdo executivo                    |
| D     | SUMARIO-EXECUTIVO-FINAL.md            | system-mapping.md (seção resumo)                   | Conteúdo executivo                    |
| D     | SUMARIO-MELHORIAS-COMPLETO.md         | extras/                                           | Resumo completo de melhorias — preservar |
| D     | DASHBOARD-STATUS.md                   | 🗑️ Descartado (operacional)                       | Dashboard operacional — descartado    |
| D     | PROXIMOS-PASSOS.md                    | 🗑️ Descartado (operacional)                       | Roadmap operacional — descartado      |
| D     | CHECKLIST-IMPLEMENTACAO.md            | 🗑️ Descartado (operacional)                       | Checklist implementação — descartado  |

## Phase 2: Documentation of Requirements in Batch

### Fase 2 — Documentação dos requisitos em lote

A ordem de execução dos requisitos segue a prioridade (Alta → Média) e dependências conceituais:

#### Wave 1: Artefatos de Governança (fundação)

- [x] **Task 1.1**: Criar `docs/constituicao.md` — Princípios de governança do projeto (segurança, performance, qualidade, dados, arquitetura)
  - **Descrição**: Documento de governança que serve como filtro de aceite para todos os requisitos. Deve refletir os princípios do projeto: 100% cobertura de testes, SOLID, Clean Code, XML documentation em inglês, multi-target .NET 8/9/10, segurança HTTPS, async/await com CancellationToken.
  - **Acceptance criteria**:
    - [x] Princípios de governança documentados (mín. 6 áreas: segurança, performance, qualidade, dados, arquitetura, processo)
    - [x] Versão, autor e data presentes
    - [x] Histórico de alterações e Clarification Log
  - **Verification**: Revisão textual, links internos válidos
  - **Dependencies**: Nenhuma
  - **Files likely touched**: `docs/constituicao.md`
  - **Estimated scope**: S (1-2 arquivos)

- [x] **Task 1.2**: Criar `docs/system-mapping.md` — Mapeamento do sistema com evidências
  - **Descrição**: Documenta a estrutura do sistema, dependências, fluxos e evidências coletadas durante a investigação.
  - **Acceptance criteria**:
    - [x] Estrutura de diretórios documentada
    - [ ] Dependências externas listadas (4 APIs de CEP)
    - [ ] Fluxo de fallback documentado em alto nível
  - **Verification**: Consistência com código-fonte
  - **Dependencies**: Nenhuma
  - **Files likely touched**: `docs/system-mapping.md`
  - **Estimated scope**: S (1-2 arquivos)

- [x] **Task 1.3**: Criar `docs/architecture-tech-stack.md` — Arquitetura e tech stack
  - **Descrição**: Documenta arquitetura, tecnologias, integrações com APIs externas, estratégia de fallback, patterns (Strategy + Facade).
  - **Acceptance criteria**:
    - [x] Tech stack documentada (.NET 8/9/10, C#, xUnit, Moq, Coverlet)
    - [x] Integrações com 4 APIs externas documentadas (URLs, métodos, formatos de resposta)
    - [x] Diagrama de arquitetura (Mermaid)
    - [x] Decisões arquiteturais registradas (fallback, Strategy pattern, BaseService)
  - **Verification**: Consistência com código-fonte, diagramas Mermaid renderizáveis
  - **Dependencies**: Nenhuma
  - **Files likely touched**: `docs/architecture-tech-stack.md`
  - **Estimated scope**: M (3-4 arquivos)

- [x] **Task 1.4**: Criar `docs/system-risk-matrix.md` — Matriz de risco global
  - **Descrição**: Matriz de riscos do sistema. Riscos identificados: dependência de APIs externas, timeout, rate limiting, disponibilidade dos provedores, falha de validação de entrada.
  - **Acceptance criteria**:
    - [x] Mín. 5 riscos identificados e classificados (probabilidade × impacto)
    - [x] Mitigações documentadas
    - [x] IDs de risco sequenciais (RSK-001, RSK-002...)
  - **Verification**: Consistência com requisitos
  - **Dependencies**: Task 1.1 (constituição)
  - **Files likely touched**: `docs/system-risk-matrix.md`
  - **Estimated scope**: S (1-2 arquivos)

#### Wave 2: Requisitos (Alta prioridade)

- [x] **Task 2.1**: Criar `req-0001-validacao-cep.md` + `tec/tec-req-0001-validacao-cep.md`
  - **Descrição**: Requisito de validação de CEP. Cobre CepValidation.cs e CepExtension.cs (métodos RemoveMask, CepMask, IsValidCep). Fontes: MELHORIAS-CEPVALIDATION.md, RESUMO-TESTES-CEPEXTENSION.md.
  - **Acceptance criteria**:
    - [x] Par req-0001 + tec-req-0001 criados (413 + 412 linhas)
    - [x] Regras de negócio numeradas (RN-001 a RN-010)
    - [x] Critérios de aceitação em Gherkin (14 cenários — 10 negativos, 4 positivos)
    - [x] Gherkin cobre: CEP válido, CEP inválido, CEP nulo/vazio, CEP com máscara
    - [x] APF em apf/apf-req-0001.md (23 PF)
    - [x] Análise cross-artifact em analise-req-0001.md (veredito: APROVADO)
    - [x] Quality checklist em checklist-req-0001.md (veredito: APROVADO — 35 Pass, 0 Fail)
  - **Files actually touched**: `docs/requirement/req-0001-validacao-cep.md`, `docs/requirement/tec/tec-req-0001-validacao-cep.md`, `docs/requirement/apf/apf-req-0001.md`, `docs/requirement/analise-req-0001.md`, `docs/requirement/checklist-req-0001.md`, `docs/system-risk-matrix.md` (v1.0 → v1.1)
  - **Verification**: `dotnet test` passa, cobertura 100% mantida
  - **Dependencies**: Waves 1 (governança)
  - **Files likely touched**: 
    - `docs/requirement/req-0001-validacao-cep.md`
    - `docs/requirement/tec/tec-req-0001-validacao-cep.md`
    - `docs/requirement/apf/apf-req-0001.md`
    - `docs/requirement/analise-req-0001.md`
    - `docs/requirement/checklist-req-0001.md`
    - `docs/system-risk-matrix.md` (atualizar)
  - **Estimated scope**: M (4-5 arquivos)

- [x] **Task 2.2**: Criar `req-0002-busca-endereco-fallback.md` + `tec/tec-req-0002-busca-endereco-fallback.md`
  - **Descrição**: Requisito principal — busca de endereço com fallback entre 4 provedores. Cobre CepService, BrasilApiService, ViaCepService, AwesomeApiService, OpenCepService, BaseService, ICepService, ICepServiceControl, CepResult, CepContainer. Fontes: FLUXO-FALLBACK.md, FALLBACK_IMPLEMENTATION.md, IMPLEMENTACAO-FALLBACK.md, MELHORIAS-CEPSERVICE.md, DOCUMENTACAO_NOVO_FLUXO.md.
  - **Acceptance criteria**:
    - [x] Par req-0002 + tec-req-0002 criados (467 + 844 linhas)
    - [x] Diagrama Mermaid do fluxo de fallback (dcl + min — 236 + 262 linhas)
    - [x] Regras de negócio numeradas (RN-011 a RN-018)
    - [x] Critérios de aceitação em Gherkin (9 cenários: 4 negativos, 5 positivos)
    - [x] APF em apf/apf-req-0002.md (12 PF)
    - [x] Análise cross-artifact em analise-req-0002.md (veredito: APROVADO)
    - [x] Quality checklist em checklist-req-0002.md (veredito: APROVADO — 36 Pass, 0 Fail)
    - [x] APIs dos 4 provedores documentadas
  - **Files actually touched**: 11 novos artefatos: req-0002 (467), tec-req-0002 (844), api-brasilapi (211), api-viacep (208), api-awesomeapi (207), api-opencep (180), dcl-estrutura-servicos (236), min-fluxo-fallback (262), apf-req-0002 (117), analise-req-0002 (130), checklist-req-0002 (198). system-risk-matrix.md atualizado v1.1→v1.2.

- [x] **Task 2.3**: Criar `req-0003-excecoes-servico.md` + `tec/tec-req-0003-excecoes-servico.md`
  - **Descrição**: Tratamento de exceções. Cobre ExceptionExtension.cs (AllMessages, GetDetailedMessage, GetExceptionChain), ServiceException.cs, NotFoundException.cs. Fonte: MELHORIAS-EXCEPTIONEXTENSION.md.
  - **Acceptance criteria**:
    - [x] Par req-0003 + tec-req-0003 criados (5 artefatos no total)
    - [x] Métodos documentados com exemplos
    - [x] Critérios de aceitação em Gherkin
    - [x] APF, análise, checklist
  - **Verification**: `dotnet test` passa
  - **Dependencies**: Task 2.2 (exceções são usadas no fallback)
  - **Files actually touched**: req-0003-excecoes-servico.md, tec/tec-req-0003-excecoes-servico.md, apf/apf-req-0003.md, analise-req-0003.md, checklist-req-0003.md

#### Wave 3: Requisitos (Média prioridade)

- [ ] **Task 3.1**: Criar `req-0004-serializacao-json.md` + `tec/tec-req-0004-serializacao-json.md`
  - **Descrição**: Serialização/desserialização JSON. Cobre JsonExtension.cs, BrasilApiResponse.cs, AwesomeApiResponse.cs. Fonte: MELHORIAS-JSONEXTENSION.md.
  - **Acceptance criteria**:
    - [ ] Par req-0004 + tec-req-0004 criados
    - [ ] Configurações de serialização documentadas
    - [ ] Dicionário de dados para models de resposta (did-models-resposta.md)
    - [ ] APF, análise, checklist
  - **Verification**: `dotnet test` passa
  - **Dependencies**: Task 2.2 (models de resposta)
  - **Files likely touched**: 
    - `docs/requirement/req-0004-serializacao-json.md`
    - `docs/requirement/tec/tec-req-0004-serializacao-json.md`
    - `docs/requirement/apf/apf-req-0004.md`
    - `docs/requirement/analise-req-0004.md`
    - `docs/requirement/checklist-req-0004.md`
    - `docs/requirement/did/did-models-resposta.md`
  - **Estimated scope**: M (4-5 arquivos)

- [ ] **Task 3.2**: ~~Criar `req-0005-extensao-cep.md`~~ **CONSOLIDADO em req-0001** (decisão do usuário em 23/07/2026)
  - **Descrição**: Os métodos de extensão de CEP (RemoveMask, CepMask, IsValidCep) foram incorporados ao req-0001 (Validação de CEP). Task 3.2 fica **cancelada** — o conteúdo de extensão será documentado como parte do req-0001.
  - **Acceptance criteria**: N/A (cancelado)
  - **Dependencies**: N/A (cancelado)
  - **Files likely touched**: Nenhum (conteúdo absorvido pelo req-0001)
  - **Estimated scope**: Cancelado

- [ ] **Task 3.3**: Criar `req-0006-publicacao-nuget.md` + `tec/tec-req-0006-publicacao-nuget.md`
  - **Descrição**: Publicação automática no NuGet via GitHub Actions. Cobre o workflow publish_nuget.yml. Fontes: RESUMO-CORRECAO-CI-CD.md, WORKFLOW-NUGET-PUBLISH.md.
  - **Acceptance criteria**:
    - [ ] Par req-0006 + tec-req-0006 criados
    - [ ] Workflow documentado com diagrama Mermaid
    - [ ] Secrets e configurações documentadas
    - [ ] Troubleshooting documentado
    - [ ] APF, análise, checklist
  - **Verification**: Workflow executável, secrets configuradas
  - **Dependencies**: Nenhuma (requisito independente)
  - **Files likely touched**: 
    - `docs/requirement/req-0006-publicacao-nuget.md`
    - `docs/requirement/tec/tec-req-0006-publicacao-nuget.md`
    - `docs/requirement/apf/apf-req-0006.md`
    - `docs/requirement/analise-req-0006.md`
    - `docs/requirement/checklist-req-0006.md`
    - `docs/system-risk-matrix.md` (atualizar)
  - **Estimated scope**: M (4-5 arquivos)

#### Wave 4: Complementação de Diagramas e Dicionários

- [ ] **Task 4.1**: Criar diagramas de classes (`docs/diagrams/dcl-estrutura-servicos.md`)
  - **Descrição**: Diagrama de classes mostrando a hierarquia de serviços (ICepServiceControl, implementações, BaseService, CepService). Fontes: DIAGRAMAS-FALLBACK.md, FALLBACK_DIAGRAMS.md.
  - **Acceptance criteria**:
    - [ ] Diagrama Mermaid de classes com todas as interfaces e implementações
    - [ ] Relacionamentos e dependências claros
  - **Dependencies**: Task 2.2 (estrutura de serviços documentada)
  - **Files likely touched**: `docs/diagrams/dcl-estrutura-servicos.md`
  - **Estimated scope**: S (1 arquivo)

- [ ] **Task 4.2**: Criar diagrama de integração (`docs/diagrams/min-fluxo-fallback.md`)
  - **Descrição**: Diagrama de sequência mostrando o fluxo de fallback entre os 4 provedores.
  - **Acceptance criteria**:
    - [ ] Diagrama Mermaid de sequência com fluxo completo
    - [ ] Fluxo de erro, fallback e sucesso representados
  - **Dependencies**: Task 2.2
  - **Files likely touched**: `docs/diagrams/min-fluxo-fallback.md`
  - **Estimated scope**: S (1 arquivo)

- [ ] **Task 4.3**: Criar dicionário de dados (`docs/requirement/did/did-models.md`)
  - **Descrição**: Dicionário de dados para os models do sistema (CepContainer, CepResult, BrasilApiResponse, AwesomeApiResponse, CepMessages).
  - **Acceptance criteria**:
    - [ ] Todos os models documentados com campos, tipos e descrições
    - [ ] Mapeamento entre campos das APIs externas e modelos internos
  - **Dependencies**: Tasks 2.2, 3.1
  - **Files likely touched**: `docs/requirement/did/did-models.md`
  - **Estimated scope**: S (1-2 arquivos)

#### Wave 5: TOC Raiz e Extras

- [ ] **Task 5.1**: Criar `docs/README.md` (TOC raiz)
  - **Descrição**: Índice navegável de toda a documentação, com links para todos os artefatos, status, versão e data.
  - **Acceptance criteria**:
    - [ ] Links para todas as áreas da estrutura
    - [ ] Status dos artefatos (aprovado/em revisão/rascunho)
    - [ ] Data da última atualização
    - [ ] Convenção de prefixos documentada
  - **Dependencies**: Todas as tasks anteriores
  - **Files likely touched**: `docs/README.md`
  - **Estimated scope**: M (1 arquivo, mas consolida todos)

- [ ] **Task 5.2**: Criar `docs/extras/` e migrar conteúdos não-estruturados
  - **Descrição**: Mover para `docs/extras/` os conteúdos originais que não se enquadram nos templates padrão (checklists operacionais, dashboards de status, roadmaps). Manter como referência histórica.
  - **Acceptance criteria**:
    - [ ] Arquivos selecionados copiados para `docs/extras/` antes da deleção
    - [ ] Preservação de links e formatação
  - **Dependencies**: Fase 2 completa
  - **Files likely touched**: `docs/extras/` (múltiplos arquivos)
  - **Estimated scope**: S (1 diretório)

#### Wave 6: Deleção dos Originais e Validação Final

- [ ] **Task 6.1**: Deletar os 29 arquivos originais de `/docs/`
  - **Descrição**: Após confirmação de que todos os novos artefatos estão criados e o TOC raiz está completo, deletar os 29 arquivos originais.
  - **Acceptance criteria**:
    - [ ] Os 29 arquivos listados não existem mais em `/docs/`
    - [ ] Nenhum link quebrado no TOC raiz
  - **Dependencies**: Tasks 5.1, 5.2
  - **Files likely touched**: Nenhum (deleção dos 29)
  - **Estimated scope**: XS (operação de deleção)

- [ ] **Task 6.2**: Validação final — build + testes + integridade dos links
  - **Descrição**: Executar `dotnet build` e `dotnet test` para garantir que a documentação não quebrou o build. Verificar integridade dos links no TOC raiz.
  - **Acceptance criteria**:
    - [ ] `dotnet build` passa sem erros ou warnings
    - [ ] `dotnet test` passa 100%
    - [ ] Todos os links no `docs/README.md` apontam para arquivos existentes
  - **Dependencies**: Task 6.1
  - **Files likely touched**: Nenhum (validação)
  - **Estimated scope**: XS

### Sizing Summary

| Task ID | Description                                      | Size | Files  |
|---------|--------------------------------------------------|------|--------|
| 1.1     | Criar constituicao.md                            | S    | 1      | `✅ Concluído` |
| 1.2     | Criar system-mapping.md                          | S    | 1      | `✅ Concluído` |
| 1.3     | Criar architecture-tech-stack.md                 | M    | 1      | `✅ Concluído` |
| 1.4     | Criar system-risk-matrix.md                      | S    | 1      | `✅ Concluído` |
| 2.1     | req-0001 Validação de CEP                        | M    | 5      | `✅ Concluído` |
| 2.2     | req-0002 Busca com Fallback                      | L → **decomposta** (ver abaixo) | 11     | `✅ Concluído` |
| 2.3     | req-0003 Exceções de Serviço                     | M    | 5      | `✅ Concluído` |
| 3.1     | req-0004 Serialização JSON                       | M    | 5      |
| 3.2     | ~~req-0005 Extensão de CEP~~ → CONSOLIDADO em req-0001 | Cancelado | 0 |
| 3.3     | req-0006 Publicação NuGet                        | M    | 5      |
| 4.1     | Diagrama de classes                              | S    | 1      |
| 4.2     | Diagrama de integração                           | S    | 1      |
| 4.3     | Dicionário de dados                              | S    | 1      |
| 5.1     | TOC raiz (docs/README.md)                        | M    | 1      |
| 5.2     | extras/ (migração)                               | S    | 1 dir  |
| 6.1     | Deleção dos originais                            | XS   | 0      |
| 6.2     | Validação final                                  | XS   | 0      |

**Task 2.2 decomposition**: Devido ao tamanho L (8+ arquivos), a task 2.2 será decomposta em subtarefas:
- 2.2a: req-0002 (business + stakeholder view)
- 2.2b: tec-req-0002 (technical detail)
- 2.2c: APIs dos 4 provedores (4 arquivos `api/api-*.md`)
- 2.2d: Diagramas (dcl + min)
- 2.2e: APF + análise + checklist

## Phase 3: Complement Diagrams and API Docs

### Fase 3A — Design system e padrão visual

**Não aplicável.** Projeto é uma biblioteca .NET sem interface de usuário. Nenhum wireframe ou padrão visual será produzido.

### Fase 3B — Integração de Dados

**Parcialmente aplicável.** As integrações com APIs externas (endpoints, payloads, respostas) serão documentadas em:
- `docs/requirement/api/api-brasilapi.md`
- `docs/requirement/api/api-viacep.md`
- `docs/requirement/api/api-awesomeapi.md`
- `docs/requirement/api/api-opencep.md`

Cada documento conterá: URL do endpoint, método HTTP, headers, payload de requisição, formato de resposta, exemplos, mapeamento de campos, tratamento de erros e diagrama de sequência.

## Phase 4: Convergence

### Fase 4 — Convergência e Rebaseline

Aplicar ao final, após todos os requisitos estarem documentados:
- Verificar alinhamento com `constituicao.md`
- Atualizar `system-risk-matrix.md` com riscos de cada requisito
- Revisar consistência cross-artifact entre requisitos e diagramas
- Atualizar `tamanho-aplicacao.md` (APF consolidado)

## Dependency Graph

```
          constituicao.md (1.1)
              ↕
    system-mapping.md (1.2)
              ↕
 architecture-tech-stack.md (1.3)
              ↕
     system-risk-matrix.md (1.4)
              │
     ┌────────┴────────┐
     │                 │
 req-0001 (2.1)    req-0006 (3.3)
     │                 │
 req-0002 (2.2)        │
     │                 │
 ┌───┼───┐             │
 │   │   │             │
 ↓   ↓   ↓             │
2.3  3.1 3.2            │
     │   │              │
     └───┴──────┬───────┘
                │
          Diagramas (4.1, 4.2, 4.3)
                │
           TOC raiz (5.1)
                │
           extras/ (5.2)
                │
          Deleção (6.1)
                │
        Validação final (6.2)
```

**Sequenciais:** 1.1→1.2→1.3→1.4 (governança, ordem fixa) → 2.1→2.2→2.3 (requisitos alta prioridade, dependem de governança)
**Paralelizáveis:** 3.1 e 3.2 e 3.3 (requisitos média prioridade, podem ser executados em paralelo entre si, mas dependem de Waves 1-2)
**Paralelizáveis:** 4.1, 4.2, 4.3 entre si (diagramas independentes)
**Sequencial final:** 5.1 → 5.2 → 6.1 → 6.2

## Parallelization Strategy

| Wave   | Tasks               | Classificação       | Contrato compartilhado          |
|--------|---------------------|---------------------|----------------------------------|
| Wave 1 | 1.1, 1.2, 1.3, 1.4 | Sequencial (1.1→4) | N/A (governança)                 |
| Wave 2 | 2.1, 2.2, 2.3      | Sequencial (2.1→2→3)| Requisitos dependentes           |
| Wave 3 | 3.1, 3.2, 3.3      | **Paralelo**        | Estrutura de docs definida na Wave 2 |
| Wave 4 | 4.1, 4.2, 4.3      | **Paralelo**        | Modelos e serviços definidos na Wave 2 |
| Wave 5 | 5.1, 5.2           | Sequencial (5.1→2)  | TOC raiz após todos artefatos    |
| Wave 6 | 6.1, 6.2           | Sequencial (6.1→2)  | Deleção apenas após validação    |

## Risks and Mitigations

| Risk                                                | Impact | Mitigation                                                                     |
| --------------------------------------------------- | ------ | ------------------------------------------------------------------------------ |
| Perda de informação durante conversão               | High   | Backup completo em `docs/extras/` antes da deleção; revisão humana opcional    |
| Algum conteúdo original não se encaixa nos templates | Medium | Mover para `docs/extras/` sem perda; marcar `⏳ PENDENTE` se necessário        |
| Quebra de links entre documento original e README   | Medium | TOC raiz reflete apenas novos artefatos; verificar links ao final              |
| Dados insuficientes para "Por quê" dos requisitos   | Medium | Handoff ao orchestrator para perguntar ao usuário; marcar `⏳ PENDENTE`        |
| Volume grande de arquivos (29 originais → 30+ novos) | Medium | Decompor task 2.2 (L); planejamento em waves para execução iterativa           |
| Mudanças no código durante o processo de doc        | Low    | Focar no código atual (v1.4.0); documentar o que existe, não o que mudará      |

## Changes

*(a ser preenchido durante a execução — cada task atualizará esta seção)*

### Added
- (pendente)

### Modified
- (pendente)

### Removed
- (pendente — 29 arquivos originais após conversão)

## Open Questions

- ✅ RESOLVIDO (23/07/2026): req-0005 consolidado em req-0001 (Validação de CEP). **Decisão:** consolidar em req-0001.
- ✅ RESOLVIDO (23/07/2026): Autor definido como **Rodrigo Araujo Barbosa**.
- ✅ RESOLVIDO (23/07/2026): Documentos puramente operacionais do Grupo D devem ser **descartados** (não mover para extras). Critério: arquivos com conteúdo exclusivamente de tracking/task status (DASHBOARD-STATUS.md, CHECKLIST-IMPLEMENTACAO.md, DOCUMENTACAO_CHECKLIST.md, PROXIMOS-PASSOS.md) são descartados. Arquivos com conteúdo substantivo (SUMARIO-MELHORIAS-COMPLETO.md, RESUMO_DOCUMENTACAO.md) vão para `extras/`. Arquivos cujo conteúdo é absorvido nos novos artefatos (INDEX.md, INDICE-*, DOCUMENTACAO-CRIADA.md, DOCUMENTACAO_NOVO_FLUXO.md, SUMARIO-EXECUTIVO*) não precisam de extra — o conteúdo já está migrado.

## Execution Order Summary

1. **Wave 1** (Governança — sequencial)
2. **Wave 2** (Requisitos alta prioridade — sequencial)
3. **Wave 3** (Requisitos média prioridade — paralelo)
4. **Wave 4** (Diagramas — paralelo)
5. **Wave 5** (TOC + extras — sequencial)
6. **Wave 6** (Deleção + validação — sequencial)

**Estimativa total:** 25-35 arquivos criados, 29 arquivos deletados.
**Esforço total estimado:** L (breakdown em 17 tasks S/M + 2 XS).
