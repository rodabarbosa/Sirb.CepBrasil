# APF — Análise de Pontos de Função — req-0006

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | apf-req-0006             |
| **Requisito**        | req-0006 — Publicação Automática no NuGet |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |

---

## 1. Premissas

- A contagem segue o padrão IFPUG (International Function Point Users Group) — Contagem Estimada.
- As funções são identificadas a partir dos requisitos funcionais (RF-26 a RF-35).
- Este requisito é **não funcional (CI/CD)** — a análise de pontos de função é adaptada para considerar o pipeline de automação como um conjunto de funções transacionais.
- Não há arquivos lógicos internos (ALI) ou arquivos de interface externa (AIE), pois o pipeline não persiste dados nem consulta arquivos externos (além do repositório Git).
- O pipeline de CI/CD é tratado como uma **consulta externa (CE)** para o processo de build/test e como uma **saída externa (SE)** para a publicação.
- RF-27 (testes), RF-28 (versão automática), RF-29 (manual), RF-34 (permissões) e RF-35 (bloqueio) são comportamentos internos do workflow e não geram funções transacionais separadas — estão agrupadas em RF-26 e RF-31.

---

## 2. Identificação de Funções

### 2.1 Funções de Dados

| Tipo | Função | Descrição | DER | Registros | Complexidade | PF |
|------|--------|-----------|-----|-----------|-------------|-----|
| — | — | Nenhuma — pipeline sem persistência de dados | — | — | — | 0 |

### 2.2 Funções Transacionais

| RF | Método / Processo | Descrição | Tipo | DETs | FTRs/ALRs | Complexidade | PF |
|----|-------------------|-----------|------|------|-----------|-------------|-----|
| RF-26 | `build-and-test` (job) | Executa build multi-target (.NET 8, 9, 10) e valida compilação | CE | 3 (3 plataformas alvo) | 1 (.csproj) | Baixa | 3 |
| RF-27 | *(parte de RF-26 — testes e gate de qualidade)* | — | — | — | — | — | — |
| RF-28 | *(parte de RF-31 — extração da versão da tag)* | — | — | — | — | — | — |
| RF-29 | *(parte de RF-31 — execução manual com versão explícita)* | — | — | — | — | — | — |
| RF-30 | *(parte de RF-31 — empacotamento com símbolos)* | — | — | — | — | — | — |
| RF-31 | `publish` (job) | Empacota e publica NuGet no NuGet.org com artefato | SE | 4 (versão, .nupkg, .snupkg, artefato) | 2 (NuGet.org, GitHub Actions) | Baixa | 4 |
| RF-32 | *(parte de RF-31 — publicação de símbolos com continue-on-error)* | — | — | — | — | — | — |
| RF-33 | *(parte de RF-31 — upload de artefato com retenção)* | — | — | — | — | — | — |
| RF-34 | *(parte do workflow — permissões declaradas no YAML)* | — | — | — | — | — | — |
| RF-35 | *(parte de RF-26 — dependência entre jobs via needs:)* | — | — | — | — | — | — |

**Legenda:**
- **EE** = External Input (Entrada Externa)
- **SE** = External Output (Saída Externa)
- **CE** = External Inquiry (Consulta Externa)
- **DET** = Data Element Type (campo de dado)
- **FTR/ALR** = File Type Referenced / Arquivo Lógico Referenciado

---

## 3. Resumo da Contagem

### 3.1 Totais por Tipo

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| EE (External Input) | 0 | 0 | 0 | 0 | 0 |
| SE (External Output) | 1 | 1 | 0 | 0 | 4 |
| CE (External Inquiry) | 1 | 1 | 0 | 0 | 3 |
| **Total** | **2** | **2** | **0** | **0** | **7** |

### 3.2 Fator de Ajuste

O fator de ajuste não é aplicado nesta contagem (contagem estimada não-ajustada).

### 3.3 Pontos de Função Não-Ajustados (PFNA)

| Item | Valor |
|------|-------|
| Total PFNA | **7 PF** |
| Funções de dados (ALI + AIE) | 0 PF |
| Funções transacionais (EE + SE + CE) | 7 PF |

---

## 4. Comparativo com Demais Requisitos

| Requisito | PF | Funções | Descrição |
|-----------|----|---------|-----------|
| req-0001 | 23 | 7 | Validação de CEP |
| req-0002 | 12 | 4 | Busca com fallback |
| req-0003 | — | — | Tratamento de exceções (a calcular) |
| req-0006 | 7 | 2 | Publicação automática NuGet (CI/CD) |
| **Total** | **42+** | **13+** | — |

---

## 5. Consolidação no Tamanho da Aplicação

Este requisito contribui com **7 PF** para o consolidado `docs/tamanho-aplicacao.md`. *(Documento não criado — pendente consolidação futura.)*

---

## 6. Premissas e Observações

1. O job `build-and-test` é contado como uma única função CE, pois encapsula restore + build multi-target + testes em uma operação única de validação.
2. O job `publish` é contado como SE porque produz uma saída externa (o pacote no NuGet.org) além de artefatos internos.
3. RF-28, RF-29, RF-30, RF-32, RF-33, RF-34 e RF-35 são comportamentos internos do workflow sem DETs ou FTRs adicionais suficientes para gerar funções separadas.
4. A natureza do requisito é não funcional (CI/CD) — a contagem APF é adaptada e serve como estimativa de esforço do pipeline, não como tamanho funcional no sentido tradicional.

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do documento APF |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
