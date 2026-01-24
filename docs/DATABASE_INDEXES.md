# Índices de Banco de Dados - Araponga

**Última Atualização**: 2025-01-23  
**Status**: ✅ Implementado

---

## 📋 Resumo

Este documento lista todos os índices de banco de dados criados no PostgreSQL para otimizar a performance das queries mais comuns do Araponga.

---

## 🔍 Índices por Tabela

### `territories`
- **`PK_territories`** (Primary Key em `id`) - Índice primário
- **`IX_territories_status`** - Índice em `status` para filtrar territórios ativos
- **`IX_territories_created_at_utc`** - Índice em `created_at_utc` para ordenação temporal

**Justificativa**: Filtros por status e ordenação por data são operações frequentes.

---

### `users`
- **`PK_users`** (Primary Key em `id`) - Índice primário
- **`IX_users_provider_external_id`** - Índice único composto em `(provider, external_id)` para busca de autenticação social
- **`IX_users_email`** - Índice único em `email` (quando não nulo) para recuperação de senha

**Justificativa**: Busca por provider/external_id é crítica para autenticação. Email é usado para recuperação de senha.

---

### `territory_memberships`
- **`PK_territory_memberships`** (Primary Key em `id`) - Índice primário
- **`IX_territory_memberships_user_territory`** - Índice único composto em `(user_id, territory_id)` para verificação de membership
- **`IX_territory_memberships_territory_id`** - Índice em `territory_id` para listar membros de um território
- **`IX_territory_memberships_user_id`** - Índice em `user_id` para listar territórios de um usuário
- **`IX_territory_memberships_role`** - Índice em `role` para filtrar por papel

**Justificativa**: Verificação de membership é uma operação muito frequente. Listagem por território e usuário também são comuns.

---

### `community_posts`
- **`PK_community_posts`** (Primary Key em `id`) - Índice primário
- **`IX_community_posts_territory_id`** - Índice em `territory_id` para feed por território
- **`IX_community_posts_author_user_id`** - Índice em `author_user_id` para posts do usuário
- **`IX_community_posts_created_at_utc`** - Índice em `created_at_utc` para ordenação temporal
- **`IX_community_posts_status`** - Índice em `status` para filtrar posts publicados
- **`IX_community_posts_type`** - Índice em `type` para filtrar por tipo (NOTICE, ALERT, etc.)
- **Índice composto** `(territory_id, status, created_at_utc DESC)` - Para queries de feed otimizadas

**Justificativa**: Feed é uma das operações mais frequentes. Ordenação por data e filtros por status/tipo são essenciais.

---

### `territory_events`
- **`PK_territory_events`** (Primary Key em `id`) - Índice primário
- **`IX_territory_events_territory_id`** - Índice em `territory_id` para listar eventos do território
- **`IX_territory_events_starts_at_utc`** - Índice em `starts_at_utc` para ordenação e filtros temporais
- **`IX_territory_events_status`** - Índice em `status` para filtrar eventos agendados/cancelados
- **Índice composto** `(territory_id, starts_at_utc, status)` - Para queries de eventos futuros

**Justificativa**: Listagem de eventos por território e data é frequente. Filtros por status são comuns.

---

### `map_entities`
- **`PK_map_entities`** (Primary Key em `id`) - Índice primário
- **`IX_map_entities_territory_id`** - Índice em `territory_id` para entidades do território
- **`IX_map_entities_type`** - Índice em `type` para filtrar por tipo de entidade
- **Índice GIST** em `location` (PostGIS) - Para queries geoespaciais (proximidade, dentro de área)

**Justificativa**: Queries geoespaciais são críticas para o mapa. Filtros por território e tipo são comuns.

---

### `store_items`
- **`PK_store_items`** (Primary Key em `id`) - Índice primário
- **`IX_store_items_store_id`** - Índice em `store_id` para itens da loja
- **`IX_store_items_status`** - Índice em `status` para filtrar itens ativos
- **`IX_store_items_category`** - Índice em `category` para filtros por categoria
- **Índice GIN** em `name` (full-text search) - Para busca textual (quando implementado)

**Justificativa**: Busca e filtros no marketplace são frequentes. Full-text search melhora performance de busca.

---

### `user_notifications`
- **`PK_user_notifications`** (Primary Key em `id`) - Índice primário
- **`IX_user_notifications_user_id`** - Índice em `user_id` para notificações do usuário
- **`IX_user_notifications_read_at_utc`** - Índice em `read_at_utc` para filtrar não lidas
- **Índice composto** `(user_id, read_at_utc, created_at_utc DESC)` - Para inbox otimizado

**Justificativa**: Inbox de notificações é acessado frequentemente. Filtros por não lidas e ordenação são essenciais.

---

### `outbox_messages`
- **`PK_outbox_messages`** (Primary Key em `id`) - Índice primário
- **`IX_outbox_messages_processed_at_utc`** - Índice em `processed_at_utc` para buscar não processadas
- **`IX_outbox_messages_process_after_utc`** - Índice em `process_after_utc` para agendamento
- **Índice composto** `(processed_at_utc, process_after_utc, occurred_at_utc)` - Para worker otimizado

**Justificativa**: Outbox pattern requer queries frequentes de mensagens não processadas.

---

### `email_queue_items`
- **`PK_email_queue_items`** (Primary Key em `id`) - Índice primário
- **`IX_email_queue_items_status`** - Índice em `status` para filtrar pendentes
- **`IX_email_queue_items_scheduled_for_utc`** - Índice em `scheduled_for_utc` para agendamento
- **Índice composto** `(status, scheduled_for_utc, priority)` - Para worker de email otimizado

**Justificativa**: Processamento de fila de emails requer queries frequentes de itens pendentes.

---

## 📊 Estatísticas de Índices

### Total de Índices
- **Índices primários**: ~30 (uma por tabela)
- **Índices secundários**: ~50+
- **Índices compostos**: ~15
- **Índices GIN/GIST**: 2 (full-text search e geoespacial)

### Tabelas com Mais Índices
1. `community_posts` - 7 índices (feed é crítico)
2. `territory_memberships` - 5 índices (verificação de acesso é frequente)
3. `territory_events` - 4 índices (listagem e filtros temporais)
4. `map_entities` - 3 índices + GIST (queries geoespaciais)

---

## 🔧 Manutenção

### Atualizar Estatísticas
```sql
ANALYZE;
```

Execute periodicamente (diariamente via cron) para manter estatísticas atualizadas para o query planner.

### Verificar Índices Não Utilizados
```sql
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_scan as index_scans
FROM pg_stat_user_indexes
WHERE idx_scan = 0
ORDER BY schemaname, tablename;
```

Índices com `idx_scan = 0` podem ser candidatos para remoção (após análise cuidadosa).

### Monitorar Tamanho dos Índices
```sql
SELECT 
    schemaname,
    tablename,
    indexname,
    pg_size_pretty(pg_relation_size(indexrelid)) as index_size
FROM pg_stat_user_indexes
ORDER BY pg_relation_size(indexrelid) DESC
LIMIT 20;
```

---

## ⚠️ Recomendações

1. **Não criar índices desnecessários**: Cada índice adiciona overhead em INSERT/UPDATE/DELETE
2. **Monitorar performance**: Use `EXPLAIN ANALYZE` para validar uso de índices
3. **Atualizar estatísticas**: Execute `ANALYZE` regularmente
4. **Considerar índices parciais**: Para filtros muito específicos (ex: `WHERE status = 'Active'`)
5. **Índices compostos**: Use quando queries filtram por múltiplas colunas

---

## 📚 Referências

- [PostgreSQL Indexes Documentation](https://www.postgresql.org/docs/current/indexes.html)
- [PostgreSQL Index Types](https://www.postgresql.org/docs/current/indexes-types.html)
- [PostGIS Spatial Indexes](https://postgis.net/docs/using_postgis_dbmanagement.html#spatial_indexes)

---

**Nota**: Este documento deve ser atualizado sempre que novos índices forem criados via migrations.
