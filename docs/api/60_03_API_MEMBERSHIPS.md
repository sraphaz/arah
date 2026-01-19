# Vínculos e Membros (Memberships) - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 👥 Vínculos e Membros (Memberships)

### Entrar no território como VISITOR (`POST /api/v1/territories/{territoryId}/enter`)

**Descrição**: Cria (ou retorna) o vínculo do usuário no território como **VISITOR**.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`

**Regras de negócio**:
- Cria `TerritoryMembership` com `Role=VISITOR` e `ResidencyVerification=NONE`
- Não existe "validação" para VISITOR; é um vínculo leve para acesso ao conteúdo público

### Solicitar residência (cria JoinRequest) (`POST /api/v1/memberships/{territoryId}/become-resident`)

**Descrição**: Cria uma solicitação (JoinRequest) para virar **RESIDENT**. O usuário permanece VISITOR até aprovação.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`
- Body opcional:
  - `recipientUserIds` (array) para convite direcionado (quando conhece alguém)
  - `message` (string) opcional

**Regras de negócio**:
- Se `recipientUserIds` for informado, o pedido é direcionado para esses destinatários (desde que elegíveis).
- Se não informar destinatários, o pedido vai para **Curator** do território.
- Se não houver Curator, faz fallback para **SystemAdmin**.
- Idempotente: se já houver JoinRequest pendente, retorna a mesma solicitação
- Regra: 1 Resident por User (se já for Resident em outro território, deve transferir)
- Anti-abuso:
  - `recipientUserIds` tem limite de **3** destinatários
  - Rate limit: no máximo **3** criações (create+cancel+recreate) por usuário/território em janela de **24h**
  - Quando estourar o rate limit, a API retorna **429 Too Many Requests**

### Consultar meu vínculo no território (`GET /api/v1/memberships/{territoryId}/me`)

**Descrição**: Consulta o vínculo do usuário autenticado com um território.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`

**Regras de negócio**:
- Retorna `role` e `residencyVerification` (`NONE`, `GEOVERIFIED`, `DOCUMENTVERIFIED`)
- Se não houver vínculo, retorna `404`

### Verificar residência por geolocalização (`POST /api/v1/memberships/{territoryId}/verify-residency/geo`)

**Descrição**: Marca `ResidencyVerification=GEOVERIFIED` quando as coordenadas estão dentro do raio permitido do território.

**Regras de negócio**:
- Requer `Role=RESIDENT` no território
- Não substitui aprovação do JoinRequest: é um passo de verificação pós-aprovação

### Verificar residência por documento (`POST /api/v1/memberships/{territoryId}/verify-residency/document`)

**Descrição**: Marca `ResidencyVerification=DOCUMENTVERIFIED`.

**Regras de negócio**:
- Requer `Role=RESIDENT` no território
- Fluxo completo com upload/evidências e revisão humana está detalhado em `33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md`

---

## 📚 Documentação Relacionada

- **[Territórios](./60_02_API_TERRITORIOS.md)** - Seleção de território
- **[Solicitações de Entrada](./60_13_API_JOIN_REQUESTS.md)** - Processo de aprovação
- **[Verificações e Evidências](./60_00_API_EVIDENCIAS.md)** - Upload de documentos
- **[Regras de Visibilidade](./60_17_API_VISIBILIDADE.md)** - Permissões por role

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
