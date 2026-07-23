# APF — Análise de Pontos de Função — req-0001

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | apf-req-0001             |
| **Requisito**        | req-0001 — Validação de CEP |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |

---

## 1. Premissas

- A contagem segue o padrão IFPUG (International Function Point Users Group) — Contagem Estimada.
- As funções são identificadas a partir dos requisitos funcionais (RF-01 a RF-07).
- Não há arquivos lógicos internos (ALI) ou arquivos de interface externa (AIE), pois o requisito não persiste dados nem consulta arquivos externos.
- A biblioteca é uma API pública de código (.NET library), portanto as funções transacionais (EE, SE, CE) representam os métodos expostos ao consumidor.

---

## 2. Identificação de Funções

### 2.1 Funções de Dados

| Tipo | Função | Descrição | DER | Registros | Complexidade | PF |
|------|--------|-----------|-----|-----------|-------------|-----|
| — | — | Nenhuma — requisito sem persistência de dados | — | — | — | 0 |

### 2.2 Funções Transacionais

| RF | Método | Descrição | Tipo | DETs | FTRs/ALRs | Complexidade | PF |
|----|--------|-----------|------|------|-----------|-------------|-----|
| RF-01 | `CepValidation.Validate(string)` | Valida formato e comprimento do CEP, lançando exceções para entradas inválidas | EE | 1 (CEP) | 1 (regras de validação) | Baixa | 3 |
| RF-02 | `CepExtension.RemoveMask(string)` | Remove caracteres não numéricos do CEP | CE | 1 (CEP) | 0 | Baixa | 3 |
| RF-03 | `CepExtension.CepMask(string)` | Aplica máscara 00000-000 ao CEP | SE | 1 (CEP) | 1 (formatação) | Baixa | 4 |
| RF-04 | `CepExtension.IsValidCep(string)` | Verifica se o CEP tem formato válido (retorno booleano) | CE | 1 (CEP) | 1 (regex + comprimento) | Baixa | 3 |
| RF-05 | `CepExtension.GetDigitsOnly(string)` | Retorna apenas os dígitos do CEP | CE | 1 (CEP) | 0 | Baixa | 3 |
| RF-06 | `CepExtension.Format(string)` | Formata CEP com segurança (retorna vazio se inválido) | SE | 1 (CEP) | 1 (validação + formatação) | Baixa | 4 |
| RF-07 | `CepExtension.Normalize(string)` | Valida e retorna apenas dígitos (ou null se inválido) | CE | 1 (CEP) | 1 (validação) | Baixa | 3 |

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
| EE (External Input) | 1 | 1 | 0 | 0 | 3 |
| SE (External Output) | 2 | 2 | 0 | 0 | 8 |
| CE (External Inquiry) | 4 | 4 | 0 | 0 | 12 |
| **Total** | **7** | **7** | **0** | **0** | **23** |

### 3.2 Fator de Ajuste

O fator de ajuste não é aplicado nesta contagem (contagem estimada não-ajustada).

### 3.3 Pontos de Função Não-Ajustados (PFNA)

| Item | Valor |
|------|-------|
| Total PFNA | **23 PF** |
| Funções de dados (ALI + AIE) | 0 PF |
| Funções transacionais (EE + SE + CE) | 23 PF |

---

## 4. Consolidação no Tamanho da Aplicação

Este requisito contribui com **23 PF** para o consolidado `docs/tamanho-aplicacao.md`.

---

## 5. Premissas e Observações

1. Cada método público é contado como uma função transacional independente, pois representa uma operação distinta que o usuário (desenvolvedor) pode invocar.
2. A complexidade é considerada **Baixa** para todos os métodos pois cada um opera sobre um único DET (o CEP como string) e referencia no máximo 1 FTR (lógica de validação).
3. Se a biblioteca for consumida como API via métodos públicos, cada chamada a um método constitui uma transação.

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do documento APF |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
