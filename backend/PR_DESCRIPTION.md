# Pull Request: Documentação, testes BFF/ApiSupport e validação de fronteiras

## Resumo

Esta PR consolida melhorias de documentação do backend, novos projetos de teste (BFF e ApiSupport), movimentação de testes por módulo, regras de onde colocar repositórios e um script de CI para validar que o Domain de um módulo não referencia a Infrastructure do mesmo módulo.

---

## Commits

### 1. docs: melhorias documentadas, regra de repositório e script de fronteiras

**Arquivos alterados:**

- **`docs/IMPROVEMENTS_AND_KNOWN_ISSUES.md`**
  - Ações concluídas documentadas: testes por módulo (Marketplace), `CacheTestHelper` em Tests.Shared, BFF, ApiSupport, critério “o que entra no Domain”, script de validação de fronteiras.

- **`docs/BACKEND_LAYERS_AND_NAMING.md`**
  - Nova seção 9 sobre o BFF (Backend for Frontend).
  - Resumo e regras de decisão atualizados.

- **`docs/TEST_SEPARATION_BY_MODULE.md`**
  - Mapeamento e ações realizadas: Moderation, Marketplace, Tests.Shared.

- **`README.md` (backend)**
  - Nova seção “Onde colocar um novo repositório”.
  - Estrutura de `Tests` atualizada (ApiSupport, Bff).

- **`scripts/validate-module-boundaries.ps1`**
  - Script novo para validar que arquivos em `Domain/` de um módulo não referenciam `Infrastructure/` do mesmo módulo, para uso em CI.

---

### 2. test: Araponga.Tests.Bff, ApiSupport e testes por módulo

**Arquivos alterados / projetos:**

- **`Araponga.Tests.Bff`** (novo)
  - Projeto de testes para o BFF.
  - Testes para `JourneyResponseCache` (18 testes).

- **`Araponga.Tests.ApiSupport`** (novo)
  - Projeto com `BaseApiFactory` e `AuthTestHelper` compartilhados para testes de API.

- **`Araponga.Tests`**
  - `ApiFactory` passa a herdar de `BaseApiFactory`.
  - `AuthTestHelper` vira facade para o `AuthTestHelper` de ApiSupport.

- **`Araponga.Tests.Modules.Subscriptions`**
  - `ApiFactory` herda `BaseApiFactory`.
  - `SubscriptionIntegrationTests` usa `AuthTestHelper` do ApiSupport.

- **`Araponga.Tests.Modules.Marketplace`**
  - `GlobalUsings` para `TestIds`.
  - Testes de Application movidos do Core para o módulo.

- **`Araponga.Tests.Modules.Moderation`**
  - Testes de Application movidos do Core para o módulo.

- **`Araponga.Tests.Shared`**
  - `CacheTestHelper` e `PatternAwareTestCacheService` consolidados para reuso entre projetos de teste.

- **`Araponga.sln`**
  - Inclusão dos projetos `Araponga.Tests.Bff` e `Araponga.Tests.ApiSupport`.

---

## Checklist

- [x] Documentação atualizada (IMPROVEMENTS, BACKEND_LAYERS, TEST_SEPARATION, README).
- [x] Script de validação de fronteiras (Domain vs Infrastructure) criado.
- [x] Testes do BFF e ApiSupport adicionados; testes por módulo (Moderation, Marketplace) movidos.
- [x] Solution atualizada com os novos projetos de teste.
