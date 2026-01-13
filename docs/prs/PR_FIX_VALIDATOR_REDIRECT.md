# Fix: Correcoes de Validator e Redirect Loop

## Resumo

Este PR corrige dois problemas criticos identificados nos testes e no developer portal:
1. Validacao de enums no `CreatePostRequestValidator` estava falhando
2. Loop infinito de redirect no developer portal

## Problemas Corrigidos

### 1. Validacao de Enums (CreatePostRequestValidator)

**Problema:**
- O validator usava `IsInEnum()` em strings (`Type` e `Visibility`)
- `IsInEnum()` so funciona com tipos enum, nao com strings
- Isso causava falha em 4 testes que criavam posts

**Solucao:**
- Trocado para `Must()` com `Enum.TryParse<PostType>()` para validar `Type`
- Trocado para `Must()` com `Enum.TryParse<PostVisibility>()` para validar `Visibility`
- Agora valida corretamente se as strings podem ser convertidas para os enums

**Testes Corrigidos:**
- `Feed_CreatePost_IgnoresGeoAnchorsFromRequest`
- `Feed_CreatePost_LikeCommentShare`
- `Feed_FiltersByAssetId`
- `CompleteResidentFlow_CadastroToPost`

### 2. Loop de Redirect no Developer Portal

**Problema:**
- `docs/devportal/index.html` redirecionava de volta para `/` com o parametro de busca
- Isso criava loop infinito: `/?fromLanding=1` -> `/devportal/?fromLanding=1` -> `/?fromLanding=1` -> ...

**Solucao:**
- `docs/devportal/index.html` agora apenas limpa o parametro `fromLanding` da URL
- Nao redireciona mais, evitando o loop
- Usa `window.history.replaceState()` para limpar o parametro sem recarregar a pagina

## Mudancas

### Arquivos Modificados
- `backend/Araponga.Api/Validators/CreatePostRequestValidator.cs` - Correcao da validacao de enums
- `docs/devportal/index.html` - Correcao do loop de redirect
- `docs/40_CHANGELOG.md` - Documentacao das correcoes
- `docs/21_CODE_REVIEW.md` - Atualizacao do status das recomendacoes
- `docs/13_DOMAIN_ROUTING.md` - Nota sobre protecao contra loops
- `docs/22_COHESION_AND_TESTS.md` - Nota sobre isolamento de testes

## Testes

Todos os 119 testes passam:
```bash
dotnet test backend/Araponga.Tests/Araponga.Tests.csproj
```

## Impacto

- ✅ Todos os testes passando (119/119)
- ✅ Developer portal nao entra mais em loop de redirect
- ✅ Validacao de entrada funcionando corretamente
- ✅ Nenhum breaking change

## Checklist

- [x] Codigo compila sem erros
- [x] Todos os testes passam
- [x] Documentacao atualizada
- [x] Sem breaking changes
- [x] Validacao funcionando corretamente
- [x] Loop de redirect corrigido
