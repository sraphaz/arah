# Territórios e Memberships - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](funcional/00_PLATAFORMA_ARAPONGA.md)

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Função de Negócio](#função-de-negócio)
3. [Elementos da Arquitetura](#elementos-da-arquitetura)
4. [Fluxos Funcionais](#fluxos-funcionais)
5. [Casos de Uso](#casos-de-uso)
6. [Regras de Negócio](#regras-de-negócio)

---

## 🎯 Visão Geral

Os domínios de **Territórios** e **Memberships** são o coração da plataforma Araponga. Eles definem como lugares físicos reais são representados digitalmente e como pessoas se relacionam com esses lugares.

### Território: Geográfico e Neutro

Um **Território** representa apenas um lugar físico real:
- Nome
- Localização (coordenadas geográficas)
- Fronteira geográfica
- **NÃO contém lógica social**

> O território existe antes do app e continua existindo mesmo sem usuários.

### Membership: Vínculo Social

Um **Membership** representa a relação entre uma pessoa e um território:
- Papel (Visitor ou Resident)
- Verificação de residência
- Permissões e capacidades
- Configurações e opt-ins

---

## 💼 Função de Negócio

### Para o Usuário

#### Como Visitante (Visitor)
- **Descobrir territórios** próximos à localização
- **Entrar como visitor** para explorar conteúdo público
- **Visualizar feed público** do território
- **Ver eventos públicos** e participar
- **Explorar mapa** com entidades públicas
- **Solicitar residência** para acesso ampliado

#### Como Morador (Resident)
- **Todas as funções de Visitor** +
- **Acessar conteúdo exclusivo** para moradores
- **Criar posts** com visibilidade RESIDENTS_ONLY
- **Participar de votações** comunitárias
- **Usar marketplace** (comprar e vender)
- **Acessar chat de moradores**
- **Criar lojas** no marketplace (após verificação)

### Para a Comunidade

- **Definir regras** de participação e visibilidade
- **Gerenciar membros** (aprovar/revogar residências)
- **Controlar acesso** através de feature flags
- **Garantir autenticidade** através de verificação de residência

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Territory (Território)
- **Propósito**: Representar lugar físico real de forma neutra
- **Atributos**:
  - `Id`: Identificador único
  - `Name`: Nome do território
  - `City`: Cidade
  - `State`: Estado
  - `Latitude`, `Longitude`: Coordenadas geográficas
  - `Boundary`: Fronteira geográfica (polígono)
  - `Tags`: Caracterização do território (definida por votações)

#### TerritoryMembership (Vínculo)
- **Propósito**: Relação entre User e Territory
- **Atributos**:
  - `UserId`: Usuário
  - `TerritoryId`: Território
  - `Role`: VISITOR ou RESIDENT
  - `ResidencyVerification`: NONE, GEOVERIFIED, DOCUMENTVERIFIED
  - `MembershipStatus`: Pending, Active, Suspended, Revoked
  - `CreatedAt`: Data de criação do vínculo

#### MembershipSettings (Configurações)
- **Propósito**: Configurações e opt-ins do membro
- **Atributos**:
  - `MarketplaceOptIn`: Opt-in para marketplace
  - Outras preferências territoriais

#### MembershipCapability (Capacidades)
- **Propósito**: Poderes operacionais territoriais
- **Tipos**: Curator, Moderator, EventOrganizer
- **Características**: Empilháveis (um membership pode ter múltiplas)

#### FeatureFlag (Flags de Funcionalidade)
- **Propósito**: Controlar funcionalidades por território
- **Exemplos**: MARKETPLACEENABLED, ALERTPOSTS, CHATENABLED

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Descoberta e Entrada como Visitor

```
┌─────────────┐
│   Usuário   │
│ Autenticado│
└──────┬──────┘
       │
       │ 1. Busca territórios próximos
       ▼
┌─────────────────────┐
│  GET /territories/  │
│  nearby?lat=&lng=   │
└──────┬──────────────┘
       │
       │ 2. Retorna lista de territórios
       ▼
┌─────────────────────┐
│  Seleciona         │
│  território        │
└──────┬──────────────┘
       │
       │ 3. Entra como Visitor
       ▼
┌─────────────────────┐
│  POST /territories/ │
│  {id}/enter         │
└──────┬──────────────┘
       │
       │ 4. Sistema cria Membership
       │    Role: VISITOR
       │    ResidencyVerification: NONE
       ▼
┌─────────────────────┐
│  Membership criado  │
│  Status: Active     │
└──────┬──────────────┘
       │
       │ 5. Usuário pode explorar
       │    conteúdo público
       ▼
┌─────────────────────┐
│  Feed Público       │
│  Mapa Público       │
│  Eventos Públicos   │
└─────────────────────┘
```

**Resultado**: Usuário vinculado como Visitor com acesso a conteúdo público

### Fluxo 2: Solicitação de Residência

```
┌─────────────┐
│   Usuário   │
│  (Visitor)  │
└──────┬──────┘
       │
       │ 1. Decide solicitar residência
       ▼
┌─────────────────────┐
│  POST /memberships/ │
│  {territoryId}/     │
│  become-resident    │
│  - message (opcional)│
│  - recipientUserIds │
│    (opcional)       │
└──────┬──────────────┘
       │
       │ 2. Sistema cria JoinRequest
       │
    ┌──┴──┐
    │Com  │  ──► 3a. Direcionado para
    │Dest.│       destinatários específicos
    └──┬──┘
       │ Sem destinatário
       ▼
┌─────────────────────┐
│  3b. Direcionado    │
│  para Curator       │
│  (ou SystemAdmin    │
│   se não houver)     │
└──────┬──────────────┘
       │
       │ 4. JoinRequest criado
       │    Status: Pending
       ▼
┌─────────────────────┐
│  Usuário aguarda    │
│  aprovação          │
└─────────────────────┘
```

**Resultado**: Solicitação de residência criada, aguardando aprovação

### Fluxo 3: Aprovação de Residência

```
┌─────────────┐
│   Curator   │
│  (ou Admin) │
└──────┬──────┘
       │
       │ 1. Recebe notificação de
       │    JoinRequest pendente
       ▼
┌─────────────────────┐
│  Revisa solicitação │
│  - Verifica dados    │
│  - Opcional: pede    │
│    documentos        │
└──────┬──────────────┘
       │
    ┌──┴──┐
    │Aprova│  ──► 2a. Aprova JoinRequest
    └──┬──┘
       │ Rejeita
       ▼
┌─────────────────────┐
│  2b. Rejeita        │
│  (com motivo)      │
└──────┬──────────────┘
       │
       │ 3. Sistema atualiza Membership
       │    Role: RESIDENT
       │    Status: Active
       ▼
┌─────────────────────┐
│  Usuário notificado │
│  da aprovação       │
└──────┬──────────────┘
       │
       │ 4. Usuário agora tem acesso
       │    ampliado (conteúdo exclusivo)
       ▼
┌─────────────────────┐
│  Acesso como        │
│  RESIDENT           │
└─────────────────────┘
```

**Resultado**: Usuário aprovado como Resident com acesso ampliado

### Fluxo 4: Verificação de Residência

```
┌─────────────┐
│   Usuário   │
│  (Resident) │
└──────┬──────┘
       │
       │ 1. Tenta acessar funcionalidade
       │    que requer verificação
       │    (ex: criar loja no marketplace)
       ▼
┌─────────────────────┐
│  Sistema verifica  │
│  ResidencyVerification│
└──────┬──────────────┘
       │
    ┌──┴──┐
    │NONE │  ──► 2a. Solicita verificação
    └──┬──┘
       │ GEOVERIFIED ou DOCUMENTVERIFIED
       ▼
┌─────────────────────┐
│  2b. Permite acesso │
└─────────────────────┘
       │
       │ (Se NONE)
       ▼
┌─────────────────────┐
│  Opção 1:           │
│  Verificação Geo    │
│  POST /verify-      │
│  residency/geo      │
│  (coordenadas dentro│
│   do raio)          │
└──────┬──────────────┘
       │
       │ 3a. Sistema marca
       │     GEOVERIFIED
       ▼
┌─────────────────────┐
│  Opção 2:           │
│  Verificação Doc    │
│  POST /verify-      │
│  residency/document │
│  (upload documentos)│
└──────┬──────────────┘
       │
       │ 3b. Sistema processa
       │     (manual ou automático)
       │     DOCUMENTVERIFIED
       ▼
┌─────────────────────┐
│  ResidencyVerification│
│  atualizado         │
└─────────────────────┘
```

**Resultado**: Usuário com residência verificada pode acessar funcionalidades sensíveis

### Fluxo 5: Seleção de Território Ativo

```
┌─────────────┐
│   Usuário   │
└──────┬──────┘
       │
       │ 1. Seleciona território
       │    para interagir
       ▼
┌─────────────────────┐
│  POST /territories/ │
│  selection           │
│  - territoryId       │
│  - X-Session-Id      │
└──────┬──────────────┘
       │
       │ 2. Sistema armazena seleção
       │    por sessão
       ▼
┌─────────────────────┐
│  Território ativo   │
│  definido para      │
│  sessão             │
└──────┬──────────────┘
       │
       │ 3. Operações subsequentes
       │    usam território ativo
       │    por padrão
       ▼
┌─────────────────────┐
│  Feed, Mapa,        │
│  Eventos, etc.      │
│  filtrados por      │
│  território ativo   │
└─────────────────────┘
```

**Resultado**: Território ativo definido para sessão, usado em operações subsequentes

---

## 📖 Casos de Uso

### Caso de Uso 1: Usuário Descobre e Entra em Território

**Ator**: Usuário autenticado  
**Pré-condições**: Usuário autenticado  
**Fluxo Principal**:
1. Usuário acessa tela de descoberta de territórios
2. Sistema busca territórios próximos à localização do usuário
3. Usuário visualiza lista de territórios
4. Usuário seleciona um território
5. Sistema cria Membership com Role=VISITOR
6. Usuário é redirecionado para feed do território

**Pós-condições**: Usuário vinculado como Visitor ao território

### Caso de Uso 2: Visitor Solicita Residência

**Ator**: Visitor  
**Pré-condições**: Usuário é Visitor no território  
**Fluxo Principal**:
1. Visitor acessa opção "Solicitar Residência"
2. (Opcional) Visitor informa mensagem e destinatários
3. Sistema cria JoinRequest
4. JoinRequest é direcionado para Curator (ou SystemAdmin)
5. Curator recebe notificação
6. Visitor aguarda aprovação

**Fluxo Alternativo**: Se já houver JoinRequest pendente, retorna a mesma solicitação

**Pós-condições**: JoinRequest criado com status Pending

### Caso de Uso 3: Curator Aprova Residência

**Ator**: Curator  
**Pré-condições**: JoinRequest pendente existe  
**Fluxo Principal**:
1. Curator recebe notificação de JoinRequest
2. Curator acessa work queue de aprovações
3. Curator revisa solicitação
4. Curator aprova JoinRequest
5. Sistema atualiza Membership: Role=RESIDENT, Status=Active
6. Usuário recebe notificação de aprovação
7. Usuário agora tem acesso ampliado

**Fluxo Alternativo**: Curator pode rejeitar com motivo

**Pós-condições**: Usuário é Resident no território

### Caso de Uso 4: Resident Verifica Residência

**Ator**: Resident  
**Pré-condições**: Usuário é Resident, ResidencyVerification=NONE  
**Fluxo Principal**:
1. Resident tenta acessar funcionalidade que requer verificação
2. Sistema bloqueia e solicita verificação
3. Resident escolhe método:
   - Verificação Geo: Sistema valida coordenadas dentro do raio
   - Verificação Doc: Resident faz upload de documentos
4. Sistema processa verificação
5. ResidencyVerification atualizado para GEOVERIFIED ou DOCUMENTVERIFIED
6. Resident pode acessar funcionalidade

**Pós-condições**: Resident com residência verificada

---

## ⚙️ Regras de Negócio

### Territórios

1. **Neutralidade**:
   - Território não contém lógica social
   - Existe independente de usuários
   - Representa apenas lugar físico

2. **Descoberta**:
   - Busca por proximidade geográfica
   - Busca por nome, cidade, estado
   - Listagem pública (não exige autenticação)

3. **Seleção**:
   - Um usuário pode ter múltiplas sessões
   - Cada sessão pode ter território ativo diferente
   - Território ativo usado por padrão em operações

### Memberships

1. **Papéis**:
   - **VISITOR**: Acesso limitado, conteúdo público
   - **RESIDENT**: Acesso ampliado, conteúdo exclusivo

2. **Regra de Residência Única**:
   - Um usuário pode ser Resident em apenas 1 território
   - Para mudar, deve transferir residência

3. **Verificação de Residência**:
   - **NONE**: Sem verificação
   - **GEOVERIFIED**: Verificado por geolocalização
   - **DOCUMENTVERIFIED**: Verificado por documentos
   - Permite acumulação (GEO + DOC)

4. **Status**:
   - **Pending**: Aguardando aprovação
   - **Active**: Ativo
   - **Suspended**: Suspenso temporariamente
   - **Revoked**: Revogado permanentemente

### Solicitação de Residência

1. **Destinatários**:
   - Se `recipientUserIds` informado: direcionado para esses usuários
   - Se não informado: direcionado para Curator
   - Se não houver Curator: fallback para SystemAdmin

2. **Anti-abuso**:
   - Máximo 3 destinatários por solicitação
   - Máximo 3 criações (create+cancel+recreate) por usuário/território em 24h
   - Rate limit: 429 Too Many Requests se excedido

3. **Idempotência**:
   - Se já houver JoinRequest pendente, retorna a mesma

### Capacidades

1. **Tipos**:
   - **Curator**: Curadoria de conteúdo e aprovação de residências
   - **Moderator**: Moderação de conteúdo e usuários
   - **EventOrganizer**: Organização de eventos

2. **Empilháveis**:
   - Um Membership pode ter múltiplas capacidades
   - Cada capacidade atua apenas no território do Membership

3. **Auditoria**:
   - Concessão/revogação com auditoria completa
   - Registro de quem concedeu/revogou e motivo

### Feature Flags

1. **Controle Territorial**:
   - Cada território pode habilitar/desabilitar funcionalidades
   - Exemplos: MARKETPLACEENABLED, ALERTPOSTS, CHATENABLED

2. **Impacto**:
   - Quando desabilitado, endpoints retornam 404
   - Evita exposição de funcionalidades desabilitadas

---

## 🔗 Integrações

### Com Outros Domínios

- **Feed**: Filtrado por território e visibilidade baseada em Membership
- **Eventos**: Vinculados a território
- **Mapa**: Entidades territoriais
- **Marketplace**: Requer Membership com verificação
- **Chat**: Canais territoriais baseados em Membership
- **Governança**: Votações territoriais baseadas em Membership

---

## 📊 Métricas e Observabilidade

### Métricas Importantes

- Territórios criados (total, por período)
- Memberships criados (Visitors, Residents)
- Taxa de conversão Visitor → Resident
- Tempo médio de aprovação de residência
- Taxa de verificação de residência (Geo vs Doc)

### Logs Estruturados

- Criação de territórios
- Criação de Memberships
- Mudanças de Role (Visitor → Resident)
- Aprovações/rejeições de residência
- Verificações de residência

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](funcional/00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Autenticação e Identidade](funcional/01_AUTENTICACAO_IDENTIDADE.md)** - Pré-requisito
- **[Feed Comunitário](funcional/03_FEED_COMUNITARIO.md)** - Conteúdo territorial
- **[Governança e Votação](funcional/13_GOVERNANCA_VOTACAO.md)** - Aprovação de residências
- **[API - Territórios](api/60_02_API_TERRITORIOS.md)** - Documentação técnica
- **[API - Memberships](api/60_03_API_MEMBERSHIPS.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
