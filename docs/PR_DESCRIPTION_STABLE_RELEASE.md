# Release estável: documentação, getting started e app (onboarding + conexões)

## Resumo

Este PR consolida uma **versão estável** do app: atualiza a documentação do projeto, adiciona um guia de **getting started** (stack local + seeds + app Flutter), documenta o que já está implementado e o que ainda falta (mídia, likes, comentários, filtros, preferências, marketplace etc.) e inclui as alterações de backend/frontend da branch (onboarding, migração única, conexões/círculo de amigos, BFF, seeds).

## Objetivo

- Quem abre o repositório consegue **ler o índice, a home e o README** e saber como rodar o app e a stack em poucos passos.
- Fica explícito **o que está pronto** (auth, onboarding, feed, mapa, eventos, perfil, publicar) e **o que virá em seguida** (imagens nos posts, curtir, comentar, compartilhar, excluir post, filtros, preferências no perfil, tipo de post, marketplace).

## Mudanças de documentação (desta release)

### Novo documento
- **[docs/STABLE_RELEASE_APP_ONBOARDING.md](./STABLE_RELEASE_APP_ONBOARDING.md)**  
  - O que está implementado (app, backend, BFF, scripts).  
  - **Como rodar o projeto (getting started)**: `run-local-stack.ps1`, Flutter, fluxo típico.  
  - Mudanças e correções desta release (migração única, RowVersion, índice Resident, onboarding UX, BFF 502, cores, seeds UTF-8).  
  - Tabela “O que ainda não está no app” (mídia, like, comentar, compartilhar, excluir post, filtros, preferências, tipo de post, marketplace).  
  - Referências para índice, DEVELOPMENT, SETUP, seeds, script local.

### Ajustes em documentos existentes
- **README.md (raiz)**  
  - Nova seção **Quick start (app + stack local)** com os dois passos (script + `flutter run`) e link para o doc da release estável.  
  - Link para [Release estável – App e Onboarding](./docs/STABLE_RELEASE_APP_ONBOARDING.md) na seção Desenvolvimento.
- **docs/DEVELOPMENT.md**  
  - **Quick start** no topo com comando do script e do app e link para STABLE_RELEASE_APP_ONBOARDING.md.
- **docs/README.md**  
  - Link para Release estável na seção Desenvolvimento.
- **scripts/seed/README.md**  
  - Remoção do bloco de log/terminal (Flutter e requests) que tinha sido colado por engano.  
  - Link no início de “Como executar” para o getting started em STABLE_RELEASE_APP_ONBOARDING.md.

## Outras alterações na branch (backend / frontend / infra)

- **Backend**: migração única (pgcrypto, RowVersion para `territory_memberships` e `community_posts`, índice único Resident em `territory_memberships`), DbContext/SharedDbContext alinhados; módulo de **conexões/círculo de amigos** (requests, aceitar, privacidade, notificações); testes e repositórios (Connections, Privacy).
- **BFF**: mensagem de 502 quando a API fecha a conexão prematuramente.
- **App (Flutter)**: onboarding com seleção só no botão “Continuar”, mapa com território selecionado, botão Voltar (logout), contorno e pin em verde floresta.
- **Scripts**: `run-local-stack.ps1` com suporte a `-ResetDatabase` e seeds UTF-8 (Camburi + Boiçucanga).

## Como testar

1. Na raiz: `.\scripts\run-local-stack.ps1` (e opcionalmente `-ResetDatabase` na primeira vez).
2. Em outro terminal: `cd frontend\araponga.app` e `flutter run`.
3. No app: check-email → signup/login → onboarding (escolher território na lista, tocar em Continuar) → feed; Explorar para trocar território.

## Checklist

- [x] Documentação nova (STABLE_RELEASE_APP_ONBOARDING.md) criada e referenciada.
- [x] README e DEVELOPMENT com quick start e link para o doc da release estável.
- [x] scripts/seed/README.md limpo e com link para getting started.
- [x] Descrição do que já existe e do que falta no app documentada.
- [ ] Build e testes do backend (recomendado rodar `dotnet test` antes de merge).
- [ ] App Flutter roda e onboarding conclui sem erros.

## Links

- [Release estável – App e Onboarding](./STABLE_RELEASE_APP_ONBOARDING.md)
- [Guia de Desenvolvimento](./DEVELOPMENT.md)
- [Seeds (Camburi / Boiçucanga)](../scripts/seed/README.md)
