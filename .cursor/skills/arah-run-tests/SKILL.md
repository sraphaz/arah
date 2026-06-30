---
name: arah-run-tests
description: Roda testes da área alterada no repo Arah (dotnet, flutter, web). Use após mudanças de código ou antes de abrir PR.
---

# run-tests (Arah)

Consulte [.skills/run-tests.skill.yaml](../../.skills/run-tests.skill.yaml).

## Comandos

```powershell
./scripts/agents/invoke-skill.ps1 -Skill run-tests -Area backend
./scripts/agents/invoke-skill.ps1 -Skill run-tests -Area bff
./scripts/agents/invoke-skill.ps1 -Skill run-tests -Area flutter
./scripts/agents/invoke-skill.ps1 -Skill run-tests -Area web
```

## Critério

Exit code 0. Se falhar, corrigir antes do PR.
