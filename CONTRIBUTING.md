# Contribuindo com o Araponga

Obrigado por considerar contribuir com o Araponga! Este é um projeto comunitário orientado ao território.

## Código de conduta

Antes de contribuir, leia o [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md). Esperamos uma colaboração respeitosa e cuidadosa.

## Formas de contribuição

- Relatar bugs e propor melhorias
- Ajustar documentação e exemplos
- Implementar funcionalidades alinhadas ao roadmap
- Revisar e validar fluxos do produto

## ⚠️ Regras de Documentação - OBRIGATÓRIO

**TODA mudança no código DEVE ser acompanhada de atualização na documentação correspondente.**

### Quando Atualizar Documentação

Consulte **[docs/CURSOR_DOCUMENTATION_RULES.md](./docs/CURSOR_DOCUMENTATION_RULES.md)** para mapeamento completo de:
- Qual documento atualizar para cada tipo de mudança
- Checklist obrigatório antes de commitar
- Templates de atualização

### Regra Fundamental

**Documentação desatualizada é considerado um bug crítico.**

Se você não tem certeza de qual documento atualizar:
1. Busque referências à funcionalidade nos documentos
2. Atualize TODOS os documentos que mencionam a funcionalidade
3. Quando em dúvida, atualize mais do que menos

### Checklist Rápido

Antes de criar PR, verifique:
- [ ] Documentação técnica atualizada
- [ ] Exemplos de código atualizados
- [ ] Changelog atualizado (se mudança significativa)
- [ ] README.md atualizado (se mudar funcionalidades principais)
- [ ] Documentação de API atualizada (se mudar endpoints)

## Configuração local (opcional)

Se precisar rodar a API localmente, utilize o .NET 8 e siga as instruções do README. Para mudanças no portal, edite os arquivos em `docs/`.

## Padrões importantes

- Use sempre **territory** (não use "place").
- Use **items** (não "listings") para produtos do marketplace.
- Use **29 fases** (não "24 fases") ao referenciar backlog.
- Não commite `bin/`, `obj/` ou artefatos gerados.
- Mantenha mudanças pequenas, focadas e bem descritas.
- Prefira contratos explícitos e documentação atualizada.

## Design e Coerência

O design do Araponga atua como campo de coerência: orienta escolhas visuais para sustentar foco, cuidado e continuidade.
Consulte o guia completo em [Design & Coerência](https://araponga.eco.br/design/).

**Princípios**
- Território como referência.
- Baixa excitação e foco.
- Silêncio funcional com hierarquia clara.
- Ação consciente e explícita.
- Evolução lenta e íntegra.

**O que é bem-vindo**
- Legibilidade e redução de ruído.
- Consistência entre telas e estados.
- Hierarquia visual clara.
- Acessibilidade (contraste, foco, navegação por teclado).

**O que evitar**
- Efeitos chamativos e distrações.
- Saturação gratuita.
- Animações celebratórias.
- Metáforas fechadas que reduzam leitura do contexto.

**Como propor mudanças visuais (passo a passo)**
1. Descreva o problema humano e o contexto onde ele aparece.
2. Apresente a proposta com antes/depois.
3. Explique a justificativa alinhada aos princípios.
4. Indique como validar o impacto (ex.: leitura mais rápida, menos passos, menos dúvidas).

## Como abrir um PR

1. Abra uma issue descrevendo o contexto ou use uma issue existente.
2. Crie um branch com nome claro (ex.: `feature/portal` ou `fix/feed-visibility`).
3. Faça commits pequenos e coerentes.
4. **Atualize documentação correspondente** (ver regras acima).
5. Descreva o que mudou e como validar.
6. Referencie testes executados (ou explique se não foram necessários).
7. Use o template de PR (`.github/pull_request_template.md`) que inclui checklist de documentação.

## Referências

- **[Regras de Documentação para Agentes](./docs/CURSOR_DOCUMENTATION_RULES.md)** - Guia completo
- **[Índice da Documentação](./docs/00_INDEX.md)** - Navegação completa
- **[Backlog API](./docs/backlog-api/README.md)** - 29 fases planejadas

## Reportando problemas de segurança

Consulte o [SECURITY.md](SECURITY.md) para detalhes de divulgação responsável.
