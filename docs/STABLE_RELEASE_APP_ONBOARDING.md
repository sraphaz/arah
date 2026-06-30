# Release estável: App, Onboarding e Stack Local

Este documento descreve a **versão estável** atual do app (Flutter), do fluxo de onboarding, das correções de backend e dos procedimentos para rodar o projeto localmente. Serve como referência para quem está chegando ao repositório e para evolução incremental.

---

## Índice

1. [O que está implementado](#o-que-está-implementado)
2. [Como rodar o projeto (getting started)](#como-rodar-o-projeto-getting-started)
3. [Mudanças e correções desta release](#mudanças-e-correções-desta-release)
4. [O que ainda não está no app (próximos passos)](#o-que-ainda-não-está-no-app-próximos-passos)
5. [Referências na documentação](#referências-na-documentação)

---

## O que está implementado

### App (Flutter – `frontend/Arah.app`)

- **Autenticação**: login com e-mail (check-email → senha ou criar conta), signup com nome e senha; **login com Google na UI** (botão "Entrar com Google" no fluxo de login — requer `GOOGLE_SIGN_IN_CLIENT_ID` e config Firebase para funcionar end-to-end).
- **Onboarding**: tela de seleção de território após login/cadastro quando não há território salvo.
  - Lista “Próximos a você” (sugeridos por lat/lng).
  - Seleção na lista **só altera o mapa e o destaque**; o botão **Continuar** é o único que conclui o onboarding e leva ao feed.
  - Contorno do território e pin no mapa em **verde floresta**.
  - Botão **Voltar** (logout e retorno à tela de login/cadastro).
- **Feed**: listagem do feed do território selecionado, com paginação e pull-to-refresh.
- **Explorar**: troca de território (lista paginada) com **busca por nome/cidade** (jornada `territories/search`); ao “entrar” em outro território, o feed e o mapa refletem o escolhido.
- **Mapa**: pins do território, contorno (polígono/círculo) em verde floresta, marcador do usuário.
- **Eventos**: lista de eventos do território, **criação de evento** (título, descrição, início/término com seletores de data/hora, local), interesse e confirmação de presença.
- **Governança**: tela de votações do território (jornada `governance`) — lista com filtro de status (Todas/Abertas/Fechadas), votar inline, ver resultados (barras) e criar votação (tipo, visibilidade, opções).
- **Mapa (deep-links)**: tocar num pin abre uma folha com "Ver detalhes" que navega para o conteúdo correspondente (evento → Eventos, asset → Assets, alerta → Alertas, post → feed).
- **Perfil**: exibição e edição de nome e bio; preferências de notificação (estrutura). Ações "Meu território" (seletor de território), "Notificações" (aba) e atalho "Governança" no menu de configurações.
- **Publicar**: criação de post (título, conteúdo, tipo, visibilidade) no território ativo.

### Backend (API + BFF)

- **API**: autenticação (check-email, login, signup), jornadas (onboarding, feed, territórios, eventos, perfil, notificações), migração única Postgres com extensão `pgcrypto` e DEFAULTs para `RowVersion`.
- **BFF**: proxy para a API por jornadas (`/api/v2/journeys/*`), tratamento de 502 com mensagem clara quando a API fecha a conexão prematuramente.

### Infra e scripts

- **Stack local**: `scripts/run-local-stack.ps1` sobe API (Docker), BFF (dotnet) e opcionalmente o app Flutter; aplica migrações e executa seeds (Camburi + Boiçucanga) com encoding UTF-8.
- **Seed**: ingestão via SQL (`scripts/seed/seed-camburi.sql`, `seed-boicucanga.sql`) com `docker cp` + `psql -f` para preservar acentos.

---

## Como rodar o projeto (getting started)

### Pré-requisitos

- Docker (para API + Postgres)
- .NET SDK (para BFF)
- Flutter (para o app)
- PowerShell (Windows) para os scripts

### Passo a passo

1. **Subir API + BFF (e seed)**  
   Na raiz do repositório:
   ```powershell
   .\scripts\run-local-stack.ps1
   ```
   - Na primeira execução, a API aplica a migração única no Postgres e o script roda os seeds Camburi e Boiçucanga.
   - Para **recriar o banco do zero** (útil após mudanças de schema):
   ```powershell
   .\scripts\run-local-stack.ps1 -ResetDatabase
   ```

2. **Rodar o app Flutter**  
   Em outro terminal:
   ```powershell
   cd frontend\Arah.app
   flutter run
   ```
   Ou use o script do projeto que define `BFF_BASE_URL` (ex.: `http://localhost:5001`).

3. **Fluxo típico no app**  
   Abrir o app → informar e-mail (check-email) → criar conta ou entrar com senha → **onboarding**: escolher território na lista (mapa atualiza), tocar em **Continuar** → feed do território. Em **Explorar** é possível trocar de território.

Detalhes dos seeds e variáveis de ambiente: [scripts/seed/README.md](../scripts/seed/README.md).

---

## Mudanças e correções desta release

### Backend

- **Migração única**  
  Toda a criação de schema e ajustes ficam em uma única migração (`UnifiedInitialCreate`), incluindo:
  - Extensão **pgcrypto** (`CREATE EXTENSION IF NOT EXISTS pgcrypto`) para uso de `gen_random_bytes()`.
  - Índice único em `territory_memberships`: **um Resident por usuário** (filtro `Role = 2`), permitindo vários Visitor por usuário.
  - DEFAULT `gen_random_bytes(8)` para as colunas **RowVersion** em `territory_memberships` e `community_posts`, evitando NOT NULL violation no INSERT.
- **DbContext (EF)**  
  Configuração de `RowVersion` com `HasDefaultValueSql("gen_random_bytes(8)").ValueGeneratedOnAdd()` em `SharedDbContext` e `ArapongaDbContext` para membership; em `ArapongaDbContext` para `community_posts`, para o EF não enviar o valor no INSERT.
- **SharedDbContext**  
  Índice único parcial de membership corrigido para filtro `Role = 2` (Resident), alinhado à migração.

### BFF

- **502 quando a API fecha a conexão**  
  Mensagem e hint em português quando a resposta termina prematuramente, orientando a verificar se a API está rodando e a checar os logs.

### App (Flutter)

- **Onboarding**  
  - Seleção na lista apenas atualiza o território exibido no mapa e o destaque do card; **apenas o botão Continuar** conclui o onboarding.
  - Mapa usa o território **selecionado** (ou o mais próximo como padrão).
  - Botão **Voltar** faz logout e limpa território, retornando à tela de login/cadastro (com `addPostFrameCallback` para evitar conflito com o redirect do router).
- **Visual**  
  - Contorno do polígono/círculo do território e pin do território em **verde floresta** (`#228B22`) na tela de onboarding e no mapa.

### Scripts e seeds

- **Encoding UTF-8**  
  Seeds executados com `docker cp` + `psql -f` e `PGCLIENTENCODING=UTF8`; arquivos SQL com `SET client_encoding = 'UTF8';` para preservar acentos (ex.: Boiçucanga, São).
- **Seed**  
  Execução automática de `seed-camburi.sql` e `seed-boicucanga.sql` pelo `run-local-stack.ps1` após subir os containers.

---

## O que ainda não está no app (próximos passos)

Evolução planejada, em ordem de prioridade sugerida:

| Área | O que falta |
|------|-------------|
| **Login Google (config)** | UI já existe; falta provisionar `GOOGLE_SIGN_IN_CLIENT_ID` (OAuth Web) e config Firebase para o fluxo funcionar end-to-end. |
| **Push / FCM** | Recepção de notificações push e deep-link a partir da notificação (hoje só o registro de device token). |
| **Mapa → post detalhe** | Deep-link de pin do tipo `post` hoje leva ao feed; falta tela de detalhe de post dedicada. |
| **Perfil** | Upload de avatar; ligar o histórico de governança (`me/profile/governance`) na UI. |
| **Marketplace (aprofundamento)** | Tela de carrinho dedicada, gestão de itens do vendedor e fluxo de pagamento (avaliar frente aos valores do produto). |

> **Entregue recentemente no app**: login com Google na UI, criação de eventos, governança/votações (listar, votar, resultados, criar) e deep-links nos pins do mapa. Feed já cobre mídia, curtir/comentar/compartilhar, exclusão e filtros.

O backend cobre amplamente essas capacidades; o app é evoluído de forma incremental (API → BFF → App) para expô-las.

---

## Referências na documentação

- **Índice geral**: [docs/00_INDEX.md](00_INDEX.md) (quando existir) ou [README.md](../README.md).
- **Desenvolvimento e setup**: [DEVELOPMENT.md](DEVELOPMENT.md), [SETUP.md](SETUP.md).
- **Seeds**: [scripts/seed/README.md](../scripts/seed/README.md).
- **Stack local**: comentários no início de [scripts/run-local-stack.ps1](../scripts/run-local-stack.ps1).
- **Visão e roadmap**: [README.md](../README.md) (seção Estado do Projeto e Fases).
