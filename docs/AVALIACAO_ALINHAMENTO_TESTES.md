# Avaliação: alinhamento dos testes com a arquitetura (Shared vs módulos)

## Resumo

Os testes **não estavam totalmente alinhados** com a modularização (repositórios core em `InMemorySharedStore` e repositórios de módulo em `InMemoryDataStore`). Várias suítes ainda usavam `InMemoryDataStore` para entidades que hoje vivem em **Shared** (User, Territory, Membership, SystemPermission, Terms, Privacy, UserInterest, Voting, etc.), o que gerava erros de compilação após a migração dos repositórios para `Araponga.Infrastructure.Shared`.

Foi feita uma **correção em lote** para alinhar os testes à regra:

- **`InMemorySharedStore`**: repositórios de agregados core (User, Territory, Membership, JoinRequest, UserPreferences, UserInterest, Voting, Vote, SystemPermission, MembershipSettings, MembershipCapability, SystemConfig, TermsOfService, TermsAcceptance, PrivacyPolicy, PrivacyPolicyAcceptance, UserDevice, TerritoryCharacterization).
- **`InMemoryDataStore`**: repositórios de módulos (Feed, Map, Report, Chat, Events, Marketplace, etc.) e audit/event bus quando usam store.

## O que foi ajustado

1. **ApplicationServiceTests** – `InMemorySystemPermissionRepository` passou a usar `SharedStore`.
2. **MapServiceEdgeCasesTests** – Segundo teste passou a usar `sharedStore` para membership, user, settings, capability, permission.
3. **VotingServiceEdgeCasesTests** – `CreateServiceAndSetupAsync` e `CreateAccessEvaluator` passaram a usar `InMemorySharedStore` (Voting, Vote, Membership e demais shared).
4. **UserInterestServiceTests / UserInterestServiceEdgeCasesTests** – `InMemoryUserInterestRepository` passou a usar `sharedStore`.
5. **TerritoryServiceEdgeCasesTests** – `InMemoryTerritoryRepository` e acesso a territórios passaram a usar `sharedStore`.
6. **VerificationServiceEdgeCasesTests** – `CreateService(dataStore, sharedStore)`; User e Membership usam `sharedStore`; Users/Memberships adicionados em `sharedStore`.
7. **JoinRequestServiceTests** – `EnsureTestUserExists(sharedStore, ...)` e `CreateService(dataStore, sharedStore)` em todos os testes.
8. **MembershipServiceTests** – `CreateService(dataStore, sharedStore)` e repositórios shared (Membership, Settings, Territory) usando `sharedStore`; testes que montam repositório manualmente também atualizados.
9. **MarketplaceServiceTests** – `CreateServicesAsync` e `CreateAccessAsync` recebem `sharedStore`; repositórios shared usam `sharedStore`.
10. **PolicyRequirementServiceEdgeCasesTests** – Terms, Privacy, Membership, Capability, Permission passaram a usar `sharedStore`.
11. **PolicySecurityTests** – Terms, Acceptance e (no teste de PolicyRequirement) Privacy, Membership, Capability, Permission passaram a usar `sharedStore`.
12. **PushNotificationServiceEdgeCasesTests** – `InMemoryUserDeviceRepository(sharedStore)` (UserDevice é core).
13. **PostInteractionServiceEdgeCasesTests** – `CreateService(ds, sharedStore)` e repositórios shared com `sharedStore`.
14. **PostFilterServiceEdgeCasesTests** – Idem.
15. **ReportServiceEdgeCasesTests / ReportServiceTests** – `InMemoryUserRepository(sharedStore)` e, onde aplicável, `CreateService(dataStore, sharedStore)`.
16. **SystemConfigServiceEdgeCasesTests** – `InMemorySystemConfigRepository(sharedStore)`; mantido `InMemoryDataStore` para `InMemoryAuditLogger`.
17. **TermsAcceptanceServiceEdgeCasesTests** – Terms e Acceptance com `sharedStore`.
18. **TerritoryAssetServiceEdgeCasesTests** – `InMemoryTerritoryMembershipRepository(sharedStore)` em todos os testes.

Foi adicionado `using Araponga.Infrastructure.Shared.InMemory` (ou equivalente) onde necessário; `GlobalUsings.cs` do projeto de testes já expõe os tipos de Shared.

## Estado atual

- **Build**: **OK** – `dotnet build backend/Araponga.Tests/Araponga.Tests.csproj` conclui com 0 erros (apenas avisos NU1603 e ASP0019).
- **Testes** (após todas as correções): **2172 passando**, **0 falhando**, **23 ignorados** (ex.: performance em CI), total **2195**. **Test Run Successful.**
  - **Correções de aplicação**: Registro de `InMemorySharedStore` na DI onde repositórios Shared são usados; uso de `sharedStore` para Users/Territories/Memberships/JoinRequests em `JoinRequestServiceEdgeCasesTests`; asserção com `sharedStore.Users` em `NotificationFlowTests`; `MembershipSettings` em `MediaInContentIntegrationTests.BecomeResidentAsync`.
  - **Correções de integração API**: Uso de `GetSharedStore()` em vez de `GetDataStore()` para dados que a API lê do Shared: **GovernanceIntegrationTests** (CreateMembershipAsync, CreateCuratorCapabilityAsync, e todas as leituras de `Votings`); **ChatScenariosTests** (Users para verificação de identidade); **MediaInContentIntegrationTests** (MembershipSettings em BecomeResidentAsync); **VotingPerformanceTests** (CreateMembershipAsync e Votings). Com isso, 401/403 foram eliminados.

## Conclusão

- **Alinhamento com a arquitetura**: os testes estão **alinhados** à separação Shared vs módulos no que diz respeito a:
  - uso de **`InMemorySharedStore`** para repositórios core e **`InMemoryDataStore`** para repositórios de módulo;
  - assinaturas de helpers (`CreateService`, `CreateAccessEvaluator`, etc.) que recebem ambos os stores quando necessário.
- **Próximos passos sugeridos** (para deixar a suíte 100% verde):
  1. Revisar testes de integração que retornam 403 (token, roles, seed de usuários/territórios na API).
  2. Revisar testes de aplicação que falham e garantir que usem IDs existentes no seed de `InMemorySharedStore` (ou que o seed seja estendido para cobrir os cenários).
  3. Opcional: padronizar helpers (por exemplo, um `TestStorePair` com `DataStore` + `SharedStore`) para reduzir duplicação e erros em novos testes.
