# Fluxo de release – Araponga

Como fazer release de versão, atualizar versão e publicar artefatos.

---

## Versão

- **App Flutter:** `frontend/araponga.app/pubspec.yaml` → campo `version` (ex.: `0.1.0+1` → semver + build number).
- **Formato:** `MAJOR.MINOR.PATCH+BUILD` (ex.: `1.2.0+3`). A tag usa só `vMAJOR.MINOR.PATCH` (ex.: `v1.2.0`).
- **Backend/API:** versão pode ser refletida em assembly ou em documentação; o CD publica a imagem Docker com tag `$sha` e `latest`.

---

## Passos para um release

### 1. Atualizar versão do app

Edite `frontend/araponga.app/pubspec.yaml`:

```yaml
version: 1.0.0+1   # 1.0.0 = versão do release; +1 = build number (incrementar a cada build de store)
```

(Opcional) Atualize `docs/CHANGELOG.md` ou o changelog do repositório com as mudanças da versão.

### 2. Commit e push

```bash
git add frontend/araponga.app/pubspec.yaml
git commit -m "chore: bump app version to 1.0.0"
git push origin main
```

### 3. Criar e enviar a tag

Use **Semantic Versioning** (ex.: `v1.0.0`):

```bash
git tag v1.0.0
git push origin v1.0.0
```

- **Disparos automáticos:**
  - O workflow **CD** roda (build API, Android APK/AAB, iOS).
  - O workflow **CD** inclui o job **GitHub Release**: cria uma release com o nome da tag e anexa os artefatos do app (APK, AAB, iOS).

### 4. Conferir o release

- **Actions:** aba *Actions* → workflow **CD** e **Release** (se configurado).
- **Release:** aba *Releases* do repositório → release da tag (ex.: `v1.0.0`) com os arquivos anexados para download.

---

## Quando usar cada número de versão

| Mudança | Exemplo |
|--------|---------|
| Bug fixes, pequenas melhorias | `1.0.0` → `1.0.1` |
| Novas funcionalidades compatíveis | `1.0.1` → `1.1.0` |
| Mudanças incompatíveis (breaking) | `1.1.0` → `2.0.0` |

---

## Resumo

| Etapa | O quê |
|-------|--------|
| 1 | Ajustar `version` em `frontend/araponga.app/pubspec.yaml` (e changelog se usar). |
| 2 | Commit + push em `main`. |
| 3 | `git tag vX.Y.Z` e `git push origin vX.Y.Z`. |
| 4 | CD gera artefatos; Release (se ativo) publica a release com os anexos. |
