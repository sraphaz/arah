# Fase 19: Sistema de Demandas e Ofertas

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Economia local e autonomia comunitária)  
**Depende de**: Fase 6 (Marketplace), Fase 7 (Pagamentos)  
**Integra com**: Fase 20 (Trocas) - pode ser desenvolvido em paralelo  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 31 para Fase 19, priorizada de P1 para P0 (Onda 3: Economia Local)

---

## 🎯 Objetivo

Implementar sistema de **demandas e ofertas** que permite:
- Moradores cadastrarem **demandas** de itens ou serviços que precisam
- Outros moradores/visitantes fazerem **ofertas** para suprir essas demandas
- Negociação entre demandante e ofertante (aceitar, negociar, recusar)
- Integração com sistema de pagamentos para ofertas aceitas
- Visibilidade territorial (demandas podem ser públicas ou apenas para moradores)

**Princípios**:
- ✅ **Economia Local**: Facilita economia local e circular
- ✅ **Bidirectional**: Complementa Marketplace (procura → oferta vs. oferta → procura)
- ✅ **Autonomia Comunitária**: Comunidade resolve suas próprias necessidades
- ✅ **Transparência**: Demandas e ofertas são visíveis (ou para moradores)
- ✅ **Flexibilidade**: Negociação permite ajustes antes de aceitar

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de marketplace (Fase 6) - oferta → procura
- ✅ Sistema de pagamentos (Fase 7)
- ✅ Sistema de inquiries (consultas sobre itens)
- ❌ Não existe sistema de demandas (procura → oferta)
- ❌ Não existe sistema de ofertas para demandas

### Diferenciação de Funcionalidades Existentes

| Funcionalidade | Direção | Foco |
|----------------|---------|------|
| **Marketplace (Fase 6)** | Oferta → Procura | Vendedor oferece, comprador procura |
| **Trocas (Fase 20)** | Troca Direta | Troca de item/serviço por outro |
| **Compra Coletiva (Fase 23)** | Organização Coletiva | Compra em grupo de produtores |
| **Demandas/Ofertas (Fase 31)** | Procura → Oferta | Comprador precisa, vendedor oferece |

---

## 📋 Requisitos Funcionais

### 1. Sistema de Demandas

#### 1.1 Criar Demanda
- ✅ Morador pode criar demanda de item ou serviço
- ✅ Campos obrigatórios:
  - Título da demanda
  - Descrição detalhada
  - Tipo (ITEM, SERVICE)
  - Categoria (opcional)
  - Localização (georreferenciamento)
  - Prazo desejado (opcional)
  - Orçamento estimado (opcional)
- ✅ Campos opcionais:
  - Tags
  - Imagens (até 5)
  - Especificações técnicas
- ✅ Visibilidade: PUBLIC, RESIDENT_ONLY
- ✅ Status: ACTIVE, FULFILLED, CANCELLED, EXPIRED

#### 1.2 Gerenciar Demandas
- ✅ Listar demandas do território (com filtros)
- ✅ Buscar demandas (por texto, categoria, tipo)
- ✅ Visualizar demanda específica
- ✅ Editar demanda (apenas criador, se ACTIVE)
- ✅ Cancelar demanda (apenas criador)
- ✅ Marcar como fulfilled (apenas criador, após aceitar oferta)

#### 1.3 Permissões
- ✅ Apenas moradores verificados podem criar demandas
- ✅ Visitantes podem visualizar demandas públicas
- ✅ Visitantes podem fazer ofertas (se permitido pelo território)

### 2. Sistema de Ofertas

#### 2.1 Criar Oferta
- ✅ Morador/visitante pode fazer oferta para uma demanda
- ✅ Campos obrigatórios:
  - Demanda ID
  - Descrição da oferta
  - Preço proposto
  - Prazo de entrega/prestação
- ✅ Campos opcionais:
  - Condições especiais
  - Imagens (até 3)
  - Disponibilidade
- ✅ Status: PENDING, ACCEPTED, REJECTED, NEGOTIATING, CANCELLED

#### 2.2 Gerenciar Ofertas
- ✅ Listar ofertas de uma demanda
- ✅ Visualizar oferta específica
- ✅ Editar oferta (apenas ofertante, se PENDING ou NEGOTIATING)
- ✅ Cancelar oferta (apenas ofertante)

### 3. Sistema de Negociação

#### 3.1 Ações do Demandante
- ✅ Aceitar oferta (marca oferta como ACCEPTED, cria transação)
- ✅ Rejeitar oferta (marca oferta como REJECTED)
- ✅ Iniciar negociação (marca oferta como NEGOTIATING, permite mensagens)
- ✅ Fazer contraproposta (via mensagens na negociação)

#### 3.2 Ações do Ofertante
- ✅ Responder negociação (via mensagens)
- ✅ Aceitar contraproposta (marca como ACCEPTED)
- ✅ Rejeitar contraproposta (marca como REJECTED)
- ✅ Cancelar oferta (marca como CANCELLED)

#### 3.3 Mensagens de Negociação
- ✅ Chat entre demandante e ofertante
- ✅ Histórico de mensagens na negociação
- ✅ Notificações de novas mensagens

### 4. Integração com Pagamentos

#### 4.1 Processamento de Oferta Aceita
- ✅ Quando oferta é aceita, cria transação de pagamento
- ✅ Integração com sistema de pagamentos (Fase 7)
- ✅ Escrow (se configurado) até entrega/prestação confirmada
- ✅ Payout para ofertante após confirmação

#### 4.2 Fluxo de Pagamento
- ✅ Demandante paga oferta aceita
- ✅ Pagamento fica em escrow (se configurado)
- ✅ Demandante confirma entrega/prestação
- ✅ Pagamento é liberado para ofertante
- ✅ Se houver disputa, sistema de resolução (WorkItem)

### 5. Notificações

#### 5.1 Notificações para Demandante
- ✅ Nova oferta recebida
- ✅ Oferta aceita/rejeitada (se ofertante cancelou)
- ✅ Nova mensagem na negociação
- ✅ Oferta cancelada pelo ofertante

#### 5.2 Notificações para Ofertante
- ✅ Oferta aceita/rejeitada
- ✅ Negociação iniciada
- ✅ Nova mensagem na negociação
- ✅ Contraproposta recebida

### 6. Visibilidade e Filtros

#### 6.1 Filtros de Busca
- ✅ Por tipo (ITEM, SERVICE)
- ✅ Por categoria
- ✅ Por status (ACTIVE, FULFILLED, etc.)
- ✅ Por localização (raio)
- ✅ Por orçamento (faixa de valores)
- ✅ Por prazo (urgente, esta semana, este mês)

#### 6.2 Visibilidade Territorial
- ✅ Demandas PUBLIC: visíveis para todos
- ✅ Demandas RESIDENT_ONLY: visíveis apenas para moradores
- ✅ Ofertas: visíveis apenas para demandante e ofertante

---

## 📋 Tarefas Detalhadas

### Semana 1: Modelo de Domínio e Repositórios

#### 1.1 Modelo de Domínio (16 horas)
- [ ] Criar `Demand` domain model
  - Id, TerritoryId, CreatedBy, Title, Description, Type, Category
  - Location (Latitude, Longitude), Tags, Images
  - Budget, Deadline, Specifications
  - Visibility, Status, CreatedAt, UpdatedAt
- [ ] Criar `DemandOffer` domain model
  - Id, DemandId, OfferedBy, Description, Price, DeliveryTime
  - Conditions, Images, Availability
  - Status, CreatedAt, UpdatedAt
- [ ] Criar `DemandNegotiation` domain model
  - Id, DemandOfferId, Messages (lista de mensagens)
  - Status, CreatedAt, UpdatedAt
- [ ] Criar enums: `DemandType`, `DemandStatus`, `OfferStatus`, `NegotiationStatus`
- [ ] Criar value objects: `DemandLocation`, `DemandBudget`

#### 1.2 Repositórios (12 horas)
- [ ] Criar `IDemandRepository`
  - GetByIdAsync, GetByTerritoryAsync, GetByUserAsync
  - CreateAsync, UpdateAsync, DeleteAsync
- [ ] Criar `IDemandOfferRepository`
  - GetByIdAsync, GetByDemandAsync, GetByUserAsync
  - CreateAsync, UpdateAsync, DeleteAsync
- [ ] Criar `IDemandNegotiationRepository`
  - GetByOfferAsync, CreateAsync, UpdateAsync
- [ ] Implementar repositórios em Infrastructure

#### 1.3 Migrations (4 horas)
- [ ] Criar migration para `DEMAND` table
- [ ] Criar migration para `DEMAND_OFFER` table
- [ ] Criar migration para `DEMAND_NEGOTIATION` table
- [ ] Criar migration para `DEMAND_NEGOTIATION_MESSAGE` table
- [ ] Testar migrations

### Semana 2: Serviços e Lógica de Negócio

#### 2.1 DemandService (20 horas)
- [ ] Criar `IDemandService` e `DemandService`
- [ ] Implementar `CreateDemandAsync`
  - Validar permissões (morador verificado)
  - Validar campos obrigatórios
  - Criar demanda com status ACTIVE
- [ ] Implementar `GetDemandsAsync`
  - Filtros: tipo, categoria, status, localização, orçamento
  - Paginação
  - Respeitar visibilidade (PUBLIC vs RESIDENT_ONLY)
- [ ] Implementar `GetDemandByIdAsync`
  - Validar visibilidade
- [ ] Implementar `UpdateDemandAsync`
  - Apenas criador, apenas se ACTIVE
- [ ] Implementar `CancelDemandAsync`
  - Apenas criador
  - Cancelar ofertas pendentes
- [ ] Implementar `MarkDemandAsFulfilledAsync`
  - Apenas criador, após aceitar oferta

#### 2.2 DemandOfferService (16 horas)
- [ ] Criar `IDemandOfferService` e `DemandOfferService`
- [ ] Implementar `CreateOfferAsync`
  - Validar permissões (morador ou visitante, se permitido)
  - Validar demanda existe e está ACTIVE
  - Validar não há oferta já aceita
  - Criar oferta com status PENDING
- [ ] Implementar `GetOffersByDemandAsync`
  - Apenas demandante e ofertante podem ver ofertas
- [ ] Implementar `GetOfferByIdAsync`
- [ ] Implementar `UpdateOfferAsync`
  - Apenas ofertante, apenas se PENDING ou NEGOTIATING
- [ ] Implementar `CancelOfferAsync`
  - Apenas ofertante

#### 2.3 DemandNegotiationService (12 horas)
- [ ] Criar `IDemandNegotiationService` e `DemandNegotiationService`
- [ ] Implementar `StartNegotiationAsync`
  - Demandante inicia negociação
  - Marca oferta como NEGOTIATING
  - Cria negociação
- [ ] Implementar `SendMessageAsync`
  - Adiciona mensagem à negociação
  - Notifica outro participante
- [ ] Implementar `AcceptOfferAsync`
  - Demandante aceita oferta
  - Marca oferta como ACCEPTED
  - Cria transação de pagamento
- [ ] Implementar `RejectOfferAsync`
  - Demandante rejeita oferta
  - Marca oferta como REJECTED

### Semana 3: API, Integrações e Testes

#### 3.1 API Controllers (16 horas)
- [ ] Criar `DemandsController`
  - POST /api/v1/demands (criar demanda)
  - GET /api/v1/demands (listar demandas)
  - GET /api/v1/demands/{id} (obter demanda)
  - PUT /api/v1/demands/{id} (atualizar demanda)
  - DELETE /api/v1/demands/{id} (cancelar demanda)
  - POST /api/v1/demands/{id}/fulfill (marcar como fulfilled)
- [ ] Criar `DemandOffersController`
  - POST /api/v1/demands/{demandId}/offers (criar oferta)
  - GET /api/v1/demands/{demandId}/offers (listar ofertas)
  - GET /api/v1/demands/{demandId}/offers/{id} (obter oferta)
  - PUT /api/v1/demands/{demandId}/offers/{id} (atualizar oferta)
  - DELETE /api/v1/demands/{demandId}/offers/{id} (cancelar oferta)
- [ ] Criar `DemandNegotiationsController`
  - POST /api/v1/demands/{demandId}/offers/{offerId}/negotiate (iniciar negociação)
  - POST /api/v1/demands/{demandId}/offers/{offerId}/negotiate/messages (enviar mensagem)
  - POST /api/v1/demands/{demandId}/offers/{offerId}/accept (aceitar oferta)
  - POST /api/v1/demands/{demandId}/offers/{offerId}/reject (rejeitar oferta)

#### 3.2 Integração com Pagamentos (12 horas)
- [ ] Integrar com `PaymentService` (Fase 7)
- [ ] Criar transação quando oferta é aceita
- [ ] Implementar escrow (se configurado)
- [ ] Implementar confirmação de entrega/prestação
- [ ] Implementar liberação de pagamento
- [ ] Integrar com `PayoutService` (Fase 7)

#### 3.3 Notificações (8 horas)
- [ ] Notificar demandante quando nova oferta é criada
- [ ] Notificar ofertante quando oferta é aceita/rejeitada
- [ ] Notificar participantes quando nova mensagem na negociação
- [ ] Notificar quando oferta é cancelada

#### 3.4 Testes (16 horas)
- [ ] Testes unitários para `DemandService`
- [ ] Testes unitários para `DemandOfferService`
- [ ] Testes unitários para `DemandNegotiationService`
- [ ] Testes de integração para API
- [ ] Testes de integração com pagamentos
- [ ] Testes de permissões e visibilidade

#### 3.5 Documentação (4 horas)
- [ ] Documentar API no DevPortal
- [ ] Criar exemplos de uso
- [ ] Documentar fluxos de negociação
- [ ] Documentar integração com pagamentos

---

## 🏗️ Arquitetura

### Modelo de Domínio

```
Demand
├── Id
├── TerritoryId
├── CreatedBy (UserId)
├── Title
├── Description
├── Type (ITEM, SERVICE)
├── Category
├── Location (Latitude, Longitude)
├── Tags
├── Images (lista)
├── Budget (opcional)
├── Deadline (opcional)
├── Specifications (opcional)
├── Visibility (PUBLIC, RESIDENT_ONLY)
├── Status (ACTIVE, FULFILLED, CANCELLED, EXPIRED)
├── CreatedAt
└── UpdatedAt

DemandOffer
├── Id
├── DemandId
├── OfferedBy (UserId)
├── Description
├── Price
├── DeliveryTime
├── Conditions (opcional)
├── Images (lista, opcional)
├── Availability (opcional)
├── Status (PENDING, ACCEPTED, REJECTED, NEGOTIATING, CANCELLED)
├── CreatedAt
└── UpdatedAt

DemandNegotiation
├── Id
├── DemandOfferId
├── Messages (lista de mensagens)
│   ├── SentBy (UserId)
│   ├── Content
│   ├── CreatedAt
├── Status (ACTIVE, ACCEPTED, REJECTED, CANCELLED)
├── CreatedAt
└── UpdatedAt
```

### Fluxo Principal

```
1. Morador cria Demanda (ACTIVE)
   ↓
2. Outro morador/visitante faz Oferta (PENDING)
   ↓
3. Demandante pode:
   - Aceitar → Cria transação de pagamento → Oferta (ACCEPTED)
   - Rejeitar → Oferta (REJECTED)
   - Negociar → Oferta (NEGOTIATING) → Mensagens → Aceitar/Rejeitar
   ↓
4. Se aceita:
   - Pagamento processado
   - Entrega/prestação confirmada
   - Pagamento liberado para ofertante
   - Demanda marcada como FULFILLED
```

---

## 🚩 Feature Flags

### Feature Flag: `DEMANDS_ENABLED`

- **Tipo**: Territorial
- **Default**: `false`
- **Descrição**: Habilita/desabilita sistema de demandas no território
- **Comportamento**: Quando desabilitado, endpoints retornam `404`

---

## ✅ Critérios de Sucesso

- ✅ Moradores podem criar demandas de itens/serviços
- ✅ Outros podem fazer ofertas para demandas
- ✅ Sistema de negociação funcional
- ✅ Integração com pagamentos funcionando
- ✅ Notificações enviadas corretamente
- ✅ Visibilidade respeitada (PUBLIC vs RESIDENT_ONLY)
- ✅ Testes com cobertura >90%
- ✅ Documentação completa

---

## 🔗 Referências

- [Fase 6: Marketplace](./FASE6.md)
- [Fase 7: Pagamentos](./FASE7.md)
- [Fase 20: Trocas Comunitárias](./FASE24.md)
- [Análise de Reorganização](../ANALISE_DEMANDAS_OFERTAS_REORGANIZACAO.md)

---

**Status**: ⏳ Pendente  
**Próximos Passos**: Aprovação e início da implementação
