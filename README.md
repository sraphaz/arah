# 🐦 Araponga

**Araponga** é uma plataforma digital comunitária orientada ao território.  
Um espaço onde tecnologia serve à vida local, à convivência e à autonomia das comunidades.

Não é uma rede social genérica.  
É uma **extensão digital do território vivo**.

---

## 🌱 Propósito

Vivemos um tempo em que plataformas digitais:
- capturam atenção,
- desorganizam comunidades,
- e desconectam pessoas do lugar onde vivem.

O Araponga nasce como um contraponto consciente a esse modelo.

> **Território como referência.  
> Comunidade como prioridade.  
> Tecnologia como ferramenta — não como fim.**

A proposta é simples e profunda:  
**recolocar o território no centro da experiência digital.**

---

## 🧭 O que é o Araponga?

O Araponga é um aplicativo/plataforma que permite:

- 📍 **Descobrir e reconhecer territórios reais**
- 👥 **Organizar comunidades locais**
- 🗞️ **Compartilhar informações relevantes ao lugar**
- 🗺️ **Visualizar eventos, avisos e iniciativas no mapa**
  - Uma entidade do território pode ser um estabelecimento, um órgão do governo, um espaço público ou um espaço natural.
- 🧑‍🌾 **Diferenciar moradores e visitantes com respeito**
- 🤝 **Fortalecer redes locais de cuidado, troca e presença**
- 🏪 **Marketplace territorial** para trocas locais
- 📅 **Eventos comunitários** organizados por território
- 🚨 **Alertas de saúde pública** e comunicação emergencial
- 💬 **Chat territorial (canais e grupos)** com governança (curadoria/moderação) e feature flags por território

Tudo isso **sem algoritmos de manipulação**,  
sem feed global infinito,  
sem extração de dados para publicidade.

---

## 🧠 Princípios fundamentais

### 1. Território é geográfico (e neutro)

No Araponga, `territory` representa **apenas um lugar físico real**:

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

O Araponga **não é**:
- um marketplace agressivo
- uma rede de engajamento infinito
- um produto de vigilância

Ele é uma **infraestrutura digital comunitária**, pensada para:

- autonomia local
- cuidado coletivo
- continuidade da vida no território
- fortalecimento do vínculo entre pessoas e lugar

---

## 🧩 Arquitetura (visão geral)

O backend segue princípios de **Clean Architecture**, com separação clara de responsabilidades:

backend/
├── Araponga.Api # API HTTP (controllers, endpoints, middlewares)
├── Araponga.Application # Casos de uso / regras de aplicação
├── Araponga.Domain # Modelo de domínio (territory, regras centrais)
├── Araponga.Infrastructure # Persistência, integrações, adapters
├── Araponga.Shared # Tipos e utilitários compartilhados
└── Araponga.Tests # Testes automatizados

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

## 📚 Documentação

**📋 [Índice Completo da Documentação](./docs/00_INDEX.md)** - Navegação estruturada

### Visão e Produto
- [Visão do Produto](./docs/01_PRODUCT_VISION.md)
- [Roadmap](./docs/02_ROADMAP.md)
- [Backlog](./docs/03_BACKLOG.md)
- [User Stories](./docs/04_USER_STORIES.md)
- [Glossário](./docs/05_GLOSSARY.md)

### Arquitetura
- [Decisões Arquiteturais (ADRs)](./docs/10_ARCHITECTURE_DECISIONS.md)
- [Arquitetura de Services](./docs/11_ARCHITECTURE_SERVICES.md)
- [Modelo de Domínio](./docs/12_DOMAIN_MODEL.md)
- [Domain Routing](./docs/13_DOMAIN_ROUTING.md)

### Desenvolvimento
- [Plano de Implementação](./docs/20_IMPLEMENTATION_PLAN.md)
- [Revisão de Código](./docs/21_CODE_REVIEW.md)
- [Análise de Coesão e Testes](./docs/22_COHESION_AND_TESTS.md)
- [Implementação de Recomendações](./docs/23_IMPLEMENTATION_RECOMMENDATIONS.md)

### Operações
- [Moderação](./docs/30_MODERATION.md)
- [Admin e Observabilidade](./docs/31_ADMIN_OBSERVABILITY.md)
- [Rastreabilidade](./docs/32_TRACEABILITY.md)

### Produção e Deploy
- [Avaliação Completa para Produção](./docs/50_PRODUCAO_AVALIACAO_COMPLETA.md)
- [Plano de Requisitos Desejáveis](./docs/51_PRODUCAO_PLANO_DESEJAVEIS.md)

---

## 🚀 Estado atual do projeto

### Funcionalidades Implementadas ✅

#### Core
- ✅ Backend estruturado com Clean Architecture
- ✅ Autenticação social com JWT e gestão de usuários
- ✅ Descoberta e seleção de territórios
- ✅ Vínculos (morador e visitante) com regras de visibilidade
- ✅ Feature flags por território
- ✅ Chat territorial: canais (público/moradores) + grupos com aprovação por curadoria

#### Feed e Social
- ✅ Feed territorial com criação, interações (like, comment, share) e moderação
- ✅ Feed pessoal e feed do território
- ✅ Posts com GeoAnchors (georreferenciamento)
- ✅ Paginação eficiente com otimizações (batch operations)

#### Mapa
- ✅ Mapa territorial com entidades (MapEntity) e relações
- ✅ Pins integrados (MapEntity + GeoAnchors de posts e assets)
- ✅ Visualização de entidades do território no mapa

#### Marketplace
- ✅ Stores (lojas/comércios) por território
- ✅ Listings (produtos e serviços) com busca e filtros
- ✅ Cart e Checkout
- ✅ Inquiries (consultas de compra)
- ✅ Platform Fees (taxas configuráveis por território)

#### Eventos
- ✅ Eventos comunitários por território
- ✅ Participações em eventos
- ✅ Eventos com georreferenciamento

#### Alertas e Saúde
- ✅ Alertas de saúde pública (Health Alerts)
- ✅ Comunicação emergencial por território

#### Assets
- ✅ Recursos compartilhados do território (Territory Assets)
- ✅ Validação e georreferenciamento de assets

#### Moderação
- ✅ Reports de posts e usuários
- ✅ Bloqueios de usuários
- ✅ Sanções territoriais e globais
- ✅ Moderação automática por threshold

#### Notificações
- ✅ Notificações in-app com outbox e inbox persistido
- ✅ Sistema confiável de entrega de notificações

#### Segurança e Produção
- ✅ JWT secret via variáveis de ambiente (obrigatório, mínimo 32 caracteres)
- ✅ HTTPS obrigatório em produção com HSTS
- ✅ Rate limiting completo (proteção contra DDoS e abuso):
  - Auth endpoints: 5 req/min
  - Feed endpoints: 100 req/min
  - Write endpoints: 30 req/min
- ✅ Security headers em todas as respostas (X-Frame-Options, CSP, etc.)
- ✅ Validação completa de input (14 validators FluentValidation)
- ✅ CORS configurado com validação em produção
- ✅ Health checks completos (liveness e readiness)
- ✅ Logging estruturado (Serilog)
- ✅ Connection pooling e retry policies
- ✅ Índices de banco para performance

#### Testes
- ✅ Testes automatizados (unidade, integração, E2E)
- ✅ Cobertura de testes ~82%
- ✅ CI configurado com builds reprodutíveis (`packages.lock.json`)

### Em Planejamento 🚧

- 🚧 Frontend e experiências móveis
- 🚧 Friends (círculo interno) e stories exclusivos
- 🚧 Admin/observabilidade com dashboards avançados
- 🚧 GeoAnchor avançado / memórias / galeria

O projeto está em **evolução ativa**, com foco em solidez antes de escala.

---

## 🛠️ Como rodar localmente

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
dotnet run --project backend/Araponga.Api
```

A API estará disponível em `http://localhost:5000` (ou porta configurada).

### Postgres (docker compose, recomendado)

```bash
docker compose up --build
```

Isso sobe a API e o PostgreSQL em containers Docker.

### Migrations (Postgres)

```bash
dotnet ef database update \
  --project backend/Araponga.Infrastructure \
  --startup-project backend/Araponga.Api
```

### Configuração (Produção)

Para rodar em produção, configure as variáveis de ambiente:

**Obrigatório**:
```bash
# JWT Secret - Mínimo 32 caracteres
# Gere com: openssl rand -base64 32
JWT__SIGNINGKEY=<secret-forte-de-pelo-menos-32-caracteres>

# CORS Origins - Não pode ser wildcard (*) em produção
Cors__AllowedOrigins__0=https://app.araponga.com
Cors__AllowedOrigins__1=https://www.araponga.com
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

O Araponga é aberto à colaboração, especialmente de pessoas interessadas em:

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

## 🌎 Visão de futuro

Algumas direções possíveis (não promessas fechadas):

- economias e moedas locais
- trocas de serviços comunitários
- governança distribuída
- integração com iniciativas regenerativas
- tecnologia como guardiã do território, não como exploradora

O Araponga não quer crescer rápido.  
Quer criar raízes profundas.

---

## ✨ Uma nota pessoal

Este projeto nasce de uma escuta atenta:

- da vida
- do território
- das comunidades
- e dos limites do modelo digital atual

Se você chegou até aqui e sentiu que isso faz sentido,  
você já faz parte da conversa.

---

## Developer Portal (GitHub Pages)

O conteúdo estático do Developer Portal vive em `backend/Araponga.Api/wwwroot/devportal` e é publicado automaticamente via GitHub Actions na branch `gh-pages` quando há push em `main` ou `master`.

---

## 📜 Licença

Este projeto é distribuído sob uma **licença aberta orientada à comunidade e ao território**.

- Versão oficial (EN): `LICENSE`
- Versão em português (PT-BR): `LICENSE.pt-BR`

🐦 Araponga canta para avisar, proteger e comunicar.  
Que esta plataforma faça o mesmo.
