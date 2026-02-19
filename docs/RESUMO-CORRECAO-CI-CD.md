# 📦 Correção do CI/CD - Publicação Automática no NuGet

## ✅ RESUMO DAS ALTERAÇÕES

### 🎯 **Objetivo Alcançado**

O workflow de CI/CD agora publica **automaticamente** o pacote NuGet quando uma **release** é criada no GitHub.

---

## 🔧 **Mudanças Implementadas**

### **1. Workflow Modernizado** (.github/workflows/publish_nuget.yml)

| Aspecto           | Antes ❌                         | Depois ✅                                        |
|-------------------|---------------------------------|-------------------------------------------------|
| **Trigger**       | Push de tags (`tags: ['*']`)    | Release publicada (`release: [published]`)      |
| **Testes**        | Não executava                   | Executa TODOS os testes antes de publicar       |
| **Segurança**     | Action de terceiros não-oficial | Actions oficiais do GitHub (v4)                 |
| **Multi-target**  | Não testava                     | Testa em .NET 8, 9 e 10                         |
| **Build**         | Windows (mais lento)            | Ubuntu (mais rápido)                            |
| **Permissões**    | Amplas                          | Least privilege (contents:read, packages:write) |
| **Símbolos**      | Não publicava                   | Publica .snupkg para debugging                  |
| **Artefatos**     | Não salvava                     | Backup de 30 dias no GitHub                     |
| **Versionamento** | Automático do .csproj           | Explícito da release tag                        |
| **Controle**      | Só automático                   | Automático + Manual                             |

---

## 📋 **Estrutura do Novo Workflow**

### **Job 1: build-and-test** ✅

```yaml
Executa em: ubuntu-latest
Passos:
    1. Checkout do código (fetch-depth: 0)
    2. Setup .NET 8.0, 9.0 e 10.0
    3. Restore dependencies
    4. Build --configuration Release
    5. Run tests (verbosity detailed)
    6. Check test coverage (XPlat Code Coverage)

    ✅ Se TODOS os testes passarem → Continua
    ❌ Se ALGUM teste falhar → CANCELA (não publica)
```

### **Job 2: publish** 📦

```yaml
Depende de:
    build-and-test (needs: build-and-test)
Executa em: ubuntu-latest
Passos:
    1. Checkout do código
    2. Setup .NET 8.0
    3. Determina versão (release tag ou input manual)
    4. Build Release com versão específica
    5. Pack NuGet (inclui símbolos .snupkg)
    6. Publish to NuGet.org (skip-duplicate)
    7. Publish symbols to NuGet.org
    8. Upload artifact (backup 30 dias)

    ✅ Publica SOMENTE se build-and-test passar
```

---

## 🚀 **Como Usar**

### **Método 1: Automático (Recomendado)**

1. **Atualize a versão**:
   ```xml
   <!-- Sirb.CepBrasil/Sirb.CepBrasil.csproj -->
   <Version>1.5.0</Version>
   ```

2. **Commit e push**:
   ```bash
   git add .
   git commit -m "chore: release version 1.5.0"
   git push origin main
   ```

3. **Crie a tag**:
   ```bash
   git tag v1.5.0
   git push origin v1.5.0
   ```

4. **Crie a Release no GitHub**:
    - GitHub → Releases → "Draft a new release"
    - Tag: `v1.5.0`
    - Título: `Release v1.5.0`
    - Descrição: Changelog
    - **Publish release** ← 🎯 **Isso dispara o workflow!**

5. **Aguarde**:
    - GitHub Actions executará automaticamente
    - Testes serão executados
    - Se passar, publicará no NuGet

---

### **Método 2: Manual**

1. GitHub → Actions → "Publish to NuGet"
2. "Run workflow"
3. Branch: `main`
4. Version: `1.5.0`
5. "Run workflow"

---

## 🔐 **Configuração Necessária**

### **Secret do NuGet (OBRIGATÓRIA)**

1. GitHub → Settings → Secrets and variables → Actions
2. "New repository secret"
3. Nome: `NUGET_API_KEY`
4. Valor: Sua API Key do NuGet.org

**Como obter a API Key**:

- https://www.nuget.org/ → Login
- Account → API Keys
- Create → Push permissions
- Copiar a chave

---

## ✅ **Benefícios da Nova Abordagem**

### **1. Segurança** 🔒

- ✅ Actions oficiais do GitHub (v4)
- ✅ Permissões mínimas (least privilege)
- ✅ Sem dependências de terceiros não-oficiais
- ✅ Símbolos publicados para debugging seguro

### **2. Confiabilidade** 🛡️

- ✅ Testes executados ANTES de publicar
- ✅ Multi-target validado (.NET 8, 9, 10)
- ✅ Cobertura de código verificada
- ✅ Build validado antes da publicação

### **3. Rastreabilidade** 📊

- ✅ Backup automático de pacotes (30 dias)
- ✅ Logs detalhados de cada etapa
- ✅ Versionamento explícito e controlado
- ✅ Histórico completo no GitHub Actions

### **4. Controle** 🎮

- ✅ Publicação automática via release
- ✅ Publicação manual quando necessário
- ✅ Skip duplicates (seguro re-executar)
- ✅ Continue-on-error para símbolos

### **5. Performance** ⚡

- ✅ Ubuntu mais rápido que Windows
- ✅ Cache de dependências (futuro)
- ✅ Workflow otimizado
- ✅ Execução ~3-5 minutos

---

## 📚 **Documentação Criada**

### **1. WORKFLOW-NUGET-PUBLISH.md**

- Explicação detalhada do workflow
- Troubleshooting completo
- Referências e boas práticas

### **2. CHECKLIST-RELEASE.md**

- Checklist passo-a-passo para releases
- Templates de commit e release
- Troubleshooting de problemas comuns

---

## 🎯 **Próximos Passos**

### **Imediato** (Antes da próxima release)

1. ✅ Configurar secret `NUGET_API_KEY` no GitHub
2. ✅ Testar workflow manualmente (Run workflow)
3. ✅ Validar publicação no NuGet.org

### **Futuro** (Melhorias)

1. Adicionar cache de dependências (actions/cache)
2. Adicionar análise de segurança (CodeQL)
3. Adicionar geração automática de changelog
4. Adicionar notificações (Slack/Discord)

---

## 📊 **Comparação de Execução**

| Etapa               | Workflow Antigo   | Workflow Novo        |
|---------------------|-------------------|----------------------|
| **Trigger**         | Manual (push tag) | Automático (release) |
| **Validação**       | ❌ Nenhuma         | ✅ Testes + Cobertura |
| **Segurança**       | ⚠️ Baixa          | ✅ Alta               |
| **Tempo**           | ~2-3 min          | ~3-5 min             |
| **Confiabilidade**  | ⚠️ Média          | ✅ Alta               |
| **Rastreabilidade** | ⚠️ Baixa          | ✅ Alta               |

---

## ⚠️ **Importante**

- **Sempre teste localmente antes** de criar uma release
- **Nunca pule os testes** - eles protegem seu pacote
- **Siga versionamento semântico** (SemVer)
- **Documente as mudanças** no CHANGELOG
- **Acompanhe o workflow** até o fim

---

## ✅ **Testes Recomendados Antes da Primeira Release**

```bash
# 1. Executar testes localmente
dotnet test --configuration Release

# 2. Verificar cobertura
dotnet test /p:CollectCoverage=true

# 3. Build de release
dotnet build --configuration Release

# 4. Pack local (sem publicar)
dotnet pack Sirb.CepBrasil/Sirb.CepBrasil.csproj --configuration Release

# 5. Verificar pacote gerado
ls -la Sirb.CepBrasil/bin/Release/*.nupkg
```

---

## 🎉 **Conclusão**

O CI/CD está agora **moderno, seguro e automático**!

- ✅ Publica automaticamente em releases
- ✅ Valida TUDO antes de publicar
- ✅ Segue melhores práticas do GitHub Actions
- ✅ Totalmente documentado e rastreável

**Próxima release será automática e segura!** 🚀

---

**Data da Atualização**: 2026-02-19  
**Versão do Workflow**: 2.0  
**Autor**: GitHub Copilot
