# API Araponga - Lógica de Negócio e Usabilidade

**Documento de Negócio Completo**  
**Versão**: 2.0  
**Data**: 2025-01-20  
**Última Atualização**: 2025-01-20 (Reorganizado em subdocumentos)

---

## 📋 Índice da Documentação da API

Este documento foi reorganizado em subdocumentos para melhor navegação e manutenção. Cada seção principal está documentada em seu próprio arquivo.

### 📚 Documentos por Categoria

#### 🎯 Fundamentos e Configuração

1. **[Visão Geral e Princípios](./60_00_API_VISAO_GERAL.md)** - Princípios fundamentais, segurança, rate limiting
2. **[Paginação](./60_00_API_PAGINACAO.md)** - Padrão de paginação em todos os endpoints
3. **[Verificações e Evidências](./60_00_API_EVIDENCIAS.md)** - Sistema de upload/download de evidências

#### 🔐 Autenticação e Identidade

4. **[Autenticação e Cadastro](./60_01_API_AUTENTICACAO.md)** - Login social, tokens JWT

#### 🗺️ Territórios e Vínculos

5. **[Territórios](./60_02_API_TERRITORIOS.md)** - Listagem, busca, seleção de territórios
6. **[Vínculos e Membros (Memberships)](./60_03_API_MEMBERSHIPS.md)** - VISITOR, RESIDENT, verificação de residência

#### 📝 Conteúdo e Interação

7. **[Feed Comunitário](./60_04_API_FEED.md)** - Posts, curtidas, comentários, compartilhamentos
8. **[Eventos](./60_05_API_EVENTOS.md)** - Criação, listagem, participação em eventos
9. **[Mídias em Conteúdo](./60_15_API_MIDIAS.md)** - Imagens, vídeos e áudios em posts, eventos, marketplace, chat

#### 🗺️ Mapa e Recursos

10. **[Mapa Territorial](./60_06_API_MAPA.md)** - Entidades do mapa, pins, confirmações
11. **[Alertas de Saúde](./60_07_API_ALERTAS.md)** - Alertas públicos de saúde
12. **[Assets (Recursos Territoriais)](./60_08_API_ASSETS.md)** - Recursos compartilhados do território

#### 💰 Economia e Marketplace

13. **[Marketplace](./60_09_API_MARKETPLACE.md)** - Loja territorial, itens, carrinho, checkout, payout

#### 💬 Comunicação

14. **[Chat](./60_10_API_CHAT.md)** - Canais, grupos, mensagens diretas, mídias no chat
15. **[Notificações](./60_11_API_NOTIFICACOES.md)** - Sistema de notificações push e in-app

#### 🛡️ Moderação e Administração

16. **[Moderação](./60_12_API_MODERACAO.md)** - Sistema de moderação e reports
17. **[Solicitações de Entrada (Join Requests)](./60_13_API_JOIN_REQUESTS.md)** - Solicitações de residência
18. **[Admin: System Config e Work Queue](./60_14_API_ADMIN.md)** - Configurações globais e filas de trabalho

#### ⚙️ Configurações e Regras

19. **[Feature Flags](./60_16_API_FEATURE_FLAGS.md)** - Sistema de feature flags por território
20. **[Regras de Visibilidade e Permissões](./60_17_API_VISIBILIDADE.md)** - Regras de acesso e visibilidade de conteúdo
21. **[Preferências de Usuário](./60_18_API_PREFERENCIAS.md)** - Configurações e preferências do usuário
22. **[Governança Comunitária](./60_19_API_GOVERNANCA.md)** - Interesses, votações, caracterização, histórico de participação

#### 📊 Referência Rápida

23. **[Resumo de Endpoints Principais](./60_99_API_RESUMO_ENDPOINTS.md)** - Lista consolidada de todos os endpoints

#### 🎯 Backend for Frontend (BFF)

24. **[Avaliação BFF](../AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md)** ⭐ - Avaliação estratégica completa da API BFF
25. **[Avaliação BFF - Resumo](../AVALIACAO_BFF_RESUMO.md)** 📋 - Resumo executivo para decisão rápida
26. **[Contrato OpenAPI BFF](../BFF_API_CONTRACT.yaml)** 📄 - Contrato OpenAPI 3.0.3 completo (v2/journeys)
27. **[Resumo de Contratos BFF](../BFF_CONTRACT_SUMMARY.md)** 📝 - Referência rápida dos contratos BFF
28. **[Guia de Implementação Frontend](../BFF_FRONTEND_IMPLEMENTATION_GUIDE.md)** 💻 - Guia completo para frontend
29. **[Exemplo Flutter Completo](../BFF_FLUTTER_EXAMPLE.md)** 📱 - Exemplo completo em Flutter/Dart
30. **[Quick Start Flutter](../BFF_FLUTTER_QUICKSTART.md)** 🚀 - Exemplo mínimo para começar
31. **[Coleção Postman BFF](../BFF_Postman_Collection.json)** 🧪 - Coleção Postman completa
32. **[Guia de Uso Postman](../BFF_POSTMAN_README.md)** 📖 - Guia de uso da coleção Postman

> **Nota**: A API BFF (v2) expõe operações em forma de jornadas (user journeys), reduzindo significativamente o número de chamadas necessárias. A API v1 continua disponível e funcional.

---

## 🚀 Como Usar Esta Documentação

### Para Desenvolvedores

1. **Comece pela [Visão Geral](./60_00_API_VISAO_GERAL.md)** para entender os princípios fundamentais
2. **Configure [Autenticação](./60_01_API_AUTENTICACAO.md)** para obter tokens JWT
3. **Explore [Territórios](./60_02_API_TERRITORIOS.md)** para entender o contexto territorial
4. **Consulte [Paginação](./60_00_API_PAGINACAO.md)** para listagens
5. **Use o [Resumo de Endpoints](./60_99_API_RESUMO_ENDPOINTS.md)** como referência rápida

### Para Integradores

- Consulte o **DevPortal** (`devportal.araponga.app/`) para exemplos práticos de código
- Use o **OpenAPI Explorer** (`devportal.araponga.app/#openapi`) para explorar contratos
- Veja os **Diagramas de Sequência** (`devportal.araponga.app/#fluxos`) para entender fluxos

### Para Analistas Funcionais

- Veja a [Wiki - Visão do Produto](../01_PRODUCT_VISION.md) para entender a visão estratégica
- Consulte a [Wiki - Modelo de Domínio](../12_DOMAIN_MODEL.md) para entender a arquitetura
- Explore os [User Stories](../04_USER_STORIES.md) para casos de uso detalhados

---

## 📖 Links Relacionados

- **DevPortal**: `devportal.araponga.app/` - Portal de desenvolvedor com exemplos práticos
- **Wiki**: `devportal.araponga.app/wiki/` - Documentação completa do projeto
- **GitHub**: `github.com/sraphaz/araponga` - Código-fonte
- **API BFF**: [Avaliação BFF](../AVALIACAO_BFF_BACKEND_FOR_FRONTEND.md) | [Contrato OpenAPI](../BFF_API_CONTRACT.yaml) | [Exemplo Flutter](../BFF_FLUTTER_EXAMPLE.md) - API v2 com jornadas otimizadas

---

**Última Atualização**: 2025-01-20  
**Versão da API**: v1  
**Status**: Produção
