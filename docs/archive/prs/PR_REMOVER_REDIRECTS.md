# Refactor: Remover Redirects Automaticos e Simplificar Navegacao

## Resumo

Este PR remove todos os redirects automaticos que estavam causando loops infinitos e simplifica a navegacao para usar apenas links estaticos. O usuario agora escolhe explicitamente onde quer ir.

## Problema

O sistema tinha redirects automaticos complexos que causavam loops infinitos:
- `/?fromLanding=1` -> `/devportal/?fromLanding=1` -> `/?fromLanding=1` -> ...
- Logica complexa com sessionStorage e verificacao de referrer
- Dificil de debugar e manter

## Solucao

Remover TODOS os redirects automaticos e usar apenas links estaticos:
- Usuario acessa `Arah.app` (Gamma) - ve opcao de ir ao Developer Portal
- Usuario acessa Developer Portal - sempre ve banner com link para voltar ao `Arah.app`
- Navegacao totalmente controlada pelo usuario via cliques

## Mudancas

### 1. Remocao de Redirects Automaticos

**docs/index.html e backend/Arah.Api/wwwroot/index.html:**
- Removido TODO o JavaScript de redirect (74 linhas)
- Substituido por pagina simples com 2 links estaticos:
  - "Visitar Arah.app" (botao principal)
  - "Developer Portal" (botao secundario)
- Design limpo e claro

**docs/devportal/index.html:**
- Removido script de limpeza de parametros
- Simplificado para apenas texto estatico

### 2. Banner Sempre Visivel no Developer Portal

**backend/Arah.Api/wwwroot/devportal/index.html:**
- Removido atributo `hidden` do banner
- Banner sempre visivel com link "← Voltar para Arah.app"
- Texto atualizado: "Developer Portal da API Arah"

**devportal.js (ambos):**
- Removida logica de mostrar/esconder banner baseado em `fromLanding`
- Removida verificacao de sessionStorage e referrer
- Banner sempre visivel

### 3. Documentacao Atualizada

**docs/13_DOMAIN_ROUTING.md:**
- Removida mencao a redirects automaticos
- Documentado novo fluxo de navegacao via links estaticos
- Atualizada descricao do comportamento

## Fluxo de Navegacao

### Antes (com redirects):
1. Usuario acessa `devportal.Arah.app`
2. JavaScript detecta origem e redireciona automaticamente
3. Loop infinito se parametros conflitarem

### Depois (sem redirects):
1. Usuario acessa `Arah.app` (Gamma)
2. Ve pagina com 2 opcoes claras:
   - Visitar Arah.app (ja esta la)
   - Developer Portal (link clicavel)
3. Clica em "Developer Portal"
4. No Developer Portal, sempre ve banner no topo:
   - "← Voltar para Arah.app" (link clicavel)
5. Clica para voltar quando quiser

## Beneficios

- ✅ **Sem loops**: Nenhum redirect automatico = nenhum loop possivel
- ✅ **Simples**: Logica clara e facil de entender
- ✅ **Manutenivel**: Sem JavaScript complexo de redirect
- ✅ **UX melhor**: Usuario tem controle total sobre navegacao
- ✅ **Previsivel**: Comportamento consistente em todos os cenarios

## Arquivos Modificados

- `docs/index.html` - Removido JavaScript, adicionados links estaticos
- `backend/Arah.Api/wwwroot/index.html` - Mesma simplificacao
- `docs/devportal/index.html` - Removido script de limpeza
- `backend/Arah.Api/wwwroot/devportal/index.html` - Banner sempre visivel
- `backend/Arah.Api/wwwroot/devportal/assets/js/devportal.js` - Removida logica de banner
- `docs/assets/js/devportal.js` - Removida logica de banner
- `docs/13_DOMAIN_ROUTING.md` - Documentacao atualizada

## Testes

- ✅ Nenhum loop de redirect
- ✅ Links funcionam corretamente
- ✅ Banner sempre visivel no devportal
- ✅ Navegacao fluida entre paginas

## Checklist

- [x] Removidos todos os redirects automaticos
- [x] Links estaticos funcionando
- [x] Banner sempre visivel no devportal
- [x] Documentacao atualizada
- [x] Sem breaking changes
- [x] Codigo mais simples e manutenivel
