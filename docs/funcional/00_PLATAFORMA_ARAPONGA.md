# Araponga - Documentação Funcional da Plataforma

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Documentação Funcional Completa  
**Tipo**: Visão Geral da Plataforma

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Motivação e Propósito](#motivação-e-propósito)
3. [Domínios e Arquitetura Funcional](#domínios-e-arquitetura-funcional)
4. [Funções de Negócio](#funções-de-negócio)
5. [Como os Elementos se Refletem em Funções](#como-os-elementos-se-refletem-em-funções)
6. [Navegação da Documentação](#navegação-da-documentação)

---

## 🎯 Visão Geral

**Araponga** é uma plataforma digital comunitária orientada ao território. Não é uma rede social genérica, mas sim uma **extensão digital do território vivo** - tecnologia que serve à vida local, à convivência e à autonomia das comunidades.

### Princípios Fundamentais

1. **Território é geográfico e neutro**
   - Representa apenas um lugar físico real (nome, localização, fronteira)
   - Não contém lógica social
   - Existe antes do app e continua existindo sem usuários

2. **Vida social acontece em camadas separadas**
   - Relações humanas (moradores, visitantes, visibilidade, moderação) não pertencem ao território
   - Pertencem a camadas sociais que referenciam o território
   - Torna o sistema mais claro, justo e adaptável

3. **Tecnologia a serviço do território**
   - Não é marketplace agressivo
   - Não é rede de engajamento infinito
   - Não é produto de vigilância
   - É infraestrutura digital comunitária para autonomia local, cuidado coletivo e continuidade da vida no território

---

## 🌱 Motivação e Propósito

### O Problema

Plataformas digitais atuais:
- Capturam atenção de forma predatória
- Desorganizam comunidades
- Desconectam pessoas do lugar onde vivem
- Extraem dados para publicidade
- Usam algoritmos de manipulação
- Criam feed global infinito sem contexto territorial

### A Solução Araponga

O Araponga nasce como **contraponto consciente** a esse modelo, oferecendo:

- **Território como referência**: O lugar físico é a unidade central
- **Comunidade como prioridade**: Organização local respeitando especificidades
- **Tecnologia como ferramenta**: Não como fim em si mesma

### Valores Fundamentais

- **Autonomia local**: Comunidades decidem suas próprias regras
- **Cuidado coletivo**: Fortalecimento de redes locais
- **Continuidade da vida no território**: Preservação e fortalecimento do vínculo entre pessoas e lugar
- **Transparência**: Decisões auditáveis, governança participativa
- **Respeito à privacidade**: Sem extração predatória de dados

---

## 🏗️ Domínios e Arquitetura Funcional

A plataforma Araponga é organizada em **domínios funcionais** que trabalham de forma integrada. Cada domínio possui responsabilidades claras e se relaciona com os demais para garantir uma experiência completa orientada ao território.

### Mapa de Domínios

```mermaid
flowchart TB
    subgraph PLATFORM["ARAPONGA PLATFORM"]
        direction TB
        
        subgraph LAYER1["Camada Base"]
            direction LR
            AUTH["Autenticação<br/>e Identidade"]
            TERR["Territórios"]
            MEMB["Memberships"]
        end
        
        subgraph CONTENT["Conteúdo e Interação"]
            direction LR
            FEED["Feed"]
            EVENTS["Eventos"]
            MAP["Mapa"]
        end
        
        subgraph LAYER2["Economia e Comunicação"]
            direction LR
            MARKET["Marketplace"]
            CHAT["Chat"]
            ALERTS["Alertas"]
        end
        
        subgraph LAYER3["Recursos e Gestão"]
            direction LR
            ASSETS["Assets"]
            MOD["Moderação"]
            NOTIF["Notificações"]
        end
        
        subgraph LAYER4["Sustentabilidade e Governança"]
            direction LR
            SUBS["Subscriptions"]
            GOV["Governança"]
            ADMIN["Admin"]
        end
        
        LAYER1 --> CONTENT
        LAYER1 --> LAYER2
        LAYER1 --> LAYER3
        LAYER1 --> LAYER4
    end
    
    style PLATFORM fill:#1a1a1a,stroke:#4dd4a8,stroke-width:3px,color:#fff
    style LAYER1 fill:#2a2a2a,stroke:#7dd3ff,stroke-width:2px,color:#fff
    style CONTENT fill:#2a2a2a,stroke:#7dd3ff,stroke-width:2px,color:#fff
    style LAYER2 fill:#2a2a2a,stroke:#7dd3ff,stroke-width:2px,color:#fff
    style LAYER3 fill:#2a2a2a,stroke:#7dd3ff,stroke-width:2px,color:#fff
    style LAYER4 fill:#2a2a2a,stroke:#7dd3ff,stroke-width:2px,color:#fff
    style AUTH fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style TERR fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style MEMB fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style FEED fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style EVENTS fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style MAP fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style MARKET fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style CHAT fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style ALERTS fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style ASSETS fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style MOD fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style NOTIF fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style SUBS fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style GOV fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
    style ADMIN fill:#3a3a3a,stroke:#4dd4a8,stroke-width:1px,color:#fff
```

### Domínios Principais

#### 1. **Autenticação e Identidade**
- **Responsabilidade**: Gerenciar identidade única do usuário, autenticação e verificação
- **Função de Negócio**: Garantir que cada pessoa tenha uma identidade verificada e segura
- **Elementos Técnicos**: User, AuthProvider, UserIdentityVerificationStatus, 2FA
- **Documentação**: [Autenticação e Identidade](funcional/01_AUTENTICACAO_IDENTIDADE.md)

#### 2. **Territórios**
- **Responsabilidade**: Representar lugares físicos reais de forma neutra
- **Função de Negócio**: Criar e gerenciar territórios geográficos como unidades centrais
- **Elementos Técnicos**: Territory, GeoAnchor, fronteiras geográficas
- **Documentação**: [Territórios e Memberships](funcional/02_TERRITORIOS_MEMBERSHIPS.md)

#### 3. **Memberships (Vínculos)**
- **Responsabilidade**: Gerenciar relação entre usuários e territórios
- **Função de Negócio**: Definir papéis (Visitor/Resident) e permissões territoriais
- **Elementos Técnicos**: TerritoryMembership, MembershipRole, MembershipCapability, MembershipSettings
- **Documentação**: [Territórios e Memberships](funcional/02_TERRITORIOS_MEMBERSHIPS.md)

#### 4. **Feed Comunitário**
- **Responsabilidade**: Publicações e timeline territorial
- **Função de Negócio**: Compartilhar informações relevantes ao território
- **Elementos Técnicos**: Post, PostGeoAnchor, Media (imagens, vídeos, áudios), visibilidade
- **Documentação**: [Feed Comunitário](funcional/03_FEED_COMUNITARIO.md)

#### 5. **Eventos**
- **Responsabilidade**: Organizar eventos comunitários por território
- **Função de Negócio**: Facilitar encontros e atividades locais
- **Elementos Técnicos**: Event, participação, georreferenciamento, mídias (capa + adicionais)
- **Documentação**: [Eventos](funcional/04_EVENTOS.md)

#### 6. **Mapa Territorial**
- **Responsabilidade**: Visualização geográfica de conteúdos
- **Função de Negócio**: Explorar publicações e eventos espacialmente
- **Elementos Técnicos**: MapEntity, MapEntityRelation, pins geográficos
- **Documentação**: [Mapa Territorial](funcional/05_MAPA_TERRITORIAL.md)

#### 7. **Marketplace**
- **Responsabilidade**: Sistema de trocas locais integrado ao território
- **Função de Negócio**: Facilitar economia local e trocas comunitárias
- **Elementos Técnicos**: Store, StoreItem, Cart, Checkout, pagamentos, mídias (imagens, vídeos, áudios)
- **Documentação**: [Marketplace](funcional/06_MARKETPLACE.md)

#### 8. **Chat**
- **Responsabilidade**: Comunicação territorial (canais, grupos, DM)
- **Função de Negócio**: Facilitar comunicação comunitária
- **Elementos Técnicos**: ChatConversation, ChatMessage, ConversationParticipant
- **Documentação**: [Chat](funcional/07_CHAT.md)

#### 9. **Alertas**
- **Responsabilidade**: Alertas de saúde pública e comunicação emergencial
- **Função de Negócio**: Comunicar informações urgentes e importantes
- **Elementos Técnicos**: Alert, notificações prioritárias
- **Documentação**: [Alertas](funcional/08_ALERTAS.md)

#### 10. **Assets (Recursos Territoriais)**
- **Responsabilidade**: Recursos compartilhados do território
- **Função de Negócio**: Compartilhar documentos, mídias e recursos comunitários
- **Elementos Técnicos**: Asset, geolocalização obrigatória
- **Documentação**: [Assets](funcional/09_ASSETS.md)

#### 11. **Moderação**
- **Responsabilidade**: Manter qualidade e segurança do conteúdo
- **Função de Negócio**: Proteger comunidade de abusos e conteúdo inadequado
- **Elementos Técnicos**: Report, Sanction, WorkItem, automações
- **Documentação**: [Moderação](funcional/10_MODERACAO.md)

#### 12. **Notificações**
- **Responsabilidade**: Sistema confiável de notificações in-app
- **Função de Negócio**: Informar usuários sobre eventos relevantes
- **Elementos Técnicos**: OutboxMessage, UserNotification, inbox persistido
- **Documentação**: [Notificações](funcional/11_NOTIFICACOES.md)

#### 13. **Subscriptions**
- **Responsabilidade**: Sistema de assinaturas recorrentes
- **Função de Negócio**: Sustentabilidade financeira da plataforma
- **Elementos Técnicos**: Subscription, Plan, pagamentos recorrentes
- **Documentação**: [Subscriptions](funcional/12_SUBSCRIPTIONS.md)

#### 14. **Governança e Votação**
- **Responsabilidade**: Decisões coletivas e governança participativa
- **Função de Negócio**: Permitir que comunidades decidam coletivamente
- **Elementos Técnicos**: Vote, Proposal, governança territorial
- **Documentação**: [Governança e Votação](funcional/13_GOVERNANCA_VOTACAO.md)

#### 15. **Admin e Configuração**
- **Responsabilidade**: Administração do sistema e configurações globais
- **Função de Negócio**: Gerenciar plataforma e territórios
- **Elementos Técnicos**: SystemConfig, SystemPermission, WorkQueue
- **Documentação**: [Admin](funcional/14_ADMIN.md)

---

## 💼 Funções de Negócio

### Para Usuários Individuais

#### Como Visitante (Visitor)
- **Descobrir territórios** próximos à localização
- **Visualizar feed público** do território
- **Explorar mapa** com entidades públicas
- **Ver eventos públicos** e participar
- **Visualizar lojas** do marketplace (sem comprar)
- **Acessar chat público** do território
- **Solicitar residência** para acesso ampliado

#### Como Morador (Resident)
- **Todas as funções de Visitor** +
- **Criar posts** (públicos e privados para moradores)
- **Criar eventos** comunitários
- **Comprar no marketplace** (após opt-in)
- **Participar de votações** comunitárias
- **Acessar conteúdo exclusivo** para moradores
- **Criar lojas** no marketplace (após verificação)
- **Acessar chat de moradores**

### Para Comunidades e Territórios

#### Organização Comunitária
- **Definir regras** de participação e visibilidade
- **Gerenciar membros** (aprovar/revogar residências)
- **Moderar conteúdo** através de curadores e moderadores
- **Organizar eventos** comunitários
- **Facilitar economia local** através do marketplace
- **Comunicar alertas** importantes
- **Tomar decisões coletivas** através de votações

#### Governança Territorial
- **Configurar feature flags** (habilitar/desabilitar funcionalidades)
- **Gerenciar capacidades** (Curator, Moderator, EventOrganizer)
- **Aplicar sanções** territoriais quando necessário
- **Revisar work items** (verificações, curadoria, moderação)
- **Auditar decisões** e ações de moderação

### Para a Plataforma

#### Sustentabilidade
- **Subscriptions**: Planos de assinatura para usuários e territórios
- **Marketplace**: Taxas de transação (futuro)
- **Pagamentos**: Processamento seguro de transações

#### Qualidade e Segurança
- **Moderação**: Proteção contra abusos
- **Verificação**: Identidade e residência verificadas
- **Auditoria**: Rastreabilidade completa de ações

---

## 🔄 Como os Elementos se Refletem em Funções

### Território como Unidade Central

**Elemento Técnico**: `Territory` (entidade geográfica neutra)

**Reflexão em Função de Negócio**:
- Usuário **descobre territórios** próximos à sua localização
- Usuário **seleciona território** para interagir
- Todo conteúdo é **vinculado a um território** específico
- Feed, mapa, eventos, marketplace são **filtrados por território**
- Regras e governança são **específicas por território**

**Fluxo de Usuário**:
```
Usuário → Localização → Descoberta de Territórios → Seleção → 
Interação Territorial (Feed, Mapa, Eventos, etc.)
```

### Membership como Vínculo Social

**Elemento Técnico**: `TerritoryMembership` (relação User ↔ Territory)

**Reflexão em Função de Negócio**:
- Usuário **entra como Visitor** (acesso limitado)
- Usuário **solicita residência** para acesso ampliado
- Sistema **diferencia permissões** entre Visitor e Resident
- Conteúdo tem **visibilidade baseada em papel** (público, resident-only)
- Marketplace requer **residência verificada** para compras

**Fluxo de Usuário**:
```
Usuário → Território → Visitor → Solicitar Residência → 
Aprovação → Resident → Acesso Ampliado
```

### Feed como Timeline Territorial

**Elemento Técnico**: `Post`, `PostGeoAnchor`, `Media`

**Reflexão em Função de Negócio**:
- Usuário **visualiza timeline** do território
- Usuário **cria posts** com texto, mídias e geolocalização
- Posts aparecem no **feed cronológico** e no **mapa** (se georreferenciados)
- Visibilidade controla **quem vê o quê** (público, resident-only)

**Fluxo de Usuário**:
```
Usuário → Feed do Território → Criar Post → Adicionar Mídia/GeoAnchor → 
Definir Visibilidade → Publicar → Aparece no Feed e Mapa
```

### Marketplace como Economia Local

**Elemento Técnico**: `Store`, `StoreItem`, `Cart`, `Checkout`

**Reflexão em Função de Negócio**:
- Morador **cria loja** no território (após verificação)
- Morador **cadastra itens** para venda
- Outros moradores **navegam lojas** e **adicionam ao carrinho**
- Sistema processa **checkout** e **pagamento**
- Vendedor recebe **payout** após venda

**Fluxo de Usuário**:
```
Morador → Marketplace → Criar Loja → Cadastrar Itens → 
Outro Morador → Navegar → Adicionar ao Carrinho → 
Checkout → Pagamento → Vendedor recebe Payout
```

### Governança como Decisão Coletiva

**Elemento Técnico**: `Vote`, `Proposal`, `MembershipCapability`

**Reflexão em Função de Negócio**:
- Curador **cria proposta** de decisão comunitária
- Moradores **votam** na proposta
- Sistema **calcula resultado** baseado em regras
- Decisão é **executada** e **auditada**

**Fluxo de Usuário**:
```
Curador → Criar Proposta → Moradores Votam → 
Sistema Calcula Resultado → Decisão Executada → 
Histórico Auditável
```

### Moderação como Proteção Comunitária

**Elemento Técnico**: `Report`, `Sanction`, `WorkItem`

**Reflexão em Função de Negócio**:
- Usuário **reporta** conteúdo ou usuário inadequado
- Moderador **revisa report** e decide ação
- Sistema **aplica sanção** (bloqueio, ocultação, etc.)
- Ação é **auditada** para transparência

**Fluxo de Usuário**:
```
Usuário → Reportar Conteúdo/Usuário → 
Moderador Revisa → Aplicar Sanção → 
Ação Auditada
```

---

## 📚 Navegação da Documentação

### Índice Completo
- **[README - Índice Completo](funcional/README.md)** - Navegação estruturada de toda a documentação funcional

### Documento Central
- **[00 - Plataforma Araponga](funcional/00_PLATAFORMA_ARAPONGA.md)** ← Você está aqui

### Documentação por Funcionalidade

#### Identidade e Vínculos
- [01 - Autenticação e Identidade](funcional/01_AUTENTICACAO_IDENTIDADE.md)
- [02 - Territórios e Memberships](funcional/02_TERRITORIOS_MEMBERSHIPS.md)

#### Conteúdo e Interação
- [03 - Feed Comunitário](funcional/03_FEED_COMUNITARIO.md)
- [04 - Eventos](funcional/04_EVENTOS.md)
- [05 - Mapa Territorial](funcional/05_MAPA_TERRITORIAL.md)

#### Economia e Comunicação
- [06 - Marketplace](funcional/06_MARKETPLACE.md)
- [07 - Chat](funcional/07_CHAT.md)
- [08 - Alertas](funcional/08_ALERTAS.md)

#### Recursos e Gestão
- [09 - Assets](funcional/09_ASSETS.md)
- [10 - Moderação](funcional/10_MODERACAO.md)
- [11 - Notificações](funcional/11_NOTIFICACOES.md)

#### Sustentabilidade e Governança
- [12 - Subscriptions](funcional/12_SUBSCRIPTIONS.md)
- [13 - Governança e Votação](funcional/13_GOVERNANCA_VOTACAO.md)
- [14 - Admin](funcional/14_ADMIN.md)

### Funcionalidades Futuras (Planejadas)

> **⚠️ Nota**: As funcionalidades abaixo estão **planejadas** mas **ainda não implementadas**.

#### Economia Local
- [15 - Compra Coletiva](funcional/15_COMPRA_COLETIVA.md) ⏳ Planejada
- [16 - Hospedagem Territorial](funcional/16_HOSPEDAGEM_TERRITORIAL.md) ⏳ Planejada
- [17 - Demandas e Ofertas](funcional/17_DEMANDAS_OFERTAS.md) ⏳ Planejada
- [18 - Trocas Comunitárias](funcional/18_TROCAS_COMUNITARIAS.md) ⏳ Planejada
- [19 - Moeda Territorial](funcional/19_MOEDA_TERRITORIAL.md) ⏳ Planejada

#### Web3 e DAO
- [20 - Web3 e Blockchain](funcional/20_WEB3_BLOCKCHAIN.md) ⏳ Planejada
- [21 - DAO e Tokenização](funcional/21_DAO_TOKENIZACAO.md) ⏳ Planejada

#### Extensões
- [22 - Learning Hub](funcional/22_LEARNING_HUB.md) ⏳ Planejada

---

## 🔮 Funcionalidades Futuras (Planejadas)

> **⚠️ Status**: As funcionalidades descritas nesta seção estão **planejadas** mas **ainda não implementadas**. Detalhes podem mudar durante o desenvolvimento.

A plataforma Araponga tem um roadmap estratégico que evolui de um MVP sólido para uma plataforma completa de organização comunitária territorial. As funcionalidades futuras estão organizadas em ondas estratégicas priorizadas.

### Onda 3: Economia Local (Próximas Prioridades)

#### Compra Coletiva
- **Status**: ⏳ Planejado (Fase 17)
- **Objetivo**: Organizar compras coletivas de alimentos e produtos locais
- **Função de Negócio**: Conectar produtores locais com consumidores, organizar rodadas de compra, integrar com votação para decisões coletivas
- **Documentação**: [Compra Coletiva](funcional/15_COMPRA_COLETIVA.md)

#### Hospedagem Territorial
- **Status**: ⏳ Planejado (Fase 18)
- **Objetivo**: Sistema de hospedagem territorial (alternativa local ao Airbnb)
- **Função de Negócio**: Moradores validados cadastram propriedades, visitantes solicitam estadias, agenda de disponibilidade, pagamentos com escrow
- **Documentação**: [Hospedagem Territorial](funcional/16_HOSPEDAGEM_TERRITORIAL.md)

#### Demandas e Ofertas
- **Status**: ⏳ Planejado (Fase 19)
- **Objetivo**: Sistema bidirecional de demandas (procura) e ofertas (suprimento)
- **Função de Negócio**: Moradores cadastram necessidades, outros fazem ofertas, negociação antes de aceitar
- **Documentação**: [Demandas e Ofertas](funcional/17_DEMANDAS_OFERTAS.md)

### Onda 4: Economia Local Completa

#### Trocas Comunitárias
- **Status**: ⏳ Planejado (Fase 20)
- **Objetivo**: Sistema de trocas diretas de itens e serviços
- **Função de Negócio**: Troca de item/serviço por outro, sem necessariamente usar moeda
- **Documentação**: [Trocas Comunitárias](funcional/18_TROCAS_COMUNITARIAS.md)

#### Entregas Territoriais
- **Status**: ⏳ Planejado (Fase 21)
- **Objetivo**: Sistema de entregas locais organizadas
- **Função de Negócio**: Organizar entregas coletivas, rotas otimizadas, pontos de entrega comunitários
- **Integração**: Com Compra Coletiva (Fase 17) para distribuição de produtos

#### Moeda Territorial
- **Status**: ⏳ Planejado (Fase 22)
- **Objetivo**: Moeda virtual específica do território
- **Função de Negócio**: Facilitar economia circular local, preparação para tokens on-chain
- **Documentação**: [Moeda Territorial](funcional/19_MOEDA_TERRITORIAL.md)

### Onda 6: Autonomia Digital

#### Chat com IA
- **Status**: ⏳ Planejado (Fase 27)
- **Objetivo**: Assistente de IA para suporte e sugestões
- **Função de Negócio**: Chatbot para suporte, sugestões automáticas, moderação assistida

#### Hub de Serviços Digitais
- **Status**: ⏳ Planejado (Fase 26)
- **Objetivo**: Plataforma de serviços digitais territoriais
- **Função de Negócio**: Oferecer serviços digitais locais, integração com serviços externos

### Onda 7-8: Web3 e DAO

#### Integração Blockchain
- **Status**: ⏳ Planejado (Fases 31-35)
- **Objetivo**: Preparar infraestrutura para Web3
- **Função de Negócio**: Camada de abstração blockchain, wallets, smart contracts básicos
- **Documentação**: [Web3 e Blockchain](funcional/20_WEB3_BLOCKCHAIN.md)

#### DAO e Tokenização
- **Status**: ⏳ Planejado (Fases 36-40)
- **Objetivo**: Governança descentralizada com tokens on-chain
- **Função de Negócio**: Tokens ERC-20, votações on-chain, proof of presence on-chain
- **Documentação**: [DAO e Tokenização](funcional/21_DAO_TOKENIZACAO.md)

### Onda 10: Extensões e Diferenciação

#### Learning Hub
- **Status**: ⏳ Planejado (Fase 45)
- **Objetivo**: Plataforma de cursos e conhecimento territorial
- **Função de Negócio**: Cursos locais, certificações, monetização de conhecimento
- **Documentação**: [Learning Hub](funcional/22_LEARNING_HUB.md)

#### Rental System
- **Status**: ⏳ Planejado (Fase 46)
- **Objetivo**: Sistema de aluguel de recursos diversos
- **Função de Negócio**: Aluguel de salas, equipamentos, espaços, veículos

#### Banco de Sementes
- **Status**: ⏳ Planejado (Fase 48)
- **Objetivo**: Sistema de banco de sementes e mudas
- **Função de Negócio**: Catalogar, trocar e preservar sementes locais

---

## 🎯 Resumo Executivo

Araponga é uma plataforma que **conecta tecnologia e território** de forma consciente. Cada elemento técnico da arquitetura se reflete diretamente em funções de negócio que servem à vida comunitária local.

**Princípios que guiam tudo**:
- Território é geográfico e neutro
- Vida social acontece em camadas separadas
- Tecnologia a serviço do território

**Resultado**: Uma infraestrutura digital que fortalece comunidades locais, respeita autonomia territorial e promove cuidado coletivo, sem algoritmos predatórios ou extração de dados.

---

---

## 📝 Notas sobre Funcionalidades Implementadas

### Sistema de Mídias (Fase 10)

A plataforma possui um sistema completo de mídias implementado que permite:

- **Posts**: Múltiplas imagens (até 10), 1 vídeo (máx. 50MB) ou 1 áudio (máx. 10MB)
- **Eventos**: Imagem de capa + até 5 adicionais, vídeos, áudios
- **Marketplace**: Múltiplas imagens (até 10), vídeos, áudios por item
- **Chat**: Imagens e áudios curtos (mensagens de voz)

**Feature Flags**: Cada território pode habilitar/desabilitar tipos de mídia:
- `MediaImagesEnabled`: Imagens em posts/eventos/marketplace
- `MediaVideosEnabled`: Vídeos em posts/eventos/marketplace
- `MediaAudioEnabled`: Áudios em posts/eventos/marketplace
- `ChatMediaImagesEnabled`: Imagens no chat
- `ChatMediaAudioEnabled`: Áudios no chat

**Georreferenciamento Automático**: Mídias com metadados de localização geram automaticamente `PostGeoAnchor`, fazendo posts aparecerem no mapa.

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Documentação Funcional Completa
