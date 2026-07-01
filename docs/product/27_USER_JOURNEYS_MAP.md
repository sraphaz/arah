# Mapa Completo de Jornadas do Usuário - Arah Flutter App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Mapa Completo de Jornadas  
**Tipo**: Documentação de Experiência do Usuário (UX)

---

## 📋 Índice

1. [Metodologia e Estrutura](#metodologia-e-estrutura)
2. [Narrativa de Acesso Inicial](#narrativa-de-acesso-inicial)
3. [Jornadas por Papel](#jornadas-por-papel)
4. [Jornadas por Módulo](#jornadas-por-módulo)
5. [Jornadas Cruzadas (Interações)](#jornadas-cruzadas-interações)
6. [Mapa Visual de Jornadas](#mapa-visual-de-jornadas)
7. [Pontos de Fricção e Otimização](#pontos-de-fricção-e-otimização)
8. [Análise de Documentação Existente](#análise-de-documentação-existente)
9. [Recomendações para Desenvolvimento](#recomendações-para-desenvolvimento)

---

## 🎯 Metodologia e Estrutura

### Definição de Jornada

Uma **jornada do usuário** no Arah é uma sequência de interações que um usuário realiza para alcançar um objetivo específico dentro da plataforma, considerando:

- **Papel do usuário**: VISITOR, RESIDENT, CURATOR, MODERATOR, EVENT_ORGANIZER, SYSTEM_ADMIN
- **Módulo/Funcionalidade**: Auth, Territórios, Feed, Mapa, Eventos, Marketplace, Chat, etc.
- **Contexto territorial**: Território ativo, localização física, vínculos
- **Estados emocionais**: Expectativa, curiosidade, satisfação, frustração
- **Touchpoints**: Telas, ações, feedbacks, transições

### Formato de Documentação

Cada jornada será documentada com:

1. **Metadados**: Papel, módulo, objetivo, prioridade
2. **Pré-condições**: Estado inicial necessário
3. **Fluxo Passo a Passo**: Interações detalhadas
4. **UI/UX Esperada**: Elementos visuais, estados, transições
5. **Feedback e Validações**: Mensagens, erros, sucessos
6. **Pós-condições**: Estado final, próximos passos
7. **Variações**: Diferentes caminhos e exceções
8. **Métricas**: Tempo estimado, passos, taxa de conversão esperada

---

## 🚀 Narrativa de Acesso Inicial

### A Primeira Experiência: Do Descobrimento ao Engajamento

**Persona**: Maria, 34 anos, moradora do Sertão do Camburi, interessada em fortalecer laços comunitários locais.

#### Cena 1: Descoberta e Download

**Contexto**: Maria ouviu sobre o Arah de uma amiga que mencionou uma plataforma para organização comunitária local.

**Ações**:
1. Maria busca "Arah" na App Store/Google Play
2. Encontra o app com ícone verde (referência ao pássaro Arah)
3. Lê descrição: "Plataforma comunitária orientada a território"
4. Vê screenshots mostrando feed local, mapa integrado, eventos
5. **Decisão**: Baixa o app

**Emoções**: Curiosidade, expectativa de relevância local

---

#### Cena 2: Primeiro Acesso e Onboarding

**Tela 1: Splash Screen**
- Logo do Arah centralizado
- Background com gradiente verde suave
- Transição suave (fade-in, 800ms)

**Tela 2: Boas-vindas (se primeiro acesso)**
- **Título**: "Bem-vinda ao Arah"
- **Subtítulo**: "Conecte-se com seu território, fortaleça sua comunidade"
- **Ilustração**: Mapa com pins, pessoas se reunindo
- **CTA**: "Começar" (botão verde floresta)
- **Skip**: "Já tenho conta" (link discreto)

**Tela 3: Permissão de Localização**
- **Título**: "Descubra seu território"
- **Descrição**: "Para encontrar territórios próximos a você, precisamos da sua localização. Não compartilhamos sua localização com outros usuários."
- **Ilustração**: Ícone de localização estilizado
- **CTAs**: 
  - "Permitir Localização" (primary)
  - "Continuar sem Localização" (secondary)
- **Contexto**: Explica benefícios (territórios próximos, eventos locais, alertas relevantes)

**Se Localização Permitida**:
- Mostra spinner "Buscando territórios próximos..."
- Carrega lista de territórios próximos (raio de 25km)

**Se Localização Negada**:
- Mostra mensagem: "Você pode ativar a localização depois nas configurações"
- Permite busca manual de territórios

---

#### Cena 3: Autenticação Social

**Tela 4: Login/Cadastro**
- **Título**: "Entre ou crie sua conta"
- **Opções de Login Social**:
  - Botão "Continuar com Google" (ícone Google)
  - Botão "Continuar com Apple" (ícone Apple) [iOS]
  - Botão "Continuar com Facebook" (ícone Facebook)
  - Link "Outros métodos" (email/telefone - futuro)

**Fluxo de Login Social**:
1. Usuário toca em "Continuar com Google"
2. Abre modal do Google Sign-In
3. Usuário autoriza
4. Retorna ao app com dados (email, nome, foto)
5. **Nova Tela**: Coleta de dados adicionais

**Tela 5: Coleta de Dados Adicionais**
- **Campos**:
  - Nome de exibição (pré-preenchido do Google, editável)
  - Email (pré-preenchido, não editável)
  - CPF ou Documento Estrangeiro (obrigatório)
    - Toggle: "CPF Brasileiro" / "Documento Estrangeiro"
    - Input com máscara e validação
    - Helper text: "Usado apenas para verificação de identidade"
  - Telefone (opcional)
  - Endereço (opcional, mas recomendado para residência)
- **Validação**: CPF válido (dígito verificador)
- **CTA**: "Continuar" (primary)

**Se 2FA Habilitado**:
- Mostra tela de código 2FA
- Input de 6 dígitos
- Botão "Verificar"
- Link "Usar código de recuperação"

**Sucesso**:
- Token JWT armazenado
- Redireciona para descoberta de territórios

---

#### Cena 4: Descoberta de Territórios

**Tela 6: Territórios Próximos**
- **Header**: "Encontre seu território"
- **Mapa Miniaturizado** (topo da tela):
  - Mapa com pins dos territórios próximos
  - Posição atual do usuário (pino azul)
  - Raio de 25km visível
- **Lista de Territórios**:
  - Card para cada território:
    - **Nome do território** (heading3)
    - **Localização** (cidade, estado) + distância
    - **Badge de proximidade**: "📍 2.3 km de você"
    - **Estatísticas** (se disponível): "1.2k moradores, 340 posts"
    - **CTA**: "Ver Detalhes" (outlined)
  - Pull-to-refresh para atualizar
  - Paginação infinita (scroll)

**Filtros** (Chips horizontais):
- "Mais Próximos" (ativo por padrão)
- "Por Nome"
- "Por Cidade"
- "Buscar" (input de busca)

**Interação com Card**:
- Tap no card → Abre detalhes do território
- Swipe left → Marcar como favorito (ícone de coração)

**Tela 7: Detalhes do Território**
- **Header**: Nome do território + botão voltar
- **Imagem de Capa** (se disponível) ou gradiente verde
- **Informações**:
  - Nome (heading2)
  - Localização (cidade, estado)
  - Distância atual
  - Descrição (se disponível)
  - Polígono no mapa (visualização)
- **Estatísticas**:
  - Número de moradores
  - Número de posts
  - Número de eventos ativos
- **Feature Flags Ativas** (badges):
  - Marketplace habilitado
  - Eventos habilitados
  - Chat habilitado
- **CTA Principal**: "Entrar como Visitante" (primary)
- **CTA Secundário**: "Solicitar Residência" (secondary)
- **Ações**:
  - Compartilhar território
  - Favoritar

---

#### Cena 5: Entrada como Visitante

**Ação**: Usuário toca em "Entrar como Visitante"

**Fluxo**:
1. Verifica localização (se permitida)
2. Valida proximidade ao território (dentro de 25km)
3. Cria vínculo VISITOR (imediato)
4. Define território como ativo (X-Session-Id)
5. Redireciona para Feed do Território

**Tela 8: Feed do Território (Primeira Vez)**
- **Header**: Nome do território + badge "VISITANTE"
- **Banner de Boas-vindas** (se primeira vez):
  - "Bem-vinda ao [Nome do Território]!"
  - "Você está vendo conteúdo público. Para acessar conteúdo exclusivo, solicite residência."
  - CTA: "Solicitar Residência" / "Fechar"
- **Feed de Posts**:
  - Cards de posts públicos
  - Pull-to-refresh
  - Paginação infinita
- **FAB**: "Criar Post" (se RESIDENT) ou "Solicitar Residência" (se VISITOR)
- **Bottom Navigation**: Feed, Mapa, Eventos, Alertas, Perfil

**Primeira Interação**:
- Tutorial overlay (se primeira vez):
  - "Este é o feed do território"
  - "Posts públicos aparecem aqui"
  - "Deslize para ver mais"
  - "Tap para fechar"

---

#### Cena 6: Exploração Inicial

**Ações da Maria**:
1. **Rola o feed**: Vê posts de outros moradores sobre eventos locais, alertas, discussões
2. **Toca em um post**: Abre detalhes
3. **Vai para o Mapa**: Vê pins de posts georreferenciados
4. **Explora Eventos**: Vê eventos próximos, alguns com data futura
5. **Verifica Perfil**: Vê seu perfil básico (nome, avatar padrão)

**Emoções**: Interesse crescente, curiosidade sobre conteúdo exclusivo

---

#### Cena 7: Solicitação de Residência

**Trigger**: Maria quer participar mais ativamente, criar posts, acessar conteúdo exclusivo.

**Tela 9: Solicitar Residência**
- **Header**: "Solicitar Residência" + botão voltar
- **Conteúdo**:
  - **Título**: "Torne-se moradora de [Nome do Território]"
  - **Descrição**: "Como moradora, você terá acesso a conteúdo exclusivo e poderá criar posts, eventos e participar de votações."
  - **Requisitos**:
    - ✓ Geolocalização dentro do território
    - ✓ Verificação opcional de documento
    - ✓ Aprovação por curadores
  - **Passo 1: Verificação de Localização**:
    - Mostra posição atual no mapa
    - Valida se está dentro do polígono ou próximo (raio de 2km)
    - Status: "✅ Localização verificada" ou "⚠️ Precisa estar mais próximo"
  - **Passo 2: Upload de Documento (Opcional)**:
    - "Adicione um comprovante de residência (opcional)"
    - Botão "Adicionar Documento"
    - Preview do documento (se adicionado)
    - Formatos aceitos: PDF, JPG, PNG
    - Tamanho máximo: 10MB
  - **Passo 3: Mensagem (Opcional)**:
    - Input de texto: "Deixe uma mensagem para os curadores (opcional)"
    - Placeholder: "Conte um pouco sobre você e sua relação com o território..."
    - Máximo: 500 caracteres
  - **Ações**:
    - Botão "Enviar Solicitação" (primary, habilitado após validação de localização)
    - Link "Cancelar"

**Envio da Solicitação**:
1. Valida localização
2. Upload do documento (se fornecido)
3. Cria JoinRequest com status PENDING
4. Notifica curadores (se não houver destinatários específicos)
5. Mostra confirmação

**Tela 10: Confirmação de Solicitação**
- **Ícone**: Check verde
- **Título**: "Solicitação Enviada"
- **Mensagem**: "Sua solicitação foi enviada aos curadores. Você receberá uma notificação quando ela for revisada."
- **Status**: Badge "PENDENTE" com cor amarela
- **CTA**: "Voltar ao Feed"

**Estado Posterior**:
- Maria continua como VISITOR
- Badge no perfil mostra "Pendente de Aprovação"
- Notificações são habilitadas para avisar sobre aprovação

---

#### Cena 8: Aprovação e Transição para Residente

**Trigger**: Curador aprova a solicitação de Maria

**Notificação Recebida**:
- Push notification: "🎉 Você foi aprovada como moradora do [Nome do Território]!"
- Badge no ícone de notificações

**Tela 11: Notificação de Aprovação**
- Maria toca na notificação
- Abre tela de notificações
- Card de notificação destacado:
  - Ícone de confirmação (verde)
  - "Sua solicitação de residência foi aprovada!"
  - "Agora você tem acesso completo ao território."
  - CTA: "Explorar Território"

**Atualização Automática**:
- Vínculo atualizado: VISITOR → RESIDENT
- Badge no header muda: "VISITANTE" → "MORADORA" (verde)
- Feed atualiza: Agora mostra posts RESIDENTS_ONLY
- FAB muda: "Criar Post" disponível

**Tela 12: Feed Atualizado (Agora como Residente)**
- **Banner de Parabéns** (se primeira vez como residente):
  - "🎉 Parabéns! Você agora é moradora!"
  - "Você tem acesso a conteúdo exclusivo e pode criar posts e eventos."
  - CTA: "Criar Primeiro Post" / "Fechar"
- Feed agora inclui:
  - Posts PUBLIC
  - Posts RESIDENTS_ONLY (novos!)
  - Eventos RESIDENTS_ONLY

**Emoções**: Satisfação, senso de pertencimento, empoderamento

---

Esta é a narrativa completa do acesso inicial. Agora vamos mapear todas as jornadas detalhadas por papel e módulo.

---

## 👥 Jornadas por Papel

---

## 🟢 JORNADAS DO VISITOR (Visitante)

### Perfil do Papel
- **Objetivo Principal**: Explorar território, descobrir conteúdo público, decidir se quer se engajar
- **Permissões**: Acesso a conteúdo PUBLIC apenas, visualização limitada
- **Limitações**: Não pode criar posts RESIDENTS_ONLY, não pode criar eventos RESIDENTS_ONLY, não pode usar marketplace (compras/vendas), não pode participar de votações

---

### Jornada 1: Explorar Feed Público do Território

**Objetivo**: Ver o que está acontecendo no território

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo VISITOR criado

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Feed
**Ação**: Usuário toca na aba "Feed" na bottom navigation

**Tela**: `FeedScreen`
**Estado Inicial**:
- Header: Nome do território + badge "VISITANTE" (cinza)
- Banner informativo (se primeira vez): "Você está vendo conteúdo público. Para acessar conteúdo exclusivo, solicite residência."
- Pull-to-refresh habilitado
- Lista vazia (loading skeleton aparece)

**Loading State**:
- 3-5 skeleton loaders de cards de post
- Animações shimmer
- Duração: ~500ms-2s (dependendo da conexão)

#### Passo 2: Feed Carregado
**Ação**: API retorna posts PUBLIC do território

**Posts Exibidos**:
- Apenas posts com `visibility = PUBLIC`
- Posts RESIDENTS_ONLY são **filtrados automaticamente** (não aparecem)
- Ordenação: Mais recentes primeiro
- Paginação: 20 posts por página

**Card de Post (Para VISITOR)**:
```
┌─────────────────────────────────────┐
│ [Avatar] Nome do Autor      há 2h  │
│                                     │
│ 📍 Próximo a você (1.2 km)          │
│                                     │
│ Título do Post (se houver)          │
│                                     │
│ Conteúdo do post aqui...            │
│ Pode ter múltiplas linhas.          │
│                                     │
│ [Imagem se houver - 16:9]           │
│                                     │
│ ─────────────────────────────────  │
│ ❤️ 12  💬 5  📤 Compartilhar       │
└─────────────────────────────────────┘
```

**Elementos Visuais**:
- Avatar do autor (circular, 40px)
- Nome do autor (heading4, clicável → abre perfil)
- Timestamp (caption, "há X minutos/horas/dias")
- Badge de proximidade "📍 Próximo a você" (se GeoAnchor disponível)
- Tipo de post (badge discreto): NOTICE, ALERT, ANNOUNCEMENT
- Conteúdo (bodyMedium, markdown renderizado)
- Imagens (se houver, aspect ratio 16:9, cached)
- Ações: Like (heart), Comentar (chat), Compartilhar (share)
- **Indicador visual**: Border sutil cinza (distinguindo de RESIDENTS_ONLY)

**Interações**:
- **Tap no card**: Abre `PostDetailScreen`
- **Tap no avatar/nome**: Abre `UserProfileScreen` do autor
- **Tap em "Gostar"**: Animação de like (scale + bounce), cor vermelha, incrementa contador
- **Tap em "Comentar"**: Abre `PostDetailScreen` com foco no input de comentário
- **Tap em "Compartilhar"**: Abre bottom sheet com opções (copiar link, compartilhar externamente)
- **Swipe left no card**: Ações rápidas (favoritar, reportar)
- **Pull to refresh**: Atualiza feed, mostra indicador de loading no topo

#### Passo 3: Scroll Infinito
**Ação**: Usuário rola até o final da lista

**Comportamento**:
- Quando chega a 80% do final, carrega próxima página automaticamente
- Indicador de loading no final: "Carregando mais posts..."
- Se não houver mais posts: Mensagem "Você viu todos os posts públicos"

#### Passo 4: Visualizar Detalhes do Post
**Ação**: Usuário toca em um card de post

**Transição**: Slide transition da direita (300ms)

**Tela**: `PostDetailScreen`
**Conteúdo**:
- Header com nome do território + botão voltar
- Card do post expandido (mesma estrutura do feed, mas maior)
- Seção de comentários (se houver)
- Ações: Like, Comentar, Compartilhar, Reportar (menu)

**Interações Específicas**:
- **Comentar**: Input na parte inferior, envia comentário (apenas VISITOR pode comentar em posts PUBLIC)
- **Reportar**: Menu de 3 pontos → "Reportar" → Modal com motivos
- **Voltar**: Swipe gesture ou botão voltar → Fade out

**Pós-condições**:
- Post marcado como "visto" (para estatísticas)
- Comentários carregados (se houver)
- Feed mantém posição ao voltar (scroll position preservado)

---

### Jornada 2: Explorar Mapa Territorial

**Objetivo**: Ver posts e eventos georreferenciados no mapa

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo VISITOR criado
- ✅ Localização permitida (recomendado)

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Mapa
**Ação**: Usuário toca na aba "Mapa" na bottom navigation

**Tela**: `MapScreen`
**Estado Inicial**:
- Mapa Google Maps em tela cheia
- Polígono do território destacado (linha verde)
- Posição do usuário (pino azul com pulso)
- Pins de posts/eventos PUBLIC apenas

#### Passo 2: Visualizar Pins no Mapa
**Pins Coloridos por Tipo**:
- **Post PUBLIC**: Pino verde claro (`forest400`)
- **Evento PUBLIC**: Pino azul (`sky400`)
- **Alerta PUBLIC**: Pino laranja (`warning400`)
- **Asset PUBLIC**: Pino terroso (`earth400`)

**Clustering**:
- Se muitos pins próximos, agrupa em cluster
- Cluster mostra número de itens
- Tap no cluster → Zoom in até separar

#### Passo 3: Interagir com Pin
**Ação**: Usuário toca em um pin

**Comportamento**:
- Bottom sheet desliza de baixo para cima (400ms)
- Mostra preview do post/evento:
  - Tipo (badge colorido)
  - Título/nome (heading4)
  - Preview do conteúdo (1-2 linhas)
  - Localização (endereço ou coordenadas)
  - Data/hora (se evento)
  - Ações: "Ver Detalhes", "Fechar"

**Ação "Ver Detalhes"**:
- Abre tela completa do post/evento
- Transição: Slide da direita
- Mapa continua no background (opacidade reduzida)

#### Passo 4: Navegar pelo Mapa
**Gestos**:
- **Pan (arrastar)**: Move mapa suavemente
- **Pinch to zoom**: Zoom in/out com gestos
- **Double tap**: Zoom in
- **Tap e segurar**: Mostra coordenadas (tooltip)

**Atualização de Pins**:
- Ao mover mapa, carrega pins visíveis na viewport (bounds)
- Lazy loading: Não carrega pins fora da tela
- Caching: Pins já carregados não são recarregados

#### Passo 5: Filtrar Conteúdo no Mapa
**Ação**: Usuário toca em filtro (ícone de funil no topo)

**Bottom Sheet de Filtros**:
- **Tipos**: 
  - ☑️ Posts (ativo)
  - ☑️ Eventos (ativo)
  - ☑️ Alertas (ativo)
  - ☑️ Assets (ativo)
- **Período**: 
  - Últimos 7 dias
  - Últimos 30 dias
  - Todos
- **Proximidade**:
  - Todos
  - Até 1 km
  - Até 5 km
  - Até 10 km

**Aplicação de Filtros**:
- Pins no mapa atualizam em tempo real
- Animações de fade out/in suaves
- Contador: "X itens encontrados"

**Pós-condições**:
- Mapa mostra apenas conteúdo filtrado
- Posição do mapa preservada
- Filtros mantidos entre sessões (opcional)

---

### Jornada 3: Explorar Eventos Públicos

**Objetivo**: Ver eventos públicos do território e marcar interesse

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo VISITOR criado

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Eventos
**Ação**: Usuário toca na aba "Eventos" na bottom navigation

**Tela**: `EventsListScreen`
**Estado Inicial**:
- Header: "Eventos" + badge "VISITANTE"
- Filtros (chips horizontais):
  - "Todos" (ativo)
  - "Próximos" (próximos 7 dias)
  - "Acontecendo Agora" (eventos em andamento)
  - "Passados" (eventos finalizados)
- Lista vazia (loading skeleton)

#### Passo 2: Lista de Eventos Carregada
**Eventos Exibidos**:
- Apenas eventos com `visibility = PUBLIC`
- Ordenação: Por data de início (próximos primeiro)
- Paginação: 20 eventos por página

**Card de Evento (Para VISITOR)**:
```
┌─────────────────────────────────────┐
│ [Imagem de Capa - 16:9]             │
│                                     │
│ 🎉 Mutirão de Limpeza               │
│                                     │
│ 📅 25 Jan, 2025 | 09:00 - 12:00    │
│ 📍 Praça Central, Sertão Camburi   │
│ 👥 23 participantes                 │
│                                     │
│ Descrição do evento...              │
│                                     │
│ ─────────────────────────────────  │
│ ✅ Marcar Interesse                 │
└─────────────────────────────────────┘
```

**Elementos Visuais**:
- Imagem de capa (16:9, cached, placeholder se não houver)
- Badge de status: "Em breve" (verde), "Ao vivo" (azul), "Finalizado" (cinza)
- Título (heading3)
- Data/hora (bodyMedium, destaque)
- Localização (bodyMedium, clicável → abre mapa)
- Número de participantes (caption)
- Descrição (bodySmall, truncada a 2 linhas)
- CTA: "Marcar Interesse" (primary) ou "Ver Detalhes" (secondary)

**Interações**:
- **Tap no card**: Abre `EventDetailScreen`
- **Tap em "Marcar Interesse"**: 
  - Badge muda para "Interessado" (verde)
  - Notificação será enviada antes do evento
  - Animações: Scale bounce + haptic feedback
- **Tap na localização**: Abre mapa com pin do evento
- **Pull to refresh**: Atualiza lista

#### Passo 3: Visualizar Detalhes do Evento
**Ação**: Usuário toca em "Ver Detalhes"

**Tela**: `EventDetailScreen`
**Conteúdo Completo**:
- Imagem de capa (expandida, hero)
- Título (heading1)
- Informações detalhadas:
  - Data/hora (com calendário para adicionar)
  - Localização (mapa miniaturizado + endereço completo)
  - Organizador (card clicável → perfil)
  - Descrição completa (markdown)
  - Participantes (lista de avatares, clicável para ver todos)
- Ações:
  - "Marcar Interesse" / "Cancelar Interesse"
  - "Compartilhar"
  - "Reportar" (menu)

**Interações Específicas**:
- **Adicionar ao Calendário**: Botão "Adicionar ao Calendário" → Integra com calendário do dispositivo
- **Navegar até Localização**: Botão "Como Chegar" → Abre Google Maps/Waze
- **Ver Participantes**: Tap na lista de participantes → Bottom sheet com lista completa

**Pós-condições**:
- Evento marcado como "interessado" (se aplicável)
- Notificações habilitadas para o evento

---

### Jornada 4: Buscar e Filtrar Conteúdo

**Objetivo**: Encontrar posts, eventos ou pessoas específicas

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo VISITOR criado

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Busca
**Ação**: Usuário toca no ícone de busca no header

**Tela**: `SearchScreen` (modal ou tela completa)
**Estado Inicial**:
- Input de busca focado (teclado aberto)
- Placeholder: "Buscar posts, eventos, pessoas..."
- Tabs: "Tudo", "Posts", "Eventos", "Pessoas"
- Área vazia (sem resultados ainda)

#### Passo 2: Digitar Busca
**Ação**: Usuário digita na busca

**Comportamento**:
- Debounce de 300ms (aguarda usuário parar de digitar)
- Busca em tempo real (se 3+ caracteres)
- Mostra resultados conforme digita
- Sugestões (se houver histórico de buscas)

#### Passo 3: Visualizar Resultados
**Resultados Exibidos** (filtrados por PUBLIC apenas):
- **Posts**: Cards compactos de posts que contêm termo de busca
- **Eventos**: Cards de eventos com termo no título/descrição
- **Pessoas**: Cards de perfil de usuários (apenas públicos)

**Agrupamento**:
- Seção "Posts" (X resultados)
- Seção "Eventos" (Y resultados)
- Seção "Pessoas" (Z resultados)

**Interações**:
- **Tap em resultado**: Abre detalhes
- **Limpar busca**: Botão X no input → Limpa resultados
- **Filtros**: Chips para refinar busca (data, tipo, localização)

#### Passo 4: Filtros Avançados
**Ação**: Usuário toca em "Filtros" (botão ao lado da busca)

**Bottom Sheet de Filtros Avançados**:
- **Tipo**:
  - Posts
  - Eventos
  - Pessoas
- **Período**:
  - Últimos 7 dias
  - Últimos 30 dias
  - Últimos 3 meses
  - Todos
- **Localização**:
  - Todo o território
  - Próximo a você (raio)
  - Região específica
- **Ordenação**:
  - Mais recentes
  - Mais relevantes
  - Mais populares (por likes)

**Aplicação**:
- Resultados atualizados em tempo real
- Chips mostram filtros ativos
- Botão "Limpar Filtros"

**Pós-condições**:
- Resultados filtrados mantidos
- Histórico de busca salvo (últimas 5 buscas)

---

### Jornada 5: Visualizar Perfil de Outro Usuário

**Objetivo**: Ver informações públicas de outro usuário

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo VISITOR criado

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Perfil
**Ação**: Usuário toca no avatar/nome de um autor de post

**Tela**: `OtherUserProfileScreen`
**Estado Inicial**:
- Header com botão voltar
- Loading skeleton (avatar, nome, bio)

#### Passo 2: Perfil Carregado
**Conteúdo Público Exibido**:
- **Header**:
  - Avatar grande (80px circular)
  - Nome (heading2)
  - Bio (se pública, bodyMedium)
  - Badge de verificação (se verificado)
  - Badge de papel no território: "MORADORA" (se RESIDENT) ou "VISITANTE"
- **Estatísticas** (públicas):
  - Posts (número)
  - Eventos criados (número)
  - Territórios (número)
- **Tabs**:
  - "Posts" (ativo)
  - "Eventos"
  - "Sobre"
- **Ações**:
  - "Seguir" (se não for seguido) / "Seguindo" (se já seguir) - futuro
  - "Enviar Mensagem" (se DM habilitado)
  - "Reportar" (menu)

**Limitações para VISITOR**:
- Não vê posts RESIDENTS_ONLY do usuário
- Não vê informações privadas
- Não pode enviar DM (apenas RESIDENT pode)

#### Passo 3: Explorar Posts do Usuário
**Ação**: Usuário rola até seção "Posts"

**Conteúdo**:
- Grid ou lista de posts PUBLIC do usuário
- Apenas posts públicos são visíveis
- Ordenação: Mais recentes primeiro
- Paginação infinita

**Interações**:
- Tap em post → Abre `PostDetailScreen`
- Pull to refresh → Atualiza lista

#### Passo 4: Explorar Eventos do Usuário
**Ação**: Usuário toca na tab "Eventos"

**Conteúdo**:
- Lista de eventos PUBLIC criados pelo usuário
- Cards de eventos (mesma estrutura da jornada 3)
- Filtros: "Todos", "Próximos", "Passados"

**Pós-condições**:
- Perfil visualizado (para estatísticas)
- Posts/eventos acessíveis

---

### Jornada 6: Solicitar Residência (VISITOR → RESIDENT)

**Objetivo**: Tornar-se moradora do território para acesso completo

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo VISITOR criado
- ✅ Localização permitida (obrigatório para verificação geo)

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Solicitação de Residência
**Triggers**:
- Banner no feed: "Solicitar Residência para acesso completo"
- FAB no feed: "Solicitar Residência"
- Menu do perfil: "Solicitar Residência"
- Card de território: "Solicitar Residência"

**Tela**: `BecomeResidentScreen`
**Estado Inicial**:
- Header: "Solicitar Residência" + botão voltar
- Progress indicator: "Passo 1 de 3"
- Loading: Verificando localização...

#### Passo 2: Verificação de Localização
**Ação**: Sistema verifica localização atual

**Validações**:
1. **Localização Disponível?**
   - ✅ Sim → Continua
   - ❌ Não → Mostra tela de permissão de localização

2. **Dentro do Território?**
   - Verifica se coordenadas estão dentro do polígono do território
   - OU se estão dentro do raio de 2km do centro do território
   - ✅ Sim → "✅ Localização verificada"
   - ❌ Não → "⚠️ Você precisa estar mais próximo do território"

**Tela de Verificação de Localização**:
```
┌─────────────────────────────────────┐
│ Passo 1: Verificação de Localização │
│                                     │
│ [Mapa com polígono do território]   │
│                                     │
│ Seu local: [Pino azul]              │
│ Território: [Polígono verde]        │
│                                     │
│ Status: ✅ Localização verificada   │
│                                     │
│ [Botão Continuar]                   │
└─────────────────────────────────────┘
```

**Se Localização Não Verificada**:
- Mensagem: "Você precisa estar fisicamente próximo ao território para solicitar residência."
- Opções:
  - "Tentar Novamente" (refaz verificação)
  - "Voltar" (cancela solicitação)
  - Link: "Por que preciso estar próximo?" (explica motivo)

#### Passo 3: Upload de Documento (Opcional)
**Ação**: Usuário pode adicionar comprovante de residência

**Tela**: `DocumentUploadStep`
**Opções**:
- Botão "Adicionar Documento"
- Formatos aceitos: PDF, JPG, PNG
- Tamanho máximo: 10MB
- Preview do documento (se adicionado)
- Botão "Remover" (se documento adicionado)

**Fluxo de Upload**:
1. Toca em "Adicionar Documento"
2. Abre seletor de arquivo (File Picker)
3. Seleciona documento
4. Preview aparece
5. Upload inicia (progress bar)
6. Upload completo → "✅ Documento enviado"

**Opcional**: 
- Checkbox "Pular esta etapa" → Permite continuar sem documento

#### Passo 4: Mensagem para Curadores (Opcional)
**Ação**: Usuário pode deixar mensagem

**Input de Texto**:
- Placeholder: "Conte um pouco sobre você e sua relação com o território..."
- Máximo: 500 caracteres
- Contador: "X/500 caracteres"
- Opcional: Pode deixar em branco

#### Passo 5: Enviar Solicitação
**Ação**: Usuário toca em "Enviar Solicitação"

**Validação Final**:
- ✅ Localização verificada
- ✅ Formulário completo (documento e mensagem são opcionais)

**Processamento**:
1. Valida localização novamente
2. Upload do documento (se fornecido)
3. Cria JoinRequest com status PENDING
4. Define destinatários:
   - Se `recipientUserIds` fornecido → Para esses usuários
   - Se não → Para curadores do território
   - Se não houver curadores → Para SystemAdmin
5. Notifica destinatários (push + in-app)
6. Retorna sucesso

**Tela de Confirmação**:
```
┌─────────────────────────────────────┐
│          ✅ Solicitação Enviada      │
│                                     │
│ Sua solicitação foi enviada aos     │
│ curadores do território.            │
│                                     │
│ Status: PENDENTE                    │
│                                     │
│ Você receberá uma notificação       │
│ quando ela for revisada.            │
│                                     │
│ [Botão Voltar ao Feed]              │
└─────────────────────────────────────┘
```

**Transição**:
- Volta para Feed
- Badge no header muda: "VISITANTE" → "PENDENTE" (amarelo)
- Banner informativo: "Aguardando aprovação de residência..."

**Pós-condições**:
- JoinRequest criada com status PENDING
- Usuário continua como VISITOR
- Notificações habilitadas para aprovação/rejeição
- Documento armazenado (se fornecido) para revisão

---

### Jornada 7: Verificar Status de Solicitação de Residência

**Objetivo**: Acompanhar status da solicitação de residência

**Pré-condições**:
- ✅ Solicitação de residência enviada (status PENDING)

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Status
**Triggers**:
- Banner no feed: "Sua solicitação está sendo revisada"
- Menu do perfil: "Status de Residência"
- Badge "PENDENTE" no header (clicável)

**Tela**: `ResidencyStatusScreen`
**Estado Inicial**:
- Header: "Status de Residência" + botão voltar
- Card de status com informações

#### Passo 2: Visualizar Status Atual
**Card de Status**:
```
┌─────────────────────────────────────┐
│ Status: PENDENTE                    │
│                                     │
│ Data da Solicitação: 20 Jan, 2025  │
│                                     │
│ Revisando: Curadores do território  │
│                                     │
│ Sua solicitação está sendo          │
│ revisada. Você receberá uma         │
│ notificação quando ela for          │
│ aprovada ou rejeitada.              │
│                                     │
│ [Ícone de relógio animado]          │
└─────────────────────────────────────┘
```

**Estados Possíveis**:
- **PENDING**: Badge amarelo, mensagem de aguardo
- **APPROVED**: Badge verde, mensagem de sucesso (quando aprovado)
- **REJECTED**: Badge vermelho, mensagem de rejeição + motivo (se fornecido)

**Ações Disponíveis**:
- **Cancelar Solicitação** (se PENDING): Botão secundário
- **Reenviar Solicitação** (se REJECTED): Botão primary
- **Voltar**: Botão voltar ou swipe gesture

#### Passo 3: Receber Notificação de Aprovação
**Trigger**: Curador aprova solicitação

**Notificação Push**:
- Título: "🎉 Você foi aprovada!"
- Corpo: "Sua solicitação de residência foi aprovada. Agora você tem acesso completo ao território."

**Badge no App**:
- Ícone de notificações com badge vermelho (número)

**Tela de Notificações**:
- Card de notificação destacado:
  - Ícone de confirmação (verde, check)
  - Título: "Residência Aprovada"
  - Mensagem: "Você agora é moradora do [Nome do Território]!"
  - CTA: "Explorar Território"

#### Passo 4: Atualização Automática do Status
**Ação**: Usuário retorna ao app ou toca na notificação

**Atualização Automática**:
- Vínculo atualizado: VISITOR → RESIDENT
- Badge no header muda: "PENDENTE" → "MORADORA" (verde)
- Feed atualiza: Agora mostra posts RESIDENTS_ONLY
- FAB muda: "Criar Post" disponível
- Permissões atualizadas:
  - Pode criar posts RESIDENTS_ONLY
  - Pode criar eventos RESIDENTS_ONLY
  - Pode usar marketplace
  - Pode participar de votações

**Banner de Parabéns**:
- Aparece no feed (se primeira vez como residente)
- "🎉 Parabéns! Você agora é moradora!"
- "Você tem acesso a conteúdo exclusivo e pode criar posts e eventos."
- CTA: "Criar Primeiro Post" / "Fechar"

**Pós-condições**:
- Status atualizado para APPROVED
- Permissões ampliadas
- Experiência completa do território desbloqueada

---

**Continuando com as jornadas do RESIDENT...** (documento continuará com todas as jornadas detalhadas)

---

## 🟦 JORNADAS DO RESIDENT (Morador)

### Perfil do Papel
- **Objetivo Principal**: Participar ativamente da comunidade, criar conteúdo, organizar eventos, usar marketplace
- **Permissões**: Acesso completo ao território (PUBLIC + RESIDENTS_ONLY)
- **Capacidades**: Pode criar posts/eventos, usar marketplace, participar de votações, usar todas as funcionalidades

---

### Jornada 8: Criar Primeiro Post

**Objetivo**: Publicar conteúdo no feed do território

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo RESIDENT ativo
- ✅ Sem sanções de posting

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Criação de Post
**Triggers**:
- FAB no feed: "+" (verde, circular)
- Banner: "Criar Primeiro Post"
- Menu do perfil: "Criar Post"

**Tela**: `CreatePostScreen`
**Estado Inicial**:
- Header: "Novo Post" + botão voltar + botão "Publicar"
- Formulário vazio
- Seletor de tipo: NOTICE (padrão), ALERT, ANNOUNCEMENT
- Toggle de visibilidade: PUBLIC (padrão) / RESIDENTS_ONLY

#### Passo 2: Preencher Conteúdo
**Campos**:
- **Título** (opcional, máximo 200 caracteres):
  - Placeholder: "Título do post (opcional)"
  - Input de texto (heading4 style enquanto digita)
- **Conteúdo** (obrigatório, máximo 4000 caracteres):
  - Placeholder: "O que está acontecendo no território?"
  - TextArea expandível (mínimo 3 linhas)
  - Contador: "X/4000 caracteres"
  - Suporte a Markdown (botão "Preview" para ver renderizado)
- **Tipo** (obrigatório):
  - Chips: "Comunicado", "Alerta", "Anúncio"
  - Seleção muda cor do card de preview
- **Visibilidade** (obrigatório):
  - Switch: "Público" / "Apenas Moradores"
  - Helper text: "Público: todos veem | Apenas Moradores: só residentes"
  - Badge visual mostrando quem verá

#### Passo 3: Adicionar Mídia (Opcional)
**Ação**: Usuário toca em "Adicionar Foto/Vídeo"

**Opções**:
- "Tirar Foto" (câmera)
- "Escolher da Galeria" (galeria)
- "Escolher Arquivo" (documentos)

**Seleção de Mídia**:
- Picker abre
- Seleção múltipla permitida (até 10 imagens/vídeos)
- Preview das mídias selecionadas (grid 3 colunas)
- GeoAnchor derivado automaticamente (se mídia tem EXIF)

**Upload**:
- Progress bar por mídia
- Compressão automática de imagens
- Thumbnails gerados
- Preview atualizado em tempo real

**Preview das Mídias**:
- Grid de miniaturas
- Botão "X" para remover cada mídia
- Drag & drop para reordenar
- Tap em miniatura → Abre viewer fullscreen

#### Passo 4: Visualizar Preview
**Card de Preview**:
- Mesma estrutura do card de post no feed
- Mostra como ficará publicado
- Atualiza em tempo real conforme edição

#### Passo 5: Validar e Publicar
**Ação**: Usuário toca em "Publicar"

**Validações**:
- ✅ Conteúdo não vazio (mínimo 10 caracteres)
- ✅ Conteúdo não excede 4000 caracteres
- ✅ Título não excede 200 caracteres (se fornecido)
- ✅ Território ativo válido
- ✅ Sem sanções de posting
- ✅ Feature flags respeitadas (ex: ALERT só se flag habilitada)

**Se Válido**:
1. Botão "Publicar" fica disabled
2. Indicador de loading: "Publicando..."
3. Upload de mídias (se houver)
4. POST /api/v1/feed
5. Sucesso → Confirmação

**Tela de Confirmação**:
```
┌─────────────────────────────────────┐
│        ✅ Post Publicado!            │
│                                     │
│ Seu post foi publicado com          │
│ sucesso no feed do território.      │
│                                     │
│ [Preview do post publicado]         │
│                                     │
│ [Botão Ver no Feed]                 │
│ [Botão Criar Outro Post]            │
└─────────────────────────────────────┘
```

**Se Erro**:
- Snackbar com mensagem de erro
- Botão "Publicar" reabilitado
- Erro específico:
  - "Conteúdo muito curto" (se < 10 caracteres)
  - "Você não tem permissão para criar posts" (se sanção)
  - "Erro ao publicar. Tente novamente." (erro genérico)

**Pós-condições**:
- Post publicado no feed
- Post aparece no mapa (se tem GeoAnchor)
- Notificações enviadas para moradores interessados (se relevante)
- Gamificação: +5 pontos por criar post (se habilitada)
- Redireciona para feed (post destacado)

---

### Jornada 9: Participar de Evento

**Objetivo**: Inscrever-se em evento comunitário

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo RESIDENT ativo

**Fluxo Passo a Passo**:

#### Passo 1: Descobrir Evento
**Ações**:
- Ver no feed (card de evento)
- Ver na lista de eventos (tab "Eventos")
- Ver no mapa (pin de evento)
- Receber notificação de novo evento

#### Passo 2: Visualizar Detalhes do Evento
**Tela**: `EventDetailScreen`
**Conteúdo Completo**:
- Imagem de capa (hero, full width)
- Título (heading1)
- **Informações Principais**:
  - Data/hora de início (destaque)
  - Data/hora de fim (se houver)
  - Localização (endereço completo)
  - Mapa miniaturizado com pin
- **Organizador**:
  - Card clicável com avatar + nome
  - Tap → Abre perfil do organizador
- **Descrição**:
  - Texto completo (markdown renderizado)
  - Expandível se muito longo
- **Participantes**:
  - Lista de avatares (primeiros 10)
  - "Ver todos (X participantes)"
  - Tap → Bottom sheet com lista completa
- **Status**:
  - Badge: "Em breve", "Acontecendo agora", "Finalizado"
  - Contador regressivo (se em breve)
- **Ações**:
  - "Participar" / "Já Participando" (primary)
  - "Adicionar ao Calendário" (secondary)
  - "Como Chegar" (secondary)
  - "Compartilhar" (menu)
  - "Reportar" (menu)

#### Passo 3: Inscrever-se no Evento
**Ação**: Usuário toca em "Participar"

**Validações**:
- ✅ Evento não está finalizado
- ✅ Usuário não está já inscrito
- ✅ Capacidade não excedida (se houver limite)

**Processamento**:
1. POST /api/v1/events/{id}/register
2. Indicador de loading: "Inscrevendo-se..."
3. Sucesso → Confirmação

**Confirmação**:
- Snackbar: "✅ Você está participando do evento!"
- Badge muda: "Participar" → "Já Participando" (verde)
- Notificação: "Você está inscrito no evento [Nome]. Lembrete será enviado antes do evento."
- Calendário: Opção de adicionar ao calendário do dispositivo

#### Passo 4: Check-in no Evento (No Dia)
**Trigger**: Dia/hora do evento chegou

**Notificação**:
- Push: "🎉 Evento começando agora: [Nome do Evento]"
- In-app: Card destacado no feed

**Ação**: Usuário vai até o local do evento

**Tela de Check-in**:
- Abre automaticamente quando usuário está próximo (dentro de 200m)
- Ou manualmente: Botão "Fazer Check-in" no evento

**Processo de Check-in**:
1. Verifica localização atual
2. Valida proximidade ao evento (raio de 200m)
3. POST /api/v1/events/{id}/checkin
4. Confirmação: "✅ Check-in realizado!"

**Benefícios do Check-in**:
- Gamificação: +20 pontos por participar (se habilitada)
- Badge de presença no evento
- Confirmação de participação

---

### Jornada 10: Criar Evento Comunitário

**Objetivo**: Organizar evento para a comunidade

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Território selecionado como ativo
- ✅ Vínculo RESIDENT ativo
- ✅ Capability EVENT_ORGANIZER (ou apenas RESIDENT, dependendo da flag)

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Criação de Evento
**Triggers**:
- FAB na lista de eventos: "+"
- Menu do perfil: "Criar Evento"
- Banner: "Organize um evento comunitário"

**Tela**: `CreateEventScreen`
**Estado Inicial**:
- Header: "Novo Evento" + botão voltar + botão "Publicar"
- Formulário vazio
- Stepper: "Passo 1 de 4"

#### Passo 2: Informações Básicas
**Campos**:
- **Título** (obrigatório, máximo 200 caracteres):
  - Placeholder: "Nome do evento"
  - Input de texto
- **Descrição** (obrigatório, máximo 2000 caracteres):
  - Placeholder: "Descreva o evento..."
  - TextArea expandível
  - Suporte a Markdown
- **Tipo** (obrigatório):
  - Chips: "Mutirão", "Reunião", "Festival", "Oficina", "Outro"
  - Seleção afeta ícone e cor do evento
- **Visibilidade** (obrigatório):
  - Switch: "Público" / "Apenas Moradores"
  - Helper text

**Validação em Tempo Real**:
- Título: Mínimo 5 caracteres
- Descrição: Mínimo 20 caracteres
- Botão "Próximo" habilitado quando válido

#### Passo 3: Data e Hora
**Campos**:
- **Data de Início** (obrigatório):
  - Date picker (calendário)
  - Validação: Não pode ser no passado (se menos de 1 hora atrás)
- **Hora de Início** (obrigatório):
  - Time picker
  - Formato: HH:mm (24h)
- **Data de Fim** (opcional):
  - Checkbox "Evento tem data de término"
  - Se marcado, mostra date picker
  - Validação: Deve ser após data de início
- **Hora de Fim** (opcional, se data de fim marcada):
  - Time picker
  - Validação: Deve ser após hora de início

**Validações**:
- Data de início não pode ser no passado (com tolerância de 1 hora)
- Data de fim deve ser após data de início
- Duração máxima: 30 dias (se configurado)

**Botão "Próximo"**: Habilitado quando data/hora válidas

#### Passo 4: Localização
**Campos**:
- **Endereço** (obrigatório):
  - Input com autocomplete (geocoding)
  - Sugestões conforme digita
  - Tap em sugestão → Preenche coordenadas automaticamente
- **Localização no Mapa**:
  - Mapa interativo
  - Pin arrastável
  - Coordenadas atualizadas ao mover pin
  - Botão "Usar Minha Localização" → Move pin para posição atual

**Validações**:
- Localização deve estar dentro do território (ou próximo, raio de 5km)
- Coordenadas válidas (lat: -90 a 90, lng: -180 a 180)

**Mapa Interativo**:
- Zoom automático para território
- Pin verde (arrastável)
- Polígono do território destacado
- Botão "Centralizar no Território"

#### Passo 5: Mídia e Configurações
**Campos**:
- **Imagem de Capa** (opcional):
  - Botão "Adicionar Imagem de Capa"
  - Picker de imagem
  - Preview da imagem selecionada
  - Aspect ratio: 16:9 (crop sugerido)
- **Capacidade** (opcional):
  - Input numérico: "Limite de participantes"
  - Se vazio, evento ilimitado
  - Validação: Mínimo 2, máximo 10000
- **Requer Aprovação** (opcional):
  - Switch: "Aprovar participantes manualmente"
  - Se habilitado, participantes precisam ser aprovados

**Validações**:
- Capacidade: Se definida, deve ser >= 2

#### Passo 6: Preview e Publicar
**Tela de Preview**:
- Card completo do evento (como aparecerá no feed)
- Todas as informações revisáveis
- Botão "Editar" em cada seção

**Ação Final**: Usuário toca em "Publicar"

**Validações Finais**:
- ✅ Todos os campos obrigatórios preenchidos
- ✅ Data/hora válidas
- ✅ Localização válida e dentro do território
- ✅ Sem conflitos (evento não sobrepõe outros eventos muito próximos no mesmo horário)

**Processamento**:
1. Upload de imagem de capa (se houver)
2. POST /api/v1/events
3. Indicador de loading: "Criando evento..."
4. Sucesso → Confirmação

**Tela de Confirmação**:
```
┌─────────────────────────────────────┐
│      ✅ Evento Criado!               │
│                                     │
│ Seu evento foi criado com sucesso   │
│ e já está visível no feed e no      │
│ mapa do território.                 │
│                                     │
│ [Preview do evento criado]          │
│                                     │
│ [Botão Ver Evento]                  │
│ [Botão Compartilhar Evento]         │
└─────────────────────────────────────┘
```

**Pós-condições**:
- Evento publicado no feed
- Evento aparece no mapa (pin azul)
- Notificações enviadas para moradores interessados em eventos
- Gamificação: +25 pontos por criar evento (se habilitada)
- Organizador adicionado automaticamente como participante

---

## 🎯 Resumo de Jornadas por Papel

### VISITOR (Visitante)
- ✅ Explorar Feed Público
- ✅ Explorar Mapa Territorial
- ✅ Explorar Eventos Públicos
- ✅ Buscar e Filtrar Conteúdo
- ✅ Visualizar Perfil de Outro Usuário
- ✅ Solicitar Residência
- ✅ Verificar Status de Solicitação

### RESIDENT (Morador)
- ✅ Criar Post (PUBLIC ou RESIDENTS_ONLY)
- ✅ Participar de Evento
- ✅ Criar Evento Comunitário
- ✅ Usar Marketplace (compras/vendas)
- ✅ Participar de Votações
- ✅ Usar Chat (canais e DM)
- ✅ Verificar Alertas de Saúde
- ✅ Sugerir Assets Territoriais
- ✅ Recuperar Conta (reset de senha, recuperação de 2FA)
- ✅ Excluir Conta (LGPD/GDPR)
- ✅ Usar Modo Offline

### CURATOR (Curador)
- ✅ Aprovar Solicitações de Residência
- ✅ Validar Assets Territoriais
- ✅ Gerenciar Votações Comunitárias
- ✅ Dashboard de Governança
- ✅ Configurar Feature Flags Territoriais
- ✅ Revisar Work Queue (evidências, documentações)

### MODERATOR (Moderador)
- ✅ Revisar Reports
- ✅ Aplicar Sanções Territoriais
- ✅ Bloquear/Desbloquear Usuários
- ✅ Ocultar/Restaurar Conteúdo
- ✅ Dashboard de Moderação

### EVENT_ORGANIZER (Organizador de Eventos)
- ✅ Gerenciar Participantes de Eventos
- ✅ Credenciar Participantes
- ✅ Dashboard de Eventos

### SYSTEM_ADMIN (Administrador do Sistema)
- ✅ Gerenciar Territórios
- ✅ Gerenciar Usuários Globais
- ✅ Configurar Sistema
- ✅ Monitorar Work Queue Global
- ✅ Dashboard Administrativo

---

### Jornada 8: Recuperação de Conta e Reset de Senha

**Objetivo**: Recuperar acesso à conta em caso de perda de credenciais

**Pré-condições**:
- ❌ Usuário perdeu acesso ao método de autenticação
- ❌ Usuário esqueceu senha/token

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Recuperação
**Ação**: Usuário toca em "Esqueceu sua conta?" na tela de login

**Tela**: `AccountRecoveryScreen`
**Opções**:
- "Recuperar via Email"
- "Recuperar via Telefone"
- "Recuperar código 2FA" (se 2FA habilitado)

#### Passo 2: Solicitar Recuperação
**Ação**: Usuário seleciona método e fornece email/telefone

**Processo**:
1. Valida email/telefone
2. Envia código de recuperação (email/SMS)
3. Mostra tela de inserção de código

**Tela**: `RecoveryCodeScreen`
- Input de código (6 dígitos)
- Botão "Reenviar código" (após 60 segundos)
- Botão "Voltar"

#### Passo 3: Validar Código
**Ação**: Usuário insere código recebido

**Validações**:
- ✅ Código válido e não expirado (15 minutos)
- ✅ Tentativas máximas: 3 (após isso, bloquear por 1 hora)

**Se Válido**:
- Permite redefinir senha/configurar novo método de autenticação
- Expira código usado

**Se Inválido**:
- Mostra erro: "Código inválido. Tente novamente."
- Incrementa tentativas

#### Passo 4: Redefinir Acesso
**Tela**: `ResetAccessScreen`
**Opções**:
- Configurar novo método de autenticação social
- Redefinir 2FA (se aplicável)
- Configurar backup codes

**Pós-condições**:
- Acesso recuperado
- Notificação enviada sobre recuperação de conta

---

### Jornada 9: Excluir Conta (LGPD/GDPR)

**Objetivo**: Excluir conta e dados pessoais conforme LGPD/GDPR

**Pré-condições**:
- ✅ Usuário autenticado
- ✅ Usuário quer excluir conta

**Fluxo Passo a Passo**:

#### Passo 1: Acessar Configurações de Conta
**Ação**: Usuário vai em Perfil → Configurações → Conta → Excluir Conta

**Tela**: `DeleteAccountScreen`
**Avisos**:
- "⚠️ Esta ação é irreversível"
- "Todos os seus dados serão excluídos permanentemente"
- "Você pode exportar seus dados antes de excluir"

#### Passo 2: Exportar Dados (Opcional)
**Ação**: Usuário toca em "Exportar Meus Dados"

**Processo**:
1. Gera arquivo JSON com todos os dados do usuário
2. Disponibiliza para download
3. Envia também por email (se configurado)

**Conteúdo do Export**:
- Perfil completo
- Posts criados
- Comentários
- Eventos criados/participados
- Vínculos territoriais
- Preferências
- Histórico de atividades

#### Passo 3: Confirmar Exclusão
**Ação**: Usuário toca em "Excluir Minha Conta"

**Confirmação Dupla**:
1. Modal de confirmação: "Tem certeza que deseja excluir sua conta?"
2. Input de confirmação: Digitar "EXCLUIR" para confirmar

**Processo**:
1. Valida confirmação
2. POST /api/v1/auth/delete-account
3. Backend marca conta para exclusão
4. Período de graça (7 dias) - usuário pode cancelar
5. Após 7 dias, exclusão permanente

#### Passo 4: Período de Graça
**Notificação**:
- "Sua conta será excluída em 7 dias. Você pode cancelar a exclusão a qualquer momento."

**Tela**: `AccountDeletionPendingScreen`
- Contador regressivo: "Exclusão em X dias"
- Botão "Cancelar Exclusão"
- Aviso: "Após exclusão, não será possível recuperar seus dados"

#### Passo 5: Exclusão Concluída
**Pós-condições**:
- Conta excluída permanentemente
- Dados removidos (anonimizados ou deletados)
- Logout automático
- Redirecionamento para tela de login

---

### Jornada 10: Usar Modo Offline

**Objetivo**: Usar funcionalidades básicas do app sem internet

**Pré-condições**:
- ✅ App já usado anteriormente (dados cacheados)
- ❌ Sem conexão à internet

**Fluxo Passo a Passo**:

#### Passo 1: Detecção de Modo Offline
**Ação**: App detecta perda de conexão

**Banner de Aviso**:
- "📴 Você está offline. Algumas funcionalidades estão limitadas."
- Badge discreto no topo da tela

#### Passo 2: Visualizar Conteúdo Cacheado
**Funcionalidades Disponíveis**:
- ✅ Feed (últimos posts cacheados)
- ✅ Eventos (próximos eventos cacheados)
- ✅ Perfil próprio
- ✅ Mapa (dados cacheados)

**Limitações**:
- ❌ Não pode atualizar feed
- ❌ Não pode buscar novos territórios
- ❌ Não pode criar eventos (requer validação de localização)

#### Passo 3: Criar Post Offline
**Ação**: Usuário cria post enquanto offline

**Fluxo**:
1. Preenche formulário normalmente
2. Toca em "Publicar"
3. Post salvo localmente (fila offline)
4. Badge: "⏳ Post será publicado quando online"
5. Post aparece no feed local com badge "Pendente"

#### Passo 4: Sincronização ao Voltar Online
**Ação**: App detecta retorno de conexão

**Processo Automático**:
1. Mostra banner: "🔄 Sincronizando dados..."
2. Processa fila offline:
   - Publica posts pendentes
   - Envia comentários pendentes
   - Registra likes pendentes
3. Atualiza feed automaticamente
4. Notifica: "✅ Sincronização concluída"

**Resolução de Conflitos**:
- Se post foi editado/deletado online enquanto offline:
  - Mostrar diálogo: "Este post foi alterado online. O que você deseja fazer?"
  - Opções: "Manter alterações online", "Sobrescrever com versão offline"

**Pós-condições**:
- Dados sincronizados
- Feed atualizado
- Fila offline vazia

---

## 🔄 Jornadas Cruzadas (Interações)

### Jornada: Reportar Conteúdo Inadequado

**Objetivo**: Denunciar post, evento ou usuário inadequado

**Papéis**: VISITOR, RESIDENT

**Fluxo**:
1. Usuário visualiza conteúdo inadequado
2. Toca em menu "..." → "Reportar"
3. Seleciona motivo (SPAM, INAPPROPRIATE_CONTENT, HARASSMENT, etc.)
4. Adiciona detalhes opcionais
5. Envia report
6. Report entra na Work Queue de moderação
7. Moderadores recebem notificação
8. Moderador revisa e decide (sanção ou arquivar)

**Feedback**:
- Confirmação: "Report enviado. Agradecemos sua contribuição."
- Atualização: "Seu report está sendo revisado."
- Resolução: "Seu report foi revisado. Ação tomada: [ação]"

---

### Jornada: Interação Social (Like, Comentário, Compartilhamento)

**Objetivo**: Engajar com conteúdo da comunidade

**Papéis**: VISITOR (apenas PUBLIC), RESIDENT (PUBLIC + RESIDENTS_ONLY)

**Fluxo**:
1. Usuário visualiza post/evento
2. Interage (like, comentário, compartilhamento)
3. Autor recebe notificação
4. Gamificação: Pontos atribuídos (se habilitada)
5. Feed atualiza em tempo real (se WebSocket habilitado)

**Microinterações**:
- Like: Animação de coração (scale + bounce)
- Comentário: Input expande, teclado abre
- Compartilhamento: Bottom sheet com opções

---

### Jornada: Notificações e Comunicação

**Objetivo**: Receber atualizações relevantes do território

**Papéis**: Todos

**Fluxo**:
1. Evento ocorre (post criado, evento próximo, report resolvido)
2. Sistema gera notificação no Outbox
3. Worker processa e cria notificação no Inbox
4. Notificação push enviada (se preferências habilitadas)
5. Badge aparece no ícone de notificações
6. Usuário toca na notificação
7. Abre tela de notificações ou conteúdo relacionado
8. Marca como lida

**Tipos de Notificação**:
- Posts: Novo post no território, comentário no seu post
- Eventos: Evento próximo, inscrição no seu evento
- Moderação: Report resolvido, sanção aplicada
- Membros: Solicitação de residência aprovada/rejeitada
- Marketplace: Nova mensagem, interesse em item

---

## 📊 Mapa Visual de Jornadas

### Fase 0: Onboarding e Primeiro Acesso
```
Download → Splash → Onboarding → Localização → Autenticação Social → 
Descoberta de Territórios → Seleção de Território → Entrada como VISITOR → 
Feed do Território (Primeira Vez)
```

### Fase 1: Exploração (VISITOR)
```
Feed Público ↔ Mapa Territorial ↔ Eventos Públicos ↔ Busca ↔ 
Perfis de Outros Usuários → Solicitar Residência → Aguardar Aprovação
```

### Fase 2: Participação Ativa (RESIDENT)
```
Criar Post → Participar Evento → Criar Evento → Usar Marketplace → 
Participar Votações → Chat → Alertas → Assets
```

### Fase 3: Governança (CURATOR)
```
Dashboard de Governança → Aprovar Residências → Validar Assets → 
Criar Votações → Configurar Feature Flags → Work Queue
```

### Fase 4: Moderação (MODERATOR)
```
Dashboard de Moderação → Revisar Reports → Aplicar Sanções → 
Bloquear Usuários → Ocultar Conteúdo
```

### Fase 5: Administração (SYSTEM_ADMIN)
```
Dashboard Administrativo → Gerenciar Territórios → 
Gerenciar Usuários → Configurar Sistema → Work Queue Global
```

---

## ⚠️ Pontos de Fricção e Otimização

### 1. Onboarding Longo
**Problema**: Muitos passos até primeira interação
**Solução**: 
- Reduzir passos obrigatórios (permitir pular onboarding)
- Mostrar conteúdo público mesmo sem cadastro (futuro)
- Onboarding progressivo (não tudo de uma vez)

### 2. Solicitação de Residência Complexa
**Problema**: Muitos passos para solicitar residência
**Solução**:
- Simplificar fluxo (localização + mensagem opcional)
- Upload de documento opcional (reduz fricção)
- Preview de benefícios antes de solicitar

### 3. Feed Lento
**Problema**: Carregamento lento do feed
**Solução**:
- Cache local (hive) para posts recentes
- Paginação infinita otimizada
- Skeleton loaders durante carregamento

### 4. Notificações Excessivas
**Problema**: Muitas notificações podem incomodar
**Solução**:
- Preferências granulares por tipo
- Agrupamento de notificações (batching)
- DND (Do Not Disturb) por horário

### 5. Mapa com Muitos Pins
**Problema**: Clutter visual no mapa
**Solução**:
- Clustering automático de pins próximos
- Filtros por tipo (posts, eventos, assets)
- Zoom automático para conteúdo relevante

---

## 📚 Análise de Documentação Existente

### Documentação Já Criada

#### 1. Visão e Produto ✅
- **`01_PRODUCT_VISION.md`**: Visão geral e princípios fundamentais
- **`02_ROADMAP.md`**: Planejamento de funcionalidades e épicos
- **`03_BACKLOG.md`**: Lista de funcionalidades e prioridades
- **`04_USER_STORIES.md`**: Histórias de usuário consolidadas
- **`05_GLOSSARY.md`**: Termos e conceitos do projeto

**Status**: Completo e bem estruturado
**Gaps Identificados**: Nenhum crítico

---

#### 2. Arquitetura e Design ✅
- **`10_ARCHITECTURE_DECISIONS.md`**: ADRs importantes (ADR-001 a ADR-009)
- **`11_ARCHITECTURE_SERVICES.md`**: Documentação dos services
- **`12_DOMAIN_MODEL.md`**: Modelo de entidades e relacionamentos (MER)
- **`13_DOMAIN_ROUTING.md`**: Roteamento e organização de domínios

**Status**: Completo e bem documentado
**Gaps Identificados**: Nenhum crítico

---

#### 3. Desenvolvimento e Implementação ✅
- **`20_IMPLEMENTATION_PLAN.md`**: Prioridades e dependências
- **`21_CODE_REVIEW.md`**: Análise de gaps e inconsistências
- **`22_COHESION_AND_TESTS.md`**: Avaliação de coesão e testes
- **`23_IMPLEMENTATION_RECOMMENDATIONS.md`**: Status das recomendações

**Status**: Completo
**Gaps Identificados**: Nenhum crítico

---

#### 4. Frontend Flutter ✅
- **`24_FLUTTER_FRONTEND_PLAN.md`**: Planejamento completo do app mobile (stack, arquitetura, funcionalidades, UX/UI)
- **`25_FLUTTER_IMPLEMENTATION_ROADMAP.md`**: Roadmap extensivo detalhado por fases, sincronizado com API até Fase 28
- **`26_FLUTTER_DESIGN_GUIDELINES.md`**: Diretrizes high-end profissionais de design (cores, formas, transições, efeitos)
- **`27_USER_JOURNEYS_MAP.md`** ⭐ (este documento): Mapa completo de jornadas do usuário

**Status**: Completo e muito detalhado
**Gaps Identificados**: 
- ⚠️ Falta documentação de testes de UI (Widget Tests, Integration Tests)
- ⚠️ Falta documentação de acessibilidade específica (WCAG, screen readers)
- ⚠️ Falta documentação de internacionalização (i18n) detalhada

---

#### 5. Operações e Governança ✅
- **`30_MODERATION.md`**: Sistema de moderação e reports
- **`31_ADMIN_OBSERVABILITY.md`**: Administração e observabilidade
- **`32_TRACEABILITY.md`**: Rastreabilidade de requisitos
- **`33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md`**: System Config, Work Queue, Evidências

**Status**: Completo
**Gaps Identificados**: Nenhum crítico

---

#### 6. API e Funcionalidades ✅
- **`60_API_LÓGICA_NEGÓCIO.md`**: Documento completo de lógica de negócio e usabilidade de todas as operações
- **`61_USER_PREFERENCES_PLAN.md`**: Planejamento de preferências de privacidade

**Status**: Completo e muito detalhado
**Gaps Identificados**: 
- ⚠️ Falta documentação de webhooks (se houver)
- ⚠️ Falta documentação de rate limiting detalhada por endpoint

---

#### 7. Backlog API ✅
- **`backlog-api/README.md`**: Índice completo do backlog API
- **`backlog-api/FASE*.md`**: Documentação detalhada de cada fase (1-28)

**Status**: Completo
**Gaps Identificados**: Nenhum crítico

---

#### 8. Segurança ✅
- **`SECURITY_CONFIGURATION.md`**: Guia completo de configuração de segurança
- **`SECURITY_AUDIT.md`**: Checklist de segurança e penetration testing

**Status**: Completo
**Gaps Identificados**: Nenhum crítico

---

#### 9. Produção e Deploy ✅
- **`50_PRODUCAO_AVALIACAO_COMPLETA.md`**: Análise de prontidão para produção
- **`51_PRODUCAO_PLANO_DESEJAVEIS.md`**: Plano de requisitos desejáveis

**Status**: Completo
**Gaps Identificados**: Nenhum crítico

---

### Documentação Faltante ou Incompleta

#### 1. Testes de UI Flutter ⚠️
**O que falta**:
- Documentação de Widget Tests (testes unitários de componentes)
- Documentação de Integration Tests (testes end-to-end)
- Documentação de Golden Tests (testes visuais)
- Estratégia de testes de acessibilidade

**Prioridade**: Alta (P1)
**Recomendação**: Criar `28_FLUTTER_TESTING_STRATEGY.md`

---

#### 2. Internacionalização (i18n) Detalhada ⚠️
**O que falta**:
- Mapeamento completo de strings traduzíveis
- Guia de adição de novos idiomas
- Estratégia de formatação de datas/números por região
- Testes de layouts RTL (Right-to-Left) se necessário

**Prioridade**: Média (P2)
**Recomendação**: Expandir seção em `24_FLUTTER_FRONTEND_PLAN.md` ou criar `29_FLUTTER_I18N_GUIDE.md`

---

#### 3. Webhooks (se houver) ⚠️
**O que falta**:
- Documentação de webhooks disponíveis
- Guia de configuração de endpoints
- Documentação de payloads e assinaturas

**Prioridade**: Baixa (P3, apenas se houver webhooks)
**Recomendação**: Criar `62_WEBHOOKS_API.md` (se necessário)

---

#### 4. Rate Limiting Detalhado ⚠️
**O que falta**:
- Limites específicos por endpoint
- Estratégias de retry recomendadas
- Tratamento de `429 Too Many Requests` detalhado

**Prioridade**: Média (P2)
**Recomendação**: Expandir seção em `60_API_LÓGICA_NEGÓCIO.md`

---

#### 5. Acessibilidade Específica (WCAG, Screen Readers) ⚠️
**O que falta**:
- Checklist WCAG AA completo
- Guia de testes com screen readers (TalkBack, VoiceOver)
- Documentação de labels semânticos
- Guia de navegação por teclado (se aplicável)

**Prioridade**: Alta (P1)
**Recomendação**: Criar `28_FLUTTER_ACCESSIBILITY_GUIDE.md`

---

### Documentação Excelente e Completíssima ✅

1. **`24_FLUTTER_FRONTEND_PLAN.md`**: Planejamento extremamente detalhado do frontend
2. **`25_FLUTTER_IMPLEMENTATION_ROADMAP.md`**: Roadmap sincronizado com backend até Fase 28
3. **`26_FLUTTER_DESIGN_GUIDELINES.md`**: Diretrizes de design profissionais e completas
4. **`60_API_LÓGICA_NEGÓCIO.md`**: Documentação completa de lógica de negócio
5. **`backlog-api/FASE*.md`**: Documentação detalhada de cada fase do backend

---

## 🎯 Recomendações para Desenvolvimento

### 1. Documentação Prioritária a Criar

#### Alta Prioridade (P1)
1. **`28_FLUTTER_TESTING_STRATEGY.md`**
   - Widget Tests
   - Integration Tests
   - Golden Tests
   - Testes de acessibilidade

2. **`28_FLUTTER_ACCESSIBILITY_GUIDE.md`**
   - Checklist WCAG AA
   - Guia de testes com screen readers
   - Labels semânticos
   - Navegação por teclado

#### Média Prioridade (P2)
3. **`29_FLUTTER_I18N_GUIDE.md`** (ou expandir seção em `24_FLUTTER_FRONTEND_PLAN.md`)
   - Mapeamento de strings
   - Guia de adição de idiomas
   - Formatação por região

4. **Expansão de `60_API_LÓGICA_NEGÓCIO.md`**
   - Rate limiting detalhado por endpoint
   - Estratégias de retry

#### Baixa Prioridade (P3)
5. **`62_WEBHOOKS_API.md`** (apenas se houver webhooks)

---

### 2. Melhorias em Documentação Existente

#### `24_FLUTTER_FRONTEND_PLAN.md`
- ✅ Já muito completo
- ⚠️ Adicionar seção de testes de UI
- ⚠️ Expandir seção de i18n

#### `27_USER_JOURNEYS_MAP.md` (este documento)
- ✅ Narrativa de acesso inicial completa
- ✅ Jornadas principais de VISITOR e RESIDENT detalhadas
- ⚠️ Adicionar jornadas detalhadas de CURATOR, MODERATOR, EVENT_ORGANIZER, SYSTEM_ADMIN (se necessário)
- ⚠️ Adicionar diagramas visuais (mermaid ou similar)

---

### 3. Ferramentas e Automação

#### Geração Automática de Documentação
1. **OpenAPI → Flutter Models**: Já mencionado, mas pode ser documentado
2. **Código → Diagramas**: Gerar diagramas de arquitetura automaticamente
3. **Testes → Coverage Reports**: Integrar cobertura de testes na documentação

#### Validação de Documentação
1. **Links quebrados**: Script para validar links internos
2. **Imagens faltantes**: Script para validar referências de imagens
3. **Consistência de nomenclatura**: Validar termos do glossário

---

### 4. Organização e Navegação

#### Índice Principal
- ✅ `00_INDEX.md` já existe e está completo
- ⚠️ Adicionar link para `27_USER_JOURNEYS_MAP.md`

#### Busca na Documentação
- ⚠️ Considerar adicionar mecanismo de busca (se houver muitos arquivos)
- ⚠️ Tags/categorias para facilitar busca

---

### 5. Documentação Visual

#### Diagramas
- ✅ Já existem alguns diagramas (C4, MER)
- ⚠️ Adicionar diagramas de fluxo de jornadas do usuário (mermaid)
- ⚠️ Adicionar wireframes/screenshots das principais telas

#### Vídeos/Tutoriais (Futuro)
- ⚠️ Considerar tutoriais em vídeo para onboarding
- ⚠️ GIFs animados para demonstrações de interações

---

## 📊 Resumo Executivo

### Documentação Existente: 95% Completa ✅

A documentação do projeto Arah é **extremamente completa e bem estruturada**. Os principais documentos cobrem:

1. ✅ Visão do produto e roadmap
2. ✅ Arquitetura e decisões técnicas
3. ✅ Planejamento completo do frontend Flutter
4. ✅ Roadmap de implementação sincronizado
5. ✅ Diretrizes de design profissionais
6. ✅ Lógica de negócio completa da API
7. ✅ Backlog detalhado por fases (1-28)
8. ✅ Segurança, moderação, operações

### Gaps Identificados: 5% ⚠️

Faltam apenas alguns documentos complementares:

1. ⚠️ Testes de UI Flutter (alta prioridade)
2. ⚠️ Acessibilidade específica (alta prioridade)
3. ⚠️ i18n detalhada (média prioridade)
4. ⚠️ Rate limiting detalhado (média prioridade)

### Próximos Passos Recomendados

1. **Criar `28_FLUTTER_TESTING_STRATEGY.md`** (alta prioridade)
2. **Criar `28_FLUTTER_ACCESSIBILITY_GUIDE.md`** (alta prioridade)
3. **Atualizar `00_INDEX.md`** com link para `27_USER_JOURNEYS_MAP.md`
4. **Expandir seção de i18n** em `24_FLUTTER_FRONTEND_PLAN.md` (média prioridade)

---

## 🎉 Conclusão

Este documento (`27_USER_JOURNEYS_MAP.md`) complementa perfeitamente a documentação existente, fornecendo:

- ✅ Narrativa completa de acesso inicial
- ✅ Jornadas detalhadas por papel (VISITOR, RESIDENT, etc.)
- ✅ Fluxos passo a passo com UI/UX esperada
- ✅ Pontos de fricção e otimizações
- ✅ Mapa visual de jornadas
- ✅ Análise completa da documentação existente
- ✅ Recomendações para desenvolvimento

A documentação do projeto Arah está **pronta para suportar o desenvolvimento completo do app Flutter**, com apenas algumas melhorias complementares recomendadas.

---

**Versão**: 1.0  
**Última Atualização**: 2025-01-20  
**Autor**: Sistema de Documentação Arah

---

## 📚 Referências Relacionadas

### Documentação Complementar do Projeto
- **[Planejamento do Frontend Flutter](./24_FLUTTER_FRONTEND_PLAN.md)** - Arquitetura e funcionalidades detalhadas
- **[Diretrizes de Design](./26_FLUTTER_DESIGN_GUIDELINES.md)** 🎨 - Design system completo usado nas jornadas
- **[Guia de Acessibilidade](./31_FLUTTER_ACCESSIBILITY_GUIDE.md)** ♿ - Como tornar as jornadas acessíveis
- **[Estratégia de Testes](./30_FLUTTER_TESTING_STRATEGY.md)** 🧪 - Testes de fluxos de usuário (integration tests)
- **[Prompt Avançado](./29_FLUTTER_ADVANCED_PROMPT.md)** 🚀 - Prompt consolidado para implementação das jornadas
