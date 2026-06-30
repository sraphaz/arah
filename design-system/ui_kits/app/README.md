# Arah App — UI Kit (premium v2)

Recriação de alta fidelidade do **app mobile da Arah**, reinterpretada numa versão **premium, moderna e minimalista**, fiel às features e diretrizes do produto real (Flutter + Material 3, tema escuro, verde copa, território-primeiro) — e estendida com as jornadas e funcionalidades pedidas para o lançamento.

Abra **`index.html`** — protótipo interativo navegável dentro de um frame iPhone.

## Conteúdo por território
Todo o conteúdo é **escopado ao território ativo** (`ARAH.getContent(territoryId)`): ao trocar de Camburi para Maresias ou Boiçucanga, mudam o **feed**, **eventos**, **entidades do mapa**, **saúde do território** e o número de moradores. Territórios sem conteúdo próprio (Juquehy, Barra do Una) usam um fallback de "comunidade em formação".

## Jornadas por papel (use "Ver como" no Perfil)
A experiência se adapta ao papel do usuário:
- **Visitante** — vê o feed público, é convidado a confirmar residência, não vota nem gere.
- **Morador** — participa: posta, comenta, confirma presença em eventos, vota em eleições, vê posts "só moradores".
- **Curador** — morador + **Gestão do território** (fila de curadoria: aprovar entidades do mapa, revisar posts sinalizados, confirmar residências; abrir eleições).

Troque de papel em **Perfil → "Ver como (demonstração de jornada)"**.

## Fluxo
`Login → Onboarding (geolocalização real → mapa → territórios próximos) → MainShell`

Bottom-nav (5 abas): **Feed · Explorar · Publicar · Serviços · Perfil**.
Topbar território-primeiro com **logo Arah**, **Mensagens** (chat) e **Notificações**.

**Serviços do território** é o hub que pré-visualiza **todo o backlog** (48 fases / 10 ondas), organizado em 4 categorias com selo de status (**No ar** vs **Em breve** + número da fase):
- **Economia local:** Mercado · Compra coletiva · Hospedagem · Demandas & ofertas · Trocas · Entregas · Moeda territorial (carteira)
- **Serviços territoriais:** Babás · Bem-estar (espaços + profissionais) · Aluguéis · Hub digital
- **Governança:** Eleições & votação · Gestão & curadoria · Moderação · Assinaturas
- **Vida no território:** Saúde do território · Painel de métricas · Banco de sementes · Aprendizado · Assistente IA · Conquistas

**Organização:** as ferramentas próprias do território também aparecem em **Explorar** (escopadas ao território ativo); o **Perfil** guarda o que é do usuário (papel, estatísticas, Minha loja, Mensagens, Configurações).

## Telas
| Área | Conteúdo |
|---|---|
| **Feed** | Filtros, banner de visitante, posts com selo morador/visitante, alertas, **fotos**, curtir; toque abre o **detalhe com comentários**. |
| **Publicar** | Título, conteúdo, **anexar foto** (upload real), tipo, visibilidade. |
| **Explorar + Mapa** | Lista de territórios; **mapa** com tiles OSM, **geolocalização real**, entidades (lugares, trilhas, cachoeiras, **nascentes**, **mirantes**, alertas), toque no pin abre detalhe + validar/curadoria. |
| **Mercado** | **Mercado local** + **Compra coletiva** (barra de progresso, participar). |
| **Mensagens** | Lista de conversas + thread com envio. |
| **Notificações** | Lista; cada item abre a tela relacionada (post, evento, mapa). |
| **Perfil** | Identidade, papéis, **seletor de jornada**, estatísticas, **grade de ferramentas**, interesses. |
| **Eventos** | Participar / cancelar presença. |
| **Saúde do território** | Indicadores reais: água potável, árvores nativas, nascentes, mirantes, santuários. |
| **Eleições** | Votação (candidatos, voto, resultados) — governança comunitária. |
| **Minha loja** | Abrir loja, vender produtos/serviços, **configuração de pagamento** (PIX, formas, taxa, repasse). |
| **Gestão** (curador) | Fila de curadoria, estatísticas, abrir eleição. |
| **Configurações** | Conta, preferências, privacidade. |

## Experiência real (ações que persistem na sessão)
O app reage de verdade dentro da sessão (sem backend):
- **Publicar post** → o post entra no topo do feed do território na hora.
- **Adicionar produto** (Minha loja) → o item aparece na lista de produtos.
- **Carteira** → **Adicionar** e **Enviar** Aratá mudam o saldo e registram a transação; **Receber** mostra QR + chave.
- **Conversar** (babá / prestador) → abre uma conversa real no chat, onde dá pra enviar mensagens.
- **Participar de evento / votar / entrar em compra coletiva** → a tela de origem reflete o resultado (presença, voto, participação) via `ctx.onConfirm`.

Pontes globais expostas pelo orquestrador: `window.openJourney(id, ctx)`, `window.appNav.{goTab,push,openChat}`, `window.arahMutate(fn)` (muta `window.ARAH` e re-renderiza), `window.arahToast(msg, icon)`.

## Transições
Troca de tela com **transição suave** (escurecer → surgir): fade + leve elevação, `cubic-bezier(.16,1,.3,1)`, respeitando `prefers-reduced-motion`.

## Jornadas navegáveis (fluxos de múltiplos passos)
Cada funcionalidade principal tem sua **jornada navegável** — um fluxo de passos com progresso, revisão e tela de sucesso (framework em `screens14.jsx`, fluxos em `screens15–16.jsx`):

| Jornada | Passos | Gatilho |
|---|---|---|
| **Reservar hospedagem** | datas → hóspedes → PIX → ✓ | Serviços → Hospedagem → Reservar |
| **Contratar babá** | dia/horário → detalhes → revisão → ✓ | Serviços → Babás → Solicitar |
| **Agendar bem-estar** | serviço/horário → confirmar → ✓ | Serviços → Bem-estar → Ver agenda |
| **Comprar no mercado** | sacola → entrega → PIX → ✓ | Mercado → item / carrinho |
| **Enviar moeda (Aratá)** | valor → destinatário → confirmar → ✓ | Carteira → Enviar |
| **Assinar plano** | plano → PIX → ✓ | Assinaturas → Apoiar |
| **Confirmar residência** | presença → comprovante → enviar → ✓ (vira morador) | Banner do feed / Perfil |
| **Demanda / oferta** | tipo/detalhes → publicar → ✓ | Demandas & ofertas → Publicar |
| **Doar semente** | espécie/qtd → doar → ✓ | Banco de sementes → Doar |
| **Propor troca** | ofereço/quero → publicar → ✓ | Trocas → Propor |
| **Matricular em oficina** | revisão → confirmar → ✓ | Aprendizado → curso |

Jornadas adicionais (em `screens18.jsx`): **votar em eleição** (candidato → confirmar → protocolo), **entrar em compra coletiva** (unidades → garantia → PIX), **adicionar produto à loja** (detalhes → foto → publicar), **receber Aratá** (QR + chave), **adicionar Aratá** (valor → PIX), **participar de evento** (acompanhantes + agenda), **abrir eleição** (curador: tipo → regras → abrir). Também em `screens17.jsx`: **alugar** e **solicitar entrega**.

Qualquer botão abre a jornada via `window.openJourney(id, ctx)`; o `JourneyHost` (em `screens16.jsx`) roteia para o fluxo certo, renderizado como overlay full-screen dentro do device. O `ctx.onConfirm` permite que a tela de origem reaja ao sucesso (ex.: marcar presença, contabilizar voto).

## O que é fiel ao código vs. proposto
- **Fiel ao repositório** (`sraphaz/arah`): feed territorial, posts (General/Alert, Public/ResidentsOnly), eventos, explorar/territórios, mapa com entidades (lugares/trilhas/nascentes/mirantes), notificações, perfil, **morador vs visitante** (controle de acesso), **curadoria/moderação comunitária**, saúde do território, interações (curtidas/comentários).
- **Proposto / net-new (sinalizado no app com selo "Em breve" + número da fase)**: Compra coletiva, Hospedagem, Demandas & ofertas, Trocas, Entregas, Moeda territorial, Babás, Bem-estar, Aluguéis, Hub digital, Assinaturas, Saúde do território, Painel de métricas, Banco de sementes, Aprendizado, Assistente IA, Conquistas. São **pré-visualizações visuais do backlog** (docs/02_ROADMAP.md, docs/03_BACKLOG.md), não código implementado.

## Arquivos
| Arquivo | Papel |
|---|---|
| `index.html` | Entrada: fontes, Material Symbols, ordem dos scripts, keyframes de transição |
| `ios-frame.jsx` | Frame iPhone (starter) — só o bezel escuro |
| `data.jsx` | Conteúdo mockado pt-BR (territórios, feed, comentários, eventos, mercado, chat, entidades, saúde, papéis) |
| `components.jsx` | Tokens `T` (com degradês de profundidade), `Icon`, `Logo`, `Avatar`, `RoleBadge`, `Btn`, `Chip`, `Card`, `Eyebrow` |
| `chrome.jsx` | `TopBar` (logo + chat + notif) + `BottomNav` (Mercado) |
| `screens1.jsx` | Feed, PostCard, PostDetail (comentários), Explore |
| `screens2.jsx` | CreatePost (com foto), Notifications |
| `screens3.jsx` | Profile (seletor de papel), Events (participar/cancelar), Health, Settings |
| `screens4.jsx` | Map (geolocalização + entidades), Login, Onboarding, TerritorySheet, Sheet |
| `screens5.jsx` | Mercado (marketplace + compra coletiva), Chat (lista + thread) |
| `screens6.jsx` | Eleições/votação, Gestão do território (curador) |
| `screens7.jsx` | Minha loja (vender + configuração de pagamento) |
| `screens8.jsx` | **Serviços hub** (diretório do backlog completo por categoria + status) |
| `screens9.jsx` | Compra coletiva, Hospedagem, Demandas & ofertas |
| `screens10.jsx` | Trocas, Entregas, Carteira / Moeda territorial |
| `screens11.jsx` | Babás, Bem-estar (espaços + profissionais) |
| `screens12.jsx` | Painel de métricas, Banco de sementes, Aprendizado |
| `screens13.jsx` | Assistente IA, Conquistas, Assinaturas |
| `screens14.jsx` | **Framework de jornada** (JourneyShell, SuccessView, campos, PIX) |
| `screens15.jsx` | Jornadas: Reserva, Babá, Bem-estar, Checkout |
| `screens16.jsx` | Jornadas: Carteira, Assinatura, Residência, Demanda/Troca/Semente, Curso + JourneyHost |
| `app.jsx` | Orquestrador: stages, abas, pilha de overlays, papéis, transições |

## Notas
- **Ícones** usam Material Symbols Rounded (ligaduras) — em capturas html-to-image aparecem como texto; no navegador real renderizam como glifos.
- **Mapa** carrega tiles OpenStreetMap (requer rede) e tenta `navigator.geolocation` (requer permissão; faz fallback para Camburi).
- Tokens completos em `../../colors_and_type.css`.
