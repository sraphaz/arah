# Rebrand Araponga → Arah – concluído

O rebrand foi aplicado. A solução é `Arah.sln`, os projetos são `Arah.*`, pastas `Arah.Api.Bff`, app Flutter `ArahApp` em `frontend/arah.app`, devportal e conteúdo estático com marca "Arah API".

**Exceção:** o **CNAME** (domínio do GitHub Pages / devportal) não foi alterado; manter conforme configurado no repositório e nos workflows.

## Banco de dados

- Em desenvolvimento, o Postgres usa por padrão usuário/senha/DB **arah** (não mais `araponga`).
- Se você tinha dados no volume antigo, pode precisar recriar o banco ou ajustar variáveis (`POSTGRES_USER`, `POSTGRES_DB`, `POSTGRES_PASSWORD`) para o que estiver usando.

## Build e testes

- Backend: `dotnet build Arah.sln` e `dotnet test` a partir da raiz.
- Flutter: após renomear, `cd frontend/arah.app` e `flutter pub get` / `flutter run`.
