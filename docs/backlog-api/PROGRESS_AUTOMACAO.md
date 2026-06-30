# Progresso da Automação Diária — Arah

Este arquivo é mantido pela rotina diária (Claude → abre o Cursor → agente do Cursor implementa)
que avança o roadmap em `docs/backlog-api/` automaticamente, todo dia às 08:00.

**Escopo por execução: uma Onda inteira** (não apenas uma user story isolada). Uma Onda agrupa
várias Fases (ver `docs/backlog-api/INDICE_DOCUMENTOS.md`, seção "Fases (1-29)"), e cada Fase
agrupa várias user stories (ver cada `FASE<N>.md`). A rotina deve implementar todas as Fases e
user stories da Onda atual antes de considerá-la concluída. Se não der tempo num único dia, a
próxima execução continua a mesma Onda de onde parou — só avança para a próxima Onda quando a
atual estiver 100% implementada.

## Ondas (ordem de execução)

1. Onda 1: MVP Essencial — FASE8, FASE9, FASE10, FASE11
2. Onda 2: Comunicação e Governança — FASE13, FASE14
3. Onda 3: Soberania Territorial — FASE18, FASE17
4. Onda 4: Economia Local — FASE20, FASE23, FASE24
5. Onda 5: Conformidade e Inteligência — FASE12, FASE15
6. Onda 6: Diferenciais — FASE16, FASE22, FASE21, FASE19
7. Onda 7: Autonomia Digital e Economia Circular — FASE25, FASE26, FASE27, FASE28

(Fases 1-7 já constam como implementadas em `docs/20_IMPLEMENTATION_PLAN.md`; a rotina deve
confirmar isso no código antes de assumir como verdade.)

## Como funciona

1. Todo dia às 08:00, o Claude abre o Cursor na pasta deste repositório.
2. Lê este arquivo para saber qual Onda está em andamento e o que já foi feito dentro dela.
3. Cria (ou reaproveita, se a Onda já estiver em andamento) uma branch `auto/onda-<N>`
   (nunca trabalha direto na `main`).
4. Pede ao agente do Cursor para implementar **todas** as Fases/user stories pendentes da Onda
   atual, com testes, seguindo os padrões já existentes no código — continuando item por item até
   a Onda inteira estar implementada ou até o tempo da execução se esgotar.
5. Atualiza este arquivo com o resultado (o que foi concluído, o que falta na Onda).
6. Commita e dá push da branch `auto/onda-<N>` (push da branch, não da main).
7. Quando a Onda inteira estiver concluída, marca como concluída aqui e a próxima execução
   começa a próxima Onda da lista acima.

Revise e dê merge nas branches `auto/*` manualmente quando estiver satisfeito com o resultado.

## Status atual

- **Última atualização**: (ainda não rodou)
- **Onda em andamento**: Onda 1 (a confirmar no código se Fases 1-7 já estão prontas e se alguma
  parte da Onda 1 já existe antes de começar)
- **Fases da Onda atual já concluídas**: —
- **Fase/user story em que parou**: —
- **Testes passando na última execução?**: —

## Histórico de execuções

| Data | Onda | Fases concluídas nesta execução | Branch | Onda finalizada? | Testes OK? | Observações |
|------|------|----------------------------------|--------|-------------------|------------|-------------|
| —    | —    | —                                | —      | —                 | —          | Arquivo criado, aguardando primeira execução automática |
