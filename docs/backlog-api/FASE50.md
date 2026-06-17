# Fase 50: Busca de Babás (Babysitters) - Serviços Territoriais

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🟡 MÉDIA (Funcionalidade de valor agregado para comunidade territorial)  
**Depende de**: Marketplace (Fase 6), Sistema de Pagamentos (Fase 6), Chat (Fase 8), Verificação de Identidade (Epic 1), Sistema de Avaliações  
**Estimativa Total**: 112 horas  
**Status**: ⏳ Pendente  
**Categoria**: Serviços Territoriais / Marketplace de Serviços

---

## 🎯 Objetivo

Implementar funcionalidade completa de busca e contratação de babás (babysitters) dentro do território, permitindo que:
- **Famílias** encontrem babás disponíveis próximas, com filtros por experiência, disponibilidade, preço e avaliações
- **Babás** se cadastrem e ofereçam seus serviços com perfil completo, disponibilidade e histórico
- **Sistema** gerencie todo o ciclo: busca, contato, negociação, contratação, pagamento e avaliação

**Princípios**:
- ✅ **Segurança**: Verificação de identidade obrigatória para babás
- ✅ **Confiança**: Sistema de avaliações e recomendações transparente
- ✅ **Territorialidade**: Serviços vinculados ao território (proximidade e comunidade)
- ✅ **Integração**: Reutiliza Marketplace, Chat, Pagamentos e Verificação existentes

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Marketplace completo (Stores, Items, Cart, Checkout)
- ✅ Sistema de pagamentos (Mercado Pago, Stripe)
- ✅ Chat territorial (canais, grupos, DM)
- ✅ Verificação de identidade e residência
- ✅ Sistema de avaliações (RatingService no Marketplace)
- ❌ Busca de serviços profissionais não existe
- ❌ Perfis de prestadores de serviço não existem
- ❌ Sistema de disponibilidade/calendário não existe

### Requisitos Funcionais

#### 1. Cadastro de Babá
- ✅ Usuário pode criar perfil de babá no território
- ✅ Formulário completo com:
  - Informações pessoais (nome, foto, descrição)
  - Experiência (anos de experiência, número de crianças já cuidadas, faixas etárias atendidas: bebês, crianças pequenas, adolescentes)
  - Certificações (primeiros socorros, curso de babá, outros cursos relevantes)
  - Disponibilidade (horários semanais, dias da semana, disponibilidade imediata/emergência)
  - Preço/hora (valor por hora de serviço)
  - Área de atuação (raio em km a partir de localização)
  - Referências (opcional: contatos de famílias anteriores)
  - Múltiplos territórios onde atua
- ✅ Verificação de identidade obrigatória (mesmo sistema de verificação de resident)
- ✅ Status do perfil: `Pending` (aguardando verificação), `Active`, `Inactive`, `Suspended` (por moderação)

#### 2. Busca de Babás
- ✅ Busca por território (território atual do usuário)
- ✅ Filtros disponíveis:
  - Disponibilidade (horários específicos, dias da semana, disponibilidade imediata)
  - Experiência (anos mínimos, faixas etárias atendidas)
  - Localização (raio em km a partir de ponto específico)
  - Avaliação mínima (ex: 4+ estrelas)
  - Preço/hora (faixa de valores)
  - Verificada (apenas babás com identidade verificada)
- ✅ Ordenação: relevância (avaliações + proximidade), avaliação, preço (menor/maior), distância
- ✅ Resultados mostram: foto, nome, avaliação média, número de avaliações, preço/hora, disponibilidade próxima, distância, badges (verificada, certificações)

#### 3. Perfil Detalhado de Babá
- ✅ Informações completas do cadastro
- ✅ Galeria de fotos (múltiplas fotos do perfil)
- ✅ Avaliações e comentários públicos de contratantes anteriores
- ✅ Calendário de disponibilidade (visualização de horários livres/ocupados)
- ✅ Histórico de serviços (quantidade total, tipos de serviços realizados)
- ✅ Badges/verificações:
  - Identidade verificada
  - Certificações (primeiros socorros, curso de babá, etc.)
  - Membro do território há X tempo
  - Babá premium (se houver plano de assinatura)
- ✅ Botão de contato (abre chat ou mostra telefone se permitido)

#### 4. Sistema de Avaliações
- ✅ Contratante pode avaliar babá após serviço concluído
- ✅ Avaliação inclui: nota (1-5 estrelas), comentário opcional, tags (pontualidade, cuidado, comunicação, etc.)
- ✅ Avaliações são públicas e vinculadas ao perfil da babá
- ✅ Sistema calcula: média de avaliações, número total, distribuição por nota
- ✅ Babá pode responder avaliações
- ✅ Sistema de recomendações: babás bem avaliadas aparecem primeiro nos resultados

#### 5. Solicitação e Contratação
- ✅ Contratante pode solicitar serviço diretamente do perfil da babá
- ✅ Solicitação inclui:
  - Data/hora de início
  - Duração estimada
  - Número e idades das crianças
  - Necessidades especiais (alergias, medicamentos, rotinas)
  - Localização (endereço ou ponto no mapa)
  - Observações adicionais
- ✅ Babá recebe notificação de nova solicitação
- ✅ Babá pode: aceitar, recusar, fazer contraproposta (data/hora/preço)
- ✅ Chat integrado para negociar detalhes antes da confirmação
- ✅ Após aceite, serviço fica confirmado e aparece no calendário de ambos
- ✅ Status do serviço: `Pending`, `Accepted`, `InProgress`, `Completed`, `Cancelled`

#### 6. Integração com Pagamentos
- ✅ Contratante pode pagar via plataforma (integração com Marketplace/pagamentos)
- ✅ Opções de pagamento:
  - Antecipado (pagamento antes do serviço)
  - Após serviço (pagamento após confirmação de conclusão)
- ✅ Sistema registra transação e libera pagamento após confirmação
- ✅ Taxa de plataforma aplicável (configurável por território)
- ✅ Histórico de pagamentos visível para ambos

#### 7. Notificações
- ✅ Babá recebe notificação de nova solicitação
- ✅ Contratante recebe notificação de aceite/recusa/contraproposta
- ✅ Ambos recebem lembretes antes do serviço (ex: 24h antes)
- ✅ Notificação de conclusão de serviço (para avaliação)

---

## 📋 Tarefas Detalhadas

### Semana 1: Modelo de Dados e Cadastro

#### 50.1 Modelo de Dados - Babá Profile
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar entidade `BabysitterProfile` no Domain:
  - [ ] `Id`, `UserId`, `TerritoryId` (múltiplos territórios via `BabysitterTerritory`)
  - [ ] `Status` (Pending, Active, Inactive, Suspended)
  - [ ] `ExperienceYears`, `ChildrenCaredForCount`
  - [ ] `AgeRanges` (enum: Babies, Toddlers, Preschoolers, SchoolAge, Teens)
  - [ ] `HourlyRate`, `ServiceRadiusKm`
  - [ ] `Description`, `Bio`
  - [ ] `CreatedAt`, `UpdatedAt`, `VerifiedAt`
- [ ] Criar entidade `BabysitterCertification`:
  - [ ] `BabysitterProfileId`, `CertificationType` (FirstAid, BabysitterCourse, Other), `IssuedBy`, `IssuedAt`, `ExpiresAt` (opcional)
- [ ] Criar entidade `BabysitterAvailability`:
  - [ ] `BabysitterProfileId`, `DayOfWeek`, `StartTime`, `EndTime`, `IsAvailable`
- [ ] Criar entidade `BabysitterTerritory`:
  - [ ] `BabysitterProfileId`, `TerritoryId`, `IsPrimary`
- [ ] Criar entidade `BabysitterReference`:
  - [ ] `BabysitterProfileId`, `ReferenceName`, `ReferenceContact`, `Relationship`, `Notes`
- [ ] Criar entidade `BabysitterService`:
  - [ ] `Id`, `BabysitterProfileId`, `RequesterUserId`, `TerritoryId`
  - [ ] `StartDateTime`, `DurationHours`, `NumberOfChildren`, `ChildrenAges`
  - [ ] `SpecialNeeds`, `Location`, `Status` (Pending, Accepted, InProgress, Completed, Cancelled)
  - [ ] `RequestedAt`, `AcceptedAt`, `CompletedAt`, `CancelledAt`
- [ ] Criar entidade `BabysitterReview`:
  - [ ] `Id`, `BabysitterProfileId`, `ServiceId`, `ReviewerUserId`
  - [ ] `Rating` (1-5), `Comment`, `Tags` (array: Punctuality, Care, Communication, etc.)
  - [ ] `CreatedAt`, `Response` (resposta da babá), `ResponseAt`
- [ ] Criar repositórios: `IBabysitterProfileRepository`, `IBabysitterServiceRepository`, `IBabysitterReviewRepository`
- [ ] Criar migrations PostgreSQL
- [ ] Testes unitários de domínio

**Arquivos a Criar**:
- `backend/Arah.Domain/Services/Babysitter/BabysitterProfile.cs`
- `backend/Arah.Domain/Services/Babysitter/BabysitterCertification.cs`
- `backend/Arah.Domain/Services/Babysitter/BabysitterAvailability.cs`
- `backend/Arah.Domain/Services/Babysitter/BabysitterTerritory.cs`
- `backend/Arah.Domain/Services/Babysitter/BabysitterReference.cs`
- `backend/Arah.Domain/Services/Babysitter/BabysitterService.cs`
- `backend/Arah.Domain/Services/Babysitter/BabysitterReview.cs`
- `backend/Arah.Application/Interfaces/IBabysitterProfileRepository.cs`
- `backend/Arah.Application/Interfaces/IBabysitterServiceRepository.cs`
- `backend/Arah.Application/Interfaces/IBabysitterReviewRepository.cs`
- `backend/Arah.Infrastructure/Postgres/Entities/BabysitterProfileRecord.cs` (+ outros records)
- `backend/Arah.Infrastructure/Postgres/Repositories/PostgresBabysitterProfileRepository.cs` (+ outros repos)

#### 50.2 Cadastro de Babá - Backend
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `BabysitterService` em Application:
  - [ ] `CreateProfileAsync`: cria perfil de babá (requer verificação de identidade)
  - [ ] `UpdateProfileAsync`: atualiza informações do perfil
  - [ ] `SetAvailabilityAsync`: define disponibilidade semanal
  - [ ] `AddCertificationAsync`: adiciona certificação
  - [ ] `AddReferenceAsync`: adiciona referência
  - [ ] `ActivateProfileAsync`: ativa perfil (após verificação)
  - [ ] `DeactivateProfileAsync`: desativa perfil
- [ ] Criar `BabysitterController`:
  - [ ] `POST /api/v1/babysitters`: criar perfil
  - [ ] `PUT /api/v1/babysitters/{id}`: atualizar perfil
  - [ ] `GET /api/v1/babysitters/me`: meu perfil de babá
  - [ ] `POST /api/v1/babysitters/{id}/availability`: definir disponibilidade
  - [ ] `POST /api/v1/babysitters/{id}/certifications`: adicionar certificação
- [ ] Criar contratos (DTOs):
  - [ ] `CreateBabysitterProfileRequest`
  - [ ] `UpdateBabysitterProfileRequest`
  - [ ] `BabysitterProfileResponse`
  - [ ] `SetAvailabilityRequest`
  - [ ] `AddCertificationRequest`
- [ ] Validações (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Babysitter/BabysitterService.cs`
- `backend/Arah.Api/Controllers/Babysitter/BabysitterController.cs`
- `backend/Arah.Api/Contracts/Babysitter/*.cs`

### Semana 2: Busca e Perfil Detalhado

#### 50.3 Busca de Babás - Backend
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `BabysitterSearchService`:
  - [ ] `SearchAsync`: busca babás com filtros
  - [ ] Filtros: território, disponibilidade, experiência, localização (raio), avaliação mínima, preço, verificação
  - [ ] Ordenação: relevância, avaliação, preço, distância
  - [ ] Paginação
- [ ] Criar endpoint `GET /api/v1/babysitters/search`:
  - [ ] Query params: `territoryId`, `availableFrom`, `availableTo`, `dayOfWeek`, `minExperienceYears`, `ageRanges[]`, `latitude`, `longitude`, `radiusKm`, `minRating`, `maxHourlyRate`, `minHourlyRate`, `verifiedOnly`, `pageNumber`, `pageSize`, `sortBy`
  - [ ] Retorna lista paginada de `BabysitterSearchResult`
- [ ] Criar endpoint `GET /api/v1/babysitters/{id}`: perfil detalhado
- [ ] Criar endpoint `GET /api/v1/babysitters/{id}/reviews`: avaliações da babá
- [ ] Criar endpoint `GET /api/v1/babysitters/{id}/availability`: calendário de disponibilidade
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Babysitter/BabysitterSearchService.cs`
- `backend/Arah.Api/Contracts/Babysitter/BabysitterSearchRequest.cs`
- `backend/Arah.Api/Contracts/Babysitter/BabysitterSearchResult.cs`
- `backend/Arah.Api/Contracts/Babysitter/BabysitterDetailResponse.cs`

#### 50.4 Sistema de Avaliações - Backend
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `BabysitterReviewService`:
  - [ ] `CreateReviewAsync`: criar avaliação (após serviço concluído)
  - [ ] `GetReviewsAsync`: listar avaliações de uma babá
  - [ ] `RespondToReviewAsync`: babá responde avaliação
  - [ ] `CalculateRatingAsync`: calcular média e estatísticas
- [ ] Criar endpoints:
  - [ ] `POST /api/v1/babysitters/{id}/reviews`: criar avaliação
  - [ ] `GET /api/v1/babysitters/{id}/reviews`: listar avaliações
  - [ ] `POST /api/v1/babysitters/reviews/{reviewId}/respond`: responder avaliação
- [ ] Integrar com `RatingService` existente (Marketplace) se possível
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Babysitter/BabysitterReviewService.cs`
- `backend/Arah.Api/Contracts/Babysitter/CreateReviewRequest.cs`
- `backend/Arah.Api/Contracts/Babysitter/ReviewResponse.cs`

### Semana 3: Solicitação e Contratação

#### 50.5 Solicitação de Serviço - Backend
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `BabysitterServiceRequestService`:
  - [ ] `CreateServiceRequestAsync`: criar solicitação de serviço
  - [ ] `AcceptServiceRequestAsync`: babá aceita solicitação
  - [ ] `RejectServiceRequestAsync`: babá recusa solicitação
  - [ ] `CounterOfferServiceRequestAsync`: babá faz contraproposta
  - [ ] `CancelServiceRequestAsync`: cancelar solicitação (antes de iniciar)
  - [ ] `CompleteServiceAsync`: marcar serviço como concluído
- [ ] Criar endpoints:
  - [ ] `POST /api/v1/babysitters/{id}/service-requests`: solicitar serviço
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/accept`: aceitar
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/reject`: recusar
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/counter-offer`: contraproposta
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/cancel`: cancelar
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/complete`: concluir
  - [ ] `GET /api/v1/babysitters/service-requests/me`: minhas solicitações (babá e contratante)
- [ ] Integrar com Chat (criar conversa quando solicitação é criada)
- [ ] Notificações: nova solicitação, aceite/recusa, contraproposta, lembretes
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Babysitter/BabysitterServiceRequestService.cs`
- `backend/Arah.Api/Contracts/Babysitter/CreateServiceRequestRequest.cs`
- `backend/Arah.Api/Contracts/Babysitter/ServiceRequestResponse.cs`
- `backend/Arah.Api/Contracts/Babysitter/CounterOfferRequest.cs`

#### 50.6 Integração com Pagamentos
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar com `CartService` e `CheckoutService` existentes:
  - [ ] Criar `StoreItem` virtual para serviço de babá (ou usar tipo especial)
  - [ ] Criar checkout específico para serviço de babá
- [ ] Criar `BabysitterPaymentService`:
  - [ ] `CreatePaymentAsync`: criar pagamento (antecipado ou agendado)
  - [ ] `ReleasePaymentAsync`: liberar pagamento após conclusão
  - [ ] `RefundPaymentAsync`: reembolsar em caso de cancelamento
- [ ] Criar endpoints:
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/pay`: pagar serviço
  - [ ] `POST /api/v1/babysitters/service-requests/{requestId}/release-payment`: liberar pagamento
- [ ] Taxa de plataforma configurável por território
- [ ] Histórico de pagamentos
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Babysitter/BabysitterPaymentService.cs`
- `backend/Arah.Api/Contracts/Babysitter/CreatePaymentRequest.cs`
- `backend/Arah.Api/Contracts/Babysitter/PaymentResponse.cs`

### Semana 4: Frontend e Testes

#### 50.7 Frontend Flutter - Busca e Perfil
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar feature `babysitters` no Flutter:
  - [ ] `lib/features/babysitters/data/repositories/babysitters_repository.dart`
  - [ ] `lib/features/babysitters/data/models/babysitter_profile.dart`
  - [ ] `lib/features/babysitters/presentation/providers/babysitters_provider.dart`
  - [ ] `lib/features/babysitters/presentation/screens/babysitters_search_screen.dart`
  - [ ] `lib/features/babysitters/presentation/screens/babysitter_detail_screen.dart`
  - [ ] `lib/features/babysitters/presentation/widgets/babysitter_card.dart`
- [ ] Tela de busca:
  - [ ] Filtros (disponibilidade, experiência, preço, avaliação)
  - [ ] Lista de resultados com cards
  - [ ] Ordenação
  - [ ] Paginação infinita
- [ ] Tela de perfil detalhado:
  - [ ] Informações completas
  - [ ] Galeria de fotos
  - [ ] Avaliações
  - [ ] Calendário de disponibilidade
  - [ ] Botão "Solicitar Serviço"
- [ ] Testes de widget

**Arquivos a Criar**:
- `frontend/arah.app/lib/features/babysitters/**/*.dart`

#### 50.8 Frontend Flutter - Cadastro e Gestão
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Tela de cadastro de babá:
  - [ ] Formulário completo (experiência, certificações, disponibilidade, preço)
  - [ ] Upload de fotos
  - [ ] Validações
- [ ] Tela de gestão de perfil (babá):
  - [ ] Editar informações
  - [ ] Gerenciar disponibilidade (calendário)
  - [ ] Adicionar certificações
  - [ ] Ver solicitações recebidas
  - [ ] Histórico de serviços
- [ ] Tela de solicitações (contratante):
  - [ ] Lista de solicitações enviadas
  - [ ] Status de cada solicitação
  - [ ] Chat integrado
- [ ] Testes de widget

#### 50.9 Testes e Documentação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos (backend)
- [ ] Testes E2E (fluxo completo: cadastro → busca → solicitação → pagamento → avaliação)
- [ ] Documentação Swagger atualizada
- [ ] Documentação de uso (README)
- [ ] Atualizar roadmap e backlog

---

## 📊 Métricas de Sucesso

- ✅ Cadastro de babá funcional com verificação de identidade
- ✅ Busca retorna resultados relevantes com filtros funcionando
- ✅ Solicitação e contratação funcionam end-to-end
- ✅ Pagamentos integrados com Marketplace
- ✅ Sistema de avaliações operacional
- ✅ Notificações enviadas corretamente
- ✅ Cobertura de testes >80%

---

## 🔗 Dependências e Integrações

- **Marketplace** (Fase 6): reutilizar estrutura de Stores/Items para serviços
- **Sistema de Pagamentos** (Fase 6): integração com Cart/Checkout
- **Chat** (Fase 8): conversas entre contratante e babá
- **Verificação de Identidade** (Epic 1): obrigatória para babás
- **Notificações** (Epic 6): notificações de solicitações, aceites, lembretes
- **Sistema de Avaliações**: pode reutilizar `RatingService` do Marketplace

---

## 📝 Notas de Implementação

- Considerar criar módulo `Arah.Modules.Babysitters` seguindo padrão de outros módulos
- Reutilizar `RatingService` existente ou criar `BabysitterReviewService` específico
- Integrar com `ChatService` para conversas sobre serviços
- Considerar feature flag por território (alguns territórios podem não querer essa funcionalidade)
- Avaliar necessidade de plano premium para babás (badge "Babá Premium")

---

## 🎯 Próximos Passos (Pós-MVP)

- [ ] Sistema de agendamento recorrente (babá fixa)
- [ ] Sistema de referências verificadas (contato automático)
- [ ] Badges e certificações verificadas pela plataforma
- [ ] Sistema de seguro (parcerias com seguradoras)
- [ ] Integração com background check (verificação de antecedentes)
- [ ] Dashboard de analytics para babás (estatísticas de serviços, receita)
