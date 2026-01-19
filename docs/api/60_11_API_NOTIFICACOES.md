# Notificações - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🔔 Notificações

### Listar Notificações (`GET /api/v1/notifications`)

**Descrição**: Obtém notificações do usuário autenticado.

**Como usar**:
- Exige autenticação
- Query params: `skip`, `take` (paginação)

**Regras de negócio**:
- **Paginação**: Padrão 50 itens
- **Ordenação**: Mais recentes primeiro
- **Tipos**: Post criado, report criado, inquiry recebido, etc.
- **Sistema**: Notificações são geradas via outbox/inbox confiável

### Marcar como Lida (`POST /api/v1/notifications/{id}/read`)

**Descrição**: Marca uma notificação como lida.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da notificação)

**Regras de negócio**:
- **Permissão**: Apenas o dono da notificação pode marcar como lida
- **Idempotente**: Pode marcar múltiplas vezes sem efeito

---

## 📚 Documentação Relacionada

- **[Preferências de Usuário](./60_18_API_PREFERENCIAS.md)** - Configurar preferências de notificações
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/notifications/paged`
- **DevPortal**: [Notificações Outbox](../devportal/#fluxo-notifications-outbox) - Diagrama de sequência

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
