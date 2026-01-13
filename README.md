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

### Desenvolvimento
- [Plano de Implementação](./docs/20_IMPLEMENTATION_PLAN.md)
- [Revisão de Código](./docs/21_CODE_REVIEW.md)
- [Análise de Coesão e Testes](./docs/22_COHESION_AND_TESTS.md)

### Operações
- [Moderação](./docs/30_MODERATION.md)
- [Admin e Observabilidade](./docs/31_ADMIN_OBSERVABILITY.md)

- **Membership**  
  Relação entre uma pessoa e um território (morador, visitante, etc.).

- **Feed / Map**  
  Informação contextual, sempre relacionada a um território específico.

---

## 🚀 Estado atual do projeto

- ✅ Backend inicial estruturado
- ✅ Autenticação social com JWT e gestão básica de usuários
- ✅ Descoberta e seleção de territórios
- ✅ Vínculos (morador e visitante) com regras de visibilidade
- ✅ Feed territorial com criação e moderação de conteúdo
- ✅ Mapa territorial com entidades e relações
- ✅ Moderação (reports e bloqueios)
- ✅ Notificações in-app com outbox e inbox persistido
- ✅ Feature flags e health check
- ✅ Testes automatizados
- ✅ CI configurado com builds reprodutíveis (`packages.lock.json`)
- 🚧 Frontend e experiências móveis em planejamento

O projeto está em **evolução ativa**, com foco em solidez antes de escala.

---

## 🛠️ Como rodar localmente

> A documentação canônica de operação está em [`docs/README.md`](docs/README.md).

### InMemory (padrão)
```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project backend/Araponga.Api
```

### Postgres (docker compose)
```bash
docker compose up --build
```

### Migrations (Postgres)
```bash
dotnet ef database update \
  --project backend/Araponga.Infrastructure \
  --startup-project backend/Araponga.Api
```

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

Consulte o guia em [`docs/41_CONTRIBUTING.md`](./docs/41_CONTRIBUTING.md).

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

## Developer Portal (GitHub Pages)

O conteúdo estático do Developer Portal vive em `backend/Araponga.Api/wwwroot/devportal` e é publicado automaticamente via GitHub Actions na branch `gh-pages` quando há push em `main` ou `master`.

## 📜 Licença

Este projeto é distribuído sob uma **licença aberta orientada à comunidade e ao território**.

- Versão oficial (EN): `LICENSE`
- Versão em português (PT-BR): `LICENSE.pt-BR`

🐦 Araponga canta para avisar, proteger e comunicar.
Que esta plataforma faça o mesmo.
