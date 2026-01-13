# Fix: GitHub Pages Deployment Timeout

## Problema

O workflow de deploy do GitHub Pages estava dando timeout após 10 minutos porque o deployment ficava em `deployment_queued` indefinidamente.

## Causa

O novo sistema de GitHub Pages (`actions/deploy-pages@v4`) pode ter filas longas ou problemas temporários que causam timeout.

## Solução Implementada

### Mudanças no Workflow

1. **Separação de Jobs**: Dividido em `build` e `deploy` para melhor organização
2. **Timeout no Job Level**: Adicionado `timeout-minutes: 5` no job de deploy
3. **Uso do Novo Sistema**: Mantido `actions/deploy-pages@v4` com configuração otimizada
4. **Permissões Ajustadas**: `contents: read` ao invés de `write` (não precisa mais escrever na branch)

### Estrutura do Workflow

```yaml
jobs:
  build:
    # Prepara artifacts
    # Upload artifact
  
  deploy:
    timeout-minutes: 5  # Timeout explícito
    # Deploy usando actions/deploy-pages@v4
```

## Benefícios

1. **Timeout Explícito**: Evita espera infinita
2. **Melhor Organização**: Separação clara entre build e deploy
3. **Compatibilidade**: Usa o sistema oficial do GitHub Pages

## Alternativa (se ainda der problema)

Se o problema persistir, podemos voltar ao método antigo usando `peaceiris/actions-gh-pages@v4`:

```yaml
- name: Deploy to GitHub Pages
  uses: peaceiris/actions-gh-pages@v4
  with:
    github_token: ${{ secrets.GITHUB_TOKEN }}
    publish_dir: dist
    publish_branch: gh-pages
```

## Organização da Documentação (Incluído)

Além do fix do GitHub Pages, este PR também organiza a documentação do projeto:

### Mudanças de Organização

1. **Documentação de PRs**: Movidos todos os arquivos `PR_*.md` para `docs/prs/`
   - Criado `docs/prs/README.md` com índice organizado
   - Estrutura mais limpa na raiz do projeto

2. **Plano de Refatoração**: Movido `PLANO_REFACTOR_RECOMENDACOES_PENDENTES.md` para `docs/`

3. **Changelog**: Removido `CHANGELOG.md` duplicado da raiz (mantido apenas `docs/40_CHANGELOG.md`)

4. **Índice Atualizado**: Atualizado `docs/00_INDEX.md` com novas seções:
   - Planos e Recomendações
   - Pull Requests

### Estrutura Final

- **Raiz**: Apenas arquivos essenciais (README, LICENSE, CODE_OF_CONDUCT, etc.)
- **docs/**: Toda documentação organizada por categorias
- **docs/prs/**: Documentação de todos os PRs

## Arquivos Modificados

- `.github/workflows/devportal-pages.yml`
- `docs/prs/README.md` (novo)
- `docs/00_INDEX.md`
- `backend/Araponga.Api/wwwroot/devportal/index.html`
- Movidos/Removidos: `PLANO_REFACTOR_RECOMENDACOES_PENDENTES.md`, `CHANGELOG.md`, `PR_*.md`

## Testes

Após merge, monitorar se o deploy completa dentro do timeout de 5 minutos.
