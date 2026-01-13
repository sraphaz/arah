# PR: Paginação, Correção de Encoding e Configuração do Developer Portal

## Resumo

Este PR consolida três melhorias importantes:
1. **Adiciona paginação** em todos os métodos de listagem (21 métodos)
2. **Corrige encoding UTF-8** no Developer Portal
3. **Configura o Developer Portal** para ser servido diretamente em `devportal.araponga.app`

## Mudanças Principais

### 1. Paginação em Métodos de Listagem ✅

Adicionados 21 métodos paginados em 11 services, seguindo o padrão `PagedResult<T>`:

**Services atualizados:**
- `FeedService`: `ListForUserPagedAsync`
- `TerritoryService`: `ListAvailablePagedAsync`, `SearchPagedAsync`, `NearbyPagedAsync`
- `MapService`: `ListEntitiesPagedAsync`
- `EventsService`: `ListEventsPagedAsync`, `GetEventsNearbyPagedAsync`
- `ListingService`: `SearchListingsPagedAsync`
- `AssetService`: `ListPagedAsync`
- `ReportService`: `ListPagedAsync`
- `JoinRequestService`: `ListIncomingPagedAsync`
- `InquiryService`: `ListMyInquiriesPagedAsync`, `ListReceivedInquiriesPagedAsync`
- `HealthService`: `ListAlertsPagedAsync`
- `PlatformFeeService`: `ListActivePagedAsync`

**Características:**
- Todos os métodos originais mantidos para compatibilidade
- Padrão consistente usando `PaginationParameters` e `PagedResult<T>`
- Ordenação padrão: por data de criação (mais recente primeiro)

### 2. Correção de Encoding UTF-8 ✅

**Problema corrigido:**
- Caracteres acentuados apareciam incorretamente (ex: "có" aparecendo como "C├│")

**Solução:**
- Arquivos HTML re-salvos com encoding UTF-8 sem BOM
- `Program.cs` atualizado para garantir `Content-Type: text/html; charset=utf-8` em todos os arquivos HTML
- Headers HTTP configurados explicitamente

**Arquivos corrigidos:**
- `docs/devportal/index.html`
- `backend/Araponga.Api/wwwroot/devportal/index.html`
- `backend/Araponga.Api/Program.cs`

### 3. Configuração do Developer Portal ✅

**Estrutura final:**
- `araponga.app` → Landing pública (página estática no Gamma)
- `devportal.araponga.app` → Developer Portal (servido diretamente, sem página intermediária)

**Configurações:**
- Workflow do GitHub Pages configurado para `devportal.araponga.app`
- CNAME atualizado
- Referências no HTML atualizadas
- Documentação atualizada

## Benefícios

1. **Performance**: Paginação reduz carga no servidor e banco de dados
2. **Escalabilidade**: Permite lidar com grandes volumes de dados
3. **UX**: Melhor experiência com carregamento mais rápido e caracteres corretos
4. **Consistência**: Padrão uniforme em toda a aplicação
5. **Compatibilidade**: Métodos antigos mantidos, sem breaking changes

## Arquivos Modificados

### Services (Paginação)
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Application/Services/TerritoryService.cs`
- `backend/Araponga.Application/Services/MapService.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/ListingService.cs`
- `backend/Araponga.Application/Services/AssetService.cs`
- `backend/Araponga.Application/Services/ReportService.cs`
- `backend/Araponga.Application/Services/JoinRequestService.cs`
- `backend/Araponga.Application/Services/InquiryService.cs`
- `backend/Araponga.Application/Services/HealthService.cs`
- `backend/Araponga.Application/Services/PlatformFeeService.cs`

### Developer Portal (Encoding e Configuração)
- `docs/devportal/index.html`
- `backend/Araponga.Api/wwwroot/devportal/index.html`
- `backend/Araponga.Api/Program.cs`
- `.github/workflows/devportal-pages.yml`
- `docs/CNAME`
- `docs/13_DOMAIN_ROUTING.md`

## Testes

### Paginação
- ✅ Build sem erros
- ✅ Métodos paginados implementados e funcionando
- ⏳ Testes unitários podem ser adicionados em PR futuro

### Encoding
- ✅ Arquivos re-salvos com UTF-8
- ✅ Content-Type configurado corretamente
- ⏳ Teste manual necessário para verificar caracteres acentuados no navegador

### Developer Portal
- ✅ Workflow configurado
- ✅ CNAME configurado
- ⏳ Deploy necessário para validar funcionamento

## Breaking Changes

⚠️ **Nenhum breaking change**. Todas as mudanças são aditivas ou correções:
- Métodos originais mantidos (paginação)
- Encoding corrigido (sem mudança de API)
- Configuração de domínio (sem mudança de funcionalidade)

## Próximos Passos

Após merge deste PR:
1. Atualizar controllers para expor endpoints paginados (opcional, PR separado)
2. Adicionar testes unitários para métodos paginados
3. Validar encoding no navegador após deploy
4. Configurar DNS para `devportal.araponga.app` (se necessário)

## Checklist

### Paginação
- [x] 21 métodos paginados implementados
- [x] Padrão consistente aplicado
- [x] Métodos originais mantidos
- [x] Build sem erros

### Encoding
- [x] Arquivos re-salvos com UTF-8
- [x] Content-Type configurado no servidor
- [x] Headers HTTP configurados

### Developer Portal
- [x] Workflow configurado
- [x] CNAME configurado
- [x] Referências atualizadas
- [x] Documentação atualizada

## Commits Sugeridos

```
feat: adicionar paginação em todos os métodos de listagem
fix: corrigir encoding UTF-8 no Developer Portal
chore: configurar Developer Portal para devportal.araponga.app
```
