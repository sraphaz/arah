# Piloto — 1ª instância gerenciada (FASE54)

Stack mínimo para reproduzir instância do zero em ambiente local/staging.

## Uso

```powershell
docker compose -f infrastructure/pilot/docker-compose.pilot.yml up -d --build
./scripts/provision/register-core-instance.ps1 `
  -ApiBaseUrl http://localhost:8080 `
  -InstanceBaseUrl http://localhost:8080 `
  -AdminToken '<jwt-system-admin>'
```

## Pipeline (ordem de dependência)

1. **FASE52** — CI/CD + imagens GHCR ✅
2. **FASE53** — registro no Core ✅
3. **FASE54** — este compose + registro piloto
4. Migrar schema + seed território (script futuro)
5. Health check verde → publicar no app

## Referências

- [FASE54.md](../../docs/backlog-api/FASE54.md)
- [FASE53 — Arah Core](../../docs/backlog-api/FASE53.md)
