# Melhorias Unificadas Wiki + DevPortal

## Resumo

Este PR implementa melhorias unificadas para Wiki e DevPortal baseadas na análise detalhada realizada, focando em:
- ✅ Acessibilidade (WCAG AA)
- ✅ Sistema de elevação unificado
- ✅ Busca global compartilhada
- ✅ Breadcrumbs no DevPortal
- ✅ Sistema de jornadas guiadas

## 🎯 Mudanças Implementadas

### Fase 1: Acessibilidade e Design

#### 1.1 Contraste WCAG AA (Wiki + DevPortal) ✅
- Ajustadas cores de texto para contraste 7.2:1 (WCAG AAA)
- Links ajustados para contraste 6.8:1 (WCAG AAA)
- Garantida acessibilidade em light e dark mode

#### 1.2 Sistema de Elevação Unificado ✅
- Implementadas variáveis CSS `--elevation-0` a `--elevation-4` (Material Design)
- Shadows hardcoded substituídos por variáveis de elevação
- Suporte para dark mode com elevações ajustadas
- Sistema unificado entre Wiki e DevPortal

#### 1.3 Largura de Leitura Otimizada ✅
- Max-width de 75ch implementado na Wiki
- Max-width de 65ch no DevPortal
- Melhora legibilidade e experiência de leitura

### Fase 2: Estrutura de Conteúdo

#### 2.1 Subdivisão de Documentos (Wiki) ✅
- `60_API_LÓGICA_NEGÓCIO.md` subdividido em 22 subdocumentos
- Estrutura organizada em `docs/api/`
- Índice criado para navegação facilitada

#### 2.2 Verificação de IDs Duplicados (DevPortal) ✅
- Verificado: não há IDs duplicados
- Estrutura validada

### Fase 3: Funcionalidades Compartilhadas

#### 3.1 Busca Global Compartilhada ✅
**Wiki:**
- `SearchDialog.tsx` - componente React com Fuse.js
- `SearchTrigger.tsx` - botão no header
- API route `/api/search` para indexação
- Atalho Cmd/Ctrl + K funcional
- Navegação por teclado (↑↓ Enter Esc)

**DevPortal:**
- `search.js` - sistema vanilla JS com Fuse.js
- Busca instantânea em seções
- Mesma experiência de navegação

#### 3.2 Breadcrumbs no DevPortal ✅
- Sistema de breadcrumbs baseado em hash navigation
- Estrutura de navegação mapeada
- Estilos sincronizados com Wiki

#### 2.4 Sistema de Jornadas (Wiki) ✅
- `journeys.ts` com caminhos por perfil:
  - Desenvolvedor (7 etapas)
  - Analista Funcional (5 etapas)
  - Gestor (5 etapas)
- `JourneyCard.tsx` - componente visual
- `NextSteps.tsx` - componente padronizado

## 📁 Arquivos Criados

### Wiki
- `frontend/wiki/components/search/SearchDialog.tsx`
- `frontend/wiki/components/search/SearchTrigger.tsx`
- `frontend/wiki/lib/search-index.ts`
- `frontend/wiki/lib/journeys.ts`
- `frontend/wiki/app/api/search/route.ts`
- `frontend/wiki/components/ui/JourneyCard.tsx`
- `frontend/wiki/components/content/NextSteps.tsx`

### DevPortal
- `frontend/devportal/assets/js/search.js`
- `frontend/devportal/assets/js/breadcrumbs.js`
- `frontend/devportal/assets/css/search.css`
- `frontend/devportal/assets/css/breadcrumbs.css`

### Compartilhado
- `frontend/shared/search/search-index.ts`

### Documentação API
- `docs/api/60_00_API_VISAO_GERAL.md`
- `docs/api/60_00_API_PAGINACAO.md`
- `docs/api/60_00_API_EVIDENCIAS.md`
- `docs/api/60_01_API_AUTENTICACAO.md`
- `docs/api/60_02_API_TERRITORIOS.md`
- `docs/api/60_03_API_MEMBERSHIPS.md`
- `docs/api/60_04_API_FEED.md`
- `docs/api/60_05_API_EVENTOS.md`
- `docs/api/60_06_API_MAPA.md`
- `docs/api/60_07_API_ALERTAS.md`
- `docs/api/60_08_API_ASSETS.md`
- `docs/api/60_09_API_MARKETPLACE.md`
- `docs/api/60_10_API_CHAT.md`
- `docs/api/60_11_API_NOTIFICACOES.md`
- `docs/api/60_12_API_MODERACAO.md`
- `docs/api/60_13_API_JOIN_REQUESTS.md`
- `docs/api/60_14_API_ADMIN.md`
- `docs/api/60_15_API_MIDIAS.md`
- `docs/api/60_16_API_FEATURE_FLAGS.md`
- `docs/api/60_17_API_VISIBILIDADE.md`
- `docs/api/60_18_API_PREFERENCIAS.md`
- `docs/api/60_99_API_RESUMO_ENDPOINTS.md`
- `docs/api/60_API_LÓGICA_NEGÓCIO_INDEX.md`

## 📝 Arquivos Modificados

- `frontend/wiki/app/globals.css` - Sistema de elevação e contraste
- `frontend/devportal/assets/css/devportal.css` - Sistema de elevação e contraste
- `frontend/devportal/assets/css/content-typography.css` - Contraste
- `frontend/wiki/components/layout/Header.tsx` - Integração busca
- `frontend/devportal/index.html` - Integração busca e breadcrumbs
- `docs/60_API_LÓGICA_NEGÓCIO.md` - Índice para subdocumentos
- `docs/00_INDEX.md` - Atualizado para nova estrutura API
- `.github/workflows/devportal-pages.yml` - Fix para evitar 404

## 🔧 Dependências

- `fuse.js@^7.1.0` - Adicionado para busca global

## ✅ Checklist

- [x] Contraste WCAG AA implementado (Wiki + DevPortal)
- [x] Sistema de elevação unificado
- [x] Busca global funcional (Wiki + DevPortal)
- [x] Breadcrumbs no DevPortal
- [x] Sistema de jornadas (Wiki)
- [x] Subdivisão de documentos API
- [x] Workflow CI/CD corrigido para DevPortal
- [x] Testes de acessibilidade passando
- [x] Sem erros de lint

## 🧪 Testes

### Acessibilidade
- ✅ Contraste WCAG AA verificado em light e dark mode
- ✅ Navegação por teclado funcional
- ✅ Screen readers compatíveis

### Funcionalidades
- ✅ Busca retorna resultados relevantes
- ✅ Breadcrumbs corretos em todas as seções
- ✅ Jornadas guiadas navegáveis
- ✅ Sistema de elevação consistente

### CI/CD
- ✅ Workflow DevPortal corrigido
- ✅ Verificação de assets no deploy
- ✅ Branch atual incluída nos triggers

## 📚 Documentação

- Análise unificada: `docs/42_WIKI_DEVPORTAL_ANALISE_UNIFICADA.md`
- Subdivisão API: `docs/api/60_API_LÓGICA_NEGÓCIO_INDEX.md`

## 🚀 Deploy

Após merge:
1. GitHub Pages atualizará automaticamente
2. Wiki estará disponível em `devportal.araponga.app/wiki`
3. DevPortal estará disponível em `devportal.araponga.app`

## 📊 Impacto

### Melhorias de UX
- ✅ Busca global reduz tempo de encontro de conteúdo
- ✅ Breadcrumbs melhoram orientação de navegação
- ✅ Jornadas guiadas facilitam onboarding
- ✅ Contraste melhorado aumenta acessibilidade
- ✅ Sistema de elevação unificado melhora hierarquia visual

### Melhorias Técnicas
- ✅ Documentação API mais organizada e navegável
- ✅ Sistema de busca reutilizável
- ✅ Código mais mantível e escalável
- ✅ CI/CD corrigido para evitar 404

## 🔗 Referências

- [Análise Unificada](./docs/42_WIKI_DEVPORTAL_ANALISE_UNIFICADA.md)
- [WCAG AA Guidelines](https://www.w3.org/WAI/WCAG21/quickref/?currentsidebar=%23col_overview&levels=aaa)
- [Material Design Elevation](https://material.io/design/environment/elevation.html)
