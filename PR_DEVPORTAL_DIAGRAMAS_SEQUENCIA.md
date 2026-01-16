# PR: Integração de Diagramas de Sequência no DevPortal

## Resumo

Integração de diagramas de sequência interativos no Developer Portal usando Mermaid.js. Os diagramas são exibidos através de acordeões expansíveis harmoniosos, permitindo visualização clara dos principais fluxos de negócio da aplicação.

## 🎯 Objetivo

Melhorar a documentação do DevPortal com visualizações interativas dos fluxos de negócio, facilitando o onboarding de desenvolvedores e a compreensão das interações entre componentes da API.

## ✨ Implementação

### Diagramas Adicionados
1. **Autenticação social → JWT** - Fluxo completo de login/cadastro com 2FA
2. **Descoberta de território** - Busca de territórios e criação de membership
3. **Feed territorial (listagem)** - Listagem paginada com filtros
4. **Criação de post** - Validações, geo anchors e publicação
5. **Membership: visitor → resident** - Solicitação e verificação de residência
6. **Moderação e governança** - Reports, triagem e sanções

### Características
- 📊 **Renderização lazy**: Diagramas renderizados apenas quando expandidos
- 🎨 **Tema personalizado**: Cores alinhadas à paleta do devportal
- ✨ **Animações suaves**: Transições CSS elegantes
- 📱 **Responsivo**: Funciona perfeitamente em mobile
- ♿ **Acessível**: Navegação por teclado e semântica HTML5

## 📁 Arquivos Modificados

**Nota**: O Developer Portal está disponível em `https://devportal.araponga.app/` e é servido pelo GitHub Pages. O código fonte está em `backend/Araponga.Api/wwwroot/devportal/` e é automaticamente deployado via workflow do GitHub Actions.

- `backend/Araponga.Api/wwwroot/devportal/index.html`
  - Adicionado Mermaid.js via CDN
  - 6 diagramas de sequência integrados
  - JavaScript para renderização lazy

- `backend/Araponga.Api/wwwroot/devportal/assets/css/devportal.css`
  - Estilos para acordeões expansíveis
  - Animações de transição
  - Responsividade

- `docs/prs/PR_DEVPORTAL_DIAGRAMAS_SEQUENCIA.md` (novo)
  - Documentação completa do PR

## 🔍 Visualização

No DevPortal, cada fluxo principal possui um acordeão "📊 Ver Diagrama de Sequência" que, ao ser clicado, revela o diagrama de forma animada e harmoniosa.

## ✅ Validações

- ✅ HTML válido (sem erros de linter)
- ✅ CSS sem conflitos
- ✅ JavaScript funcional
- ✅ Renderização correta dos diagramas
- ✅ Responsividade testada
- ✅ Acessibilidade verificada

## 📚 Documentação Completa

Para detalhes técnicos completos, consulte:
**[docs/prs/PR_DEVPORTAL_DIAGRAMAS_SEQUENCIA.md](./docs/prs/PR_DEVPORTAL_DIAGRAMAS_SEQUENCIA.md)**

---

**Status**: ✅ Pronto para review  
**Impacto**: DevPortal apenas (sem impacto em código de produção)  
**Breaking Changes**: Nenhum  
**Dependências**: Mermaid.js v10 via CDN
