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
├── Araponga.Api # API HTTP (controllers, endpoints)
├── Araponga.Application # Casos de uso / regras de aplicação
├── Araponga.Domain # Modelo de domínio (territory, regras centrais)
├── Araponga.Infrastructure # Persistência, integrações, adapters
├── Araponga.Shared # Tipos e utilitários compartilhados
└── Araponga.Tests # Testes automatizados


### Conceitos centrais do domínio

- **Territory**  
  Lugar físico real, neutro e persistente.

---

## 📚 Documentação

- [Visão do Produto](/docs/PRODUCT_VISION.md)
- [Backlog](/docs/BACKLOG.md)
- [User Stories](/docs/USER_STORIES.md)
- [Modelo de Domínio (MER)](/docs/DOMAIN_MODEL_MER.md)
- [Roadmap](/docs/ROADMAP.md)
- [Moderação e Reports](/docs/MODERATION_REPORTS.md)
- [Admin e Observabilidade](/docs/ADMIN_OBSERVABILITY.md)
- [Glossário](/docs/GLOSSARY.md)
- [Plano de Implementação](/docs/IMPLEMENTATION_PLAN.md)

- **Membership**  
  Relação entre uma pessoa e um território (morador, visitante, etc.).

- **Feed / Map**  
  Informação contextual, sempre relacionada a um território específico.
  - Integração de dados via `GET /api/v1/map/pins` (MapEntity + GeoAnchors de posts).

---

## 🧩 Feature flags (MVP)
- `GET /api/v1/territories/{territoryId}/features`
- `PUT /api/v1/territories/{territoryId}/features` (curadoria)

## 🔗 Endpoints MVP (parâmetros principais)
- `GET /api/v1/territories/nearby?lat=-23.37&lng=-45.02&radiusKm=25&limit=20`
- `GET /api/v1/map/pins?territoryId={territoryId}`
- `GET /api/v1/reports?territoryId={territoryId}&targetType=POST&status=OPEN`

---

## 🚀 Estado atual do projeto

- ✅ Backend inicial estruturado
- ✅ Autenticação social com JWT e gestão básica de usuários
- ✅ Descoberta e seleção de territórios
- ✅ Vínculos (morador e visitante) com regras de visibilidade
- ✅ Feed territorial com criação e moderação de conteúdo
- ✅ Mapa territorial com entidades e relações
- ✅ Moderação (reports e bloqueios)
- ✅ Feature flags e health check
- ✅ Testes automatizados
- ✅ CI configurado com builds reprodutíveis (`packages.lock.json`)
- 🚧 Frontend e experiências móveis em planejamento

O projeto está em **evolução ativa**, com foco em solidez antes de escala.

---

## 🛠️ Como rodar localmente

### Pré-requisitos
- .NET SDK 8.x
- Git
- Docker (para Postgres via compose)

### InMemory (padrão)
```bash
git clone https://github.com/sraphaz/araponga.git
cd araponga
dotnet restore
dotnet build
dotnet test
dotnet run --project backend/Araponga.Api
```

### Postgres (docker compose)
```bash
docker compose up --build
```

Para rodar localmente com Postgres sem compose, defina:
```
Persistence__Provider=Postgres
ConnectionStrings__Postgres=Host=localhost;Port=5432;Database=araponga;Username=araponga;Password=araponga
```

### Migrations (Postgres)
Aplicar migrations manualmente (recomendado quando não usar auto-migrate):
```bash
dotnet ef database update \
  --project backend/Araponga.Infrastructure \
  --startup-project backend/Araponga.Api
```

Auto-migrate (opcional, desligado por padrão): defina `Persistence__ApplyMigrations=true`.

---

## 🔐 Autenticação (JWT)

A autenticação utiliza JWT assinado (HS256). O fluxo principal é:
1. `POST /api/v1/auth/social` retorna o token JWT.
2. Use `Authorization: Bearer <token>` em endpoints protegidos.

Exemplo rápido:
```bash
curl -s -X POST http://localhost:8080/api/v1/auth/social \
  -H "Content-Type: application/json" \
  -d '{
    "provider":"google",
    "externalId":"demo-user",
    "displayName":"Demo",
    "cpf":"123.456.789-00",
    "foreignDocument":null,
    "phoneNumber":"(11) 99999-0000",
    "address":"Rua das Flores, 100",
    "email":"demo@araponga.com"
  }' | jq
```

Configuração JWT (via `appsettings`/env):
```
Jwt__Issuer, Jwt__Audience, Jwt__SigningKey, Jwt__ExpirationMinutes
```
> A chave padrão é **apenas para DEV**. Troque em produção.

---

## 👥 Membership (visitor → resident)

- Declaração: `POST /api/v1/territories/{territoryId}/membership`
- Status atual: `GET /api/v1/territories/{territoryId}/membership/me`

Regras:
- Se já existe `VISITOR` e o usuário pede `RESIDENT`, o vínculo é atualizado para `RESIDENT` com status `PENDING`.
- Se já é `RESIDENT` e pede `VISITOR`, mantém `RESIDENT` (sem downgrade).
- Se já é o mesmo papel, a operação é idempotente.

---

## 📍 PresencePolicy (geo headers)

Configuração: `PresencePolicy:Policy` (`None | ResidentOnly | VisitorAndResident`).

Padrão: `ResidentOnly` (somente `RESIDENT` exige geo).
- `VisitorAndResident`: visitor também exige `X-Geo-Latitude` / `X-Geo-Longitude`.
- `None`: nenhum vínculo exige geo.

---

## 🧾 Headers e contexto (MVP)

- **X-Session-Id**: identifica a sessão do cliente e permite selecionar o território ativo.
  - Usado para `POST /api/v1/territories/selection` e como fallback de `territoryId` em feed/mapa.
  - Também sustenta ações anônimas (ex.: likes com `session:{id}`).
- **X-Geo-Latitude / X-Geo-Longitude**: presença física mínima.
  - Obrigatório conforme `PresencePolicy` (default: somente para `RESIDENT`).
  - Obrigatório para criar posts (GeoAnchors).

### Portal de autosserviço

A página inicial da API (`/`) serve um portal estático com explicação do produto,
domínios, fluxos e quickstart. Em desenvolvimento, acesse também:

- `/swagger` (documentação da API)
- `/health` (status simples)

Quando a API está rodando localmente em ambiente de desenvolvimento, o portal
exibe um preview do Swagger para navegação e testes rápidos.

Para publicação como site estático, o portal também está disponível em `docs/` e
pode ser hospedado via GitHub Pages (basta apontar a origem para a pasta `docs`).
A versão do GitHub Pages inclui links diretos para documentação, user stories e changelog.

## 🤝 Contribuindo

Consulte o guia em [`CONTRIBUTING.md`](CONTRIBUTING.md).

O Araponga é aberto à colaboração, especialmente de pessoas interessadas em:

tecnologia com impacto social

comunidades locais

território, cultura e soberania

arquitetura de software consciente

regeneração e autonomia

Formas de contribuir:

código

testes

documentação

ideias

feedback conceitual

Antes de abrir PRs grandes, abra uma issue para alinharmos a direção.

## 🌎 Visão de futuro

Algumas direções possíveis (não promessas fechadas):

economias e moedas locais

trocas de serviços comunitários

governança distribuída

integração com iniciativas regenerativas

tecnologia como guardiã do território, não como exploradora

O Araponga não quer crescer rápido.
Quer criar raízes profundas.

✨ Uma nota pessoal

Este projeto nasce de uma escuta atenta:

da vida

do território

das comunidades

e dos limites do modelo digital atual

Se você chegou até aqui e sentiu que isso faz sentido,
você já faz parte da conversa.

## 📜 Licença

Este projeto é distribuído sob uma **licença aberta orientada à comunidade e ao território**.

- Versão oficial (EN): `LICENSE`
- Versão em português (PT-BR): `LICENSE.pt-BR`

🐦 Araponga canta para avisar, proteger e comunicar.
Que esta plataforma faça o mesmo.
