# Seeds — territórios e massa de teste

## Territórios padrão (litoral SP)

| Arquivo | Região | Centro aprox. |
|---------|--------|---------------|
| `seed-camburi.sql` | Camburi, São Sebastião/SP | -23.76, -45.64 |
| `seed-boicucanga.sql` | Boiçucanga, São Sebastião/SP | -23.78, -45.59 |

Aplicados automaticamente por `scripts/run-local-stack.ps1`.

## Massa de socorro (capital e outras regiões)

Se você testa **longe do litoral** (ex.: Grande São Paulo, interior, outro estado), o onboarding com raio de **10 km** não encontra Camburi/Boiçucanga.

### Preset São Paulo capital

```powershell
.\scripts\seed\run-seed-territory-local.ps1 -Preset sao-paulo-centro
```

Cria **Centro — São Paulo/SP** (Paulista, Sé, Pinheiros…) com raio 12 km.

Também roda no `run-local-stack.ps1` após os seeds do litoral.

### Contorno oficial IBGE (recomendado para município inteiro)

Para desenho geométrico **alinhado à fonte governamental** (limites legais do município, SIRGAS 2000):

```powershell
.\scripts\seed\fetch-ibge-municipality-boundary.ps1 -City Socorro -Uf SP -Apply
```

O script:

1. Resolve o **código IBGE** via API de Localidades (`3552106` = Socorro/SP).
2. Baixa a **malha municipal** via API de Malhas v3 (GeoJSON).
3. Grava `BoundaryPolygonJson` no Postgres (contorno no mapa do app).
4. Gera SQL reutilizável em `scripts/seed/seed-{cidade}-{UF}-ibge.sql`.

**Fontes oficiais:**

| Fonte | Uso |
|-------|-----|
| [IBGE — API Localidades](https://servicodados.ibge.gov.br/api/docs/localidades) | Nome → código do município |
| [IBGE — API Malhas v3](https://servicodados.ibge.gov.br/api/docs/malhas?versao=3) | Polígono simplificado (web/mobile) |
| [IBGE — Malha Municipal (SHP)](https://www.ibge.gov.br/geociencias/organizacao-do-territorio/malhas-territoriais.html) | Download completo para GIS |

Parâmetros úteis: `-Resolucao` (0–5), `-Qualidade` (minima \| intermediaria \| maxima), `-IbgeCode`, `-OutputSql`.

### Sua cidade (círculo aproximado, sem IBGE)

1. Abra Google Maps → clique direito no seu bairro → copie lat/lng.
2. Execute:

```powershell
.\scripts\seed\run-seed-territory-local.ps1 `
  -Name "Centro" `
  -City "Sua Cidade" `
  -State "UF" `
  -Latitude -25.4284 `
  -Longitude -49.2733 `
  -RadiusKm 10
```

Requisito: Postgres no ar (`docker compose` ou `run-local-stack.ps1`).

## Produto (pendente)

Quando **não houver território próximo**, o app deve oferecer cadastro da região (cidade + coordenadas), não deixar o usuário preso no onboarding.

**Visão:** onboarding detecta lista vazia → reverse geocode (cidade/UF) → busca malha IBGE → cria território com `BoundaryPolygon` → usuário confirma e entra.

- API: `POST /api/v1/territories/suggestions` (já aceita polígono ou raio).
- Falta: serviço backend `IbgeBoundaryResolver`, jornada BFF e UI no app.

Ver `frontend/arah.app/docs/CORRECOES_PENDENTES.md` §5 e §6.
