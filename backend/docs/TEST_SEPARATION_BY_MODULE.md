# Separação de testes por módulo

Este documento avalia **quais testes do Araponga.Tests (Core) são específicos de módulo** e recomenda onde devem ficar: no Core (integração/Platform) ou no projeto de teste do módulo (`Araponga.Tests.Modules.<Nome>`).

---

## 1. Critérios

- **Manter no Core (Araponga.Tests):**
  - Testes de **API** que cruzam vários módulos (ex.: GovernanceIntegrationTests, EndToEndTests).
  - Testes de **serviços Platform** (Auth, Territory, Membership, JoinRequest, FeatureFlag, DataExport, etc.).
  - Testes de **integração** que exigem ApiFactory e vários módulos (ex.: fluxos de feed + mapa + marketplace).
  - Testes de **Infrastructure** compartilhada (UserRepository, MembershipSettings, SystemConfig, TokenService, etc.).
  - Testes de **Domain** do núcleo (User, Territory, Membership, Voting, etc.).

- **Mover para Araponga.Tests.Modules.X:**
  - Testes que exercitam **apenas** serviços/domínio/infra do módulo X e não dependem de helpers pesados do Core (ex.: FeedServiceTestHelper com muitos módulos).
  - Quando o projeto **Araponga.Tests.Modules.X** já existe e tem as referências necessárias (Application, Infrastructure, módulo X).

- **Criar novo Araponga.Tests.Modules.X (opcional):**
  - Quando o módulo tem regras de negócio complexas e a equipe quer "testar só o módulo X" (ex.: Feed, Events, Chat, Alerts). Prioridade baixa; não é obrigatório.

---

## 2. Mapeamento atual (avaliação)

| Módulo | Testes no Core (Araponga.Tests) | Testes no módulo (Tests.Modules.X) | Recomendação |
|--------|---------------------------------|------------------------------------|--------------|
| **Connections** | Api/ConnectionsIntegrationTests (integração HTTP) | Domain + Application (ConnectionService, NotificationFlow) | Manter integração no Core; Domain/Application já no módulo. |
| **Moderation** | Application: DocumentEvidence*, WorkQueue*, Verification*, ReportCreated*, ModerationCase*, TerritoryAssetCuration*, UserBlockCache*; Infrastructure: DocumentEvidenceRepository, WorkItemRepository | Domain (Evidence, WorkItem) | **Mover** testes Application/Infra de Moderation para Tests.Modules.Moderation (reduz concentração no Core). ModerationCaseServiceTests depende de FeedServiceTestHelper → manter no Core ou extrair helper compartilhado. |
| **Marketplace** | Application: Cart*, Store*, Inquiry*, Rating*, PlatformFee*, SellerPayout*, MarketplaceSearch*, MarketplaceService*, TerritoryPayoutConfig*; Api: MarketplaceControllerTests | Domain (Cart, Store, StoreItem, etc.) | **Opcional:** mover testes Application de Marketplace para Tests.Modules.Marketplace (projeto já existe com Domain). Integração API pode ficar no Core. |
| **Map** | Application: MapService*, MapEntityCache*; ApplicationServiceTests (MapService_*) | Domain (MapEntity, MapEntityCategory) | **Opcional:** criar Application em Tests.Modules.Map e mover MapService*; ou manter no Core (poucos arquivos). |
| **Feed** | Application: PostCreation*, PostEdit*, PostFilter*, PostInteraction*, FeedServiceTestHelper; ApplicationServiceTests (FeedService_*, Health*); Api: RequestValidation (GetFeed_*), ControllerIntegration (GetFeed_*) | — | Maioria em ApplicationServiceTests (muitos FeedService_*). **Opcional:** criar Araponga.Tests.Modules.Feed e mover Post* e FeedServiceTestHelper + testes de Feed; exige refactor de helpers. |
| **Events** | Application: EventService*, EventCache*, EventReminder*; ApplicationServiceTests (EventsService_*); Api: EventsControllerTests | — | **Opcional:** criar Tests.Modules.Events se houver necessidade de isolar testes de eventos. |
| **Chat** | Application: ChatService*; Api: ChatScenariosTests | — | **Opcional:** criar Tests.Modules.Chat para ChatService* e cenários. |
| **Alerts** | Application: HealthService*, AlertCache*; ApplicationServiceTests (Health*); Api: AlertsControllerTests | — | **Opcional:** criar Tests.Modules.Alerts. |
| **Subscriptions** | — | Application + Api + Performance (próprio ApiFactory) | Já isolado. |
| **Notifications** | Application: NotificationConfig*, NotificationFlow*, PushNotification* | — | Manter no Core (cross-cutting com Platform). |

---

## 3. Ações priorizadas

1. **Feito:** Testes **Application** do módulo Moderation foram movidos para `Araponga.Tests.Modules.Moderation/Application`. Testes **Application** do módulo Marketplace (Cart, Store, Inquiry, Rating, PlatformFee, SellerPayout, MarketplaceSearch, MarketplaceService, TerritoryPayoutConfig) foram movidos para `Araponga.Tests.Modules.Marketplace/Application`.
2. **Feito:** `CacheTestHelper` e `PatternAwareTestCacheService` extraídos para `Araponga.Tests.Shared/TestHelpers`, permitindo que Core e módulos (Marketplace, etc.) usem o mesmo helper. Core passou a usar o helper compartilhado via `global using Araponga.Tests.Shared.TestHelpers`.
3. **Documentar:** Feito — README dos testes referencia este documento e a convenção “teste 100% do módulo → preferir Tests.Modules.X quando o projeto existir”.
4. **Não obrigatório:** Criar Tests.Modules.Feed, .Events, .Chat, .Alerts só se houver demanda (regras complexas, time dedicado).

---

## 4. Referências

- [README dos testes](../Tests/Araponga.Tests/README.md) — convenções e estrutura.
- [IMPROVEMENTS_AND_KNOWN_ISSUES.md](IMPROVEMENTS_AND_KNOWN_ISSUES.md) — seção 3.1 (assimetria de testes por módulo).
- [ADR-013 / ADR-019](../../docs/10_ARCHITECTURE_DECISIONS.md) — estrutura de testes em níveis.
