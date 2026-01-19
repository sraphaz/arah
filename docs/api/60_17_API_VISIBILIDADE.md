# Regras de Visibilidade e Permissões - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🔒 Regras de Visibilidade e Permissões

### Visibilidade de Conteúdo

**PUBLIC (Público)**:
- Visível para todos usuários autenticados
- Visitantes (VISITOR) podem ver
- Moradores (RESIDENT) podem ver

**RESIDENTS_ONLY (Apenas Moradores)**:
- Visível apenas para moradores verificados (RESIDENT + `ResidencyVerification != NONE`)
- Visitantes não veem
- Moradores não verificados (RESIDENT + `ResidencyVerification = NONE`) não veem

### Permissões por Role

**VISITOR (Visitante)**:
- ✅ Ver posts públicos
- ✅ Ver eventos públicos
- ✅ Ver entidades públicas do mapa
- ✅ Criar eventos
- ✅ Reportar alertas
- ✅ Sugerir entidades
- ✅ Reportar posts/usuários
- ✅ Bloquear usuários
- ✅ Criar solicitações de entrada
- ❌ Ver conteúdo RESIDENTS_ONLY
- ❌ Comentar posts
- ❌ Compartilhar posts
- ❌ Criar stores/items
- ❌ Criar assets
- ❌ Relacionar-se com entidades

**RESIDENT (não verificado)**:
- ✅ Todas permissões de VISITOR
- ❌ Ver conteúdo RESIDENTS_ONLY
- ❌ Criar stores/items
- ❌ Criar assets
- ❌ Relacionar-se com entidades

**RESIDENT (verificado)**:
- ✅ Todas permissões de VISITOR
- ✅ Ver conteúdo RESIDENTS_ONLY
- ✅ Comentar posts
- ✅ Compartilhar posts
- ✅ Criar stores/items
- ✅ Criar assets
- ✅ Relacionar-se com entidades

**CURATOR (Curador)**:
- ✅ Todas permissões de RESIDENT (verificado)
- ✅ Validar entidades
- ✅ Validar alertas
- ✅ Validar assets
- ✅ Listar reports
- ✅ Atualizar feature flags
- ✅ Aprovar/rejeitar join requests

### Sanções

**PostingRestriction (Restrição de Postagem)**:
- Usuário não pode criar posts no território
- Usuário não pode criar eventos
- Usuário não pode criar alertas

**Scope (Escopo de Sanção)**:
- **TERRITORY**: Sanção aplicada apenas ao território específico
- **GLOBAL**: Sanção aplicada a todos os territórios

**Duração**:
- Sanções podem ter data de início e fim
- Sanções ativas são verificadas automaticamente

---

## 📚 Documentação Relacionada

- **[Vínculos e Membros](./60_03_API_MEMBERSHIPS.md)** - Roles e verificação de residência
- **[Moderação](./60_12_API_MODERACAO.md)** - Sistema de reports e sanções
- **[Feed Comunitário](./60_04_API_FEED.md)** - Visibilidade de posts
- **[Mapa Territorial](./60_06_API_MAPA.md)** - Visibilidade de entidades

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
