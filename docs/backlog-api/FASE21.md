# Fase 21: Suporte a Criptomoedas (Crypto Ready)

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟢 MÉDIA  
**Bloqueia**: Aceitar pagamentos em criptomoedas  
**Estimativa Total**: 152 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Tornar a aplicação "crypto ready" para aceitar pagamentos tanto em gateways tradicionais (Stripe, MercadoPago) quanto em criptomoedas (Bitcoin, Ethereum, stablecoins), mantendo o mesmo modelo justo, transparente e seguro.

---

## 📋 Contexto e Requisitos

### Visão Geral
A aplicação deve suportar **múltiplos métodos de pagamento** de forma unificada:
- **Pagamentos Tradicionais**: Stripe, MercadoPago, PagSeguro (já implementado)
- **Criptomoedas**: Bitcoin, Ethereum, USDC, USDT, etc.

### Princípios
1. **Firme**: Arquitetura sólida e extensível
2. **Conciso**: Interface unificada, código limpo
3. **Justo**: Mesmas fees, mesma transparência, mesmo tratamento
4. **Seguro**: Validações específicas para cripto, proteção contra fraudes
5. **Testável**: Testes completos para ambos os métodos

### Requisitos Funcionais
- ✅ Aceitar pagamentos em criptomoedas
- ✅ Mesma lógica de fees e payout (tradicional e cripto)
- ✅ Rastreabilidade completa (blockchain + sistema)
- ✅ Conversão automática (opcional) ou manter em cripto
- ✅ Validação de endereços de carteira
- ✅ Confirmação de transações blockchain
- ✅ Suporte a múltiplas criptomoedas
- ✅ Configuração por território (quais criptos aceitar)
- ✅ Feature flags para habilitar/desabilitar cripto
- ✅ Módulos administrativos para gerenciar cripto

---

## 🚩 Feature Flags e Dependências

### Grupos de Funcionalidades

#### Grupo 1: Marketplace Base
**Feature Flags**:
- `MarketplaceEnabled` (base) - Habilita marketplace no território

**Dependências**:
- Nenhuma (base)

**Funcionalidades**:
- Criar lojas
- Adicionar itens
- Carrinho e checkout

---

#### Grupo 2: Pagamentos Tradicionais
**Feature Flags**:
- `MarketplaceEnabled` (obrigatório)
- `PaymentEnabled` (novo) - Habilita pagamentos tradicionais

**Dependências**:
- `MarketplaceEnabled` → `PaymentEnabled`

**Funcionalidades**:
- Pagamentos via gateway (Stripe, MercadoPago)
- Webhooks de pagamento
- Reembolsos
- Payout tradicional

---

#### Grupo 3: Pagamentos em Criptomoedas
**Feature Flags**:
- `MarketplaceEnabled` (obrigatório)
- `PaymentEnabled` (obrigatório)
- `CryptoPaymentsEnabled` (novo) - Habilita pagamentos em cripto

**Dependências**:
- `MarketplaceEnabled` → `PaymentEnabled` → `CryptoPaymentsEnabled`

**Funcionalidades**:
- Pagamentos em Bitcoin, Ethereum, USDC, USDT
- Validação de endereços
- Confirmações blockchain
- Payout em cripto ou conversão para fiat

---

#### Grupo 4: Conversão de Moedas
**Feature Flags**:
- `MarketplaceEnabled` (obrigatório)
- `PaymentEnabled` (obrigatório)
- `CryptoPaymentsEnabled` (obrigatório)
- `CurrencyConversionEnabled` (novo) - Habilita conversão automática

**Dependências**:
- `MarketplaceEnabled` → `PaymentEnabled` → `CryptoPaymentsEnabled` → `CurrencyConversionEnabled`

**Funcionalidades**:
- Conversão cripto → fiat
- Conversão fiat → cripto
- Cache de cotações

---

### Matriz de Dependências

| Feature Flag | Depende de | Bloqueia | Grupo |
|--------------|------------|----------|-------|
| `MarketplaceEnabled` | - | - | 1: Marketplace Base |
| `PaymentEnabled` | `MarketplaceEnabled` | - | 2: Pagamentos Tradicionais |
| `CryptoPaymentsEnabled` | `PaymentEnabled` | - | 3: Pagamentos Cripto |
| `CurrencyConversionEnabled` | `CryptoPaymentsEnabled` | - | 4: Conversão |

### Regras de Validação

1. **Não pode habilitar `PaymentEnabled` sem `MarketplaceEnabled`**
2. **Não pode habilitar `CryptoPaymentsEnabled` sem `PaymentEnabled`**
3. **Não pode habilitar `CurrencyConversionEnabled` sem `CryptoPaymentsEnabled`**
4. **Pode desabilitar `CryptoPaymentsEnabled` mantendo `PaymentEnabled`** (volta para apenas tradicional)

---

## 📋 Tarefas Detalhadas

### Semana 17: Arquitetura, Feature Flags e Abstração

#### 17.1 Feature Flags para Criptomoedas
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar `CryptoPaymentsEnabled = 10` ao enum `FeatureFlag`
- [ ] Adicionar `CurrencyConversionEnabled = 11` ao enum `FeatureFlag`
- [ ] Criar `FeatureFlagDependencyValidator` (valida dependências)
- [ ] Atualizar `TerritoryFeatureFlagGuard` com métodos para cripto
- [ ] Implementar validação de dependências no `FeaturesController`
- [ ] Documentar feature flags e dependências

**Arquivos a Modificar**:
- `backend/Araponga.Application/Models/FeatureFlag.cs`
- `backend/Araponga.Application/Services/TerritoryFeatureFlagGuard.cs`
- `backend/Araponga.Api/Controllers/FeaturesController.cs`

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/FeatureFlagDependencyValidator.cs`

**Critérios de Sucesso**:
- ✅ Feature flags adicionadas
- ✅ Validação de dependências funcionando
- ✅ Guard methods criados
- ✅ Documentação completa

---

#### 17.2 Estender Interface de Pagamento para Suportar Cripto
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `PaymentMethod` enum (Traditional, Crypto)
- [ ] Criar `CryptoCurrency` enum (Bitcoin, Ethereum, USDC, USDT, etc.)
- [ ] Estender `IPaymentGateway` com métodos específicos para cripto
- [ ] Criar `ICryptoPaymentGateway` (interface específica para cripto)
- [ ] Criar `PaymentMethodResolver` (resolver qual gateway usar)
- [ ] Documentar arquitetura unificada

**Arquivos a Criar**:
- `backend/Araponga.Domain/Marketplace/PaymentMethod.cs`
- `backend/Araponga.Domain/Marketplace/CryptoCurrency.cs`
- `backend/Araponga.Application/Interfaces/ICryptoPaymentGateway.cs`
- `backend/Araponga.Application/Services/PaymentMethodResolver.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Interfaces/IPaymentGateway.cs` (estender se necessário)
- `backend/Araponga.Domain/Marketplace/TerritoryPaymentConfig.cs` (adicionar suporte a cripto)

**Critérios de Sucesso**:
- ✅ Interface unificada criada
- ✅ Resolver de método de pagamento funcionando
- ✅ Documentação completa

---

#### 17.3 Modelos de Domínio para Criptomoedas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoPayment` (entidade para pagamentos em cripto)
- [ ] Criar `CryptoTransaction` (transação blockchain)
- [ ] Criar `WalletAddress` (endereços de carteira)
- [ ] Criar `BlockchainConfirmation` (confirmações de blockchain)
- [ ] Criar relacionamento com `Checkout` e `FinancialTransaction`
- [ ] Criar migration para tabelas de cripto
- [ ] Documentar modelos

**Arquivos a Criar**:
- `backend/Araponga.Domain/Marketplace/CryptoPayment.cs`
- `backend/Araponga.Domain/Marketplace/CryptoTransaction.cs`
- `backend/Araponga.Domain/Marketplace/WalletAddress.cs`
- `backend/Araponga.Domain/Marketplace/BlockchainConfirmation.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/CryptoPaymentRecord.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/XXXXXX_AddCryptoPaymentSupport.cs`

**Critérios de Sucesso**:
- ✅ Modelos de domínio completos
- ✅ Relacionamentos funcionando
- ✅ Migration criada e testada

---

#### 17.4 Configuração de Criptomoedas por Território
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Estender `TerritoryPaymentConfig` para suportar criptomoedas
- [ ] Criar `TerritoryCryptoConfig` (quais criptos aceitar por território)
- [ ] Campos: `AllowedCryptocurrencies`, `AutoConvertToFiat`, `MinConfirmations`
- [ ] Criar `ITerritoryCryptoConfigRepository` e implementação
- [ ] Criar `TerritoryCryptoConfigService`
- [ ] Criar endpoints de configuração (FinancialManager/SystemAdmin)
- [ ] Validar feature flag `CryptoPaymentsEnabled` antes de configurar
- [ ] Documentar configurações

**Arquivos a Criar**:
- `backend/Araponga.Domain/Marketplace/TerritoryCryptoConfig.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryCryptoConfigRepository.cs`
- `backend/Araponga.Application/Services/TerritoryCryptoConfigService.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryCryptoConfigRepository.cs`
- `backend/Araponga.Api/Controllers/TerritoryCryptoConfigController.cs`

**Critérios de Sucesso**:
- ✅ Configuração por território funcionando
- ✅ Validação de feature flag implementada
- ✅ Validações implementadas
- ✅ Endpoints funcionando
- ✅ Documentação completa

---

### Semana 18: Implementação de Gateways e Serviços

#### 18.1 Integração com Provedor de Cripto (BitPay/Coinbase Commerce)
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Escolher provedor (BitPay, Coinbase Commerce, ou outro)
- [ ] Criar `BitPayCryptoGateway` ou `CoinbaseCryptoGateway`
- [ ] Implementar `ICryptoPaymentGateway`
- [ ] Métodos: `CreateCryptoPaymentAsync`, `GetCryptoPaymentStatusAsync`
- [ ] Métodos: `ValidateWalletAddressAsync`, `GetBlockchainConfirmationsAsync`
- [ ] Integrar com API do provedor
- [ ] Implementar webhooks para confirmações blockchain
- [ ] Validar feature flag `CryptoPaymentsEnabled` antes de processar
- [ ] Documentar integração

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Payments/BitPayCryptoGateway.cs` (ou Coinbase)
- `backend/Araponga.Application/Models/CryptoPaymentModels.cs`

**Critérios de Sucesso**:
- ✅ Gateway de cripto funcionando
- ✅ Validação de feature flag implementada
- ✅ Criação de pagamentos funcionando
- ✅ Verificação de status funcionando
- ✅ Webhooks funcionando
- ✅ Documentação completa

---

#### 18.2 Validação e Segurança de Cripto
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoValidationService`
- [ ] Implementar validação de endereços de carteira (Bitcoin, Ethereum, etc.)
- [ ] Implementar validação de assinaturas (se necessário)
- [ ] Implementar verificação de confirmações blockchain
- [ ] Implementar proteção contra double spending
- [ ] Implementar rate limiting específico para cripto
- [ ] Implementar detecção de transações suspeitas (valores anômalos, etc.)
- [ ] Documentar validações

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CryptoValidationService.cs`
- `backend/Araponga.Application/Services/CryptoAddressValidator.cs`

**Critérios de Sucesso**:
- ✅ Validação de endereços funcionando
- ✅ Verificação de confirmações funcionando
- ✅ Proteção contra double spending implementada
- ✅ Detecção de fraudes funcionando
- ✅ Documentação completa

---

#### 18.3 Serviço de Pagamento Unificado
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Estender `PaymentService` para suportar cripto
- [ ] Integrar `PaymentMethodResolver` no fluxo
- [ ] Implementar lógica unificada (tradicional e cripto)
- [ ] Manter mesma lógica de fees (aplicar fees em cripto ou converter)
- [ ] Integrar com `SellerPayoutService` (payout em cripto ou fiat)
- [ ] Criar rastreabilidade completa (FinancialTransaction para cripto)
- [ ] Validar feature flags antes de processar
- [ ] Documentar serviço unificado

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/PaymentService.cs` (estender)
- `backend/Araponga.Application/Services/SellerPayoutService.cs` (suportar cripto)

**Critérios de Sucesso**:
- ✅ Pagamentos tradicionais e cripto funcionando
- ✅ Validação de feature flags implementada
- ✅ Mesma lógica de fees aplicada
- ✅ Rastreabilidade completa
- ✅ Payout funcionando para ambos

---

#### 18.4 Conversão de Moedas (Opcional)
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CurrencyConversionService`
- [ ] Integrar com API de cotações (CoinGecko, CoinMarketCap, etc.)
- [ ] Implementar cache de cotações (TTL configurável)
- [ ] Implementar conversão cripto → fiat (se configurado)
- [ ] Implementar conversão fiat → cripto (se necessário)
- [ ] Validar feature flag `CurrencyConversionEnabled` antes de converter
- [ ] Validar cotações (proteção contra manipulação)
- [ ] Documentar conversão

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/CurrencyConversionService.cs`
- `backend/Araponga.Application/Interfaces/ICurrencyRateProvider.cs`
- `backend/Araponga.Infrastructure/External/CoinGeckoRateProvider.cs` (exemplo)

**Critérios de Sucesso**:
- ✅ Conversão de moedas funcionando
- ✅ Validação de feature flag implementada
- ✅ Cache de cotações implementado
- ✅ Validação de cotações funcionando
- ✅ Documentação completa

---

### Semana 19: Módulos Administrativos e Testes

#### 19.1 Módulo Administrativo de Criptomoedas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `AdminCryptoConfigController` (apenas SystemAdmin)
- [ ] Endpoint: `GET /api/v1/admin/crypto-config` (listar configurações globais)
- [ ] Endpoint: `PUT /api/v1/admin/crypto-config` (configurar provedores globais)
- [ ] Criar `AdminCryptoTransactionsController` (apenas SystemAdmin)
- [ ] Endpoint: `GET /api/v1/admin/crypto-transactions` (listar transações)
- [ ] Endpoint: `GET /api/v1/admin/crypto-transactions/{id}` (detalhes)
- [ ] Endpoint: `POST /api/v1/admin/crypto-transactions/{id}/reconcile` (conciliação manual)
- [ ] Criar `AdminCryptoRatesController` (apenas SystemAdmin)
- [ ] Endpoint: `GET /api/v1/admin/crypto-rates` (visualizar cotações)
- [ ] Endpoint: `POST /api/v1/admin/crypto-rates/refresh` (forçar atualização)
- [ ] Implementar autorizações (apenas SystemAdmin)
- [ ] Documentar módulos administrativos

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/AdminCryptoConfigController.cs`
- `backend/Araponga.Api/Controllers/AdminCryptoTransactionsController.cs`
- `backend/Araponga.Api/Controllers/AdminCryptoRatesController.cs`
- `backend/Araponga.Api/Contracts/Admin/CryptoAdminContracts.cs`

**Critérios de Sucesso**:
- ✅ Módulos administrativos criados
- ✅ Autorizações funcionando (apenas SystemAdmin)
- ✅ Endpoints funcionando
- ✅ Documentação completa

---

#### 19.2 Testes Unitários
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoPaymentServiceTests`
  - [ ] Testar criação de pagamento em cripto
  - [ ] Testar validação de feature flags
  - [ ] Testar validação de endereços
  - [ ] Testar verificação de confirmações
  - [ ] Testar proteção contra double spending
- [ ] Criar `CryptoValidationServiceTests`
  - [ ] Testar validação de endereços Bitcoin
  - [ ] Testar validação de endereços Ethereum
  - [ ] Testar validação de endereços USDC/USDT
  - [ ] Testar detecção de endereços inválidos
- [ ] Criar `PaymentMethodResolverTests`
  - [ ] Testar resolução de método tradicional
  - [ ] Testar resolução de método cripto
  - [ ] Testar fallback
  - [ ] Testar validação de feature flags
- [ ] Criar `CurrencyConversionServiceTests`
  - [ ] Testar conversão cripto → fiat
  - [ ] Testar cache de cotações
  - [ ] Testar validação de cotações
  - [ ] Testar validação de feature flag
- [ ] Criar `FeatureFlagDependencyValidatorTests`
  - [ ] Testar validação de dependências
  - [ ] Testar erros quando dependências faltam
  - [ ] Testar desabilitação em cascata
- [ ] Criar `TerritoryCryptoConfigServiceTests`
- [ ] Alcançar cobertura >90% em todos os serviços

**Arquivos a Criar**:
- `backend/Araponga.Tests/Application/CryptoPaymentServiceTests.cs`
- `backend/Araponga.Tests/Application/CryptoValidationServiceTests.cs`
- `backend/Araponga.Tests/Application/PaymentMethodResolverTests.cs`
- `backend/Araponga.Tests/Application/CurrencyConversionServiceTests.cs`
- `backend/Araponga.Tests/Application/FeatureFlagDependencyValidatorTests.cs`
- `backend/Araponga.Tests/Application/TerritoryCryptoConfigServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Cobertura >90% em todos os serviços
- ✅ Todos os testes passando
- ✅ Edge cases cobertos

---

#### 19.3 Testes de Integração
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoPaymentControllerTests`
  - [ ] Testar endpoints de pagamento em cripto
  - [ ] Testar autorizações
  - [ ] Testar validações
  - [ ] Testar validação de feature flags
- [ ] Criar `TerritoryCryptoConfigControllerTests`
  - [ ] Testar configuração de criptomoedas
  - [ ] Testar autorizações
  - [ ] Testar validação de feature flags
- [ ] Criar `AdminCryptoConfigControllerTests`
  - [ ] Testar endpoints administrativos
  - [ ] Testar autorizações (apenas SystemAdmin)
- [ ] Criar testes end-to-end do fluxo completo:
  - [ ] Checkout → Pagamento Cripto → Confirmação Blockchain → SellerTransaction
  - [ ] Testar rastreabilidade completa
  - [ ] Testar payout em cripto
  - [ ] Testar conversão de moedas
- [ ] Criar `MockCryptoGateway` para testes
- [ ] Testar integração com gateway real (sandbox)

**Arquivos a Criar**:
- `backend/Araponga.Tests/Api/CryptoPaymentControllerTests.cs`
- `backend/Araponga.Tests/Api/TerritoryCryptoConfigControllerTests.cs`
- `backend/Araponga.Tests/Api/AdminCryptoConfigControllerTests.cs`
- `backend/Araponga.Tests/Integration/CryptoPaymentFlowTests.cs`
- `backend/Araponga.Infrastructure/Payments/MockCryptoGateway.cs`

**Critérios de Sucesso**:
- ✅ Todos os endpoints testados
- ✅ Fluxo completo testado end-to-end
- ✅ Integração com gateway testada
- ✅ Todos os testes passando

---

#### 19.4 Testes de Cache
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoPaymentCacheTests`
  - [ ] Testar cache de feature flags de cripto
  - [ ] Testar invalidação de cache quando feature flag muda
  - [ ] Testar cache de configurações de cripto
  - [ ] Testar cache de cotações de moedas
  - [ ] Testar TTL de cache
  - [ ] Testar fallback quando cache falha
- [ ] Criar `CurrencyConversionCacheTests`
  - [ ] Testar cache de cotações
  - [ ] Testar invalidação de cache
  - [ ] Testar TTL configurável
  - [ ] Testar cache distribuído (Redis)
- [ ] Criar `CryptoConfigCacheTests`
  - [ ] Testar cache de configurações por território
  - [ ] Testar invalidação quando configuração muda
- [ ] Testar performance de cache (hit/miss rates)
- [ ] Testar concorrência de cache

**Arquivos a Criar**:
- `backend/Araponga.Tests/Infrastructure/Cache/CryptoPaymentCacheTests.cs`
- `backend/Araponga.Tests/Infrastructure/Cache/CurrencyConversionCacheTests.cs`
- `backend/Araponga.Tests/Infrastructure/Cache/CryptoConfigCacheTests.cs`

**Critérios de Sucesso**:
- ✅ Todos os testes de cache passando
- ✅ Performance de cache validada
- ✅ Invalidação funcionando corretamente

---

#### 19.5 Testes de Rate Limiting
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoPaymentRateLimitTests`
  - [ ] Testar rate limiting em `POST /api/v1/payments/create` (crypto)
  - [ ] Testar rate limiting em `POST /api/v1/payments/confirm` (crypto)
  - [ ] Testar rate limiting em `POST /api/v1/payments/webhook` (crypto)
  - [ ] Testar rate limiting por usuário autenticado
  - [ ] Testar rate limiting por endereço de carteira
  - [ ] Testar headers X-RateLimit-* retornados
  - [ ] Testar retorno 429 quando excedido
- [ ] Criar `CryptoConfigRateLimitTests`
  - [ ] Testar rate limiting em endpoints de configuração
  - [ ] Testar rate limiting em endpoints administrativos
- [ ] Criar `CryptoAdminRateLimitTests`
  - [ ] Testar rate limiting em endpoints administrativos
  - [ ] Testar rate limiting diferenciado para SystemAdmin
- [ ] Testar rate limiting com múltiplas requisições simultâneas
- [ ] Testar rate limiting após período de janela

**Arquivos a Criar**:
- `backend/Araponga.Tests/Security/CryptoPaymentRateLimitTests.cs`
- `backend/Araponga.Tests/Security/CryptoConfigRateLimitTests.cs`
- `backend/Araponga.Tests/Security/CryptoAdminRateLimitTests.cs`

**Critérios de Sucesso**:
- ✅ Todos os testes de rate limiting passando
- ✅ Rate limiting funcionando corretamente
- ✅ Headers retornados corretamente

---

### Semana 20: Testes de Segurança, Documentação e Portal

#### 20.1 Testes de Segurança Específicos para Cripto
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `CryptoPaymentSecurityTests`
  - [ ] Testar validação de endereços maliciosos
  - [ ] Testar proteção contra double spending
  - [ ] Testar proteção contra replay attacks
  - [ ] Testar validação de confirmações blockchain
  - [ ] Testar rate limiting em endpoints de cripto
  - [ ] Testar que feature flags são validadas
- [ ] Criar `CryptoAddressSecurityTests`
  - [ ] Testar injeção de endereços inválidos
  - [ ] Testar endereços de teste em produção
  - [ ] Testar endereços de outras redes (mainnet vs testnet)
- [ ] Criar `CryptoTransactionSecurityTests`
  - [ ] Testar manipulação de valores
  - [ ] Testar manipulação de confirmações
  - [ ] Testar proteção contra race conditions
- [ ] Criar `CryptoAdminSecurityTests`
  - [ ] Testar que apenas SystemAdmin acessa módulos administrativos
  - [ ] Testar que dados sensíveis não são expostos
- [ ] Testar proteção de dados sensíveis (chaves privadas, seeds, etc.)

**Arquivos a Criar**:
- `backend/Araponga.Tests/Security/CryptoPaymentSecurityTests.cs`
- `backend/Araponga.Tests/Security/CryptoAddressSecurityTests.cs`
- `backend/Araponga.Tests/Security/CryptoTransactionSecurityTests.cs`
- `backend/Araponga.Tests/Security/CryptoAdminSecurityTests.cs`

**Critérios de Sucesso**:
- ✅ Todas as vulnerabilidades identificadas e corrigidas
- ✅ Proteções contra ataques implementadas
- ✅ Todos os testes de segurança passando

---

#### 20.2 Documentação Técnica Completa
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `docs/CRYPTO_PAYMENT_ARCHITECTURE.md`
  - [ ] Arquitetura unificada (tradicional + cripto)
  - [ ] Fluxo completo de pagamento em cripto
  - [ ] Diagramas de sequência
  - [ ] Modelos de domínio
  - [ ] Feature flags e dependências
- [ ] Criar `docs/CRYPTO_CONFIGURATION.md`
  - [ ] Como configurar criptomoedas por território
  - [ ] Explicação de cada configuração
  - [ ] Exemplos de configuração
  - [ ] Feature flags necessárias
- [ ] Criar `docs/CRYPTO_SECURITY.md`
  - [ ] Validações de segurança específicas
  - [ ] Proteções implementadas
  - [ ] Boas práticas
  - [ ] Rate limiting específico
- [ ] Criar `docs/CRYPTO_INTEGRATION.md`
  - [ ] Como integrar novos provedores
  - [ ] Como adicionar novas criptomoedas
  - [ ] Exemplos de código
- [ ] Criar `docs/FEATURE_FLAGS_CRYPTO.md`
  - [ ] Lista completa de feature flags
  - [ ] Dependências entre flags
  - [ ] Grupos de funcionalidades
  - [ ] Como habilitar/desabilitar
- [ ] Atualizar `docs/API.md` com novos endpoints
- [ ] Atualizar `docs/CHANGELOG.md`

**Arquivos a Criar**:
- `docs/CRYPTO_PAYMENT_ARCHITECTURE.md`
- `docs/CRYPTO_CONFIGURATION.md`
- `docs/CRYPTO_SECURITY.md`
- `docs/CRYPTO_INTEGRATION.md`
- `docs/FEATURE_FLAGS_CRYPTO.md`

**Arquivos a Modificar**:
- `docs/API.md`
- `docs/CHANGELOG.md`

**Critérios de Sucesso**:
- ✅ Documentação técnica completa
- ✅ Documentação de API atualizada
- ✅ Exemplos e diagramas incluídos

---

#### 20.3 Atualização Detalhada do DevPortal
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar seção "Pagamentos e Criptomoedas" no DevPortal
- [ ] Documentar feature flags de cripto:
  - [ ] `CryptoPaymentsEnabled` - Descrição, dependências, como habilitar
  - [ ] `CurrencyConversionEnabled` - Descrição, dependências, como habilitar
- [ ] Adicionar card "Feature Flags e Dependências"
  - [ ] Matriz de dependências
  - [ ] Grupos de funcionalidades
  - [ ] Regras de validação
  - [ ] Exemplos de configuração
- [ ] Adicionar card "Pagamentos em Criptomoedas"
  - [ ] Como funciona
  - [ ] Criptomoedas suportadas
  - [ ] Fluxo completo
  - [ ] Exemplos de API
- [ ] Adicionar card "Configuração de Criptomoedas"
  - [ ] Como configurar por território
  - [ ] Validações necessárias
  - [ ] Exemplos de configuração
- [ ] Adicionar card "Módulos Administrativos"
  - [ ] Endpoints administrativos
  - [ ] Autorizações necessárias
  - [ ] Exemplos de uso
- [ ] Adicionar card "Segurança e Rate Limiting"
  - [ ] Validações de segurança
  - [ ] Rate limiting específico
  - [ ] Boas práticas
- [ ] Adicionar exemplos de código para cada funcionalidade
- [ ] Adicionar diagramas de fluxo
- [ ] Atualizar OpenAPI/Swagger com novos endpoints

**Arquivos a Modificar**:
- `backend/Araponga.Api/wwwroot/devportal/index.html`
- `backend/Araponga.Api/wwwroot/devportal/openapi.json` (se existir)

**Critérios de Sucesso**:
- ✅ DevPortal atualizado com todas as seções
- ✅ Feature flags documentadas com dependências
- ✅ Exemplos de código incluídos
- ✅ Diagramas incluídos
- ✅ OpenAPI atualizado

---

#### 20.4 Revisão de Segurança Completa para Cripto
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `docs/validation/VALIDACAO_SEGURANCA_CRIPTO.md`
  - [ ] Análise completa de segurança para cripto
  - [ ] Identificação de vulnerabilidades específicas
  - [ ] Recomendações de correção
- [ ] Revisar validação de endereços
  - [ ] Validar que todos os endereços são verificados
  - [ ] Validar que endereços de teste são rejeitados em produção
  - [ ] Validar que endereços de outras redes são rejeitados
- [ ] Revisar confirmações blockchain
  - [ ] Validar que número mínimo de confirmações é respeitado
  - [ ] Validar que confirmações são verificadas corretamente
  - [ ] Validar proteção contra reorganizações de blockchain
- [ ] Revisar proteção contra double spending
  - [ ] Validar que transações duplicadas são detectadas
  - [ ] Validar que hash de transação é único
  - [ ] Validar proteção contra replay attacks
- [ ] Revisar proteção de dados sensíveis
  - [ ] Validar que chaves privadas nunca são armazenadas
  - [ ] Validar que seeds nunca são expostos
  - [ ] Validar que apenas endereços públicos são armazenados
- [ ] Revisar rate limiting
  - [ ] Validar rate limiting em endpoints de cripto
  - [ ] Validar rate limiting por endereço de carteira
  - [ ] Validar rate limiting em módulos administrativos
- [ ] Revisar feature flags
  - [ ] Validar que feature flags são verificadas antes de processar
  - [ ] Validar que dependências são respeitadas
- [ ] Revisar módulos administrativos
  - [ ] Validar que apenas SystemAdmin acessa
  - [ ] Validar que dados sensíveis não são expostos
- [ ] Implementar correções identificadas
- [ ] Validar que todas as vulnerabilidades foram corrigidas

**Arquivos a Criar**:
- `docs/validation/VALIDACAO_SEGURANCA_CRIPTO.md`

**Arquivos a Modificar**:
- Todos os serviços e controllers de cripto (aplicar correções)

**Critérios de Sucesso**:
- ✅ Análise de segurança completa realizada
- ✅ Todas as vulnerabilidades identificadas e corrigidas
- ✅ Documentação de segurança criada
- ✅ Validações de segurança implementadas
- ✅ Testes de segurança passando

---

## 📊 Resumo da Fase 8

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Feature Flags para Cripto | 8h | ❌ Pendente | 🟢 Média |
| Estender Interface de Pagamento | 16h | ❌ Pendente | 🟢 Média |
| Modelos de Domínio para Cripto | 16h | ❌ Pendente | 🟢 Média |
| Configuração de Criptomoedas | 12h | ❌ Pendente | 🟢 Média |
| Integração com Provedor de Cripto | 24h | ❌ Pendente | 🟢 Média |
| Validação e Segurança de Cripto | 20h | ❌ Pendente | 🟢 Média |
| Serviço de Pagamento Unificado | 16h | ❌ Pendente | 🟢 Média |
| Conversão de Moedas | 12h | ❌ Pendente | 🟢 Média |
| **Módulo Administrativo** | **16h** | ❌ Pendente | 🟢 Média |
| **Testes Unitários** | **20h** | ❌ Pendente | 🟢 Média |
| **Testes de Integração** | **16h** | ❌ Pendente | 🟢 Média |
| **Testes de Cache** | **12h** | ❌ Pendente | 🟢 Média |
| **Testes de Rate Limiting** | **12h** | ❌ Pendente | 🟢 Média |
| **Testes de Segurança** | **16h** | ❌ Pendente | 🟢 Média |
| **Documentação Técnica** | **16h** | ❌ Pendente | 🟢 Média |
| **Atualização DevPortal** | **16h** | ❌ Pendente | 🟢 Média |
| **Revisão de Segurança** | **20h** | ❌ Pendente | 🟢 Média |
| **Total** | **152h (28 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 8

### Funcionalidades
- ✅ Pagamentos em criptomoedas funcionando
- ✅ Feature flags implementadas com dependências validadas
- ✅ Mesma lógica de fees aplicada (justo)
- ✅ Rastreabilidade completa (blockchain + sistema)
- ✅ Validação de endereços funcionando
- ✅ Confirmações blockchain verificadas
- ✅ Proteção contra double spending implementada
- ✅ Suporte a múltiplas criptomoedas
- ✅ Configuração por território funcionando
- ✅ Conversão de moedas funcionando (se configurado)
- ✅ Payout em cripto ou fiat funcionando
- ✅ Módulos administrativos funcionando

### Qualidade
- ✅ Cobertura de testes >90% em todos os serviços
- ✅ Testes unitários completos
- ✅ Testes de integração completos
- ✅ Testes de cache completos
- ✅ Testes de rate limiting completos
- ✅ Testes de segurança completos
- ✅ Todos os testes passando
- Considerar **Testcontainers + PostgreSQL** para testes de integração (tradicional + cripto, transações) com banco real — requisito "Testável" (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Documentação
- ✅ Documentação técnica completa
- ✅ Documentação de API atualizada
- ✅ DevPortal atualizado com todas as seções
- ✅ Feature flags documentadas com dependências
- ✅ Documentação de segurança criada

### Segurança
- ✅ Análise de segurança completa realizada
- ✅ Todas as vulnerabilidades identificadas e corrigidas
- ✅ Validação de endereços implementada
- ✅ Verificação de confirmações implementada
- ✅ Proteção contra double spending implementada
- ✅ Proteção de dados sensíveis implementada
- ✅ Rate limiting em endpoints de cripto
- ✅ Módulos administrativos protegidos

---

## 🔗 Dependências

- **Fase 7**: Sistema de Payout completo (para payout em cripto)
- **Fase 6**: Sistema de Pagamentos completo (base para extensão)

---

## 📝 Notas de Implementação

### Feature Flags e Dependências

**Hierarquia de Dependências**:
```
MarketplaceEnabled (base)
  └─> PaymentEnabled (pagamentos tradicionais)
      └─> CryptoPaymentsEnabled (pagamentos em cripto)
          └─> CurrencyConversionEnabled (conversão automática)
```

**Validação de Dependências**:
- Ao habilitar `CryptoPaymentsEnabled`, sistema valida que `PaymentEnabled` está habilitado
- Ao desabilitar `PaymentEnabled`, sistema desabilita automaticamente `CryptoPaymentsEnabled` e `CurrencyConversionEnabled`
- Ao habilitar `CurrencyConversionEnabled`, sistema valida que `CryptoPaymentsEnabled` está habilitado

### Arquitetura Unificada

```
PaymentService
├── PaymentMethodResolver
│   ├── Traditional → IPaymentGateway (Stripe, MercadoPago)
│   └── Crypto → ICryptoPaymentGateway (BitPay, Coinbase)
├── Feature Flag Validation
│   ├── MarketplaceEnabled (obrigatório)
│   ├── PaymentEnabled (obrigatório para pagamentos)
│   └── CryptoPaymentsEnabled (obrigatório para cripto)
├── Mesma lógica de fees
├── Mesma rastreabilidade
└── Mesmo payout (crypto ou fiat)
```

### Fluxo de Pagamento em Cripto

1. **Checkout criado**:
   - Comprador escolhe método: Cripto
   - Sistema valida feature flag `CryptoPaymentsEnabled`
   - Sistema cria `CryptoPayment` com endereço de carteira
   - Sistema gera QR code ou link para pagamento

2. **Comprador paga**:
   - Comprador envia cripto para endereço gerado
   - Provedor (BitPay/Coinbase) detecta pagamento
   - Webhook notifica sistema

3. **Confirmação Blockchain**:
   - Sistema verifica número mínimo de confirmações
   - Sistema valida transação blockchain
   - Sistema protege contra double spending

4. **Checkout marcado como Paid**:
   - Mesma lógica que pagamento tradicional
   - Cria `SellerTransaction`
   - Atualiza `SellerBalance`
   - Cria `FinancialTransaction` para rastreabilidade

5. **Payout**:
   - Vendedor pode receber em cripto (se configurado)
   - Ou converter para fiat automaticamente (se `CurrencyConversionEnabled`)
   - Mesma lógica de retenção e limites

### Segurança Específica para Cripto

1. **Validação de Endereços**:
   - Validar formato (checksum, rede, etc.)
   - Rejeitar endereços de teste em produção
   - Rejeitar endereços de outras redes

2. **Confirmações Blockchain**:
   - Número mínimo configurável por cripto
   - Verificar confirmações antes de marcar como pago
   - Proteção contra reorganizações

3. **Proteção contra Double Spending**:
   - Validar hash de transação único
   - Verificar que transação não foi usada antes
   - Proteção contra replay attacks

4. **Proteção de Dados Sensíveis**:
   - Nunca armazenar chaves privadas
   - Nunca armazenar seeds
   - Apenas endereços públicos

5. **Rate Limiting**:
   - Rate limiting específico para endpoints de cripto
   - Rate limiting por endereço de carteira
   - Rate limiting em módulos administrativos

### Módulos Administrativos

**Acesso**: Apenas SystemAdmin

**Funcionalidades**:
- Configurar provedores globais de cripto
- Visualizar todas as transações em cripto
- Conciliação manual de transações
- Gerenciar cotações de moedas
- Forçar atualização de cotações

### Testes de Cache

**O que testar**:
- Cache de feature flags de cripto
- Cache de configurações de cripto
- Cache de cotações de moedas
- Invalidação quando dados mudam
- TTL configurável
- Fallback quando cache falha
- Performance (hit/miss rates)

### Testes de Rate Limiting

**O que testar**:
- Rate limiting em endpoints de pagamento cripto
- Rate limiting em endpoints de configuração
- Rate limiting em módulos administrativos
- Rate limiting por usuário
- Rate limiting por endereço de carteira
- Headers X-RateLimit-* retornados
- Retorno 429 quando excedido

---

**Status**: ⏳ **FASE 21 PENDENTE**  
**Prioridade**: 🟢 OPCIONAL (Aceitar pagamentos em criptomoedas)
