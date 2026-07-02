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
3. **Corpo do PR**: descrição clara + `Spec-Id: <id>` quando fase S0+; usar template `.github/pull_request_template.md`.
4. **Commit**: `feat|fix|docs|refactor|test|chore(module): descrição`.
5. Nunca merge automático; humano faz o merge após CI verde + bots resolvidos.
