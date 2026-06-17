# Fase 51: Espaços e Prestadores de Wellness - Agendas Compartilhadas

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🟡 MÉDIA (Funcionalidade de valor agregado para comunidade territorial)  
**Depende de**: Marketplace (Fase 6), Sistema de Pagamentos (Fase 6), Chat (Fase 8), Verificação de Identidade (Epic 1), Map Entities (Epic 2), Sistema de Avaliações  
**Estimativa Total**: 140 horas  
**Status**: ⏳ Pendente  
**Categoria**: Serviços Territoriais / Marketplace de Serviços / Wellness

---

## 🎯 Objetivo

Implementar funcionalidade completa de conexão entre **espaços de bem-estar** (yoga centers, espaços holísticos, spas) e **prestadores de serviços de wellness** (professores de yoga, massagistas, terapeutas de som, etc.), permitindo que:
- **Espaços** compartilhem suas agendas e permitam que profissionais marquem horários
- **Prestadores** encontrem espaços adequados e reservem horários nas agendas compartilhadas
- **Clientes** encontrem prestadores e vejam onde eles atendem (espaços ou locais próprios)
- **Sistema** gerencie todo o ciclo: busca, reserva de espaço, aprovação, agendamento de clientes, pagamento

**Princípios**:
- ✅ **Colaboração**: Facilita conexão entre espaços e profissionais
- ✅ **Flexibilidade**: Espaços controlam suas agendas compartilhadas
- ✅ **Territorialidade**: Espaços e prestadores vinculados ao território
- ✅ **Integração**: Reutiliza Map Entities, Marketplace, Chat, Pagamentos e Verificação existentes

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Marketplace completo (Stores, Items, Cart, Checkout)
- ✅ Sistema de pagamentos (Mercado Pago, Stripe)
- ✅ Chat territorial (canais, grupos, DM)
- ✅ Verificação de identidade e residência
- ✅ Sistema de avaliações (RatingService no Marketplace)
- ✅ Map Entities (lugares físicos no território)
- ❌ Sistema de agendas compartilhadas não existe
- ❌ Conexão entre espaços e prestadores não existe
- ❌ Gestão de reservas em espaços não existe

### Requisitos Funcionais

#### 1. Cadastro de Espaços de Wellness
- ✅ Espaço físico pode se cadastrar como "Wellness Space" no território
- ✅ Formulário completo com:
  - Informações básicas (nome, descrição, endereço, localização no mapa)
  - Tipos de serviços oferecidos (yoga, massagem, terapia sonora, meditação, pilates, etc.)
  - Capacidade (número máximo de pessoas por sessão)
  - Equipamentos disponíveis (mats, aparelhos, instrumentos, etc.)
  - Fotos do espaço (galeria)
  - Políticas de uso (regras, cancelamento, limpeza)
  - Contato (telefone, email, horários de atendimento)
- ✅ Espaço pode ativar "Agenda Compartilhada":
  - Define horários disponíveis para profissionais marcarem
  - Configura slots recorrentes (ex: toda segunda 9h-11h) ou pontuais
  - Define duração padrão de slots (ex: 1h, 1.5h, 2h)
  - Define regras: antecipação mínima para reserva, duração mínima/máxima, política de cancelamento
- ✅ Status do espaço: `Pending`, `Active`, `Inactive`, `Suspended`
- ✅ Espaço pode bloquear horários específicos (manutenção, eventos privados)

#### 2. Cadastro de Prestadores de Serviços de Wellness
- ✅ Usuário pode criar perfil de prestador de wellness no território
- ✅ Formulário completo com:
  - Informações pessoais (nome, foto, descrição, bio)
  - Especialidades (yoga (tipos: Hatha, Vinyasa, etc.), massagem (tipos: relaxante, terapêutica, etc.), terapia sonora, reiki, acupuntura, etc.)
  - Certificações (formações, cursos, certificações profissionais)
  - Experiência (anos de experiência, número de clientes atendidos)
  - Preços (por sessão, pacotes, valores diferentes por tipo de serviço)
  - Área de atuação (raio em km, territórios onde atende)
  - Agenda pessoal (horários disponíveis para atender clientes em locais próprios ou móveis)
- ✅ Prestador pode ter múltiplas especialidades
- ✅ Verificação de identidade obrigatória
- ✅ Status do perfil: `Pending`, `Active`, `Inactive`, `Suspended`

#### 3. Busca de Espaços e Prestadores
- ✅ Busca unificada ou separada (espaços vs prestadores)
- ✅ Filtros para espaços:
  - Tipo de serviço oferecido
  - Localização (raio em km)
  - Capacidade (mínima/máxima)
  - Equipamentos disponíveis
  - Agenda compartilhada ativa
  - Disponibilidade (horários livres próximos)
- ✅ Filtros para prestadores:
  - Especialidade
  - Certificações específicas
  - Localização (raio em km)
  - Preço (faixa de valores)
  - Avaliação mínima
  - Verificado
- ✅ Ordenação: relevância, avaliação, distância, preço
- ✅ Resultados mostram: foto, nome, tipo (espaço/prestador), especialidades, localização, disponibilidade, avaliações

#### 4. Agenda Compartilhada de Espaços
- ✅ Espaço define horários disponíveis para reserva:
  - Slots recorrentes (ex: toda segunda 9h-11h, toda quinta 14h-16h)
  - Slots pontuais (data/hora específica)
  - Duração de cada slot
  - Capacidade do slot (quantos profissionais podem usar simultaneamente, se aplicável)
- ✅ Espaço pode bloquear horários:
  - Bloqueio recorrente (ex: domingos fechado)
  - Bloqueio pontual (ex: 15/02/2026 10h-12h para manutenção)
- ✅ Visualização da agenda:
  - Calendário mensal/semanal com slots livres/ocupados/bloqueados
  - Lista de reservas pendentes e confirmadas
  - Histórico de reservas passadas

#### 5. Reserva de Horários em Espaços
- ✅ Prestador visualiza agenda compartilhada de um espaço
- ✅ Prestador pode solicitar reserva de slot específico:
  - Seleciona data/hora e duração
  - Informa tipo de serviço que vai oferecer
  - Número de participantes esperados (se aplicável)
  - Observações (equipamentos necessários, configuração especial, etc.)
- ✅ Espaço recebe notificação de nova solicitação
- ✅ Espaço pode:
  - Aprovar solicitação (slot fica reservado)
  - Rejeitar solicitação (com motivo opcional)
  - Fazer contraproposta (data/hora alternativa, duração diferente)
- ✅ Após aprovação:
  - Slot aparece como ocupado na agenda do espaço
  - Prestador recebe confirmação
  - Reserva aparece no calendário do prestador
- ✅ Prestador pode cancelar reserva (conforme política do espaço)
- ✅ Status da reserva: `Pending`, `Approved`, `Rejected`, `CounterOffered`, `Cancelled`, `Completed`

#### 6. Gestão de Reservas
- ✅ Espaço pode ver todas as reservas:
  - Pendentes (aguardando aprovação)
  - Confirmadas (futuras)
  - Histórico (passadas)
- ✅ Espaço pode filtrar por prestador, data, status
- ✅ Prestador pode ver suas reservas em espaços:
  - Próximas reservas
  - Histórico
  - Status de cada reserva
- ✅ Notificações:
  - Prestador: confirmação de aprovação, lembretes antes da reserva
  - Espaço: nova solicitação, cancelamento

#### 7. Agendamento de Clientes com Prestadores
- ✅ Cliente pode buscar prestadores e ver:
  - Agenda pessoal do prestador (horários disponíveis)
  - Espaços onde prestador tem reservas confirmadas (horários disponíveis para agendamento)
- ✅ Cliente pode agendar sessão:
  - Escolhe prestador
  - Escolhe data/hora (da agenda pessoal ou de espaço reservado)
  - Escolhe tipo de serviço
  - Informa necessidades especiais (se aplicável)
- ✅ Prestador recebe notificação e pode aceitar/rejeitar
- ✅ Após aceite, sessão fica agendada
- ✅ Integração com pagamentos (cliente paga pela sessão)

#### 8. Integração com Map Entities
- ✅ Espaços de wellness aparecem como MapEntity no mapa
- ✅ Categoria especial: "Wellness Space"
- ✅ Pin mostra: nome, tipo, disponibilidade de agenda compartilhada
- ✅ Cliente pode ver no mapa onde prestadores atendem (espaços reservados)

---

## 📋 Tarefas Detalhadas

### Semana 1: Modelo de Dados - Espaços e Prestadores

#### 51.1 Modelo de Dados - Wellness Spaces
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar entidade `WellnessSpace` no Domain:
  - [ ] `Id`, `TerritoryId`, `OwnerUserId` (dono do espaço)
  - [ ] `Name`, `Description`, `Address`, `Latitude`, `Longitude`
  - [ ] `ServiceTypes` (array: Yoga, Massage, SoundTherapy, etc.)
  - [ ] `Capacity`, `Equipment` (array de strings)
  - [ ] `Photos` (array de MediaAsset IDs)
  - [ ] `Policies`, `ContactPhone`, `ContactEmail`
  - [ ] `Status` (Pending, Active, Inactive, Suspended)
  - [ ] `SharedCalendarEnabled` (boolean)
  - [ ] `CreatedAt`, `UpdatedAt`
- [ ] Criar entidade `WellnessSpaceCalendarSlot`:
  - [ ] `WellnessSpaceId`, `DayOfWeek` (nullable para slots pontuais), `Date` (nullable para slots pontuais)
  - [ ] `StartTime`, `EndTime`, `DurationMinutes`
  - [ ] `Capacity` (quantos profissionais podem usar simultaneamente)
  - [ ] `IsRecurring`, `IsBlocked`
  - [ ] `Rules` (JSON: minAdvanceBookingHours, cancellationPolicy, etc.)
- [ ] Criar entidade `WellnessSpaceReservation`:
  - [ ] `Id`, `WellnessSpaceId`, `ProviderUserId`, `CalendarSlotId`
  - [ ] `RequestedDate`, `RequestedStartTime`, `RequestedDurationMinutes`
  - [ ] `ServiceType`, `ExpectedParticipants`, `Notes`
  - [ ] `Status` (Pending, Approved, Rejected, CounterOffered, Cancelled, Completed)
  - [ ] `ApprovedDate`, `ApprovedStartTime`, `ApprovedDurationMinutes` (pode diferir do solicitado)
  - [ ] `RejectionReason`, `CounterOfferNotes`
  - [ ] `RequestedAt`, `ApprovedAt`, `RejectedAt`, `CancelledAt`, `CompletedAt`
- [ ] Criar repositórios: `IWellnessSpaceRepository`, `IWellnessSpaceCalendarRepository`, `IWellnessSpaceReservationRepository`
- [ ] Criar migrations PostgreSQL
- [ ] Testes unitários de domínio

**Arquivos a Criar**:
- `backend/Arah.Domain/Services/Wellness/WellnessSpace.cs`
- `backend/Arah.Domain/Services/Wellness/WellnessSpaceCalendarSlot.cs`
- `backend/Arah.Domain/Services/Wellness/WellnessSpaceReservation.cs`
- `backend/Arah.Application/Interfaces/IWellnessSpaceRepository.cs`
- `backend/Arah.Application/Interfaces/IWellnessSpaceCalendarRepository.cs`
- `backend/Arah.Application/Interfaces/IWellnessSpaceReservationRepository.cs`
- `backend/Arah.Infrastructure/Postgres/Entities/WellnessSpaceRecord.cs` (+ outros records)
- `backend/Arah.Infrastructure/Postgres/Repositories/PostgresWellnessSpaceRepository.cs` (+ outros repos)

#### 51.2 Modelo de Dados - Wellness Providers
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar entidade `WellnessProvider` no Domain:
  - [ ] `Id`, `UserId`, `TerritoryId` (múltiplos via `WellnessProviderTerritory`)
  - [ ] `Specialties` (array: YogaHatha, YogaVinyasa, MassageRelaxing, MassageTherapeutic, SoundTherapy, Reiki, Acupuncture, etc.)
  - [ ] `Certifications` (array de strings)
  - [ ] `ExperienceYears`, `ClientsServedCount`
  - [ ] `Pricing` (JSON: por sessão, pacotes, valores por tipo)
  - [ ] `ServiceRadiusKm`
  - [ ] `Description`, `Bio`
  - [ ] `Status` (Pending, Active, Inactive, Suspended)
  - [ ] `CreatedAt`, `UpdatedAt`, `VerifiedAt`
- [ ] Criar entidade `WellnessProviderAvailability`:
  - [ ] `WellnessProviderId`, `DayOfWeek`, `StartTime`, `EndTime`, `IsAvailable`
  - [ ] `LocationType` (OwnSpace, Mobile, Both)
- [ ] Criar entidade `WellnessProviderTerritory`:
  - [ ] `WellnessProviderId`, `TerritoryId`, `IsPrimary`
- [ ] Criar entidade `WellnessServiceBooking`:
  - [ ] `Id`, `WellnessProviderId`, `ClientUserId`, `WellnessSpaceReservationId` (nullable - se for em espaço)
  - [ ] `ServiceType`, `ScheduledDate`, `ScheduledStartTime`, `DurationMinutes`
  - [ ] `Location` (endereço ou referência ao espaço)
  - [ ] `SpecialNeeds`, `Status` (Pending, Confirmed, Completed, Cancelled)
  - [ ] `RequestedAt`, `ConfirmedAt`, `CompletedAt`, `CancelledAt`
- [ ] Criar repositórios: `IWellnessProviderRepository`, `IWellnessServiceBookingRepository`
- [ ] Criar migrations PostgreSQL
- [ ] Testes unitários de domínio

**Arquivos a Criar**:
- `backend/Arah.Domain/Services/Wellness/WellnessProvider.cs`
- `backend/Arah.Domain/Services/Wellness/WellnessProviderAvailability.cs`
- `backend/Arah.Domain/Services/Wellness/WellnessProviderTerritory.cs`
- `backend/Arah.Domain/Services/Wellness/WellnessServiceBooking.cs`
- `backend/Arah.Application/Interfaces/IWellnessProviderRepository.cs`
- `backend/Arah.Application/Interfaces/IWellnessServiceBookingRepository.cs`
- `backend/Arah.Infrastructure/Postgres/Entities/WellnessProviderRecord.cs` (+ outros records)
- `backend/Arah.Infrastructure/Postgres/Repositories/PostgresWellnessProviderRepository.cs` (+ outros repos)

### Semana 2: Backend - Cadastro e Busca

#### 51.3 Cadastro de Espaços - Backend
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `WellnessSpaceService` em Application:
  - [ ] `CreateSpaceAsync`: cria espaço de wellness
  - [ ] `UpdateSpaceAsync`: atualiza informações
  - [ ] `EnableSharedCalendarAsync`: ativa agenda compartilhada
  - [ ] `AddCalendarSlotAsync`: adiciona slot à agenda
  - [ ] `BlockCalendarSlotAsync`: bloqueia slot específico
  - [ ] `RemoveCalendarSlotAsync`: remove slot
- [ ] Criar `WellnessSpaceController`:
  - [ ] `POST /api/v1/wellness/spaces`: criar espaço
  - [ ] `PUT /api/v1/wellness/spaces/{id}`: atualizar espaço
  - [ ] `GET /api/v1/wellness/spaces/{id}`: detalhes do espaço
  - [ ] `POST /api/v1/wellness/spaces/{id}/calendar/enable`: ativar agenda compartilhada
  - [ ] `POST /api/v1/wellness/spaces/{id}/calendar/slots`: adicionar slot
  - [ ] `POST /api/v1/wellness/spaces/{id}/calendar/slots/{slotId}/block`: bloquear slot
  - [ ] `GET /api/v1/wellness/spaces/{id}/calendar`: visualizar agenda
- [ ] Criar contratos (DTOs)
- [ ] Validações (FluentValidation)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Wellness/WellnessSpaceService.cs`
- `backend/Arah.Api/Controllers/Wellness/WellnessSpaceController.cs`
- `backend/Arah.Api/Contracts/Wellness/*.cs`

#### 51.4 Cadastro de Prestadores - Backend
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `WellnessProviderService` em Application:
  - [ ] `CreateProviderAsync`: cria perfil de prestador
  - [ ] `UpdateProviderAsync`: atualiza informações
  - [ ] `SetAvailabilityAsync`: define disponibilidade pessoal
  - [ ] `AddSpecialtyAsync`: adiciona especialidade
  - [ ] `AddCertificationAsync`: adiciona certificação
- [ ] Criar `WellnessProviderController`:
  - [ ] `POST /api/v1/wellness/providers`: criar perfil
  - [ ] `PUT /api/v1/wellness/providers/{id}`: atualizar perfil
  - [ ] `GET /api/v1/wellness/providers/me`: meu perfil
  - [ ] `POST /api/v1/wellness/providers/{id}/availability`: definir disponibilidade
- [ ] Criar contratos (DTOs)
- [ ] Validações
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Wellness/WellnessProviderService.cs`
- `backend/Arah.Api/Controllers/Wellness/WellnessProviderController.cs`
- `backend/Arah.Api/Contracts/Wellness/*.cs`

#### 51.5 Busca de Espaços e Prestadores - Backend
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `WellnessSearchService`:
  - [ ] `SearchSpacesAsync`: busca espaços com filtros
  - [ ] `SearchProvidersAsync`: busca prestadores com filtros
  - [ ] `SearchUnifiedAsync`: busca unificada (espaços + prestadores)
- [ ] Criar endpoints:
  - [ ] `GET /api/v1/wellness/spaces/search`: busca espaços
  - [ ] `GET /api/v1/wellness/providers/search`: busca prestadores
  - [ ] `GET /api/v1/wellness/search`: busca unificada
- [ ] Filtros, ordenação, paginação
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Wellness/WellnessSearchService.cs`
- `backend/Arah.Api/Contracts/Wellness/WellnessSearchRequest.cs`
- `backend/Arah.Api/Contracts/Wellness/WellnessSearchResult.cs`

### Semana 3: Backend - Reservas e Agendas

#### 51.6 Reserva de Horários em Espaços - Backend
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `WellnessSpaceReservationService`:
  - [ ] `RequestReservationAsync`: prestador solicita reserva
  - [ ] `ApproveReservationAsync`: espaço aprova
  - [ ] `RejectReservationAsync`: espaço rejeita
  - [ ] `CounterOfferReservationAsync`: espaço faz contraproposta
  - [ ] `CancelReservationAsync`: cancelar reserva
  - [ ] `GetReservationsAsync`: listar reservas (espaço ou prestador)
- [ ] Criar endpoints:
  - [ ] `POST /api/v1/wellness/spaces/{id}/reservations`: solicitar reserva
  - [ ] `POST /api/v1/wellness/reservations/{reservationId}/approve`: aprovar
  - [ ] `POST /api/v1/wellness/reservations/{reservationId}/reject`: rejeitar
  - [ ] `POST /api/v1/wellness/reservations/{reservationId}/counter-offer`: contraproposta
  - [ ] `POST /api/v1/wellness/reservations/{reservationId}/cancel`: cancelar
  - [ ] `GET /api/v1/wellness/reservations/me`: minhas reservas
  - [ ] `GET /api/v1/wellness/spaces/{id}/reservations`: reservas do espaço
- [ ] Validações (slot disponível, conflitos, políticas)
- [ ] Notificações (nova solicitação, aprovação, rejeição, contraproposta)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Wellness/WellnessSpaceReservationService.cs`
- `backend/Arah.Api/Contracts/Wellness/RequestReservationRequest.cs`
- `backend/Arah.Api/Contracts/Wellness/ReservationResponse.cs`
- `backend/Arah.Api/Contracts/Wellness/CounterOfferRequest.cs`

#### 51.7 Agendamento de Clientes - Backend
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `WellnessServiceBookingService`:
  - [ ] `CreateBookingAsync`: cliente agenda sessão
  - [ ] `ConfirmBookingAsync`: prestador confirma
  - [ ] `RejectBookingAsync`: prestador rejeita
  - [ ] `CompleteBookingAsync`: marcar como concluído
  - [ ] `CancelBookingAsync`: cancelar agendamento
  - [ ] `GetProviderAvailabilityAsync`: disponibilidade do prestador (agenda pessoal + espaços reservados)
- [ ] Criar endpoints:
  - [ ] `POST /api/v1/wellness/providers/{id}/bookings`: agendar sessão
  - [ ] `POST /api/v1/wellness/bookings/{bookingId}/confirm`: confirmar
  - [ ] `POST /api/v1/wellness/bookings/{bookingId}/reject`: rejeitar
  - [ ] `POST /api/v1/wellness/bookings/{bookingId}/complete`: concluir
  - [ ] `GET /api/v1/wellness/providers/{id}/availability`: disponibilidade
  - [ ] `GET /api/v1/wellness/bookings/me`: meus agendamentos
- [ ] Integração com pagamentos
- [ ] Notificações
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Arah.Application/Services/Wellness/WellnessServiceBookingService.cs`
- `backend/Arah.Api/Contracts/Wellness/CreateBookingRequest.cs`
- `backend/Arah.Api/Contracts/Wellness/BookingResponse.cs`

### Semana 4: Integração com Map e Frontend

#### 51.8 Integração com Map Entities
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar categoria `WellnessSpace` no MapService
- [ ] Espaços de wellness aparecem como MapEntity
- [ ] Pin mostra informações do espaço e disponibilidade
- [ ] Cliente pode ver no mapa onde prestadores atendem
- [ ] Testes de integração

#### 51.9 Frontend Flutter - Espaços
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar feature `wellness` no Flutter:
  - [ ] `lib/features/wellness/data/repositories/wellness_repository.dart`
  - [ ] `lib/features/wellness/data/models/wellness_space.dart`
  - [ ] `lib/features/wellness/presentation/screens/wellness_spaces_search_screen.dart`
  - [ ] `lib/features/wellness/presentation/screens/wellness_space_detail_screen.dart`
  - [ ] `lib/features/wellness/presentation/screens/wellness_space_calendar_screen.dart`
- [ ] Tela de busca de espaços
- [ ] Tela de detalhes do espaço (agenda compartilhada visível)
- [ ] Tela de gestão de agenda (para dono do espaço)
- [ ] Testes de widget

**Arquivos a Criar**:
- `frontend/arah.app/lib/features/wellness/**/*.dart`

#### 51.10 Frontend Flutter - Prestadores e Reservas
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Tela de busca de prestadores
- [ ] Tela de perfil do prestador
- [ ] Tela de solicitação de reserva em espaço
- [ ] Tela de gestão de reservas (prestador)
- [ ] Tela de agendamento de sessão com prestador
- [ ] Testes de widget

### Semana 5: Testes e Documentação

#### 51.11 Testes e Documentação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração completos (backend)
- [ ] Testes E2E (fluxo completo: cadastro espaço → cadastro prestador → reserva → agendamento cliente)
- [ ] Documentação Swagger atualizada
- [ ] Documentação de uso (README)
- [ ] Atualizar roadmap e backlog

---

## 📊 Métricas de Sucesso

- ✅ Cadastro de espaços e prestadores funcional
- ✅ Agenda compartilhada operacional
- ✅ Reservas funcionam end-to-end (solicitação → aprovação → confirmação)
- ✅ Agendamento de clientes integrado com disponibilidade de prestadores
- ✅ Integração com Map Entities funcionando
- ✅ Notificações enviadas corretamente
- ✅ Cobertura de testes >80%

---

## 🔗 Dependências e Integrações

- **Map Entities** (Epic 2): espaços aparecem no mapa
- **Marketplace** (Fase 6): estrutura de busca e avaliações
- **Sistema de Pagamentos** (Fase 6): pagamento de sessões agendadas
- **Chat** (Fase 8): comunicação entre espaço/prestador/cliente
- **Verificação de Identidade** (Epic 1): obrigatória para prestadores
- **Notificações** (Epic 6): notificações de reservas, aprovações, agendamentos
- **Sistema de Avaliações**: avaliações de espaços e prestadores

---

## 📝 Notas de Implementação

- Considerar criar módulo `Arah.Modules.Wellness` seguindo padrão de outros módulos
- Reutilizar `RatingService` existente para avaliações
- Integrar com `ChatService` para comunicação sobre reservas
- Considerar feature flag por território
- Avaliar necessidade de plano premium para espaços ou prestadores
- Sistema de calendário pode reutilizar lógica de `Events` (Fase 8) se aplicável

---

## 🎯 Próximos Passos (Pós-MVP)

- [ ] Sistema de pacotes de sessões (descontos para múltiplas sessões)
- [ ] Sistema de workshops/eventos em espaços (múltiplos participantes)
- [ ] Integração com sistema de check-in (QR code para confirmar presença)
- [ ] Dashboard de analytics para espaços e prestadores
- [ ] Sistema de recomendações baseado em histórico e preferências
- [ ] Integração com sistema de avaliações pós-sessão
