# ✅ Checklist de Sincronização Wiki

**Verificação de que todos os documentos de onboarding estão incluídos na Wiki**

---

## 📋 Documentos de Onboarding

### ✅ Mapeamento no Script

Verifique que estes arquivos estão no mapeamento `$mainDocs` em `script-sync-wiki.ps1`:

- [x] `ONBOARDING_PUBLICO.md` → `Onboarding-Público`
- [x] `ONBOARDING_DEVELOPERS.md` → `Onboarding-Desenvolvedores`
- [x] `ONBOARDING_ANALISTAS_FUNCIONAIS.md` → `Onboarding-Analistas-Funcionais`
- [x] `CARTILHA_COMPLETA.md` → `Cartilha-Completa`
- [x] `DISCORD_SETUP.md` → `Discord-Setup`

### ✅ Página Inicial da Wiki

- [x] `WIKI_HOME.md` é usado para criar `Home.md` na Wiki
- [x] Script ajusta links de onboarding automaticamente
- [x] Home.md direciona para Onboarding-Público

---

## 🔗 Ajuste de Links

O script `script-sync-wiki.ps1` ajusta automaticamente os seguintes formatos de link:

### Links Relativos
- `../ONBOARDING_PUBLICO.md` → `[Onboarding Público](Onboarding-Público)`
- `../docs/ONBOARDING_DEVELOPERS.md` → `[Onboarding Desenvolvedores](Onboarding-Desenvolvedores)`
- `./ONBOARDING_ANALISTAS_FUNCIONAIS.md` → `[Onboarding Analistas Funcionais](Onboarding-Analistas-Funcionais)`

### Links Absolutos do GitHub
- `https://github.com/sraphaz/araponga/blob/main/docs/ONBOARDING_PUBLICO.md` → `[Onboarding Público](Onboarding-Público)`
- `https://github.com/sraphaz/araponga/blob/main/docs/CARTILHA_COMPLETA.md` → `[Cartilha Completa](Cartilha-Completa)`

---

## 🚀 Como Sincronizar

### Executar o Script

```powershell
# No PowerShell, na pasta docs/backlog-api/
.\script-sync-wiki.ps1
```

### O que o Script Faz

1. ✅ Clona ou cria a Wiki do GitHub
2. ✅ Cria `Home.md` usando `WIKI_HOME.md` (se existir)
3. ✅ Copia todos os documentos mapeados em `$mainDocs`
4. ✅ Ajusta links automaticamente para estrutura da Wiki
5. ✅ Faz commit e push para a Wiki

---

## 📊 Status Atual

**Última verificação**: 2025-01-20

### Arquivos Garantidos na Sincronização

| Arquivo Original | Nome na Wiki | Status |
|-----------------|--------------|--------|
| `WIKI_HOME.md` | `Home.md` | ✅ Usado como template |
| `ONBOARDING_PUBLICO.md` | `Onboarding-Público` | ✅ Mapeado |
| `ONBOARDING_DEVELOPERS.md` | `Onboarding-Desenvolvedores` | ✅ Mapeado |
| `ONBOARDING_ANALISTAS_FUNCIONAIS.md` | `Onboarding-Analistas-Funcionais` | ✅ Mapeado |
| `CARTILHA_COMPLETA.md` | `Cartilha-Completa` | ✅ Mapeado |
| `DISCORD_SETUP.md` | `Discord-Setup` | ✅ Mapeado |

---

## 🔍 Verificação Manual

Após executar o script, verifique na Wiki:

1. ✅ `Home.md` existe e tem conteúdo de `WIKI_HOME.md`
2. ✅ `Onboarding-Público` existe
3. ✅ `Onboarding-Desenvolvedores` existe
4. ✅ `Onboarding-Analistas-Funcionais` existe
5. ✅ `Cartilha-Completa` existe
6. ✅ `Discord-Setup` existe
7. ✅ Links entre documentos funcionam corretamente
8. ✅ Home.md direciona para Onboarding-Público

---

## 📝 Notas

- **WIKI_HOME.md** não é copiado como página separada - é usado apenas para criar `Home.md`
- Todos os outros documentos são copiados como páginas individuais na Wiki
- Links são ajustados automaticamente pelo script
- O script adiciona um link "Documento completo" no repositório no final de cada página

---

**Última Atualização**: 2025-01-20
