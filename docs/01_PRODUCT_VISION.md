# Visão do Produto (ARAPONGA)

**Versão**: 2.0  
**Data**: 2025-01-20  
**Última Atualização**: 2025-01-20  
**Status**: ✅ MVP Completo + Fases 1-8 Implementadas

---

## Propósito
Araponga é uma plataforma **território-first** e **comunidade-first** para organização comunitária local.
O território físico é a unidade central e a presença no território é requisito para vínculo.

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

### ⏳ Planejado (Pós-MVP - Fases 9-29)

- [⏳ Fase 9] **Perfil de Usuário Completo** - Preferências, configurações avançadas.
- [⏳ Fase 10] **Mídias em Conteúdo** - Vídeos, áudios, documentos.
- [⏳ Fase 11] **Edição e Gestão** - Edição de posts, eventos, items.
- [⏳ Fase 13] **Conector de Emails** - Notificações por email.
- [⏳ Fase 14] **Governança Comunitária** - Votação, decisões coletivas.
- [⏳ POST-MVP] **Friends (círculo interno)** e stories exclusivos.
- [⏳ POST-MVP] **Admin/observabilidade** com visão de territórios e saúde do sistema.
- [⏳ POST-MVP] **GeoAnchor avançado / memórias / galeria**.
- [⏳ POST-MVP] **Produtos/serviços territoriais, integrações e indicadores comunitários**.
- [⏳ Fases 15-29] Ver [Backlog API](./backlog-api/README.md) para detalhes completos das 29 fases.

## Público e impacto esperado
- Moradores, visitantes e organizações locais que precisam de um canal digital **ancorado ao território**.
- Fortalecimento de redes locais, comunicação de utilidade pública e presença comunitária real.

## Boas práticas transversais (MVP)
- **Consentimento explícito de localização** e explicação do motivo da coleta.
- **Falhas de geolocalização** comunicadas com orientação clara para o usuário.
- **Auditoria mínima** de ações de moderação e sanções.
- **Proteção contra abuso de reports** (deduplicação por janela de tempo).
- **Observabilidade mínima** (logs e métricas para erros de localização, reports e moderação).
