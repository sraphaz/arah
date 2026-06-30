# Handoff — Arquitetura C4 e Sustentação Operacional

**Versão**: 1.0  
**Data**: 2026-06-30  
**Origem**: Pacote *Arquitetura C4 Arah* (modelo C4, monetização, operação multi-instância, cockpit do implementador)

Este diretório concentra a documentação interativa de handoff que define **o que sustenta operacionalmente a plataforma** — além das funcionalidades comunitárias do backlog original.

---

## Documentos interativos (HTML)

Abra no navegador (servidor local ou extensão Live Server). Todos compartilham o design system Arah em `_ds/`.

| Documento | Arquivo | Público |
|-----------|---------|---------|
| **Operação por agentes** | [Operacao por Agentes - Arah.dc.html](./Operacao%20por%20Agentes%20-%20Arah.dc.html) | Engenharia, Cursor, PM |
| **Arquitetura C4** | [Arquitetura C4 Arah.dc.html](./arquitetura-c4/Arquitetura%20C4%20Arah.dc.html) | Backend, mobile, arquitetura |
| **Plano operacional & pipelines** | [Plano Operacional e Pipelines - Arah.dc.html](./arquitetura-c4/Plano%20Operacional%20e%20Pipelines%20-%20Arah.dc.html) | DevOps/SRE, plataforma |
| **Operação, instâncias & federação** | [Anexo Handoff - Operacao Instancias e Federacao.dc.html](./arquitetura-c4/Anexo%20Handoff%20-%20Operacao%20Instancias%20e%20Federacao.dc.html) | Backend, infra, plataforma |
| **Adendo de monetização** | [Adendo de Monetizacao - Handoff Arah.dc.html](./arquitetura-c4/Adendo%20de%20Monetizacao%20-%20Handoff%20Arah.dc.html) | Produto, backend, financeiro |
| **Cockpit do implementador** | [Cockpit do Implementador Arah.dc.html](./arquitetura-c4/Cockpit%20do%20Implementador%20Arah.dc.html) | Frontend web, produto |
| **Programa de implementadores** | [Programa de Implementadores Arah.dc.html](./arquitetura-c4/Programa%20de%20Implementadores%20Arah.dc.html) | Produto, operação, site |
| **Backlog atualizado (impacto)** | [Backlog Atualizado - Arah.dc.html](./arquitetura-c4/Backlog%20Atualizado%20-%20Arah.dc.html) | Planejamento, PM |

---

## Uso no site e portal de implementadores

Estes HTMLs podem ser:

- **Embutidos ou linkados** no portal institucional (`frontend/arah.site` ou equivalente) na seção *Para implementadores*
- **Referenciados** no DevPortal (`frontend/devportal/`) como documentação de arquitetura e operação
- **Publicados** como artefatos estáticos no wiki interno

Para servir localmente:

```powershell
cd docs/handoff/arquitetura-c4
python -m http.server 8080
# Abrir http://localhost:8080/Arquitetura%20C4%20Arah.dc.html
```

- [docs/ops/AGENT_OPERATION.md](../ops/AGENT_OPERATION.md) — infraestrutura implementada
- [AGENTS.md](../../AGENTS.md) — manual dos agentes

---

## Integração com o backlog

**Ordem de execução**: infraestrutura de agentes (PR 1–4) → **depois** Onda S / FASE52+.

O realinhamento oficial do backlog está em:

- **[REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md](../backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)** — ondas, prioridades, impacto nas fases existentes
- **Fases 52–61** — épicos de sustentação mapeados para implementação

---

## Princípios (resumo)

1. **Morador nunca paga** — cobrança só em atividade comercial e serviços opcionais
2. **Núcleo aberto** — receita por operação e serviço, não paywall de funcionalidade essencial
3. **Território como fronteira de dados** — federação opt-in; Core coordena, não possui conteúdo
4. **Transparência por padrão** — taxa, split e repasses visíveis antes e depois do pagamento
5. **A sustentação não atrasa o comunitário** — viabiliza financiá-lo; funcionalidades ganham ganchos de receita

---

**Referências cruzadas**: [Roadmap](../02_ROADMAP.md) · [Backlog API](../backlog-api/README.md) · [Modelo de domínio](../12_DOMAIN_MODEL.md) · [ADRs](../10_ARCHITECTURE_DECISIONS.md)
