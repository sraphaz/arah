# Roadmap Estratégico - Araponga
## Planejamento de Desenvolvimento e Evolução da Plataforma

**Versão**: 3.0  
**Data**: 2025-01-20  
**Última Atualização**: 2025-01-20  
**Status**: ✅ MVP Completo + Fases 1-8 Implementadas | 📊 Estratégia de Convergência em Andamento

---

## 📋 Visão Executiva

O roadmap do Araponga apresenta uma estratégia de desenvolvimento progressivo que evolui de um MVP sólido para uma plataforma completa de organização comunitária territorial com governança descentralizada e economia circular tokenizada. O planejamento incorpora padrões de mercado estabelecidos por plataformas líderes, mantendo os valores fundamentais do projeto (território-first, comunidade-first).

**Referência Estratégica**: [Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md) | [Mapa de Funcionalidades](./38_MAPA_FUNCIONALIDADES_MERCADO.md)

---

## ✅ Estado Atual - Fases Implementadas

### MVP Completo

O MVP do Araponga foi concluído com sucesso, estabelecendo uma base técnica robusta e funcionalidades core essenciais para operação de comunidades territoriais.

**Funcionalidades Core Implementadas**:
- ✅ Backend estruturado com Clean Architecture
- ✅ Descoberta e seleção de territórios geográficos
- ✅ Sistema de membership (Visitor/Resident) com verificação de identidade e residência
- ✅ Feed territorial cronológico com georreferenciamento
- ✅ Mapa territorial integrado com pins e entidades
- ✅ Marketplace completo (Stores, Items, Cart, Checkout, Inquiries)
- ✅ Eventos comunitários com georreferenciamento
- ✅ Chat territorial (canais públicos/moradores, grupos, DM)
- ✅ Assets territoriais com geolocalização obrigatória
- ✅ Sistema completo de mídia (upload, armazenamento, processamento)
- ✅ Sistema de moderação (reports, bloqueios, sanções, automações)
- ✅ Notificações in-app confiáveis (Outbox/Inbox pattern)

**Infraestrutura e Qualidade**:
- ✅ Testes automatizados com cobertura >90%
- ✅ CI/CD com builds reprodutíveis
- ✅ Observabilidade completa (Serilog, Prometheus, OpenTelemetry)
- ✅ Segurança avançada (JWT, 2FA, rate limiting, CSRF, sanitização)

### Fases 1-8 Implementadas

| Fase | Título | Status | Duração | Qualidade |
|------|--------|--------|---------|-----------|
| **Fase 1** | Segurança e Fundação Crítica | ✅ Completo | 14 dias | ⭐⭐⭐⭐⭐ |
| **Fase 2** | Qualidade de Código e Confiabilidade | ✅ Completo | 14 dias | ⭐⭐⭐⭐⭐ |
| **Fase 3** | Performance e Escalabilidade | ✅ Completo | 14 dias | ⭐⭐⭐⭐⭐ |
| **Fase 4** | Observabilidade e Monitoramento | ✅ Completo | 14 dias | ⭐⭐⭐⭐⭐ |
| **Fase 5** | Segurança Avançada | ✅ Completo | 14 dias | ⭐⭐⭐⭐⭐ |
| **Fase 6** | Sistema de Pagamentos | ✅ Completo | 14 dias | ⭐⭐⭐⭐⭐ |
| **Fase 7** | Sistema de Payout e Gestão Financeira | ✅ Completo | 28 dias | ⭐⭐⭐⭐⭐ |
| **Fase 8** | Infraestrutura de Mídia | ✅ Completo | 15 dias | ⭐⭐⭐⭐⭐ |

**Referências**: [Status das Fases](./STATUS_FASES.md) | [Backlog API Completo](./backlog-api/README.md)

---

## 🗺️ Roadmap Estratégico - Próximas Fases

O roadmap estratégico foi reorganizado com base em análise comparativa de mercado e padrões de investimento, priorizando funcionalidades que elevam o Araponga ao nível de plataformas líderes. As fases foram reorganizadas em ondas estratégicas com foco em valor entregue e convergência de mercado.

### Onda 1: MVP Essencial (Mês 0-3) 🔴 CRÍTICO

**Objetivo**: Completar funcionalidades essenciais para MVP completo.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 9** | Perfil de Usuário Completo | 🔴 P0 | 21 dias | ⏳ Planejado |
| **Fase 10** | Mídias Avançadas (Vídeos, Áudios) | 🔴 P0 | 25 dias | ⏳ Planejado |
| **Fase 11** | Edição e Gestão | 🟡 P1 | 15 dias | ⏳ Planejado |
| **Fase 12** | Otimizações Finais | 🟡 P1 | 28 dias | ⏳ Planejado |

**Resultado Esperado**: MVP completo com todas as funcionalidades essenciais.

---

### Onda 2: Governança e Sustentabilidade (Mês 0-6) 🔴 CRÍTICO

**Objetivo**: Implementar base de governança participativa e sustentabilidade financeira.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 13** | Conector de Envio de Emails | 🔴 P0 | 14 dias | ⏳ Planejado |
| **Fase 14** | Governança Comunitária e Votação | 🔴 P0 | 21 dias | ⏳ Planejado |
| **Fase 15** | Subscriptions & Recurring Payments | 🔴 P0 | 45 dias | ⏳ Novo |
| **Fase 16** | Finalização Completa das Fases 1-15 | 🔴 P0 | 20 dias | ⏳ Novo ⭐ NOVA (renumerada de 14.8) |

**Resultado Esperado**: Governança participativa funcional, aumento de engajamento sem complexidade de blockchain. Base completa das fases 1-15 finalizada.

**Referência**: [Estratégia de Convergência - Fase 1](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md#fase-1-fundação-de-governança-mês-0-3) | [Fase 14.8](./backlog-api/FASE14_8.md)

---


---

### Onda 3: Economia Local (Mês 6-9) 🔴 CRÍTICO

**Objetivo**: Implementar funcionalidades de economia local que geram valor imediato.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 17** | Sistema de Compra Coletiva | 🔴 P0 | 28 dias | ⏳ Planejado ⬆️ **P1→P0** |
| **Fase 18** | Sistema de Hospedagem Territorial | 🔴 P0 | 56 dias | ⏳ Novo ⬆️ **P1→P0** |
| **Fase 19** | Sistema de Demandas e Ofertas | 🔴 P0 | 21 dias | ⏳ Novo ⬆️ **P1→P0** ⭐ NOVA |

**Resultado Esperado**: Economia local funcional com compras coletivas, hospedagem e demandas/ofertas, gerando valor imediato com pagamentos convencionais.

**Justificativa**: Considerando contexto brasileiro, economia local gera valor imediato sem necessidade de blockchain. Usuários podem usar serviços agora com PIX, cartão, etc.

**Referência**: [Reavaliação Blockchain Prioridade](./REAVALIACAO_BLOCKCHAIN_PRIORIDADE.md)

---

### Onda 7: Preparação Web3 (Mês 12+) 🟡 ALTA

**Objetivo**: Preparar infraestrutura técnica para integração blockchain quando houver demanda.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 31** | Avaliação e Escolha de Blockchain | 🟡 P1 | 14 dias | ⏳ Novo ⬇️ **P0→P1** |
| **Fase 32** | Camada de Abstração Blockchain | 🟡 P1 | 30 dias | ⏳ Novo ⬇️ **P0→P1** |
| **Fase 33** | Integração Wallet (WalletConnect) | 🟡 P1 | 30 dias | ⏳ Novo ⬇️ **P0→P1** |
| **Fase 34** | Smart Contracts Básicos | 🟡 P1 | 45 dias | ⏳ Novo ⬇️ **P0→P1** |
| **Fase 35** | Suporte a Criptomoedas | 🟡 P1 | 28 dias | ⏳ Planejado |

**Resultado Esperado**: Base técnica sólida para Web3, permitindo implementação de DAO quando houver demanda real.

**Justificativa**: Adoção brasileira de blockchain ainda é baixa. Web3 pode ser adicionado depois quando houver demanda real, priorizando funcionalidades que geram valor imediato.

**Referência**: [Reavaliação Blockchain Prioridade](./REAVALIACAO_BLOCKCHAIN_PRIORIDADE.md)

---

### Onda 8: DAO e Tokenização (Mês 18+) 🟡 ALTA

**Objetivo**: Implementar DAO completa com tokens on-chain quando houver demanda.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 36** | Tokens On-chain (ERC-20) | 🟡 P1 | 60 dias | ⏳ Novo ⬇️ **P0→P1** |
| **Fase 37** | Governança Tokenizada | 🟡 P1 | 30 dias | ⏳ Novo ⬇️ **P0→P1** |
| **Fase 38** | Proof of Presence On-chain | 🟡 P1 | 30 dias | ⏳ Novo |

**Resultado Esperado**: DAO completa e competitiva, alinhada com padrões de mercado, quando houver demanda real.

**Justificativa**: Adoção brasileira de blockchain ainda é baixa. DAO pode ser implementada depois quando houver demanda real, priorizando funcionalidades que geram valor imediato.

**Referência**: [Reavaliação Blockchain Prioridade](./REAVALIACAO_BLOCKCHAIN_PRIORIDADE.md)

---

### Onda 4: Economia Local Completa (Mês 9-12) 🟡 ALTA

**Objetivo**: Completar funcionalidades de economia local e circular.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 20** | Sistema de Trocas Comunitárias | 🟡 P1 | 21 dias | ⏳ Planejado ⭐ Renumerada (era 24) |
| **Fase 21** | Sistema de Entregas Territoriais | 🟡 P1 | 28 dias | ⏳ Planejado ⭐ Renumerada (era 28) |
| **Fase 22** | Sistema de Moeda Territorial | 🟡 P1 | 35 dias | ⏳ Planejado ⭐ Renumerada (era 20) |

**Resultado Esperado**: Economia local completa com compras coletivas, hospedagem, demandas/ofertas, trocas, entregas e moeda territorial.

**Justificativa**: Priorizar serviços que funcionam com pagamentos atuais, criando ecossistema robusto antes de implementar moeda territorial virtual.

### Onda 6: Autonomia Digital (Mês 12-18) 🟡 ALTA

**Objetivo**: Implementar funcionalidades de autonomia digital e serviços.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 26** | Hub de Serviços Digitais | 🟡 P1 | 21 dias | ⏳ Planejado ⭐ Renumerada (era 25) |
| **Fase 27** | Chat com IA | 🟡 P1 | 14 dias | ⏳ Planejado ⭐ Renumerada (era 26) |
| **Fase 28** | Negociação Territorial | 🟡 P1 | 28 dias | ⏳ Planejado ⭐ Renumerada (era 27) |
| **Fase 30** | Mobile Avançado | 🟡 P1 | 14 dias | ⏳ Planejado ⭐ Renumerada (era 29) |
| **Fase 48** | Banco de Sementes e Mudas | 🟢 P2 | 21 dias | ⏳ Planejado ⭐ Renumerada (era 28, 29) |

**Resultado Esperado**: Economia circular funcional com compras coletivas, hospedagem territorial, trocas, entregas e recursos compartilhados. Autonomia digital com serviços digitais integrados. Moeda territorial implementada após ecossistema robusto de serviços.

**Justificativa do Reposicionamento**: Priorizar serviços (Compra Coletiva, Hospedagem, Trocas, Entregas) que funcionam com pagamentos atuais, criando ecossistema robusto antes de implementar moeda territorial virtual.

**Referência**: [Análise de Inserção de Hospedagem](./ANALISE_INSERCAO_HOSPEDAGEM_ROADMAP.md) | [Proposta de Implementação](./PROPOSTA_IMPLEMENTACAO_HOSPEDAGEM.md) | [Análise de Coesão](./ANALISE_COESAO_FASES_15_FINAL.md) | [Análise Demandas/Ofertas e Reorganização](./ANALISE_DEMANDAS_OFERTAS_REORGANIZACAO.md)

---

### Onda 10: Extensões e Diferenciação (Mês 18+) 🟢 MÉDIA

**Objetivo**: Implementar funcionalidades que diferenciam o Araponga no mercado.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 44** | Integrações Externas | 🟢 P2 | 35 dias | ⏳ Planejado ⭐ Renumerada (era 22) |
| **Fase 45** | Learning Hub | 🟢 P2 | 60 dias | ⏳ Novo |
| **Fase 46** | Rental System | 🟢 P2 | 45 dias | ⏳ Novo |
| **Fase 48** | Banco de Sementes | 🟢 P2 | 21 dias | ⏳ Planejado ⭐ Renumerada (era 28, 29) |

**Resultado Esperado**: Plataforma completa e diferenciada, com funcionalidades avançadas.

**Referência**: [Estratégia de Convergência - Fase 5](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md#fase-5-diferenciação-mês-12-18)

---

### Onda 5: Conformidade e Soberania (Mês 6-12) 🟡 ALTA

**Objetivo**: Implementar funcionalidades de inteligência artificial, saúde territorial e métricas.

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 23** | Inteligência Artificial | 🟡 P1 | 28 dias | ⏳ Planejado ⭐ Renumerada (era 15) |
| **Fase 24** | Saúde Territorial e Monitoramento | 🟡 P1 | 35 dias | ⏳ Planejado ⭐ Renumerada (era 18) |
| **Fase 25** | Dashboard de Métricas | 🟡 P1 | 14 dias | ⏳ Novo |

**Nota**: Gamificação (Fase 42) foi movida para Onda 9, depois de funcionalidades core.

### Onda 9: Gamificação e Diferenciação (Mês 18+) 🟡 ALTA

**Objetivo**: Implementar gamificação e diferenciação (DEPOIS de funcionalidades core).

| Fase | Título | Prioridade | Duração | Status |
|------|--------|------------|---------|--------|
| **Fase 42** | Gamificação Harmoniosa | 🟡 P1 | 28 dias | ⏳ Planejado ⭐ Renumerada (era 17) |
| **Fase 43** | Arquitetura Modular | 🟡 P1 | 35 dias | ⏳ Planejado ⭐ Renumerada (era 19) |

**Resultado Esperado**: Gamificação implementada DEPOIS de funcionalidades que enriquecem o produto, servindo como incentivo para uso de funcionalidades já implementadas.

**Justificativa**: Gamificação é decoração/incentivo, não funcionalidade core. Deve vir depois de funcionalidades que geram valor real para usuários.

**Referência**: [Análise de Reorganização](./ANALISE_DEMANDAS_OFERTAS_REORGANIZACAO.md)

---

## 📊 Resumo Estratégico

### Priorização Atualizada

**Nota**: As fases estão organizadas em ordem sequencial de prioridade de implementação. Ver [Ordem de Fases por Prioridade](./ORDEM_FASES_POR_PRIORIDADE.md) para detalhes completos.

| Prioridade | Descrição | Timeline | Fases (Ordem Sequencial) |
|------------|-----------|----------|--------------------------|
| 🔴 **P0 - Crítico** | Valor imediato e funcionalidades essenciais | 0-12 meses | 9-10, 13-16, 17-19 |
| 🟡 **P1 - Alta** | Importante, incluindo Web3 quando houver demanda | 0-18 meses | 11-12, 20-30, 31-42 |
| 🟢 **P2 - Média** | Desejável, mas não bloqueante | 12-24 meses | 43-48 |

**Nota**: Blockchain (Fases 16-21) foi reposicionada de P0 para P1 considerando contexto brasileiro. Ver [Reavaliação Blockchain Prioridade](./REAVALIACAO_BLOCKCHAIN_PRIORIDADE.md)

**Referência**: [Mapeamento de Renumeração](./MAPEAMENTO_RENUMERACAO_FASES.md) | [Ordem de Fases por Prioridade](./ORDEM_FASES_POR_PRIORIDADE.md)

### Marcos Críticos

| Marco | Prazo | Funcionalidades | Impacto |
|-------|-------|-----------------|---------|
| **Governança Básica** | Mês 3 | Votação (Fase 14) + Subscriptions (Fase 15) | Alto |
| **Economia Local** | Mês 6 | Compra Coletiva (Fase 17) + Hospedagem (Fase 18) + Demandas/Ofertas (Fase 19) | Crítico |
| **Sustentabilidade** | Mês 6 | Subscriptions (Fase 15) + Ticketing (Fase 39) | Médio-Alto |
| **Web3 Ready** | Mês 12+ | Blockchain (Fases 31-34) + Wallets (Fase 33) | Médio (quando houver demanda) |
| **DAO Completa** | Mês 18+ | Tokens (Fase 36) + Governança Tokenizada (Fase 37) | Médio (quando houver demanda) |
| **Diferenciação** | Mês 18+ | Learning Hub (Fase 46) + Rental System (Fase 47) + IA (Fase 23, 40) | Médio |

---

## 📚 Referências Estratégicas

- **[Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md)** - Plano estratégico completo de convergência
- **[Mapa de Funcionalidades](./38_MAPA_FUNCIONALIDADES_MERCADO.md)** - Mapeamento completo de funcionalidades vs. mercado
- **[Backlog API Completo](./backlog-api/README.md)** - Detalhes completos de todas as fases
- **[Status das Fases](./STATUS_FASES.md)** - Status detalhado de implementação

---

**Última Atualização**: 2026-01-25  
**Versão**: 3.2 - Numeração Coerente  
**Status**: ✅ MVP Completo | 📊 Estratégia Atualizada | 🔄 Numeração Coerente

**Nota Importante**: As fases foram reorganizadas para garantir **coerência na numeração**: fases com números menores são implementadas antes de fases com números maiores. A **Fase 14.8** foi renumerada para **Fase 16** (depois de Fase 15), e todas as fases subsequentes foram renumeradas para manter a sequência coerente.

**Referências**: 
- [Mapa Completo das Fases](./backlog-api/MAPA_FASES.md) ⭐ NOVO - Mapa centralizado
- [Guia de Reorganização](./backlog-api/GUIA_REORGANIZACAO_FASES.md) ⭐ NOVO - Guia passo-a-passo
- [Reorganização Numeração Coerente](./REORGANIZACAO_NUMERACAO_COERENTE.md)
- [Ordem de Fases por Prioridade](./ORDEM_FASES_POR_PRIORIDADE.md)
- [Mapeamento de Renumeração](./MAPEAMENTO_RENUMERACAO_FASES.md)
