# Ingestão de dados – seed Camburi

O conteúdo do território **Camburi** (São Sebastião, SP) é populado por **ingestão de dados**, não por lógica de seed dentro da aplicação.

## O que é ingerido

- **Território** Camburi (centroide -23.76281, -45.63691, raio 3,5 km), se ainda não existir.
- **Usuário** seed "Comunidade Camburi" (auth `seed` / `seed-camburi`).
- **Membership** desse usuário no território (Resident).
- **3 posts** no feed do território.
- **2 eventos** no território.
- **2 alertas de saúde** no território (base para testar a funcionalidade de alertas no front).

## Como executar

Para um **quick start** completo (API + BFF + seeds + app), veja [docs/STABLE_RELEASE_APP_ONBOARDING.md](../../docs/STABLE_RELEASE_APP_ONBOARDING.md#como-rodar-o-projeto-getting-started).

### 0) Stack local (automático)

Ao subir o stack com `.\scripts\run-local-stack.ps1` (Docker), o script **executa o seed automaticamente** no container Postgres usando `docker cp` + `psql -f` (UTF-8 preservado; não usa pipe do PowerShell). Não é necessário ter `psql` instalado no host.

### 1) Só SQL (manual)

Com **psql** e conexão ao Postgres (ex.: banco já criado pelo Docker):

```bash
# Linux/macOS
psql -h localhost -p 5432 -U araponga -d araponga -f scripts/seed/seed-camburi.sql

# Windows (PowerShell)
& "C:\Program Files\PostgreSQL\16\bin\psql.exe" -h localhost -p 5432 -U araponga -d araponga -f scripts/seed/seed-camburi.sql
```

Ou use o script PowerShell que descobre o `psql` e usa variáveis de ambiente (ou padrões):

```powershell
.\scripts\seed\run-seed-camburi.ps1
```

O SQL é **idempotente**: pode rodar mais de uma vez; território e usuário não duplicam, e posts/eventos só são inseridos se ainda não existirem.

### 2) Território pela API e depois SQL

Se preferir criar o território pela API administrativa:

1. `POST /api/v1/admin/seed/territories/camburi` (com usuário admin autenticado).
2. Depois rode o SQL acima. O script detecta o território "Camburi" em São Sebastião/SP e associa usuário, posts e eventos a ele (mesmo que o território tenha sido criado pela API com outro ID).

## Arquivos

| Arquivo | Descrição |
|--------|-----------|
| `seed-camburi.sql` | Script SQL de ingestão (território opcional + usuário + membership + posts + eventos). |
| `seed-boicucanga.sql` | Script SQL: território Boiçucanga (São Sebastião, SP), polígono 11 vértices + usuário seed, membership, 3 posts, 2 eventos, 2 alertas (base mínima para testar feed, eventos e alertas no front). |
| `run-seed-camburi.ps1` | Script PowerShell que executa o SQL (usa `psql` e variáveis de ambiente ou padrões). |
| `README.md` | Este arquivo. |

O `run-local-stack.ps1` executa em sequência `seed-camburi.sql` e `seed-boicucanga.sql` para permitir testar a alternância entre territórios na tela de seleção.

## Variáveis de ambiente (opcional)

Para `run-seed-camburi.ps1` ou para montar o comando `psql` manualmente:

- `POSTGRES_HOST` (default: `localhost`)
- `POSTGRES_PORT` (default: `5432`)
- `POSTGRES_DB` (default: `araponga`)
- `POSTGRES_USER` (default: `araponga`)
- `POSTGRES_PASSWORD` (default: `araponga`)

Se o backend estiver no Docker com `docker-compose`, use `Host=localhost` e a porta exposta do Postgres (ex.: 5432).
