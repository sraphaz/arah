# PR: Adicionar Paginação em Métodos de Listagem

## Resumo

Este PR adiciona suporte a paginação em todos os métodos de listagem da aplicação, implementando métodos paginados que retornam `PagedResult<T>` seguindo o padrão já estabelecido no projeto.

## Mudanças Principais

### 1. Métodos Paginados Adicionados nos Services ✅

**FeedService:**
- ✅ `ListForUserPagedAsync` - Paginação para feed pessoal do usuário

**TerritoryService:**
- ✅ `ListAvailablePagedAsync` - Paginação para listagem de territórios disponíveis
- ✅ `SearchPagedAsync` - Paginação para busca de territórios
- ✅ `NearbyPagedAsync` - Paginação para territórios próximos

**MapService:**
- ✅ `ListEntitiesPagedAsync` - Paginação para listagem de entidades do mapa

**EventsService:**
- ✅ `ListEventsPagedAsync` - Paginação para listagem de eventos
- ✅ `GetEventsNearbyPagedAsync` - Paginação para eventos próximos

**ListingService:**
- ✅ `SearchListingsPagedAsync` - Paginação para busca de listings

**AssetService:**
- ✅ `ListPagedAsync` - Paginação para listagem de assets

**ReportService:**
- ✅ `ListPagedAsync` - Paginação para listagem de reports

**JoinRequestService:**
- ✅ `ListIncomingPagedAsync` - Paginação para listagem de join requests recebidos

**InquiryService:**
- ✅ `ListMyInquiriesPagedAsync` - Paginação para inquiries do usuário
- ✅ `ListReceivedInquiriesPagedAsync` - Paginação para inquiries recebidos

**HealthService:**
- ✅ `ListAlertsPagedAsync` - Paginação para listagem de alertas de saúde

**PlatformFeeService:**
- ✅ `ListActivePagedAsync` - Paginação para listagem de configurações de taxa ativas

### 2. Padrão de Implementação

Todos os métodos paginados seguem o mesmo padrão:
- Aceitam `PaginationParameters` como parâmetro
- Retornam `PagedResult<T>` com informações de paginação
- Mantêm os métodos não-paginados originais para compatibilidade
- Ordenação padrão: por data de criação (mais recente primeiro)

### 3. Estrutura de Paginação

```csharp
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}

public sealed class PaginationParameters
{
    public int PageNumber { get; }  // Default: 1
    public int PageSize { get; }    // Default: 20, Max: 100
    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}
```

## Benefícios

1. **Performance**: Reduz carga no servidor e banco de dados ao limitar resultados
2. **Escalabilidade**: Permite lidar com grandes volumes de dados
3. **UX**: Melhora experiência do usuário com carregamento mais rápido
4. **Consistência**: Padrão uniforme em toda a aplicação
5. **Compatibilidade**: Métodos antigos mantidos para não quebrar integrações existentes

## Próximos Passos

Após merge deste PR, podemos continuar com:
- Atualizar controllers para expor endpoints paginados
- Adicionar documentação Swagger para os novos endpoints
- Atualizar testes para incluir cenários de paginação
- Considerar implementar paginação no nível do repositório para melhor performance

## Checklist

- [x] Métodos paginados adicionados em todos os services (21 métodos)
- [x] Padrão consistente de implementação
- [x] Métodos originais mantidos para compatibilidade
- [x] Build sem erros
- [ ] Controllers atualizados para expor endpoints paginados (próximo passo)
- [ ] Testes atualizados para incluir paginação
- [ ] Documentação Swagger atualizada

## Breaking Changes

⚠️ **Nenhum breaking change**. Os métodos originais foram mantidos e novos métodos paginados foram adicionados.

## Commits

- `feat: adicionar paginação em FeedService, TerritoryService e MapService`
- `feat: adicionar paginação em EventsService, ListingService e AssetService`
- `feat: adicionar paginação em ReportService, JoinRequestService e InquiryService`
- `feat: adicionar paginação em HealthService e PlatformFeeService`
