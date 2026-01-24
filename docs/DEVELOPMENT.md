# Desenvolvimento

## Guia para Desenvolvedores

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
