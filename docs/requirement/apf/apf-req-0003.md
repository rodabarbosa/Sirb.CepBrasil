# APF — Análise de Pontos de Função — req-0003

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | apf-req-0003             |
| **Requisito**        | req-0003 — Tratamento de Exceções de Serviço |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |

---

## 1. Premissas

- A contagem segue o padrão IFPUG (International Function Point Users Group) — Contagem Estimada.
- As funções são identificadas a partir dos requisitos funcionais (RF-17 a RF-25).
- Não há arquivos lógicos internos (ALI) ou arquivos de interface externa (AIE), pois o requisito não persiste dados nem consulta arquivos externos (as exceções são acionadas durante integrações, mas a lógica de tratamento é local).
- A biblioteca é uma API pública de código (.NET library), portanto as funções transacionais (EE, SE, CE) representam os métodos expostos ao consumidor.
- RF-22 e RF-23 são comportamentos internos de integração que não geram funções transacionais separadas — estão agrupadas em RF-17/RF-18 (definição das exceções) e RF-19/RF-20/RF-21 (métodos de extensão).
- RF-24 (ThrowIf) e RF-25 (Exceções no CepResult) são contados como parte das funções de definição das exceções e do CepResult, sem PF adicional.

---

## 2. Identificação de Funções

### 2.1 Funções de Dados

| Tipo | Função | Descrição | DER | Registros | Complexidade | PF |
|------|--------|-----------|-----|-----------|-------------|-----|
| — | — | Nenhuma — requisito sem persistência de dados | — | — | — | 0 |

### 2.2 Funções Transacionais

| RF | Método/Classe | Descrição | Tipo | DETs | FTRs/ALRs | Complexidade | PF |
|----|--------------|-----------|------|------|-----------|-------------|-----|
| RF-17 | `NotFoundException` | Definição da exceção de domínio para CEP não encontrado (6 overloads + ThrowIf) | CE | 3 (message, innerException, condition) | 1 (Exception base) | Baixa | 3 |
| RF-18 | `ServiceException` | Definição da exceção de domínio para erro de serviço externo (5 overloads + ThrowIf) | CE | 3 (message, innerException, condition) | 1 (Exception base) | Baixa | 3 |
| RF-19 | `ExceptionExtension.AllMessages()` | Método de extensão que concatena mensagens da cadeia de exceções | CE | 1 (exception) | 1 (Exception) | Baixa | 3 |
| RF-20 | `ExceptionExtension.GetDetailedMessage()` | Método de extensão com formatação detalhada e stack trace opcional | CE | 2 (exception, includeStackTrace) | 1 (Exception) | Baixa | 3 |
| RF-21 | `ExceptionExtension.GetExceptionChain()` | Método privado de iteração pela cadeia (não contado separadamente — parte de RF-19/RF-20) | — | — | — | — | — |
| RF-24 | `ThrowIf(bool, string, Exception)` | Método estático condicional (não contado separadamente — incluso em RF-17 e RF-18) | — | — | — | — | — |
| RF-25 | `CepResult.Exceptions` | Propriedade de registro de exceções (não contado — já existe em req-0002) | — | — | — | — | — |

**Legenda:**
- **CE** = External Inquiry (Consulta Externa)
- **DET** = Data Element Type (campo de dado)
- **FTR/ALR** = File Type Referenced / Arquivo Lógico Referenciado

### 2.3 Detalhamento de Complexidade

| Função | DETs | FTRs | Complexidade | Justificativa |
|--------|------|------|-------------|---------------|
| NotFoundException | 3 | 1 | Baixa | 3 DETs (string message, Exception innerException, bool condition do ThrowIf); 1 FTR (Exception base) |
| ServiceException | 3 | 1 | Baixa | Idem NotFoundException |
| AllMessages | 1 | 1 | Baixa | 1 DET (Exception input); 1 FTR (Exception) |
| GetDetailedMessage | 2 | 1 | Baixa | 2 DETs (Exception exception, bool includeStackTrace); 1 FTR (Exception) |

---

## 3. Resumo da Contagem

### 3.1 Totais por Tipo

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| CE (External Inquiry) | 4 | 4 | 0 | 0 | 12 |
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

## 4. Comparativo com req-0001 e req-0002

| Requisito | PF | Funções | Descrição |
|-----------|----|---------|-----------|
| req-0001  | 23 | 7 | Validação de CEP (métodos utilitários) |
| req-0002  | 12 | 4 | Busca com fallback (orquestração) |
| req-0003  | 12 | 4 | Tratamento de exceções (exceções + extensões) |
| **Total** | **47** | **15** | — |

---

## 5. Consolidação no Tamanho da Aplicação

Este requisito contribui com **12 PF** para o consolidado `docs/tamanho-aplicacao.md`. *(Documento não criado — pendente consolidação futura.)*

---

## 6. Premissas e Observações

1. As classes `NotFoundException` e `ServiceException` são contadas como CE por representarem consultas à hierarquia de exceções (criação + validação condicional).
2. `ExceptionExtension.AllMessages()` e `GetDetailedMessage()` são contados como CE separados pois representam consultas públicas distintas com diferentes DETs.
3. `GetExceptionChain()` é privado e não gera PF separado — está embutido em `AllMessages()` e `GetDetailedMessage()`.
4. `ThrowIf` é método integrado às classes de exceção e não gera PF separado.
5. A propriedade `Exceptions` do `CepResult` já foi contada no req-0002 (faz parte do modelo `CepResult`).
6. As integrações com `BaseService` (RF-22) e `CepService` (RF-23) são comportamentos internos que usam as funções já contadas, sem gerar PF adicional.

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do documento APF |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**