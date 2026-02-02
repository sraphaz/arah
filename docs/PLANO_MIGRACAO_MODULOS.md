# Plano de Migração de Repositórios para Módulos

**Data**: 2026-01-28  
**Status**: ✅ Concluído (registros dos módulos ativos; API usa apenas módulos)  
**Tipo**: Documentação Técnica - Migração

---

## 📋 Estratégia de Migração

A migração será feita **gradualmente**, módulo por módulo, seguindo o padrão já estabelecido pelo **FeedModule**.

### Padrão a Seguir (Baseado em FeedModule)

1. **Criar estrutura de diretórios no módulo**:
   ```
   Araponga.Modules.Xxx.Infrastructure/
   ├── Postgres/
   │   ├── Entities/
   │   │   └── XxxRecord.cs (mover de Infrastructure.Postgres.Entities)
   │   ├── XxxDbContext.cs (novo)
   │   ├── XxxMappers.cs (se necessário)
   │   └── PostgresXxxRepository.cs (mover de Infrastructure.Postgres)
   └── XxxModule.cs (atualizar)
   ```

2. **Criar DbContext do módulo**:
   - Herdar de `DbContext`
   - Configurar `DbSet<T>` para cada entidade do módulo
   - Configurar `OnModelCreating` com mapeamentos

3. **Mover repositórios**:
   - Copiar arquivos de `Araponga.Infrastructure.Postgres` para o módulo
   - Atualizar namespace
   - Atualizar dependência de `ArapongaDbContext` para `XxxDbContext`
   - Atualizar referências de entidades

4. **Mover entidades**:
   - Copiar arquivos de `Araponga.Infrastructure.Postgres.Entities` para o módulo
   - Atualizar namespace
   - Verificar dependências (algumas podem precisar ficar em Shared)

5. **Atualizar módulo**:
   - Registrar `XxxDbContext` no `RegisterServices`
   - Registrar repositórios do módulo
   - Remover registros de `AddPostgresRepositories` (ou comentar temporariamente)

6. **Atualizar referências**:
   - Atualizar `Araponga.Api.csproj` se necessário
   - Atualizar outros projetos que referenciam os repositórios

7. **Testar**:
   - Build deve passar
   - Testes devem passar
   - Funcionalidade deve continuar funcionando

---

## 🎯 Ordem de Migração Recomendada

### Fase 1: Módulos Simples (Alta Prioridade)

#### 1. Chat Module (4 repositórios)
**Complexidade**: Média  
**Dependências**: Baixas (apenas ChatConversation, ChatMessage, etc.)

**Entidades a mover**:
- `ChatConversationRecord`
- `ChatConversationParticipantRecord`
- `ChatMessageRecord`
- `ChatConversationStatsRecord`

**Repositórios a mover**:
- `PostgresChatConversationRepository`
- `PostgresChatConversationParticipantRepository`
- `PostgresChatMessageRepository`
- `PostgresChatConversationStatsRepository`

**Passos**:
1. Criar `ChatDbContext` com DbSets para as 4 entidades
2. Mover entidades para `Araponga.Modules.Chat.Infrastructure.Postgres.Entities`
3. Mover repositórios para `Araponga.Modules.Chat.Infrastructure.Postgres`
4. Atualizar repositórios para usar `ChatDbContext`
5. Atualizar `ChatModule.RegisterServices`
6. Remover registros de `AddPostgresRepositories`

#### 2. Events Module (2 repositórios)
**Complexidade**: Média  
**Dependências**: Médias (pode depender de Territories)

**Entidades a mover**:
- `TerritoryEventRecord` (verificar se já existe)
- `EventParticipationRecord` (verificar se já existe)

**Repositórios a mover**:
- `PostgresTerritoryEventRepository`
- `PostgresEventParticipationRepository`

#### 3. Map Module (2 repositórios)
**Complexidade**: Baixa  
**Dependências**: Baixas

**Entidades a mover**:
- `MapEntityRecord` (verificar se já existe)
- `MapEntityRelationRecord` (verificar se já existe)

**Repositórios a mover**:
- `PostgresMapRepository`
- `PostgresMapEntityRelationRepository`

### Fase 2: Módulos Médias (Média Prioridade)

#### 4. Alerts Module (1 repositório)
**Complexidade**: Baixa  
**Dependências**: Baixas

#### 5. Assets Module (3 repositórios)
**Complexidade**: Média  
**Dependências**: Médias

#### 6. Notifications Module (2 repositórios)
**Complexidade**: Média  
**Dependências**: Médias

### Fase 3: Módulos Complexos (Baixa Prioridade)

#### 7. Subscriptions Module (6 repositórios)
**Complexidade**: Alta  
**Dependências**: Altas (Financial, Payments)

#### 8. Moderation Module (5 repositórios)
**Complexidade**: Alta  
**Dependências**: Altas (WorkItems, Reports, etc.)

#### 9. Marketplace Module (12+ repositórios)
**Complexidade**: Muito Alta  
**Dependências**: Altas (Financial, Payments, etc.)

---

## ⚠️ Considerações Importantes

### Entidades Compartilhadas

Algumas entidades podem ser compartilhadas entre módulos ou com Shared:
- **UserRecord**, **TerritoryRecord**: Devem ficar em Shared
- **MediaAssetRecord**: Pode ficar em Shared (cross-cutting)
- **NotificationConfigRecord**: Pode ficar em Shared ou Notifications

### Dependências entre Módulos

- **Chat** pode depender de **Territories** (Shared)
- **Events** pode depender de **Territories** e **Feed**
- **Map** pode depender de **Feed** e **Events**

### ArapongaDbContext

Durante a migração, `ArapongaDbContext` ainda será necessário para:
- Entidades que ainda não foram migradas
- Compatibilidade temporária
- Repositórios que dependem de múltiplos módulos

Após a migração completa, `ArapongaDbContext` pode ser removido ou mantido apenas para compatibilidade.

---

## 📝 Checklist por Módulo

Para cada módulo, verificar:

- [ ] Estrutura de diretórios criada
- [ ] DbContext criado e configurado
- [ ] Entidades movidas/copiadas
- [ ] Repositórios movidos e atualizados
- [ ] Módulo atualizado para registrar DbContext e repositórios
- [ ] Registros removidos de `AddPostgresRepositories`
- [ ] Build passa
- [ ] Testes passam
- [ ] Funcionalidade testada manualmente

---

## 🚀 Próximos Passos Imediatos

1. **Criar estrutura base do ChatModule** (ChatDbContext + diretórios)
2. **Mover primeira entidade** (ChatConversationRecord) como prova de conceito
3. **Mover primeiro repositório** (PostgresChatConversationRepository)
4. **Testar build e funcionalidade**
5. **Repetir para demais repositórios do Chat**

---

**Última atualização**: 2026-02-02. Em 02/02/2026 foi removido o registro duplicado de `IFeedRepository` em `AddPostgresRepositories`; a API usa exclusivamente os módulos para todos os domínios migrados.
