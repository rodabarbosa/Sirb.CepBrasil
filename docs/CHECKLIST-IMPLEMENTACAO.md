# 📋 Checklist de Implementação - Fluxo de Fallback v1.4.0

## 🎯 Objetivo

Implementar estratégia robusta de busca de CEP com fallback automático entre 4 serviços.

---

## 📊 FASE 1: PREPARAÇÃO

### Leitura e Compreensão

- [ ] Li SUMARIO-EXECUTIVO.md (5 min)
- [ ] Vi DIAGRAMAS-FALLBACK.md e entendi fluxo (10 min)
- [ ] Li FLUXO-FALLBACK.md completamente (20 min)
- [ ] Entendo a ordem: BrasilAPI → ViaCEP → AwesomeAPI → OpenCEP
- [ ] Entendo os 3 resultados: sucesso, não encontrado, erro
- [ ] Entendo que falha completa só se os 4 tiverem erro

### Ambiente de Desenvolvimento

- [ ] .NET SDK 8.0+ instalado
- [ ] .NET SDK 9.0+ instalado
- [ ] .NET SDK 10.0+ instalado
- [ ] IDE configurada (Visual Studio / Rider / VS Code)
- [ ] Git branch criado para desenvolvimento
- [ ] Solução compila sem erros

### Preparação do Projeto

- [ ] Revisei estrutura de pastas existente
- [ ] Revisei arquivo Sirb.CepBrasil.csproj
- [ ] Revisei projeto de testes Sirb.CepBrasil.Test
- [ ] Revisei interfaces existentes (ICepService, ICepServiceControl)
- [ ] Revisei modelos (CepContainer, CepResult)

---

## 📝 FASE 2: IMPLEMENTAÇÃO - INTERFACES E MODELOS

### Validar Interfaces

- [ ] ICepService existe e está correta
    - [ ] Tem método `FindAsync(string cep, CancellationToken)`
    - [ ] Retorna `Task<CepResult>`
    - [ ] XML documentation presente

- [ ] ICepServiceControl existe e está correta
    - [ ] Tem método `FindAsync(string cep, CancellationToken)`
    - [ ] Retorna `Task<CepContainer>`
    - [ ] XML documentation presente

### Validar Modelos

- [ ] CepContainer existe
    - [ ] Tem propriedades: Cep, Logradouro, Bairro, Cidade, Estado
    - [ ] Tem constructor e getters/setters

- [ ] CepResult existe
    - [ ] Tem propriedade Success (bool)
    - [ ] Tem propriedade CepContainer
    - [ ] Tem propriedade Message (string)
    - [ ] Tem constructor(success, container, message)

- [ ] ServiceException existe
    - [ ] Herda de Exception
    - [ ] Pode receber mensagem no constructor

---

## 🔧 FASE 3: IMPLEMENTAÇÃO - SERVIÇOS (BrasilAPI)

### BrasilApiService.cs

#### Criar Arquivo

- [ ] Arquivo criado em: `Sirb.CepBrasil/Services/BrasilApiService.cs`
- [ ] Namespace correto: `Sirb.CepBrasil.Services`
- [ ] Herda de `ICepServiceControl`

#### Implementar Método

- [ ] Método `FindAsync` implementado
    - [ ] Parâmetros: string cep, CancellationToken cancellationToken
    - [ ] Retorna `Task<CepContainer>`
    - [ ] Valida CEP (não vazio, formato correto)
    - [ ] Chama API: `GET https://brasilapi.com.br/api/address/v2/{cep}`
    - [ ] Trata resposta 200 OK
    - [ ] Trata resposta 404 Not Found (retorna null)
    - [ ] Trata erros HTTP (HttpRequestException)
    - [ ] Respeita CancellationToken
    - [ ] Timeout de 30 segundos

#### Documentação

- [ ] Classe tem XML summary
- [ ] Método tem XML documentation completa
    - [ ] `<summary>`
    - [ ] `<param name="cep">`
    - [ ] `<param name="cancellationToken">`
    - [ ] `<returns>`
    - [ ] `<exception>`
    - [ ] `<example>`

#### Mapeamento de Dados

- [ ] Resposta JSON mapeada para CepContainer:
    - [ ] cep → Cep
    - [ ] state → Estado
    - [ ] city → Cidade
    - [ ] neighborhood → Bairro
    - [ ] street → Logradouro

---

## 🔧 FASE 3B: IMPLEMENTAÇÃO - SERVIÇOS (AwesomeAPI)

### AwesomeApiService.cs

#### Criar Arquivo

- [ ] Arquivo criado em: `Sirb.CepBrasil/Services/AwesomeApiService.cs`
- [ ] Namespace correto: `Sirb.CepBrasil.Services`
- [ ] Herda de `ICepServiceControl`

#### Implementar Método

- [ ] Método `FindAsync` implementado
    - [ ] Parâmetros: string cep, CancellationToken cancellationToken
    - [ ] Retorna `Task<CepContainer>`
    - [ ] Chama API: `GET https://awesomeapi.com.br/api/cep/{cep}`
    - [ ] Trata resposta 200 OK
    - [ ] Trata resposta 404 Not Found (retorna null)
    - [ ] Trata erros HTTP
    - [ ] Respeita CancellationToken
    - [ ] Timeout de 30 segundos

#### Documentação

- [ ] Classe tem XML summary
- [ ] Método tem XML documentation completa

#### Mapeamento de Dados

- [ ] Resposta JSON mapeada para CepContainer:
    - [ ] cep → Cep
    - [ ] state → Estado
    - [ ] city → Cidade
    - [ ] district → Bairro
    - [ ] address → Logradouro

---

## 🔧 FASE 3C: IMPLEMENTAÇÃO - SERVIÇOS (OpenCEP)

### OpenCepService.cs

#### Criar Arquivo

- [ ] Arquivo criado em: `Sirb.CepBrasil/Services/OpenCepService.cs`
- [ ] Namespace correto: `Sirb.CepBrasil.Services`
- [ ] Herda de `ICepServiceControl`

#### Implementar Método

- [ ] Método `FindAsync` implementado
    - [ ] Parâmetros: string cep, CancellationToken cancellationToken
    - [ ] Retorna `Task<CepContainer>`
    - [ ] Chama API: `GET https://cep.dev/{cep}`
    - [ ] Trata resposta 200 OK
    - [ ] Trata resposta 404 Not Found (retorna null)
    - [ ] Trata erros HTTP
    - [ ] Respeita CancellationToken
    - [ ] Timeout de 30 segundos

#### Documentação

- [ ] Classe tem XML summary
- [ ] Método tem XML documentation completa

#### Mapeamento de Dados

- [ ] Resposta JSON mapeada para CepContainer:
    - [ ] cep → Cep
    - [ ] state → Estado
    - [ ] city → Cidade
    - [ ] neighborhood → Bairro
    - [ ] street → Logradouro

---

## 🔧 FASE 3D: IMPLEMENTAÇÃO - SERVIÇOS (ViaCEP)

### ViaCepService.cs (Existente)

#### Validar/Refatorar

- [ ] Arquivo existe em: `Sirb.CepBrasil/Services/ViaCepService.cs`
- [ ] Implementa `ICepServiceControl`
- [ ] Método `FindAsync` existe
    - [ ] Parâmetros: string cep, CancellationToken cancellationToken
    - [ ] Retorna `Task<CepContainer>`
    - [ ] Chama API: `GET https://viacep.com.br/ws/{cep}/json`
    - [ ] Trata resposta 200 OK
    - [ ] Trata resposta 404 Not Found (retorna null)
    - [ ] Trata erros HTTP
    - [ ] Respeita CancellationToken
    - [ ] Timeout de 30 segundos

#### Documentação

- [ ] Classe tem XML summary
- [ ] Método tem XML documentation completa

#### Mapeamento de Dados

- [ ] Resposta JSON mapeada para CepContainer:
    - [ ] cep → Cep
    - [ ] uf → Estado
    - [ ] localidade → Cidade
    - [ ] bairro → Bairro
    - [ ] logradouro → Logradouro

---

## ⚙️ FASE 4: IMPLEMENTAÇÃO - ORQUESTRADOR

### CepServiceOrchestrator.cs

#### Criar Arquivo

- [ ] Arquivo criado em: `Sirb.CepBrasil/Services/CepServiceOrchestrator.cs`
- [ ] Namespace correto: `Sirb.CepBrasil.Services`
- [ ] Implementa `ICepService`
- [ ] Sealed class

#### Constructor

- [ ] Aceita `ICepServiceControl[]` (array dos 4 serviços em ordem)
- [ ] Aceita `ILogger<CepServiceOrchestrator>`
- [ ] Valida parâmetros (não nulo)
- [ ] Armazena em campos privados readonly

#### Método FindAsync

- [ ] Parâmetros: string cep, CancellationToken cancellationToken
- [ ] Retorna `Task<CepResult>`

#### Lógica de Validação

- [ ] Valida se CEP é nulo/vazio → `ArgumentNullException`
- [ ] Valida formato do CEP (8 dígitos) → `ArgumentException`
- [ ] Limpa formatação do CEP (remove hífen)

#### Lógica de Fallback

- [ ] Loop através dos 4 serviços em ordem
- [ ] Para cada serviço:
    - [ ] Log da tentativa (informação)
    - [ ] Chama `service.FindAsync(cep, cancellationToken)`
    - [ ] Se sucesso (resultado não null):
        - [ ] Log de sucesso (informação)
        - [ ] Retorna imediatamente `new CepResult(true, result, null)`
    - [ ] Se não encontrado (resultado null):
        - [ ] Log (informação)
        - [ ] Continua para próximo serviço
    - [ ] Se erro (exceção):
        - [ ] Log do erro (warning)
        - [ ] Adiciona exceção à lista
        - [ ] Continua para próximo serviço

#### Comportamento Final

- [ ] Se todos os 4 serviços tiveram erro:
    - [ ] Log de erro (error)
    - [ ] Monta mensagem com detalhes dos erros
    - [ ] Lança `ServiceException` com mensagem
- [ ] Se nenhum encontrou (todos retornaram null ou tiveram erro):
    - [ ] Log (informação)
    - [ ] Retorna `null`

#### Documentação

- [ ] Classe tem XML summary
- [ ] Constructor tem XML documentation
- [ ] Método FindAsync tem XML documentation completa
    - [ ] `<summary>`
    - [ ] `<remarks>` com fluxo detalhado
    - [ ] `<param>`
    - [ ] `<returns>`
    - [ ] `<exception>`
    - [ ] `<example>`

---

## 🧪 FASE 5: TESTES - BrasilApiServiceTest

### Criar Arquivo

- [ ] Arquivo criado em: `Sirb.CepBrasil.Test/Services/BrasilApiServiceTest.cs`

### Teste 1: Sucesso

- [ ] **Nome:** `FindAsync_QuandoCepValido_DeveRetornarSucesso`
- [ ] **DisplayName:** "Deve retornar sucesso quando CEP é válido"
- [ ] Mock HttpClient ou use real (se possível)
- [ ] Arrange: Cria serviço com CEP válido
- [ ] Act: Chama `FindAsync("01310100", ...)`
- [ ] Assert: Resultado não null, Cep correto, tem dados

### Teste 2: Não Encontrado

- [ ] **Nome:** `FindAsync_QuandoCepNaoEncontrado_DeveRetornarNull`
- [ ] **DisplayName:** "Deve retornar null quando CEP não é encontrado"
- [ ] Arrange: CEP válido mas inexistente
- [ ] Act: Chama `FindAsync("00000000", ...)`
- [ ] Assert: Resultado é null

### Teste 3: Erro HTTP

- [ ] **Nome:** `FindAsync_QuandoServicoRetornaErro_DeveLancarExcecao`
- [ ] **DisplayName:** "Deve lançar exceção quando serviço falha"
- [ ] Arrange: Mock retornando erro 503
- [ ] Act: Chama `FindAsync(...)`
- [ ] Assert: Lança `HttpRequestException`

### Teste 4: Timeout

- [ ] **Nome:** `FindAsync_QuandoTempoExpirado_DeveLancarOperationCanceledException`
- [ ] **DisplayName:** "Deve lançar exceção quando timeout ocorre"
- [ ] Arrange: CancellationTokenSource com timeout curto (10ms)
- [ ] Act: Chama `FindAsync(...)`
- [ ] Assert: Lança `OperationCanceledException`

### Teste 5: CEP Inválido

- [ ] **Nome:** `FindAsync_QuandoCepInvalido_DeveLancarArgumentException`
- [ ] **DisplayName:** "Deve lançar exceção quando CEP está em formato inválido"
- [ ] Arrange: CEP inválido ("123" ou "abcd")
- [ ] Act: Chama `FindAsync("123", ...)`
- [ ] Assert: Lança `ArgumentException`

---

## 🧪 FASE 5B: TESTES - AwesomeApiServiceTest

- [ ] Arquivo criado: `Sirb.CepBrasil.Test/Services/AwesomeApiServiceTest.cs`
- [ ] 5 testes (mesmos padrões de BrasilApiServiceTest)
- [ ] Nomenclatura obrigatória
- [ ] DisplayName obrigatório

---

## 🧪 FASE 5C: TESTES - OpenCepServiceTest

- [ ] Arquivo criado: `Sirb.CepBrasil.Test/Services/OpenCepServiceTest.cs`
- [ ] 5 testes (mesmos padrões)
- [ ] Nomenclatura obrigatória
- [ ] DisplayName obrigatório

---

## 🧪 FASE 5D: TESTES - CepServiceOrchestratorTest

### Criar Arquivo

- [ ] Arquivo criado em: `Sirb.CepBrasil.Test/Services/CepServiceOrchestratorTest.cs`

### Teste 1: Usa BrasilAPI Primeiro

- [ ] **Nome:** `FindAsync_DeveUsarBrasilApiPrimeiro`
- [ ] **DisplayName:** "Deve tentar BrasilAPI primeiro"
- [ ] Arrange: Mock dos 4 serviços, BrasilAPI retorna resultado
- [ ] Act: Chama `FindAsync("01310100", ...)`
- [ ] Assert: BrasilAPI foi chamado 1x, outros nunca foram chamados

### Teste 2: Fallback para ViaCEP

- [ ] **Nome:** `FindAsync_DeveUsarViaCepSeBrasilApiFalhar`
- [ ] **DisplayName:** "Deve tentar ViaCEP se BrasilAPI falhar"
- [ ] Arrange: BrasilAPI lança exceção, ViaCEP retorna resultado
- [ ] Act: Chama `FindAsync("01310100", ...)`
- [ ] Assert: Ambos chamados, AwesomeAPI/OpenCEP não chamados

### Teste 3: Sequência Completa

- [ ] **Nome:** `FindAsync_DeveSeqüenciarTentativas`
- [ ] **DisplayName:** "Deve tentar todos os serviços em sequência se falharem"
- [ ] Arrange: Primeiros 3 falham, OpenCEP sucede
- [ ] Act: Chama `FindAsync("01310100", ...)`
- [ ] Assert: Todos foram chamados na ordem certa

### Teste 4: Nenhum Encontra

- [ ] **Nome:** `FindAsync_SeNenhumEncontrar_DeveRetornarNull`
- [ ] **DisplayName:** "Deve retornar null se nenhum serviço encontrar CEP"
- [ ] Arrange: Todos retornam null
- [ ] Act: Chama `FindAsync("00000000", ...)`
- [ ] Assert: Resultado é null

### Teste 5: Todos Falham

- [ ] **Nome:** `FindAsync_SeTodosServicosFalharem_DeveLancarServiceException`
- [ ] **DisplayName:** "Deve lançar exceção se todos os serviços falharem"
- [ ] Arrange: Todos lançam exceção
- [ ] Act: Chama `FindAsync("01310100", ...)`
- [ ] Assert: Lança `ServiceException` com mensagem apropriada

### Teste 6: Retorna na Primeira

- [ ] **Nome:** `FindAsync_DeveRetornarNaPrimeiraOpçãoBemsucedida`
- [ ] **DisplayName:** "Deve retornar resultado da primeira opção bem-sucedida"
- [ ] Arrange: BrasilAPI sucede
- [ ] Act: Chama `FindAsync("01310100", ...)`
- [ ] Assert: Resultado correto, ViaCEP/AwesomeAPI/OpenCEP nunca chamados

---

## ✅ FASE 6: VALIDAÇÃO E QUALIDADE

### Build

- [ ] `dotnet build` executa sem erros
- [ ] `dotnet build` sem warnings
- [ ] Multi-target .NET 8, 9, 10 funciona

### Testes

- [ ] `dotnet test` passa 100%
- [ ] Cobertura de código em 100%
    - [ ] Todos os serviços cobertos
    - [ ] Orquestrador coberto
    - [ ] Todos os caminhos cobertos
    - [ ] Todas as exceções cobertas

### Qualidade de Código

- [ ] Segue SOLID Principles
- [ ] Sem código duplicado
- [ ] Nomes descritivos
- [ ] Sem "magic numbers"
- [ ] Métodos pequenos e focados

### Documentação XML

- [ ] Todas as classes públicas têm documentation
- [ ] Todos os métodos públicos têm documentation
- [ ] Todos os parâmetros documentados
- [ ] Todos os retornos documentados
- [ ] Todas as exceções documentadas
- [ ] Exemplos inclusos quando apropriado

### Testes

- [ ] Nomenclatura: `Metodo_Quando_Deve`
- [ ] DisplayName obrigatório em todos
- [ ] Estrutura AAA (Arrange-Act-Assert)
- [ ] Assertions xUnit nativo
- [ ] Sem FluentAssertions
- [ ] Um Assert por resultado esperado

---

## 📚 FASE 7: DOCUMENTAÇÃO DO PROJETO

### README.md

- [ ] Seção adicionada: "Novo Fluxo de Fallback v1.4.0"
- [ ] Explica os 4 serviços
- [ ] Mostra diagrama do fallback
- [ ] Link para FLUXO-FALLBACK.md

### Arquivos de Documentação

- [ ] FLUXO-FALLBACK.md criado ✅
- [ ] IMPLEMENTACAO-FALLBACK.md criado ✅
- [ ] DIAGRAMAS-FALLBACK.md criado ✅
- [ ] SUMARIO-EXECUTIVO.md criado ✅
- [ ] INDICE-DOCUMENTACAO.md criado ✅

---

## 🔒 FASE 8: SEGURANÇA E TRATAMENTO DE ERROS

### Validação de Entrada

- [ ] CEP nulo → ArgumentNullException
- [ ] CEP vazio → ArgumentNullException
- [ ] CEP inválido → ArgumentException
- [ ] Todos com mensagens claras

### Tratamento de Erros

- [ ] HttpRequestException capturada
- [ ] OperationCanceledException capturada
- [ ] Timeout (30s) por tentativa
- [ ] Log de cada erro (mas não dados sensíveis)

### Resiliência

- [ ] CancellationToken respeitado
- [ ] Sem retry automático (já é fallback)
- [ ] Sem bloqueio de thread
- [ ] Uso correto de async/await

### Segurança

- [ ] Sem hardcoded secrets
- [ ] Sem exposição de informações internas
- [ ] Sanitização de resposta
- [ ] Logging seguro (sem dados sensíveis)

---

## 🚀 FASE 9: INTEGRAÇÃO

### Registrar no DI Container

- [ ] HttpClient para BrasilApiService registrado
- [ ] HttpClient para AwesomeApiService registrado
- [ ] HttpClient para OpenCepService registrado
- [ ] HttpClient para ViaCepService registrado
- [ ] ICepServiceControl[] array registrado
- [ ] CepServiceOrchestrator registrado como ICepService
- [ ] ILogger registrado

### Testar Integração

- [ ] Resolve corretamente do DI
- [ ] Ordem dos serviços está correta
- [ ] Logging funciona
- [ ] Timeout está correto

---

## 📋 FASE 10: ENTREGA

### Code Review

- [ ] Enviado para revisão
- [ ] Todos os comentários respondidos
- [ ] Mudanças solicitadas implementadas

### Pull Request

- [ ] Descrição clara
- [ ] Linkas para FLUXO-FALLBACK.md
- [ ] Linkas para IMPLEMENTACAO-FALLBACK.md
- [ ] Mencionados os testes (100%)
- [ ] Mencionada compatibilidade (.NET 8, 9, 10)

### Merge

- [ ] Aprovado
- [ ] Merged para main/develop
- [ ] Build CI passa
- [ ] Deploy bem-sucedido

---

## ✨ RESUMO FINAL

| Fase                    | Status | Itens                                  |
|-------------------------|--------|----------------------------------------|
| 1. Preparação           | ⏳      | Leitura, ambiente, projeto             |
| 2. Interfaces e Modelos | ⏳      | Validação                              |
| 3. Serviços (4)         | ⏳      | BrasilAPI, AwesomeAPI, OpenCEP, ViaCEP |
| 4. Orquestrador         | ⏳      | CepServiceOrchestrator                 |
| 5. Testes               | ⏳      | 20+ testes com 100% cobertura          |
| 6. Validação            | ⏳      | Build, testes, qualidade               |
| 7. Documentação         | ⏳      | XML, README, arquivos .md              |
| 8. Segurança            | ⏳      | Validação, tratamento, logging         |
| 9. Integração           | ⏳      | DI Container, testes                   |
| 10. Entrega             | ⏳      | Code review, PR, merge                 |

---

## 📞 Precisa de Ajuda?

- **Não entendi o fluxo?** → Leia FLUXO-FALLBACK.md
- **Não sei como codificar?** → Copie de IMPLEMENTACAO-FALLBACK.md
- **Não sei os testes?** → Veja seção "Estratégia de Testes"
- **Preciso visual?** → Veja DIAGRAMAS-FALLBACK.md
- **Preciso de referências?** → Consulte IMPLEMENTACAO-FALLBACK.md

---

**Data Início:** 2026-02-18  
**Versão:** 1.4.0  
**Boa sorte! 🚀**
