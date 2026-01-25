# Índice da Documentação

---

Este documento organiza toda a documentação do projeto Araponga de forma estruturada e fácil de navegar.

## Estrutura da Documentação

### Visão e Produto
Documentos sobre a visão do produto, roadmap e funcionalidades.

- **[Visão do Produto](./01_PRODUCT_VISION.md)** ⭐ - Visão geral, princípios e evolução estratégica do Araponga
- **[Roadmap Estratégico](./02_ROADMAP.md)** ⭐ - Planejamento completo de funcionalidades e épicos, reorganizado estrategicamente
- **[Backlog](./03_BACKLOG.md)** - Lista de funcionalidades e prioridades
- **[User Stories](./04_USER_STORIES.md)** - Histórias de usuário consolidadas (MVP e pós-MVP)
- **[Glossário](./05_GLOSSARY.md)** - Termos e conceitos do projeto

### Arquitetura e Design
Documentos técnicos sobre arquitetura, decisões e design.

- **[Decisões Arquiteturais (ADRs)](./10_ARCHITECTURE_DECISIONS.md)** - Decisões arquiteturais importantes (ADR-001 a ADR-009)
- **[Arquitetura de Services](./11_ARCHITECTURE_SERVICES.md)** - Documentação dos services da camada de aplicação
- **[Modelo de Domínio (MER)](./12_DOMAIN_MODEL.md)** - Modelo de entidades e relacionamentos
- **[Domain Routing](./13_DOMAIN_ROUTING.md)** - Roteamento e organização de domínios

### Desenvolvimento e Implementação
Documentos sobre desenvolvimento, testes e implementação.

- **[Plano de Implementação](./20_IMPLEMENTATION_PLAN.md)** - Prioridades e dependências de implementação
- **[Revisão de Código](./21_CODE_REVIEW.md)** - Análise de gaps, inconsistências e melhorias
- **[Análise de Coesão e Testes](./22_COHESION_AND_TESTS.md)** - Avaliação de coesão e cobertura de testes
- **[Implementação de Recomendações](./23_IMPLEMENTATION_RECOMMENDATIONS.md)** - Status das recomendações implementadas
- **[Planejamento do Frontend Flutter](./24_FLUTTER_FRONTEND_PLAN.md)** - Planejamento completo do app mobile Flutter (arquitetura, stack, funcionalidades, UX/UI)
- **[Roadmap de Implementação do Frontend Flutter](./25_FLUTTER_IMPLEMENTATION_ROADMAP.md)** ⭐ - Roadmap extensivo detalhado por fases, sincronizado com API até Fase 28 (jornadas, funcionalidades, papéis, padrões elevados)
- **[Diretrizes de Design do App Flutter](./26_FLUTTER_DESIGN_GUIDELINES.md)** 🎨 ⭐ - Diretrizes high-end profissionais de design (cores, formas, transições, efeitos, estratégias de conversão preservando identidade visual)

### Operações e Governança
Documentos sobre moderação, observabilidade e operações.

- **[Moderação e Reports](./30_MODERATION.md)** - Sistema de moderação e reports
- **[Admin e Observabilidade](./31_ADMIN_OBSERVABILITY.md)** - Administração e observabilidade do sistema
- **[Rastreabilidade](./32_TRACEABILITY.md)** - Rastreabilidade de requisitos e funcionalidades
- **[System Config, Work Queue e Evidências](./33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)** - Configurações globais, fila genérica e evidências documentais (P0)
- **[API - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO.md)** - Documento índice (reorganizado em 22 subdocumentos - ver [índice completo](./api/60_API_LÓGICA_NEGÓCIO_INDEX.md))
- **Chat**: documentado em `60_API_LÓGICA_NEGÓCIO.md` (seção "Chat") e no DevPortal/OpenAPI (`backend/Araponga.Api/wwwroot/devportal/openapi.json`)
- **[Mídias em Conteúdo](./MEDIA_IN_CONTENT.md)** - Documentação completa da integração de mídias em Posts, Eventos, Marketplace e Chat
- **[Sistema de Mídia](./MEDIA_SYSTEM.md)** - Documentação técnica do sistema de mídia (armazenamento, processamento, APIs)
- **[Planejamento: Preferências de Usuário](./61_USER_PREFERENCES_PLAN.md)** - Planejamento completo da funcionalidade de preferências de privacidade e configurações do usuário

### Segurança
Documentos sobre segurança, configuração e implementação de medidas de segurança.

- **[Configuração de Segurança](./SECURITY_CONFIGURATION.md)** - Guia completo de configuração de segurança (JWT, Rate Limiting, CORS, HTTPS)
- **[Security Audit e Penetration Testing](./SECURITY_AUDIT.md)** - Checklist de segurança e guia de penetration testing
- **[Fase 1: Implementação de Segurança](./backlog-api/implementacoes/FASE1_IMPLEMENTACAO_RESUMO.md)** - Resumo da implementação da Fase 1 (Segurança Crítica)
- **[Fase 5: Segurança Avançada](./backlog-api/implementacoes/FASE5_IMPLEMENTACAO_RESUMO.md)** - Resumo da implementação da Fase 5 (2FA, Sanitização, CSRF, Secrets Management, Security Headers, Auditoria)

### Histórico e Mudanças
Documentos sobre histórico e mudanças do projeto.

- **[Changelog](./40_CHANGELOG.md)** - Histórico de mudanças e versões
- **[Onboarding Público](./ONBOARDING_PUBLICO.md)** 🌟 - Página pública de entrada para conhecer o projeto e escolher seu caminho
- **[Cartilha Completa](./CARTILHA_COMPLETA.md)** 📘 - Guia detalhado para todos os papéis no projeto
- **[Onboarding para Desenvolvedores](./ONBOARDING_DEVELOPERS.md)** 🌱 - Guia completo para novos desenvolvedores usando Cursor
- **[Onboarding para Analistas Funcionais](./ONBOARDING_ANALISTAS_FUNCIONAIS.md)** 👁️ - Guia para analistas funcionais que observam territórios e propõem melhorias
- **[FAQ - Perguntas Frequentes](./ONBOARDING_FAQ.md)** ❓ - Respostas para dúvidas comuns sobre onboarding e contribuição
- **[Sistema de Mentoria](./MENTORIA.md)** 🤝 - Guia para mentores e quem busca orientação
- **[Priorização de Propostas](./PRIORIZACAO_PROPOSTAS.md)** 📊 - Critérios e processo para validar funcionalidades
- **[Estrutura do Projeto](./PROJECT_STRUCTURE.md)** 📁 - Guia para navegar o código
- **[Estratégia de Consolidação do Ecossistema](./ECOSSISTEMA_CONSOLIDACAO_ESTRATEGIA.md)** 🌱 - Plano estratégico completo
- **[Checklist de Implementação](./ECOSSISTEMA_CONSOLIDACAO_CHECKLIST.md)** ✅ - Checklist detalhado da estratégia
- **[Configuração do Discord](./DISCORD_SETUP.md)** 💬 - Guia para configurar e usar o Discord do time
- **[Contribuindo](./41_CONTRIBUTING.md)** - Guia para contribuidores

### Planos e Recomendações
Documentos de planejamento e recomendações de implementação.

- **[Backlog API](./backlog-api/README.md)** - Backlog completo da API com fases organizadas estrategicamente (número de fases calculado dinamicamente - ver `PROJECT_PHASES_CONFIG.md`)
- **[Status das Fases](./STATUS_FASES.md)** - Status detalhado das fases (número calculado dinamicamente - ver `PROJECT_PHASES_CONFIG.md`)
- **[Recomendações de Segurança](./recommendations/RECOMENDACOES_SEGURANCA_PROXIMOS_PASSOS.md)** - Recomendações de segurança e próximos passos
- **[Plano de Refatoração - Recomendações Pendentes](./recommendations/PLANO_REFACTOR_RECOMENDACOES_PENDENTES.md)** - Plano de implementação de recomendações pendentes
- **[Planejamento: Preferências de Usuário](./61_USER_PREFERENCES_PLAN.md)** - Planejamento completo da funcionalidade de preferências de privacidade e configurações do usuário

### Estratégia e Benchmarking
Documentos de estratégia de convergência de mercado e análise comparativa.

- **[Mapa de Funcionalidades - Mercado](./38_MAPA_FUNCIONALIDADES_MERCADO.md)** 📊 ⭐⭐⭐ - Mapeamento completo de funcionalidades implementadas, planejadas e previstas para atingir níveis de mercado, comparação com Closer.earth e plataformas líderes
- **[Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md)** 🎯 ⭐⭐⭐ - Plano estratégico completo de convergência com padrões de mercado, análise competitiva, roadmap de adequação e mudanças organizacionais

### Produção e Deploy
Documentos sobre prontidão para produção e deploy.

- **[Avaliação Completa para Produção](./50_PRODUCAO_AVALIACAO_COMPLETA.md)** - Análise completa de prontidão para produção, gaps críticos e recomendações
- **[Plano de Requisitos Desejáveis](./51_PRODUCAO_PLANO_DESEJAVEIS.md)** - Plano detalhado de implementação para requisitos desejáveis (pós-lançamento)

### Pull Requests
Documentação detalhada de todos os Pull Requests implementados.

- **[Índice de PRs](./prs/README.md)** - Documentação completa de todos os PRs

### Documentação Organizada por Categoria

#### Refatorações
- **[Refatoração User-Centric Membership](./refactoring/REFACTOR_USER_CENTRIC_MEMBERSHIP.md)** - Refatoração completa do modelo
- **[Hierarquia de Permissões e Auditoria](./refactoring/70_HIERARQUIA_PERMISSOES_E_AUDITORIA.md)** - Implementação de hierarquia e auditoria
- **[Resumo do Modelo](./refactoring/REFACTOR_MODEL_SUMMARY.md)** - Resumo das mudanças
- Ver mais em: [refactoring/README.md](./refactoring/README.md)

#### Validações
- **[Validação de Segurança](./validation/VALIDACAO_SEGURANCA.md)** - Validação completa de segurança
- Ver mais em: [validation/README.md](./validation/README.md)

#### Recomendações
- **[Recomendações de Segurança](./recommendations/RECOMENDACOES_SEGURANCA_PROXIMOS_PASSOS.md)** - Recomendações e próximos passos
- **[Plano de Refatoração](./recommendations/PLANO_REFACTOR_RECOMENDACOES_PENDENTES.md)** - Plano de recomendações pendentes
- Ver mais em: [recommendations/README.md](./recommendations/README.md)

#### Análises
- **[Análises técnicas (índice)](./analysis/README.md)** - Índice de análises técnicas
- Ver mais em: [analysis/README.md](./analysis/README.md)

## Busca Rápida

### Por Tópico

**Produto e Funcionalidades:**
- Visão do Produto → `01_PRODUCT_VISION.md`
- User Stories → `04_USER_STORIES.md`
- Roadmap → `02_ROADMAP.md`
- Mapa de Funcionalidades → `38_MAPA_FUNCIONALIDADES_MERCADO.md`
- Estratégia de Convergência → `39_ESTRATEGIA_CONVERGENCIA_MERCADO.md`

**Arquitetura:**
- Decisões Arquiteturais → `10_ARCHITECTURE_DECISIONS.md`
- Arquitetura de Services → `11_ARCHITECTURE_SERVICES.md`
- Modelo de Domínio → `12_DOMAIN_MODEL.md`

**Desenvolvimento:**
- Plano de Implementação → `20_IMPLEMENTATION_PLAN.md`
- Revisão de Código → `21_CODE_REVIEW.md`
- Análise de Coesão → `22_COHESION_AND_TESTS.md`
- Planejamento do Frontend Flutter → `24_FLUTTER_FRONTEND_PLAN.md`

**Operações:**
- Moderação → `30_MODERATION.md`
- Observabilidade → `31_ADMIN_OBSERVABILITY.md`

## Convenções de Nomenclatura

### Arquivos na Raiz
Todos os arquivos na raiz seguem o padrão:
- `NN_NOME_DESCRITIVO.md` onde `NN` é um número sequencial para ordenação
- Nomes em inglês para consistência
- Hífens substituídos por underscores para melhor ordenação
- Categorias por prefixo numérico:
- `00-09`: Índices e guias
- `10-19`: Arquitetura e Design
- `20-29`: Desenvolvimento e Implementação
- `30-39`: Operações, Governança e Estratégia
- `40-49`: Histórico e Mudanças
- `50-59`: Produção e Deploy
- `60-69`: API e Funcionalidades

### Pastas Organizacionais
Documentos adicionais são organizados em pastas:
- **`refactoring/`** - Documentação de refatorações realizadas
- **`validation/`** - Validações técnicas (REST, segurança, estrutura)
- **`recommendations/`** - Recomendações e planos futuros
- **`analysis/`** - Análises técnicas do projeto
- **`prs/`** - Documentação detalhada de Pull Requests

## Migração de Nomes Antigos

| Nome Antigo | Nome Novo |
|------------|-----------|
| `PRODUCT_VISION.md` | `01_PRODUCT_VISION.md` |
| `USER_STORIES.md` | `04_USER_STORIES.md` |
| `user-stories.md` | (removido - duplicado) |
| `ROADMAP.md` | `02_ROADMAP.md` |
| `BACKLOG.md` | `03_BACKLOG.md` |
| `GLOSSARY.md` | `05_GLOSSARY.md` |
| `DECISOES_ARQUITETURAIS.md` | `10_ARCHITECTURE_DECISIONS.md` |
| `ARQUITETURA_SERVICES.md` | `11_ARCHITECTURE_SERVICES.md` |
| `DOMAIN_MODEL_MER.md` | `12_DOMAIN_MODEL.md` |
| `domain-routing.md` | `13_DOMAIN_ROUTING.md` |
| `IMPLEMENTATION_PLAN.md` | `20_IMPLEMENTATION_PLAN.md` |
| `PLANO_IMPLEMENTACAO_RECOMENDACOES.md` | (consolidado em `20_IMPLEMENTATION_PLAN.md`) |
| `REVISAO_CODIGO.md` | `21_CODE_REVIEW.md` |
| `ANALISE_COESAO_E_TESTES.md` | `22_COHESION_AND_TESTS.md` |
| `IMPLEMENTACAO_RECOMENDACOES.md` | `23_IMPLEMENTATION_RECOMMENDATIONS.md` |
| `MODERATION_REPORTS.md` | `30_MODERATION.md` |
| `ADMIN_OBSERVABILITY.md` | `31_ADMIN_OBSERVABILITY.md` |
| `TRACEABILITY.md` | `32_TRACEABILITY.md` |
| `CHANGELOG.md` | `40_CHANGELOG.md` |
| `CONTRIBUTING.md` | `41_CONTRIBUTING.md` |

## Mudanças Recentes no Projeto

### Estratégia de Convergência de Mercado (2025-01-20)
- **Análise Comparativa**: Análise profunda do Closer.earth e padrões de mercado estabelecidos
- **Novas Fases Estratégicas**: 15 novas fases identificadas (Fase 30 documentada + Fases 31-44 conceituais) para convergência com padrões de investimento
- **Priorização Atualizada**: Reorganização completa de prioridades baseada em gaps críticos vs. mercado
- **Roadmap Revisado**: Roadmap estratégico reorganizado em 9 ondas com foco em valor entregue
- **Mapa de Funcionalidades**: Mapeamento completo de funcionalidades implementadas, planejadas e previstas
- Ver detalhes em: [Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md) | [Mapa de Funcionalidades](./38_MAPA_FUNCIONALIDADES_MERCADO.md)

### Novas Fases Estratégicas Adicionadas (2025-01-20)
- **Fase 30**: Proof of Sweat (Tradicional) - Sistema de registro de atividades territoriais
- **Fase 31**: Dashboard de Métricas Comunitárias - Transparência e visualização de impacto
- **Fase 32**: Subscriptions & Recurring Payments - Sustentabilidade financeira
- **Fase 33**: Ticketing para Eventos - Monetização de eventos
- **Fases 34-37**: Preparação Web3 - Blockchain, wallets, smart contracts
- **Fases 38-40**: DAO e Tokenização - Tokens on-chain, governança tokenizada
- **Fases 41-44**: Diferenciação - Learning Hub, Rental System, Agente IA

### Hierarquia de Permissões e Auditoria (2025-01-16)
- **SystemAdmin implícito**: SystemAdmin tem todas as MembershipCapabilities em todos os territórios
- **Operações de configuração**: `GrantAsync`/`RevokeAsync` implementados com auditoria completa
- **Auditoria completa**: Todos os eventos de grant/revoke são registrados
- **Invalidação automática de cache**: Via eventos de domínio
- Ver detalhes em: [Hierarquia de Permissões](./refactoring/70_HIERARQUIA_PERMISSOES_E_AUDITORIA.md)

### Refatoração User-Centric Membership (2025-01)
- **Reorganização de Domínio**: Criada pasta `Membership/` em Domain
- **UserTerritory removido**: Substituído por `TerritoryMembership`
- **SystemPermission**: Permissões globais (Admin, SystemOperator) separadas de roles territoriais
- **AuthProvider**: Renomeado de `Provider` para `AuthProvider` no User
- **Renomeação listing → item**: API e contratos atualizados (`/api/v1/listings` → `/api/v1/items`)
- **Documentação XML**: Adicionada em todas as entidades principais
- Ver detalhes em: [Refatoração User-Centric](./refactoring/REFACTOR_USER_CENTRIC_MEMBERSHIP.md)

## Documentação Externa

- **Developer Portal**: Disponível em `/devportal` quando a API está rodando
- **Swagger/OpenAPI**: Disponível em `/swagger` em desenvolvimento
- **Health Check**: Disponível em `/health`
