# PR: Implementar Cache Completo para Performance

## Resumo

Este PR implementa cache em memória (`IMemoryCache`) em todos os pontos críticos da aplicação para reduzir consultas ao banco de dados e melhorar a performance.

## Mudanças Implementadas

### Novos Cache Services

1. **UserBlockCacheService** (`backend/Araponga.Application/Services/UserBlockCacheService.cs`)
   - Cache de bloqueios de usuários (TTL: 15 minutos)
   - Usado em `PostFilterService` e `MapService` para filtrar conteúdo bloqueado
   - Invalidação automática quando bloqueios são criados/removidos

2. **MapEntityCacheService** (`backend/Araponga.Application/Services/MapEntityCacheService.cs`)
   - Cache de entidades do mapa por território (TTL: 10 minutos)
   - Usado em `MapService` para listagens de entidades
   - Invalidação automática quando entidades são criadas/validadas

3. **EventCacheService** (`backend/Araponga.Application/Services/EventCacheService.cs`)
   - Cache de eventos por território com filtros (TTL: 5 minutos)
   - Usado em `EventsService` para listagens de eventos
   - Invalidação automática quando eventos são criados/atualizados

4. **AlertCacheService** (`backend/Araponga.Application/Services/AlertCacheService.cs`)
   - Cache de alertas de saúde por território (TTL: 10 minutos)
   - Usado em `HealthService` para listagens de alertas
   - Invalidação automática quando alertas são criados/validados

### Services Atualizados

- **PostFilterService**: Usa `UserBlockCacheService` para cachear bloqueios
- **MapService**: Usa `MapEntityCacheService` e `UserBlockCacheService`
- **EventsService**: Usa `EventCacheService` para cachear eventos
- **HealthService**: Usa `AlertCacheService` para cachear alertas
- **UserBlockService**: Invalida cache quando bloqueios mudam

### Cache Já Implementado (PRs Anteriores)

- **TerritoryCacheService**: Territórios ativos (TTL: 30 minutos)
- **FeatureFlagCacheService**: Feature flags (TTL: 15 minutos)
- **AccessEvaluator**: Membership status (TTL: 10 minutos)

## Estratégia de Cache

### TTL (Time To Live)
- **Territories**: 30 minutos (dados mudam raramente)
- **Feature Flags**: 15 minutos (configurações estáveis)
- **User Blocks**: 15 minutos (mudanças frequentes)
- **Map Entities**: 10 minutos (mudanças moderadas)
- **Alerts**: 10 minutos (mudanças moderadas)
- **Events**: 5 minutos (dados mais dinâmicos)
- **Membership**: 10 minutos (mudanças moderadas)

### Invalidação de Cache
- Cache é invalidado automaticamente quando dados são criados/atualizados
- Invalidação é feita nos services que modificam os dados
- Padrão: cache opcional (nullable) para manter compatibilidade com testes

## Arquivos Modificados

### Novos Arquivos
- `backend/Araponga.Application/Services/UserBlockCacheService.cs`
- `backend/Araponga.Application/Services/MapEntityCacheService.cs`
- `backend/Araponga.Application/Services/EventCacheService.cs`
- `backend/Araponga.Application/Services/AlertCacheService.cs`

### Arquivos Modificados
- `backend/Araponga.Application/Services/PostFilterService.cs`
- `backend/Araponga.Application/Services/MapService.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/HealthService.cs`
- `backend/Araponga.Application/Services/UserBlockService.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`

## Testes

- ✅ Build sem erros
- ✅ 121 testes passando
- ✅ Cache é opcional (nullable) para manter compatibilidade com testes existentes

## Impacto Esperado

### Performance
- **Redução de consultas ao banco**: ~70-80% em endpoints de listagem
- **Latência reduzida**: Respostas mais rápidas para dados em cache
- **Menor carga no banco**: Especialmente em picos de tráfego

### Pontos de Cache Implementados
1. Territórios ativos
2. Feature flags
3. Membership status
4. User blocks
5. Map entities
6. Events
7. Alerts

## Próximos Passos (Futuro)

- [ ] Monitorar hit rate do cache em produção
- [ ] Ajustar TTLs baseado em métricas reais
- [ ] Considerar cache distribuído (Redis) para ambientes multi-instância
- [ ] Adicionar métricas de cache (hit/miss rates)

## Checklist

- [x] Cache implementado em todos os pontos críticos
- [x] Invalidação de cache quando dados mudam
- [x] Cache opcional para compatibilidade com testes
- [x] Build sem erros
- [x] Todos os testes passando
- [x] Documentação atualizada
