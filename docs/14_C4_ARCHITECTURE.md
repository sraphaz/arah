---
title: Arquitetura C4
description: Modelo C4 da plataforma Arah — Contexto, Containers, Componentes, mapa de domínios e implantação.
---

# Arquitetura C4 da Plataforma Arah

Este documento descreve a arquitetura da plataforma **Arah** usando o **modelo C4**
(Contexto → Containers → Componentes), além do mapa de domínios, da topologia de
implantação e dos diagramas de sequência das funcionalidades implementadas e previstas.

> 📐 **Documento interativo completo**: a versão navegável (com todos os diagramas de
> sequência, zoom e legendas) é publicada no **site técnico (DevPortal)** em
> [`/architecture/`](/architecture/) (ex.: `https://devportal.arah.app/architecture/`).
> Quando a API está rodando localmente, também fica disponível em
> `http://localhost:5178/devportal/architecture/`.

Princípio transversal: **tudo é escopado pelo território ativo** — nada é global por
padrão. Visibilidade e papéis (visitante, morador, curador) são sempre impostos pelo
servidor.

---

## Nível 1 · Contexto

Quem usa o Arah e com quais sistemas externos ele conversa. Três papéis humanos —
**visitante**, **morador** e **curador** — operam a plataforma a partir do app. O papel
é sempre por território.

![Diagrama de Contexto (C4 Nível 1)](/wiki/architecture/c4-context.webp)

- **Visitante** — vê o feed público e o mapa do território.
- **Morador** — publica, vende, vota e participa da vida local.
- **Curador** — modera, confirma residências e abre eleições.
- **Sistemas externos**: Google Identity (login social/OAuth), OpenStreetMap (tiles do
  mapa), Provedor PIX/PSP (pagamentos e repasses), FCM/APNs (push) e GPS do dispositivo
  (localização privada — nunca exposta a outros usuários; serve apenas para sugerir o
  território e estimar presença).

---

## Nível 2 · Containers

As peças executáveis. O app fala com a API .NET por REST sobre HTTPS (JWT), recebe
tempo-real pelo gateway e envia mídia direto ao storage via URL assinada. A API orquestra
banco, storage, workers e os serviços externos.

![Diagrama de Containers (C4 Nível 2)](/wiki/architecture/c4-containers.webp)

- **App Mobile** — Flutter · Material 3 · Riverpod (o produto principal).
- **Portal Web** — front-end web complementar.
- **API .NET** — DDD, REST `/v1`, escopo territorial, regras de papel/visibilidade,
  paginação por cursor e idempotência. Existe ainda um **BFF** que expõe jornadas
  (`/api/v2/journeys/*`) consumidas pelo app.
- **Realtime Gateway** (WebSocket/SSE), **Background Workers** (push/reconciliação),
  **Banco relacional** (PostgreSQL) e **Object Storage** (mídia/imagens).

---

## Nível 3 · Componentes

### App Flutter

Camadas (Clean Architecture + Riverpod): apresentação (telas, navegação), aplicação
(controllers), domínio (entidades) e dados (repositórios, API client, cache local,
outbox offline, realtime client).

![Componentes do App Flutter (C4 Nível 3)](/wiki/architecture/c4-components-app.webp)

### API .NET

Pipeline de requisição (controllers v1, auth/JWT, escopo territorial, validação & papel,
idempotência) e módulos de domínio DDD (Identity & Auth, Territories, Membership,
Feed & Content, Map & Entities, Events, Marketplace, Payments, Governance, Moderation,
Messaging, Notifications, Media) — além dos módulos de economia & serviços previstos.

![Componentes da API .NET (C4 Nível 3)](/wiki/architecture/c4-components-api.webp)

---

## Mapa de domínios

Visão holística de todas as funcionalidades, organizadas por área e sempre dentro do
escopo do **território ativo**: Core (Identity & Auth, Membership), Conteúdo & lugar,
Economia & mercado, Governança e Vida & dados — apoiadas por um barramento de serviços
compartilhados (Payments, Notifications, Media, Realtime).

![Mapa de domínios](/wiki/architecture/c4-domain-map.webp)

---

## Implantação e sequências

O documento interativo inclui ainda a **topologia de implantação** e os **diagramas de
sequência** de cada funcionalidade (autenticação & sessão, onboarding territorial, feed,
publicação com mídia, troca de território, mapa, eventos, mensagens em tempo real,
notificações & deep link, perfil, mercado & checkout PIX, minha loja, eleições & votação,
curadoria & moderação, renovação de token), além das jornadas previstas.

➡️ Consulte a versão navegável em [`/architecture/`](/architecture/) (DevPortal).

---

## Fontes e modelos como código

- Documento interativo (C4 + sequências): `frontend/devportal/architecture/` (publicado em `/architecture/`; via API local em `/devportal/architecture/`).
- DSL Structurizr / C4 (fonte): [`design/Archtecture/C4_Context.md`](https://github.com/sraphaz/arah/blob/main/design/Archtecture/C4_Context.md), `C4_Containers.md`, `C4_Components.md`.
- Modelo LikeC4 (arquitetura como código): `docs/design/arah.likec4` (ver `docs/design/README_LIKEC4.md`).
- Modelo de dados (MER): `design/Archtecture/MER.md`.

## Documentos relacionados

- [Decisões Arquiteturais (ADRs)](/docs/10_ARCHITECTURE_DECISIONS)
- [Serviços da Arquitetura](/docs/11_ARCHITECTURE_SERVICES)
- [Modelo de Domínio](/docs/12_DOMAIN_MODEL)
- [Roteamento de Domínios](/docs/13_DOMAIN_ROUTING)
