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

## Arquivos Modificados

- `.github/workflows/devportal-pages.yml`

## Testes

Após merge, monitorar se o deploy completa dentro do timeout de 5 minutos.
