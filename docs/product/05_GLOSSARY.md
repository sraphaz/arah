# Glossário de Termos e Conceitos

**Versão**: 2.0  
**Data**: 2025-01-20  
**Aplicação**: Documentação, código, comunicação e materiais do projeto Arah

---

## Terminologia Fundamental

### Território e Geografia

- **Territory (Território)**: unidade central do sistema; lugar físico real e neutro com fronteiras geográficas definidas. Existe antes do app e continua existindo sem usuários. Organiza contexto e governança, não é um filtro.

- **GeoAnchor / PostGeoAnchor**: ponto georreferenciado (latitude/longitude) que ancora conteúdo ao território. Permite visualização espacial e integração com mapa.

- **Presença no Território**: requisito fundamental para vínculo no MVP; associação não ocorre remotamente. Geolocalização do usuário valida proximidade física ao território.

- **Consentimento de localização**: autorização explícita do usuário para uso de geolocalização no território. Fundamenta privacidade e transparência.

### Vínculos e Papéis

- **Visitor (Visitante)**: usuário presente no território, com vínculo básico no MVP. Acesso a conteúdo público, participação em canais públicos.

- **Resident (Morador)**: usuário com vínculo aprovado no território e acesso a conteúdo restrito. Requer verificação de residência (geográfica e/ou documental).

- **Membership (Vínculo)**: relação User ↔ Territory. Representa presença e participação, não hierarquia social. Caracterizada por MembershipRole (Visitor/Resident) e MembershipStatus (Pending, Active, Suspended, Revoked).

- **Friends (Círculo interno)**: relação de confiança pós-MVP (não hierárquica) com conteúdos exclusivos. Baseada em conexões pessoais, não em papéis territoriais.

### Conteúdo e Feed

- **Post**: publicação de conteúdo no território. Pode ter visibilidade Public ou Restricted. Suporta 0..N GeoAnchors para georreferenciamento.

- **Feed**: linha do tempo de posts (pessoal ou do território). Organização temporal de conteúdo, integrada ao mapa via GeoAnchors.

- **Mapa**: visualização espacial dos posts via GeoAnchor. Integrado ao feed com sincronia temporal (pós-MVP).

- **Sincronia Feed-Mapa**: relação temporal entre a timeline de posts e o pin correspondente no mapa. Melhora experiência de navegação e contexto espacial.

- **Public/Restricted (Visibilidade)**: regras de acesso a posts conforme papel (visitor/resident) e, no pós-MVP, friends. Define quem pode visualizar conteúdo.

- **Items (Itens)**: produtos e serviços do marketplace. **NUNCA use "listings"** — use sempre "items" ou "itens".

### Moderação e Governança

- **Report**: denúncia de post ou usuário com motivo e detalhes. Inicia processo de moderação com rastreabilidade.

- **Sanction (Sanção)**: ação disciplinar aplicada a um usuário, territorial ou global. Pode ser temporária ou permanente, com justificativa documentada.

- **Curator (Curador)**: capability territorial (`MembershipCapability`) para curadoria/validação e decisões territoriais (ex.: residência, assets). Reconhece e valoriza inteligência comunitária.

- **Moderator (Moderador)**: capability territorial (`MembershipCapability`) para decisões de moderação (casos de reports, sanções, etc.). Respeita autonomia territorial.

- **Work Queue / WorkItem**: fila genérica de "trabalhos" que exigem revisão/decisão humana (verificações, curadoria, moderação), com status e outcome. Transparência em processos.

- **Outcome**: resultado de um WorkItem (Approved/Rejected/NoAction). Decisão documentada e auditável.

### Permissões e Administração

- **SystemAdmin (Admin global)**: permissão global (`SystemPermission`) que permite operar configurações do sistema e decisões globais (ex.: verificação de identidade). Não substitui autonomia territorial.

- **SystemConfig**: configuração global calibrável (chave/valor/categoria/descrição), auditável, usada para parametrizar o comportamento do sistema. Transparência em configurações.

### Comunicação

- **Chat**: módulo de comunicação entre usuários, com governança territorial e regras explícitas de acesso. Respeita autonomia territorial e preferências de privacidade.

- **Canal (Territory channel)**: conversa "implícita" do território (ex.: canal público e canal de moradores). Acesso deriva do `territoryId` + `MembershipRole` + regras de verificação.

- **Grupo (Group)**: conversa privada (invite-only) dentro de um território. Criado por morador validado e usuário verificado; habilitação/aprovação por curadoria.

- **DM / Direct Message**: conversa privada entre usuários. Respeita preferências e bloqueios; pode ser habilitada por feature flag do território.

- **ConversationStats**: resumo por conversa (última mensagem/preview/contagem) usado para performance (evita agregações caras em listagens/inbox).

### Autenticação e Segurança

- **Cadastro/Autenticação**: requisito do MVP para consultar feed e mapa. Garante rastreabilidade e governança.

- **DocumentEvidence (Evidência documental)**: metadados de um arquivo enviado (kind, contentType, size, hash, storageKey); o arquivo em si fica no storage. Base para verificação transparente.

### Armazenamento e Mídia

- **Download por proxy**: padrão em que a API faz o streaming do arquivo do storage para o cliente, aplicando autorização e auditoria (sem expor URL pública/pré-assinada). Segurança e controle.

- **S3/MinIO**: storage de objetos compatível com S3; usado para armazenar arquivos/evidências fora do banco, com acesso controlado pela API.

### Marketplace

- **Store (Loja)**: espaço comercial dentro de um território. Gerida por usuário (owner), com habilitadores de pagamento e status.

- **StoreItem (Item da Loja)**: produto ou serviço oferecido na loja. Type, title, pricingType, status. **NUNCA use "listing"**.

- **Cart (Carrinho)**: conjunto de items selecionados para compra. Associado ao usuário e território.

- **Checkout**: processo de finalização de compra. Status e informações de pagamento.

- **ItemInquiry (Consulta sobre Item)**: mensagem de interesse sobre um item. Comunicação pré-compra respeitando governança territorial.

### Assets Territoriais

- **MapEntity (Entidade do Mapa)**: recurso georreferenciado no território. Name, category, lat/lng, status, visibility. Representa pontos de interesse comunitário.

- **MapEntityRelation**: vínculo de usuário com entidade do mapa. Moradores se conectam a recursos territoriais.

### Notificações

- **OutboxMessage**: mensagem de evento aguardando processamento. Garante confiabilidade em notificações.

- **UserNotification**: notificação no inbox do usuário. Title, body, kind, dataJson, createdAt, readAt. Respeita preferências do usuário.

---

## Terminologia Evitada

### ❌ Não Use

- **"Place" ou "Location"** → Use **"Territory"** ou **"Território"**
- **"Listings"** → Use **"Items"** ou **"Itens"**
- **"24 fases"** ou valores hardcoded → Use **"número total de fases"** calculado dinamicamente (ver `docs/PROJECT_PHASES_CONFIG.md`)
- **"User" ou "Member" genérico** → Prefira **"Visitor"** ou **"Resident"** conforme contexto

### ✅ Use Sempre

- **Territory** (não "place" ou "location")
- **Items** (não "listings")
- **Membership** (para vínculo usuário-território)
- **Visitor/Resident** (para papéis, não "user" genérico)
- **Número total de fases** calculado dinamicamente (ver `docs/PROJECT_PHASES_CONFIG.md`) - NUNCA hardcode valores fixos

---

## Princípios de Nomenclatura

1. **Território como Referência**: Termos relacionados a território são prioritários e neutros.

2. **Transparência**: Nomes explícitos sobre função e propósito.

3. **Autonomia**: Termos que respeitam escolhas locais e governança comunitária.

4. **Respeito à Inteligência**: Termos que valorizam conhecimento e contribuições (ex.: Curator, não "moderador" genérico).

---

## Referências

- [Modelo de Domínio](./12_DOMAIN_MODEL.md) - Estrutura completa de entidades e relacionamentos
- [Design System - Identidade Visual](./DESIGN_SYSTEM_IDENTIDADE_VISUAL.md) - Conjunto semântico preferencial
- [Visão do Produto](./01_PRODUCT_VISION.md) - Princípios e valores fundamentais
