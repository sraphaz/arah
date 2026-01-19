# Admin: System Config e Work Queue - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 🧰 Admin: System Config e Work Queue

> Referência detalhada: **[33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md](../33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)**.

### System Config (SystemAdmin)

**Objetivo**: centralizar configurações calibráveis (providers, segurança, moderação, validação).

- `GET /api/v1/admin/system-config`
- `GET /api/v1/admin/system-config/{key}`
- `PUT /api/v1/admin/system-config`

### Work Items (filas)

**Objetivo**: padronizar revisões humanas (verificação, curadoria, moderação).

**Globais (SystemAdmin)**:
- `GET /api/v1/admin/work-items`
- `POST /api/v1/admin/work-items/{workItemId}/complete`

**Territoriais (Curator/Moderator)**:
- `GET /api/v1/territories/{territoryId}/work-items`
- `POST /api/v1/territories/{territoryId}/work-items/{workItemId}/complete`

---

## 📚 Documentação Relacionada

- **[33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md](../33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)** - Referência detalhada completa
- **[Verificações e Evidências](./60_00_API_EVIDENCIAS.md)** - Upload/download de evidências
- **[Moderação](./60_12_API_MODERACAO.md)** - Sistema de reports e curadoria

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
