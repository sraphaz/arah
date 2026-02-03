# Template: Seção TDD/BDD para Fases

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Template Obrigatório

---

## 📋 Uso

Este template **DEVE** ser incluído em **TODAS as fases** do backlog, após a seção "📋 Tarefas Detalhadas" e antes de "📊 Resumo da Fase".

**Referência**: [Plano Completo TDD/BDD](../23_TDD_BDD_PLANO_IMPLEMENTACAO.md)

---

## 🧪 Estratégia TDD/BDD

### Contexto

Esta fase segue o padrão estabelecido na **Fase 0: Fundação TDD/BDD**, garantindo:
- ✅ **TDD obrigatório**: Testes escritos ANTES do código (Red-Green-Refactor)
- ✅ **BDD para funcionalidades de negócio**: Features Gherkin documentam comportamento
- ✅ **Cobertura >90%**: Meta obrigatória para todas as funcionalidades
- ✅ **Cobertura >95%**: Obrigatória para funcionalidades críticas (segurança, pagamentos, blockchain)

### Tempo Adicional Estimado

- **+20% de tempo** para implementação TDD/BDD
- **+10% de tempo** para documentação BDD

**Duração ajustada**: [Duração Original] → [Duração Original × 1.2] dias

---

### TDD: Test-Driven Development

#### Processo Red-Green-Refactor

Para cada funcionalidade implementada nesta fase:

1. **Red**: Escrever teste que falha
   - Teste unitário (xUnit) para lógica de negócio
   - Teste de integração (E2E) para fluxos completos
   - Nomenclatura: `MethodName_Scenario_ExpectedBehavior`

2. **Green**: Implementar mínimo para passar
   - Implementar apenas o necessário para o teste passar
   - Não adicionar funcionalidades extras (YAGNI)

3. **Refactor**: Melhorar código mantendo testes passando
   - Refatorar código mantendo todos os testes verdes
   - Aplicar princípios SOLID, Clean Code

#### Testes Obrigatórios

**Para cada funcionalidade**:
- [ ] Testes unitários (Domain, Application)
- [ ] Testes de integração (API, E2E)
- [ ] Testes de validação (edge cases, erros)
- [ ] Testes de segurança (quando aplicável)

**Cobertura mínima**:
- ✅ **>90%** para funcionalidades padrão
- ✅ **>95%** para funcionalidades críticas (segurança, pagamentos, blockchain)

---

### BDD: Behavior-Driven Development

#### Features Gherkin Obrigatórias

**Para funcionalidades de negócio críticas**, criar features Gherkin (SpecFlow):

**Estrutura de arquivo**:
```
backend/Araponga.Tests/
├── Api/BDD/
│   └── [FeatureName].feature
├── Application/BDD/
│   └── [FeatureName].feature
└── Domain/BDD/
    └── [FeatureName].feature
```

**Template de Feature**:
```gherkin
Feature: [Nome da Funcionalidade]
  Como um [tipo de usuário]
  Eu quero [ação]
  Para [objetivo/valor]

  Background:
    Dado que existe um território "[Nome]"
    E que existe um usuário "[Nome]" como [papel]

  Scenario: [Cenário de sucesso]
    Dado que [condição inicial]
    Quando [ação do usuário]
    Então [resultado esperado]
    E [resultado adicional]

  Scenario: [Cenário de erro]
    Dado que [condição inicial]
    Quando [ação inválida]
    Então deve retornar erro "[mensagem]"
```

#### Features BDD Obrigatórias para Esta Fase

[Listar funcionalidades que DEVEM ter BDD nesta fase]

**Exemplo**:
- [ ] `Feature: Criar Post` - Fluxo completo de criação de post
- [ ] `Feature: Editar Post` - Fluxo de edição com validações
- [ ] `Feature: Avaliar Item` - Sistema de avaliações do marketplace

---

### Checklist TDD/BDD por Funcionalidade

Para cada funcionalidade implementada, validar:

**TDD**:
- [ ] Teste escrito ANTES do código (Red)
- [ ] Teste passa após implementação (Green)
- [ ] Código refatorado mantendo testes verdes (Refactor)
- [ ] Cobertura >90% (ou >95% se crítico)
- [ ] Testes de edge cases implementados
- [ ] Testes de erro implementados

**BDD** (quando aplicável):
- [ ] Feature Gherkin criada
- [ ] Steps implementados (SpecFlow)
- [ ] Feature documenta comportamento de negócio
- [ ] Feature serve como documentação viva
- [ ] Feature validada com stakeholders (quando aplicável)

**Integração**:
- [ ] Testes de integração E2E implementados
- [ ] Testes de API implementados
- [ ] Testes de segurança implementados (quando aplicável)
- [ ] Todos os testes passando no CI/CD

---

### Métricas de Sucesso

**Ao final da fase**:
- ✅ Cobertura de código >90% (ou >95% se crítico)
- ✅ Todas as funcionalidades de negócio com BDD
- ✅ 100% dos testes passando
- ✅ Nenhum teste ignorado ou comentado
- ✅ Documentação BDD atualizada

---

### Referências

- [Plano Completo TDD/BDD](../23_TDD_BDD_PLANO_IMPLEMENTACAO.md)
- [Fase 0: Fundação TDD/BDD](./FASE0.md)
- [Análise de Coesão e Testes](../22_COHESION_AND_TESTS.md)
- [Padrões de Código](../21_CODE_REVIEW.md)

---

## 📝 Notas de Implementação

### Exemplos de Testes TDD

**Teste Unitário (Domain)**:
```csharp
[Fact]
public async Task CreatePostAsync_WhenUserIsResident_ReturnsSuccess()
{
    // Arrange
    var dataStore = new InMemoryDataStore();
    var service = FeedServiceTestHelper.CreateFeedService(dataStore);
    
    // Act
    var result = await service.CreatePostAsync(...);
    
    // Assert
    Assert.True(result.IsSuccess);
}
```

**Teste de Integração (API)**:
```csharp
[Fact]
public async Task CreatePost_WithValidData_ReturnsCreated()
{
    using var factory = new ApiFactory();
    using var client = factory.CreateClient();
    
    var token = await LoginForTokenAsync(client, "google", "test-user");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);
    
    var request = new CreatePostRequest(...);
    var response = await client.PostAsJsonAsync("api/v1/feed/posts", request);
    
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

### Exemplos de Features BDD

**Feature Gherkin**:
```gherkin
Feature: Criar Post no Feed
  Como um residente do território
  Eu quero criar posts no feed
  Para compartilhar informações com a comunidade

  Background:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente

  Scenario: Criar post com sucesso
    Dado que o usuário "João" está autenticado
    Quando ele cria um post com o conteúdo "Olá comunidade!"
    Então o post deve ser criado com sucesso
    E o post deve aparecer no feed do território

  Scenario: Criar post sem autenticação
    Quando um usuário não autenticado tenta criar um post
    Então deve retornar erro "Unauthorized"
```

---

**Última Atualização**: 2025-01-20
