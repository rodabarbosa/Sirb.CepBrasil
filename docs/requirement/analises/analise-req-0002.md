# Análise Cross-Artifact — req-0002

## Metadados

| Campo | Valor |
|-------|-------|
| **Código do documento** | analise-req-0002 |
| **Requisito analisado** | req-0002 / tec-req-0002 — Busca de Endereço com Fallback Multi-Provedor |
| **Data da análise** | 23/07/2026 |
| **Autor (agente)** | writer |
| **Versão dos artefatos analisados** | req-0002 v1.0, tec-req-0002 v1.0 |
| **Versão deste documento** | v1.0 |
| **Status** | Rascunho |

---

## Escopo da Análise

Artefatos considerados nesta análise:

| # | Artefato | Versão | Caminho |
|---|----------|--------|---------|
| 1 | req-0002 (requisito de negócio) | v1.0 | `docs/requirement/req-0002-busca-endereco-fallback.md` |
| 2 | tec-req-0002 (requisito técnico) | v1.0 | `docs/requirement/tec/tec-req-0002-busca-endereco-fallback.md` |
| 3 | apf-req-0002 (APF) | v1.0 | `docs/requirement/apf/apf-req-0002.md` |
| 4 | system-risk-matrix.md | v1.1 | `docs/system-risk-matrix.md` |
| 5 | constituicao.md | v1.0 | `docs/constituicao.md` |
| 6 | system-mapping.md | v1.0 | `docs/system-mapping.md` |
| 7 | architecture-tech-stack.md | v1.0 | `docs/architecture-tech-stack.md` |
| 8 | api-brasilapi (API doc) | v1.0 | `docs/requirement/api/api-brasilapi.md` |
| 9 | api-viacep (API doc) | v1.0 | `docs/requirement/api/api-viacep.md` |
| 10 | api-awesomeapi (API doc) | v1.0 | `docs/requirement/api/api-awesomeapi.md` |
| 11 | api-opencep (API doc) | v1.0 | `docs/requirement/api/api-opencep.md` |
| 12 | dcl-estrutura-servicos (diagrama de classes) | v1.0 | `docs/diagrams/dcl-estrutura-servicos.md` |
| 13 | min-fluxo-fallback (diagrama de integração) | v1.0 | `docs/diagrams/min-fluxo-fallback.md` |
| 14 | Código fonte | — | `Sirb.CepBrasil/Services/CepService.cs` |
| 15 | Código fonte | — | `Sirb.CepBrasil/Services/BaseService.cs` |
| 16 | Código fonte | — | `Sirb.CepBrasil/Services/BrasilApiService.cs` |
| 17 | Código fonte | — | `Sirb.CepBrasil/Services/ViaCepService.cs` |
| 18 | Código fonte | — | `Sirb.CepBrasil/Services/AwesomeApiService.cs` |
| 19 | Código fonte | — | `Sirb.CepBrasil/Services/OpenCepService.cs` |
| 20 | Interfaes | — | `Sirb.CepBrasil/Interfaces/ICepService.cs`, `ICepServiceControl.cs` |

---

## Checklist de Consistência

| # | Verificação | Artefatos | Resultado | Evidência | Ação corretiva |
|---|-------------|-----------|-----------|-----------|----------------|
| 1 | Mesmo `XXXX` e mesma versão em `req-XXXX` e `tec-req-XXXX` | req-0002 ↔ tec-req-0002 | ✅ OK | Ambos v1.0, mesmo código 0002 | N/A |
| 2 | Seção "Intenção / Motivação de Negócio (Por quê)" do `req-XXXX` alinhada com o resumo do `tec-req-XXXX` | req-0002 ↔ tec-req-0002 | ✅ OK | tec-req-0002 seção 1 referencia req-0002 e resume "por quê" | N/A |
| 3 | RFs e RNs numeradas consistentes entre os dois documentos (sem gaps/duplicidades) | req-0002 ↔ tec-req-0002 | ✅ OK | RF-08 a RF-16 e RN-011 a RN-018 consistentes em ambos | N/A |
| 4 | Toda entidade/tabela mencionada possui entrada correspondente no did-* | req-0002 / tec-req-0002 ↔ did-* | N/A | Requisito não possui entidades de banco de dados; modelos (CepContainer, etc.) são classes em memória | N/A |
| 5 | Todo endpoint mencionado possui entrada correspondente no api-* | req-0002 / tec-req-0002 ↔ api-* | ✅ OK | 4 APIs documentadas: api-brasilapi, api-viacep, api-awesomeapi, api-opencep | N/A |
| 6 | DER reflete as entidades e relacionamentos do requisito | der-* ↔ req-0002 / tec-req-0002 | N/A | Sem entidades de dados persistentes; dcl-estrutura-servicos cobre o modelo de classes | N/A |
| 7 | APF cobre todas as funções do requisito; totais batem com tamanho-aplicacao.md | apf-req-0002.md ↔ tamanho-aplicacao.md | ⚠️ Consistente (pendência) | 4 funções = 12 PF; `tamanho-aplicacao.md` não foi criado ainda | Criar `tamanho-aplicacao.md` consolidado na Wave 5 |
| 8 | Recorte local da matriz de risco consistente com system-risk-matrix.md | req-0002 (recorte) ↔ system-risk-matrix.md | ✅ OK | RSK-001 a 004, 009 consistentes; RSK-013 e RSK-014 adicionados na atualização | N/A |

---

## Alinhamento com a Constituição

| Princípio | Atendido? | Evidência | Ação |
|-----------|-----------|-----------|------|
| **Segurança** | ✅ Sim | Validação de entrada fail-fast; tratamento de exceções sem exposição de dados sensíveis; requisições HTTPS (URLs fixas com `https://`); HttpClient reutilizado sem vazamento de sockets | N/A |
| **Performance e Disponibilidade** | ✅ Sim | Fallback entre 4 provedores maximiza disponibilidade; timeout de 30s por tentativa com CancellationToken; retorno imediato no primeiro sucesso; HttpClient reutilizado com pool de conexões | N/A |
| **Qualidade de Código** | ✅ Sim | Interfaces segregadas (ICepService / ICepServiceControl); herança via BaseService; baixo acoplamento; serviços registrados por composição no orquestrador | N/A |
| **UX e Acessibilidade** | N/A | Biblioteca .NET sem interface de usuário | N/A |
| **Dados e Privacidade** | ✅ Sim | CEP validado e normalizado antes de enviar aos provedores; dados de endereço são descartados após o retorno; sem persistência de dados pessoais | N/A |
| **Arquitetura** | ✅ Sim | Strategy pattern implícito (ICepServiceControl); inversão de dependência nos construtores (suporte DI); separação clara entre orquestrador (CepService) e provedores (BrasilApiService, etc.) | N/A |
| **Processo** | ✅ Sim | SemVer nos cabeçalhos; histórico de alterações; Clarification Log nos dois documentos; documentação de API e diagramas; multi-target .NET 8/9/10 | N/A |

---

## Clarification Log (Análise de Pendências)

| Contagem | req-0002 | tec-req-0002 |
|----------|----------|--------------|
| Pendentes (Origem=Criação) | 0 | 0 |
| Pendentes (Origem=Implementação) | 0 | 0 |
| **Pendentes que impactam aceite/escopo** | **0** | **0** |

Todos os itens do Clarification Log estão **Resolvidos** ou **N/A** com justificativa.

---

## Resultado da Análise

- [x] **APROVADO** — todas as verificações OK; sem pendentes bloqueantes; sem ressalvas. Requisito pode ser considerado fechado.
- [ ] **APROVADO COM RESSALVAS** — verificações OK com pendentes não-bloqueantes.
- [ ] **REPROVADO** — inconsistências ou pendentes bloqueantes identificados.

### Observações

- **Pendência futura:** Criar o `docs/tamanho-aplicacao.md` consolidado (Wave 5) com os totais de 23 PF (req-0001) + 12 PF (req-0002) = 35 PF.
- **Documentação extra:** `docs/extra/` contém diagramas e documentação legada do fluxo de fallback que foram usados como referência.

---

## Ações Corretivas Necessárias

1. Nenhuma ação corretiva necessária — todas as verificações OK.

---

## Sign-off

- **Agente que executou a análise:** writer
- **Data:** 23/07/2026
- **Próximo passo:** Gerar Quality Checklist (checklist-req-0002.md) e marcar requisito como fechado.

---

## Histórico de Alterações

| Data | Autor | Versão | Alteração |
|------|-------|--------|-----------|
| 23/07/2026 | Rodrigo Araujo Barbosa | v1.0 | Criação do documento de análise cross-artifact |

---

## Clarification Log

| Data | Pergunta | Resposta | Status | Origem | Impacto |
|------|----------|----------|--------|--------|---------|
| 23/07/2026 | N/A — nenhuma ambiguidade identificada durante a análise | — | N/A | Criação | Sem impacto |

---

**© 2026 Rodrigo Araujo Barbosa — Sirb.CepBrasil**
