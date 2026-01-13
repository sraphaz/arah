# Guia de Testes - Araponga

## Princípios de Testes

### 1. Isolamento Completo
**Cada teste deve ser independente e não depender da ordem de execução.**

- ✅ Cada teste cria seu próprio `ApiFactory` usando `using var factory = new ApiFactory()`
- ✅ Cada teste cria seu próprio `InMemoryDataStore` para testes de aplicação
- ✅ Não há estado compartilhado entre testes
- ✅ Testes podem ser executados em qualquer ordem

### 2. Setup e Cleanup

#### Testes de API (ApiScenariosTests, EndToEndTests)
```csharp
[Fact]
public async Task MyTest()
{
    // Cada teste cria seu próprio factory isolado
    using var factory = new ApiFactory();
    using var client = factory.CreateClient();
    
    // Teste aqui...
    // O factory será descartado automaticamente ao final do teste
}
```

#### Testes de Aplicação (ApplicationServiceTests, MarketplaceServiceTests)
```csharp
[Fact]
public async Task MyTest()
{
    // Cada teste cria seu próprio dataStore isolado
    var dataStore = new InMemoryDataStore();
    var service = FeedServiceTestHelper.CreateFeedService(dataStore);
    
    // Teste aqui...
    // O dataStore é descartado quando sai do escopo
}
```

### 3. Fixtures Compartilhadas (Opcional)

Se você precisar compartilhar setup entre múltiplos testes na mesma classe, use `IClassFixture`:

```csharp
public class MyTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public MyTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Test1()
    {
        using var client = _factory.CreateClient();
        // Teste aqui...
    }
}
```

**IMPORTANTE**: Mesmo usando `IClassFixture`, cada instância do `ApiFactory` cria seu próprio `InMemoryDataStore` isolado.

## Estrutura de Testes

### ApiScenariosTests
- Testes de integração da API
- Cada teste cria seu próprio `ApiFactory`
- Testam fluxos completos de requisições HTTP

### ApplicationServiceTests
- Testes unitários dos services da camada de aplicação
- Cada teste cria seu próprio `InMemoryDataStore`
- Testam lógica de negócio isolada

### MarketplaceServiceTests
- Testes específicos do módulo Marketplace
- Cada teste cria seu próprio `InMemoryDataStore`
- Testam stores, listings, cart, inquiries

### RepositoryTests
- Testes de infraestrutura (repositórios)
- Cada teste cria seu próprio `InMemoryDataStore`
- Testam operações CRUD básicas

### EndToEndTests
- Testes end-to-end de fluxos críticos
- Cada teste cria seu próprio `ApiFactory`
- Testam fluxos completos do usuário

## Boas Práticas

1. **Sempre use `using` para garantir cleanup automático**
   ```csharp
   using var factory = new ApiFactory();
   ```

2. **Cada teste deve ser auto-suficiente**
   - Não dependa de dados criados por outros testes
   - Crie todos os dados necessários no próprio teste

3. **Use helpers para reduzir duplicação**
   - `FeedServiceTestHelper.CreateFeedService()`
   - Métodos privados para setup comum dentro da mesma classe

4. **Nomes descritivos**
   - `ResidentCanCreateStoreAndListing` ✅
   - `Test1` ❌

5. **Teste um comportamento por vez**
   - Um teste = uma verificação específica
   - Se precisar testar múltiplos cenários, crie múltiplos testes

## Troubleshooting

### Testes falhando intermitentemente
- Verifique se há estado compartilhado
- Certifique-se de que cada teste cria seu próprio `ApiFactory` ou `InMemoryDataStore`

### Testes dependendo da ordem de execução
- Isso é um anti-pattern! Cada teste deve ser independente
- Revise o código para garantir isolamento completo

### Performance lenta
- Testes de API são mais lentos (criam servidor HTTP completo)
- Prefira testes de aplicação quando possível
- Use `IClassFixture` para compartilhar factory apenas quando necessário
