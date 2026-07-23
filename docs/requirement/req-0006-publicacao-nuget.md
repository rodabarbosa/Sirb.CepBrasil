# req-0006 — Publicação Automática no NuGet

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | req-0006                 |
| **Título**           | Publicação Automática no NuGet |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |
| **Bundle**           | Não (requisito isolado)  |
| **Tipo**             | Não funcional (CI/CD)    |

---

## Histórico de Alterações

| Data       | Autor                 | Versão | Descrição                        |
|------------|-----------------------|--------|----------------------------------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0   | Criação do documento de requisito |

---

## 1. Introdução

### 1.1 Objetivo

Este documento especifica o requisito de **Publicação Automática no NuGet** da biblioteca Sirb.CepBrasil. Define a automação do pipeline de CI/CD para build, versionamento, testes e publicação do pacote NuGet no NuGet.org, garantindo que cada release seja publicada de forma consistente, segura e rastreável.

### 1.2 Escopo

**In scope:**

- Workflow de CI/CD no GitHub Actions para build e publicação
- Trigger automático ao publicar uma release no GitHub
- Trigger manual opcional via `workflow_dispatch` com especificação de versão
- Build multi-target (.NET 8, 9 e 10) em configuração Release
- Execução de testes unitários antes da publicação
- Empacotamento do NuGet com símbolos (.snupkg)
- Publicação no NuGet.org com skip-duplicate
- Upload de artefatos como backup (retenção de 30 dias)
- Versionamento explícito extraído da tag da release
- Qualidade: testes executados são gate para publicação

**Out of scope:**

- Cache de dependências no CI (a ser considerado em versão futura)
- Análise de segurança automatizada (CodeQL)
- Geração automática de changelog
- Notificações (Slack, Discord, e-mail)
- Publicação em feeds privados (NuGet interno, GitHub Packages)
- Assinatura de pacotes (authenticode / strong name)
- Publicação automática em múltiplos feeds simultaneamente

### 1.3 Glossário

| Termo | Definição |
|-------|-----------|
| CI/CD | Integração Contínua / Entrega Contínua — automação de build, teste e deploy |
| GitHub Actions | Plataforma de automação de workflows do GitHub |
| Release | Versão publicada no GitHub com tag semântica (ex.: v1.4.0) |
| NuGet.org | Repositório público de pacotes NuGet |
| `.nupkg` | Pacote NuGet compilado |
| `.snupkg` | Pacote de símbolos para debugging (Source Link) |
| Multi-target | Compilação para múltiplas versões do .NET simultaneamente |
| Skip-duplicate | Comportamento que evita erro ao tentar publicar versão já existente |
| Least privilege | Princípio de segurança: conceder apenas as permissões mínimas necessárias |
| SemVer | Versionamento Semântico (MAJOR.MINOR.PATCH) |
| `NUGET_API_KEY` | Secret do GitHub Actions contendo a chave de API do NuGet.org |

---

## 2. Intenção / Motivação de Negócio (Por quê)

### Problema de negócio real sendo resolvido

A biblioteca Sirb.CepBrasil é distribuída como pacote NuGet para consumidores .NET. Sem um pipeline automatizado de publicação, cada nova versão exige etapas manuais propensas a erro: build local, empacotamento manual, upload via navegador para o NuGet.org. Esse processo manual não executa testes consistentemente, não valida o build multi-target em todas as plataformas e não garante que apenas código testado e aprovado seja publicado.

### Por que importa (impacto mensurável)

- **Consistência:** todo pacote publicado passa pelo mesmo processo de build, teste e empacotamento, eliminando variações entre releases manuais.
- **Qualidade:** testes são executados automaticamente antes da publicação — se falharem, o pacote não é publicado. Isso impede que código com regressões chegue aos consumidores.
- **Rastreabilidade:** cada release no GitHub gera uma execução do workflow com logs completos, artefatos preservados por 30 dias e vínculo direto entre a tag, o changelog e o pacote publicado.
- **Segurança:** o workflow usa actions oficiais do GitHub (v4), permissões mínimas (least privilege) e a chave de API do NuGet armazenada como secret — nunca exposta em logs.
- **Velocidade:** o processo é acionado automaticamente ao publicar uma release, sem intervenção manual. O tempo total de publicação é de ~3-5 minutos.

### Quem é afetado

- **Mantenedor da biblioteca:** Rodrigo Araujo Barbosa — responsável por criar releases, configurar secrets e monitorar o workflow.
- **Consumidores da biblioteca:** desenvolvedores que instalam o pacote `Sirb.CepBrasil` via NuGet — recebem atualizações consistentes e testadas.
- **Equipe de operações:** monitora o pipeline de CI/CD e acompanha falhas de publicação.

### Critério de sucesso mensurável

- **Pipeline funcional:** uma release publicada no GitHub dispara automaticamente o workflow e o pacote é publicado no NuGet.org em até 10 minutos.
- **Gate de qualidade:** se qualquer teste unitário falhar, o workflow é interrompido e o pacote **não** é publicado.
- **100% dos builds passam** em todas as 3 plataformas alvo (.NET 8, 9 e 10).
- **Publicação manual alternativa:** o workflow pode ser acionado manualmente via GitHub Actions UI, com especificação explícita da versão.

---

## 3. Descrição Geral

### 3.1 Missão da Funcionalidade

Automatizar o processo de build, teste e publicação do pacote NuGet da biblioteca Sirb.CepBrasil, garantindo que toda release seja publicada com qualidade, segurança e rastreabilidade, sem intervenção manual no processo de publicação.

### 3.2 Atores

| Ator | Descrição |
|------|-----------|
| Mantenedor | Responsável por criar releases e monitorar o pipeline de CI/CD |
| GitHub Actions | Sistema de automação que executa o workflow |
| NuGet.org | Repositório destino do pacote |
| Consumidor | Desenvolvedor que consome o pacote publicado |

### 3.3 Contexto de Uso

O pipeline é composto por 2 jobs sequenciais:

1. **build-and-test:** executa checkout, setup .NET multi-target, restore, build em Release e testes
2. **publish:** dependente do job anterior — executa checkout, determina a versão, build com versão específica, pack com símbolos, push para NuGet.org e upload de artefato

---

## 4. Artefatos Relacionados

### Documentos que impactam este artefato

- `docs/constituicao.md` (seção 3.2 CI/CD — qualidade, testes obrigatórios antes da publicação)
- `docs/architecture-tech-stack.md` (stack .NET, GitHub Actions)
- `docs/system-mapping.md` (estrutura do repositório, dependências)
- `docs/system-risk-matrix.md` (matriz de risco global)
- `.github/workflows/publish_nuget.yml` (workflow de publicação)
- `docs/extra/WORKFLOW-NUGET-PUBLISH.md` (documentação do workflow)
- `docs/extra/RESUMO-CORRECAO-CI-CD.md` (resumo das correções de CI/CD)
- `docs/extra/CHECKLIST-RELEASE.md` (checklist de release)

### Documentos impactados por este artefato

- `docs/requirement/tec/tec-req-0006-publicacao-nuget.md`
- `docs/requirement/apf/apf-req-0006.md`
- `docs/requirement/analises/analise-req-0006.md`
- `docs/requirement/checklist/checklist-req-0006.md`
- `docs/system-risk-matrix.md`

### Componentes técnicos relacionados

| Componente | Caminho | Função |
|------------|---------|--------|
| Workflow de publicação | `.github/workflows/publish_nuget.yml` | Pipeline de CI/CD para build, teste e publicação |
| Projeto da biblioteca | `Sirb.CepBrasil/Sirb.CepBrasil.csproj` | Projeto multi-target empacotado como NuGet |
| Projeto de testes | `Sirb.CepBrasil.Test/Sirb.CepBrasil.Test.csproj` | Testes unitários executados no pipeline |
| Secret NuGet | `NUGET_API_KEY` (GitHub Secrets) | Chave de API para publicação no NuGet.org |
| Release no GitHub | Interface GitHub Releases | Gatilho automático do workflow |
| Documentação do workflow | `docs/extra/WORKFLOW-NUGET-PUBLISH.md` | Guia de uso do workflow |
| Checklist de release | `docs/extra/CHECKLIST-RELEASE.md` | Passos para criar uma release |

---

## 5. Requisitos Funcionais

| ID | Descrição | Prioridade | Regra de negócio associada |
|----|-----------|------------|----------------------------|
| RF-26 | Executar build multi-target (.NET 8, 9, 10) em configuração Release | Alta | RN-026 |
| RF-27 | Executar todos os testes unitários antes da publicação | Alta | RN-027, RN-031 |
| RF-28 | Extrair versão automaticamente da tag da release (ex.: v1.4.0 → 1.4.0) | Alta | RN-028 |
| RF-29 | Permitir execução manual do workflow com versão explícita | Alta | RN-029 |
| RF-30 | Empacotar NuGet com símbolos (.snupkg) | Alta | RN-030 |
| RF-31 | Publicar pacote no NuGet.org com skip-duplicate | Alta | RN-031 |
| RF-32 | Publicar símbolos no NuGet.org com continue-on-error | Média | RN-030 |
| RF-33 | Fazer upload do pacote como artefato do workflow (retenção 30 dias) | Média | RN-032 |
| RF-34 | Utilizar permissões mínimas (least privilege) no workflow | Alta | RN-033 |
| RF-35 | Bloquear publicação se o job build-and-test falhar | Alta | RN-027, RN-034 |

---

## 6. Regras de Negócio

| ID | Regra | Origem | Impacto |
|----|-------|--------|---------|
| RN-026 | O build deve ser multi-target para .NET 8, 9 e 10 simultaneamente em configuração Release | Compatibilidade multi-versão | Garante que o pacote funciona nas 3 plataformas |
| RN-027 | Testes unitários devem ser executados **antes** do empacotamento; se falharem, o workflow deve ser interrompido e o pacote não publicado | Gate de qualidade | Impede publicação de código com regressões |
| RN-028 | A versão do pacote é extraída da tag da release (removendo o prefixo "v") ou do input manual | Versionamento explícito | Garante rastreabilidade entre tag e versão publicada |
| RN-029 | O workflow pode ser acionado manualmente via `workflow_dispatch` com versão explícita como parâmetro obrigatório | Flexibilidade de controle | Permite republicação e publicação fora de release |
| RN-030 | O pacote NuGet deve incluir símbolos (.snupkg) para debugging via Source Link; a publicação de símbolos não deve bloquear o workflow se falhar | Debugging | Símbolos são opcionais; falha não impede publicação |
| RN-031 | A publicação no NuGet.org deve usar `--skip-duplicate` para evitar erro ao republicar versão já existente | Idempotência | Permite reexecução segura do workflow |
| RN-032 | O artefato .nupkg gerado deve ser preservado como artefato do workflow por 30 dias | Backup | Permite recuperação em caso de falha no push |
| RN-033 | O workflow deve usar permissões mínimas (`contents: read, packages: write`) | Segurança | Previne alterações não intencionais no repositório |
| RN-034 | O job de publicação (`publish`) deve depender do job de build e teste (`build-and-test`) via `needs:` | Ordem de execução | Garante sequência correta: testar antes de publicar |

---

## 7. Critérios de Aceitação (Gherkin)

```gherkin
Funcionalidade: Publicação Automática no NuGet
  Como mantenedor da biblioteca
  Quero publicar automaticamente o pacote no NuGet ao criar uma release
  Para garantir consistência, qualidade e rastreabilidade nas publicações

  Cenário: Release publicada dispara workflow automaticamente
    Dado que uma nova release é publicada no GitHub
    E que a tag da release segue o padrão "vX.Y.Z" (ex.: "v1.5.0")
    Quando o workflow "Publish to NuGet" é acionado
    Então o job build-and-test deve iniciar automaticamente
    E a versão extraída deve ser "X.Y.Z" (sem o prefixo "v")

  Cenário: Build e testes passam, pacote é publicado
    Dado que o job build-and-test executou com sucesso
    E que todos os testes passaram em .NET 8, 9 e 10
    Quando o job publish é executado
    Então o pacote .nupkg deve ser gerado
    E o pacote .snupkg deve ser gerado
    E ambos devem ser publicados no NuGet.org
    E o artefato deve ser salvo com retenção de 30 dias

  Cenário: Testes falham, publicação é bloqueada
    Dado que o job build-and-test está em execução
    E que pelo menos um teste unitário falha
    Quando o job build-and-test é concluído
    Então o workflow deve ser interrompido
    E o job publish não deve ser executado
    E nenhum pacote deve ser publicado no NuGet.org

  Cenário: Execução manual com versão explícita
    Dado que o mantenedor aciona o workflow manualmente
    E que a versão informada é "1.5.0"
    Quando o workflow é executado
    Então a versão "1.5.0" deve ser usada no build e pack
    E o pacote deve ser publicado com essa versão

  Cenário: Versão já existe no NuGet (skip-duplicate)
    Dado que a versão "1.4.0" já existe no NuGet.org
    E que o workflow tenta publicar "1.4.0"
    Quando o comando dotnet nuget push é executado
    Então o comando --skip-duplicate deve evitar erro
    E o workflow deve continuar sem falha

  Cenário: Falha na publicação de símbolos não bloqueia o workflow
    Dado que o pacote .nupkg foi publicado com sucesso
    E que a publicação do .snupkg falha
    Quando o step "Publish symbols to NuGet" é executado
    Então o workflow deve continuar com continue-on-error
    E a publicação do .nupkg não deve ser revertida

  Cenário: Build multi-target valida todas as plataformas
    Dado que o setup .NET configurou as versões 8.0.x, 9.0.x e 10.0.x
    Quando o comando dotnet build --configuration Release é executado
    Então o build deve compilar para net8.0, net9.0 e net10.0
    E nenhum warning de compilação deve ser gerado em nenhuma plataforma

  Cenário: Permissões mínimas são aplicadas
    Dado que o workflow está configurado
    Quando as permissões são verificadas
    Então contents deve ser "read"
    E packages deve ser "write"
    E nenhuma outra permissão deve ser concedida
```

---

## 8. Requisitos Não Funcionais (NFRs)

| Categoria | Requisito | Métrica/Critério |
|-----------|-----------|------------------|
| Desempenho | Tempo total de execução do workflow | ≤ 10 minutos (build + teste + publicação) |
| Desempenho | Build multi-target (.NET 8, 9, 10) simultâneo | Único comando dotnet build |
| Segurança | Chave de API armazenada como secret do GitHub | `NUGET_API_KEY` em GitHub Secrets |
| Segurança | Permissões mínimas no workflow | `contents: read, packages: write` |
| Segurança | Actions oficiais do GitHub (v4) | `actions/checkout@v4`, `actions/setup-dotnet@v4`, `actions/upload-artifact@v4` |
| Confiabilidade | Skip-duplicate habilitado | `--skip-duplicate` no comando de push |
| Confiabilidade | Símbolos com continue-on-error | Falha em .snupkg não bloqueia publicação |
| Confiabilidade | Artefato preservado por 30 dias | `retention-days: 30` no upload-artifact |
| Manutenibilidade | Workflow versionado no repositório | Arquivo `.github/workflows/publish_nuget.yml` |
| Manutenibilidade | Versionamento explícito por release tag | Compatível com SemVer |
| Disponibilidade | Trigger automático + manual | Release publicada + workflow_dispatch |

---

## 9. Wireframe

**Não aplicável.** Requisito de CI/CD — não envolve interface de usuário.

```
⏳ PENDENTE [Fase 3 — Design] — N/A: Requisito de pipeline CI/CD sem interface de usuário.
```

---

## 10. Integração de API / Banco de Dados

**Não aplicável.** O pipeline de CI/CD opera exclusivamente no ambiente do GitHub Actions — não envolve chamadas de API da aplicação ou banco de dados.

```
⏳ PENDENTE [Fase 3 — Dados] — N/A: Requisito não envolve banco de dados ou endpoints de API da aplicação.
```

### Integrações Externas do Pipeline

| Integração | Finalidade | Tipo |
|------------|------------|------|
| GitHub Actions | Execução do workflow | CI/CD runner |
| NuGet.org | Publicação do pacote | Push de pacote |
| GitHub Releases | Gatilho automático | Trigger via webhook |

---

## 11. Matriz de Risco do Requisito (Recorte Local)

| ID | Requisito | Descrição do Risco | Probabilidade | Impacto | Score | Nível | Mitigação | Status |
|----|-----------|-------------------|---------------|---------|-------|-------|-----------|--------|
| RSK-019 | req-0006 | Exposição da `NUGET_API_KEY` em logs ou artefatos do workflow | B (2) — Baixa | MA (5) — Muito Alto | 10 | 🟠 Alto | Secret armazenada no GitHub Secrets; workflow usa `${{ secrets.NUGET_API_KEY }}` que é mascarada automaticamente; permissões mínimas no workflow | 🟢 Aceito |
| RSK-020 | req-0006 | Build falha em uma das plataformas alvo (.NET 8, 9 ou 10) | B (2) — Baixa | A (4) — Alta | 8 | 🟡 Médio | Build multi-target no CI valida todas as plataformas simultaneamente; `EnforceCodeStyleInBuild = true` no .csproj | 🟢 Aceito |
| RSK-021 | req-0006 | Publicação de versão incorreta (tag vs versão esperada) | B (2) — Baixa | M (3) — Médio | 6 | 🟡 Médio | Versão extraída programaticamente da tag (remoção do prefixo "v"); workflow_dispatch exige versão explícita como input obrigatório | 🟢 Aceito |
| RSK-022 | req-0006 | Workflow não executa por falta de permissões ou token expirado | B (2) — Baixa | M (3) — Médio | 6 | 🟡 Médio | Permissões declaradas no workflow (`contents: read, packages: write`); GITHUB_TOKEN é gerenciado automaticamente pelo GitHub Actions | 🟢 Aceito |
| RSK-023 | req-0006 | NuGet.org fora do ar no momento da publicação | B (2) — Baixa | A (4) — Alta | 8 | 🟡 Médio | Artefato preservado por 30 dias permite republicação manual posterior; skip-duplicate evita conflito ao republicar | 🟡 Mitigado |
| RSK-024 | req-0006 | Testes com falha intermitente (flaky tests) bloqueiam publicação | M (3) — Média | M (3) — Médio | 9 | 🟡 Médio | Workflow pode ser reexecutado manualmente; testes 100% determinísticos sem dependência externa | 🟡 Mitigado |

---

## 12. Análise de Pontos de Função (APF) Vinculada

- **Documento APF:** `docs/requirement/apf/apf-req-0006.md`
- **Total estimado:** ver documento APF específico.

---

## 13. Impactos Técnicos

### CI/CD / Infraestrutura

- `.github/workflows/publish_nuget.yml` — workflow de build, teste e publicação
- GitHub Secrets (`NUGET_API_KEY`) — chave de API do NuGet.org
- GitHub Releases — gatilho do workflow

### Backend / Código

- `Sirb.CepBrasil/Sirb.CepBrasil.csproj` — projeto multi-target com configuração de empacotamento
- `Sirb.CepBrasil.Test/Sirb.CepBrasil.Test.csproj` — projeto de testes executados no pipeline

### Frontend

- N/A — biblioteca .NET sem UI

### Banco de Dados

- N/A — sem persistência

### Integrações Externas

| Integração | Tipo | Segurança |
|------------|------|-----------|
| NuGet.org (push) | Publicação de pacote | API Key via GitHub Secrets |
| GitHub Actions | Runner de CI/CD | GITHUB_TOKEN automático |

### Design / UX

- N/A — sem interface de usuário

---

## 14. Rastreabilidade

| Item | Referência |
|------|------------|
| Workflow CI/CD | `.github/workflows/publish_nuget.yml` (118 linhas) |
| Projeto Sirb.CepBrasil | `Sirb.CepBrasil/Sirb.CepBrasil.csproj` (multi-target net8.0;net9.0;net10.0) |
| Projeto de testes | `Sirb.CepBrasil.Test/Sirb.CepBrasil.Test.csproj` |
| Documento técnico | `docs/requirement/tec/tec-req-0006-publicacao-nuget.md` |
| APF | `docs/requirement/apf/apf-req-0006.md` |
| Análise cross-artifact | `docs/requirement/analises/analise-req-0006.md` |
| Quality Checklist | `docs/requirement/checklist/checklist-req-0006.md` |
| Matriz de risco global | `docs/system-risk-matrix.md` |
| Constituição | `docs/constituicao.md` (seção 3.2 — CI/CD) |
| Documentos de apoio | `docs/extra/WORKFLOW-NUGET-PUBLISH.md`, `docs/extra/RESUMO-CORRECAO-CI-CD.md`, `docs/extra/CHECKLIST-RELEASE.md` |

---

## 15. Validações Realizadas para esta Documentação

- [x] README do projeto analisado
- [x] Código do workflow analisado (`.github/workflows/publish_nuget.yml`)
- [x] Projeto `.csproj` da biblioteca analisado (configurações de empacotamento)
- [x] Projeto `.csproj` de testes analisado
- [x] Documentos de apoio consultados (`WORKFLOW-NUGET-PUBLISH.md`, `RESUMO-CORRECAO-CI-CD.md`, `CHECKLIST-RELEASE.md`)
- [x] Constituição do projeto revisada (seção CI/CD)
- [x] Arquitetura e tech stack revisados
- [x] Artefatos relacionados revisados (req-0001, req-0002, req-0003)

---

## 16. Clarification Log

| Data | Pergunta | Resposta | Status (Resolvido/Pendente) | Origem (Criação/Implementação) | Impacto no requisito |
|------|----------|----------|----------------------------|-------------------------------|----------------------|
| 23/07/2026 | Qual nome de usuário deve constar no campo `Autor` e nas entradas de histórico deste documento? | Rodrigo Araujo Barbosa | Resolvido | Criação | Define autor do documento |
| 23/07/2026 | O workflow atual executa testes ou estão comentados? | Os steps de teste estão comentados no workflow (`#`) conforme linhas 46-50 do publish_nuget.yml | Resolvido | Criação | RN-027 documenta que testes devem ser gate; o workflow atual tem os steps comentados — pendente ativação |
| 23/07/2026 | A numeração de RF/RN deve continuar a partir de req-0003 (RF-25 e RN-025)? | Sim, iniciar RF em RF-26 e RN em RN-026 | Resolvido | Criação | Define numeração dos requisitos e regras |

### Cobertura do Clarification (8 áreas)

| # | Área | Status (Resolvido \| N/A) | Ref. entrada no Log | Justificativa (se N/A) |
|---|------|---------------------------|---------------------|------------------------|
| 1 | Atores e personas | Resolvido | 23/07/2026 — Autor definido | — |
| 2 | Fluxos principais e alternativos | Resolvido | Coberto nos 8 cenários Gherkin (release automática, manual, falha de teste, skip-duplicate) | — |
| 3 | Exceções e erros | Resolvido | 23/07/2026 — Testes comentados documentados como pendência; falha de teste bloqueia publicação | — |
| 4 | Integrações externas | Resolvido | 23/07/2026 — NuGet.org, GitHub Actions, GitHub Releases documentados | — |
| 5 | Requisitos não-funcionais | Resolvido | Coberto na seção NFR (desempenho ≤ 10min, segurança least privilege, confiabilidade) | — |
| 6 | Dados e privacidade | N/A | — | Pipeline não processa dados pessoais; apenas chave de API armazenada como secret |
| 7 | Validações e regras de negócio | Resolvido | Coberto nas RN-026 a RN-034 | — |
| 8 | Critérios de aceite e mensurabilidade | Resolvido | Coberto nos 8 cenários Gherkin | — |

**Cobertura atual:** 7/8 (Resolvido). **Meta:** 8/8.

---

## 17. Esclarecimentos Adicionais

| Data | Nota |
|------|------|
| 23/07/2026 | Documento criado com base no workflow `.github/workflows/publish_nuget.yml` (v2.0) e documentos de apoio em docs/extra |
| 23/07/2026 | Os steps de teste no workflow atual estão comentados (linhas 46-50). Para atender à RN-027 (gate de qualidade), devem ser ativados antes da primeira publicação |
| 23/07/2026 | Este requisito é não funcional (CI/CD) — não há código de aplicação associado, apenas configuração de pipeline |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
