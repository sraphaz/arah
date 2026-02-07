# Regras de Documentação para Agentes Cursor - Arah

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: ⚠️ OBRIGATÓRIO

---

## 🚨 REGRA FUNDAMENTAL

**TODA mudança no código DEVE ser acompanhada de atualização na documentação correspondente.**

Esta é uma regra **NÃO NEGOCIÁVEL**. Documentação desatualizada é considerado um bug crítico.

### Auto-Aprendizado Após Revisões

**⚠️ OBRIGATÓRIO**: Após qualquer revisão técnica (design, código, arquitetura), seguir o processo de auto-aprendizado:

1. **Capturar Lições**: Documentar em `docs/LICOES_APRENDIDAS.md`
2. **Atualizar Diretrizes**: Aplicar lições aprendidas nas diretrizes relevantes
3. **Validar**: Garantir que mudanças estão corretas e completas

**Processo Completo**: Ver `docs/PROCESSO_AUTO_APRENDIZADO_REVISOES.md`

---

## 📋 Mapeamento: Mudança de Código → Documentação

### Mudanças em Controllers/Endpoints

**Arquivos a Atualizar:**
- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - Seção correspondente
- ✅ `frontend/devportal/index.html` - Se mudar DevPortal
- ✅ `docs/CHANGELOG.md` - Se mudança significativa

**Exemplo:**
```markdown
Se você adicionar `POST /api/v1/feed/posts/{id}/reactions`:
1. Atualizar docs/60_API_LÓGICA_NEGÓCIO.md → Seção "Feed Comunitário"
2. Adicionar exemplo em docs/60_API_LÓGICA_NEGÓCIO.md
3. Atualizar docs/CHANGELOG.md com nova funcionalidade
```

### Mudanças em Services/Application

**Arquivos a Atualizar:**
- ✅ `docs/11_ARCHITECTURE_SERVICES.md` - Se mudar service
- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - Se mudar lógica de negócio
- ✅ `docs/22_COHESION_AND_TESTS.md` - Se implementar nova funcionalidade

### Mudanças em Domain/Entities

**Arquivos a Atualizar:**
- ✅ `docs/12_DOMAIN_MODEL.md` - Atualizar entidades
- ✅ `docs/10_ARCHITECTURE_DECISIONS.md` - Se for decisão arquitetural
- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - Se afetar contratos de API

### Mudanças em Infrastructure

**Arquivos a Atualizar:**
- ✅ `docs/10_ARCHITECTURE_DECISIONS.md` - Se mudar decisão de infraestrutura
- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - Se afetar comportamento da API
- ✅ Documentação específica (ex: `docs/MEDIA_SYSTEM.md` para mudanças em mídia)

### Completar Fase do Backlog

**Arquivos a Atualizar:**
- ✅ `docs/backlog-api/FASE*.md` - Marcar como completo
- ✅ `docs/backlog-api/README.md` - Atualizar status
- ✅ `docs/backlog-api/implementacoes/FASE*_IMPLEMENTACAO_RESUMO.md` - Criar resumo
- ✅ `docs/CHANGELOG.md` - Adicionar entrada da fase
- ✅ `docs/STATUS_FASES.md` - Se existir, atualizar status
- ✅ `docs/02_ROADMAP.md` - Se mudar roadmap

### Mudanças em Segurança

**Arquivos a Atualizar:**
- ✅ `docs/SECURITY_CONFIGURATION.md`
- ✅ `docs/SECURITY_AUDIT.md`
- ✅ `docs/CHANGELOG.md`

### Mudanças em Configuração

**Arquivos a Atualizar:**
- ✅ `docs/SECURITY_CONFIGURATION.md` - Se mudar configurações de segurança
- ✅ `README.md` - Se mudar configurações principais
- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - Se afetar comportamento

---

## ✅ Checklist Automático para Cada PR

Antes de criar qualquer PR, você DEVE verificar:

### Checklist de Documentação

- [ ] **Identifiquei todas as mudanças no código**
- [ ] **Identifiquei quais documentos precisam ser atualizados** (usar mapeamento acima)
- [ ] **Atualizei TODOS os documentos identificados**
- [ ] **Atualizei exemplos de código se mudar contratos**
- [ ] **Atualizei Changelog se mudança for significativa**
- [ ] **Verifiquei que links em documentação funcionam**
- [ ] **Verifiquei que informações não conflitam entre documentos**
- [ ] **Mencionei atualização de documentação no PR**

### Template de Descrição de PR

```markdown
## Mudanças

[Descrição das mudanças]

## Documentação Atualizada

- ✅ `docs/60_API_LÓGICA_NEGÓCIO.md` - [O que foi atualizado]
- ✅ `docs/CHANGELOG.md` - [O que foi atualizado]
- ✅ [outros arquivos] - [O que foi atualizado]

## Testes

[Descrição dos testes]
```

---

## 🔍 Como Identificar Documentos a Atualizar

### Passo 1: Identificar Tipo de Mudança
- [ ] Nova funcionalidade/feature?
- [ ] Mudança em API/endpoint?
- [ ] Mudança em modelo de domínio?
- [ ] Mudança em arquitetura?
- [ ] Completar fase do backlog?
- [ ] Mudança em segurança/configuração?

### Passo 2: Consultar Mapeamento
Use a seção "Mapeamento: Mudança de Código → Documentação" acima.

### Passo 3: Buscar Referências
Use `grep` ou busca no código para encontrar documentos que mencionam a funcionalidade:
```bash
# Exemplo: se mudar Feed
grep -r "feed\|Feed" docs/ --include="*.md"
```

### Passo 4: Atualizar Todos os Documentos Encontrados

---

## 📝 Template de Atualização de Documentação

### Para Nova Funcionalidade

```markdown
## [Nome da Funcionalidade]

**Status**: ✅ Implementado  
**Data**: YYYY-MM-DD

### Descrição
[Descrição da funcionalidade]

### Endpoints
- `POST /api/v1/...` - [Descrição]
- `GET /api/v1/...` - [Descrição]

### Exemplo
```json
{
  "exemplo": "de request"
}
```

### Regras de Negócio
- [Regra 1]
- [Regra 2]
```

### Para Atualizar Changelog

```markdown
## [YYYY-MM-DD] - [Título]

### [Categoria]
- ✅ [Descrição da mudança]
  - Detalhes adicionais
  - Arquivos modificados

### Documentação
- ✅ Atualizado `docs/60_API_LÓGICA_NEGÓCIO.md`
- ✅ Atualizado `docs/CHANGELOG.md`
```

---

## 🚫 O Que NÃO Fazer

- ❌ **NUNCA** commitar mudanças de código sem atualizar documentação
- ❌ **NUNCA** assumir que documentação está atualizada
- ❌ **NUNCA** deixar documentação para "depois"
- ❌ **NUNCA** atualizar apenas parte da documentação relacionada

---

## ✅ O Que SEMPRE Fazer

- ✅ **SEMPRE** atualizar documentação junto com código
- ✅ **SEMPRE** verificar quais documentos precisam atualização
- ✅ **SEMPRE** atualizar todos os documentos relacionados
- ✅ **SEMPRE** mencionar atualização de documentação no PR
- ✅ **SEMPRE** verificar links após atualizar documentação

---

## 🔄 Sincronização com Wiki

Se você atualizar documentos que são sincronizados com Wiki do GitHub:

1. **Mencionar no PR** que documentação foi atualizada
2. **Executar script de sincronização** após merge:
   ```powershell
   cd docs/backlog-api
   .\script-sync-wiki.ps1
   ```

Documentos sincronizados:
- Todos os documentos em `docs/backlog-api/`
- Documentos principais em `docs/` (00-70)
- DevPortal em `frontend/devportal/`

---

## 📚 Referências Rápidas

### Documentos Principais
- `docs/60_API_LÓGICA_NEGÓCIO.md` - Lógica de negócio e APIs
- `docs/12_DOMAIN_MODEL.md` - Modelo de domínio
- `docs/10_ARCHITECTURE_DECISIONS.md` - Decisões arquiteturais
- `docs/22_COHESION_AND_TESTS.md` - Status de implementação
- `docs/CHANGELOG.md` - Histórico de mudanças
- `docs/backlog-api/README.md` - Backlog completo (número de fases calculado dinamicamente - ver `PROJECT_PHASES_CONFIG.md`)

### Estrutura de Documentação
- `docs/00_INDEX.md` - Índice completo
- `docs/README.md` - Visão geral
- `docs/STATUS_FASES.md` - Status das fases (número calculado dinamicamente - ver `PROJECT_PHASES_CONFIG.md`)

---

## 🎯 Lembrete Final

**Documentação desatualizada é um bug crítico.**

Se você não tem certeza de qual documento atualizar:
1. Busque referências à funcionalidade nos documentos
2. Atualize TODOS os documentos que mencionam a funcionalidade
3. Quando em dúvida, atualize mais do que menos

**É melhor atualizar documentação desnecessariamente do que deixar desatualizada.**
