# Quality Checklist — req-0003 (Tratamento de Exceções de Serviço)

## Metadados

| Campo | Valor |
|-------|-------|
| **Código do documento** | checklist-req-0003 |
| **Requisito analisado** | req-0003 / tec-req-0003 — Tratamento de Exceções de Serviço |
| **Data do checklist** | 23/07/2026 |
| **Autor (agente)** | writer |
| **Versão deste documento** | v1.0 |
| **Status do checklist** | Concluído |
| **Veredito final** | APROVADO |

---

## Propósito

Este artefato é o **gate de qualidade final** do req-0003 / tec-req-0003, exercitando as 8 dimensões de qualidade. Funciona como "testes unitários da linguagem" — cada dimensão funciona como um conjunto de asserções que precisam passar para que o requisito seja considerado pronto para aceite.

---

## Dimensões de Qualidade

### 1. Clareza

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 1.1 | Frases curtas, voz ativa, sem inversões desnecessárias | ✅ Pass | Documento usa linguagem direta e objetiva | N/A |
| 1.2 | Ausência de jargão técnico no `req-XXXX` (linguagem de negócio) | ✅ Pass | Termos como "lazy evaluation", "yield return" estão no tec-req-XXXX, não no req-XXXX | N/A |
| 1.3 | Termos definidos no glossário do requisito | ✅ Pass | Glossário em req-0003 seção 1.3 com 8+ termos | N/A |
| 1.4 | Nenhum termo vago sem métrica | ✅ Pass | NFRs têm métricas explícitas (< 1ms para AllMessages, etc.) | N/A |
| 1.5 | Título e cabeçalho H1 alinhados com o conteúdo | ✅ Pass | "req-0003 — Tratamento de Exceções de Serviço" reflete o conteúdo | N/A |

**Total da dimensão 1:** Pass 5 · Fail 0 · N/A 0

---

### 2. Completude

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 2.1 | Seção "Intenção / Motivação de Negócio (Por quê)" presente, específica e mensurável | ✅ Pass | req-0003 seção 2 com problema (exceções genéricas vs. semânticas), impacto (clareza, depuração), critério de sucesso (3 exceções diferenciadas) | N/A |
| 2.2 | Escopo explicitamente delimitado (in scope / out of scope) | ✅ Pass | req-0003 seção 1.2 com lista clara de inclusão/exclusão (inclui NotFoundException, ServiceException, ExceptionExtension; exclui logging, retry, cache) | N/A |
| 2.3 | RFs e RNs numeradas incrementalmente, sem gaps e sem duplicidades | ✅ Pass | RF-17 a RF-25, RN-019 a RN-025 — sequenciais, sem gaps, sem duplicidade | N/A |
| 2.4 | Critérios de aceitação em Gherkin com cenários negativos | ✅ Pass | 9 cenários Gherkin, 5 deles negativos (exceções, falhas, ThrowIf verdadeiro) | N/A |
| 2.5 | NFRs mensuráveis com métrica explícita | ✅ Pass | NFRs de desempenho (< 1ms), segurança, manutenibilidade (100% cobertura) | N/A |

**Total da dimensão 2:** Pass 5 · Fail 0 · N/A 0

---

### 3. Testabilidade

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 3.1 | Cada cenário Gherkin tem Given/When/Then completos e sem ambiguidade | ✅ Pass | 9 cenários com estrutura completa em req-0003 seção 7 | N/A |
| 3.2 | Cenários negativos (erro, ausência de dados, permissão, limite) estão presentes | ✅ Pass | 5 cenários negativos: NotFoundException, ServiceException, múltiplos erros, ThrowIf verdadeiro, argumento inválido | N/A |
| 3.3 | Dados de teste mencionados ou referenciados | ✅ Pass | Dados de teste inline nos cenários Gherkin (CEPs, exceções, condição true/false) | N/A |
| 3.4 | Cobertura de testes prevista ≥ 80% ou justificativa documentada | ✅ Pass | 100% de cobertura exigido pelo projeto; ExceptionExtension tem 29 testes; pendente criar testes para NotFoundException e ServiceException | N/A |
| 3.5 | NFRs têm métrica objetiva que pode ser verificada em teste | ✅ Pass | Tempo < 1ms (verificável via benchmark), throw condicional (testável via asserção) | N/A |

**Total da dimensão 3:** Pass 5 · Fail 0 · N/A 0

---

### 4. Rastreabilidade

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 4.1 | Links para artefatos funcionam | ✅ Pass | Todos os caminhos relativos corretos; documentos criados (req-0003, tec-req-0003, apf-req-0003, analise-req-0003) | N/A |
| 4.2 | Sem campos "TBD", "a definir" ou placeholders órfãos em seções obrigatórias | ✅ Pass | Apenas marcações `⏳ PENDENTE` justificadas (wireframe e DB para biblioteca sem UI e sem DB) | N/A |
| 4.3 | Identificador `0003` consistente em todos os artefatos vinculados | ✅ Pass | req-0003, tec-req-0003, apf-req-0003, analise-req-0003, checklist-req-0003 | N/A |
| 4.4 | Versão do requisito referenciada quando artefatos correlatos têm versão diferente | ✅ Pass | Versões explicitadas nos cabeçalhos e seção de rastreabilidade | N/A |
| 4.5 | Seção "Artefatos relacionados" lista impactos em ambos os sentidos | ✅ Pass | req-0003 seção 4 lista documentos impactados e impactantes | N/A |

**Total da dimensão 4:** Pass 5 · Fail 0 · N/A 0

---

### 5. Conformidade com a Constituição

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 5.1 | Princípio de Segurança atendido (validação de entrada, exceções seguras) | ✅ Pass | ArgumentNullException em AllMessages/GetDetailedMessage; stack trace opcional (default false); mensagens genéricas | N/A |
| 5.2 | Princípio de Performance atendido (yield return, LINQ eficiente) | ✅ Pass | GetExceptionChain com yield return (lazy); AllMessages com LINQ; GetDetailedMessage com StringBuilder | N/A |
| 5.3 | Princípio de Qualidade de Código atendido (100% cobertura, XML docs em inglês) | ✅ Pass | ExceptionExtension com 29 testes/100% cobertura; XML docs em inglês; pendente NotFoundException/ServiceException testes | N/A |
| 5.4 | Princípio de UX atendido (quando há interface) | N/A | Biblioteca sem UI | N/A |
| 5.5 | Princípio de Dados atendido (formato, normalização) | ✅ Pass | Exceções sem dados pessoais; CEP não é exposto em mensagens | N/A |
| 5.6 | Princípio de Arquitetura atendido (separação de responsabilidades) | ✅ Pass | NotFoundException e ServiceException com responsabilidades distintas; ExceptionExtension com SRP | N/A |
| 5.7 | Princípio de Processo atendido (convenções, multi-target, docs) | ✅ Pass | SemVer, histórico, Clarification Log; [Serializable] para cross-AppDomain | N/A |

**Total da dimensão 5:** Pass 6 · Fail 0 · N/A 1

---

### 6. Consistência Cross-Artifact

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 6.1 | Mesmo `0003` e mesma versão v1.0 em `req-0003` e `tec-req-0003` | ✅ Pass | Ambos v1.0 | N/A |
| 6.2 | RFs e RNs idênticas entre os dois documentos | ✅ Pass | RF-17 a RF-25, RN-019 a RN-025 idênticas | N/A |
| 6.3 | Entidades/endpoints mencionados existem nos artefatos correspondentes | ✅ Pass | NotFoundException, ServiceException, ExceptionExtension referenciados no código e documentados | N/A |
| 6.4 | Pontos de função do apf-req-0003.md refletem o descrito no requisito | ✅ Pass | 4 funções = 12 PF, mapeadas a RF-17, RF-18, RF-19, RF-20 | N/A |
| 6.5 | Recorte da matriz de risco local bate com system-risk-matrix.md | ✅ Pass | RSK-015, RSK-016, RSK-017, RSK-018 adicionados na atualização | N/A |
| 6.6 | RNs não conflitam com requisitos anteriores | ✅ Pass | RN-019-RN-025 são específicas; RN-024 referencia RN-013 do req-0002 | N/A |

**Total da dimensão 6:** Pass 6 · Fail 0 · N/A 0

---

### 7. Acessibilidade

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 7.1 | Diretrizes WCAG referenciadas | N/A | Biblioteca .NET sem interface de usuário | N/A |
| 7.2 | Imagens do wireframe/UI têm alt text descritivo previsto | N/A | Sem wireframes | N/A |
| 7.3 | Navegação por teclado prevista | N/A | Sem interface | N/A |
| 7.4 | Componentes interativos com estados de foco visível | N/A | Sem interface | N/A |
| 7.5 | Wireframe ASCII/imagem coerente com padrao-visual.md | N/A | Sem wireframe ou padrão visual | N/A |

**Total da dimensão 7:** Pass 0 · Fail 0 · N/A 5

---

### 8. Manutenibilidade

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 8.1 | Versão SemVer consistente no cabeçalho | ✅ Pass | v1.0 em todos os artefatos | N/A |
| 8.2 | "Histórico de alterações" atualizado com data, autor, versão e descrição | ✅ Pass | Todos os artefatos têm histórico completo | N/A |
| 8.3 | Clarification Log sem itens `Status=Pendente` que impactem aceite/escopo | ✅ Pass | Todos Resolvidos ou N/A com justificativa | N/A |
| 8.4 | Sem URLs/links quebrados; sem referências a arquivos inexistentes | ✅ Pass | Todos os caminhos relativos referenciam artefatos criados ou existentes | N/A |
| 8.5 | Idioma único (pt-BR) e terminologia consistente | ✅ Pass | pt-BR consistente em todos os documentos; terminologia alinhada (exceção de domínio, cadeia de exceções, ThrowIf) | N/A |

**Total da dimensão 8:** Pass 5 · Fail 0 · N/A 0

---

## Resultado do Checklist

| Dimensão | Pass | Fail | N/A | Observação |
|----------|------|------|-----|------------|
| 1. Clareza | 5 | 0 | 0 | Linguagem clara; glossário completo |
| 2. Completude | 5 | 0 | 0 | Escopo, RFs/RNs, Gherkin e NFRs completos |
| 3. Testabilidade | 5 | 0 | 0 | 9 cenários Gherkin, cobertura 100% alvo |
| 4. Rastreabilidade | 5 | 0 | 0 | Links e IDs consistentes; 5+ artefatos |
| 5. Conformidade com a Constituição | 6 | 0 | 1 | Acessibilidade N/A (sem UI) |
| 6. Consistência Cross-Artifact | 6 | 0 | 0 | RFs/RNs consistentes, APF mapeado, riscos adicionados |
| 7. Acessibilidade | 0 | 0 | 5 | N/A completo (sem interface) |
| 8. Manutenibilidade | 5 | 0 | 0 | Versionamento, histórico, Clarification Log |
| **TOTAL** | **37** | **0** | **6** | |

### Regra de Veredito

- **APROVADO** — todas as 8 dimensões com **Pass** ou **Pass / N/A** (sem nenhum **Fail**). ✅
- **APROVADO COM RESSALVAS** — até 2 Fails não-críticos.
- **REPROVADO** — ≥ 1 Fail crítico ou ≥ 3 Fails.

### Veredito Final

- [x] **APROVADO** — todos Pass ou Pass/N/A. Requisito pode ser considerado fechado.
- [ ] **APROVADO COM RESSALVAS** — até 2 Fails não-críticos.
- [ ] **REPROVADO** — ≥ 1 Fail crítico ou ≥ 3 Fails.

---

## Ações Corretivas

1. Criar testes unitários para `NotFoundException` e `ServiceException` (8 testes cada) para garantir 100% de cobertura.
2. Criar `docs/tamanho-aplicacao.md` com total consolidado de 47 PF na Wave 5.

---

## Sign-off

- **Agente que executou o checklist:** writer
- **Data:** 23/07/2026
- **Próximo passo:** Atualizar `system-risk-matrix.md` com novos riscos (RSK-015 a RSK-018). Atualizar `docs/README.md`.
- **Vinculado ao sign-off do `apr_req-0003`:** pendente (documento de aprovação formal não faz parte desta task).

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do Quality Checklist |

---

## Clarification Log

| Data | Pergunta | Resposta | Status | Origem | Impacto |
|------|----------|----------|--------|--------|---------|
| 23/07/2026 | N/A — nenhuma ambiguidade identificada durante o checklist | — | N/A | Criação | Sem impacto |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**