# Configurar Developer Portal em devportal.araponga.app

## Resumo

Este PR garante que o Developer Portal seja servido diretamente em `devportal.araponga.app` sem página intermediária.

## Mudanças

### 1. Configuração do Domínio

- ✅ Workflow do GitHub Pages configurado para usar `devportal.araponga.app`
- ✅ Arquivo CNAME configurado em `docs/CNAME`
- ✅ Referências no HTML do devportal atualizadas

### 2. Estrutura Final

- `devportal.araponga.app` → Developer Portal (servido diretamente, sem página intermediária)
- `araponga.app` → Landing pública (Gamma)

### 3. Arquivos Modificados

- `.github/workflows/devportal-pages.yml` - CNAME configurado para `devportal.araponga.app`
- `docs/CNAME` - Configurado para `devportal.araponga.app`
- `docs/devportal/index.html` - Referência no footer atualizada
- `backend/Araponga.Api/wwwroot/devportal/index.html` - Referência no footer atualizada
- `docs/13_DOMAIN_ROUTING.md` - Documentação atualizada

## Estrutura de Domínios

- **`araponga.app`** → Landing pública (página estática no Gamma)
- **`devportal.araponga.app`** → Developer Portal (GitHub Pages, servido diretamente)

## Notas

- Não deve existir página intermediária
- O Developer Portal é servido diretamente em `devportal.araponga.app`
- O workflow copia o conteúdo do devportal para a raiz do dist, garantindo que não haja página intermediária

## Checklist

- [x] Workflow configurado para `devportal.araponga.app`
- [x] CNAME configurado
- [x] Referências no HTML atualizadas
- [x] Documentação atualizada
