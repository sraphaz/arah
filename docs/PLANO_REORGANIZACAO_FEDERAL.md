# Plano de Reorganização Federal da Documentação

Documentação organizada por **domínios/temas** (estrutura federal), sem duplicação de nomes e com um único lugar por tema.

---

## 1. Princípios

- **Um tema, uma pasta**: documentação agrupada por domínio (produto, arquitetura, API, operações, backlog, etc.).
- **Sem duplicados de nome**: cada nome de arquivo único no repositório reflete um único propósito; duplicados são unificados ou renomeados.
- **Raiz limpa**: na raiz ficam apenas README, CONTRIBUTING, CODE_OF_CONDUCT, SECURITY, LICENSE(s), configs (Docker, package.json, global.json).
- **PRs e temporários**: descrições de PR e arquivos de sessão vão para arquivo ou são removidos (conteúdo já está no git).

---

## 2. Estrutura Federal Proposta (`/docs`)

```
docs/
├── README.md                      # Índice geral (o que é o Arah, links por domínio)
├── CHANGELOG.md                   # Único changelog (unificado a partir de 40_CHANGELOG + CHANGELOG)
│
├── 01-produto/                    # Visão, roadmap, glossário, user stories
│   ├── visao-produto.md
│   ├── roadmap.md
│   ├── backlog.md
│   ├── user-stories.md
│   └── glossario.md
│
├── 02-arquitetura/                # ADRs, serviços, domínio, roteamento
│   ├── decisoes-arquiteturais.md
│   ├── arquitetura-servicos.md
│   ├── modelo-dominio.md
│   └── domain-routing.md
│
├── 03-api/                        # Documentação da API (já existe docs/api/)
│   ├── visao-geral.md
│   ├── autenticacao.md
│   ├── territorios.md
│   ├── ... (subdocs por recurso)
│   └── logica-negocio-index.md
│
├── 04-bff/                        # BFF: contrato, guias, Postman
│   ├── avaliacao-bff.md
│   ├── contrato-openapi.md
│   ├── guia-frontend.md
│   ├── exemplo-flutter.md
│   └── postman-readme.md
│
├── 05-app-flutter/                # App mobile: plano, roadmap, design, testes, acessibilidade
│   ├── plano-frontend.md
│   ├── roadmap-implementacao.md
│   ├── diretrizes-design.md
│   ├── metricas-logging.md
│   ├── testes.md
│   ├── acessibilidade.md
│   ├── i18n.md
│   └── alinhamento-api.md
│
├── 06-operacoes/                  # Runbook, monitoramento, segurança, deploy
│   ├── runbook.md
│   ├── operacao-basica.md
│   ├── operations-manual.md
│   ├── monitoring.md
│   ├── metrics.md
│   ├── troubleshooting.md
│   ├── connection-pooling-metrics.md
│   ├── security-configuration.md
│   ├── security-audit.md
│   └── deployment-multi-instance.md
│
├── 07-governanca-moderacao/       # Moderação, admin, system config, work queue, rastreabilidade
│   ├── moderacao.md
│   ├── admin-observability.md
│   ├── system-config-workqueue.md
│   ├── traceability.md
│   └── community-moderation.md
│
├── 08-backlog-fases/              # Fases do backlog (FASE1.md ... FASE49, implementações)
│   ├── README.md
│   ├── FASE1.md ... FASE49.md
│   ├── implementacoes/
│   └── arquivos-originais/
│
├── 09-funcional/                  # Descrições funcionais por capacidade (já existe docs/funcional/)
│   ├── plataforma.md
│   ├── autenticacao.md
│   ├── territorios-memberships.md
│   └── ...
│
├── 10-wiki-devportal/             # Wiki, DevPortal, análise, sync
│   ├── wiki-home.md
│   ├── devportal-analise.md
│   └── wiki-sync-checklist.md
│
├── 11-tecnico-especial/            # Instalador, modularização, Cursor, configuração
│   ├── indice-tecnico.md
│   ├── instalador-visual.md
│   ├── modularizacao.md
│   └── cursor-config.md
│
└── archive/                       # Histórico de PRs e documentos substituídos
    ├── prs/                       # Descrições de PR (referência)
    └── substituidos/              # Versões antigas unificadas
```

---

## 3. Duplicados Identificados e Ação

| Nome            | Localizações | Ação |
|-----------------|-------------|------|
| **CONTRIBUTING.md** | Raiz, docs/41_CONTRIBUTING.md | Manter **raiz** como canônico. docs/41 → remover ou converter em 1 linha apontando para `/CONTRIBUTING.md`. |
| **README.md**   | Raiz, docs/README.md, backend/, frontend/wiki/, frontend/portal/, scripts/seed/, docs/backlog-api/, docs/funcional/ | Raiz = projeto. docs/README.md = índice da documentação (renomear para docs/00_INDEX.md ou manter como README do “pacote” docs). Outros READMEs permanecem em cada módulo (backend, wiki, portal, seed). |
| **CHANGELOG.md**| docs/CHANGELOG.md, docs/40_CHANGELOG.md | **Unificar** em um único docs/CHANGELOG.md: usar 40_CHANGELOG como base (versões e fases), acrescentar entradas “Unreleased” do outro. Depois remover 40_CHANGELOG.md. |
| **README_IMAGENS.md** | frontend/wiki/public/, (outro?) | Manter só em frontend/wiki/public/ (wiki). Se existir outro, unificar conteúdo e manter um. |

---

## 4. Arquivos na Raiz – Ação

| Arquivo | Ação |
|---------|------|
| DOCUMENTATION_REORGANIZATION_PLAN.md | Mover para docs/archive/substituidos/ (substituído por este plano). |
| Kanban sem título 1.md, Kanban sem título.md | Mover para docs/archive/ ou design/Backlog/ (se forem backlog); senão archive. |
| pr_body_completo.txt, pr_body_wiki_header_icons.txt | Remover (temporários de PR). |
| PR_FASE10_COMPLETA.md, PR_FASE12_COMPLETA.md, PR_FIX_*.md, PR_WIKI_*.md, PR_FIXES.md | Mover para docs/archive/prs/. |
| RESUMO_IMPLEMENTACAO_FASES_9_12.md, RESUMO_IMPLEMENTACAO_MVP.md | Mover para docs/08-backlog-fases/resumos/ ou archive. |
| WIKI_COMPATIBILITY_GUARANTEE.md | Mover para docs/10-wiki-devportal/. |
| Sem título 1.canvas, Sem título.canvas | Mover para design/ ou archive (se forem rascunhos). |

Manter na raiz: README.md, CONTRIBUTING.md, CODE_OF_CONDUCT.md, SECURITY.md, LICENSE, LICENSE.pt-BR, .cursorrules, Dockerfile, docker-compose*.yml, package.json, global.json, .gitignore, .editorconfig, .env.example.

---

## 5. Classificação por Tema (Resumo)

- **Produto**: 01_PRODUCT_VISION, 02_ROADMAP, 03_BACKLOG, 04_USER_STORIES, 05_GLOSSARY → 01-produto/
- **Arquitetura**: 10_ARCHITECTURE_*, 11_*, 12_DOMAIN_MODEL, 13_DOMAIN_ROUTING → 02-arquitetura/
- **API**: docs/api/* (60_*) → 03-api/ (ou manter docs/api/ e apenas referenciar no índice federal).
- **BFF**: BFF_*, BFF_Postman* → 04-bff/
- **App Flutter**: 24_* a 29_*, 31_FLUTTER_*, 32_*, 33_* (Flutter), 34_*, 38_* (Flutter), etc. → 05-app-flutter/
- **Operações**: RUNBOOK, OPERACAO_BASICA, OPERATIONS_MANUAL, DEPLOYMENT_*, CONNECTION_POOLING_*, MONITORING, METRICS, TROUBLESHOOTING, SECURITY_* → 06-operacoes/
- **Governança/Admin**: 30_MODERATION, 31_ADMIN_*, 32_TRACEABILITY, 33_ADMIN_*, COMMUNITY_MODERATION → 07-governanca-moderacao/
- **Backlog/Fases**: backlog-api/* → 08-backlog-fases/
- **Funcional**: funcional/* → 09-funcional/
- **Wiki/DevPortal**: WIKI_*, 42_WIKI_*, ANALISE_DEVPORTAL_* → 10-wiki-devportal/
- **Técnico**: TECNICO_*, CURSOR_*, CURSOR_CONFIG_* → 11-tecnico-especial/
- **PRs / resumos antigos**: docs/prs/*, PR_* da raiz → docs/archive/prs/

---

## 6. Documentação a Atualizar (Implementado no App)

Garantir que estes temas reflitam o que **já está implementado** no app (rebrand Arah, arah.app, BFF, jornadas):

- **docs/funcional/00_PLATAFORMA_ARAPONGA.md** → Renomear/conteúdo para “Plataforma Arah”; referências a “Araponga” → “Arah”.
- **README.md (raiz e docs)** → Verificar menções a arah.app, BFF, e links para DevPortal/Wiki.
- **02_ROADMAP.md / backlog** → Marcar itens já entregues (onboarding, feed, mapa, eventos, perfil, notificações, territórios).
- **BFF_FLUTTER_EXAMPLE.md, BFF_FRONTEND_IMPLEMENTATION_GUIDE.md** → Conferir exemplos com package arah_app e endpoints atuais.
- **DevPortal e Wiki** → Trocar referências antigas (araponga.app, araponga) por arah.app e Arah onde fizer sentido.
- **docs/api/** e **60_API_LÓGICA_NEGÓCIO** → Revisar lista de endpoints e jornadas implementadas vs. documentado.

---

## 7. Ordem de Execução Sugerida

1. **Criar pastas** em docs/ (01-produto … 11-tecnico-especial, archive/prs, archive/substituidos).
2. **Unificar CHANGELOG**: mesclar docs/40_CHANGELOG.md em docs/CHANGELOG.md; remover 40_CHANGELOG.md.
3. **Resolver CONTRIBUTING**: manter raiz; docs/41_CONTRIBUTING.md → remover ou redirecionar.
4. **Mover documentos** da raiz para docs/ conforme tabela da seção 4 (e segundo mapa da seção 5).
5. **Mover docs existentes** dos números (01_*, 02_*, …) e demais para as pastas federais (renomear para kebab-case onde ajudar).
6. **Arquivar** docs/prs/* e PR_*.md da raiz em docs/archive/prs/.
7. **Atualizar** docs/00_INDEX.md (ou docs/README.md) com a nova estrutura federal e links.
8. **Atualizar** referências em documentos e no wiki (links quebrados, nomes de arquivo).
9. **Revisar e atualizar** documentos listados na seção 6 (implementado no app).

---

## 8. Validação Final

- [ ] Nenhum arquivo duplicado com o mesmo nome e propósito distinto.
- [ ] Raiz só com arquivos essenciais listados acima.
- [ ] docs/ organizado apenas por pastas temáticas (federal).
- [ ] Índice (README ou 00_INDEX) reflete a estrutura e abre por tema.
- [ ] Links internos e do wiki testados (scripts de link já existentes).
- [ ] Documentação de produto/API/BFF/app alinhada ao que está implementado (Arah, arah.app).
