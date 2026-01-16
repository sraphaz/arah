# Plano de Implementação (Prioridades e Dependências)

**Última Atualização**: 2025-01-16  
**Status**: MVP Completo + Fases 1-8 Implementadas ✅

Este documento organiza **épicos → features → user stories** em ordem de prioridade
com dependências explícitas para orientar a sequência de entrega.

**Nota**: Para o backlog completo de fases futuras (9-24), ver [Backlog API](./backlog-api/README.md).

## Legenda
- **P0**: crítico para o MVP.
- **P1**: importante para estabilidade do MVP.
- **Depende de**: requisitos técnicos/funcionais necessários antes da execução.

---

## Fase 0 — Fundação (P0)
### Epic: Cadastro, autenticação e presença
- **Feature: Cadastro e login obrigatórios para consulta**
  - Stories: **[MVP][P0] Cadastro e autenticação**
  - Depende de: infraestrutura mínima de auth.

- **Feature: Presença e localização**
  - Stories: **[MVP][P0] Descobrir territórios próximos**
  - Depende de: permissões de localização e consentimento.

---

## Fase 1 — Território e vínculo (P0/P1)
### Epic: Território e vínculo
- **Feature: Descoberta de território por presença**
  - Stories: **[MVP][P0] Descobrir territórios próximos**
  - Depende de: cadastro/autenticação.

- **Feature: Vínculo visitor/resident**
  - Stories: **[MVP][P0] Declarar vínculo visitor/resident**
  - Stories: **[MVP][P1] Consultar meu vínculo**
  - Depende de: descoberta de território + localização válida.

- **Feature: Consulta de território**
  - Stories: **[MVP][P1] Consultar território por ID**
  - Depende de: autenticação.

---

## Fase 2 — Feed e mapa integrados (P0)
### Epic: Feed e mapa integrados
- **Feature: Feed do território**
  - Stories: **[MVP][P0] Feed do território**
  - Depende de: vínculo visitor/resident e regras de visibilidade.

- **Feature: Postagem com GeoAnchor**
  - Stories: **[MVP][P0] Post com GeoAnchor**
  - Depende de: feed do território + geolocalização opcional via mídias.

- **Feature: Mapa + feed integrados**
  - Stories: **[MVP][P0] Sincronia feed ↔ mapa**
  - Depende de: posts com GeoAnchor + integração base com provedor de mapas.

- **Feature: Feed pessoal**
  - Stories: **[MVP][P1] Feed pessoal**
  - Depende de: criação de posts.

---

## Fase 3 — Moderação e segurança (P0/P1)
### Epic: Moderação e segurança
- **Feature: Reports e bloqueio**
  - Stories: **[MVP][P0] Reportar post**
  - Stories: **[MVP][P0] Reportar usuário**
  - Stories: **[MVP][P0] Bloquear usuário**
  - Depende de: feed e perfis de usuários.

- **Feature: Moderação automática (básica)**
  - Stories: **[MVP][P1] Moderação automática simples**
  - Stories: **[MVP][P1] Sanções territoriais e globais**
  - Depende de: reports + regras de visibilidade + auditoria mínima.

---

## Fase 4 — Integrações e observabilidade (P1)
### Epic: Integrações e observabilidade técnica
- **Feature: Provedor de mapas**
  - Stories: **[MVP][P1] Integração de mapas (base)**
  - Depende de: feed + GeoAnchors.

- **Feature: Observabilidade mínima**
  - Stories: **[MVP][P1] Observabilidade técnica mínima**
  - Depende de: fluxo de auth, localização, reports/moderação.

---

## Pós-MVP (registro de próximos passos)
- Friends (círculo interno) e stories.
- GeoAnchor avançado (memórias, galeria, pins visuais).
- Mapa de entidades além de posts.
- Indicadores de saúde territorial e alertas ambientais.
- Admin/observabilidade ampliada.
- Produtos/serviços territoriais e integrações externas.
