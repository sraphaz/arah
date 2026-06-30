// data.jsx — Arah mock content. Território-primeiro, comunidade-primeiro. pt-BR.
// v3: content scoped PER TERRITORY (feed, events, map entities, health, members),
//     comments, photo posts, marketplace, compra coletiva, chat, store, roles.

const ARAH = {
  me: { name: 'Ana Ribeiro', handle: 'ana.ribeiro', avatar: '#4F956F', role: 'visitante' },

  territories: [
    { id: 't1', name: 'Camburi', region: 'São Sebastião · SP', members: 1284, distance: 0.4, desc: 'Vila caiçara entre a mata e o mar.', active: true },
    { id: 't2', name: 'Maresias', region: 'São Sebastião · SP', members: 2106, distance: 6.2, desc: 'Praia, surfe e comércio local.' },
    { id: 't3', name: 'Boiçucanga', region: 'São Sebastião · SP', members: 1740, distance: 9.1, desc: 'Comunidade ativa e feira de domingo.' },
    { id: 't4', name: 'Juquehy', region: 'São Sebastião · SP', members: 980, distance: 12.7, desc: 'Rio, mangue e trilhas.' },
    { id: 't5', name: 'Barra do Una', region: 'São Sebastião · SP', members: 612, distance: 18.3, desc: 'Reserva e pesca artesanal.' },
  ],

  // ---- Per-territory content ----
  content: {
    t1: {
      tile: { x: 3057, y: 4652 },
      health: { agua: 92, nativas: 78, nascentes: 6, mirantes: 4, santuarios: 2 },
      feed: [
        { id: 'p1', author: 'Conselho de Moradores', handle: 'conselho.camburi', avatar: '#4F956F', role: 'morador', time: '2h', type: 'Alert', visibility: 'Public', pinned: true,
          title: 'Mutirão de limpeza no costão neste sábado', body: 'Encontro às 8h na praça da capela. Traga luvas e água. Vamos cuidar do nosso território juntos — café da manhã comunitário ao final.', likes: 64, comments: 12 },
        { id: 'p2', author: 'Dona Marta', handle: 'marta.peixaria', avatar: '#C9962B', role: 'morador', time: '5h', type: 'General', visibility: 'Public', photo: '../../assets/cover-rainforest.png',
          title: 'Chegou robalo fresco na peixaria', body: 'Pescado da manhã pelo seu Tião. Quem quiser, é só passar até as 18h. Apoiem a pesca artesanal da vila 🐟', likes: 38, comments: 7 },
        { id: 'p3', author: 'Coletivo Maré', handle: 'coletivo.mare', avatar: '#2A6FDB', role: 'morador', time: '1d', type: 'General', visibility: 'ResidentsOnly',
          title: 'Roda de conversa: água e saneamento', body: 'Próxima terça, 19h, no centro comunitário. Vamos discutir a captação da nascente e os próximos passos com a associação.', likes: 91, comments: 28 },
        { id: 'p4', author: 'Bruno Caiçara', handle: 'bruno.trilhas', avatar: '#377B57', role: 'visitante', time: '2d', type: 'General', visibility: 'Public',
          title: 'Trilha da Cachoeira reaberta', body: 'Galho caído foi removido pelo mutirão. Caminho liberado, mas atenção no trecho de pedra após a chuva.', likes: 47, comments: 5 },
      ],
      events: [
        { id: 'e1', day: '07', mon: 'JUN', title: 'Mutirão de limpeza do costão', time: 'Sáb · 08:00', place: 'Praça da Capela', going: 42, tag: 'Comunidade', status: 'interested', desc: 'Limpeza coletiva do costão e da faixa de areia. Traga luvas e garrafa de água.' },
        { id: 'e2', day: '10', mon: 'JUN', title: 'Roda de conversa: água e saneamento', time: 'Ter · 19:00', place: 'Centro Comunitário', going: 28, tag: 'Assembleia', status: 'confirmed', desc: 'Discussão aberta sobre a captação da nascente e o saneamento da vila.' },
        { id: 'e3', day: '15', mon: 'JUN', title: 'Feira de produtores da vila', time: 'Dom · 09:00', place: 'Largo da Igreja', going: 130, tag: 'Feira', desc: 'Produtores locais, agroecologia e artesanato caiçara.' },
      ],
      entities: [
        { id: 'm_peixaria', x: 38, y: 38, kind: 'place', icon: 'storefront', color: '#A6D6B9', label: 'Peixaria da Marta', sub: 'Comércio local · aberto', confirmed: true },
        { id: 'm_evento', x: 60, y: 30, kind: 'event', icon: 'event', color: '#86AEEA', label: 'Mutirão do costão', sub: 'Sáb · 08:00', confirmed: true },
        { id: 'm_alerta', x: 30, y: 62, kind: 'alert', icon: 'warning', color: '#E8A06A', label: 'Trecho escorregadio', sub: 'Trilha · após a chuva', confirmed: true },
        { id: 'm_cachoeira', x: 70, y: 60, kind: 'waterfall', icon: 'water_drop', color: '#6FC5D6', label: 'Cachoeira do Bracuí', sub: 'Banho · trilha 40min', confirmed: true },
        { id: 'm_nascente', x: 52, y: 72, kind: 'spring', icon: 'water', color: '#6FC5D6', label: 'Nascente do Bracuí', sub: 'Água potável · confirmada', confirmed: true },
        { id: 'm_mirante', x: 46, y: 24, kind: 'viewpoint', icon: 'landscape', color: '#A6D6B9', label: 'Mirante da Pedra', sub: 'Vista 360° · trilha 20min', confirmed: false },
      ],
    },
    t2: {
      tile: { x: 3059, y: 4653 },
      health: { agua: 74, nativas: 52, nascentes: 3, mirantes: 5, santuarios: 1 },
      feed: [
        { id: 'm2p1', author: 'Escola de Surfe Maresias', handle: 'surf.maresias', avatar: '#2A6FDB', role: 'morador', time: '1h', type: 'General', visibility: 'Public', photo: '../../assets/cover-rainforest.png',
          title: 'Aula aberta de surfe neste domingo', body: 'Aula gratuita pra moradores às 7h, antes do movimento. Pranchas emprestadas. Bora pegar onda com responsabilidade 🌊', likes: 112, comments: 19 },
        { id: 'm2p2', author: 'Defesa Civil', handle: 'defesa.maresias', avatar: '#E8A06A', role: 'morador', time: '3h', type: 'Alert', visibility: 'Public', pinned: true,
          title: 'Trânsito intenso na entrada da praia', body: 'Feriado prolongado lotou a Rio-Santos. Evitem a rua da praia entre 10h e 14h. Usem o estacionamento do largo.', likes: 58, comments: 8 },
        { id: 'm2p3', author: 'Restaurante da Praia', handle: 'restaurante.maresias', avatar: '#C9962B', role: 'morador', time: '6h', type: 'General', visibility: 'Public',
          title: 'Menu caiçara de inverno', body: 'Caldo de peixe, moqueca e pastel de camarão. Moradores têm 15% no almoço durante a semana 🍲', likes: 44, comments: 6 },
        { id: 'm2p4', author: 'Coletivo Praia Limpa', handle: 'praialimpa', avatar: '#377B57', role: 'morador', time: '1d', type: 'General', visibility: 'ResidentsOnly',
          title: 'Coleta de guimbas na areia', body: 'Sábado de manhã. Vamos mapear os pontos mais sujos e instalar cinzeiros ecológicos na orla.', likes: 73, comments: 14 },
      ],
      events: [
        { id: 'm2e1', day: '08', mon: 'JUN', title: 'Campeonato de surfe amador', time: 'Dom · 07:00', place: 'Praia de Maresias', going: 214, tag: 'Esporte', desc: 'Etapa local do circuito caiçara. Categorias mirim, amador e master.' },
        { id: 'm2e2', day: '14', mon: 'JUN', title: 'Feira gastronômica da orla', time: 'Sáb · 17:00', place: 'Calçadão da Praia', going: 156, tag: 'Feira', status: 'interested', desc: 'Food trucks, produtores e música ao vivo.' },
      ],
      entities: [
        { id: 'm2_pico', x: 44, y: 42, kind: 'place', icon: 'surfing', color: '#86AEEA', label: 'Pico do Surfe', sub: 'Ondas · nível intermediário', confirmed: true },
        { id: 'm2_quiosque', x: 62, y: 36, kind: 'place', icon: 'storefront', color: '#A6D6B9', label: 'Quiosque do Zé', sub: 'Comércio · aberto', confirmed: true },
        { id: 'm2_alerta', x: 34, y: 58, kind: 'alert', icon: 'warning', color: '#E8A06A', label: 'Trânsito intenso', sub: 'Entrada da praia · 10h–14h', confirmed: true },
        { id: 'm2_mirante', x: 56, y: 66, kind: 'viewpoint', icon: 'landscape', color: '#A6D6B9', label: 'Mirante do Pontal', sub: 'Pôr do sol · trilha 15min', confirmed: true },
      ],
    },
    t3: {
      tile: { x: 3060, y: 4654 },
      health: { agua: 81, nativas: 69, nascentes: 4, mirantes: 2, santuarios: 3 },
      feed: [
        { id: 'm3p1', author: 'Associação de Boiçucanga', handle: 'assoc.boicucanga', avatar: '#4F956F', role: 'morador', time: '2h', type: 'General', visibility: 'Public', pinned: true,
          title: 'Feira de domingo confirmada', body: 'Das 8h às 13h no largo. Hortaliças, peixe, pães e artesanato. Tragam suas sacolas retornáveis 🧺', likes: 88, comments: 21 },
        { id: 'm3p2', author: 'Escola da Vila', handle: 'escola.boicucanga', avatar: '#C9962B', role: 'morador', time: '7h', type: 'General', visibility: 'Public',
          title: 'Aulas de reforço abertas', body: 'Voluntários dão reforço de matemática e leitura às quartas e sextas. Inscrições na secretaria.', likes: 52, comments: 9 },
        { id: 'm3p3', author: 'Defesa Civil', handle: 'defesa.boicucanga', avatar: '#E8A06A', role: 'morador', time: '1d', type: 'Alert', visibility: 'Public',
          title: 'Rio cheio após a chuva', body: 'Nível do rio subiu. Evitem a travessia na ponte velha até segunda. Usem a passarela nova.', likes: 41, comments: 4 },
      ],
      events: [
        { id: 'm3e1', day: '09', mon: 'JUN', title: 'Feira de domingo', time: 'Dom · 08:00', place: 'Largo da Vila', going: 190, tag: 'Feira', status: 'confirmed', desc: 'Feira semanal de produtores e artesãos de Boiçucanga.' },
        { id: 'm3e2', day: '16', mon: 'JUN', title: 'Plantio de mudas nativas', time: 'Sáb · 09:00', place: 'Margem do Rio', going: 64, tag: 'Comunidade', desc: 'Recuperação da mata ciliar com mudas do viveiro comunitário.' },
      ],
      entities: [
        { id: 'm3_feira', x: 48, y: 40, kind: 'event', icon: 'storefront', color: '#A6D6B9', label: 'Feira da Vila', sub: 'Dom · 08h–13h', confirmed: true },
        { id: 'm3_igreja', x: 60, y: 50, kind: 'place', icon: 'church', color: '#A6D6B9', label: 'Igreja Matriz', sub: 'Centro histórico', confirmed: true },
        { id: 'm3_rio', x: 36, y: 64, kind: 'alert', icon: 'warning', color: '#E8A06A', label: 'Ponte velha', sub: 'Rio cheio · evitar', confirmed: true },
        { id: 'm3_cachoeira', x: 68, y: 62, kind: 'waterfall', icon: 'water_drop', color: '#6FC5D6', label: 'Cachoeira do Sertão', sub: 'Trilha 50min', confirmed: false },
      ],
    },
  },

  // Fallback content for territories without bespoke data (t4, t5)
  fallbackContent: (t) => ({
    tile: { x: 3058, y: 4655 },
    health: { agua: 70, nativas: 60, nascentes: 2, mirantes: 1, santuarios: 1 },
    feed: [
      { id: t.id + 'fp1', author: 'Associação Local', handle: 'assoc.' + t.name.toLowerCase().replace(/\s/g, ''), avatar: '#4F956F', role: 'morador', time: '4h', type: 'General', visibility: 'Public',
        title: `Bem-vindo a ${t.name}`, body: `O território de ${t.name} ainda está se organizando na Arah. Seja um dos primeiros a publicar e fortalecer a comunidade local.`, likes: 12, comments: 2 },
      { id: t.id + 'fp2', author: 'Moradores de ' + t.name, handle: 'moradores', avatar: '#377B57', role: 'morador', time: '2d', type: 'General', visibility: 'Public',
        title: 'Grupo de WhatsApp migrando pra cá', body: 'Estamos trazendo os avisos da vila pro app. Em breve mais conteúdo do território por aqui.', likes: 8, comments: 1 },
    ],
    events: [
      { id: t.id + 'fe1', day: '21', mon: 'JUN', title: 'Encontro de moradores', time: 'Sáb · 16:00', place: 'Centro da Vila', going: 18, tag: 'Comunidade', desc: `Primeiro encontro de moradores de ${t.name} para organizar o território na Arah.` },
    ],
    entities: [
      { id: t.id + '_centro', x: 50, y: 46, kind: 'place', icon: 'place', color: '#A6D6B9', label: 'Centro da Vila', sub: 'Ponto de encontro', confirmed: true },
      { id: t.id + '_praia', x: 60, y: 60, kind: 'place', icon: 'beach_access', color: '#86AEEA', label: 'Praia', sub: 'Acesso público', confirmed: false },
    ],
  }),

  getContent(tid) {
    if (this.content[tid]) return this.content[tid];
    return this.fallbackContent(this.territories.find(t => t.id === tid));
  },

  comments: {
    p1: [
      { id: 'c1', author: 'Bruno Caiçara', avatar: '#377B57', role: 'visitante', time: '1h', body: 'Vou levar dois sacos extras e luvas. Bora!' },
      { id: 'c2', author: 'Dona Marta', avatar: '#C9962B', role: 'morador', time: '1h', body: 'Levo o café e os bolos da padaria 🙌' },
      { id: 'c3', author: 'Seu Tião', avatar: '#4F956F', role: 'morador', time: '40min', body: 'Confirmado. Levo o caminhão pra recolher o lixo pesado.' },
    ],
  },

  notifications: [
    { id: 'n1', kind: 'alert', read: false, link: 'post:p1', title: 'Novo alerta no seu território', body: 'Conselho de Moradores publicou um aviso sobre o mutirão de sábado.', time: 'há 2h' },
    { id: 'n2', kind: 'event', read: false, link: 'events', title: 'Lembrete de evento', body: 'Roda de conversa sobre água começa em 1 dia.', time: 'há 5h' },
    { id: 'n3', kind: 'map', read: false, link: 'map:m_nascente', title: 'Nova entidade no mapa', body: 'Nascente do Bracuí foi confirmada pela curadoria. Ver no mapa.', time: 'há 8h' },
    { id: 'n4', kind: 'connection', read: true, link: 'profile', title: 'Bruno Caiçara entrou em Camburi', body: 'Agora faz parte do seu território.', time: 'ontem' },
    { id: 'n5', kind: 'post', read: true, link: 'post:p4', title: 'Seu post recebeu 12 comentários', body: '“Trilha da Cachoeira reaberta” está movimentando a vila.', time: 'há 2 dias' },
  ],

  market: [
    { id: 'mk1', title: 'Robalo fresco (kg)', seller: 'Peixaria da Marta', price: 'R$ 42', tag: 'Alimento', avatar: '#C9962B', icon: 'set_meal' },
    { id: 'mk2', title: 'Passeio de canoa no mangue', seller: 'Bruno Caiçara', price: 'R$ 80', tag: 'Serviço', avatar: '#377B57', icon: 'kayaking' },
    { id: 'mk3', title: 'Cestas de orgânicos da feira', seller: 'Coletivo Maré', price: 'R$ 55', tag: 'Alimento', avatar: '#2A6FDB', icon: 'shopping_basket' },
    { id: 'mk4', title: 'Artesanato em fibra de bananeira', seller: 'Dona Lurdes', price: 'R$ 35', tag: 'Artesanato', avatar: '#4F956F', icon: 'redeem' },
  ],
  groupBuys: [
    { id: 'gb1', title: 'Compra coletiva de placas solares', org: 'Conselho de Moradores', goal: 30, joined: 22, unit: 'famílias', deadline: 'até 20/jun', icon: 'solar_power' },
    { id: 'gb2', title: 'Cisternas de captação de chuva', org: 'Coletivo Maré', goal: 15, joined: 9, unit: 'casas', deadline: 'até 30/jun', icon: 'water_drop' },
  ],

  // My store (net-new): seller products + payment config
  myStore: {
    open: false,
    name: 'Ateliê da Ana',
    products: [
      { id: 'sp1', title: 'Aulas de reforço (hora)', price: 'R$ 40', tag: 'Serviço', icon: 'school', active: true },
      { id: 'sp2', title: 'Doce de banana caseiro', price: 'R$ 18', tag: 'Alimento', icon: 'lunch_dining', active: true },
    ],
    sales: { month: 'R$ 1.240', orders: 31 },
    payment: { pix: 'ana.ribeiro@email.com', methods: ['PIX', 'Cartão', 'Dinheiro'], fee: '0%', payout: 'Banco do Brasil ····2207' },
  },

  chats: [
    { id: 'ch1', name: 'Conselho de Moradores', avatar: '#4F956F', role: 'morador', last: 'Confirmado o caminhão pro mutirão.', time: '10:24', unread: 2,
      thread: [
        { me: false, body: 'Oi Ana! Você confirma presença no mutirão de sábado?', time: '10:20' },
        { me: true, body: 'Confirmo sim! Posso levar luvas extras.', time: '10:22' },
        { me: false, body: 'Perfeito. Confirmado o caminhão pro mutirão.', time: '10:24' },
      ] },
    { id: 'ch2', name: 'Dona Marta', avatar: '#C9962B', role: 'morador', last: 'Separei o robalo, viu?', time: 'ontem', unread: 0,
      thread: [
        { me: true, body: 'Dona Marta, ainda tem robalo pra hoje?', time: 'ontem' },
        { me: false, body: 'Separei o robalo, viu? Pode passar até as 18h.', time: 'ontem' },
      ] },
    { id: 'ch3', name: 'Coletivo Maré', avatar: '#2A6FDB', role: 'morador', last: 'Te vejo na roda de conversa 💧', time: 'seg', unread: 0,
      thread: [{ me: false, body: 'Te vejo na roda de conversa 💧', time: 'seg' }] },
  ],

  profile: {
    name: 'Ana Ribeiro', handle: 'ana.ribeiro', avatar: '#4F956F',
    bio: 'Caiçara, professora e voluntária no coletivo de águas. Território é casa.',
    posts: 23, territories: 2, since: '2024',
    interests: ['Meio ambiente', 'Pesca artesanal', 'Educação', 'Cultura caiçara', 'Saneamento'],
  },
  interestPool: ['Meio ambiente', 'Pesca artesanal', 'Educação', 'Cultura caiçara', 'Saneamento', 'Turismo de base', 'Agroecologia', 'Saúde', 'Esporte', 'Artesanato', 'Música', 'Trilhas'],

  // ============================================================
  // BACKLOG DOMAINS — visual preview of the full roadmap (48 fases / 10 ondas)
  // status: 'live' (implemented MVP) | 'soon' (planned roadmap)
  // ============================================================
  serviceCategories: [
    { id: 'economia', label: 'Economia local', icon: 'payments', color: '#A6D6B9', desc: 'Mercado, compras coletivas e renda no território.',
      items: [
        { id: 'market', icon: 'storefront', label: 'Mercado', sub: 'Lojas, itens e checkout', status: 'live' },
        { id: 'groupbuy', icon: 'groups_2', label: 'Compra coletiva', sub: 'Unir pedidos da vila', status: 'soon', phase: 'Fase 17' },
        { id: 'hosting', icon: 'cottage', label: 'Hospedagem', sub: 'Estadias no território', status: 'soon', phase: 'Fase 18' },
        { id: 'demands', icon: 'swap_horiz', label: 'Demandas & ofertas', sub: 'Quem precisa, quem oferece', status: 'soon', phase: 'Fase 19' },
        { id: 'trades', icon: 'handshake', label: 'Trocas comunitárias', sub: 'Escambo e doações', status: 'soon', phase: 'Fase 20' },
        { id: 'delivery', icon: 'local_shipping', label: 'Entregas', sub: 'Logística local', status: 'soon', phase: 'Fase 21' },
        { id: 'wallet', icon: 'account_balance_wallet', label: 'Moeda territorial', sub: 'Carteira e créditos', status: 'soon', phase: 'Fase 22' },
      ] },
    { id: 'servicos', label: 'Serviços territoriais', icon: 'concierge', color: '#86AEEA', desc: 'Profissionais e espaços de confiança local.',
      items: [
        { id: 'sitters', icon: 'child_care', label: 'Babás', sub: 'Cuidado infantil verificado', status: 'soon', phase: 'Épico 10' },
        { id: 'wellness', icon: 'self_improvement', label: 'Bem-estar', sub: 'Yoga, terapias e espaços', status: 'soon', phase: 'Épico 10' },
        { id: 'rental', icon: 'category', label: 'Aluguéis', sub: 'Equipamentos e espaços', status: 'soon', phase: 'Fase 46' },
        { id: 'digital', icon: 'apps', label: 'Hub digital', sub: 'Serviços digitais locais', status: 'soon', phase: 'Fase 26' },
      ] },
    { id: 'governanca', label: 'Governança', icon: 'how_to_vote', color: '#E8A06A', desc: 'Decisões coletivas e cuidado comunitário.',
      items: [
        { id: 'elections', icon: 'how_to_vote', label: 'Eleições & votação', sub: 'Conselho e consultas', status: 'live' },
        { id: 'manage', icon: 'shield_person', label: 'Gestão & curadoria', sub: 'Moderação comunitária', status: 'live' },
        { id: 'moderation', icon: 'gavel', label: 'Moderação & denúncias', sub: 'Reports, bloqueios, sanções', status: 'live' },
        { id: 'subscriptions', icon: 'card_membership', label: 'Assinaturas', sub: 'Apoio recorrente ao território', status: 'soon', phase: 'Fase 15' },
      ] },
    { id: 'territorio', label: 'Vida no território', icon: 'eco', color: '#A6D6B9', desc: 'Saúde, cultura e regeneração do lugar.',
      items: [
        { id: 'health', icon: 'water_drop', label: 'Saúde do território', sub: 'Indicadores vivos', status: 'soon', phase: 'Fase 24' },
        { id: 'metrics', icon: 'monitoring', label: 'Painel de métricas', sub: 'Pulso da comunidade', status: 'soon', phase: 'Fase 25' },
        { id: 'seeds', icon: 'potted_plant', label: 'Banco de sementes', sub: 'Mudas e troca de sementes', status: 'soon', phase: 'Fase 48' },
        { id: 'learning', icon: 'school', label: 'Aprendizado', sub: 'Saberes e oficinas locais', status: 'soon', phase: 'Fase 45' },
        { id: 'ai', icon: 'auto_awesome', label: 'Assistente IA', sub: 'Apoio inteligente do território', status: 'soon', phase: 'Fase 27' },
        { id: 'achievements', icon: 'workspace_premium', label: 'Conquistas', sub: 'Reconhecimento comunitário', status: 'soon', phase: 'Fase 42' },
      ] },
  ],

  groupBuyDetail: {
    title: 'Compra coletiva de placas solares', org: 'Conselho de Moradores', icon: 'solar_power',
    goal: 30, joined: 22, unit: 'famílias', deadline: 'até 20/jun', price: 'R$ 2.400', saved: 'R$ 1.100',
    desc: 'Quanto mais famílias entram, menor o preço por placa. Negociação coletiva direto com o fornecedor regional.',
    tiers: [{ n: 10, price: 'R$ 3.500' }, { n: 20, price: 'R$ 2.400' }, { n: 30, price: 'R$ 1.900' }],
    participants: ['#C9962B', '#377B57', '#2A6FDB', '#4F956F', '#E8A06A'],
  },

  hosting: [
    { id: 'h1', title: 'Casa caiçara à beira-mar', host: 'Dona Marta', avatar: '#C9962B', price: 'R$ 180', unit: 'noite', rating: 4.9, reviews: 32, tag: 'Casa inteira', guests: 4, icon: 'cottage' },
    { id: 'h2', title: 'Quarto na vila, perto da trilha', host: 'Bruno Caiçara', avatar: '#377B57', price: 'R$ 90', unit: 'noite', rating: 4.8, reviews: 18, tag: 'Quarto privativo', guests: 2, icon: 'bedroom_parent' },
    { id: 'h3', title: 'Camping ecológico do rio', host: 'Coletivo Maré', avatar: '#2A6FDB', price: 'R$ 45', unit: 'noite', rating: 4.7, reviews: 41, tag: 'Camping', guests: 6, icon: 'camping' },
  ],

  demands: [
    { id: 'd1', kind: 'demanda', title: 'Preciso de pedreiro para muro', who: 'Seu Tião', avatar: '#4F956F', tag: 'Construção', time: '2h', icon: 'construction' },
    { id: 'd2', kind: 'oferta', title: 'Ofereço aulas de violão', who: 'Bruno Caiçara', avatar: '#377B57', tag: 'Educação', time: '4h', icon: 'music_note' },
    { id: 'd3', kind: 'demanda', title: 'Procuro carona para São Sebastião', who: 'Ana Ribeiro', avatar: '#4F956F', tag: 'Transporte', time: '6h', icon: 'directions_car' },
    { id: 'd4', kind: 'oferta', title: 'Conserto de redes de pesca', who: 'Dona Marta', avatar: '#C9962B', tag: 'Serviço', time: '1d', icon: 'phishing' },
  ],

  trades: [
    { id: 'tr1', title: 'Troco mudas de ipê por adubo', who: 'Coletivo Maré', avatar: '#2A6FDB', want: 'Adubo orgânico', icon: 'potted_plant' },
    { id: 'tr2', title: 'Doação: roupas infantis', who: 'Dona Lurdes', avatar: '#4F956F', want: 'Doação', icon: 'volunteer_activism' },
    { id: 'tr3', title: 'Troco peixe por hortaliças', who: 'Seu Tião', avatar: '#4F956F', want: 'Hortaliças', icon: 'set_meal' },
  ],

  deliveries: [
    { id: 'dl1', from: 'Peixaria da Marta', to: 'Praça da Capela', status: 'a caminho', courier: 'Bruno', eta: '15 min', icon: 'two_wheeler' },
    { id: 'dl2', from: 'Feira da Vila', to: 'Rua das Conchas, 12', status: 'entregue', courier: 'Ana', eta: 'concluído', icon: 'check_circle' },
  ],

  wallet: {
    balance: 'A̧ 340', brl: 'R$ 340,00', name: 'Aratá', symbol: 'A̧',
    transactions: [
      { id: 'w1', label: 'Feira da Vila', sub: 'Compra de orgânicos', val: '- A̧ 55', icon: 'shopping_basket', neg: true },
      { id: 'w2', label: 'Mutirão do costão', sub: 'Crédito por participação', val: '+ A̧ 30', icon: 'volunteer_activism', neg: false },
      { id: 'w3', label: 'Aulas de violão', sub: 'Recebido de Marina', val: '+ A̧ 80', icon: 'music_note', neg: false },
      { id: 'w4', label: 'Peixaria da Marta', sub: 'Robalo fresco', val: '- A̧ 42', icon: 'set_meal', neg: true },
    ],
  },

  sitters: [
    { id: 's1', name: 'Marina Souza', avatar: '#E8A06A', rating: 4.9, reviews: 27, price: 'R$ 35/h', exp: '8 anos', verified: true, badges: ['Primeiros socorros', 'Verificada'], dist: '0.6 km', ages: '0–6 anos' },
    { id: 's2', name: 'Júlia Caiçara', avatar: '#377B57', rating: 4.8, reviews: 14, price: 'R$ 30/h', exp: '5 anos', verified: true, badges: ['Curso de babá'], dist: '1.2 km', ages: '2–10 anos' },
    { id: 's3', name: 'Rosa Maria', avatar: '#C9962B', rating: 5.0, reviews: 41, price: 'R$ 40/h', exp: '12 anos', verified: true, badges: ['Primeiros socorros', 'Pedagogia'], dist: '2.0 km', ages: '0–12 anos' },
  ],

  wellness: {
    spaces: [
      { id: 'ws1', name: 'Espaço Maré Yoga', type: 'Estúdio de yoga', icon: 'self_improvement', avatar: '#A6D6B9', slots: '6 horários livres', cap: '12 pessoas' },
      { id: 'ws2', name: 'Casa Holística da Vila', type: 'Terapias integrativas', icon: 'spa', avatar: '#86AEEA', slots: '3 horários livres', cap: '8 pessoas' },
    ],
    providers: [
      { id: 'wp1', name: 'Clara Ventos', spec: 'Professora de Yoga', avatar: '#377B57', rating: 4.9, price: 'R$ 60', icon: 'self_improvement', verified: true },
      { id: 'wp2', name: 'Tomás Lua', spec: 'Terapia sonora', avatar: '#2A6FDB', rating: 4.8, price: 'R$ 90', icon: 'music_note', verified: true },
    ],
  },

  courses: [
    { id: 'co1', title: 'Pesca artesanal sustentável', teacher: 'Seu Tião', avatar: '#4F956F', lessons: 6, level: 'Iniciante', tag: 'Saberes locais', icon: 'phishing' },
    { id: 'co2', title: 'Agroecologia no quintal', teacher: 'Coletivo Maré', avatar: '#2A6FDB', lessons: 8, level: 'Todos', tag: 'Agroecologia', icon: 'eco' },
    { id: 'co3', title: 'Construção com bioconstrução', teacher: 'Bruno Caiçara', avatar: '#377B57', lessons: 5, level: 'Intermediário', tag: 'Ofícios', icon: 'handyman' },
  ],

  seeds: [
    { id: 'se1', name: 'Ipê-amarelo', who: 'Coletivo Maré', qty: '20 sementes', tag: 'Nativa', icon: 'park' },
    { id: 'se2', name: 'Manjericão', who: 'Dona Lurdes', qty: '15 mudas', tag: 'Tempero', icon: 'potted_plant' },
    { id: 'se3', name: 'Juçara (palmito)', who: 'Seu Tião', qty: '8 mudas', tag: 'Nativa', icon: 'forest' },
    { id: 'se4', name: 'Hortelã', who: 'Ana Ribeiro', qty: '12 mudas', tag: 'Medicinal', icon: 'eco' },
  ],

  achievements: [
    { id: 'a1', label: 'Guardião do território', sub: '5 mutirões', icon: 'shield', got: true },
    { id: 'a2', label: 'Semeador', sub: '10 mudas doadas', icon: 'potted_plant', got: true },
    { id: 'a3', label: 'Voz ativa', sub: 'Votou em 3 eleições', icon: 'how_to_vote', got: true },
    { id: 'a4', label: 'Vizinho presente', sub: '20 eventos', icon: 'groups', got: false, progress: 70 },
    { id: 'a5', label: 'Comerciante local', sub: 'Loja com 50 vendas', icon: 'storefront', got: false, progress: 62 },
    { id: 'a6', label: 'Mestre de ofício', sub: 'Ensinou 1 curso', icon: 'school', got: false, progress: 0 },
  ],

  subscriptionTiers: [
    { id: 'sub1', name: 'Apoiador', price: 'R$ 9', period: '/mês', perks: ['Selo de apoiador', 'Acesso a relatórios do território'], popular: false },
    { id: 'sub2', name: 'Guardião', price: 'R$ 29', period: '/mês', perks: ['Tudo do Apoiador', 'Prioridade em compras coletivas', 'Voz nas consultas'], popular: true },
    { id: 'sub3', name: 'Mantenedor', price: 'R$ 79', period: '/mês', perks: ['Tudo do Guardião', 'Reconhecimento público', 'Reunião mensal com o conselho'], popular: false },
  ],

  aiPrompts: ['O que está acontecendo em Camburi hoje?', 'Onde encontro peixe fresco agora?', 'Resumir a última assembleia', 'Quais trilhas estão abertas?'],

  metrics: {
    engagement: 78, posts: 142, events: 12, newMembers: 34,
    bars: [{ label: 'Seg', v: 40 }, { label: 'Ter', v: 65 }, { label: 'Qua', v: 52 }, { label: 'Qui', v: 80 }, { label: 'Sex', v: 72 }, { label: 'Sáb', v: 95 }, { label: 'Dom', v: 88 }],
  },

  rentals: [
    { id: 'rt1', title: 'Caiaque duplo', owner: 'Bruno Caiçara', avatar: '#377B57', price: 'R$ 40', unit: 'dia', tag: 'Lazer', icon: 'kayaking' },
    { id: 'rt2', title: 'Furadeira + kit', owner: 'Seu Tião', avatar: '#4F956F', price: 'R$ 25', unit: 'dia', tag: 'Ferramenta', icon: 'handyman' },
    { id: 'rt3', title: 'Barraca 4 pessoas', owner: 'Coletivo Maré', avatar: '#2A6FDB', price: 'R$ 35', unit: 'dia', tag: 'Camping', icon: 'camping' },
    { id: 'rt4', title: 'Som + microfone', owner: 'Dona Marta', avatar: '#C9962B', price: 'R$ 60', unit: 'dia', tag: 'Eventos', icon: 'speaker' },
  ],

  digitalServices: [
    { id: 'ds1', title: 'Criação de logo e identidade', who: 'Marina Designer', avatar: '#E8A06A', price: 'R$ 250', tag: 'Design', icon: 'palette' },
    { id: 'ds2', title: 'Site para comércio local', who: 'Bruno Dev', avatar: '#377B57', price: 'R$ 800', tag: 'Web', icon: 'language' },
    { id: 'ds3', title: 'Gestão de redes sociais', who: 'Coletivo Maré', avatar: '#2A6FDB', price: 'R$ 180/mês', tag: 'Marketing', icon: 'campaign' },
    { id: 'ds4', title: 'Edição de vídeos', who: 'Júlia Filma', avatar: '#C9962B', price: 'R$ 120', tag: 'Vídeo', icon: 'movie' },
  ],
};

window.ARAH = ARAH;
