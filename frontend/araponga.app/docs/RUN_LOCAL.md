# Rodar o app + BFF + API localmente (stack integrado)

Para **testar e debugar** o app Flutter apontando para o BFF local, que por sua vez aponta para a API, use o stack local integrado.

## Configuração do ambiente (uma vez)

Na raiz do repositório, rode o setup para criar/ajustar o `.env` (ex.: JWT para a API, Postgres, Redis, MinIO):

```powershell
.\scripts\setup-env.ps1
```

Isso copia `.env.example` para `.env` (se não existir) e gera um `JWT_SIGNINGKEY` seguro para desenvolvimento. O **run-local-stack.ps1** também chama esse passo automaticamente se não houver `.env`.

Para recriar o `.env` a partir do exemplo: `.\scripts\setup-env.ps1 -Force`

## Visão geral

```
[App Flutter]  -->  http://localhost:5001 (BFF)  -->  http://localhost:8080 (API)
                         dotnet run                      Docker (api + postgres + redis + minio)
```

- **API**: roda em Docker (porta 8080). Swagger: http://localhost:8080/swagger  
- **BFF**: roda com `dotnet run` (porta 5001). O app usa esta URL por padrão.  
- **App**: `flutter run` com `BFF_BASE_URL=http://localhost:5001` (já é o default em `app_config.dart`).

## Passo a passo

### 1. Subir API e BFF (um comando)

Na **raiz do repositório** (use `.\` — no PowerShell é obrigatório para scripts na pasta atual):

```powershell
.\scripts\run-local-stack.ps1
```

Se você já estiver em `scripts\`, use: `.\run-local-stack.ps1`

Isso vai:

- Subir os containers (postgres, redis, minio, api) com `docker compose -f docker-compose.dev.yml up -d`
- Aguardar a API responder em `/health`
- **Ao final do Docker:** mostrar se deu **sucesso** ou **erro**, listar o que subiu e as URLs para acessar (API, BFF, Swagger, Postgres, etc.)
- Iniciar o BFF com `dotnet run` em **http://localhost:5001**, com `Bff:ApiBaseUrl=http://localhost:8080`

Se o Docker falhar, o script exibe "DOCKER: falhou ao subir os containers" em vermelho e sugere ver os logs com `docker compose -f docker-compose.dev.yml logs`. Não é mais necessário abrir o Docker Desktop para saber se deu erro.

**Falhas e rerun:** O script está preparado para falhas e para ser executado de novo a qualquer momento. Se o daemon Docker não estiver rodando, a porta 5001 já estiver em uso (ex.: BFF anterior) ou o BFF encerrar com erro, você verá uma mensagem clara e o script sai com código 1. Basta corrigir (ex.: abrir o Docker Desktop, fechar o processo na 5001) e rodar o script novamente. O `docker compose up -d` é idempotente: containers já no ar permanecem.

Deixe esse terminal aberto (o BFF roda em primeiro plano e você vê os logs).  
Se quiser rodar o BFF em background:

```powershell
.\scripts\run-local-stack.ps1 -Detached
```

Se a API já estiver rodando (por exemplo só Docker) e você quiser apenas o BFF:

```powershell
.\scripts\run-local-stack.ps1 -SkipDocker
```

### 2. Rodar o app Flutter

Em **outro terminal**, na pasta do app (sempre com `.\` no PowerShell):

```powershell
cd frontend\araponga.app
.\scripts\run-app-local.ps1
```

Isso executa `flutter run --dart-define=BFF_BASE_URL=http://localhost:5001`.

- **Emulador Android**: o localhost do host é acessível como `10.0.2.2`. Use:
  ```powershell
  .\scripts\run-app-local.ps1 -Android
  ```
  (define `BFF_BASE_URL=http://10.0.2.2:5001`)

- **iOS Simulator**: `localhost:5001` funciona direto.

- **Device físico na mesma rede**: descubra o IP da máquina (ex.: 192.168.1.10) e use:
  ```powershell
  .\scripts\run-app-local.ps1 -BffUrl "http://192.168.1.10:5001"
  ```

### 3. Conferir se está tudo no ar

| O quê   | URL                        |
|--------|----------------------------|
| API     | http://localhost:8080      |
| Swagger | http://localhost:8080/swagger |
| Health  | http://localhost:8080/health  |
| BFF     | http://localhost:5001      |
| BFF journeys | http://localhost:5001/bff/journeys |

O app já está configurado para usar **http://localhost:5001** como padrão em desenvolvimento (`lib/core/config/app_config.dart`). O script só garante que você suba API e BFF antes de rodar o app.

## Pré-requisitos

- **Docker Desktop** (para API + postgres, redis, minio)
- **.env** na raiz do repo: rode `.\scripts\setup-env.ps1` uma vez (cria `.env` a partir de `.env.example` e gera JWT para dev)
- **.NET 8 SDK** (para o BFF)
- **Flutter** no PATH. Se não tiver: na raiz do repo rode `.\scripts\install-flutter.ps1` (instala em `%LOCALAPPDATA%\flutter` e adiciona ao PATH).

Se o build da API no Docker falhar, confira se o `Dockerfile` usado pelo `docker-compose.dev.yml` está alinhado à estrutura do backend (caminhos em `backend/`).

### "Running scripts is disabled on this system"

O PowerShell pode estar com a política de execução que bloqueia `.ps1`. Escolha uma opção:

**Opção 1 – Permitir scripts para o seu usuário (recomendado):**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```
Depois rode de novo: `.\scripts\run-local-stack.ps1`

**Opção 2 – Rodar só esta vez sem mudar a política:**
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-local-stack.ps1
```
(na raiz do repo; se estiver em `scripts\`, use `-File .\run-local-stack.ps1`)

### "flutter is not recognized"

Se o script do app disser que Flutter não foi encontrado, o `flutter` não está no PATH desse terminal. Você pode:

- Abrir um **novo terminal** (ou o terminal do Cursor/VS Code onde a extensão Flutter costuma configurar o PATH).
- Rodar na pasta do app:  
  `flutter run --dart-define=BFF_BASE_URL=http://localhost:5001`  
  (em um terminal onde `flutter` funciona).
- Adicionar o Flutter ao PATH do usuário (pasta `bin` dentro da instalação do Flutter).

## Resumo rápido

```powershell
# Terminal 1 (raiz do repo)
.\scripts\run-local-stack.ps1

# Terminal 2 (app)
cd frontend\araponga.app
.\scripts\run-app-local.ps1
```

Assim você consegue testar e debugar **app → BFF → API** tudo localmente de forma integrada.
