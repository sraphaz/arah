# Modularização - Arquitetura Modular do Araponga

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: 📋 Documentação Técnica  
**Tipo**: Documentação Técnica - Arquitetura Modular

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Princípios de Modularização](#princípios-de-modularização)
3. [Arquitetura Modular](#arquitetura-modular)
4. [Módulos do Sistema](#módulos-do-sistema)
5. [Organização por Domínios](#organização-por-domínios)
6. [Feature Flags e Configuração](#feature-flags-e-configuração)
7. [Dependências entre Módulos](#dependências-entre-módulos)
8. [Extensibilidade](#extensibilidade)
9. [Boas Práticas](#boas-práticas)

---

## 🎯 Visão Geral

O Araponga utiliza uma **arquitetura modular** baseada em **Clean Architecture** e **Domain-Driven Design (DDD)**, onde cada módulo representa um domínio funcional específico com responsabilidades claras e bem definidas.

### Características da Modularização

- ✅ **Separação por Domínios**: Cada módulo representa um domínio funcional
- ✅ **Independência Relativa**: Módulos podem ser habilitados/desabilitados
- ✅ **Feature Flags**: Controle granular de funcionalidades por território
- ✅ **Clean Architecture**: Camadas bem definidas (Domain, Application, Infrastructure, API)
- ✅ **Extensibilidade**: Fácil adicionar novos módulos sem impactar existentes

### Isolamento: infraestrutura independente (não microserviços)

**Isolamento real** (deploy, rede, falhas independentes) só existe com **serviços separados** (microserviços). Para o tamanho e estágio do projeto, o Araponga **não** adota microserviços.

O objetivo adotado é ter **infraestrutura independente por módulo**, isolando **pontos de manutenção e de falha** dentro do mesmo processo:

- **Manutenção**: alterações em Feed (schema, repositório, bugs) ficam contidas no projeto `Araponga.Modules.Feed.Infrastructure`; o mesmo para Events, Map, Chat, Marketplace, etc. Menos risco de regressão em outros domínios e ownership claro por módulo.
- **Falha**: um bug ou problema de persistência em um módulo fica limitado ao código e ao DbContext daquele módulo; o restante da aplicação continua referenciando apenas interfaces da Application.
- **Evolução**: se no futuro um módulo precisar virar serviço separado, a fronteira já está desenhada (projeto de infra + contratos de aplicação).

Ou seja: **uma infraestrutura modular**, não vários serviços, com isolamento de responsabilidade e de impacto por domínio.

---

## 🏗️ Princípios de Modularização

### 1. Domínio como Unidade de Modularização

Cada módulo representa um **domínio funcional** completo:
- Autenticação e Identidade
- Territórios
- Feed Comunitário
- Eventos
- Marketplace
- Chat
- etc.

### 2. Clean Architecture por Módulo

Cada módulo segue Clean Architecture com camadas:

```
Módulo/
├── Domain/           # Entidades, Value Objects, Interfaces
├── Application/     # Services, Use Cases, DTOs
├── Infrastructure/   # Repositories, External Services
└── Api/             # Controllers, Endpoints
```

### 3. Feature Flags por Módulo

Módulos podem ser habilitados/desabilitados via feature flags:
- Controle por território
- Rollout gradual
- A/B testing
- Desativação de funcionalidades

### 4. Dependências Explícitas

- Módulos core são sempre habilitados
- Módulos opcionais podem depender de outros módulos
- Dependências são validadas na inicialização

---

## 🧩 Arquitetura Modular

### Estrutura de Camadas

```
Araponga.Api (API Layer)
    ↓
Araponga.Application (Application Layer)
    ├── Módulo 1 (Domain Services)
    ├── Módulo 2 (Domain Services)
    └── Módulo N (Domain Services)
    ↓
Araponga.Domain (Domain Layer)
    ├── Módulo 1 (Entities, Value Objects)
    ├── Módulo 2 (Entities, Value Objects)
    └── Módulo N (Entities, Value Objects)
    ↓
Araponga.Infrastructure (Infrastructure Layer)
    ├── Módulo 1 (Repositories, External Services)
    ├── Módulo 2 (Repositories, External Services)
    └── Módulo N (Repositories, External Services)
```

### Organização Física

```
backend/
├── Araponga.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs          # Módulo: Autenticação
│   │   ├── TerritoriesController.cs   # Módulo: Territórios
│   │   ├── FeedController.cs          # Módulo: Feed
│   │   ├── EventsController.cs        # Módulo: Eventos
│   │   ├── MarketplaceController.cs  # Módulo: Marketplace
│   │   └── ChatController.cs          # Módulo: Chat
│   └── ...
├── Araponga.Application/
│   ├── Services/
│   │   ├── Auth/                      # Módulo: Autenticação
│   │   ├── Territories/               # Módulo: Territórios
│   │   ├── Feed/                      # Módulo: Feed
│   │   ├── Events/                    # Módulo: Eventos
│   │   ├── Marketplace/               # Módulo: Marketplace
│   │   └── Chat/                      # Módulo: Chat
│   └── ...
├── Araponga.Domain/
│   ├── Users/                         # Módulo: Autenticação
│   ├── Territories/                   # Módulo: Territórios
│   ├── Feed/                          # Módulo: Feed
│   ├── Events/                        # Módulo: Eventos
│   ├── Marketplace/                   # Módulo: Marketplace
│   └── Chat/                          # Módulo: Chat
└── Araponga.Infrastructure/
    ├── Postgres/
    │   ├── Entities/                  # Mapeamento por módulo
    │   └── Repositories/              # Repositories por módulo
    └── ...
```

---

## 📦 Módulos do Sistema

### Módulos Core (Sempre Habilitados)

#### 1. Autenticação e Identidade
- **Responsabilidade**: Gerenciar identidade única do usuário, autenticação e verificação
- **Elementos**: User, AuthProvider, UserIdentityVerificationStatus, 2FA
- **Feature Flags**: Nenhum (sempre habilitado)

#### 2. Territórios
- **Responsabilidade**: Representar lugares físicos reais de forma neutra
- **Elementos**: Territory, GeoAnchor, fronteiras geográficas
- **Feature Flags**: Nenhum (sempre habilitado)

#### 3. Memberships
- **Responsabilidade**: Gerenciar relação entre usuários e territórios
- **Elementos**: TerritoryMembership, MembershipRole, MembershipCapability
- **Feature Flags**: Nenhum (sempre habilitado)

### Módulos de Conteúdo

#### 4. Feed Comunitário
- **Responsabilidade**: Publicações e timeline territorial
- **Elementos**: Post, PostGeoAnchor, Media
- **Feature Flags**: 
  - `AlertPosts` - Permitir posts do tipo ALERT
  - `EventPosts` - Permitir posts de eventos
  - `MediaImagesEnabled` - Habilitar imagens
  - `MediaVideosEnabled` - Habilitar vídeos
  - `MediaAudioEnabled` - Habilitar áudios

#### 5. Eventos
- **Responsabilidade**: Organizar eventos comunitários por território
- **Elementos**: Event, participação, georreferenciamento
- **Feature Flags**: 
  - `EventPosts` - Habilitar eventos no feed
  - `MediaImagesEnabled` - Habilitar imagens em eventos
  - `MediaVideosEnabled` - Habilitar vídeos em eventos
  - `MediaAudioEnabled` - Habilitar áudios em eventos

#### 6. Mapa Territorial
- **Responsabilidade**: Visualização geográfica de conteúdos
- **Elementos**: MapEntity, MapEntityRelation
- **Feature Flags**: Nenhum específico (usa flags de conteúdo)

#### 7. Alertas
- **Responsabilidade**: Alertas de saúde pública e comunicação emergencial
- **Elementos**: Alert, notificações prioritárias
- **Feature Flags**: 
  - `AlertPosts` - Habilitar posts do tipo ALERT

#### 8. Assets
- **Responsabilidade**: Recursos compartilhados do território
- **Elementos**: Asset, geolocalização obrigatória
- **Feature Flags**: Nenhum específico

### Módulos de Comunicação e Economia

#### 9. Chat
- **Responsabilidade**: Comunicação territorial (canais, grupos, DM)
- **Elementos**: ChatConversation, ChatMessage, ConversationParticipant
- **Feature Flags**: 
  - `ChatEnabled` - Master switch do chat
  - `ChatTerritoryPublicChannel` - Canal público
  - `ChatTerritoryResidentsChannel` - Canal de moradores
  - `ChatGroups` - Grupos no chat
  - `ChatDmEnabled` - Mensagens diretas
  - `ChatMediaEnabled` - Mídias no chat
  - `ChatMediaImagesEnabled` - Imagens no chat
  - `ChatMediaAudioEnabled` - Áudios no chat

#### 10. Marketplace
- **Responsabilidade**: Sistema de trocas locais integrado ao território
- **Elementos**: Store, StoreItem, Cart, Checkout
- **Feature Flags**: 
  - `MarketplaceEnabled` - Habilitar marketplace
  - `MediaImagesEnabled` - Habilitar imagens em itens
  - `MediaVideosEnabled` - Habilitar vídeos em itens
  - `MediaAudioEnabled` - Habilitar áudios em itens

#### 11. Subscriptions
- **Responsabilidade**: Sistema de assinaturas recorrentes
- **Elementos**: Subscription, Plan, pagamentos recorrentes
- **Feature Flags**: Nenhum específico (habilitado se módulo ativo)

### Módulos de Governança

#### 12. Moderação
- **Responsabilidade**: Manter qualidade e segurança do conteúdo
- **Elementos**: Report, Sanction, WorkItem
- **Feature Flags**: Nenhum específico (habilitado se módulo ativo)

#### 13. Governança e Votação
- **Responsabilidade**: Decisões coletivas e governança participativa
- **Elementos**: Vote, Proposal
- **Feature Flags**: Nenhum específico (habilitado se módulo ativo)

#### 14. Notificações
- **Responsabilidade**: Sistema confiável de notificações in-app
- **Elementos**: OutboxMessage, UserNotification
- **Feature Flags**: Nenhum específico (habilitado se módulo ativo)

### Módulos Administrativos

#### 15. Admin e Configuração
- **Responsabilidade**: Administração do sistema e configurações globais
- **Elementos**: SystemConfig, SystemPermission, WorkQueue
- **Feature Flags**: Nenhum específico (habilitado se módulo ativo)

#### 16. Conexões (Círculo de Amigos)
- **Responsabilidade**: Conexões entre usuários (círculo de amigos), priorização no feed e notificações de solicitação/aceitação
- **Elementos**: UserConnection, ConnectionPrivacySettings, ConnectionStatus, ConnectionRequestPolicy, ConnectionVisibility
- **Feature Flags**: 
  - `ConnectionsEnabled` (15) — habilitar conexões no território
  - `ConnectionsFeedPrioritize` (16) — priorizar posts de conexões no feed

---

## 🗺️ Organização por Domínios

### Princípio: Domínio como Unidade

Cada módulo representa um **domínio funcional** completo:

1. **Domínio = Módulo**: Um domínio funcional = um módulo
2. **Responsabilidade Única**: Cada módulo tem uma responsabilidade clara
3. **Independência Relativa**: Módulos podem funcionar independentemente (com dependências explícitas)
4. **Feature Flags**: Controle granular de funcionalidades

### Mapeamento Domínio → Módulo

| Domínio Funcional | Módulo | Status |
|-------------------|--------|--------|
| Autenticação e Identidade | Auth | Core |
| Territórios | Territories | Core |
| Memberships | Memberships | Core |
| Feed Comunitário | Feed | Opcional |
| Eventos | Events | Opcional |
| Mapa Territorial | Map | Opcional |
| Marketplace | Marketplace | Opcional |
| Chat | Chat | Opcional |
| Alertas | Alerts | Opcional |
| Assets | Assets | Opcional |
| Moderação | Moderation | Opcional |
| Notificações | Notifications | Opcional |
| Subscriptions | Subscriptions | Opcional |
| Governança | Governance | Opcional |
| Admin | Admin | Opcional |
| Conexões (Círculo de Amigos) | Connections | Opcional |

---

## ⚙️ Feature Flags e Configuração

### Sistema de Feature Flags

Feature flags permitem controle granular de funcionalidades:

1. **Por Território**: Cada território pode ter flags diferentes
2. **Por Módulo**: Flags específicas de cada módulo
3. **Hierarquia**: Flags podem depender de outras flags
4. **Validação**: Dependências são validadas automaticamente

### Exemplo de Feature Flags

```csharp
public enum FeatureFlag
{
    // Feed
    AlertPosts = 1,
    EventPosts = 2,
    
    // Marketplace
    MarketplaceEnabled = 3,
    
    // Chat
    ChatEnabled = 4,
    ChatTerritoryPublicChannel = 5,
    ChatTerritoryResidentsChannel = 6,
    ChatGroups = 7,
    ChatDmEnabled = 8,
    ChatMediaEnabled = 9,
    
    // Mídias
    MediaImagesEnabled = 10,
    MediaVideosEnabled = 11,
    MediaAudioEnabled = 12,
    ChatMediaImagesEnabled = 13,
    ChatMediaAudioEnabled = 14,
    // Conexões (Círculo de Amigos)
    ConnectionsEnabled = 15,
    ConnectionsFeedPrioritize = 16
}
```

### Validação de Dependências

```csharp
// ChatMediaEnabled requer ChatEnabled
if (flag == FeatureFlag.ChatMediaEnabled && 
    !IsEnabled(territoryId, FeatureFlag.ChatEnabled))
{
    throw new InvalidOperationException(
        "ChatMediaEnabled requires ChatEnabled");
}
```

---

## 🔗 Dependências entre Módulos

### Módulos Core

**Sempre habilitados**, sem dependências:
- Autenticação e Identidade
- Territórios
- Memberships

### Dependências Explícitas

| Módulo | Depende de | Tipo |
|--------|------------|------|
| Feed | Territórios, Memberships | Obrigatória |
| Eventos | Territórios, Memberships | Obrigatória |
| Mapa | Territórios, Feed/Eventos | Obrigatória |
| Marketplace | Territórios, Memberships | Obrigatória |
| Chat | Territórios, Memberships, Notificações | Obrigatória |
| Alertas | Territórios, Feed | Obrigatória |
| Assets | Territórios, Memberships | Obrigatória |
| Moderação | Territórios, Memberships | Obrigatória |
| Governança | Territórios, Memberships | Obrigatória |
| Notificações | Autenticação | Obrigatória |
| Subscriptions | Autenticação, Territórios | Obrigatória |
| Admin | Autenticação | Obrigatória |
| Connections (Círculo de Amigos) | Autenticação, Memberships, Notificações | Obrigatória |

### Validação de Dependências

O sistema valida dependências na inicialização:

```csharp
public void ValidateModuleDependencies(
    IEnumerable<string> enabledModules)
{
    var requiredModules = new Dictionary<string, string[]>
    {
        { "Chat", new[] { "Territories", "Memberships", "Notifications" } },
        { "Marketplace", new[] { "Territories", "Memberships" } },
        // ...
    };
    
    foreach (var module in enabledModules)
    {
        if (requiredModules.TryGetValue(module, out var deps))
        {
            foreach (var dep in deps)
            {
                if (!enabledModules.Contains(dep))
                {
                    throw new InvalidOperationException(
                        $"{module} requires {dep}");
                }
            }
        }
    }
}
```

---

## 🔌 Extensibilidade

### Adicionar Novo Módulo

1. **Criar Estrutura de Diretórios**:
   ```
   backend/
   ├── Araponga.Domain/NewModule/
   ├── Araponga.Application/Services/NewModule/
   ├── Araponga.Infrastructure/Postgres/Entities/NewModule/
   └── Araponga.Api/Controllers/NewModuleController.cs
   ```

2. **Definir Feature Flags** (se necessário):
   ```csharp
   public enum FeatureFlag
   {
       // ... flags existentes
       NewModuleEnabled = 15
   }
   ```

3. **Registrar no DI Container**:
   ```csharp
   services.AddScoped<INewModuleService, NewModuleService>();
   ```

4. **Adicionar ao Instalador**:
   - Adicionar módulo à lista de seleção
   - Configurar feature flags relacionadas
   - Adicionar validações de dependências

### Boas Práticas para Novos Módulos

- ✅ Seguir Clean Architecture
- ✅ Definir responsabilidade única e clara
- ✅ Usar feature flags quando apropriado
- ✅ Documentar dependências
- ✅ Adicionar testes unitários e de integração
- ✅ Seguir padrões de nomenclatura existentes

---

## ✅ Boas Práticas

### 1. Separação de Responsabilidades

- Cada módulo tem uma responsabilidade única e clara
- Módulos não devem conhecer detalhes internos de outros módulos
- Comunicação entre módulos via interfaces bem definidas

### 2. Feature Flags

- Use feature flags para funcionalidades que podem ser desabilitadas
- Valide dependências entre flags
- Documente flags e suas dependências

### 3. Dependências

- Torne dependências explícitas
- Valide dependências na inicialização
- Documente dependências entre módulos

### 4. Testes

- Teste cada módulo independentemente
- Teste integração entre módulos
- Teste feature flags e suas dependências

### 5. Documentação

- Documente responsabilidade de cada módulo
- Documente feature flags e dependências
- Documente APIs e contratos

---

## 📚 Referências

- [Clean Architecture](../.cursorrules) - Princípios de Clean Architecture
- [Domain-Driven Design](./12_DOMAIN_MODEL.md) - Modelo de domínio
- [Feature Flags](./api/60_16_API_FEATURE_FLAGS.md) - Sistema de feature flags
- [Arquitetura de Services](./11_ARCHITECTURE_SERVICES.md) - Organização de services
- [Plataforma Araponga](./funcional/00_PLATAFORMA_ARAPONGA.md) - Visão geral dos domínios

---

**Última atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: 📋 Documentação Técnica
