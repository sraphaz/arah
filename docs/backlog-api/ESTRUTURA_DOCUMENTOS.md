# Estrutura de Documentos - Backlog API

**Data**: 2025-01-13  
**Objetivo**: Documentar a estrutura organizada dos documentos

---

## 📁 Estrutura de Pastas

```
docs/
├── backlog-api/               # Backlog API (organizado)
│   ├── README.md              # Índice principal
│   ├── FASE1.md até FASE29.md # Documentos de fases (29 fases)
│   ├── RESUMO_*.md            # Resumos executivos
│   ├── REORGANIZACAO_*.md     # Documentos de reorganização
│   └── ROADMAP_*.md           # Roadmaps visuais
│
├── prs/                       # Pull Requests e mudanças
├── refactoring/               # Refatorações
├── recommendations/            # Recomendações
├── validation/                # Validações de segurança
└── [outros documentos gerais] # Documentação geral do projeto
```

---

## 📋 Documentos na Raiz docs/

### Documentos Gerais (Mantidos na Raiz)
- `00_INDEX.md` - Índice geral
- `01_PRODUCT_VISION.md` - Visão do produto
- `02_ROADMAP.md` - Roadmap geral
- `03_BACKLOG.md` - Backlog geral
- `README.md` - README principal

### Documentos Técnicos (Mantidos na Raiz)
- `10_ARCHITECTURE_DECISIONS.md`
- `11_ARCHITECTURE_SERVICES.md`
- `12_DOMAIN_MODEL.md`
- `20_IMPLEMENTATION_PLAN.md`
- `60_API_LÓGICA_NEGÓCIO.md`

### Documentos de Avaliação (Mantidos na Raiz)
- `50_PRODUCAO_AVALIACAO_COMPLETA.md`
- `70_AVALIACAO_GERAL_APLICACAO.md`
- `AVALIACAO_COMPLETA_APLICACAO.md`

### Documentos de Fases (Movidos para backlog-api/)
- `FASE1_*.md` → `backlog-api/implementacoes/FASE1_*.md`
- `FASE2_*.md` → `backlog-api/implementacoes/FASE2_*.md`
- `TESTES_FASE7_RESUMO.md` → `backlog-api/implementacoes/`

### Documentos de Plano (Movidos para backlog-api/)
- `PLANO_ACAO_10_10.md` → `backlog-api/arquivos-originais/PLANO_ACAO_10_10_ORIGINAL.md`
- `71_PLANO_ACAO_10_10.md` → `backlog-api/arquivos-originais/PLANO_ACAO_10_10_ALTERNATIVO.md`
- `PLANO_ACAO_10_10_RESUMO.md` → `backlog-api/PLANO_ACAO_10_10_RESUMO.md`
- `MAPA_CORRELACAO_FUNCIONALIDADES.md` → `backlog-api/MAPA_CORRELACAO_FUNCIONALIDADES.md`

---

## 📋 Documentos em backlog-api/

### Documentos Principais
- `README.md` - Índice e visão geral
- `ESTRUTURA_DOCUMENTOS.md` - Este arquivo

### Fases (FASE1.md até FASE29.md)
- `FASE1.md` até `FASE29.md` - Documentos de fases (29 fases)

### Resumos e Estratégias
- `RESUMO_EXECUTIVO_ESTRATEGICO.md` - Resumo executivo
- `RESUMO_REORGANIZACAO_FINAL.md` - Resumo da reorganização
- `RESUMO_NOVAS_FASES.md` - Resumo de novas fases
- `RESUMO_REALINHAMENTO.md` - Resumo do realinhamento
- `RESUMO_EXPANSAO_FUNCIONALIDADES.md` - Resumo de expansão

### Reorganizações e Análises
- `REORGANIZACAO_ESTRATEGICA_FINAL.md` - Reorganização estratégica
- `REVISAO_COMPLETA_PRIORIDADES.md` - Revisão de prioridades
- `REALINHAMENTO_ESTRATEGICO_FASES_8_14.md` - Realinhamento
- `ANALISE_IMPACTO_FASES_11_14.md` - Análise de impacto
- `ORGANIZACAO_FASES_11_14.md` - Organização de fases
- `ATUALIZACAO_ORDEM_FASES.md` - Atualização de ordem

### Roadmaps
- `ROADMAP_VISUAL.md` - Roadmap visual

### Implementações (Nova Pasta)
- `implementacoes/` - Documentos de implementação das fases

---

## 🔄 Plano de Normalização

### Passo 1: Criar Estrutura de Pastas
- [x] Criar `backlog-api/implementacoes/`
- [x] Criar `backlog-api/arquivos-originais/`

### Passo 2: Mover Documentos de Fases
- [x] Mover `FASE1_*.md` → `backlog-api/implementacoes/`
- [x] Mover `FASE2_*.md` → `backlog-api/implementacoes/`
- [x] Mover `TESTES_FASE7_RESUMO.md` → `backlog-api/implementacoes/`

### Passo 3: Mover Documentos de Plano
- [x] Mover `PLANO_ACAO_10_10.md` → `backlog-api/arquivos-originais/`
- [x] Mover `71_PLANO_ACAO_10_10.md` → `backlog-api/arquivos-originais/`
- [x] Mover `PLANO_ACAO_10_10_RESUMO.md` → `backlog-api/`
- [x] Mover `MAPA_CORRELACAO_FUNCIONALIDADES.md` → `backlog-api/`

### Passo 4: Normalizar Nomes
- [ ] Garantir que todas as fases seguem padrão `FASE{N}.md`
- [ ] Garantir que todos os resumos seguem padrão `RESUMO_*.md`
- [ ] Garantir que todas as análises seguem padrão `*_ANALISE_*.md` ou `*_IMPACTO_*.md`

---

**Status**: 📋 **ESTRUTURA DEFINIDA**
