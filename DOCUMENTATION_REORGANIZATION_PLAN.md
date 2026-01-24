# 📋 Plano de Reorganização de Documentação

## 🎯 Situação Atual

**Raiz do repositório tem 43+ arquivos desorganizados:**
- ❌ Múltiplos `pr_body_*.txt` arquivos temporários
- ❌ Múltiplos `PR_*.md` com descrições de PRs históricas
- ❌ Arquivos de sessão (`RESUMO_IMPLEMENTACAO_SESSAO.md`)
- ❌ Arquivos de verificação (`verificacao_branch.md`)
- ✅ Diretorios bem estruturados: `docs/`, `design/`, `backend/`, `frontend/`, `scripts/`

## 📁 Estrutura Proposta

### Raiz (apenas essenciais)
```
/
├── README.md                 # Documentação principal do projeto
├── CONTRIBUTING.md           # Guia de contribuição
├── CODE_OF_CONDUCT.md        # Código de conduta
├── SECURITY.md               # Política de segurança
├── LICENSE                   # Licença MIT
├── LICENSE.pt-BR             # Licença em português
├── Dockerfile                # Configuração Docker
├── docker-compose.yml        # Compose development
├── package.json              # Dependências frontend
├── global.json               # Configurações .NET globais
└── .cursorrules              # Regras Cursor
```

### `/docs` (Centralizar TODA documentação)
```
/docs/
├── README.md                 # Index da documentação
├── DEVELOPMENT.md            # Guia de desenvolvimento
├── API.md                    # Documentação API
├── ARCHITECTURE.md           # Arquitetura do sistema
├── SETUP.md                  # Guia de setup
├── CHANGELOG.md              # Histórico de versões
├── backlog-api/              # Backlog de features (MANTER)
│   ├── FASE1.md
│   ├── FASE2.md
│   ├── ...
│   └── FASE14_5.md
├── wiki/                     # Documentação do wiki (NEW)
│   ├── README.md
│   ├── pages.md
│   └── ...
├── community-moderation.md   # Políticas da comunidade
├── governance-system.md      # Sistema de governança
└── voting-system.md          # Sistema de votação
```

### `/design` (MANTER - já bem estruturado)
```
/design/
├── Architecture/             # Diagramas C4
├── Backlog/                  # Backlog de design
└── Wireframes/               # Wireframes do app
```

### `/scripts` (Adicionar discord-setup.md)
```
/scripts/
├── discord-setup.md          # Guia de setup Discord (move from root)
├── discord-setup.js
├── check-design-compliance.sh
└── ... (outros scripts)
```

## 🗑️ Arquivos a Remover (Limpeza)

### Temporários (nunca commitados)
- `pr_body_*.txt` (todos - 11 arquivos)
- `pr_*.md` (descrições de PRs históricas - 14 arquivos)
- `RESUMO_IMPLEMENTACAO_SESSAO.md` (sessão específica)
- `verificacao_branch.md` (verificação temporária)
- `PREPARACAO_FASE10_RESUMO.md` (preparação histórica)
- `CORRECAO_CSS_DEVPORTAL.md` (correção específica)

**Total a remover: ~25 arquivos**

## ✅ Arquivos a Manter/Reorganizar

### Raiz (8 arquivos)
- README.md
- CONTRIBUTING.md
- CODE_OF_CONDUCT.md
- SECURITY.md
- LICENSE
- LICENSE.pt-BR
- Dockerfile
- docker-compose.yml

### Para `/docs` (10 arquivos)
- `CHANGELOG.md` → `/docs/CHANGELOG.md`
- `COMMUNITY_MODERATION.md` → `/docs/COMMUNITY_MODERATION.md`
- `GOVERNANCE_SYSTEM.md` → `/docs/GOVERNANCE_SYSTEM.md`
- `VOTING_SYSTEM.md` → `/docs/VOTING_SYSTEM.md`
- Arquivos de backlog-api (já em `/docs`)
- Criar novos: `DEVELOPMENT.md`, `API.md`, `ARCHITECTURE.md`, `SETUP.md`

### Para `/scripts` (1 arquivo)
- `discord-setup-guide.md` → `/scripts/discord-setup-guide.md`

## 🔗 Impacto no Frontend Wiki

**Verificar referências em:**
- `frontend/` - componentes de wiki
- Links internos no `docs/`
- URLs hardcoded em comentários

**Ação necessária:**
- Atualizar imports/requires se houver
- Validar links no wiki frontend
- Testar navegação pós-reorganização

## 📋 Checklist de Implementação

### Fase 1: Preparação
- [ ] Criar branch `chore/docs-reorganization`
- [ ] Documentar estrutura atual em `docs/STRUCTURE.md`
- [ ] Revisar todas as referências cross-file

### Fase 2: Limpeza
- [ ] Remover 25+ arquivos temporários
- [ ] Commitar: `chore(docs): Remove temporary PR/session files`

### Fase 3: Reorganização
- [ ] Mover arquivos para `/docs`
- [ ] Mover `discord-setup-guide.md` para `/scripts`
- [ ] Criar arquivos novos (DEVELOPMENT.md, SETUP.md, etc)
- [ ] Commitar: `chore(docs): Centralize documentation structure`

### Fase 4: Validação
- [ ] Executar build/tests
- [ ] Verificar wiki frontend
- [ ] Validar todos os links internos
- [ ] Commitar: `chore(docs): Validate reorganized structure`

### Fase 5: Merge
- [ ] Review PR
- [ ] Merge para main
- [ ] Criar PR para documentar mudança

## 📊 Benefícios

✅ **Organização:** Raiz limpa, documentação centralizada
✅ **Manutenibilidade:** Estrutura clara e lógica
✅ **Escalabilidade:** Fácil adicionar novos documentos
✅ **Frontend Wiki:** URLs consistentes e previsíveis
✅ **CI/CD:** Menos arquivos na raiz para verificar

## 🚀 Próximos Passos

1. Confirmar se plano está de acordo
2. Iniciar implementação (Fase 1)
3. Validar impacto no frontend wiki
4. Executar mudanças
5. Documentar resultado em `docs/STRUCTURE.md`
