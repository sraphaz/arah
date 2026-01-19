# Solicitações de Entrada (Join Requests) - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🔗 Solicitações de Entrada (Join Requests)

> Nota: o caminho recomendado para "virar morador" é `POST /api/v1/memberships/{territoryId}/become-resident`,
> que cria a JoinRequest com destinatários automáticos. O endpoint abaixo existe para casos avançados (escolha manual).

### Criar Solicitação (`POST /api/v1/territories/{territoryId}/join-requests`)

**Descrição**: Solicita aprovação para virar morador (escolhendo destinatários específicos).

**Como usar**:
- Exige autenticação
- Path param: `territoryId`
- Body: `recipientUserIds` (array de IDs de usuários destinatários)

**Regras de negócio**:
- **Permissão**: Visitantes autenticados podem criar solicitações
- **Destinatários**: Apenas moradores já verificados (geo/doc) ou curadores podem ser destinatários (SystemAdmin também é aceito)
- **Status**: Solicitação é criada como `PENDING`
- **Não gera post**: Solicitação não aparece no feed (não é broadcast)
- **Privacidade**: Apenas destinatários veem a solicitação

### Listar Solicitações Recebidas (`GET /api/v1/join-requests/incoming`)

**Descrição**: Lista solicitações onde o usuário é destinatário.

**Como usar**:
- Exige autenticação
- Query params: `status` (PENDING, APPROVED, REJECTED), `skip`, `take` (paginação)

**Regras de negócio**:
- **Permissão**: Apenas destinatários veem suas solicitações recebidas
- **Filtros**: `status` é opcional
- **Paginação**: Padrão 20 itens

### Aprovar Solicitação (`POST /api/v1/join-requests/{id}/approve`)

**Descrição**: Aprova uma solicitação de entrada.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da solicitação)

**Regras de negócio**:
- **Permissão**: Apenas destinatários da solicitação ou curadores podem aprovar
- **Promoção**: Ao aprovar, o requester recebe membership `RESIDENT` com `ResidencyVerification=NONE` (não verificado)
- **Status**: Solicitação é marcada como `APPROVED`

### Rejeitar Solicitação (`POST /api/v1/join-requests/{id}/reject`)

**Descrição**: Rejeita uma solicitação de entrada.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da solicitação)

**Regras de negócio**:
- **Permissão**: Apenas destinatários da solicitação ou curadores podem rejeitar
- **Não promove**: Rejeição não altera membership do requester
- **Status**: Solicitação é marcada como `REJECTED`

### Cancelar Solicitação (`POST /api/v1/join-requests/{id}/cancel`)

**Descrição**: Cancela uma solicitação criada pelo próprio usuário.

**Como usar**:
- Exige autenticação
- Path param: `id` (ID da solicitação)

**Regras de negócio**:
- **Permissão**: Apenas o criador da solicitação pode cancelar
- **Status**: Solicitação é marcada como `CANCELLED`

---

## 📚 Documentação Relacionada

- **[Vínculos e Membros](./60_03_API_MEMBERSHIPS.md)** - Processo completo de virar morador
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Permissões após aprovação
- **[Paginação](./60_00_API_PAGINACAO.md)** - Versão paginada: `GET /api/v1/join-requests/incoming/paged`

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
