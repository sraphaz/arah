# Estrutura da Documentação

## Organização de Arquivos

A documentação está centralizada em `/docs/` com a seguinte estrutura:

### Raiz do Projeto

```
/
├── README.md                 # Documentação principal
├── CONTRIBUTING.md           # Guia de contribuição
├── CODE_OF_CONDUCT.md        # Código de conduta
├── SECURITY.md               # Política de segurança
├── LICENSE                   # Licença MIT
└── LICENSE.pt-BR             # Licença em português
```

### Documentação Centralizada

```
/docs/
├── README.md                 # Index da documentação
├── DEVELOPMENT.md            # Guia para desenvolvedores
├── SETUP.md                  # Guia de instalação
├── API.md                    # Documentação de API
├── ARCHITECTURE.md           # Arquitetura do sistema
├── CHANGELOG.md              # Histórico de versões
│
├── backlog-api/              # Backlog de features
│   ├── FASE1.md
│   ├── FASE2.md
│   ├── ...
│   └── FASE14_5.md
│
├── COMMUNITY_MODERATION.md   # Políticas de moderação
├── GOVERNANCE_SYSTEM.md      # Sistema de governança
├── VOTING_SYSTEM.md          # Sistema de votação
│
├── ONBOARDING_PUBLICO/       # Onboarding públicos
├── ONBOARDING_DEVELOPERS/    # Onboarding para desenvolvedores
├── ONBOARDING_ANALISTAS_FUNCIONAIS/
│
├── 00_INDEX/                 # Índice interativo
├── 01_PRODUCT_VISION/        # Visão do produto
├── 02_ROADMAP/               # Roadmap
├── 10_ARCHITECTURE_DECISIONS/
├── 11_ARCHITECTURE_SERVICES/
├── 12_DOMAIN_MODEL/
├── 33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md
└── DISCORD_SETUP/            # Configuração Discord
```

### Scripts

```
/scripts/
├── discord-setup-guide.md    # Guia de setup Discord
├── discord-setup.js
├── check-design-compliance.sh
└── ... (utilitários)
```

### Design

```
/design/
├── Architecture/             # Diagramas C4
├── Backlog/                  # Backlog de design
└── Wireframes/               # Wireframes
```

## Regras de Criação de Documentação

✅ **FAZER:**
- Documentação de features → `/docs/`
- Guias de onboarding → `/docs/ONBOARDING_*/`
- Roadmap e backlog → `/docs/backlog-api/`
- Scripts com docs → `/scripts/`
- Diagramas e design → `/design/`

❌ **NÃO FAZER:**
- Arquivos temporários na raiz
- Múltiplas versões de PR body
- Documentação espalhada em vários lugares
- PR descriptions fora de `/docs/`

## Referências no Wiki

O wiki frontend (`/frontend/wiki`) busca documentação automaticamente em `/docs/`.

Links no wiki:
- `/docs/GOVERNANCE_SYSTEM.md` → `/wiki/docs/governance_system`
- `/docs/backlog-api/FASE14_5.md` → `/wiki/docs/backlog-api/fase14_5`

## Atualizando Documentação

1. Verifique se a documentação já existe em `/docs/`
2. Se criar nova, coloque em `/docs/`
3. Se for script, coloque em `/scripts/`
4. Se for design, coloque em `/design/`
5. **Nunca** crie arquivos temporários na raiz

## Ver Também

- [DOCUMENTATION_REORGANIZATION_PLAN.md](../DOCUMENTATION_REORGANIZATION_PLAN.md)
- [WIKI_COMPATIBILITY_GUARANTEE.md](../WIKI_COMPATIBILITY_GUARANTEE.md)
