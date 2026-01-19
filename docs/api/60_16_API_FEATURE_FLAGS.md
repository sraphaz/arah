# Feature Flags - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## ⚙️ Feature Flags

### Listar Feature Flags (`GET /api/v1/territories/{territoryId}/features`)

**Descrição**: Obtém feature flags habilitadas no território.

**Como usar**:
- Exige autenticação
- Path param: `territoryId`

**Regras de negócio**:
- **Permissão**: Todos usuários autenticados podem consultar
- **Retorno**: Lista de flags **habilitadas** no território (strings em `UPPERCASE`, sem underscores).
- **Exemplos**: `ALERTPOSTS`, `MARKETPLACEENABLED`, `CHATENABLED`, etc.

### Atualizar Feature Flags (`PUT /api/v1/territories/{territoryId}/features`)

**Descrição**: Atualiza feature flags do território (curadoria).

**Como usar**:
- Exige autenticação
- Path param: `territoryId`
- Body: `enabledFlags: string[]` (lista de flags habilitadas, em qualquer casing)

**Regras de negócio**:
- **Permissão**: Apenas curadores (CURATOR) podem atualizar
- **Validação**: Flags inválidas são rejeitadas
- **Auditoria**: Alterações são registradas em log

### Flags Principais

- **`ALERTPOSTS`**: Habilita posts do tipo ALERT no feed
- **`MARKETPLACEENABLED`**: Habilita marketplace no território
- **`CHATENABLED`**: Master switch do chat no território
- **`CHATTERITORYPUBLICCHANNEL`**: Habilita canal público do chat
- **`CHATTERITORYRESIDENTSCHANNEL`**: Habilita canal de moradores do chat
- **`CHATGROUPS`**: Habilita criação de grupos no chat
- **`CHATDMENABLED`**: Habilita mensagens diretas (DM)
- **`CHATMEDIAENABLED`**: Habilita mídias no chat
- **`MEDIAIMAGESENABLED`**: Habilita imagens em posts/eventos
- **`MEDIAVIDEOSENABLED`**: Habilita vídeos em posts/eventos
- **`MEDIAAUDIOENABLED`**: Habilita áudios em posts/eventos

---

## 📚 Documentação Relacionada

- **[Chat](./60_10_API_CHAT.md)** - Feature flags do chat
- **[Mídias](./60_15_API_MIDIAS.md)** - Feature flags de mídias
- **[Marketplace](./60_09_API_MARKETPLACE.md)** - Feature flag do marketplace
- **[Feed Comunitário](./60_04_API_FEED.md)** - Feature flag de alertas

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
