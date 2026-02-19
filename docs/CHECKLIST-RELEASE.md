# ✅ Checklist de Release - Sirb.CepBrasil

## 📋 Pré-Release

- [ ] **Atualizar versão no `.csproj`**
  ```xml
  <Version>X.Y.Z</Version>
  ```

- [ ] **Atualizar `CHANGELOG.md`**
  - [ ] Adicionar seção para nova versão
  - [ ] Listar features adicionadas
  - [ ] Listar bugs corrigidos
  - [ ] Listar breaking changes (se houver)

- [ ] **Executar testes localmente**
  ```bash
  dotnet test --configuration Release
  ```

- [ ] **Verificar cobertura de código**
  ```bash
  dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
  ```

- [ ] **Build local**
  ```bash
  dotnet build --configuration Release
  ```

- [ ] **Verificar warnings de compilação**
  - [ ] Nenhum warning deve existir

- [ ] **Revisar documentação XML**
  - [ ] Todos os métodos públicos documentados
  - [ ] Documentação em inglês

---

## 🏷️ Criação da Tag e Release

- [ ] **Commit das alterações**
  ```bash
  git add .
  git commit -m "chore: release version X.Y.Z"
  git push origin main
  ```

- [ ] **Criar tag**
  ```bash
  git tag vX.Y.Z
  git push origin vX.Y.Z
  ```

- [ ] **Criar Release no GitHub**
  - [ ] Ir em: Releases → Draft a new release
  - [ ] Escolher tag: vX.Y.Z
  - [ ] Título: Release vX.Y.Z
  - [ ] Descrição: Copiar do CHANGELOG.md
  - [ ] Marcar como pre-release (se aplicável)
  - [ ] Publicar release

---

## 🤖 Acompanhar Workflow

- [ ] **Ir para Actions**
  - [ ] Workflow: "Publish to NuGet"
  - [ ] Ver execução em andamento

- [ ] **Job: build-and-test**
  - [ ] ✅ Checkout OK
  - [ ] ✅ Setup .NET OK
  - [ ] ✅ Restore OK
  - [ ] ✅ Build OK
  - [ ] ✅ Tests OK (todos passaram)
  - [ ] ✅ Coverage OK

- [ ] **Job: publish**
  - [ ] ✅ Checkout OK
  - [ ] ✅ Setup .NET OK
  - [ ] ✅ Version determined OK
  - [ ] ✅ Build Release OK
  - [ ] ✅ Pack OK
  - [ ] ✅ Publish to NuGet OK
  - [ ] ✅ Publish symbols OK
  - [ ] ✅ Upload artifact OK

---

## ✅ Pós-Release

- [ ] **Verificar no NuGet.org**
  - [ ] Acessar: https://www.nuget.org/packages/Sirb.CepBrasil
  - [ ] Nova versão está listada
  - [ ] Aguardar indexação (15-30 min)

- [ ] **Testar instalação**
  ```bash
  dotnet new console -n TesteCepBrasil
  cd TesteCepBrasil
  dotnet add package Sirb.CepBrasil --version X.Y.Z
  dotnet build
  ```

- [ ] **Validar README.md no NuGet**
  - [ ] Badge de versão atualizado
  - [ ] Instruções corretas

- [ ] **Atualizar badges no README.md do repositório**
  - [ ] NuGet version
  - [ ] NuGet downloads
  - [ ] Build status

- [ ] **Comunicar release** (se aplicável)
  - [ ] Anunciar em discussões do GitHub
  - [ ] Atualizar documentação externa

---

## 🚨 Se Algo Der Errado

### ❌ Testes Falharam

1. Ver logs do workflow
2. Reproduzir erro localmente
3. Corrigir o erro
4. Commit e push
5. **NÃO** criar nova release (a anterior ainda não foi publicada)
6. Deletar tag e release falha:
   ```bash
   git tag -d vX.Y.Z
   git push origin :refs/tags/vX.Y.Z
   ```
7. Repetir processo de release

### ❌ Publicação Falhou

1. Ver logs do job "publish"
2. Verificar se `NUGET_API_KEY` está configurada
3. Verificar se versão já existe no NuGet
4. Se necessário, execute manualmente:
   - Actions → Publish to NuGet → Run workflow
   - Escolha a branch e versão
   - Run

### ❌ Versão Errada Publicada

**ATENÇÃO**: NuGet não permite deletar versões

1. Se erro crítico: marque como deprecated no NuGet.org
2. Incremente versão (X.Y.Z+1)
3. Crie nova release com correção

---

## 📌 Versionamento Semântico

Siga [SemVer](https://semver.org/):

- **MAJOR** (X.0.0): Breaking changes
- **MINOR** (0.X.0): Novas features (backward compatible)
- **PATCH** (0.0.X): Bug fixes (backward compatible)

Exemplos:
- `1.4.0` → `1.4.1`: Bug fix
- `1.4.0` → `1.5.0`: Nova feature
- `1.4.0` → `2.0.0`: Breaking change

---

## 📝 Notas

- **Tempo total estimado**: 10-15 minutos
- **Tempo de indexação no NuGet**: 15-30 minutos
- **Workflow executa**: ~3-5 minutos

---

**Template de Commit de Release**:
```
chore: release version X.Y.Z

- Feature: [descrição]
- Fix: [descrição]
- Breaking: [descrição]
```

**Template de Descrição de Release**:
```markdown
## 🚀 O que há de novo

### ✨ Features
- [Feature 1]
- [Feature 2]

### 🐛 Bug Fixes
- [Fix 1]
- [Fix 2]

### ⚠️ Breaking Changes
- [Breaking 1]

### 📦 Instalação

```bash
dotnet add package Sirb.CepBrasil --version X.Y.Z
```

## 📊 Estatísticas

- **Testes**: XX passando
- **Cobertura**: XX%
- **Frameworks suportados**: .NET 8.0, 9.0, 10.0
```
