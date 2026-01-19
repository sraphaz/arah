# Verificações e Evidências - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 📎 Verificações e Evidências (upload/download)

### Upload (multipart/form-data)

- **Identidade (global)**:
  - `POST /api/v1/verification/identity/document/upload`
- **Residência (territorial)**:
  - `POST /api/v1/memberships/{territoryId}/verify-residency/document/upload`

### Decisão de verificação (fila humana)

- **Identidade (SystemAdmin)**:
  - `POST /api/v1/admin/verifications/identity/{workItemId}/decide`
- **Residência (Curator)**:
  - `POST /api/v1/territories/{territoryId}/verification/residency/{workItemId}/decide`

### Download por proxy (stream via API)

- **Admin (SystemAdmin)**:
  - `GET /api/v1/admin/evidences/{evidenceId}/download`
- **Território (Curator/Moderator)**:
  - `GET /api/v1/territories/{territoryId}/evidences/{evidenceId}/download`

---

## 📚 Documentação Relacionada

- **[Admin: System Config e Work Queue](./60_14_API_ADMIN.md)** - Sistema de filas e configurações
- **[Vínculos e Membros](./60_03_API_MEMBERSHIPS.md)** - Verificação de residência
- **[33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md](../33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)** - Referência detalhada

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
