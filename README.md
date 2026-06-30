# Arah

**Arah** é uma plataforma digital comunitária orientada ao território. Tecnologia que serve à vida local, à convivência e à autonomia das comunidades.

Não é uma rede social genérica. É uma **extensão digital do território vivo**.

---

## Propósito

Plataformas digitais capturam atenção, desorganizam comunidades e desconectam pessoas do lugar onde vivem.

O Arah é um contraponto consciente a esse modelo.

**Território como referência. Comunidade como prioridade. Tecnologia como ferramenta — não como fim.**

---

## O que é o Arah

Plataforma que permite:

- **Descobrir e reconhecer territórios reais**
- **Organizar comunidades locais**
- **Compartilhar informações relevantes ao lugar**
- **Visualizar eventos, avisos e iniciativas no mapa**
  - Entidades do território podem ser estabelecimentos, órgãos do governo, espaços públicos ou espaços naturais.
- **Diferenciar moradores e visitantes com respeito**
- **Fortalecer redes locais de cuidado, troca e presença**
- **Marketplace territorial** para trocas locais
- **Eventos comunitários** organizados por território
- **Alertas de saúde pública** e comunicação emergencial
- **Chat territorial (canais e grupos)** com governança (curadoria/moderação) e feature flags por território

Sem algoritmos de manipulação, feed global infinito ou extração de dados para publicidade.

---

## Princípios Fundamentais

### 1. Território é geográfico (e neutro)

No Arah, `territory` representa **apenas um lugar físico real**:

- nome
- localização
- fronteira geográfica

Ele **não contém lógica social**.

> O território existe antes do app  
> e continua existindo mesmo sem usuários.

Essa decisão arquitetural evita:
- centralização indevida
- conflitos de poder
- confusão entre espaço físico e relações sociais

---

### 2. Vida social acontece em camadas separadas

Relações humanas como:
- moradores
- visitantes
- visibilidade de conteúdo
- regras de convivência
- moderação

**não pertencem ao território**.

Elas pertencem a **camadas sociais que referenciam o território**.

Isso torna o sistema:
- mais claro
- mais justo
- mais adaptável ao tempo

---

### 3. Tecnologia a serviço do território

O Arah **não é**:
- um marketplace agressivo
- uma rede de engajamento infinito
- um produto de vigilância

Ele é uma **infraestrutura digital comunitária**, pensada para:

- autonomia local
- cuidado coletivo
- continuidade da vida no território
- fortalecimento do vínculo entre pessoas e lugar

---

## Arquitetura

O backend segue princípios de **Clean Architecture**, com separação clara de responsabilidades:

backend/
├── Arah.Api # API HTTP (controllers, endpoints, middlewares)
├── Arah.Application # Casos de uso / regras de aplicação
├── Arah.Domain # Modelo de domínio (territory, regras centrais)
├── Arah.Infrastructure # Persistência, integrações, adapters
├── Arah.Shared # Tipos e utilitários compartilhados
└── Arah.Tests # Testes automatizados

### Conceitos centrais do domínio

- **Territory**  
  Lugar físico real, neutro e persistente.

- **Membership**  
  Relação entre uma pessoa e um território (morador, visitante, etc.).

- **Feed / Map**  
  Informação contextual, sempre relacionada a um território específico.

- **Marketplace**  
  Sistema de trocas locais integrado ao território (stores, listings, cart, checkout).

- **Events**  
  Eventos comunitários organizados por território.

- **Alerts**  
  Alertas de saúde pública e comunicação emergencial.

- **Assets**  
  Recursos compartilhados do território (documentos, mídias, etc.).

---

## Quick start (app + stack local)

Para rodar **API, BFF, seeds e app Flutter** do zero:

1. **Subir a stack**: `.\scripts\run-local-stack.ps1` (PowerShell na raiz do repositório).  
   Use `-ResetDatabase` para recriar o banco e aplicar migrações.
2. **Rodar o app**: em outro terminal, `cd frontend\arah.app` e `flutter run`.

Guia completo (pré-requisitos, fluxo de onboarding, o que está implementado e o que falta): **[Release estável – App e Onboarding](./docs/STABLE_RELEASE_APP_ONBOARDING.md)**.

---

## Documentação

**[Índice Completo da Documentação](./docs/00_INDEX.md)** — Navegação estruturada

### Fases e Roadmap
- [**Backlog de Fases (48 fases completas)** →  `docs/backlog-api/`](./docs/backlog-api/) — **48 fases planejadas, 12 completas (Fases 1-12), 36 pendentes**
- [**Resumo Detalhado de Todas as Fases** → `docs/backlog-api/README.md`](./docs/backlog-api/README.md) — Resumo completo organizado por ondas estratégicas
- [Roadmap Completo](./docs/02_ROADMAP.md) — Visão de longo prazo
- [Mapa Completo das Fases](./docs/backlog-api/MAPA_FASES.md) — Mapa centralizado de todas as fases
- [Estrutura da Documentação](./docs/STRUCTURE.md) — Onde encontrar cada documento

### Visão e Produto
- [Visão do Produto](./docs/01_PRODUCT_VISION.md) — Princípios e valores
- [Glossário](./docs/05_GLOSSARY.md) — Terminologia do projeto

### Arquitetura
- [Decisões Arquiteturais (ADRs)](./docs/10_ARCHITECTURE_DECISIONS.md)
- [Arquitetura de Services](./docs/11_ARCHITECTURE_SERVICES.md)
- [Modelo de Domínio](./docs/12_DOMAIN_MODEL.md)

### Desenvolvimento
- [Guia de Desenvolvimento](./docs/DEVELOPMENT.md) — Setup local e padrões
- [Release estável – App e Onboarding](./docs/STABLE_RELEASE_APP_ONBOARDING.md) — Getting started, o que está no app e o que falta
- [Matriz API / BFF / App](./docs/FEATURE_MATRIX_API_BFF_APP.md) — Funcionalidades por camada, o que desenvolver no app/BFF/API
- [Configuração e Setup](./docs/SETUP.md) — Instalar e rodar o projeto
- [API Documentation](./docs/API.md) — Endpoints principais
- [Plano de Implementação](./docs/20_IMPLEMENTATION_PLAN.md)
- [Análise de Coesão e Testes](./docs/22_COHESION_AND_TESTS.md)

### Fases Técnicas
Documentação técnica das fases de implementação: Instalador, Modularização, BFF e Frontend.

- **[Índice de Documentação Técnica](./docs/TECNICO_INDEX.md)** ⭐ — Índice completo de todas as fases técnicas
- **[Instalador Visual](./docs/TECNICO_INSTALADOR_VISUAL.md)** 🛠️ — Sistema de instalação e configuração visual (15 passos, Monolito/Multi-Cluster, módulos, feature flags)
- **[Modularização](./docs/TECNICO_MODULARIZACAO.md)** 🧩 — Arquitetura modular e organização por domínios (15 módulos, feature flags, dependências)
- **[Backend for Frontend (BFF)](./docs/AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md)** 🔌 — Camada de abstração para interfaces (jornadas, contratos, exemplos)
- **[Frontend Flutter](./docs/24_FLUTTER_FRONTEND_PLAN.md)** 📱 — Planejamento completo do app mobile (arquitetura, stack, funcionalidades, UX/UI)

### Operações e Segurança
- [Documentação de Segurança](./docs/SECURITY_CONFIGURATION.md) — Configuração segura
- [Avaliação para Produção](./docs/50_PRODUCAO_AVALIACAO_COMPLETA.md) — Checklist
- [Governance System](./docs/GOVERNANCE_SYSTEM.md) — Votação e moderação
- [Community Moderation](./docs/COMMUNITY_MODERATION.md) — Políticas de moderação
- [Voting System](./docs/VOTING_SYSTEM.md) — Sistema de votação

### Frontend e Wiki
- [Wiki Frontend](./frontend/wiki) — Documentação interativa com Next.js
- [Developer Portal](./frontend/devportal) — Portal público (GitHub Pages)

---

## 🚀 Estado do Projeto - 48 Fases de Desenvolvimento

O Arah está em **desenvolvimento ativo** com **16 fases completas** (Fases 1-16) implementadas e validadas. O projeto segue um modelo de desenvolvimento disciplinado com foco em arquitetura sólida e evolução estratégica.

### 📊 Progresso do Desenvolvimento

| Fases | Status | Descrição |
|-------|--------|-----------|
| **Fases 1-8** | ✅ **COMPLETAS** | Fundação crítica: segurança, qualidade, performance, observabilidade, pagamentos, mídia |
| **Fases 9-12** | ✅ **MVP ESSENCIAL COMPLETO** | Perfil de Usuário, Mídias Avançadas, Edição e Gestão, Otimizações Finais (100%, Fase 12 encerrada) |
| **Fases 13-14** | ✅ **COMPLETAS** | Conector de Emails (100%), Governança Comunitária (100%) |
| **Fases 15-16** | ✅ **COMPLETAS** | Subscriptions & Recurring Payments (100%), Finalização Completa Fases 1-15 (100%) |
| **Fases 17-48** | 📋 **PLANEJADAS** | Economia local, Web3, DAO, diferenciação |

**Total de fases planejadas**: 48 fases  
**Fases completas**: 16 (Fases 1-16) ✅  
**Fases pendentes**: 32 (Fases 17-48) ⏳  
**Progresso**: ~33% do roadmap completo (~290 dias de ~1,327 dias totais)

---

### ✅ Funcionalidades Implementadas (Fases 1-16 - Fundação + MVP Essencial + Governança + Subscriptions)

#### Fase 1: Segurança e Fundação Crítica ✅
- ✅ Autenticação JWT com secret management robusto
- ✅ Autorização baseada em roles e capabilities
- ✅ Rate limiting e proteção contra DDoS/abuso
- ✅ Sanitização e validação de entrada (14 validadores FluentValidation)
- ✅ Security headers (HTTPS/HSTS obrigatório, X-Frame-Options, CSP, etc.)
- ✅ Testes de segurança (SQL injection, XSS, CSRF, path traversal)

#### Fase 2: Qualidade de Código e Confiabilidade ✅
- ✅ Testes unitários, integração e BDD
- ✅ Cobertura >90% nas camadas de negócio (Domain ~84-85%, Application ~70-72%)
- ✅ 2021 testes passando (100% taxa de sucesso)
- ✅ Refatoração e melhoria contínua
- ✅ Padrões de código e documentação

#### Fase 3: Performance e Escalabilidade ✅
- ✅ Cache distribuído com Redis
- ✅ Otimização de queries e índices de banco
- ✅ Paginação eficiente (cursor-based)
- ✅ Connection pooling com retry policies
- ✅ Cache invalidation automático

#### Fase 4: Observabilidade e Monitoramento ✅
- ✅ Logging estruturado com Serilog
- ✅ Métricas (Prometheus, OpenTelemetry)
- ✅ Health checks (liveness + readiness)
- ✅ Rastreamento e diagnóstico completo
- ✅ Métricas de connection pooling em tempo real

#### Fase 5: Segurança Avançada ✅
- ✅ 2FA (Two-Factor Authentication)
- ✅ CSRF protection
- ✅ Security headers completos
- ✅ Auditoria e logs de segurança
- ✅ CORS configurado com validação

#### Fase 6: Sistema de Pagamentos ✅
- ✅ Integração Stripe completa
- ✅ Checkout e processamento de pagamentos
- ✅ Webhooks e gestão de transações
- ✅ Suporte a múltiplos métodos de pagamento

#### Fase 7: Sistema de Payout e Gestão Financeira ✅
- ✅ Payouts para vendedores
- ✅ Gestão financeira completa
- ✅ Relatórios e reconciliação
- ✅ Balanços e transações

#### Fase 8: Infraestrutura de Mídia ✅
- ✅ Upload e armazenamento (S3-compatible: Local, S3, Azure Blob)
- ✅ Processamento de mídia
- ✅ Cache de URLs com Redis/Memory Cache
- ✅ Suporte a múltiplos provedores
- ✅ Configuração por território de provedores de mídia

#### Fase 9: Perfil de Usuário Completo ✅
- ✅ Avatar e bio funcionais
- ✅ Visualização de perfis com privacidade
- ✅ Sistema de estatísticas de contribuição territorial
- ✅ Endpoint de estatísticas do próprio perfil
- ✅ 18 testes unitários passando

#### Fase 10: Mídias Avançadas ✅ ~98%
- ✅ Suporte a vídeos e áudios em posts, eventos e marketplace
- ✅ Validações e limites implementados
- ✅ Chat com imagens e áudios curtos
- ✅ Múltiplas mídias por conteúdo

#### Fase 11: Edição e Gestão ✅
- ✅ Edição completa de posts e eventos
- ✅ Sistema de avaliações (lojas e itens)
- ✅ Busca full-text no marketplace
- ✅ Histórico completo de atividades (posts, eventos, participações, compras, vendas)

#### Fase 12: Otimizações Finais ✅ **100%** (encerrada)
- ✅ Exportação de Dados (LGPD) - 100%
- ✅ Sistema de Políticas e Termos - 100%
- ✅ Analytics e Métricas - 100%
- ✅ Notificações Push - 100%
- ✅ Testes de Performance - 100%
- ✅ Documentação de Operação - 100%
- ✅ CI/CD Pipeline - 100%
- ✅ Response Compression (gzip/brotli) - 100%

#### Fase 13: Conector de Emails ✅ **100%** (completa)
- ✅ Sistema de envio de emails via SMTP
- ✅ Templates HTML para notificações
- ✅ Fila de emails com processamento assíncrono
- ✅ Integração com Outbox pattern
- ✅ Suporte a múltiplos provedores SMTP

#### Fase 14: Governança Comunitária ✅ **100%** (completa)
- ✅ Sistema de votação territorial
- ✅ Moderação comunitária
- ✅ Feature flags por território
- ✅ Chat territorial com governança

#### Fase 15: Subscriptions & Recurring Payments ✅ **100%** (completa)
- ✅ Sistema completo de assinaturas recorrentes
- ✅ Integração com Stripe e Mercado Pago
- ✅ Planos de assinatura (FREE, BASIC, PREMIUM)
- ✅ Cupons e descontos
- ✅ Renovação automática de assinaturas
- ✅ Períodos de trial
- ✅ Analytics de assinaturas (MRR, churn rate)
- ✅ Webhooks para processamento de pagamentos
- ✅ Cobertura de testes ~93% (75/81 cenários)

#### Fase 16: Finalização Completa Fases 1-15 ✅ **100%** (completa)
- ✅ Sistema de Políticas de Termos e Critérios de Aceite
- ✅ Validação completa de endpoints (Fases 9, 11, 12, 13)
- ✅ Cobertura de testes Fase 15 (~93%)
- ✅ Testes de integração Subscriptions (100% - 9/9 testes passando)
- ✅ Documentação atualizada

---

### 📱 App (Flutter) — Entregas Recentes

O app consome o backend via **BFF** (jornadas `/api/v2/journeys/*`). Entregas recentes que expõem capacidades já existentes no backend:

- ✅ **Login com Google na UI** — botão "Entrar com Google" no fluxo de login (requer `GOOGLE_SIGN_IN_CLIENT_ID` + config Firebase para E2E).
- ✅ **Criação de eventos** — formulário com data/hora e local (jornada `events/create-event`).
- ✅ **Governança/Votações** — listar, votar, ver resultados e criar votações (jornada `governance`, expõe a Fase 14).
- ✅ **Deep-links no mapa** — tocar num pin abre detalhes e navega para evento/asset/alerta/feed.

Detalhes e o que falta no app: [Release estável – App e Onboarding](./docs/STABLE_RELEASE_APP_ONBOARDING.md) · [Matriz API/BFF/App](./docs/FEATURE_MATRIX_API_BFF_APP.md).

---

### 🔒 Segurança e Confiabilidade (Cross-Phase)

- ✅ JWT com secret de 32+ caracteres via variáveis de ambiente
- ✅ HTTPS obrigatório com HSTS em produção
- ✅ Rate limiting com proteção contra DDoS/abuso:
  - Auth: 5 req/min
  - Feed: 100 req/min
  - Escrita: 30 req/min
- ✅ Security headers (X-Frame-Options, CSP, X-Content-Type-Options, etc.)
- ✅ 14 validadores FluentValidation
- ✅ Testes de segurança: SQL injection, XSS, CSRF, path traversal, etc.
- ✅ CORS configurado com validação em produção
- ✅ Connection pooling com retry policies
- ✅ Índices de banco para performance crítica
- ✅ Cache invalidation automático em 9 services
- ✅ Logging estruturado com Serilog
- ✅ Health checks (liveness + readiness)
- ✅ Vulnerabilidades DoS em System.Text.Json resolvidas (atualizado para 8.0.5)

---

### 🧪 Testes (Fases 1-8 + Enterprise Coverage Phases 7-9)

- ✅ **2194 testes** totais (**2171 passando**, 23 pulados, **0 falhando**)
- ✅ **100% de taxa de sucesso** nos testes executados (2171/2171)
- ✅ Testes de unidade, integração e E2E
- ✅ 14 testes de segurança
- ✅ 7 testes de performance com SLAs
- ✅ **Cobertura** (Domain ~84–85%, Application ~70–72% nas camadas de negócio; overall ~34–36% linhas)
- ✅ CI com coverage (Codecov + Job Summary); local: `./scripts/run-coverage.ps1`

**Enterprise-Level Test Coverage**:
- ✅ Phases 7–9: 156 testes de edge cases (Application, Infrastructure, API)
- ✅ Fase 90%: 139 testes Application (lotes 1–8) + Fase 3 branches + Domain
- ✅ Fase 15: 75 testes de subscriptions (93% cobertura)
- ✅ **Status**: **2171 testes passando**, 23 skipped

Ver documentação completa: [`docs/ENTERPRISE_COVERAGE_PHASES_7_8_9_STATUS.md`](./docs/ENTERPRISE_COVERAGE_PHASES_7_8_9_STATUS.md)

---

### 📋 Resumo das 48 Fases Organizadas por Ondas Estratégicas

#### ✅ Fases 1-8: Fundação Crítica (Completas)
- **Fase 1**: Segurança e Fundação Crítica (14d) ✅
- **Fase 2**: Qualidade de Código (14d) ✅
- **Fase 3**: Performance e Escalabilidade (14d) ✅
- **Fase 4**: Observabilidade (14d) ✅
- **Fase 5**: Segurança Avançada (14d) ✅
- **Fase 6**: Sistema de Pagamentos (14d) ✅
- **Fase 7**: Sistema de Payout (28d) ✅
- **Fase 8**: Infraestrutura de Mídia (15d) ✅

#### ✅ Onda 1: MVP Essencial (Fases 9-12) - COMPLETA
- **Fase 9**: Perfil de Usuário Completo (21d) ✅ **100%**
- **Fase 10**: Mídias Avançadas (25d) ✅ **~98%**
- **Fase 11**: Edição e Gestão (15d) ✅ **100%**
- **Fase 12**: Otimizações Finais (28d) ✅ **100%** (encerrada)

**Total MVP Essencial**: ✅ **100% COMPLETO** (Fase 12 encerrada)

#### ✅ Onda 2: Governança e Sustentabilidade (Fases 13-16) - P0 Crítico - COMPLETA
- **Fase 13**: Conector de Emails (14d) ✅ **100%** (completa)
- **Fase 14**: Governança/Votação (21d) ✅ **100%** (completa)
- **Fase 15**: Subscriptions & Recurring Payments (60d) ✅ **100%** (completa)
- **Fase 16**: Finalização Completa Fases 1-15 (20d) ✅ **100%** (completa)

#### 🔴 Onda 3: Economia Local (Fases 17-19) - P0 Crítico
- **Fase 17**: Compra Coletiva (28d) ⏳
- **Fase 18**: Hospedagem Territorial (56d) ⏳
- **Fase 19**: Demandas e Ofertas (21d) ⏳

#### 🟡 Onda 4: Economia Local Completa (Fases 20-22) - P1 Alta
- **Fase 20**: Trocas Comunitárias (21d) ⏳
- **Fase 21**: Entregas Territoriais (28d) ⏳
- **Fase 22**: Moeda Territorial (35d) ⏳

#### 🟡 Onda 5: Conformidade e Soberania (Fases 23-25) - P1 Alta
- **Fase 23**: Inteligência Artificial (28d) ⏳
- **Fase 24**: Saúde Territorial (35d) ⏳
- **Fase 25**: Dashboard Métricas (14d) ⏳

#### 🟡 Onda 6: Autonomia Digital (Fases 26-30) - P1 Alta
- **Fase 26**: Hub Serviços Digitais (21d) ⏳
- **Fase 27**: Chat com IA (14d) ⏳
- **Fase 28**: Negociação Territorial (28d) ⏳
- **Fase 30**: Mobile Avançado (14d) ⏳

#### 🟡 Onda 7: Preparação Web3 (Fases 31-35) - P1 Quando Houver Demanda
- **Fase 31**: Avaliação Blockchain (14d) ⏳
- **Fases 32-35**: Abstração, Wallets, Smart Contracts, Criptomoedas (133d) ⏳

#### 🟡 Onda 8: DAO e Tokenização (Fases 36-40) - P1 Quando Houver Demanda
- **Fases 36-40**: Tokens, Governança Tokenizada, Proof of Presence, Ticketing, Agente IA (231d) ⏳

#### 🟡 Onda 9: Gamificação e Diferenciação (Fases 41-43) - P1/P2
- **Fase 42**: Gamificação Harmoniosa (28d) ⏳
- **Fase 41**: Proof of Sweat (30d) ⏳
- **Fase 43**: Arquitetura Modular (35d) ⏳

#### 🟢 Onda 10: Extensões e Diferenciação (Fases 44-48) - P2 Média
- **Fase 44**: Integrações Externas (35d) ⏳
- **Fase 45**: Learning Hub (60d) ⏳
- **Fase 46**: Rental System (45d) ⏳
- **Fase 48**: Banco de Sementes (21d) ⏳

**Roadmap completo e detalhado**: [`docs/backlog-api/README.md`](./docs/backlog-api/README.md) com resumo completo de todas as 48 fases

---

### 📈 Métricas do Projeto

| Métrica | Valor |
|---------|-------|
| **Linhas de código** | ~40.000+ |
| **Endpoints de API** | 150+ |
| **Testes automatizados** | 2194 (2171 passando, 23 pulados, 0 falhando) |
| **Cobertura de testes** | Domain ~84–85%, Application ~70–72% (camadas de negócio); overall ~34–36% linhas |
| **Taxa de sucesso** | 100% (2021/2021 testes executados) |
| **Coverage no CI** | Codecov + Job Summary; local `./scripts/run-coverage.ps1` |
| **Camadas de arquitetura** | 5 (Domain, Application, Infrastructure, API, Tests) |
| **Serviços de domínio** | 25+ |
| **Repositórios** | 20+ |
| **Migrations do BD** | 40+ |
| **Security validators** | 14 |
| **Fases planejadas** | 48 |
| **Fases completas** | 16 (Fases 1-16) |
| **Fases pendentes** | 32 (Fases 17-48) |
| **Progresso do roadmap** | ~33% (290 dias de ~1,327 dias totais) |

---

### 🎯 Próximos Passos Imediatos

1. **Fase 17**: Compra Coletiva (P0 Crítico) - Organização de compras coletivas, agrupamento de pedidos
2. **Fase 18**: Hospedagem Territorial (P0 Crítico) - Sistema de hospedagem, agenda, aprovação
3. **Fase 19**: Demandas e Ofertas (P0 Crítico) - Moradores cadastram demandas, outros fazem ofertas
4. **App (Flutter)**: provisionar config do Google Sign-In (OAuth/Firebase); push/FCM; tela de detalhe de post; upload de avatar
5. **Testes**: Validar cobertura de 90%+ (2171+ testes passando)
6. **Documentação**: Manter wiki, README e docs de fases sincronizados com cada entrega (app/BFF/API)
7. **Admin Dashboard**: Ferramentas de observabilidade para moderadores
8. **Escalabilidade**: Preparar para múltiplos territórios/usuários em produção

O projeto está em **evolução disciplinada**, com foco em solidez e escalabilidade antes de crescimento agressivo.

**Status Atual**: ✅ **Fases 1-16 Completas (100%)** - MVP Essencial + Comunicação + Governança + Subscriptions  
**Próxima Fase**: [Fase 17 - Compra Coletiva](./docs/backlog-api/FASE17.md) (28 dias, P0 Crítica)  
**📋 Plano de Implantação**: Ver [STATUS_FASES.md](./docs/STATUS_FASES.md) para roadmap completo

---

## Como Rodar Localmente

> A documentação canônica de operação está em [`docs/README.md`](docs/README.md).

### Pré-requisitos

- .NET 8 SDK
- Docker (opcional, para Postgres)
- Git

### InMemory (padrão, para desenvolvimento)

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project backend/Arah.Api
```

**Testes com cobertura**: `./scripts/run-coverage.ps1` (ou `pwsh scripts/run-coverage.ps1`). O CI (GitHub Actions) também roda coverage e envia ao Codecov.

A API estará disponível em `http://localhost:5000` (ou porta configurada).

### Postgres (docker compose, básico)

```bash
docker compose up --build
```

Isso sobe a API e o PostgreSQL em containers Docker.

### Ambiente Completo Docker (desenvolvimento/pré-produção, recomendado)

Para um ambiente completo com PostgreSQL, Redis, MinIO e API:

```powershell
# 1. Configurar variáveis de ambiente
cp .env.example .env
# Edite .env e configure JWT_SIGNINGKEY (obrigatório!)

# 2. Iniciar ambiente completo
.\scripts\docker-dev.ps1 up -Build

# Ou usando docker-compose diretamente:
docker-compose -f docker-compose.dev.yml up -d --build
```

**Serviços incluídos:**
- PostgreSQL 16 com PostGIS
- Redis 7 (cache distribuído)
- MinIO (storage S3-compatible)
- API Arah

**Endpoints:**
- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- MinIO Console: http://localhost:9001 (minioadmin/minioadmin)

Veja a [documentação completa do ambiente Docker](./docs/DOCKER_DEV_ENVIRONMENT.md) para mais detalhes.

### Migrations (Postgres)

```bash
dotnet ef database update \
  --project backend/Arah.Infrastructure \
  --startup-project backend/Arah.Api
```

### Configuração (Produção)

Para rodar em produção, configure as variáveis de ambiente:

**Obrigatório**:
```bash
# JWT Secret - Mínimo 32 caracteres
# Gere com: openssl rand -base64 32
JWT__SIGNINGKEY=<secret-forte-de-pelo-menos-32-caracteres>

# CORS Origins - Não pode ser wildcard (*) em produção
Cors__AllowedOrigins__0=https://app.arah.com
Cors__AllowedOrigins__1=https://www.arah.com
```

**Opcional** (se usar Postgres):
```bash
ConnectionStrings__Postgres=<connection-string>
Persistence__Provider=Postgres
Persistence__ApplyMigrations=true
```

**Opcional** (ajustar rate limiting):
```json
{
  "RateLimiting": {
    "PermitLimit": 60,
    "WindowSeconds": 60,
    "QueueLimit": 0
  }
}
```

**Documentação Completa**:
- [Configuração de Segurança](./docs/SECURITY_CONFIGURATION.md) - Guia completo de configuração
- [Avaliação para Produção](./docs/50_PRODUCAO_AVALIACAO_COMPLETA.md) - Checklist completo
- [Política de Segurança](./SECURITY.md) - Medidas de segurança implementadas

### Portal de autosserviço

A página inicial da API (`/`) serve um portal estático com explicação do produto,
domínios, fluxos e quickstart. Em desenvolvimento, acesse também:

- `/swagger` (documentação interativa da API)
- `/health` (health check de liveness)
- `/health/ready` (health check de readiness, verifica dependências)

Quando a API está rodando localmente em ambiente de desenvolvimento, o portal
exibe um preview do Swagger para navegação e testes rápidos.

Para publicação como site estático, o portal também está disponível em `docs/` e
pode ser hospedado via GitHub Pages (basta apontar a origem para a pasta `docs`).
A versão do GitHub Pages inclui links diretos para documentação, user stories e changelog.

---

## 🤝 Contribuindo

Consulte o guia em [`docs/41_CONTRIBUTING.md`](./docs/41_CONTRIBUTING.md).

O Arah é aberto à colaboração, especialmente de pessoas interessadas em:

- tecnologia com impacto social
- comunidades locais
- território, cultura e soberania
- arquitetura de software consciente
- regeneração e autonomia

Formas de contribuir:

- código
- testes
- documentação
- ideias
- feedback conceitual

Antes de abrir PRs grandes, abra uma issue para alinharmos a direção.

---

## Visão de Futuro

Direções possíveis (não promessas fechadas):

- Economias e moedas locais
- Trocas de serviços comunitários
- Governança distribuída
- Integração com iniciativas regenerativas
- Tecnologia como guardiã do território, não como exploradora

O Arah não quer crescer rápido. Quer criar raízes profundas.

---

## Uma Nota

Este projeto nasce de uma escuta atenta da vida, do território, das comunidades e dos limites do modelo digital atual.

Se você chegou até aqui e sentiu que isso faz sentido, você já faz parte da conversa.

---

## Developer Portal (GitHub Pages)

O conteúdo estático do Developer Portal vive em `frontend/devportal` e é publicado automaticamente via GitHub Actions na branch `gh-pages` quando há push em `main` ou `master`.

---

## Licença

Este projeto é distribuído sob uma **licença aberta orientada à comunidade e ao território**.

- Versão oficial (EN): `LICENSE`
- Versão em português (PT-BR): `LICENSE.pt-BR`

Arah canta para avisar, proteger e comunicar. Que esta plataforma faça o mesmo.
