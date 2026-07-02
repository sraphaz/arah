---
name: arah-domain-consult
description: Consulta os pareceres dos agentes de domínio do Arah (territorio, marketplace, monetizacao, design-ux, etc.). Use antes de concluir mudanças em código de domínio ou quando precisar do parecer de um domínio específico.
---

# domain-consult (Arah)

Comunicação passiva: o hook `stop` já gera os pareceres automaticamente em
`.cursor/domain-review.md`. **Primeiro leia esse arquivo** — na maioria dos
casos ele já contém o parecer atualizado para as mudanças da árvore de trabalho.

## Regeneração manual (apenas se necessário)

```powershell
./scripts/agents/domain-autoreview.ps1 -Force -Json
```

Parecer de um domínio específico:

```powershell
./scripts/agents/post-domain-consult.ps1 -DomainId mercado-economia -ChangedFiles <arquivos> -Trigger manual
```

## Critério

Endereçar cada item de "Validar no PR" do parecer aplicável à mudança antes de
concluir. Detalhes: [docs/governance/DEFINITION_OF_DONE.md](../../docs/governance/DEFINITION_OF_DONE.md).
