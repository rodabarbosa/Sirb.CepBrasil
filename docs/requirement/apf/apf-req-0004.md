# APF — Análise de Pontos de Função — req-0004

## Metadados

| Campo                | Valor                    |
|----------------------|--------------------------|
| **Código**           | apf-req-0004             |
| **Requisito**        | req-0004 — Serialização/Desserialização JSON |
| **Data de criação**  | 23/07/2026               |
| **Última atualização**| 23/07/2026               |
| **Autor**            | Rodrigo Araujo Barbosa   |
| **Versão**           | v1.0                     |
| **Status**           | Rascunho                 |

---

## 1. Premissas

- A contagem segue o padrão IFPUG (International Function Point Users Group) — Contagem Estimada.
- As funções são identificadas a partir dos requisitos funcionais (RF-26 a RF-35).
- Não há arquivos lógicos internos (ALI) ou arquivos de interface externa (AIE), pois o requisito não persiste dados nem consulta arquivos externos (os modelos são DTOs em memória).
- A biblioteca é uma API pública de código (.NET library), portanto as funções transacionais (EE, SE, CE) representam os métodos expostos ao consumidor.
- `BrasilApiResponse` e `AwesomeApiResponse` são classes `internal sealed` (não expostas ao consumidor) — não geram PF separados; são consideradas parte da implementação das funções de desserialização.
- `BooleanOrStringConverter` é um conversor interno para o campo `Erro` do `CepContainer` — não gera PF separado; está embutido em RF-31 (que faz parte do `CepContainer`).
- RF-32 e RF-33 (validação de entrada) são comportamentos já inclusos em RF-26 e RF-27 — sem PF adicional.
- RF-34 (tratamento de erro) está incluso em RF-26 e RF-27 — sem PF adicional.
- RF-35 (reutilização de instância) é otimização interna — sem PF adicional.

---

## 2. Identificação de Funções

### 2.1 Funções de Dados

| Tipo | Função | Descrição | DER | Registros | Complexidade | PF |
|------|--------|-----------|-----|-----------|-------------|-----|
| — | — | Nenhuma — requisito sem persistência de dados | — | — | — | 0 |

### 2.2 Funções Transacionais

| RF | Método/Classe | Descrição | Tipo | DETs | FTRs/ALRs | Complexidade | PF |
|----|--------------|-----------|------|------|-----------|-------------|-----|
| RF-26 | `JsonExtension.ToJson()` | Serializa qualquer objeto para string JSON com configurações padronizadas | CE | 1 (object value) | 0 (não referência arquivos) | Baixa | 3 |
| RF-27 | `JsonExtension.FromJson<T>()` | Desserializa string JSON para tipo genérico T | CE | 1 (string value) + 1 (type parameter T) | 0 (não referência arquivos) | Baixa | 3 |
| RF-29 | `BrasilApiResponse` | Modelo de resposta com mapeamento JsonPropertyName para BrasilAPI | CE | 6 (Cep, Uf, Cidade, Bairro, Logradouro, Service) | 0 | Baixa | 3 |
| RF-30 | `AwesomeApiResponse` | Modelo de resposta com mapeamento JsonPropertyName para AwesomeAPI | CE | 6 (Cep, Uf, Cidade, Bairro, Logradouro, Service) | 0 | Baixa | 3 |
| RF-31 | `BooleanOrStringConverter` + `CepContainer.Erro` | Conversor customizado para campo erro (booleano ou string) | CE | 2 (Erro do CepContainer, BooleanOrStringConverter) | 0 | Baixa | 3 |

**Legenda:**
- **CE** = External Inquiry (Consulta Externa)
- **DET** = Data Element Type (campo de dado)
- **FTR/ALR** = File Type Referenced / Arquivo Lógico Referenciado

### 2.3 Detalhamento de Complexidade

| Função | DETs | FTRs | Complexidade | Justificativa |
|--------|------|------|-------------|---------------|
| ToJson | 1 | 0 | Baixa | 1 DET (object value); sem FTRs (opera em memória) |
| FromJson | 2 | 0 | Baixa | 2 DETs (string value, type parameter T); sem FTRs |
| BrasilApiResponse | 6 | 0 | Baixa | 6 DETs (campos do modelo); sem FTRs (DTO puro) |
| AwesomeApiResponse | 6 | 0 | Baixa | 6 DETs (campos do modelo); sem FTRs (DTO puro) |
| BooleanOrStringConverter | 2 | 0 | Baixa | 2 DETs (Erro, converter); sem FTRs |

---

## 3. Resumo da Contagem

### 3.1 Totais por Tipo

| Tipo | Qtd | Baixa | Média | Alta | Total PF |
|------|-----|-------|-------|------|----------|
| CE (External Inquiry) | 5 | 5 | 0 | 0 | 15 |
| **Total** | **5** | **5** | **0** | **0** | **15** |

### 3.2 Fator de Ajuste

O fator de ajuste não é aplicado nesta contagem (contagem estimada não-ajustada).

### 3.3 Pontos de Função Não-Ajustados (PFNA)

| Item | Valor |
|------|-------|
| Total PFNA | **15 PF** |
| Funções de dados (ALI + AIE) | 0 PF |
| Funções transacionais (EE + SE + CE) | 15 PF |

---

## 4. Comparativo com req-0001, req-0002 e req-0003

| Requisito | PF | Funções | Descrição |
|-----------|----|---------|-----------|
| req-0001  | 23 | 7 | Validação de CEP (métodos utilitários) |
| req-0002  | 12 | 4 | Busca com fallback (orquestração) |
| req-0003  | 12 | 4 | Tratamento de exceções (exceções + extensões) |
| req-0004  | 15 | 5 | Serialização JSON (extensões + modelos) |
| **Total** | **62** | **20** | — |

---

## 5. Consolidação no Tamanho da Aplicação

Este requisito contribui com **15 PF** para o consolidado `docs/tamanho-aplicacao.md`. *(Documento não criado — pendente consolidação futura.)*

---

## 6. Premissas e Observações

1. `JsonExtension.ToJson()` e `FromJson<T>()` são contados como CE separados pois representam consultas públicas distintas com diferentes DETs.
2. `BrasilApiResponse` e `AwesomeApiResponse` são classes `internal` — contadas como CE porque representam consultas de mapeamento de dados entre formatos.
3. `BooleanOrStringConverter` está embutido no `CepContainer.cs` — contado como parte de RF-31.
4. `CepContainer` já existia antes deste requisito (modelo unificado de endereço) — seus campos não são contados novamente. Apenas o `BooleanOrStringConverter` e a propriedade `Erro` são adições.
5. As validações de entrada (RF-32, RF-33) e tratamento de erros (RF-34) são comportamentos internos dos métodos `ToJson()` e `FromJson<T>()` — não geram PF separado.
6. A reutilização da instância de `JsonSerializerOptions` (RF-35) é otimização interna — sem PF adicional.

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do documento APF |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
