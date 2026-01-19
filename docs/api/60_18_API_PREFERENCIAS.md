# Preferências de Usuário - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 👤 Preferências de Usuário

### Obter Preferências (`GET /api/v1/users/me/preferences`)

**Descrição**: Obtém as preferências de privacidade e notificações do usuário autenticado.

**Como usar**:
- Requisição autenticada (token JWT obrigatório)
- Retorna preferências existentes ou cria preferências padrão se não existirem

**Regras de negócio**:
- Se o usuário não tiver preferências configuradas, retorna valores padrão:
  - `profileVisibility`: `Public`
  - `contactVisibility`: `ResidentsOnly`
  - `shareLocation`: `false`
  - `showMemberships`: `true`
  - Todas as notificações habilitadas por padrão

**Resposta**:
- **200 OK**: Preferências do usuário
- **401 Unauthorized**: Token inválido ou ausente

### Atualizar Preferências de Privacidade (`PUT /api/v1/users/me/preferences/privacy`)

**Descrição**: Atualiza as preferências de privacidade do usuário autenticado.

**Como usar**:
- Body: `profileVisibility` (Public, ResidentsOnly, Private), `contactVisibility` (Public, ResidentsOnly, Private), `shareLocation` (boolean), `showMemberships` (boolean)

**Regras de negócio**:
- **Permissão**: Apenas o próprio usuário pode atualizar suas preferências
- **Validação**: Valores devem ser válidos para cada campo
- **Efeito imediato**: Mudanças são aplicadas imediatamente

### Atualizar Preferências de Notificações (`PUT /api/v1/users/me/preferences/notifications`)

**Descrição**: Atualiza as preferências de notificações do usuário autenticado.

**Como usar**:
- Body: Objeto com flags booleanas para cada tipo de notificação

**Regras de negócio**:
- **Permissão**: Apenas o próprio usuário pode atualizar suas preferências
- **Tipos**: Post criado, report criado, inquiry recebido, join request, etc.
- **Efeito imediato**: Mudanças são aplicadas imediatamente

### Preferências de Mídia (`GET /api/v1/user/media-preferences` e `PUT /api/v1/user/media-preferences`)

**Descrição**: Controla como o usuário visualiza mídias (auto-play, tipos de mídia).

**Recursos**:
- Controlar auto-play de vídeos e áudios
- Escolher quais tipos de mídia visualizar (imagens, vídeos, áudios)

---

## 📚 Documentação Relacionada

- **[Mídias em Conteúdo](./60_15_API_MIDIAS.md)** - Preferências de mídia
- **[Notificações](./60_11_API_NOTIFICACOES.md)** - Preferências de notificações
- **[Chat](./60_10_API_CHAT.md)** - Preferências de privacidade afetam DM
- **[61_USER_PREFERENCES_PLAN.md](../61_USER_PREFERENCES_PLAN.md)** - Planejamento completo da funcionalidade

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
