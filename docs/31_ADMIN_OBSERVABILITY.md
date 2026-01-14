# Admin e Observabilidade

## Visão geral
O módulo administrativo é **pós-MVP** e serve para observar a saúde do ecossistema territorial,
com foco em transparência operacional e suporte à moderação.

## Escopo do módulo admin [POST-MVP]
- Visão por território (atividade, número de vínculos, nível de reports).
- Painel de erros e alertas do sistema.
- Relatórios agregados de moderação.
- Acompanhamento de sanções (territoriais e globais).
- Indicadores de saúde do sistema (latência, falhas, filas).

## Base P0 já implementada
- **SystemConfig**: configurações globais calibráveis (sem segredos) via endpoints admin.
- **Work Queue**: fila genérica de itens de trabalho para suportar automação + fallback humano.
- Ver detalhes em: `docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md`

## Objetivos
- Dar visibilidade para a equipe sobre o estado dos territórios.
- Identificar padrões de risco e problemas operacionais.
- Suportar decisões de governança sem interferir na autonomia do território.
