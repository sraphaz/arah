# Domínios públicos do Araponga

## Visão geral

O portal técnico do Araponga fica em `https://araponga.eco.br/` e é servido pelo GitHub Pages a partir da pasta `docs/`.
A apresentação pública (landing) continua hospedada no Gamma, usando um domínio próprio separado para a experiência pública.

## Domínios adquiridos

- `araponga.eco.br` → portal técnico (GitHub Pages).
- `araponga.app` → landing pública (Gamma).
- `araponga.org` → redireciona para `https://araponga.eco.br/`.

## Lógica de publicação e redirect

- O `docs/index.html` mantém o redirect automático para a landing pública.
- Quando o usuário retorna da landing, a experiência fica no portal técnico (`araponga.eco.br`).
- Ajustes de DNS e redirects externos (ex: `araponga.org` → `araponga.eco.br`) são feitos no provedor de domínio.

## Validação rápida

- Abra `https://araponga.eco.br/` e confirme que o redirecionamento leva para a landing pública.
- Verifique se o retorno para o portal técnico funciona com o parâmetro `?fromLanding=1`.
