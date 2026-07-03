---
name: arah-craft
description: Disciplina de craftsmanship (Clean Code, Clean Architecture, SOLID, TDD) no ciclo projetar -> desenvolver -> testar no repo Arah. Use ANTES e DURANTE mudanças de código (backend .NET, Flutter, Next.js) e ao planejar como algo será programado e testado, não só na revisão do PR.
---

# craft-review (Arah) — Uncle Bob no ciclo projetar → desenvolver → testar

Consulte [.skills/craft-review.skill.yaml](../../.skills/craft-review.skill.yaml).
Complementa `architecture-review` (fronteiras/ADR) e `code-review` (checklist de PR);
não substitui linter/formatter nem os testes automatizados.

## Quando acionar

- Ao **projetar** uma feature: decidir fronteiras, responsabilidade e se um padrão se justifica.
- Ao **desenvolver**: manter funções pequenas, nomes que revelam intenção, `Result<T>`.
- Ao **testar**: TDD quando aplicável, cobertura > 90%, teste para toda mudança de comportamento.

## Fases

**Projetar** — Dependency rule (Api → Infra → App → Domain); Single Responsibility;
padrão só na 3ª duplicação / 2º eixo de mudança (sem cargo cult); YAGNI/KISS.

**Desenvolver** — funções curtas (um nível de abstração); nomes de intenção
(`territory`/`items`/`membership`); `Result<T>` em vez de null; async com
`CancellationToken`; DRY; sem comentários que narram o óbvio.

**Testar** — toda mudança de comportamento tem teste; cobertura > 90% na área
alterada; AC da spec ligado a `covered_by` (rastreabilidade fina).

## Comando

```powershell
./scripts/agents/invoke-skill.ps1 -Skill craft-review
./scripts/agents/craft-review-check.ps1 -ChangedFiles backend/Arah.Domain/Foo.cs
```

## Critério

Soft por padrão (avisos + checklist por fase, exit 0). Use `-Strict` para tratar
avisos como erro quando quiser promover a gate duro.

## Referências

- [docs/governance/21_CODE_REVIEW.md](../../docs/governance/21_CODE_REVIEW.md)
- [docs/governance/22_COHESION_AND_TESTS.md](../../docs/governance/22_COHESION_AND_TESTS.md)
- [docs/governance/DEFINITION_OF_DONE.md](../../docs/governance/DEFINITION_OF_DONE.md)
