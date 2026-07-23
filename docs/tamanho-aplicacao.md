# Tamanho da Aplicação — Sirb.CepBrasil

> **Propósito:** Consolidar a Análise de Pontos de Função (APF) de todos os requisitos do sistema, fornecendo uma visão unificada do tamanho funcional estimado.

| Campo           | Valor                    |
|-----------------|--------------------------|
| **Versão**      | v1.1                     |
| **Data**        | 23/07/2026               |
| **Autor**       | Rodrigo Araujo Barbosa   |
| **Status**      | `rascunho`               |

---

## Histórico de Alterações

| Data       | Versão | Descrição                                    | Autor                 |
|------------|--------|-----------------------------------------------|-----------------------|
| 23/07/2026 | v1.1   | Atualização consolidada com req-0003 (12 PF) e req-0004 (15 PF); total 69 PF | Rodrigo Araujo Barbosa |
| 23/07/2026 | v1.0   | Criação do consolidado com req-0001, req-0002 e req-0006 | Rodrigo Araujo Barbosa |

---

## 1. Resumo Consolidado

| Requisito | Descrição | PF | Funções | Tipo |
|-----------|-----------|----|---------|------|
| req-0001  | Validação de CEP | 23 | 7 | Funcional (validação) |
| req-0002  | Busca de Endereço com Fallback Multi-Provedor | 12 | 4 | Funcional (orquestração) |
| req-0003  | Tratamento de Exceções de Serviço | 12 | 4 | Funcional (exceções) |
| req-0004  | Serialização/Desserialização JSON | 15 | 5 | Funcional (serialização) |
| req-0006  | Publicação Automática no NuGet | 7 | 2 | Não funcional (CI/CD) |
| **Total** | | **69** | **22** | — |

> **Nota:** req-0005 foi cancelado e não faz parte do escopo da aplicação.

---

## 2. Detalhamento por Requisito

### 2.1 req-0001 — Validação de CEP (23 PF)

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| EE (External Input) | 1 | 1 | 0 | 0 | 3 |
| SE (External Output) | 2 | 2 | 0 | 0 | 8 |
| CE (External Inquiry) | 4 | 4 | 0 | 0 | 12 |
| ALI (Internal Logical File) | 0 | 0 | 0 | 0 | 0 |
| AIE (External Interface File) | 0 | 0 | 0 | 0 | 0 |
| **Total** | **7** | **7** | **0** | **0** | **23** |

**Referência:** `docs/requirement/apf/apf-req-0001.md`

### 2.2 req-0002 — Busca de Endereço com Fallback Multi-Provedor (12 PF)

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| EE (External Input) | 2 | 2 | 0 | 0 | 6 |
| SE (External Output) | 0 | 0 | 0 | 0 | 0 |
| CE (External Inquiry) | 2 | 2 | 0 | 0 | 6 |
| ALI (Internal Logical File) | 0 | 0 | 0 | 0 | 0 |
| AIE (External Interface File) | 0 | 0 | 0 | 0 | 0 |
| **Total** | **4** | **4** | **0** | **0** | **12** |

**Referência:** `docs/requirement/apf/apf-req-0002.md`

### 2.3 req-0003 — Tratamento de Exceções de Serviço (12 PF)

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| EE (External Input) | 0 | 0 | 0 | 0 | 0 |
| SE (External Output) | 0 | 0 | 0 | 0 | 0 |
| CE (External Inquiry) | 4 | 4 | 0 | 0 | 12 |
| ALI (Internal Logical File) | 0 | 0 | 0 | 0 | 0 |
| AIE (External Interface File) | 0 | 0 | 0 | 0 | 0 |
| **Total** | **4** | **4** | **0** | **0** | **12** |

**Referência:** `docs/requirement/apf/apf-req-0003.md`

### 2.4 req-0004 — Serialização/Desserialização JSON (15 PF)

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| EE (External Input) | 0 | 0 | 0 | 0 | 0 |
| SE (External Output) | 0 | 0 | 0 | 0 | 0 |
| CE (External Inquiry) | 5 | 5 | 0 | 0 | 15 |
| ALI (Internal Logical File) | 0 | 0 | 0 | 0 | 0 |
| AIE (External Interface File) | 0 | 0 | 0 | 0 | 0 |
| **Total** | **5** | **5** | **0** | **0** | **15** |

**Referência:** `docs/requirement/apf/apf-req-0004.md`

### 2.5 req-0006 — Publicação Automática no NuGet (7 PF)

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| EE (External Input) | 0 | 0 | 0 | 0 | 0 |
| SE (External Output) | 1 | 1 | 0 | 0 | 4 |
| CE (External Inquiry) | 1 | 1 | 0 | 0 | 3 |
| ALI (Internal Logical File) | 0 | 0 | 0 | 0 | 0 |
| AIE (External Interface File) | 0 | 0 | 0 | 0 | 0 |
| **Total** | **2** | **2** | **0** | **0** | **7** |

**Referência:** `docs/requirement/apf/apf-req-0006.md`
**Nota:** Requisito não funcional (CI/CD). A contagem APF é adaptada para estimativa de esforço do pipeline.

---

## 3. Estimativa de Esforço

Considerando uma produtividade de referência de **10 PF por mês/homem** (média para bibliotecas .NET):

| Item | Valor |
|------|-------|
| Total PF | 69 PF |
| Esforço estimado | ~6,9 meses/homem |
| Requisitos pendentes | Nenhum (todos calculados) |

> **Nota:** Esta é uma estimativa baseada em contagem IFPUG estimada. O valor real pode variar conforme complexidade técnica e contexto do time.

---

## 4. Premissas da Contagem

- A contagem segue o padrão IFPUG (International Function Point Users Group) — Contagem Estimada.
- As funções são identificadas a partir dos requisitos funcionais de cada `req-XXXX`.
- Para o req-0006 (CI/CD), a contagem APF é adaptada — não há funções de dados, e as funções transacionais representam jobs do pipeline.
- A produtividade de 10 PF/mês é uma referência geral; o valor real pode variar conforme complexidade técnica e contexto do time.
- req-0003 e req-0004 tiveram seus APF calculados na v1.1, completando a cobertura de todos os 5 requisitos ativos.

---

## 5. Próximas Atualizações

- [ ] Revisar totais após inclusão de novos requisitos
- [ ] Refinar estimativa de esforço com dados reais de produtividade
- [ ] Promover status para `aprovado` após revisão dos APFs

---

## 6. Clarificações e Notas

| Data | Nota |
|------|------|
| 23/07/2026 | v1.1: Consolidado completo com todos os 5 requisitos ativos. req-0003 (12 PF) e req-0004 (15 PF) adicionados. Total: 69 PF. |
| 23/07/2026 | v1.0: Documento criado como consolidado inicial. req-0003 pendente de cálculo APF. |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
