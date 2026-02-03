# Changelog - Araponga

**Versão**: 3.0  
**Data**: 2025-01-20  
**Última Atualização**: 2025-01-20  

---

## 📋 Índice

1. [Mudanças Recentes](#mudanças-recentes)
2. [Versões Anteriores](#versões-anteriores)
3. [Histórico Completo](#histórico-completo)

---

## 🆕 Mudanças Recentes
### Versão 3.6 (2026-02-02) - Conexões / Círculo de Amigos (Fase 49 - MVP) ✅

**Status**: ✅ **MVP implementado**

#### Conexões e Círculo de Amigos
- **Feature flags por território**: `ConnectionsEnabled` (15) e `ConnectionsFeedPrioritize` (16)
- **Domínio**: `UserConnection`, `ConnectionStatus`, `ConnectionPrivacySettings`, `ConnectionRequestPolicy`, `ConnectionVisibility`; repositórios `IUserConnectionRepository`, `IConnectionPrivacySettingsRepository`
- **Infraestrutura**: `PostgresUserConnectionRepository`, `PostgresConnectionPrivacySettingsRepository`; tabelas `user_connections`, `connection_privacy_settings` (migration `AddConnectionsModule`)
- **Application**: `ConnectionService` (solicitar, aceitar, rejeitar, remover, listar); `AcceptedConnectionsProvider` para integração com Feed
- **Integração Feed**: parâmetro `prioritizeConnections` (default true) em `GET /api/v1/feed` e `GET /api/v1/feed/paged`; priorização por conexões aceitas quando a feature flag está ativa no território
- **API**: `ConnectionsController` — `POST /api/v1/connections/request`, `POST /api/v1/connections/{id}/accept`, `POST /api/v1/connections/{id}/reject`, `DELETE /api/v1/connections/{id}`, `GET /api/v1/connections`, `GET /api/v1/connections/pending`, `GET /api/v1/connections/users/search`, `GET /api/v1/connections/suggestions`, `GET/PUT /api/v1/connections/privacy`
- **Notificações**: eventos `ConnectionRequestedEvent` e `ConnectionAcceptedEvent`; handlers enfileiram `notification.dispatch` (kinds `connection.request` e `connection.accepted`) para caixa de entrada in-app do destinatário
- **Testes de integração**: `ConnectionNotificationFlowTests` (fluxo request/accept → Outbox); `ConnectionsIntegrationTests` (API: request + accept e asserção das mensagens de notificação no Outbox)

**Documentação**: Ver `docs/backlog-api/FASE49_CONEXOES_CIRCULO_AMIGOS.md` e `docs/funcional/23_CONEXOES_CIRCULO_AMIGOS.md`.

---

### Versão 3.5 (2026-01-25) - Fase 13: Conector de Envio de Emails - COMPLETA ✅

**Status**: ✅ **Fase 13 Finalizada (100%)**

#### 📧 Sistema de Envio de Emails Completo

**Infraestrutura**:
- ✅ Interface `IEmailSender` e modelo `EmailMessage` implementados
- ✅ `SmtpEmailSender` com suporte a MailKit e configuração flexível
- ✅ Sistema de templates de email (`EmailTemplateService`) com suporte a placeholders, condicionais e loops
- ✅ Queue assíncrona de envio (`EmailQueueService` e `EmailQueueWorker`)
- ✅ Retry policy com exponential backoff (até 4 tentativas)
- ✅ Dead letter queue para falhas persistentes
- ✅ Rate limiting (100 emails/minuto)

**Templates de Email**:
- ✅ `welcome.html` - Email de boas-vindas
- ✅ `password-reset.html` - Recuperação de senha
- ✅ `event-reminder.html` - Lembretes de eventos
- ✅ `marketplace-order.html` - Confirmação de pedidos
- ✅ `alert-critical.html` - Alertas críticos
- ✅ Layout base (`_layout.html`) responsivo

**Integração**:
- ✅ Integração completa com sistema de notificações (`OutboxDispatcherWorker`)
- ✅ `EmailNotificationMapper` para mapear tipos de notificação para templates
- ✅ Verificação automática de preferências de email do usuário
- ✅ Priorização de emails (Critical, High, Normal, Low)

**Preferências de Email**:
- ✅ Modelo de domínio `EmailPreferences` com `EmailFrequency` e `EmailTypes`
- ✅ Endpoint `PUT /api/v1/users/me/preferences/email` implementado
- ✅ Suporte a opt-in/opt-out por tipo de email
- ✅ Frequências: Imediato, Diário, Semanal

**Casos de Uso**:
- ✅ Email de boas-vindas ao criar conta (`AuthService`)
- ✅ Email de recuperação de senha (`PasswordResetService`)
- ✅ Emails de notificações importantes (eventos, marketplace, alertas críticos)
- ✅ Respeito às preferências do usuário

**Testes**:
- ✅ Testes unitários (`EmailServiceEdgeCasesTests`, `EmailTemplateServiceEdgeCasesTests`, `EmailQueueServiceEdgeCasesTests`)
- ✅ Testes de integração E2E (`EmailIntegrationTests`)
- ✅ Cobertura de edge cases (Unicode, validações, retry logic)

**Documentação**:
- ✅ `docs/EMAIL_SYSTEM.md` - Documentação técnica completa
- ✅ `docs/FASE13_STATUS_IMPLEMENTACAO.md` - Status detalhado da implementação
- ✅ Configuração documentada (SMTP, templates, queue)

**Nota**: SendGrid (opcional) pode ser implementado posteriormente se necessário para produção.

---

### Versao 3.4 (2026-01-23) - Recuperacao de acesso e health checks

- Adicionados endpoints de recuperacao de acesso via email (`/api/v1/auth/password-reset` e `/api/v1/auth/password-reset/confirm`).
- Health checks estendidos com cache, storage, event bus e endpoint `/health/live`.
- Validacoes comuns e regras de geolocalizacao centralizadas.
- Excecoes tipadas adicionadas e mapeadas para ProblemDetails.
- Documentacao de fases e API atualizada para refletir o status atual.

### Versão 3.3 (2025-01-20) - Integração TDD/BDD nas Fases do Projeto

#### 🎯 Integração Completa com Backlog

**Documentos Atualizados**:
- ✅ **[Backlog API - README](./backlog-api/README.md)** - Todas as ondas e fases atualizadas com TDD/BDD, durações ajustadas (+20%), dependência da Fase 0 adicionada
- ✅ **[Status das Fases](./STATUS_FASES.md)** - Todas as fases (9-43) atualizadas com colunas TDD/BDD, dependências, durações ajustadas
- ✅ **[FASE9.md](./backlog-api/FASE9.md)** - Seção completa de TDD/BDD adicionada, exemplos Gherkin, estrutura de testes, dependência da Fase 0

**Mudanças no Backlog**:
- ✅ **Fase 0** documentada como bloqueante para todas as fases futuras (9-43)
- ✅ **Durações ajustadas**: Todas as fases futuras com +20% de tempo para TDD/BDD
- ✅ **BDD obrigatório** identificado por fase (governança, financeiro, Web3, DAO)
- ✅ **Cobertura obrigatória**: >90% para features críticas, >95% para smart contracts
- ✅ **Dependências atualizadas**: Todas as fases futuras dependem da Fase 0

**Novas Fases Estratégicas (30-43) com TDD/BDD**:
- ✅ Onda 0 (Governança): +20% de tempo, BDD obrigatório para votação
- ✅ Onda 0.5 (Financeira): +20% de tempo, BDD obrigatório para subscriptions/ticketing
- ✅ Onda 0.6 (Web3): +20% de tempo, **>95% cobertura obrigatória** para smart contracts
- ✅ Onda 0.7 (DAO): +20% de tempo, **>95% cobertura obrigatória** para tokens e governança on-chain
- ✅ Onda 0.8 (Diferenciação): +20% de tempo, BDD para funcionalidades complexas

**Resumo de Esforço Atualizado**:
- **Total**: 44 fases (incluindo Fase 0)
- **Duração original**: ~1121 dias
- **Duração com TDD/BDD**: ~1345 dias (+20%)
- **Duração com paralelização**: ~790 dias (aprox. 16 meses)

---

### Versão 3.2 (2025-01-20) - Integração TDD/BDD na Estratégia de Convergência

#### 🎯 Integração Estratégica

**Documentos Atualizados**:
- ✅ **[Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md)** - Integrado planejamento TDD/BDD como Fase 0 (bloqueante), ajustadas durações das fases estratégicas (+20%), adicionado TDD/BDD como vantagem competitiva e mitigação de riscos técnicos

**Mudanças Estratégicas**:
- ✅ **Fase 0 adicionada**: Fundação TDD/BDD (2 semanas) como pré-requisito bloqueante
- ✅ **Durações ajustadas**: Todas as fases estratégicas (1-5) com +20% de tempo para TDD/BDD
- ✅ **Cobertura obrigatória**: >90% para features críticas, >95% para smart contracts
- ✅ **BDD obrigatório**: Features de negócio críticas (Governança, Web3, DAO, Financeiro)

---

### Versão 3.1 (2025-01-20) - Plano TDD/BDD e Reavaliação de Fases

#### 🧪 Implementação de TDD e BDD

**Documentos Criados**:
- ✅ **[Plano de Implementação TDD/BDD](./23_TDD_BDD_PLANO_IMPLEMENTACAO.md)** - Plano completo de implementação de TDD e BDD considerando backend (.NET), frontend Wiki (Next.js) e DevPortal (HTML/JS)

**Documentos Atualizados**:
- ✅ **[Análise de Coesão e Testes](./22_COHESION_AND_TESTS.md)** - Adicionada seção completa sobre TDD/BDD com padrões, exemplos e métricas
- ✅ **[Roadmap Estratégico](./02_ROADMAP.md)** - Adicionada Onda 0 (Fundação TDD/BDD) como fase bloqueante, ajustadas durações das fases futuras (+20% para TDD/BDD)
- ✅ **[Índice da Documentação](./00_INDEX.md)** - Adicionado link para plano TDD/BDD
- ✅ **[`.cursorrules`](../.cursorrules)** - Adicionadas diretrizes obrigatórias de TDD e BDD

#### 📋 Mudanças no Roadmap

**Nova Fase Bloqueante**:
- **Fase 0**: Fundação TDD/BDD (14 dias) - Setup SpecFlow, Jest, documentação, piloto e treinamento

**Ajustes nas Fases Futuras**:
- Todas as fases futuras terão **+20% de tempo** para implementação TDD/BDD obrigatória
- Cobertura de testes >90% obrigatória
- BDD obrigatório para features de negócio críticas

**Exemplos de Ajustes**:
- Fase 9 (Perfil de Usuário): 21 → 25 dias
- Fase 10 (Mídias Avançadas): 25 → 30 dias
- Fase 14 (Governança): 21 → 25 dias
- Fase 30 (Proof of Sweat): 30 → 36 dias

---

### Versão 3.0 (2025-01-20) - Estratégia de Convergência de Mercado

#### 🎯 Novos Documentos Estratégicos

**Documentos Criados**:
- ✅ **[Mapa de Funcionalidades - Mercado](./38_MAPA_FUNCIONALIDADES_MERCADO.md)** - Mapeamento completo de funcionalidades implementadas, planejadas e previstas para atingir níveis de mercado, comparação detalhada com Closer.earth e plataformas líderes
- ✅ **[Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md)** - Plano estratégico completo de convergência com padrões de mercado estabelecidos por projetos que recebem investimentos significativos

**Documentos Atualizados**:
- ✅ **[Roadmap Estratégico](./02_ROADMAP.md)** - Reorganizado completamente em 9 ondas estratégicas, incluindo novas fases identificadas
- ✅ **[Visão do Produto](./01_PRODUCT_VISION.md)** - Atualizada com evolução estratégica, Web3 e DAO na visão
- ✅ **[Backlog API](./backlog-api/README.md)** - Expandido de 29 para 43 fases, incluindo novas fases estratégicas (30-43)
- ✅ **[Índice da Documentação](./00_INDEX.md)** - Atualizado com novos documentos estratégicos e melhorias de linguagem

#### 🆕 Novas Fases Estratégicas Identificadas

**Onda 0: Fundação de Governança** (Mês 0-3)
- **Fase 30**: Proof of Sweat (Tradicional) - 30 dias
- **Fase 31**: Dashboard de Métricas Comunitárias - 14 dias

**Onda 0.5: Sustentabilidade Financeira** (Mês 3-6)
- **Fase 32**: Subscriptions & Recurring Payments - 45 dias
- **Fase 33**: Ticketing para Eventos - 21 dias

**Onda 0.6: Preparação Web3** (Mês 6-9)
- **Fase 34**: Avaliação e Escolha de Blockchain - 14 dias
- **Fase 35**: Camada de Abstração Blockchain - 30 dias
- **Fase 36**: Integração Wallet (WalletConnect) - 30 dias
- **Fase 37**: Smart Contracts Básicos - 45 dias

**Onda 0.7: DAO e Tokenização** (Mês 9-12)
- **Fase 38**: Tokens On-chain (ERC-20) - 60 dias
- **Fase 39**: Governança Tokenizada - 30 dias
- **Fase 40**: Proof of Presence On-chain - 30 dias

**Onda 0.8: Diferenciação** (Mês 12-18)
- **Fase 41**: Learning Hub - 60 dias
- **Fase 43**: Rental System - 45 dias
- **Fase 43**: Agente IA (Versão Básica) - 90 dias

#### 🔄 Reorganização de Prioridades

**Prioridades Atualizadas**:
- 🔴 **P0 - Crítico (0-12 meses)**: Governança, Sustentabilidade Financeira, Web3, DAO e Tokenização
- 🟡 **P1 - Alta (0-18 meses)**: MVP Essencial, Soberania Territorial, Economia Circular
- 🟢 **P2 - Média (12-24 meses)**: Diferenciação, Extensões, Otimizações

**Marco Crítico**: DAO Completa em 12 meses (crítico para competir no mercado)

#### 📊 Melhorias de Documentação

**Linguagem e Formato**:
- ✅ Linguagem atualizada para padrão corporativo/profissional
- ✅ Formato estruturado com seções claras e navegáveis
- ✅ Tabelas comparativas e métricas objetivas
- ✅ Referências cruzadas entre documentos estratégicos

**Estrutura**:
- ✅ Resumos executivos nos principais documentos
- ✅ Índices estruturados e navegáveis
- ✅ Comparações objetivas com mercado
- ✅ Métricas e KPIs claros

#### 📈 Impacto das Mudanças

**Roadmap Atualizado**:
- **Total de Fases**: 29 → **43 fases** (14 novas fases estratégicas)
- **Duração Estimada**: 380 dias → **~1133 dias sequenciais** / **~672 dias com paralelização** (aprox. 14 meses)
- **80% do valor**: Em 12 meses (DAO Completa)
- **90% do valor**: Em 18 meses (Diferenciação)

**Convergência de Mercado**:
- ✅ Gaps críticos identificados e priorizados
- ✅ Funcionalidades essenciais para investimento identificadas
- ✅ Roadmap alinhado com padrões de mercado
- ✅ Estratégia de implementação gradual (Web2 → Web3)

---

## 📅 Versões Anteriores

### Versão 2.0 (2025-01-20) - MVP Completo

**Status**: MVP Completo + Fases 1-8 Implementadas

**Principais Conquistas**:
- ✅ MVP completo com todas as funcionalidades core
- ✅ 8 fases de infraestrutura implementadas (Segurança, Qualidade, Performance, Observabilidade)
- ✅ Marketplace completo implementado
- ✅ Sistema de mídia completo
- ✅ Chat territorial implementado
- ✅ Cobertura de testes >90%

**Documentação**:
- ✅ Roadmap atualizado
- ✅ Product Vision atualizado
- ✅ Backlog completo (29 fases)

---

## 📚 Referências Estratégicas

- **[Mapa de Funcionalidades](./38_MAPA_FUNCIONALIDADES_MERCADO.md)** - Mapeamento completo vs. mercado
- **[Estratégia de Convergência](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md)** - Plano estratégico completo
- **[Roadmap Estratégico](./02_ROADMAP.md)** - Planejamento atualizado
- **[Visão do Produto](./01_PRODUCT_VISION.md)** - Visão atualizada

---

**Última Atualização**: 2025-01-20  
**Versão**: 3.0  
**Status**: ✅ MVP Completo | 📊 Estratégia Atualizada

