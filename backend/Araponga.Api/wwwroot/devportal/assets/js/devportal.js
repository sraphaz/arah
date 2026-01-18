(function () {
  // Banner sempre visível - sem lógica de redirect.

  var LANG_STORAGE_KEY = 'devportal.lang';

  var I18N = {
    pt: {
      'ui.skip': 'Pular para conteúdo',
      'ui.portalTitle': 'Developer Portal da API Araponga',
      'ui.lang': 'Idioma',
      'ui.backHome': '← Voltar para araponga.app',
      'ui.heroTag': 'Araponga API • Developer Portal',
      'ui.heroTitle': 'Infraestrutura territorial para comunidades locais.',
      'ui.heroLeadHtml': 'O Araponga é uma plataforma comunitária <strong>territory-first</strong>. O território organiza contexto, visibilidade e governança. Este portal documenta o produto e a API exatamente como implementada hoje, para acelerar o onboarding de desenvolvedores com segurança e previsibilidade.',
      'overview.eyebrow': 'Visão geral',
      'overview.title': 'API orientada a território + curadoria comunitária.',
      'overview.lead': 'A API da Araponga cria um núcleo confiável para território, vínculos, feed, mapa, marketplace, eventos e chat. Ela prioriza regras explícitas (visibilidade, validação e moderação) para que experiências locais sejam auditáveis e evoluam com segurança.',
      'how.eyebrow': 'Como o Araponga funciona',
      'how.title': 'Do visitante ao morador validado',
      'territories.eyebrow': 'Territórios',
      'territories.title': 'Unidade primária com vínculo estrutural',
      'concepts.eyebrow': 'Conceitos de produto',
      'concepts.title': 'Semântica de negócio',
      'domain.eyebrow': 'Modelo de domínio',
      'domain.title': 'Entidades principais e relacionamentos',
      'domain.diagramPlaceholder': 'Diagrama do domínio: removido temporariamente (vou refazer a imagem).',
      'flows.eyebrow': 'Fluxos principais',
      'flows.title': 'Sequências orientadas a produto e API',
      'usecases.eyebrow': 'Casos de uso',
      'usecases.title': 'Como usar a API (cenários práticos)',
      'usecases.leadHtml': 'Estes cenários conectam objetivo → pré-requisitos → endpoints. Em geral, você vai precisar de um JWT e de um território selecionado via <code>X-Session-Id</code>.',
      'marketplace.eyebrow': 'Marketplace',
      'marketplace.title': 'Economia local territorial',
      'marketplace.lead': 'O marketplace permite que moradores criem lojas, listem produtos/serviços, gerenciem carrinho de compras e recebam inquiries de interessados. Tudo ancorado no território.',
      'events.eyebrow': 'Eventos',
      'events.title': 'Eventos territoriais com data/hora e localização',
      'events.lead': 'Eventos são criados por moradores e aparecem no feed e no mapa. Eles têm data/hora de início (obrigatória) e fim (opcional), além de geolocalização obrigatória.',
      'auth.eyebrow': 'Autenticação',
      'auth.title': 'JWT (HS256)',
      'auth.leadHtml': 'O token é emitido pelo endpoint <code>/api/v1/auth/social</code>. Ele utiliza algoritmo HS256 e valida <code>issuer</code> e <code>audience</code> conforme configuração em <code>appsettings.json</code>.',
      'headers.eyebrow': 'Contexto e headers',
      'headers.title': 'Território, sessão e geolocalização',
      'headers.leadHtml': 'Várias rotas aceitam o território via <code>territoryId</code> na query ou via sessão com <code>X-Session-Id</code>. Além disso, algumas operações exigem presença geográfica.',
      'openapi.eyebrow': 'OpenAPI',
      'openapi.title': 'API Explorer (Swagger UI local)',
      'openapi.leadHtml': 'O explorer usa a especificação real da API. Ele tenta carregar o Swagger dinâmico em <code>/swagger/v1/swagger.json</code> (quando o backend roda em <code>Development</code>) e usa fallback para <code>../openapi.json</code>. A navegação é feita por tags, endpoints e schemas.',
      'openapi.download': 'Baixar OpenAPI (JSON)',
      'openapi.open': 'Abrir Explorer',
      'openapi.statusReady': 'Explorer pronto para carregar.',
      'errors.eyebrow': 'Erros & convenções',
      'errors.title': 'Como interpretar respostas',
      'quickstart.eyebrow': 'Quickstart',
      'quickstart.title': 'Copie e cole (5–10 comandos)',
      'quickstart.bashTitle': 'Bash (Linux/macOS/Git Bash)',
      'quickstart.powershellTitle': 'PowerShell (Windows)',
      'quickstart.tipHtml': '<strong>Dica:</strong> fonte única do portal em <code>backend/Araponga.Api/wwwroot/devportal</code>. O GitHub Pages publica exatamente essa pasta.',
      'versions.eyebrow': 'Versões & compatibilidade',
      'versions.title': 'Versionamento',
      'versions.leadHtml': 'A API atual utiliza prefixo <code>/api/v1</code>. Mudanças compatíveis devem manter o contrato dentro dessa versão. Para evolução maior, a estratégia recomendada é introduzir um novo prefixo.',
      'ui.quickstart': 'Quickstart',
      'ui.openapi': 'OpenAPI',
      'ui.auth': 'Autenticação',
      'ui.selectTerritory': 'Selecionar território',
      'nav.title': 'Conteúdo',
      'nav.overview': 'Visão geral',
      'nav.howItWorks': 'Como o Araponga funciona',
      'nav.territories': 'Territórios',
      'nav.productConcepts': 'Conceitos de produto',
      'nav.domainModel': 'Modelo de domínio',
      'nav.flows': 'Fluxos principais',
      'nav.useCases': 'Casos de uso',
      'nav.marketplace': 'Marketplace',
      'nav.events': 'Eventos',
      'nav.jwt': 'Autenticação (JWT)',
      'nav.headers': 'Território & headers',
      'nav.admin': 'Admin & filas',
      'nav.openapi': 'OpenAPI / Explorer',
      'nav.errors': 'Erros & convenções',
      'nav.quickstart': 'Quickstart',
      'nav.versions': 'Versões',
      'admin.eyebrow': 'Admin',
      'admin.title': 'System Config, Work Queue e evidências',
      'admin.leadHtml': 'Para suportar validações e governança com rastreabilidade, a API implementa <strong>SystemConfig</strong> (configurações calibráveis), <strong>WorkItem</strong> (fila genérica de revisão humana) e <strong>DocumentEvidence</strong> (metadados de documentos com upload e download por proxy). Leia o documento completo em <a href=\"https://github.com/sraphaz/araponga/blob/main/docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md\" rel=\"noopener\">docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md</a>.',
      'admin.endpointsTitle': 'Endpoints principais',
      'admin.notesTitle': 'Notas',
      'admin.note1Html': '<strong>Autorização</strong>: SystemAdmin decide identidade; Curator decide residência e curadoria; Curator/Moderator decide moderação.',
      'admin.note2Html': '<strong>Proxy download</strong>: a API faz streaming do storage para aplicar autorização e auditoria (sem URL pública).',
      'admin.note3Html': '<strong>OpenAPI</strong>: veja as rotas na seção OpenAPI — tags <code>Admin</code>, <code>Verification</code>, <code>Moderation</code>.',
      'openapi.loading': 'Carregando especificação…',
      'openapi.ready': 'Explorer pronto.',
      'openapi.loadFailed': 'Não foi possível carregar a especificação.',
      'openapi.processFailed': 'Falha ao processar a especificação.',
      'openapi.noContainer': 'Explorer indisponível: container não encontrado.',
      'copy.label': 'Copiar bloco de código',
      'copy.button': 'Copiar',
      'copy.done': 'Copiado',
      'copy.fail': 'Falhou'
    },
    es: {
      'ui.skip': 'Saltar al contenido',
      'ui.portalTitle': 'Portal de Desarrolladores de la API Araponga',
      'ui.lang': 'Idioma',
      'ui.backHome': '← Volver a araponga.app',
      'ui.heroTag': 'Araponga API • Portal de Desarrolladores',
      'ui.heroTitle': 'Infraestructura territorial para comunidades locales.',
      'ui.heroLeadHtml': 'Araponga es una plataforma comunitaria <strong>territory-first</strong>. El territorio organiza contexto, visibilidad y gobernanza. Este portal documenta el producto y la API tal como están implementados hoy, para acelerar el onboarding de desarrolladores con seguridad y previsibilidad.',
      'overview.eyebrow': 'Visión general',
      'overview.title': 'API orientada al territorio + curaduría comunitaria.',
      'overview.lead': 'La API de Araponga crea un núcleo confiable para territorio, vínculos, feed, mapa, marketplace y eventos. Prioriza reglas explícitas (visibilidad, validación y moderación) para que las experiencias locales sean auditables y evolucionen con seguridad.',
      'how.eyebrow': 'Cómo funciona Araponga',
      'how.title': 'Del visitante al residente validado',
      'territories.eyebrow': 'Territorios',
      'territories.title': 'Unidad primaria con vínculo estructural',
      'concepts.eyebrow': 'Conceptos del producto',
      'concepts.title': 'Semántica de negocio',
      'domain.eyebrow': 'Modelo de dominio',
      'domain.title': 'Entidades principales y relaciones',
      'domain.diagramPlaceholder': 'Diagrama de dominio: eliminado temporalmente (lo reharemos).',
      'flows.eyebrow': 'Flujos principales',
      'flows.title': 'Secuencias orientadas a producto y API',
      'usecases.eyebrow': 'Casos de uso',
      'usecases.title': 'Cómo usar la API (escenarios prácticos)',
      'usecases.leadHtml': 'Estos escenarios conectan objetivo → pre-requisitos → endpoints. En general, necesitarás un JWT y un territorio seleccionado vía <code>X-Session-Id</code>.',
      'marketplace.eyebrow': 'Marketplace',
      'marketplace.title': 'Economía local territorial',
      'marketplace.lead': 'El marketplace permite que los residentes creen tiendas, publiquen productos/servicios, gestionen el carrito y reciban consultas de interesados. Todo anclado al territorio.',
      'events.eyebrow': 'Eventos',
      'events.title': 'Eventos territoriales con fecha/hora y ubicación',
      'events.lead': 'Los eventos son creados por residentes y aparecen en el feed y el mapa. Tienen inicio (obligatorio) y fin (opcional), además de geolocalización obligatoria.',
      'auth.eyebrow': 'Autenticación',
      'auth.title': 'JWT (HS256)',
      'auth.leadHtml': 'El token se emite en el endpoint <code>/api/v1/auth/social</code>. Usa el algoritmo HS256 y valida <code>issuer</code> y <code>audience</code> según la configuración en <code>appsettings.json</code>.',
      'headers.eyebrow': 'Contexto y headers',
      'headers.title': 'Territorio, sesión y geolocalización',
      'headers.leadHtml': 'Varias rutas aceptan el territorio por <code>territoryId</code> en la query o por sesión con <code>X-Session-Id</code>. Además, algunas operaciones requieren presencia geográfica.',
      'openapi.eyebrow': 'OpenAPI',
      'openapi.title': 'API Explorer (Swagger UI local)',
      'openapi.leadHtml': 'El Explorer usa la especificación real de la API. Intenta cargar Swagger dinámico en <code>/swagger/v1/swagger.json</code> (cuando el backend corre en <code>Development</code>) y usa fallback a <code>../openapi.json</code>. La navegación se hace por tags, endpoints y schemas.',
      'openapi.download': 'Descargar OpenAPI (JSON)',
      'openapi.open': 'Abrir Explorer',
      'openapi.statusReady': 'Explorer listo para cargar.',
      'errors.eyebrow': 'Errores y convenciones',
      'errors.title': 'Cómo interpretar respuestas',
      'quickstart.eyebrow': 'Inicio rápido',
      'quickstart.title': 'Copia y pega (5–10 comandos)',
      'quickstart.bashTitle': 'Bash (Linux/macOS/Git Bash)',
      'quickstart.powershellTitle': 'PowerShell (Windows)',
      'quickstart.tipHtml': '<strong>Consejo:</strong> la fuente única del portal está en <code>backend/Araponga.Api/wwwroot/devportal</code>. GitHub Pages publica exactamente esa carpeta.',
      'versions.eyebrow': 'Versiones y compatibilidad',
      'versions.title': 'Versionado',
      'versions.leadHtml': 'La API actual usa el prefijo <code>/api/v1</code>. Los cambios compatibles deben mantener el contrato dentro de esta versión. Para una evolución mayor, la estrategia recomendada es introducir un nuevo prefijo.',
      'ui.quickstart': 'Inicio rápido',
      'ui.openapi': 'OpenAPI',
      'ui.auth': 'Autenticación',
      'ui.selectTerritory': 'Seleccionar territorio',
      'nav.title': 'Contenido',
      'nav.overview': 'Visión general',
      'nav.howItWorks': 'Cómo funciona Araponga',
      'nav.territories': 'Territorios',
      'nav.productConcepts': 'Conceptos del producto',
      'nav.domainModel': 'Modelo de dominio',
      'nav.flows': 'Flujos principales',
      'nav.useCases': 'Casos de uso',
      'nav.marketplace': 'Marketplace',
      'nav.events': 'Eventos',
      'nav.jwt': 'Autenticación (JWT)',
      'nav.headers': 'Territorio y headers',
      'nav.admin': 'Admin y colas',
      'nav.openapi': 'OpenAPI / Explorer',
      'nav.errors': 'Errores y convenciones',
      'nav.quickstart': 'Inicio rápido',
      'nav.versions': 'Versiones',
      'admin.eyebrow': 'Admin',
      'admin.title': 'System Config, Work Queue y evidencias',
      'admin.leadHtml': 'Para soportar validaciones y gobernanza con trazabilidad, la API implementa <strong>SystemConfig</strong> (configuración calibrable), <strong>WorkItem</strong> (cola genérica de revisión humana) y <strong>DocumentEvidence</strong> (metadatos de documentos con upload y descarga por proxy). Lee el documento completo en <a href=\"https://github.com/sraphaz/araponga/blob/main/docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md\" rel=\"noopener\">docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md</a>.',
      'admin.endpointsTitle': 'Endpoints principales',
      'admin.notesTitle': 'Notas',
      'admin.note1Html': '<strong>Autorización</strong>: SystemAdmin decide identidad; Curator decide residencia y curaduría; Curator/Moderator decide moderación.',
      'admin.note2Html': '<strong>Descarga por proxy</strong>: la API hace streaming desde el storage para aplicar autorización y auditoría (sin URL pública).',
      'admin.note3Html': '<strong>OpenAPI</strong>: mira las rutas en OpenAPI — tags <code>Admin</code>, <code>Verification</code>, <code>Moderation</code>.',
      'openapi.loading': 'Cargando especificación…',
      'openapi.ready': 'Explorer listo.',
      'openapi.loadFailed': 'No se pudo cargar la especificación.',
      'openapi.processFailed': 'Error al procesar la especificación.',
      'openapi.noContainer': 'Explorer no disponible: contenedor no encontrado.',
      'copy.label': 'Copiar bloque de código',
      'copy.button': 'Copiar',
      'copy.done': 'Copiado',
      'copy.fail': 'Falló'
    },
    en: {
      'ui.skip': 'Skip to content',
      'ui.portalTitle': 'Araponga API Developer Portal',
      'ui.lang': 'Language',
      'ui.backHome': '← Back to araponga.app',
      'ui.heroTag': 'Araponga API • Developer Portal',
      'ui.heroTitle': 'Territory infrastructure for local communities.',
      'ui.heroLeadHtml': 'Araponga is a community platform that is <strong>territory-first</strong>. Territory organizes context, visibility, and governance. This portal documents the product and the API exactly as implemented today, to speed up developer onboarding with safety and predictability.',
      'overview.eyebrow': 'Overview',
      'overview.title': 'Territory-oriented API + community curation.',
      'overview.lead': 'Araponga’s API provides a reliable core for territory, memberships, feed, map, marketplace, and events. It prioritizes explicit rules (visibility, validation, and moderation) so local experiences are auditable and evolve safely.',
      'how.eyebrow': 'How Araponga works',
      'how.title': 'From visitor to validated resident',
      'territories.eyebrow': 'Territories',
      'territories.title': 'Primary unit with structural linkage',
      'concepts.eyebrow': 'Product concepts',
      'concepts.title': 'Business semantics',
      'domain.eyebrow': 'Domain model',
      'domain.title': 'Core entities and relationships',
      'domain.diagramPlaceholder': 'Domain diagram: temporarily removed (will be redone).',
      'flows.eyebrow': 'Main flows',
      'flows.title': 'Product- and API-oriented sequences',
      'usecases.eyebrow': 'Use cases',
      'usecases.title': 'How to use the API (practical scenarios)',
      'usecases.leadHtml': 'These scenarios connect goal → prerequisites → endpoints. In general, you’ll need a JWT and a territory selected via <code>X-Session-Id</code>.',
      'marketplace.eyebrow': 'Marketplace',
      'marketplace.title': 'Territory-based local economy',
      'marketplace.lead': 'The marketplace lets residents create stores, publish products/services, manage carts, and receive inquiries. Everything is anchored to territory.',
      'events.eyebrow': 'Events',
      'events.title': 'Territory events with date/time and location',
      'events.lead': 'Events are created by residents and appear in the feed and on the map. They have a required start time, an optional end time, and mandatory geolocation.',
      'auth.eyebrow': 'Authentication',
      'auth.title': 'JWT (HS256)',
      'auth.leadHtml': 'The token is issued by <code>/api/v1/auth/social</code>. It uses HS256 and validates <code>issuer</code> and <code>audience</code> based on <code>appsettings.json</code>.',
      'headers.eyebrow': 'Context & headers',
      'headers.title': 'Territory, session, and geolocation',
      'headers.leadHtml': 'Several routes accept territory via <code>territoryId</code> in the query or via session using <code>X-Session-Id</code>. Some operations also require geographic presence.',
      'openapi.eyebrow': 'OpenAPI',
      'openapi.title': 'API Explorer (local Swagger UI)',
      'openapi.leadHtml': 'The Explorer uses the real API specification. It tries to load dynamic Swagger at <code>/swagger/v1/swagger.json</code> (when the backend runs in <code>Development</code>) and falls back to <code>../openapi.json</code>. Navigation is by tags, endpoints, and schemas.',
      'openapi.download': 'Download OpenAPI (JSON)',
      'openapi.open': 'Open Explorer',
      'openapi.statusReady': 'Explorer ready to load.',
      'errors.eyebrow': 'Errors & conventions',
      'errors.title': 'How to interpret responses',
      'quickstart.eyebrow': 'Quickstart',
      'quickstart.title': 'Copy & paste (5–10 commands)',
      'quickstart.bashTitle': 'Bash (Linux/macOS/Git Bash)',
      'quickstart.powershellTitle': 'PowerShell (Windows)',
      'quickstart.tipHtml': '<strong>Tip:</strong> single source of truth lives in <code>backend/Araponga.Api/wwwroot/devportal</code>. GitHub Pages deploys that folder as-is.',
      'versions.eyebrow': 'Versions & compatibility',
      'versions.title': 'Versioning',
      'versions.leadHtml': 'The current API uses the <code>/api/v1</code> prefix. Compatible changes should keep the contract within this version. For larger evolution, the recommended strategy is to introduce a new prefix.',
      'ui.quickstart': 'Quickstart',
      'ui.openapi': 'OpenAPI',
      'ui.auth': 'Authentication',
      'ui.selectTerritory': 'Select territory',
      'nav.title': 'Contents',
      'nav.overview': 'Overview',
      'nav.howItWorks': 'How Araponga works',
      'nav.territories': 'Territories',
      'nav.productConcepts': 'Product concepts',
      'nav.domainModel': 'Domain model',
      'nav.flows': 'Main flows',
      'nav.useCases': 'Use cases',
      'nav.marketplace': 'Marketplace',
      'nav.events': 'Events',
      'nav.jwt': 'Authentication (JWT)',
      'nav.headers': 'Territory & headers',
      'nav.admin': 'Admin & queues',
      'nav.openapi': 'OpenAPI / Explorer',
      'nav.errors': 'Errors & conventions',
      'nav.quickstart': 'Quickstart',
      'nav.versions': 'Versions',
      'admin.eyebrow': 'Admin',
      'admin.title': 'System Config, Work Queue and evidences',
      'admin.leadHtml': 'To support validations and governance with traceability, the API provides <strong>SystemConfig</strong> (calibratable settings), <strong>WorkItem</strong> (a generic human review queue), and <strong>DocumentEvidence</strong> (document metadata with upload and proxy download). Read the full doc at <a href=\"https://github.com/sraphaz/araponga/blob/main/docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md\" rel=\"noopener\">docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md</a>.',
      'admin.endpointsTitle': 'Key endpoints',
      'admin.notesTitle': 'Notes',
      'admin.note1Html': '<strong>Authorization</strong>: SystemAdmin decides identity; Curator decides residency and curation; Curator/Moderator decides moderation.',
      'admin.note2Html': '<strong>Proxy download</strong>: the API streams from storage to enforce authorization and audit (no public URL).',
      'admin.note3Html': '<strong>OpenAPI</strong>: see routes in OpenAPI — tags <code>Admin</code>, <code>Verification</code>, <code>Moderation</code>.',
      'openapi.loading': 'Loading specification…',
      'openapi.ready': 'Explorer ready.',
      'openapi.loadFailed': 'Could not load the specification.',
      'openapi.processFailed': 'Failed to process the specification.',
      'openapi.noContainer': 'Explorer unavailable: container not found.',
      'copy.label': 'Copy code block',
      'copy.button': 'Copy',
      'copy.done': 'Copied',
      'copy.fail': 'Failed'
    },
    it: {
      'ui.skip': 'Vai al contenuto',
      'ui.portalTitle': 'Portale Sviluppatori API Araponga',
      'ui.lang': 'Lingua',
      'ui.backHome': '← Torna a araponga.app',
      'ui.heroTag': 'Araponga API • Portale Sviluppatori',
      'ui.heroTitle': 'Infrastruttura territoriale per comunità locali.',
      'ui.heroLeadHtml': 'Araponga è una piattaforma comunitaria <strong>territory-first</strong>. Il territorio organizza contesto, visibilità e governance. Questo portale documenta il prodotto e l’API esattamente come sono implementati oggi, per accelerare l’onboarding degli sviluppatori con sicurezza e prevedibilità.',
      'overview.eyebrow': 'Panoramica',
      'overview.title': 'API orientata al territorio + curatela della comunità.',
      'overview.lead': 'L’API di Araponga fornisce un nucleo affidabile per territorio, memberships, feed, mappa, marketplace ed eventi. Dà priorità a regole esplicite (visibilità, validazione e moderazione) affinché le esperienze locali siano verificabili e possano evolvere in sicurezza.',
      'how.eyebrow': 'Come funziona Araponga',
      'how.title': 'Da visitatore a residente validato',
      'territories.eyebrow': 'Territori',
      'territories.title': 'Unità primaria con collegamento strutturale',
      'concepts.eyebrow': 'Concetti di prodotto',
      'concepts.title': 'Semantica di business',
      'domain.eyebrow': 'Modello di dominio',
      'domain.title': 'Entità principali e relazioni',
      'domain.diagramPlaceholder': 'Diagramma di dominio: rimosso temporaneamente (verrà rifatto).',
      'flows.eyebrow': 'Flussi principali',
      'flows.title': 'Sequenze orientate a prodotto e API',
      'usecases.eyebrow': 'Casi d’uso',
      'usecases.title': 'Come usare l’API (scenari pratici)',
      'usecases.leadHtml': 'Questi scenari collegano obiettivo → prerequisiti → endpoint. In genere, ti serviranno un JWT e un territorio selezionato tramite <code>X-Session-Id</code>.',
      'marketplace.eyebrow': 'Marketplace',
      'marketplace.title': 'Economia locale basata sul territorio',
      'marketplace.lead': 'Il marketplace permette ai residenti di creare negozi, pubblicare prodotti/servizi, gestire il carrello e ricevere richieste di informazioni. Tutto è ancorato al territorio.',
      'events.eyebrow': 'Eventi',
      'events.title': 'Eventi territoriali con data/ora e posizione',
      'events.lead': 'Gli eventi sono creati dai residenti e compaiono nel feed e sulla mappa. Hanno un orario di inizio obbligatorio, una fine opzionale e geolocalizzazione obbligatoria.',
      'auth.eyebrow': 'Autenticazione',
      'auth.title': 'JWT (HS256)',
      'auth.leadHtml': 'Il token è emesso dall’endpoint <code>/api/v1/auth/social</code>. Usa HS256 e valida <code>issuer</code> e <code>audience</code> in base a <code>appsettings.json</code>.',
      'headers.eyebrow': 'Contesto e header',
      'headers.title': 'Territorio, sessione e geolocalizzazione',
      'headers.leadHtml': 'Diverse rotte accettano il territorio tramite <code>territoryId</code> nella query oppure tramite sessione con <code>X-Session-Id</code>. Inoltre, alcune operazioni richiedono presenza geografica.',
      'openapi.eyebrow': 'OpenAPI',
      'openapi.title': 'API Explorer (Swagger UI locale)',
      'openapi.leadHtml': 'L’Explorer usa la specifica reale dell’API. Prova a caricare Swagger dinamico su <code>/swagger/v1/swagger.json</code> (quando il backend gira in <code>Development</code>) e fa fallback a <code>../openapi.json</code>. La navigazione avviene per tag, endpoint e schemi.',
      'openapi.download': 'Scarica OpenAPI (JSON)',
      'openapi.open': 'Apri Explorer',
      'openapi.statusReady': 'Explorer pronto per il caricamento.',
      'errors.eyebrow': 'Errori e convenzioni',
      'errors.title': 'Come interpretare le risposte',
      'quickstart.eyebrow': 'Avvio rapido',
      'quickstart.title': 'Copia e incolla (5–10 comandi)',
      'quickstart.bashTitle': 'Bash (Linux/macOS/Git Bash)',
      'quickstart.powershellTitle': 'PowerShell (Windows)',
      'quickstart.tipHtml': '<strong>Suggerimento:</strong> fonte unica in <code>backend/Araponga.Api/wwwroot/devportal</code>. GitHub Pages pubblica esattamente quella cartella.',
      'versions.eyebrow': 'Versioni e compatibilità',
      'versions.title': 'Versionamento',
      'versions.leadHtml': 'L’API attuale usa il prefisso <code>/api/v1</code>. Le modifiche compatibili dovrebbero mantenere il contratto dentro questa versione. Per evoluzioni maggiori, la strategia consigliata è introdurre un nuovo prefisso.',
      'ui.quickstart': 'Avvio rapido',
      'ui.openapi': 'OpenAPI',
      'ui.auth': 'Autenticazione',
      'ui.selectTerritory': 'Seleziona territorio',
      'nav.title': 'Contenuti',
      'nav.overview': 'Panoramica',
      'nav.howItWorks': 'Come funziona Araponga',
      'nav.territories': 'Territori',
      'nav.productConcepts': 'Concetti di prodotto',
      'nav.domainModel': 'Modello di dominio',
      'nav.flows': 'Flussi principali',
      'nav.useCases': 'Casi d’uso',
      'nav.marketplace': 'Marketplace',
      'nav.events': 'Eventi',
      'nav.jwt': 'Autenticazione (JWT)',
      'nav.headers': 'Territorio e header',
      'nav.admin': 'Admin e code',
      'nav.openapi': 'OpenAPI / Explorer',
      'nav.errors': 'Errori e convenzioni',
      'nav.quickstart': 'Avvio rapido',
      'nav.versions': 'Versioni',
      'admin.eyebrow': 'Admin',
      'admin.title': 'System Config, Work Queue e evidenze',
      'admin.leadHtml': 'Per supportare validazioni e governance con tracciabilità, l’API fornisce <strong>SystemConfig</strong> (impostazioni calibrabili), <strong>WorkItem</strong> (coda generica di revisione umana) e <strong>DocumentEvidence</strong> (metadati dei documenti con upload e download via proxy). Leggi il documento completo in <a href=\"https://github.com/sraphaz/araponga/blob/main/docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md\" rel=\"noopener\">docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md</a>.',
      'admin.endpointsTitle': 'Endpoint principali',
      'admin.notesTitle': 'Note',
      'admin.note1Html': '<strong>Autorizzazione</strong>: SystemAdmin decide identità; Curator decide residenza e curatela; Curator/Moderator decide moderazione.',
      'admin.note2Html': '<strong>Download via proxy</strong>: l’API fa streaming dallo storage per applicare autorizzazione e audit (senza URL pubblico).',
      'admin.note3Html': '<strong>OpenAPI</strong>: vedi le rotte in OpenAPI — tag <code>Admin</code>, <code>Verification</code>, <code>Moderation</code>.',
      'openapi.loading': 'Caricamento specifica…',
      'openapi.ready': 'Explorer pronto.',
      'openapi.loadFailed': 'Impossibile caricare la specifica.',
      'openapi.processFailed': 'Errore nel processare la specifica.',
      'openapi.noContainer': 'Explorer non disponibile: contenitore non trovato.',
      'copy.label': 'Copia blocco di codice',
      'copy.button': 'Copia',
      'copy.done': 'Copiato',
      'copy.fail': 'Errore'
    },
    hi: {
      'ui.skip': 'सामग्री पर जाएँ',
      'ui.portalTitle': 'Araponga API डेवलपर पोर्टल',
      'ui.lang': 'भाषा',
      'ui.backHome': '← araponga.app पर वापस जाएँ',
      'ui.heroTag': 'Araponga API • डेवलपर पोर्टल',
      'ui.heroTitle': 'स्थानीय समुदायों के लिए क्षेत्रीय अवसंरचना।',
      'ui.heroLeadHtml': 'Araponga एक <strong>territory-first</strong> समुदाय प्लेटफ़ॉर्म है। Territory संदर्भ, visibility और governance को व्यवस्थित करता है। यह पोर्टल आज जैसी लागू की गई product और API को दस्तावेज़ करता है, ताकि developer onboarding सुरक्षित और पूर्वानुमेय रूप से तेज़ हो सके।',
      'ui.quickstart': 'त्वरित शुरुआत',
      'ui.openapi': 'OpenAPI',
      'ui.auth': 'प्रमाणीकरण',
      'ui.selectTerritory': 'क्षेत्र चुनें',
      'nav.title': 'विषयसूची',
      'nav.overview': 'सारांश',
      'nav.howItWorks': 'Araponga कैसे काम करता है',
      'nav.territories': 'क्षेत्र',
      'nav.productConcepts': 'उत्पाद अवधारणाएँ',
      'nav.domainModel': 'डोमेन मॉडल',
      'nav.flows': 'मुख्य प्रवाह',
      'nav.useCases': 'उपयोग मामले',
      'nav.marketplace': 'Marketplace',
      'nav.events': 'इवेंट्स',
      'nav.jwt': 'प्रमाणीकरण (JWT)',
      'nav.headers': 'क्षेत्र और headers',
      'nav.admin': 'Admin और queues',
      'nav.openapi': 'OpenAPI / Explorer',
      'nav.errors': 'त्रुटियाँ और नियम',
      'nav.quickstart': 'त्वरित शुरुआत',
      'nav.versions': 'संस्करण',
      'admin.eyebrow': 'Admin',
      'admin.title': 'System Config, Work Queue और evidences',
      'admin.leadHtml': 'Traceability के साथ validations और governance को सपोर्ट करने के लिए API <strong>SystemConfig</strong> (calibratable settings), <strong>WorkItem</strong> (generic human review queue) और <strong>DocumentEvidence</strong> (upload + proxy download के साथ document metadata) प्रदान करती है। पूरा डॉक पढ़ें: <a href=\"https://github.com/sraphaz/araponga/blob/main/docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md\" rel=\"noopener\">docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md</a>.',
      'admin.endpointsTitle': 'मुख्य endpoints',
      'admin.notesTitle': 'नोट्स',
      'admin.note1Html': '<strong>Authorization</strong>: SystemAdmin identity decide करता है; Curator residency/curation decide करता है; Curator/Moderator moderation decide करता है।',
      'admin.note2Html': '<strong>Proxy download</strong>: API storage से stream करके authorization और audit enforce करती है (कोई public URL नहीं)।',
      'admin.note3Html': '<strong>OpenAPI</strong>: OpenAPI में routes देखें — tags <code>Admin</code>, <code>Verification</code>, <code>Moderation</code>।',
      'overview.eyebrow': 'अवलोकन',
      'overview.title': 'Territory-आधारित API + समुदाय क्यूरेशन।',
      'overview.lead': 'Araponga की API territory, memberships, feed, map, marketplace और events के लिए एक भरोसेमंद core देती है। यह explicit rules (visibility, validation, moderation) को प्राथमिकता देती है ताकि स्थानीय अनुभव audit-योग्य रहें और सुरक्षित रूप से विकसित हों।',
      'how.eyebrow': 'Araponga कैसे काम करता है',
      'how.title': 'Visitor से validated resident तक',
      'territories.eyebrow': 'Territories',
      'territories.title': 'Primary unit (structural linkage सहित)',
      'concepts.eyebrow': 'Product concepts',
      'concepts.title': 'Business semantics',
      'domain.eyebrow': 'Domain model',
      'domain.title': 'Core entities and relationships',
      'domain.diagramPlaceholder': 'Domain diagram: temporarily removed (will be redone).',
      'flows.eyebrow': 'Main flows',
      'flows.title': 'Product- and API-oriented sequences',
      'usecases.eyebrow': 'Use cases',
      'usecases.title': 'How to use the API (practical scenarios)',
      'usecases.leadHtml': 'These scenarios connect goal → prerequisites → endpoints. In general, you’ll need a JWT and a territory selected via <code>X-Session-Id</code>.',
      'marketplace.eyebrow': 'Marketplace',
      'marketplace.title': 'Territory-based local economy',
      'marketplace.lead': 'The marketplace lets residents create stores, publish products/services, manage carts, and receive inquiries. Everything is anchored to territory.',
      'events.eyebrow': 'Events',
      'events.title': 'Territory events with date/time and location',
      'events.lead': 'Events are created by residents and appear in the feed and on the map. They have a required start time, an optional end time, and mandatory geolocation.',
      'auth.eyebrow': 'Authentication',
      'auth.title': 'JWT (HS256)',
      'auth.leadHtml': 'The token is issued by <code>/api/v1/auth/social</code>. It uses HS256 and validates <code>issuer</code> and <code>audience</code> based on <code>appsettings.json</code>.',
      'headers.eyebrow': 'Context & headers',
      'headers.title': 'Territory, session, and geolocation',
      'headers.leadHtml': 'Several routes accept territory via <code>territoryId</code> in the query or via session using <code>X-Session-Id</code>. Some operations also require geographic presence.',
      'openapi.eyebrow': 'OpenAPI',
      'openapi.title': 'API Explorer (local Swagger UI)',
      'openapi.leadHtml': 'The Explorer uses the real API specification. It tries to load dynamic Swagger at <code>/swagger/v1/swagger.json</code> (when the backend runs in <code>Development</code>) and falls back to <code>../openapi.json</code>. Navigation is by tags, endpoints, and schemas.',
      'openapi.download': 'Download OpenAPI (JSON)',
      'openapi.open': 'Open Explorer',
      'openapi.statusReady': 'Explorer ready to load.',
      'errors.eyebrow': 'Errors & conventions',
      'errors.title': 'How to interpret responses',
      'quickstart.eyebrow': 'Quickstart',
      'quickstart.title': 'Copy & paste (5–10 commands)',
      'quickstart.bashTitle': 'Bash (Linux/macOS/Git Bash)',
      'quickstart.powershellTitle': 'PowerShell (Windows)',
      'quickstart.tipHtml': '<strong>Tip:</strong> single source of truth lives in <code>backend/Araponga.Api/wwwroot/devportal</code>. GitHub Pages deploys that folder as-is.',
      'versions.eyebrow': 'Versions & compatibility',
      'versions.title': 'Versioning',
      'versions.leadHtml': 'The current API uses the <code>/api/v1</code> prefix. Compatible changes should keep the contract within this version. For larger evolution, the recommended strategy is to introduce a new prefix.',
      'openapi.loading': 'स्पेसिफिकेशन लोड हो रहा है…',
      'openapi.ready': 'Explorer तैयार है।',
      'openapi.loadFailed': 'स्पेसिफिकेशन लोड नहीं हो सका।',
      'openapi.processFailed': 'स्पेसिफिकेशन प्रोसेस करने में विफल।',
      'openapi.noContainer': 'Explorer उपलब्ध नहीं: कंटेनर नहीं मिला।',
      'copy.label': 'कोड कॉपी करें',
      'copy.button': 'कॉपी',
      'copy.done': 'कॉपी हो गया',
      'copy.fail': 'विफल'
    }
  };

  function normalizeLang(lang) {
    if (!lang) return 'pt';
    var v = String(lang).toLowerCase();
    if (v.startsWith('pt')) return 'pt';
    if (v.startsWith('es')) return 'es';
    if (v.startsWith('en')) return 'en';
    if (v.startsWith('it')) return 'it';
    if (v.startsWith('hi')) return 'hi';
    return 'pt';
  }

  function getInitialLang() {
    try {
      var urlLang = new URL(window.location.href).searchParams.get('lang');
      if (urlLang) return normalizeLang(urlLang);
    } catch (e) {
      // ignore
    }

    try {
      var stored = localStorage.getItem(LANG_STORAGE_KEY);
      if (stored) return normalizeLang(stored);
    } catch (e) {
      // ignore
    }

    // default: Português
    return 'pt';
  }

  function setDocumentLang(lang) {
    var html = document.documentElement;
    var map = { pt: 'pt-BR', es: 'es', en: 'en', it: 'it', hi: 'hi' };
    html.setAttribute('lang', map[lang] || 'pt-BR');
  }

  function t(lang, key) {
    return (I18N[lang] && I18N[lang][key]) || I18N.pt[key] || key;
  }

  function applyI18n(lang) {
    setDocumentLang(lang);

    var nodes = document.querySelectorAll('[data-i18n]');
    nodes.forEach(function (el) {
      var key = el.getAttribute('data-i18n');
      el.textContent = t(lang, key);
    });

    var htmlNodes = document.querySelectorAll('[data-i18n-html]');
    htmlNodes.forEach(function (el) {
      var key = el.getAttribute('data-i18n-html');
      el.innerHTML = t(lang, key);
    });

    // Atualiza botões "Copiar" já renderizados
    var copyButtons = document.querySelectorAll('.copy-button');
    copyButtons.forEach(function (btn) {
      btn.textContent = t(lang, 'copy.button');
      btn.setAttribute('aria-label', t(lang, 'copy.label'));
    });

    var select = document.getElementById('lang-select');
    if (select) {
      select.value = lang;
    }
  }

  var explorerButton = document.querySelector('[data-explorer-button]');
  var explorerContainer = document.getElementById('openapi-browser');
  var explorerStatus = document.querySelector('[data-explorer-status]');
  var explorerLoaded = false;

  var currentLang = getInitialLang();
  applyI18n(currentLang);

  var langSelect = document.getElementById('lang-select');
  if (langSelect) {
    langSelect.addEventListener('change', function (event) {
      var next = normalizeLang(event.target.value);
      currentLang = next;
      try {
        localStorage.setItem(LANG_STORAGE_KEY, next);
      } catch (e) {
        // ignore
      }
      applyI18n(next);
    });
  }

  function setStatus(message, isError) {
    if (!explorerStatus) {
      return;
    }

    explorerStatus.textContent = message;
    explorerStatus.style.color = isError ? '#f26d6d' : 'inherit';
  }

  async function resolveSpecUrl() {
    var candidates = [
      '/swagger/v1/swagger.json',
      './openapi.json',
      '../openapi.json',
      '/devportal/openapi.json',
      '/openapi.json'
    ];

    for (var i = 0; i < candidates.length; i++) {
      var url = candidates[i];
      try {
        var response = await fetch(url, { cache: 'no-store' });
        if (response.ok) {
          return url;
        }
      } catch (error) {
        // ignore
      }
    }

    return '/openapi.json';
  }

  async function loadExplorer() {
    if (explorerLoaded) {
      explorerContainer?.classList.toggle('swagger-hidden');
      return;
    }

    if (!explorerContainer) {
      setStatus(t(currentLang, 'openapi.noContainer'), true);
      return;
    }

    setStatus(t(currentLang, 'openapi.loading'));
    var specUrl = await resolveSpecUrl();

    try {
      var response = await fetch(specUrl, { cache: 'no-store' });
      if (!response.ok) {
        setStatus(t(currentLang, 'openapi.loadFailed'), true);
        return;
      }

      var spec = await response.json();
      renderSpec(spec, explorerContainer);
      explorerLoaded = true;
      explorerContainer.classList.remove('swagger-hidden');
      setStatus(t(currentLang, 'openapi.ready'));
    } catch (error) {
      setStatus(t(currentLang, 'openapi.processFailed'), true);
    }
  }

  if (explorerButton) {
    explorerButton.addEventListener('click', function () {
      if (explorerContainer && explorerLoaded) {
        explorerContainer.classList.toggle('swagger-hidden');
        explorerButton.setAttribute(
          'aria-expanded',
          explorerContainer.classList.contains('swagger-hidden') ? 'false' : 'true'
        );
        return;
      }

      loadExplorer();
      explorerButton.setAttribute('aria-expanded', 'true');
    });
  }

  function renderSpec(spec, container) {
    container.innerHTML = '';
    if (!spec || !spec.paths) {
      container.textContent = 'Spec inválida.';
      return;
    }

    var tagMap = {};
    Object.keys(spec.paths).forEach(function (path) {
      var methods = spec.paths[path];
      Object.keys(methods).forEach(function (method) {
        var operation = methods[method];
        var tags = operation.tags && operation.tags.length ? operation.tags : ['General'];
        tags.forEach(function (tag) {
          tagMap[tag] = tagMap[tag] || [];
          tagMap[tag].push({ method: method.toUpperCase(), path: path, operation: operation });
        });
      });
    });

    Object.keys(tagMap).sort().forEach(function (tag) {
      var section = document.createElement('section');
      section.className = 'openapi-tag';
      var heading = document.createElement('h3');
      heading.textContent = tag + ' (' + tagMap[tag].length + ')';
      section.appendChild(heading);

      tagMap[tag].forEach(function (entry) {
        var details = document.createElement('details');
        details.className = 'endpoint';
        var summary = document.createElement('summary');
        var method = document.createElement('span');
        method.className = 'method ' + entry.method.toLowerCase();
        method.textContent = entry.method;
        summary.appendChild(method);
        var title = document.createElement('span');
        title.textContent = entry.path + (entry.operation.summary ? ' — ' + entry.operation.summary : '');
        summary.appendChild(title);
        details.appendChild(summary);

        var body = document.createElement('div');
        body.style.marginTop = '10px';

        if (entry.operation.parameters && entry.operation.parameters.length) {
          var params = document.createElement('p');
          params.innerHTML =
            '<strong>Parâmetros:</strong> ' +
            entry.operation.parameters
              .map(function (param) {
                return param.name + ' (' + param.in + ')';
              })
              .join(', ');
          body.appendChild(params);
        }

        if (entry.operation.requestBody) {
          var requestSchema = extractSchemaName(entry.operation.requestBody);
          var req = document.createElement('p');
          req.innerHTML = '<strong>Body:</strong> ' + requestSchema;
          body.appendChild(req);
        }

        if (entry.operation.responses) {
          var responseList = document.createElement('ul');
          responseList.style.margin = '6px 0 0';
          Object.keys(entry.operation.responses).forEach(function (status) {
            var responseSchema = extractSchemaName(entry.operation.responses[status]);
            var item = document.createElement('li');
            item.textContent = status + (responseSchema ? ' → ' + responseSchema : '');
            responseList.appendChild(item);
          });
          body.appendChild(responseList);
        }

        details.appendChild(body);
        section.appendChild(details);
      });

      container.appendChild(section);
    });

    if (spec.components && spec.components.schemas) {
      var schemasSection = document.createElement('section');
      schemasSection.className = 'openapi-tag';
      var schemasTitle = document.createElement('h3');
      schemasTitle.textContent = 'Schemas';
      schemasSection.appendChild(schemasTitle);

      var grid = document.createElement('div');
      grid.className = 'schema-grid';
      Object.keys(spec.components.schemas).sort().forEach(function (name) {
        var schema = spec.components.schemas[name];
        var card = document.createElement('div');
        card.className = 'schema-card';
        var title = document.createElement('h4');
        title.textContent = name;
        card.appendChild(title);

        if (schema.properties) {
          var list = document.createElement('ul');
          Object.keys(schema.properties).forEach(function (prop) {
            list.appendChild(document.createElement('li')).textContent = prop;
          });
          card.appendChild(list);
        } else {
          var empty = document.createElement('p');
          empty.textContent = 'Sem propriedades detalhadas.';
          card.appendChild(empty);
        }

        grid.appendChild(card);
      });

      schemasSection.appendChild(grid);
      container.appendChild(schemasSection);
    }
  }

  function extractSchemaName(target) {
    if (!target) {
      return '';
    }

    if (target.content && target.content['application/json'] && target.content['application/json'].schema) {
      return extractSchemaName(target.content['application/json'].schema);
    }

    if (target.schema) {
      return extractSchemaName(target.schema);
    }

    if (target.$ref) {
      return target.$ref.split('/').pop();
    }

    if (target.type === 'array' && target.items) {
      var itemsName = extractSchemaName(target.items);
      return itemsName ? 'Array<' + itemsName + '>' : 'Array';
    }

    return target.title || target.type || '';
  }

  async function copyToClipboard(text) {
    if (navigator.clipboard && navigator.clipboard.writeText) {
      await navigator.clipboard.writeText(text);
      return;
    }

    var textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.setAttribute('readonly', 'true');
    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';
    textarea.style.pointerEvents = 'none';
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
  }

  function enhanceCodeBlocks() {
    var blocks = document.querySelectorAll('.code-block');
    blocks.forEach(function (block) {
      if (block.querySelector('.copy-button')) {
        return;
      }

      var button = document.createElement('button');
      button.type = 'button';
      button.className = 'copy-button';
      button.textContent = t(currentLang, 'copy.button');
      button.setAttribute('aria-label', t(currentLang, 'copy.label'));

      button.addEventListener('click', async function () {
        var codeEl = block.querySelector('code');
        var text = (codeEl ? codeEl.innerText : block.innerText) || '';

        try {
          await copyToClipboard(text.trimEnd());
          var original = button.textContent;
          button.textContent = t(currentLang, 'copy.done');
          setTimeout(function () {
            button.textContent = original;
          }, 1200);
        } catch (error) {
          button.textContent = t(currentLang, 'copy.fail');
          setTimeout(function () {
            button.textContent = t(currentLang, 'copy.button');
          }, 1200);
        }
      });

      block.appendChild(button);
    });
  }

  enhanceCodeBlocks();

  // Sincronização de scroll: destaque automático no menu lateral conforme rolagem
  (function initScrollSync() {
    var navLinks = document.querySelectorAll('.nav a[href^="#"]');
    var sections = Array.from(navLinks).map(function(link) {
      var href = link.getAttribute('href');
      if (href && href.startsWith('#')) {
        var id = href.substring(1);
        var section = document.getElementById(id);
        return section ? { link: link, section: section, id: id } : null;
      }
      return null;
    }).filter(function(item) { return item !== null; });

    if (sections.length === 0) {
      return;
    }

    // IntersectionObserver para detectar qual seção está visível
    var observerOptions = {
      root: null,
      rootMargin: '-20% 0px -60% 0px', // Considera header e ajusta margem para ativação
      threshold: [0, 0.1, 0.2, 0.3, 0.4, 0.5]
    };

    var activeLink = null;

    function updateActiveLink(targetId) {
      // Remove classe active de todos os links
      navLinks.forEach(function(link) {
        link.classList.remove('active');
        link.removeAttribute('aria-current');
      });

      // Adiciona classe active no link correspondente
      var targetLink = sections.find(function(item) { return item.id === targetId; });
      if (targetLink && targetLink.link) {
        targetLink.link.classList.add('active');
        targetLink.link.setAttribute('aria-current', 'page');
        activeLink = targetLink.link;
      }
    }

    var observer = new IntersectionObserver(function(entries) {
      // Prioriza seções mais próximas do topo da viewport
      var visibleSections = entries
        .filter(function(entry) { return entry.isIntersecting; })
        .map(function(entry) {
          var rect = entry.boundingClientRect;
          return {
            id: entry.target.id,
            top: rect.top,
            intersectionRatio: entry.intersectionRatio
          };
        })
        .sort(function(a, b) {
          // Prioriza maior intersectionRatio, depois menor distância do topo
          if (Math.abs(a.intersectionRatio - b.intersectionRatio) > 0.1) {
            return b.intersectionRatio - a.intersectionRatio;
          }
          return Math.abs(a.top) - Math.abs(b.top);
        });

      if (visibleSections.length > 0) {
        updateActiveLink(visibleSections[0].id);
      }
    }, observerOptions);

    // Observa todas as seções
    sections.forEach(function(item) {
      if (item.section) {
        observer.observe(item.section);
      }
    });

    // Fallback: atualiza active link no scroll (para garantir que sempre há um ativo)
    var scrollTimeout;
    function handleScroll() {
      clearTimeout(scrollTimeout);
      scrollTimeout = setTimeout(function() {
        var scrollY = window.scrollY || window.pageYOffset;
        var headerOffset = 200; // Offset aproximado do header

        // Encontra a seção mais próxima do topo
        var closestSection = null;
        var closestDistance = Infinity;

        sections.forEach(function(item) {
          if (item.section) {
            var rect = item.section.getBoundingClientRect();
            var distance = Math.abs(rect.top - headerOffset);

            if (rect.top <= headerOffset + 100 && distance < closestDistance) {
              closestDistance = distance;
              closestSection = item;
            }
          }
        });

        if (closestSection && (!activeLink || activeLink !== closestSection.link)) {
          updateActiveLink(closestSection.id);
        }
      }, 100);
    }

    window.addEventListener('scroll', handleScroll, { passive: true });

    // Atualiza active link inicial baseado na posição da página
    handleScroll();
  })();
})();

