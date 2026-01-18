# Visão do Produto - Araponga
## Plataforma de Organização Comunitária Territorial

**Versão**: 3.0  
**Data**: 2025-01-20  
**Última Atualização**: 2025-01-20  
**Status**: ✅ MVP Completo + Fases 1-8 Implementadas | 📊 Estratégia de Convergência em Andamento

---

## 🎯 Propósito e Visão

**Araponga** é uma plataforma operacional para organização comunitária territorial que combina infraestrutura digital robusta com governança descentralizada e economia circular tokenizada. A plataforma evolui de um MVP sólido para uma solução completa que compete com padrões de mercado estabelecidos por projetos que recebem investimentos significativos.

### Princípios Fundamentais

Araponga é **território-first** e **comunidade-first**: o território físico é a unidade central e a presença no território é requisito para vínculo. Este princípio fundamental diferencia o Araponga no mercado, combinando organização geográfica com governança participativa e economia local.

### Evolução Estratégica

O Araponga incorpora padrões de mercado estabelecidos (DAO, tokenização, Web3) mantendo os valores fundamentais do projeto. A convergência estratégica não significa abandonar valores, mas sim incorporar tecnologias e modelos essenciais para competir no mercado de investimento.

**Referência Estratégica**: [Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md) | [Mapa de Funcionalidades](./38_MAPA_FUNCIONALIDADES_MERCADO.md)

## Princípios e regras inegociáveis
- **Território é a unidade central** e representa um lugar físico real.
- **Presença física é critério de vínculo**: no MVP não é possível associar território remotamente.
- **Consulta exige cadastro**: no MVP, feed e mapa exigem usuário autenticado.
- **Conteúdo pode ser georreferenciado via GeoAnchor**.
- **Feed pessoal (perfil) + feed do território** coexistem.
- **Postagens podem ter 0..N GeoAnchors**, derivados de mídias quando disponíveis.
- **Visibilidade e papéis**: visitor e resident no MVP; friends/círculo interno é pós-MVP.
- **Mapa + feed integrados**: pins retornam dados mínimos para projeção no mapa; sincronia UI é pós-MVP.
- **Moderação**: reports de posts e de usuários; bloqueio; automações simples no MVP; sanções territoriais vs globais.
- **Notificações in-app**: eventos geram mensagens confiáveis via outbox, convertidas em inbox do usuário.
- **Admin**: visão administrativa para observar territórios, erros e relatórios (pós-MVP).

## Fluxo mínimo (MVP)
1. Usuário se cadastra.
2. A localização permite encontrar o território próximo (ex.: “Sertão do Camburi”).
3. Entra como **visitor**.
4. É incentivado a postar no feed.
5. A postagem aparece no mapa (via pins) quando há GeoAnchor, e sempre no feed; a sincronia visual entre pin e timeline fica para pós-MVP.

## Funcionalidades principais (classificação)

### ✅ Implementado (MVP + Fases 1-8)

- [✅ MVP] **Território + vínculo** com presença física local.
- [✅ MVP] **Feeds** (pessoal e do território) com posts georreferenciados.
- [✅ MVP] **Mapa integrado ao feed** via pins com dados mínimos.
- [✅ MVP] **Visibilidade visitor/resident**.
- [✅ MVP] **Reports e bloqueio** com moderação básica.
- [✅ MVP] **Notificações in-app** com outbox/inbox confiáveis.
- [✅ Fase 1] **Segurança e Fundação Crítica** - JWT, Rate Limiting, CORS, HTTPS, validações robustas.
- [✅ Fase 2] **Qualidade de Código** - Paginação completa (15 endpoints), FluentValidation, cobertura de testes >90%.
- [✅ Fase 3] **Performance e Escalabilidade** - Cache distribuído, otimizações de queries, paginação eficiente.
- [✅ Fase 4] **Observabilidade** - Logging estruturado, métricas, health checks.
- [✅ Fase 5] **Segurança Avançada** - 2FA, sanitização, CSRF protection, secrets management.
- [✅ Fase 6] **Sistema de Pagamentos** - Integração com gateway de pagamento, gestão de transações.
- [✅ Fase 7] **Sistema de Payout** - Pagamentos para vendedores, gestão de saldos.
- [✅ Fase 8] **Infraestrutura de Mídia** - Upload, armazenamento (S3/Local), processamento de imagens.
- [✅ Fase 8] **Mídias em Conteúdo** - Posts com múltiplas imagens (até 10), eventos com capa, marketplace com imagens, chat com imagens.
- [✅ MVP] **Marketplace Completo** - Stores, Items, Cart, Checkout, Inquiries.
- [✅ MVP] **Eventos Comunitários** - Criação, participação, georreferenciamento.
- [✅ MVP] **Chat Territorial** - Canais públicos/moradores, grupos com aprovação.
- [✅ MVP] **Assets Territoriais** - Recursos com geolocalização obrigatória.
- [✅ MVP] **Sistema de Mídia** - Upload, armazenamento, processamento.

### ⏳ Planejado - Fundação de Governança (Mês 0-3)

- [⏳ Fase 14] **Governança Comunitária e Votação** - Sistema de votação tradicional, preparação para blockchain
- [⏳ Fase 30] **Proof of Sweat (Tradicional)** - Sistema de registro de atividades territoriais, recompensas por participação
- [⏳ Fase 31] **Dashboard de Métricas Comunitárias** - Transparência e visualização de impacto

### ⏳ Planejado - Sustentabilidade Financeira (Mês 3-6)

- [⏳ Fase 32] **Subscriptions & Recurring Payments** - Planos de assinatura, pagamentos recorrentes
- [⏳ Fase 33] **Ticketing para Eventos** - Venda de ingressos, QR codes, controle de capacidade
- [⏳ Fase 13] **Conector de Emails** - Notificações por email

### ⏳ Planejado - Essencial Pós-MVP (Mês 0-6)

- [⏳ Fase 9] **Perfil de Usuário Completo** - Preferências, configurações avançadas
- [⏳ Fase 10] **Mídias Avançadas** - Vídeos, áudios, documentos
- [⏳ Fase 11] **Edição e Gestão** - Edição de posts, eventos, items

### ⏳ Planejado - Preparação Web3 (Mês 6-9)

- [⏳ Fase 34-37] **Integração Blockchain** - Avaliação blockchain, camada de abstração, wallets, smart contracts

### ⏳ Planejado - DAO e Tokenização (Mês 9-12)

- [⏳ Fase 38] **Tokens On-chain** - Smart contracts de tokens (ERC-20), distribuição inicial
- [⏳ Fase 39] **Governança Tokenizada** - Votações on-chain, execução automática, histórico imutável
- [⏳ Fase 20] **Moeda Territorial (Web3)** - Integração com blockchain, conversão moeda ↔ token
- [⏳ Fase 40] **Proof of Presence On-chain** - Check-ins on-chain, mint de tokens por presença

### ⏳ Planejado - Soberania Territorial (Mês 6-12)

- [⏳ Fase 18] **Saúde Territorial e Monitoramento** - Atividades territoriais, sensores, observações
- [⏳ Fase 17] **Gamificação Harmoniosa** - Sistema de pontos, badges, reconhecimento

### ⏳ Planejado - Economia Circular (Mês 12-18)

- [⏳ Fase 23] **Compra Coletiva** - Organização de compras comunitárias
- [⏳ Fase 24] **Sistema de Trocas Comunitárias** - Trocas locais, economia circular

### ⏳ Planejado - Diferenciação (Mês 12-18)

- [⏳ Fase 41] **Learning Hub** - Sistema de cursos, certificações, monetização de conhecimento
- [⏳ Fase 42] **Booking System** - Reservas de hóspedes/voluntários, gestão de acomodações
- [⏳ Fase 43] **Agente IA (Versão Básica)** - Chatbot, sugestões automáticas, moderação assistida

### ⏳ Planejado - Otimizações (Mês 6-18)

- [⏳ Fase 12] **Otimizações Finais** - Performance, escalabilidade, refinamentos
- [⏳ Fase 15] **Inteligência Artificial** - IA para governança, suporte, automações
- [⏳ Fase 21] **Suporte a Criptomoedas** - Pagamentos em cripto, conversão
- [⏳ Fase 22] **Integrações Externas** - APIs de terceiros, webhooks
- [⏳ Fase 29] **Suporte Mobile Avançado** - Push notifications, background tasks, deep linking

**Referências**: [Roadmap Estratégico](./02_ROADMAP.md) | [Backlog API Completo](./backlog-api/README.md)

## 🎯 Público e Impacto Esperado

### Público-Alvo

**Usuários Principais**:
- **Moradores** de territórios (Residents) - Participação plena, voto em votações, acesso a recursos
- **Visitantes** de territórios (Visitors) - Acesso limitado, participação em eventos
- **Curadores e Moderadores** - Governança territorial, moderação de conteúdo
- **Organizações Locais** - Gestão de territórios, eventos, marketplace

### Impacto Esperado

**Sustentabilidade Comunitária**:
- Fortalecimento de redes locais através de comunicação digital ancorada ao território
- Autonomia territorial através de governança participativa e economia circular
- Transparência e democracia através de votações e decisões coletivas auditáveis

**Diferenciação de Mercado**:
- Plataforma competitiva ao nível de projetos com investimento significativo
- Governança descentralizada com DAO e tokens on-chain
- Economia circular tokenizada que recompensa participação ativa

## ✅ Boas Práticas Transversais

### Privacidade e Segurança

- **Consentimento explícito de localização** e explicação do motivo da coleta
- **Falhas de geolocalização** comunicadas com orientação clara para o usuário
- **Proteção de dados sensíveis** (LGPD/GDPR compliance)
- **Auditoria completa** de ações de moderação e sanções

### Observabilidade e Transparência

- **Observabilidade completa** (logs estruturados, métricas, health checks)
- **Proteção contra abuso** (deduplicação de reports por janela de tempo)
- **Transparência total** em decisões comunitárias (votações auditáveis)
- **Rastreabilidade** de todas as operações críticas

### Governança

- **Decisões coletivas** através de votações transparentes
- **Distribuição de poder** proporcional (tradicional ou tokenizada)
- **Auditoria on-chain** quando blockchain estiver implementado
- **Histórico imutável** de votações e decisões importantes

---

## 📚 Referências Estratégicas

- **[Roadmap Estratégico](./02_ROADMAP.md)** - Planejamento completo de desenvolvimento
- **[Estratégia de Convergência de Mercado](./39_ESTRATEGIA_CONVERGENCIA_MERCADO.md)** - Plano estratégico de convergência
- **[Mapa de Funcionalidades](./38_MAPA_FUNCIONALIDADES_MERCADO.md)** - Mapeamento completo vs. mercado
- **[Backlog API Completo](./backlog-api/README.md)** - Detalhes de todas as fases

---

**Última Atualização**: 2025-01-20  
**Versão**: 3.0  
**Status**: ✅ MVP Completo | 📊 Estratégia Atualizada
