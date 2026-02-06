# Desenvolvimento

## Guia para Desenvolvedores

### Quick start (app + stack local)

Para subir API, BFF, seeds e app Flutter em poucos passos:

- **Stack**: na raiz do repositório, execute `.\scripts\run-local-stack.ps1` (PowerShell). O script sobe API (Docker), BFF e aplica os seeds Camburi e Boiçucanga. Use `-ResetDatabase` para recriar o banco.
- **App**: em outro terminal, `cd frontend/araponga.app` e `flutter run`.

Documentação completa (getting started, release estável, o que falta no app): **[STABLE_RELEASE_APP_ONBOARDING.md](./STABLE_RELEASE_APP_ONBOARDING.md)**.

### Configuração Inicial

1. Clone o repositório
2. Instale dependências:
   ```bash
   dotnet restore
   npm install
   ```

### Estrutura do Projeto

```
backend/
  ├── Araponga.Api/          # Controllers e endpoints
  ├── Araponga.Application/  # Application services
  ├── Araponga.Domain/       # Domain models
  ├── Araponga.Infrastructure/ # Database e externos
  └── Araponga.Tests/        # Testes

frontend/
  ├── wiki/                  # Documentação interativa
  ├── devportal/             # Portal de desenvolvimento
  └── ...                    # Aplicação principal
```

### Build e Testes

```bash
# Build
dotnet build Araponga.sln

# Testes
dotnet test backend/Araponga.Tests/Araponga.Tests.csproj

# Wiki local
cd frontend/wiki
npm run dev
```

### Padrões de Código

- Usar `Result<T>` para operações que podem falhar
- Validações via FluentValidation
- Repositórios para acesso a dados
- Serviços para lógica de negócio

### Contribuindo

Veja [CONTRIBUTING.md](../CONTRIBUTING.md) para detalhes sobre PRs e commits.
