# Fase 59: Federação entre Instâncias (Opt-in)

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🟡 P2 → P1 na Onda S3  
**Onda**: S3 — Rede & federação  
**Depende de**: FASE53, FASE58  
**Estimativa Total**: 140 horas  
**Status**: ⏳ Pendente  
**Handoff**: [Anexo O4](../handoff/arquitetura-c4/Anexo%20Handoff%20-%20Operacao%20Instancias%20e%20Federacao.dc.html)

---

## Objetivo

Permitir que territórios em instâncias distintas **conversem opt-in**: identidade federada, diretório global, mensagens cross-território e marketplace inter-territorial.

**Princípio**: território é fronteira de dados; federação é escolha da governança local.

---

## Capacidades federadas

| Capacidade | Descrição |
|------------|-----------|
| Identidade federada | `globalUserId` no Core; perfil local por território |
| Diretório | App descobre territórios próximos em outras instâncias |
| Mensagens | Eventos assinados instância↔instância |
| Marketplace inter-territorial | Opt-in; taxa/split de origem preservados |

---

## Modo soberano (self-hosted)

- Implementador hospeda stack própria
- Conecta ao Core por federação
- Recebe pacote Helm/Compose + runbook
- Responsável por uptime, backup, patches

---

## Endpoints

```
POST /federation/links
GET  /federation/identity/{globalUserId}
POST /federation/events
```

---

## Entidade

`FederationLink` — fromTerritoryId, toTerritoryId, scopes[], status

---

## Critérios de aceite

- [ ] Morador loga em território B com conta de território A
- [ ] Território publica no diretório; app descobre via Core
- [ ] DM cross-território com eventos assinados
- [ ] Federação desligável pela governança do território
- [ ] Modo soberano provisionado com guia completo
- [ ] Dados pessoais não cruzam sem consentimento

---

## Referências

- [FASE53](./FASE53.md) · [REALINHAMENTO](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
