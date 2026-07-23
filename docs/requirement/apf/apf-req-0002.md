# APF — Análise de Pontos de Função — req-0002

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | apf-req-0002             |
| **Requisito**        | req-0002 — Busca de Endereço com Fallback Multi-Provedor |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |

---

## 1. Premissas

- A contagem segue o padrão IFPUG (International Function Point Users Group) — Contagem Estimada.
- As funções são identificadas a partir dos requisitos funcionais (RF-08 a RF-16).
- Não há arquivos lógicos internos (ALI) ou arquivos de interface externa (AIE), pois o requisito não persiste dados nem consulta arquivos externos (as APIs externas são acessadas via HTTP, não como AIE).
- A biblioteca é uma API pública de código (.NET library), portanto as funções transacionais (EE, SE, CE) representam os métodos expostos ao consumidor.
- RF-09, RF-10, RF-11, RF-15 e RF-16 são comportamentos internos do método `FindAsync` e não geram funções transacionais separadas — estão agrupadas em RF-08.

---

## 2. Identificação de Funções

### 2.1 Funções de Dados

| Tipo | Função | Descrição | DER | Registros | Complexidade | PF |
|------|--------|-----------|-----|-----------|-------------|-----|
| — | — | Nenhuma — requisito sem persistência de dados | — | — | — | 0 |

### 2.2 Funções Transacionais

| RF | Método | Descrição | Tipo | DETs | FTRs/ALRs | Complexidade | PF |
|----|--------|-----------|------|------|-----------|-------------|-----|
| RF-08 | `CepService.FindAsync(string, CancellationToken)` | Orquestra fallback entre 4 provedores retornando CepResult | CE | 2 (CEP input, CepResult output) | 1 (CepValidation) | Baixa | 3 |
| RF-09 | *(parte de RF-08 — retorno no primeiro sucesso)* | — | — | — | — | — | — |
| RF-10 | *(parte de RF-08 — consolidação de erros)* | — | — | — | — | — | — |
| RF-11 | *(parte de RF-08 — null quando não encontrado)* | — | — | — | — | — | — |
| RF-12 | `CepService.CepService()` | Construtor sem parâmetros — gerencia HttpClient internamente | EE | 0 | 1 (HttpClient) | Baixa | 3 |
| RF-13 | `CepService.CepService(HttpClient)` | Construtor com injeção de HttpClient externo (DI) | EE | 1 (HttpClient) | 1 (HttpClient) | Baixa | 3 |
| RF-14 | `CepService.Dispose()` | Libera recursos (HttpClient criado internamente) | CE | 0 | 1 (HttpClient) | Baixa | 3 |
| RF-15 | *(parte de RF-08 — CancellationToken / timeout)* | — | — | — | — | — | — |
| RF-16 | *(parte de RF-08 — validação de entrada)* | — | — | — | — | — | — |

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
| EE (External Input) | 2 | 2 | 0 | 0 | 6 |
| SE (External Output) | 0 | 0 | 0 | 0 | 0 |
| CE (External Inquiry) | 2 | 2 | 0 | 0 | 6 |
| **Total** | **4** | **4** | **0** | **0** | **12** |

### 3.2 Fator de Ajuste

O fator de ajuste não é aplicado nesta contagem (contagem estimada não-ajustada).

### 3.3 Pontos de Função Não-Ajustados (PFNA)

| Item | Valor |
|------|-------|
| Total PFNA | **12 PF** |
| Funções de dados (ALI + AIE) | 0 PF |
| Funções transacionais (EE + SE + CE) | 12 PF |

---

## 4. Comparativo com req-0001

| Requisito | PF | Funções | Descrição |
|-----------|----|---------|-----------|
| req-0001  | 23 | 7 | Validação de CEP (métodos utilitários) |
| req-0002  | 12 | 4 | Busca com fallback (orquestração) |
| **Total** | **35** | **11** | — |

---

## 5. Consolidação no Tamanho da Aplicação

Este requisito contribui com **12 PF** para o consolidado `docs/tamanho-aplicacao.md`. *(Documento não criado — pendente consolidação futura.)*

---

## 6. Premissas e Observações

1. O método `FindAsync` é contado como uma única função CE, pois encapsula toda a lógica de fallback (RF-08 a RF-11, RF-15, RF-16) em uma única operação pública.
2. Os construtores `CepService()` e `CepService(HttpClient)` são contados separadamente como EE por representarem modos distintos de inicialização com diferentes DETs.
3. O método `Dispose()` é contado como CE de baixa complexidade por representar uma operação de cleanup com um único FTR (HttpClient).
4. Os 4 serviços provedores (BrasilApiService, ViaCepService, AwesomeApiService, OpenCepService) são internos e não expostos ao consumidor — portanto não geram funções transacionais adicionais.
5. `BaseService`, `CepValidation` e `CepContainer` são classes internas de infraestrutura e não são contadas separadamente.

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do documento APF |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
