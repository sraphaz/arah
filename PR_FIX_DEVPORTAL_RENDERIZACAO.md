# Fix: Corrigir Renderização do Developer Portal

## Resumo

Este PR corrige o problema onde o Developer Portal em `devportal.araponga.app` mostrava apenas uma mensagem de "Carregando..." ao invés do conteúdo completo do portal. O problema ocorreu após a remoção de scripts de redirect que também continham a lógica de renderização do conteúdo.

## Problema

O Developer Portal não estava renderizando o conteúdo completo:

- **`https://devportal.araponga.app/devportal/`** mostrava apenas:
  ```
  Developer Portal - Carregando...
  Se você foi redirecionado aqui, o portal deve carregar automaticamente.
  ```

- **`https://devportal.araponga.app/`** mostrava apenas:
  ```
  Araponga API
  Redirecionando para o aplicativo principal e o Developer Portal.
  Se o redirecionamento não acontecer, acesse araponga.app ou Developer Portal.
  ```

**Causa raiz:**
- O arquivo `docs/devportal/index.html` continha apenas um placeholder simples
- O conteúdo completo estava apenas em `backend/Araponga.Api/wwwroot/devportal/index.html`
- Quando o GitHub Pages serve o conteúdo de `docs/`, ele não tinha acesso ao conteúdo completo
- Os caminhos dos assets estavam usando caminhos absolutos (`/devportal/assets/...`) que não funcionavam no contexto do GitHub Pages

## Solução

1. **Substituir `docs/devportal/index.html` pelo conteúdo completo**
   - Copiar o conteúdo completo de `backend/Araponga.Api/wwwroot/devportal/index.html`
   - Garantir que o portal tenha toda a documentação da API

2. **Ajustar caminhos de assets para relativos**
   - Mudar de `/devportal/assets/...` para `../assets/...`
   - Funciona tanto no GitHub Pages quanto quando servido pela API local

3. **Melhorar resolução de caminhos do OpenAPI**
   - Atualizar `devportal.js` para tentar múltiplos caminhos:
     - Primeiro: `/swagger/v1/swagger.json` (quando backend está rodando)
     - Segundo: `../openapi.json` (caminho relativo no GitHub Pages)
     - Terceiro: `/openapi.json` (caminho absoluto como fallback)

## Mudanças

### 1. Conteúdo Completo do Portal

**docs/devportal/index.html:**
- Substituído placeholder simples pelo conteúdo completo (582 linhas)
- Inclui todas as seções: visão geral, conceitos, fluxos, autenticação, OpenAPI, quickstart, etc.
- Caminhos de assets ajustados para relativos (`../assets/...`)
- Caminho do OpenAPI ajustado para `../openapi.json`

### 2. Melhoria na Resolução de Especificação OpenAPI

**docs/assets/js/devportal.js:**
- Função `resolveSpecUrl()` agora tenta múltiplos caminhos:
  1. `/swagger/v1/swagger.json` (preferido, quando backend está rodando)
  2. `../openapi.json` (caminho relativo para GitHub Pages)
  3. `/openapi.json` (fallback absoluto)
- Garante que o explorer funcione em ambos os contextos

## Resultado

### Antes:
- Portal mostrava apenas mensagem de "Carregando..."
- Usuários não conseguiam acessar a documentação
- Assets não carregavam corretamente

### Depois:
- Portal renderiza completamente com toda a documentação
- Todas as seções visíveis e funcionais
- Assets carregam corretamente
- Explorer OpenAPI funciona tanto localmente quanto no GitHub Pages

## Arquivos Modificados

- `docs/devportal/index.html` - Substituído pelo conteúdo completo (368 → 53,418 bytes)
- `docs/assets/js/devportal.js` - Melhorada resolução de caminhos do OpenAPI (7,212 → 15,556 bytes)

## Testes

- ✅ Portal renderiza completamente em `devportal.araponga.app`
- ✅ Assets (CSS, imagens) carregam corretamente
- ✅ Navegação entre seções funciona
- ✅ Explorer OpenAPI funciona (tenta múltiplos caminhos)
- ✅ Links internos e externos funcionam
- ✅ Banner de retorno para `araponga.app` visível

## Checklist

- [x] Conteúdo completo do portal copiado para `docs/devportal/index.html`
- [x] Caminhos de assets ajustados para relativos
- [x] Resolução de OpenAPI melhorada com múltiplos fallbacks
- [x] Portal renderiza completamente no GitHub Pages
- [x] Assets carregam corretamente
- [x] Explorer OpenAPI funcional
- [x] Sem breaking changes
- [x] Compatível com ambos os contextos (GitHub Pages e API local)
