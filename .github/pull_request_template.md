## 🤖 Checklist de agente (se PR operado por agente)

- [ ] Corpo do PR segue [.agents/templates/pr-body.md](.agents/templates/pr-body.md)
- [ ] Agente e skills listados
- [ ] Escopo respeita manifest em `.agents/`
- [ ] `scripts/agents/validate-manifests.ps1` passou (se alterou `.agents/` ou `.skills/`)

## 📋 Descrição

[Descreva as mudanças realizadas]

## 🔄 Tipo de Mudança

- [ ] Nova funcionalidade
- [ ] Correção de bug
- [ ] Refatoração
- [ ] Mudança em documentação
- [ ] Mudança em configuração
- [ ] Outro: [descreva]

## ✅ Checklist de Documentação

**⚠️ OBRIGATÓRIO**: Marque todos os itens que se aplicam às suas mudanças

### Documentação Técnica
- [ ] Atualizei `docs/60_API_LÓGICA_NEGÓCIO.md` (se mudou lógica de negócio/API)
- [ ] Atualizei `docs/12_DOMAIN_MODEL.md` (se mudou modelo de domínio)
- [ ] Atualizei `docs/10_ARCHITECTURE_DECISIONS.md` (se tomou decisão arquitetural)
- [ ] Atualizei `docs/11_ARCHITECTURE_SERVICES.md` (se mudou services)
- [ ] Atualizei `docs/22_COHESION_AND_TESTS.md` (se implementou nova funcionalidade)

### Documentação de Produto
- [ ] Atualizei `README.md` (se mudou funcionalidades principais)
- [ ] Atualizei `docs/02_ROADMAP.md` (se mudou roadmap)
- [ ] Atualizei `docs/03_BACKLOG.md` (se completou epic/feature)
- [ ] Atualizei `docs/01_PRODUCT_VISION.md` (se mudou visão)

### Histórico e Changelog
- [ ] Atualizei `docs/40_CHANGELOG.md` (se mudança significativa)

### Backlog e Fases
- [ ] Atualizei `docs/backlog-api/FASE*.md` (se completou fase)
- [ ] Atualizei `docs/backlog-api/README.md` (se mudou status de fase)
- [ ] Criei `docs/backlog-api/implementacoes/FASE*_IMPLEMENTACAO_RESUMO.md` (se completou fase)
- [ ] Atualizei `docs/STATUS_FASES.md` (se mudou status de fase)

### DevPortal
- [ ] Atualizei `backend/Arah.Api/wwwroot/devportal/index.html` (se mudou DevPortal)
- [ ] Atualizei exemplos de código (se mudou contratos)

### Segurança
- [ ] Atualizei `docs/SECURITY_CONFIGURATION.md` (se mudou configurações de segurança)
- [ ] Atualizei `docs/SECURITY_AUDIT.md` (se adicionou medidas de segurança)

### Validação
- [ ] Verifiquei que todos os links em documentação funcionam
- [ ] Verifiquei que exemplos de código estão atualizados
- [ ] Verifiquei que informações não conflitam entre documentos

## 📝 Lista de Documentos Atualizados

Liste todos os arquivos de documentação que você atualizou:

- `docs/...` - [O que foi atualizado]
- `docs/...` - [O que foi atualizado]

## 🧪 Testes

- [ ] Testes unitários adicionados/atualizados
- [ ] Testes de integração adicionados/atualizados
- [ ] Testes passando localmente
- [ ] Cobertura de testes mantida/ampliada

## 🔗 Links Relacionados

- Issue: #[número]
- Documentação relacionada: [links]
- PRs relacionados: #[números]

## 📸 Screenshots (se aplicável)

[Adicione screenshots se mudanças afetam UI/UX]

## ⚠️ Breaking Changes

- [ ] Esta mudança quebra compatibilidade com versões anteriores
- [ ] Documentei breaking changes em `docs/40_CHANGELOG.md`
- [ ] Adicionei guia de migração (se necessário)

## 🔄 Sincronização Wiki

- [ ] Documentação atualizada será sincronizada com Wiki do GitHub
- [ ] Executei `docs/backlog-api/script-sync-wiki.ps1` (se mudou fases ou documentos principais)

---

**⚠️ Lembrete**: Documentação desatualizada é considerado um bug crítico. Certifique-se de atualizar TODA a documentação relacionada às suas mudanças.
