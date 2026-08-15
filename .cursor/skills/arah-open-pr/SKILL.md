---
name: arah-open-pr
description: Abre PR no repo Arah com validação completa (build, testes, docs, spec). Use ao finalizar uma tarefa que deve virar Pull Request.
---

# open-pr (Arah)

Consulte [.skills/open-pr.skill.yaml](../../.skills/open-pr.skill.yaml).

## Procedimento

1. **Validar** (obrigatório — nunca abrir PR quebrado):

```powershell
git fetch origin main; git merge origin/main   # resolver conflitos
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
```

2. **Docs no mesmo PR**: `docs/CHANGELOG.md`, `docs/STATUS_FASES.md`, fase/spec quando aplicável (skill `arah-sync-docs`).
3. **Corpo do PR**: usar [.agents/templates/pr-body.md](../../.agents/templates/pr-body.md) + `.github/pull_request_template.md`.
   - Incluir `Spec-Id: <id>` quando fase S0+.
   - Preencher **Pareceres endereçados** (obrigatório): DomainId + como foi atendido — sem isso o ciclo fica só em sinalização.
4. **Commit**: `feat|fix|docs|refactor|test|chore(module): descrição`.
5. Após Orchestrate: ler `<!-- arah-pr-graph -->` e pareceres `arah-domain-consult:*`; responder bots/threads.
6. Nunca merge automático; humano faz o merge após `pr-ready` / `ready-for-merge` (CI verde + threads de review resolvidas).
