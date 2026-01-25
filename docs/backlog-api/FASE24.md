# Fase 20: Sistema de Trocas Comunitárias

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🟡 ALTA (Economia circular e autonomia comunitária)  
**Depende de**: Fase 6 (Marketplace), Fase 42 (Gamificação), Fase 22 (Moeda Territorial)  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 24 para Fase 20 (Onda 4: Economia Local Completa)

---

## 🎯 Objetivo

Implementar sistema de **trocas comunitárias** que:
- Permite usuários trocarem produtos e serviços sem usar dinheiro
- Facilita economia circular local (troca direta)
- Integra com sistema de moeda territorial (trocas podem usar moeda como complemento)
- Gamifica participação em trocas (Fase 42)
- Organiza trocas comunitárias (eventos de troca)
- Sistema de matching (sugestões de trocas compatíveis)

**Princípios**:
- ✅ **Economia Circular**: Reutilização e troca de recursos
- ✅ **Autonomia Local**: Trocas dentro do território
- ✅ **Transparência**: Todas as trocas são visíveis (ou para moradores)
- ✅ **Gamificação Harmoniosa**: Participação gera contribuições
- ✅ **Flexibilidade**: Trocas diretas ou com complemento em moeda

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de marketplace (Fase 6)
- ✅ Sistema de gamificação (Fase 42)
- ✅ Sistema de moeda territorial (Fase 22)
- ❌ Não existe sistema de trocas
- ❌ Não existe sistema de matching de trocas
- ❌ Não existe sistema de eventos de troca

### Requisitos Funcionais

#### 1. Sistema de Ofertas de Troca
- ✅ Criar oferta de troca (o que oferece, o que procura)
- ✅ Categorias de ofertas (produtos, serviços, conhecimento)
- ✅ Status: ACTIVE, PENDING, COMPLETED, CANCELLED
- ✅ Visibilidade: PUBLIC, RESIDENT_ONLY
- ✅ Complemento em moeda territorial (opcional)

#### 2. Sistema de Propostas de Troca
- ✅ Usuários podem propor troca para uma oferta
- ✅ Negociação entre partes
- ✅ Aceitar/rejeitar proposta
- ✅ Status: PENDING, ACCEPTED, REJECTED, CANCELLED

#### 3. Sistema de Matching
- ✅ Sugerir trocas compatíveis (algoritmo de matching)
- ✅ Baseado em: o que oferece vs o que procura
- ✅ Notificações de matches potenciais
- ✅ Ranking de compatibilidade

#### 4. Sistema de Eventos de Trocas
- ✅ Criar evento de troca comunitária (tipo feira de trocas)
- ✅ Participação de usuários
- ✅ Agenda de eventos de troca
- ✅ Integração com sistema de eventos (Fase existente)

#### 5. Integração com Moeda Territorial
- ✅ Trocas podem ter complemento em moeda territorial
- ✅ Exemplo: "Troco X por Y + 10 moedas territoriais"
- ✅ Pagamento do complemento via carteira (Fase 22)

#### 6. Gamificação
- ✅ Participação em troca gera contribuição
- ✅ Organizar evento de troca gera mais pontos
- ✅ Trocas bem-sucedidas geram mais pontos

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Modelo de Domínio e Ofertas de Troca

#### 20.1 Modelo de Domínio - Trocas
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `TradeCategory`:
  - [ ] `PRODUCT` (produto)
  - [ ] `SERVICE` (serviço)
  - [ ] `KNOWLEDGE` (conhecimento)
  - [ ] `OTHER` (outro)
- [ ] Criar enum `TradeOfferStatus`:
  - [ ] `ACTIVE` (ativa)
  - [ ] `PENDING` (em negociação)
  - [ ] `COMPLETED` (completada)
  - [ ] `CANCELLED` (cancelada)
- [ ] Criar enum `TradeProposalStatus`:
  - [ ] `PENDING` (pendente)
  - [ ] `ACCEPTED` (aceita)
  - [ ] `REJECTED` (rejeitada)
  - [ ] `CANCELLED` (cancelada)
- [ ] Criar modelo `TradeOffer`:
  - [ ] `Id`, `TerritoryId`, `UserId` (quem oferece)
  - [ ] `Title` (string)
  - [ ] `Description?` (nullable)
  - [ ] `Category` (TradeCategory)
  - [ ] `OfferingDescription` (text, o que oferece)
  - [ ] `SeekingDescription` (text, o que procura)
  - [ ] `CurrencyComplement?` (nullable, complemento em moeda territorial)
  - [ ] `Status` (TradeOfferStatus)
  - [ ] `Visibility` (PUBLIC, RESIDENT_ONLY)
  - [ ] `LocationLat?` (nullable)
  - [ ] `LocationLng?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `TradeProposal`:
  - [ ] `Id`, `TradeOfferId`, `ProposerUserId` (quem propõe)
  - [ ] `Message?` (nullable, mensagem da proposta)
  - [ ] `ProposedOffering` (text, o que propõe oferecer)
  - [ ] `CurrencyComplement?` (nullable, complemento em moeda)
  - [ ] `Status` (TradeProposalStatus)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `Trade`:
  - [ ] `Id`, `TradeOfferId`, `TradeProposalId`
  - [ ] `OffererUserId`, `ProposerUserId`
  - [ ] `Status` (PENDING, CONFIRMED, COMPLETED, CANCELLED)
  - [ ] `CurrencyComplement?` (nullable)
  - [ ] `CompletedAtUtc?` (nullable)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `TradeEvent`:
  - [ ] `Id`, `TerritoryId`, `OrganizerUserId`
  - [ ] `Title` (string)
  - [ ] `Description?` (nullable)
  - [ ] `EventDate` (DateTime)
  - [ ] `LocationLat`, `LocationLng`
  - [ ] `Status` (PLANNED, IN_PROGRESS, COMPLETED, CANCELLED)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `TradeEventParticipation`:
  - [ ] `Id`, `TradeEventId`, `UserId`
  - [ ] `WillBringItems` (bool)
  - [ ] `ItemsDescription?` (nullable)
  - [ ] `JoinedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Trades/TradeOffer.cs`
- `backend/Araponga.Domain/Trades/TradeCategory.cs`
- `backend/Araponga.Domain/Trades/TradeOfferStatus.cs`
- `backend/Araponga.Domain/Trades/TradeProposal.cs`
- `backend/Araponga.Domain/Trades/TradeProposalStatus.cs`
- `backend/Araponga.Domain/Trades/Trade.cs`
- `backend/Araponga.Domain/Trades/TradeStatus.cs`
- `backend/Araponga.Domain/Trades/TradeEvent.cs`
- `backend/Araponga.Domain/Trades/TradeEventParticipation.cs`
- `backend/Araponga.Application/Interfaces/ITradeOfferRepository.cs`
- `backend/Araponga.Application/Interfaces/ITradeProposalRepository.cs`
- `backend/Araponga.Application/Interfaces/ITradeRepository.cs`
- `backend/Araponga.Application/Interfaces/ITradeEventRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTradeOfferRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTradeProposalRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTradeRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTradeEventRepository.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Testes de repositório passando

---

### Semana 2: Sistema de Ofertas e Propostas

#### 20.2 Sistema de Ofertas de Troca
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TradeOfferService`:
  - [ ] `CreateOfferAsync(Guid territoryId, Guid userId, ...)` → criar oferta
  - [ ] `ListOffersAsync(Guid territoryId, ...)` → listar ofertas
  - [ ] `GetOfferAsync(Guid offerId)` → obter oferta
  - [ ] `UpdateOfferAsync(Guid offerId, ...)` → atualizar oferta
  - [ ] `CancelOfferAsync(Guid offerId, Guid userId)` → cancelar oferta
  - [ ] `CompleteOfferAsync(Guid offerId, Guid userId)` → completar oferta
- [ ] Criar `TradeOfferController`:
  - [ ] `POST /api/v1/trade-offers` → criar oferta
  - [ ] `GET /api/v1/trade-offers` → listar ofertas
  - [ ] `GET /api/v1/trade-offers/{id}` → obter oferta
  - [ ] `PATCH /api/v1/trade-offers/{id}` → atualizar oferta
  - [ ] `POST /api/v1/trade-offers/{id}/cancel` → cancelar oferta
  - [ ] `POST /api/v1/trade-offers/{id}/complete` → completar oferta
- [ ] Feature flags: `TradesEnabled`, `TradeOffersPublic`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TradeOfferService.cs`
- `backend/Araponga.Api/Controllers/TradeOfferController.cs`
- `backend/Araponga.Api/Contracts/Trades/CreateTradeOfferRequest.cs`
- `backend/Araponga.Api/Contracts/Trades/TradeOfferResponse.cs`
- `backend/Araponga.Api/Validators/CreateTradeOfferRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Sistema de ofertas funcionando
- ✅ API funcionando
- ✅ Testes passando

---

#### 20.3 Sistema de Propostas de Troca
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TradeProposalService`:
  - [ ] `CreateProposalAsync(Guid offerId, Guid proposerUserId, ...)` → criar proposta
  - [ ] `ListProposalsAsync(Guid offerId, ...)` → listar propostas
  - [ ] `GetProposalAsync(Guid proposalId)` → obter proposta
  - [ ] `AcceptProposalAsync(Guid proposalId, Guid offererUserId)` → aceitar proposta
  - [ ] `RejectProposalAsync(Guid proposalId, Guid offererUserId)` → rejeitar proposta
  - [ ] `CancelProposalAsync(Guid proposalId, Guid proposerUserId)` → cancelar proposta
- [ ] Lógica de criação de troca:
  - [ ] Quando proposta é aceita, criar `Trade`
  - [ ] Notificar ambas as partes
  - [ ] Processar complemento em moeda (se houver)
- [ ] Criar `TradeProposalController`:
  - [ ] `POST /api/v1/trade-offers/{offerId}/proposals` → criar proposta
  - [ ] `GET /api/v1/trade-offers/{offerId}/proposals` → listar propostas
  - [ ] `GET /api/v1/trade-proposals/{id}` → obter proposta
  - [ ] `POST /api/v1/trade-proposals/{id}/accept` → aceitar proposta
  - [ ] `POST /api/v1/trade-proposals/{id}/reject` → rejeitar proposta
  - [ ] `DELETE /api/v1/trade-proposals/{id}` → cancelar proposta
- [ ] Feature flags: `TradeProposalsEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TradeProposalService.cs`
- `backend/Araponga.Api/Controllers/TradeProposalController.cs`
- `backend/Araponga.Api/Contracts/Trades/CreateTradeProposalRequest.cs`
- `backend/Araponga.Api/Contracts/Trades/TradeProposalResponse.cs`
- `backend/Araponga.Api/Validators/CreateTradeProposalRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Sistema de propostas funcionando
- ✅ Criação de troca funcionando
- ✅ API funcionando
- ✅ Testes passando

---

### Semana 3: Matching e Eventos de Trocas

#### 20.4 Sistema de Matching de Trocas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TradeMatchingService`:
  - [ ] `FindMatchesAsync(Guid offerId, ...)` → encontrar matches
  - [ ] `CalculateCompatibilityScoreAsync(Guid offerId, Guid otherOfferId)` → calcular score
  - [ ] `SuggestMatchesAsync(Guid userId, ...)` → sugerir matches para usuário
- [ ] Algoritmo de matching:
  - [ ] Comparar "o que oferece" vs "o que procura"
  - [ ] Considerar categorias
  - [ ] Considerar localização (proximidade)
  - [ ] Considerar histórico de trocas
  - [ ] Score de compatibilidade (0-100)
- [ ] Notificações de matches:
  - [ ] Notificar quando novo match é encontrado
  - [ ] Notificar quando match tem alta compatibilidade
- [ ] Criar `TradeMatchingController`:
  - [ ] `GET /api/v1/trade-offers/{id}/matches` → encontrar matches
  - [ ] `GET /api/v1/trades/suggestions` → sugestões de matches
- [ ] Feature flags: `TradeMatchingEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TradeMatchingService.cs`
- `backend/Araponga.Api/Controllers/TradeMatchingController.cs`
- `backend/Araponga.Api/Contracts/Trades/TradeMatchResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de matching funcionando
- ✅ Algoritmo de compatibilidade funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 20.5 Sistema de Eventos de Trocas
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `TradeEventService`:
  - [ ] `CreateEventAsync(Guid territoryId, Guid organizerUserId, ...)` → criar evento
  - [ ] `ListEventsAsync(Guid territoryId, ...)` → listar eventos
  - [ ] `GetEventAsync(Guid eventId)` → obter evento
  - [ ] `JoinEventAsync(Guid eventId, Guid userId, ...)` → participar do evento
  - [ ] `ListParticipantsAsync(Guid eventId)` → listar participantes
- [ ] Integrar com sistema de eventos existente (Fase existente):
  - [ ] Eventos de troca aparecem na lista de eventos
  - [ ] Integração com sistema de notificações
- [ ] Criar `TradeEventController`:
  - [ ] `POST /api/v1/trade-events` → criar evento
  - [ ] `GET /api/v1/trade-events` → listar eventos
  - [ ] `GET /api/v1/trade-events/{id}` → obter evento
  - [ ] `POST /api/v1/trade-events/{id}/join` → participar
  - [ ] `GET /api/v1/trade-events/{id}/participants` → listar participantes
- [ ] Feature flags: `TradeEventsEnabled`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TradeEventService.cs`
- `backend/Araponga.Api/Controllers/TradeEventController.cs`
- `backend/Araponga.Api/Contracts/Trades/CreateTradeEventRequest.cs`
- `backend/Araponga.Api/Contracts/Trades/TradeEventResponse.cs`

**Critérios de Sucesso**:
- ✅ Sistema de eventos funcionando
- ✅ Integração com eventos existente funcionando
- ✅ Testes passando

---

### Semana 3: Integrações

#### 20.6 Integração com Moeda Territorial e Gamificação
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar com `WalletService` (Fase 22):
  - [ ] Processar complemento em moeda territorial
  - [ ] Transferência de moeda quando proposta é aceita
  - [ ] Reembolso se troca é cancelada
- [ ] Integrar com `ContributionService` (Fase 42):
  - [ ] Participação em troca gera contribuição
  - [ ] Organizar evento de troca gera mais pontos
  - [ ] Trocas bem-sucedidas geram mais pontos
- [ ] Criar `TradeService`:
  - [ ] `ConfirmTradeAsync(Guid tradeId, Guid userId)` → confirmar troca
  - [ ] `CompleteTradeAsync(Guid tradeId, Guid userId)` → completar troca
  - [ ] `CancelTradeAsync(Guid tradeId, Guid userId, string reason)` → cancelar troca
- [ ] Criar `TradeController`:
  - [ ] `GET /api/v1/trades` → listar trocas
  - [ ] `GET /api/v1/trades/{id}` → obter troca
  - [ ] `POST /api/v1/trades/{id}/confirm` → confirmar troca
  - [ ] `POST /api/v1/trades/{id}/complete` → completar troca
  - [ ] `POST /api/v1/trades/{id}/cancel` → cancelar troca
- [ ] Feature flags: `TradesTerritoryCurrencyEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/TradeService.cs`
- `backend/Araponga.Api/Controllers/TradeController.cs`
- `backend/Araponga.Api/Contracts/Trades/TradeResponse.cs`

**Critérios de Sucesso**:
- ✅ Integração com moeda territorial funcionando
- ✅ Integração com gamificação funcionando
- ✅ Sistema de trocas funcionando
- ✅ Testes passando

---

## 📊 Resumo da Fase 20

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Ofertas | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Propostas | 24h | ❌ Pendente | 🔴 Alta |
| Sistema de Matching | 16h | ❌ Pendente | 🟡 Média |
| Sistema de Eventos | 16h | ❌ Pendente | 🟡 Média |
| Integrações | 16h | ❌ Pendente | 🔴 Alta |
| **Total** | **120h (21 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 20

### Funcionalidades
- ✅ Sistema completo de ofertas de troca funcionando
- ✅ Sistema de propostas funcionando
- ✅ Sistema de matching funcionando
- ✅ Sistema de eventos de troca funcionando
- ✅ Integração com moeda territorial funcionando
- ✅ Integração com gamificação funcionando

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Feature flags implementados
- ✅ Validações e segurança implementadas
- Considerar **Testcontainers + PostgreSQL** para testes de integração (trocas, matching, eventos) com banco real (estratégia na Fase 19; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração com Fase 6 (Marketplace) funcionando
- ✅ Integração com Fase 42 (Gamificação) funcionando
- ✅ Integração com Fase 22 (Moeda Territorial) funcionando
- ✅ Integração com sistema de eventos existente funcionando

---

## 🔗 Dependências

- **Fase 6**: Marketplace (base para produtos/serviços)
- **Fase 42**: Gamificação (contribuições por trocas)
- **Fase 22**: Moeda Territorial (complemento em moeda)

---

## 📝 Notas de Implementação

### Fluxo de Troca

1. **Usuário cria oferta de troca**
   - Define o que oferece
   - Define o que procura
   - Opcional: complemento em moeda territorial

2. **Sistema sugere matches**
   - Algoritmo encontra ofertas compatíveis
   - Notifica usuário sobre matches

3. **Usuário propõe troca**
   - Cria proposta para uma oferta
   - Define o que oferece em troca
   - Opcional: complemento em moeda

4. **Ofertante aceita/rejeita**
   - Se aceita, cria `Trade`
   - Processa complemento em moeda (se houver)

5. **Troca é confirmada e completada**
   - Ambas as partes confirmam
   - Troca é marcada como completada
   - Gamificação gera contribuições

### Algoritmo de Matching

**Fatores de Compatibilidade**:
- Categoria (produto, serviço, conhecimento)
- Descrição (similaridade textual)
- Localização (proximidade)
- Histórico de trocas (reputação)
- Score final: 0-100

**Exemplo**:
- Oferta A: "Ofereço: hortaliças | Procuro: frutas"
- Oferta B: "Ofereço: frutas | Procuro: hortaliças"
- Score: 95 (match perfeito)

### Eventos de Trocas

**Tipo de Evento**:
- Feira de trocas comunitária
- Trocas organizadas em local específico
- Data e hora definidas
- Participantes trazem itens para trocar

**Integração**:
- Aparece na lista de eventos do território
- Notificações para participantes
- Gamificação de participação

---

**Status**: ⏳ **FASE 20 PENDENTE**  
**Depende de**: Fases 6, 42, 22  
**Crítico para**: Economia Circular e Autonomia Comunitária  
**Nota**: Esta fase foi renumerada de Fase 24 para Fase 20 conforme reorganização estratégica
