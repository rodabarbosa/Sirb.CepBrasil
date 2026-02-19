# 📦 Workflow de Publicação no NuGet - Documentação

## 🎯 Objetivo

Este workflow automatiza a publicação do pacote `Sirb.CepBrasil` no NuGet.org sempre que uma **release** for publicada no GitHub.

---

## 🚀 Como Funciona

### **Triggers (Gatilhos)**

O workflow é disparado automaticamente em **2 situações**:

1. **Release Publicada** (automático):
   ```yaml
   on:
     release:
       types: [published]
   ```
   - Quando você cria e publica uma release no GitHub
   - A versão é extraída automaticamente da tag da release (ex: `v1.4.0` → `1.4.0`)

2. **Manualmente** (opcional):
   ```yaml
   workflow_dispatch:
     inputs:
       version:
         description: 'Version to publish (e.g., 1.4.0)'
         required: true
         type: string
   ```
   - Você pode executar manualmente via GitHub Actions UI
   - Útil para re-publicar ou publicar versões específicas

---

## 📋 Processo de Publicação

O workflow é dividido em **2 jobs sequenciais**:

### **Job 1: Build and Test** ✅

Valida o código antes de publicar:

```yaml
build-and-test:
  runs-on: ubuntu-latest
  steps:
    - Checkout do código
    - Setup .NET (8.0, 9.0, 10.0)
    - Restaurar dependências
    - Build em Release
    - Executar todos os testes
    - Verificar cobertura de código
```

**✅ Se todos os testes passarem** → Continua para o próximo job  
**❌ Se algum teste falhar** → Workflow é cancelado (não publica)

---

### **Job 2: Publish** 📦

Publica o pacote no NuGet.org:

```yaml
publish:
  needs: build-and-test  # Só executa se Job 1 passar
  runs-on: ubuntu-latest
  steps:
    - Checkout do código
    - Setup .NET 8.0
    - Determina a versão (da release ou manual)
    - Build com a versão específica
    - Empacota o NuGet (com símbolos)
    - Publica no NuGet.org
    - Upload do pacote como artefato (backup)
```

---

## 🔐 Configuração Necessária

### **Secret Obrigatória**

Você precisa configurar a seguinte secret no GitHub:

1. Vá em: **Settings** → **Secrets and variables** → **Actions** → **New repository secret**
2. Nome: `NUGET_API_KEY`
3. Valor: Sua API Key do NuGet.org

**Como obter a API Key do NuGet:**
1. Acesse https://www.nuget.org/
2. Faça login na sua conta
3. Vá em: **Account** → **API Keys**
4. Crie uma nova chave com permissões de **Push**

---

## 📝 Como Publicar uma Nova Versão

### **Método 1: Criar Release no GitHub (Recomendado)**

1. **Atualize a versão no projeto**:
   ```xml
   <!-- Sirb.CepBrasil/Sirb.CepBrasil.csproj -->
   <PropertyGroup>
     <Version>1.5.0</Version>
   </PropertyGroup>
   ```

2. **Commit e push das alterações**:
   ```bash
   git add .
   git commit -m "chore: bump version to 1.5.0"
   git push origin main
   ```

3. **Crie uma tag**:
   ```bash
   git tag v1.5.0
   git push origin v1.5.0
   ```

4. **Crie a Release no GitHub**:
   - Vá em: **Releases** → **Draft a new release**
   - Escolha a tag: `v1.5.0`
   - Título: `Release v1.5.0`
   - Descrição: Liste as mudanças (changelog)
   - Clique em: **Publish release**

5. **Aguarde o workflow**:
   - Vá em **Actions** e acompanhe o workflow
   - Se tudo estiver OK, o pacote será publicado automaticamente

---

### **Método 2: Execução Manual**

1. Vá em: **Actions** → **Publish to NuGet**
2. Clique em: **Run workflow**
3. Escolha a branch: `main`
4. Digite a versão: `1.5.0`
5. Clique em: **Run workflow**

---

## ✅ Melhorias Implementadas

Comparado com o workflow antigo, as melhorias são:

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Trigger** | Push de tags | Release publicada + Manual |
| **Testes** | ❌ Não executava | ✅ Executa antes de publicar |
| **Build** | Windows | Linux (mais rápido) |
| **Multi-target** | ❌ | ✅ .NET 8, 9 e 10 |
| **Actions** | v2 (desatualizado) | v4 (atual) |
| **Segurança** | Permissões amplas | Least privilege |
| **Cobertura** | ❌ | ✅ Verifica cobertura |
| **Símbolos** | ❌ | ✅ Publica .snupkg |
| **Artefatos** | ❌ | ✅ Backup de 30 dias |
| **Versionamento** | Automático (às vezes confuso) | Explícito da release |

---

## 🔍 Monitoramento

### **Ver Status do Workflow**

1. Vá em: **Actions**
2. Selecione: **Publish to NuGet**
3. Veja o histórico de execuções

### **Ver Logs Detalhados**

1. Clique na execução específica
2. Clique no job: **build-and-test** ou **publish**
3. Expanda os steps para ver logs

### **Verificar Publicação**

Após a publicação bem-sucedida:
- Acesse: https://www.nuget.org/packages/Sirb.CepBrasil
- Verifique se a nova versão está listada
- Tempo de indexação: ~15-30 minutos

---

## ⚠️ Troubleshooting

### **Erro: "NUGET_API_KEY not found"**

**Causa**: Secret não configurada  
**Solução**: Configure a secret conforme seção "Configuração Necessária"

### **Erro: "Tests failed"**

**Causa**: Algum teste unitário falhou  
**Solução**: 
1. Veja os logs do step "Run tests"
2. Corrija os testes localmente
3. Commit e push novamente

### **Erro: "Package already exists"**

**Causa**: Você está tentando publicar uma versão que já existe no NuGet  
**Solução**: 
1. Incremente a versão no `.csproj`
2. Crie uma nova release com a nova versão

### **Erro: "401 Unauthorized"**

**Causa**: API Key inválida ou expirada  
**Solução**: 
1. Gere uma nova API Key no NuGet.org
2. Atualize a secret `NUGET_API_KEY` no GitHub

---

## 🎯 Boas Práticas

1. **Sempre crie releases semânticas**: `v1.0.0`, `v1.1.0`, `v2.0.0`
2. **Documente as mudanças**: Use o changelog da release
3. **Teste localmente antes**: Execute `dotnet test` antes de criar a release
4. **Valide a versão**: Certifique-se de que incrementou corretamente
5. **Monitore a publicação**: Acompanhe o workflow até o fim

---

## 📚 Referências

- [GitHub Actions - Publishing to NuGet](https://docs.github.com/en/actions/publishing-packages/publishing-nuget-packages)
- [NuGet.org Documentation](https://docs.microsoft.com/en-us/nuget/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Releases](https://docs.github.com/en/repositories/releasing-projects-on-github)

---

**Última atualização**: 2026-02-19  
**Versão do workflow**: 2.0
