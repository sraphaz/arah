# Contribuindo com o Araponga

Obrigado por considerar contribuir com o Araponga! Este projeto é comunitário e orientado a territory.

## Objetivo

Criar infraestrutura digital que respeite território, privacidade e governança local — sem extração de dados.

## Como rodar localmente

```bash
git clone https://github.com/sraphaz/araponga.git
cd araponga
dotnet restore
dotnet run --project backend/Araponga.Api
```

A API ficará disponível conforme configurado no `launchSettings.json`.

## Como abrir um PR

1. Abra uma issue descrevendo a necessidade ou use uma issue existente.
2. Crie um branch com nome claro (ex.: `feature/portal` ou `fix/feed-visibility`).
3. Mantenha commits pequenos e coerentes.
4. Atualize documentação quando necessário.
5. Abra o PR descrevendo contexto, mudanças e como testar.

## Padrões importantes

- Use sempre **territory** (não use “place”).
- Não commite `bin/` ou `obj/`.
- Mantenha as mudanças pequenas e focadas.
- Prefira endpoints e contratos explícitos.

## Como reportar issues

Abra issues no GitHub com:

- contexto do problema
- passos para reproduzir
- comportamento esperado vs observado
- logs ou prints relevantes
