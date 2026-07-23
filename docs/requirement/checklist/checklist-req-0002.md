# Quality Checklist — req-0002 (Busca de Endereço com Fallback Multi-Provedor)

## Metadados

| Campo | Valor |
|-------|-------|
| **Código do documento** | checklist-req-0002 |
| **Requisito analisado** | req-0002 / tec-req-0002 — Busca de Endereço com Fallback Multi-Provedor |
| **Data do checklist** | 23/07/2026 |
| **Autor (agente)** | writer |
| **Versão deste documento** | v1.0 |
| **Status do checklist** | Concluído |
| **Veredito final** | APROVADO |

---

## Propósito

Este artefato é o **gate de qualidade final** do req-0002 / tec-req-0002, exercitando as 8 dimensões de qualidade. Funciona como "testes unitários da linguagem" — cada dimensão funciona como um conjunto de asserções que precisam passar para que o requisito seja considerado pronto para aceite.

---

## Dimensões de Qualidade

### 1. Clareza

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 1.1 | Frases curtas, voz ativa, sem inversões desnecessárias | ✅ Pass | Documento usa linguagem direta e objetiva | N/A |
| 1.2 | Ausência de jargão técnico no `req-XXXX` (linguagem de negócio) | ✅ Pass | Termos como "fallback", "provedor", "timeout" são definidos no glossário | N/A |
| 1.3 | Termos definidos no glossário do requisito | ✅ Pass | Glossário em req-0002 seção 1.3 com 10+ termos | N/A |
| 1.4 | Nenhum termo vago sem métrica | ✅ Pass | NFRs têm métricas explícitas (timeout 30s, latência < 120s pior caso) | N/A |
| 1.5 | Título e cabeçalho H1 alinhados com o conteúdo | ✅ Pass | "req-0002 — Busca de Endereço com Fallback Multi-Provedor" reflete o conteúdo | N/A |

**Total da dimensão 1:** Pass 5 · Fail 0 · N/A 0

---

### 2. Completude

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 2.1 | Seção "Intenção / Motivação de Negócio (Por quê)" presente, específica e mensurável | ✅ Pass | req-0002 seção 2 com problema (disponibilidade), impacto (latência), público (aplicações críticas) | N/A |
| 2.2 | Escopo explicitamente delimitado (in scope / out of scope) | ✅ Pass | req-0002 seção 1.2 com lista clara de inclusão/exclusão (inclui fallback, exclui cache) | N/A |
| 2.3 | RFs e RNs numeradas incrementalmente, sem gaps e sem duplicidades | ✅ Pass | RF-08 a RF-16, RN-011 a RN-018 — sequenciais, sem gaps, sem duplicidade | N/A |
| 2.4 | Critérios de aceitação em Gherkin com cenários negativos | ✅ Pass | 9 cenários Gherkin, 4 deles negativos (RF-11, RF-12, RF-13, RF-15) | N/A |
| 2.5 | NFRs mensuráveis com métrica explícita | ✅ Pass | NFRs de desempenho (timeout 30s), disponibilidade, manutenibilidade | N/A |

**Total da dimensão 2:** Pass 5 · Fail 0 · N/A 0

---

### 3. Testabilidade

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 3.1 | Cada cenário Gherkin tem Given/When/Then completos e sem ambiguidade | ✅ Pass | 9 cenários com estrutura completa em req-0002 seção 7 | N/A |
| 3.2 | Cenários negativos (erro, ausência de dados, permissão, limite) estão presentes | ✅ Pass | 4 cenários negativos: todos provedores com erro, fallback parcial, retorno null, timeout | N/A |
| 3.3 | Dados de teste mencionados ou referenciados | ✅ Pass | Dados de teste inline nos cenários Gherkin (CEPs válidos e inválidos) | N/A |
| 3.4 | Cobertura de testes prevista ≥ 80% ou justificativa documentada | ✅ Pass | 100% de cobertura exigido pelo projeto; testes existentes no código | N/A |
| 3.5 | NFRs têm métrica objetiva que pode ser verificada em teste | ✅ Pass | Timeout 30s (verificável via teste de integração), comportamento de fallback (testável com mocks) | N/A |

**Total da dimensão 3:** Pass 5 · Fail 0 · N/A 0

---

### 4. Rastreabilidade

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 4.1 | Links para artefatos funcionam | ✅ Pass | Todos os caminhos relativos corretos; documentos criados (req-0002, tec-req-0002, apf-req-0002, analise-req-0002, api-*, diagramas) | N/A |
| 4.2 | Sem campos "TBD", "a definir" ou placeholders órfãos em seções obrigatórias | ✅ Pass | Apenas marcações `⏳ PENDENTE` justificadas (wireframe e DB para biblioteca sem UI e sem DB) | N/A |
| 4.3 | Identificador `0002` consistente em todos os artefatos vinculados | ✅ Pass | req-0002, tec-req-0002, apf-req-0002, analise-req-0002, checklist-req-0002, api-* | N/A |
| 4.4 | Versão do requisito referenciada quando artefatos correlatos têm versão diferente | ✅ Pass | Versões explicitadas nos cabeçalhos e seção de rastreabilidade | N/A |
| 4.5 | Seção "Artefatos relacionados" lista impactos em ambos os sentidos | ✅ Pass | req-0002 seção 4 lista documentos impactados e impactantes | N/A |

**Total da dimensão 4:** Pass 5 · Fail 0 · N/A 0

---

### 5. Conformidade com a Constituição

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 5.1 | Princípio de Segurança atendido (validação de entrada, HTTPS, tratamento de exceções) | ✅ Pass | CepValidation fail-fast; URLs HTTPS fixas; exceções consolidadas sem expor detalhes técnicos | N/A |
| 5.2 | Princípio de Performance atendido (async, HttpClient, timeout) | ✅ Pass | Async/await em toda a cadeia; HttpClient reutilizado; timeout 30s por tentativa; fallback paralelo com CancellationToken | N/A |
| 5.3 | Princípio de Qualidade de Código atendido (100% cobertura, XML docs em inglês, AAA) | ✅ Pass | Testes seguem padrão AAA; XML docs em inglês nas classes públicas; cobertura 100% do CepService | N/A |
| 5.4 | Princípio de UX atendido (quando há interface) | N/A | Biblioteca sem UI | N/A |
| 5.5 | Princípio de Dados atendido (formato, normalização) | ✅ Pass | CEP normalizado (RemoveMask) antes de enviar aos provedores; sem persistência de dados | N/A |
| 5.6 | Princípio de Arquitetura atendido (separação de responsabilidades) | ✅ Pass | Composição (CepService contém provedores), herança (BaseService), interfaces segregadas (ICepService/ICepServiceControl) | N/A |
| 5.7 | Princípio de Processo atendido (convenções, multi-target, docs) | ✅ Pass | SemVer, histórico, Clarification Log; multi-target .NET 8/9/10; documentação de API por provedor | N/A |

**Total da dimensão 5:** Pass 6 · Fail 0 · N/A 1

---

### 6. Consistência Cross-Artifact

| # | Verificação | Resultado | Evidência/Seção | Ação corretiva |
|---|-------------|-----------|-----------------|----------------|
| 6.1 | Mesmo `0002` e mesma versão v1.0 em `req-0002` e `tec-req-0002` | ✅ Pass | Ambos v1.0 | N/A |
| 6.2 | RFs e RNs idênticas entre os dois documentos | ✅ Pass | RF-08 a RF-16, RN-011 a RN-018 idênticas | N/A |
| 6.3 | Entidades/tabelas/endpoints mencionados existem no did-* e api-* | ✅ Pass | 4 APIs documentadas (api-brasilapi, api-viacep, api-awesomeapi, api-opencep); modelos em memória sem did-* | N/A |
| 6.4 | Pontos de função do apf-req-0002.md refletem o descrito no requisito | ✅ Pass | 4 funções = 12 PF, mapeadas a RF-08, RF-12, RF-13, RF-14 | N/A |
| 6.5 | Recorte da matriz de risco local bate com system-risk-matrix.md | ✅ Pass | RSK-001-004, 009, 013, 014 consistentes (013 e 014 adicionados nesta atualização) | N/A |

**Total da dimensão 6:** Pass 5 · Fail 0 · N/A 0

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
| 8.2 | "Histórico de alterações" atualizado com data, autor, versão e descrição | ✅ Pass | Todos os 11 artefatos têm histórico completo | N/A |
| 8.3 | Clarification Log sem itens `Status=Pendente` que impactem aceite/escopo | ✅ Pass | Todos Resolvidos ou N/A com justificativa | N/A |
| 8.4 | Sem URLs/links quebrados; sem referências a arquivos inexistentes | ✅ Pass | Todos os caminhos relativos referenciam artefatos criados ou existentes | N/A |
| 8.5 | Idioma único (pt-BR) e terminologia consistente | ✅ Pass | pt-BR consistente em todos os documentos; terminologia alinhada (fallback, provedor, CepResult, CepContainer) | N/A |

**Total da dimensão 8:** Pass 5 · Fail 0 · N/A 0

---

## Resultado do Checklist

| Dimensão | Pass | Fail | N/A | Observação |
|----------|------|------|-----|------------|
| 1. Clareza | 5 | 0 | 0 | Linguagem clara; glossário completo |
| 2. Completude | 5 | 0 | 0 | Escopo, RFs/RNs, Gherkin e NFRs completos |
| 3. Testabilidade | 5 | 0 | 0 | 9 cenários Gherkin, cobertura 100% |
| 4. Rastreabilidade | 5 | 0 | 0 | Links e IDs consistentes; 11 artefatos |
| 5. Conformidade com a Constituição | 6 | 0 | 1 | Acessibilidade N/A (sem UI) |
| 6. Consistência Cross-Artifact | 5 | 0 | 0 | APIs documentadas, APF mapeado, riscos consistentes |
| 7. Acessibilidade | 0 | 0 | 5 | N/A completo (sem interface) |
| 8. Manutenibilidade | 5 | 0 | 0 | Versionamento, histórico, Clarification Log |
| **TOTAL** | **36** | **0** | **6** | |

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

Nenhuma ação corretiva necessária — todos os itens Pass ou N/A.

---

## Sign-off

- **Agente que executou o checklist:** writer
- **Data:** 23/07/2026
- **Próximo passo:** Marcar requisito req-0002 como fechado. Atualizar `system-risk-matrix.md` e `docs/README.md`.
- **Vinculado ao sign-off do `apr_req-0002`:** pendente (documento de aprovação formal não faz parte desta task).

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
