## 📋 Descrição

Este PR implementa dois componentes importantes para organização e documentação do projeto Araponga:

1. **Frontend Wiki** - Site completo de documentação construído com Next.js 15, usando a identidade visual do Araponga
2. **Script de Setup do Discord** - Automação para configurar o servidor Discord do projeto

### Wiki Araponga (`frontend/wiki/`)

Criação de um site completo de documentação que substitui a Wiki do GitHub por uma experiência melhor:

- ✅ **Next.js 15** com TypeScript e Tailwind CSS
- ✅ **Identidade Visual do Araponga** (glass-card, paleta Forest, watermark)
- ✅ **Carregamento dinâmico** de documentos markdown de `docs/`
- ✅ **Navegação hierárquica** organizada por categorias
- ✅ **CI/CD completo** com GitHub Actions (testes + deploy automático)
- ✅ **Deploy para GitHub Pages** em `wiki.araponga.app` (CNAME configurado)

### Script Discord (`scripts/discord-setup.js`)

Automação para configurar o servidor Discord do Araponga:

- ✅ Criação automática de categorias e canais
- ✅ Configuração de permissões
- ✅ Mensagem de boas-vindas fixada
- ✅ Geração de link permanente de convite
- ✅ Tratamento de erros robusto com diagnósticos detalhados

## 🔄 Tipo de Mudança

- [x] Nova funcionalidade
- [ ] Correção de bug
- [ ] Refatoração
- [x] Mudança em documentação
- [x] Mudança em configuração
- [ ] Outro

## ✅ Checklist de Documentação

### Documentação Técnica
- [x] Documentação do Wiki criada em `frontend/wiki/README.md`
- [x] Documentação do CI/CD criada em `frontend/wiki/CI_CD.md`
- [x] Documentação do Discord atualizada em `docs/DISCORD_SETUP.md`
- [x] Guia do script Discord criado em `scripts/discord-setup-guide.md`

### Documentação de Produto
- [x] Atualizado `docs/ONBOARDING_PUBLICO.md` com link permanente do Discord
- [x] Atualizado `docs/DISCORD_SETUP.md` com link permanente

### Histórico e Changelog
- [ ] Será atualizado em `docs/40_CHANGELOG.md` após merge

## 📝 Lista de Documentos Atualizados

- `docs/ONBOARDING_PUBLICO.md` - Link permanente do Discord atualizado
- `docs/DISCORD_SETUP.md` - Link permanente do Discord e documentação do setup
- `scripts/discord-setup-guide.md` - Novo guia completo para usar o script
- `frontend/wiki/README.md` - Novo: documentação do Wiki
- `frontend/wiki/CI_CD.md` - Novo: documentação do CI/CD

## 🧪 Testes

- [x] Testes Jest criados para validação de documentos (`__tests__/docs.test.ts`)
- [x] CI/CD configurado com testes automatizados
- [x] Type check configurado (`npm run type-check`)
- [x] Lint configurado (`npm run lint`)
- [x] Build validado localmente

### Testes Incluídos

- ✅ Verificação de existência da pasta `docs/`
- ✅ Validação de arquivos markdown
- ✅ Teste de arquivos principais (`00_INDEX.md`, `ONBOARDING_PUBLICO.md`)
- ✅ Validação de encoding UTF-8

## 🔗 Links Relacionados

- Link permanente do Discord: https://discord.gg/auwqN8Yjgw
- Wiki (quando deployado): https://wiki.araponga.app
- Script Discord: `scripts/discord-setup.js`

## 📸 Screenshots (se aplicável)

O Wiki utiliza a mesma identidade visual do portal principal do Araponga (glass-card, paleta Forest, watermark do logo).

## ⚠️ Breaking Changes

- [ ] Esta mudança quebra compatibilidade com versões anteriores
- [ ] Documentei breaking changes em `docs/40_CHANGELOG.md`
- [ ] Adicionei guia de migração (se necessário)

**Nenhum breaking change**. Adiciona novas funcionalidades sem impactar existentes.

## 🔄 Sincronização Wiki

- [x] Wiki será substituída pelo novo site em `wiki.araponga.app`
- [ ] Será necessário desabilitar Wiki do GitHub após deploy bem-sucedido

## 📦 Arquivos Criados

### Frontend Wiki
- `frontend/wiki/app/page.tsx` - Página inicial
- `frontend/wiki/app/docs/[slug]/page.tsx` - Páginas dinâmicas de documentos
- `frontend/wiki/app/docs/page.tsx` - Lista de todos os documentos
- `frontend/wiki/app/layout.tsx` - Layout raiz
- `frontend/wiki/app/globals.css` - Estilos globais com identidade visual
- `frontend/wiki/__tests__/docs.test.ts` - Testes
- `frontend/wiki/jest.config.js` - Configuração Jest
- `frontend/wiki/jest.setup.js` - Setup Jest
- `frontend/wiki/package.json` - Dependências e scripts
- `frontend/wiki/next.config.mjs` - Configuração Next.js
- `frontend/wiki/tailwind.config.ts` - Configuração Tailwind
- `frontend/wiki/tsconfig.json` - Configuração TypeScript
- `frontend/wiki/CI_CD.md` - Documentação CI/CD
- `frontend/wiki/README.md` - Documentação do Wiki

### CI/CD
- `.github/workflows/wiki-pages.yml` - Workflow completo de CI/CD

### Scripts Discord
- `scripts/discord-setup.js` - Script de automação do Discord
- `scripts/discord-setup-guide.md` - Guia completo do script

## 🔧 Configurações Necessárias Após Merge

### GitHub Pages

1. **Habilitar GitHub Pages**:
   - Settings → Pages → Source: **GitHub Actions**

2. **Configurar DNS** (após primeiro deploy):
   - Adicionar registro CNAME: `wiki` → `<usuario>.github.io`
   - Ou usar IPs A do GitHub Pages

### Discord

Nenhuma configuração adicional necessária. O script pode ser executado novamente para atualizar configurações.

## 🚀 Como Testar

### Wiki Localmente

```bash
cd frontend/wiki
npm install
npm run dev
# Acessar http://localhost:3001
```

### Script Discord

```bash
node scripts/discord-setup.js
# Seguir instruções no guia: scripts/discord-setup-guide.md
```

## 📊 Estatísticas

- **Arquivos criados**: ~20+ arquivos
- **Linhas de código**: ~2000+ linhas
- **Testes**: 5 testes automatizados
- **CI/CD**: 3 jobs (CI, Build, Deploy)

---

**⚠️ Lembrete**: Após o merge, será necessário habilitar GitHub Pages nas configurações do repositório para que o deploy automático funcione.
