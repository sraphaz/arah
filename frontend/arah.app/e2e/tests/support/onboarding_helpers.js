const fs = require('fs');
const path = require('path');

const COVER_RAINFOREST_DATA_URL = `data:image/png;base64,${fs
  .readFileSync(
    path.resolve(__dirname, '../../../../../design-system/assets/cover-rainforest.png'),
  )
  .toString('base64')}`;

const GEO_POSITION = {
  latitude: -23.76281,
  longitude: -45.63691,
};

function isoAgo({ days = 0, hours = 0, minutes = 0 }) {
  return new Date(
    Date.now() - (((days * 24 + hours) * 60 + minutes) * 60 * 1000),
  ).toISOString();
}

const TERRITORIES = [
  {
    id: 't1',
    name: 'Camburi',
    region: 'São Sebastião · SP',
    members: 1284,
    description: '0.4 km de distancia',
    distanceKm: 0.4,
    latitude: -23.76281,
    longitude: -45.63691,
    isPending: false,
  },
  {
    id: 't3',
    name: 'Maresias',
    region: 'São Sebastião · SP',
    members: 2106,
    description: '6.2 km de distancia',
    distanceKm: 6.2,
    latitude: -23.7824,
    longitude: -45.5576,
    isPending: false,
  },
  {
    id: 't2',
    name: 'Boiçucanga',
    region: 'São Sebastião · SP',
    members: 1740,
    description: '9.1 km de distancia',
    distanceKm: 9.1,
    latitude: -23.7772,
    longitude: -45.6438,
    isPending: false,
  },
];

const FEED_ITEMS = [
  {
    post: {
      id: 'post-1',
      title: 'Mutirao da praca',
      content: 'Sabado, 9h. Leve agua e ferramentas leves.',
      type: 'general',
      visibility: 'Public',
      createdAtUtc: '2026-11-06T23:00:00Z',
    },
    counts: {
      likes: 4,
      comments: 1,
      shares: 0,
    },
    userInteractions: {
      liked: false,
      shared: false,
    },
    media: [],
    metadata: {
      canDelete: false,
    },
    author: {
      displayName: 'Conselho de Moradores',
      handle: 'conselho.camburi',
      membership: 'MORADOR',
    },
  },
];

const SHELL_FEED_ITEMS = [
  {
    post: {
      id: 'p1',
      title: 'Mutirão de limpeza no costão neste sábado',
      content:
        'Encontro às 8h na praça da capela. Traga luvas e água. Vamos cuidar do nosso território juntos — café da manhã comunitário ao final.',
      type: 'alert',
      visibility: 'Public',
      createdAtUtc: isoAgo({ hours: 2 }),
      isHighlighted: true,
    },
    counts: {
      likes: 64,
      comments: 12,
      shares: 0,
    },
    userInteractions: {
      liked: false,
      shared: false,
    },
    media: [],
    metadata: {
      canDelete: false,
      isHighlighted: true,
    },
    author: {
      displayName: 'Conselho de Moradores',
      handle: 'conselho.camburi',
      membership: 'MORADOR',
    },
  },
  {
    post: {
      id: 'p2',
      title: 'Chegou robalo fresco na peixaria',
      content:
        'Pescado da manhã pelo seu Tião. Quem quiser, é só passar até as 18h. Apoiem a pesca artesanal da vila 🐟',
      type: 'general',
      visibility: 'Public',
      createdAtUtc: isoAgo({ hours: 5 }),
    },
    counts: {
      likes: 38,
      comments: 7,
      shares: 0,
    },
    userInteractions: {
      liked: false,
      shared: false,
    },
    media: [
      {
        url: COVER_RAINFOREST_DATA_URL,
      },
    ],
    metadata: {
      canDelete: false,
    },
    author: {
      displayName: 'Dona Marta',
      handle: 'marta.peixaria',
      membership: 'MORADOR',
    },
  },
  {
    post: {
      id: 'p3',
      title: 'Roda de conversa: água e saneamento',
      content:
        'Próxima terça, 19h, no centro comunitário. Vamos discutir a captação da nascente e os próximos passos com a associação.',
      type: 'general',
      visibility: 'ResidentsOnly',
      createdAtUtc: isoAgo({ days: 1 }),
    },
    counts: {
      likes: 91,
      comments: 28,
      shares: 0,
    },
    userInteractions: {
      liked: false,
      shared: false,
    },
    media: [],
    metadata: {
      canDelete: false,
    },
    author: {
      displayName: 'Coletivo Maré',
      handle: 'coletivo.mare',
      membership: 'MORADOR',
    },
  },
];

const SHELL_NOTIFICATIONS = [
  {
    id: 'n1',
    title: 'Novo alerta no seu território',
    body: 'Conselho de Moradores publicou um aviso sobre o mutirão de sábado.',
    kind: 'alert',
    createdAtUtc: '2026-11-07T00:15:00Z',
    readAtUtc: null,
  },
  {
    id: 'n2',
    title: 'Lembrete de evento',
    body: 'Roda de conversa sobre água começa em 1 dia.',
    kind: 'event',
    createdAtUtc: '2026-11-06T22:00:00Z',
    readAtUtc: null,
  },
];

const SHELL_CHAT_CHANNELS = [
  {
    id: 'ch1',
    name: 'Conselho de Moradores',
    kind: 'CHANNEL',
    status: 'ACTIVE',
    unreadCount: 2,
  },
];

const TILE_SVG = `
<svg xmlns="http://www.w3.org/2000/svg" width="256" height="256" viewBox="0 0 256 256">
  <rect width="256" height="256" fill="#edf1eb"/>
  <path d="M0 180 C38 164, 54 166, 86 178 S154 214, 256 182" fill="none" stroke="#b7c0b3" stroke-width="20" stroke-linecap="round"/>
  <path d="M-10 72 L68 118 L168 76 L266 122" fill="none" stroke="#cdd5ca" stroke-width="18" stroke-linecap="round"/>
  <path d="M24 0 L72 256 M114 0 L152 256 M200 0 L228 256" stroke="#dbe1d7" stroke-width="4" opacity="0.72"/>
  <path d="M0 38 L256 38 M0 126 L256 126 M0 212 L256 212" stroke="#dbe1d7" stroke-width="4" opacity="0.65"/>
  <circle cx="178" cy="92" r="22" fill="#d9dfd4"/>
  <circle cx="62" cy="188" r="16" fill="#d9dfd4"/>
</svg>
`.trim();

function territoryDetailFor(territoryId) {
  const territory = TERRITORIES.find((entry) => entry.id === territoryId);
  if (!territory) {
    return null;
  }

  return {
    id: territory.id,
    name: territory.name,
    latitude: territory.latitude,
    longitude: territory.longitude,
    radiusKm: territory.id === 't1' ? 1.2 : 1.1,
  };
}

function jsonResponse(body) {
  return {
    status: 200,
    contentType: 'application/json; charset=utf-8',
    body: JSON.stringify(body),
  };
}

async function grantOnboardingGeolocation(context, baseURL) {
  await context.setGeolocation(GEO_POSITION);
  await context.grantPermissions(['geolocation'], { origin: baseURL });
  await context.grantPermissions(['geolocation'], {
    origin: 'http://127.0.0.1:4184',
  });
}

async function mockTileServer(target) {
  await target.route('https://tile.openstreetmap.org/**', async (route) => {
    await route.fulfill({
      status: 200,
      contentType: 'image/svg+xml; charset=utf-8',
      body: TILE_SVG,
    });
  });
}

async function mockOnboardingBff(page) {
  await page.route('**/api/v2/journeys/**', async (route) => {
    const request = route.request();
    const url = new URL(request.url());
    const path = url.pathname;

    if (path.endsWith('/auth/check-email')) {
      await route.fulfill(jsonResponse({ exists: false }));
      return;
    }

    if (path.endsWith('/auth/signup')) {
      await route.fulfill(
        jsonResponse({
          userId: 'user-1',
          displayName: 'Pessoa Teste',
          email: 'pessoa@example.com',
          token: 'access-token-test',
          refreshToken: 'refresh-token-test',
          expiresInSeconds: 3600,
        }),
      );
      return;
    }

    if (path.endsWith('/onboarding/suggested-territories')) {
      await route.fulfill(jsonResponse({ territories: TERRITORIES }));
      return;
    }

    if (path.endsWith('/onboarding/complete')) {
      const body = request.postDataJSON();
      const selected =
        TERRITORIES.find((entry) => entry.id === body.selectedTerritoryId) ||
        TERRITORIES[0];
      await route.fulfill(
        jsonResponse({
          territory: {
            id: selected.id,
            name: selected.name,
          },
        }),
      );
      return;
    }

    if (path.endsWith('/territories/paged')) {
      await route.fulfill(
        jsonResponse({
          items: TERRITORIES.map(({ id, name, description, region, members }) => ({
            id,
            name,
            region,
            members,
            description,
          })),
          pageNumber: 1,
          pageSize: 50,
          totalCount: TERRITORIES.length,
        }),
      );
      return;
    }

    const territoryDetailMatch = path.match(/\/territories\/([^/]+)$/);
    if (territoryDetailMatch) {
      const detail = territoryDetailFor(territoryDetailMatch[1]);
      await route.fulfill(jsonResponse(detail ?? {}));
      return;
    }

    const membershipMatch = path.match(/\/membership\/([^/]+)\/me$/);
    if (membershipMatch) {
      await route.fulfill(
        jsonResponse({
          id: `membership-${membershipMatch[1]}`,
          userId: 'user-1',
          territoryId: membershipMatch[1],
          role: 'VISITOR',
        }),
      );
      return;
    }

    if (path.endsWith('/feed/territory-feed')) {
      await route.fulfill(
        jsonResponse({
          items: FEED_ITEMS,
          pageNumber: 1,
          pageSize: 20,
          totalCount: FEED_ITEMS.length,
        }),
      );
      return;
    }

    await route.fulfill(jsonResponse({}));
  });
}

async function mockOnboardingJourney(page) {
  await mockTileServer(page);
  await mockOnboardingBff(page);
}

async function mockShellFeedBff(page) {
  await page.route('**/api/v2/journeys/**', async (route) => {
    const request = route.request();
    const url = new URL(request.url());
    const path = url.pathname;

    if (path.endsWith('/onboarding/suggested-territories')) {
      await route.fulfill(jsonResponse({ territories: TERRITORIES }));
      return;
    }

    if (path.endsWith('/onboarding/complete')) {
      const body = request.postDataJSON();
      const selected =
        TERRITORIES.find((entry) => entry.id === body.selectedTerritoryId) ||
        TERRITORIES[0];
      await route.fulfill(
        jsonResponse({
          territory: {
            id: selected.id,
            name: selected.name,
          },
        }),
      );
      return;
    }

    if (path.endsWith('/territories/paged')) {
      await route.fulfill(
        jsonResponse({
          items: TERRITORIES.map(({ id, name, description, region, members }) => ({
            id,
            name,
            description,
            region,
            members,
          })),
          pageNumber: 1,
          pageSize: 50,
          totalCount: TERRITORIES.length,
        }),
      );
      return;
    }

    const membershipMatch = path.match(/\/membership\/([^/]+)\/me$/);
    if (membershipMatch) {
      await route.fulfill(
        jsonResponse({
          id: `membership-${membershipMatch[1]}`,
          userId: 'user-1',
          territoryId: membershipMatch[1],
          role: 'VISITOR',
        }),
      );
      return;
    }

    if (path.endsWith('/feed/territory-feed')) {
      await route.fulfill(
        jsonResponse({
          items: SHELL_FEED_ITEMS,
          pageNumber: 1,
          pageSize: 20,
          totalCount: SHELL_FEED_ITEMS.length,
        }),
      );
      return;
    }

    if (path.endsWith('/notifications/paged')) {
      await route.fulfill(
        jsonResponse({
          items: SHELL_NOTIFICATIONS,
          pageNumber: 1,
          pageSize: 20,
          totalCount: SHELL_NOTIFICATIONS.length,
          totalPages: 1,
        }),
      );
      return;
    }

    if (/\/notifications\/[^/]+\/read$/.test(path)) {
      await route.fulfill({ status: 204, body: '' });
      return;
    }

    if (/\/territories\/[^/]+\/chat\/channels$/.test(path)) {
      await route.fulfill(jsonResponse({ channels: SHELL_CHAT_CHANNELS }));
      return;
    }

    if (/\/territories\/[^/]+\/chat\/groups$/.test(path)) {
      await route.fulfill(jsonResponse({ groups: [] }));
      return;
    }

    if (path.endsWith('/me/profile')) {
      await route.fulfill(
        jsonResponse({
          id: 'user-1',
          displayName: 'Ana Ribeiro',
          email: 'ana@example.com',
          bio: 'Caiçara, professora e voluntária.',
          interests: ['Meio ambiente', 'Cultura caiçara'],
          avatarUrl: null,
          createdAtUtc: '2026-01-02T12:00:00Z',
        }),
      );
      return;
    }

    if (path.endsWith('/me/profile/stats')) {
      await route.fulfill(
        jsonResponse({
          postsCount: 23,
          connectionsCount: 12,
          interestsCount: 2,
        }),
      );
      return;
    }

    if (path.endsWith('/connections')) {
      await route.fulfill(jsonResponse([]));
      return;
    }

    await route.fulfill(jsonResponse({}));
  });
}

async function activateFlutterSemantics(page) {
  const placeholder = page.locator('flt-semantics-placeholder');
  await placeholder
    .first()
    .waitFor({ state: 'attached', timeout: 5000 })
    .catch(() => {});
  if ((await placeholder.count()) > 0) {
    await placeholder.first().evaluate((node) => node.click());
  }
}

async function seedAuthenticatedSession(page, options = {}) {
  const territoryId = options.selectedTerritoryId;
  await page.addInitScript(({ selectedTerritoryId }) => {
    localStorage.setItem('flutter.arah_theme_mode', 'dark');
    if (selectedTerritoryId) {
      localStorage.setItem('flutter.arah_selected_territory_id', selectedTerritoryId);
    }
  }, { selectedTerritoryId: territoryId });
  await page.goto('/', { waitUntil: 'networkidle' });
  await page.evaluate(async ({ selectedTerritoryId }) => {
    const publicKeyKey = 'FlutterSecureStorage';
    const encoder = new TextEncoder();

    const base64Encode = (bytes) => {
      let binary = '';
      bytes.forEach((byte) => {
        binary += String.fromCharCode(byte);
      });
      return btoa(binary);
    };

    const ensureKey = async () => {
      const algorithm = { name: 'AES-GCM', length: 256 };
      if (localStorage.getItem(publicKeyKey)) {
        const raw = Uint8Array.from(atob(localStorage.getItem(publicKeyKey)), (char) =>
          char.charCodeAt(0),
        );
        return crypto.subtle.importKey('raw', raw, algorithm, false, [
          'encrypt',
          'decrypt',
        ]);
      }

      const key = await crypto.subtle.generateKey(algorithm, true, [
        'encrypt',
        'decrypt',
      ]);
      const rawKey = new Uint8Array(await crypto.subtle.exportKey('raw', key));
      localStorage.setItem(publicKeyKey, base64Encode(rawKey));
      return key;
    };

    const writeSecureValue = async (key, value) => {
      const iv = crypto.getRandomValues(new Uint8Array(12));
      const cryptoKey = await ensureKey();
      const encrypted = new Uint8Array(
        await crypto.subtle.encrypt(
          { name: 'AES-GCM', iv },
          cryptoKey,
          encoder.encode(value),
        ),
      );
      localStorage.setItem(
        `${publicKeyKey}.${key}`,
        `${base64Encode(iv)}.${base64Encode(encrypted)}`,
      );
    };

    const expiry = new Date(Date.now() + 60 * 60 * 1000).toISOString();
    await writeSecureValue('arah_access_token', 'access-token-test');
    await writeSecureValue('arah_refresh_token', 'refresh-token-test');
    await writeSecureValue('arah_token_expiry', expiry);
    if (selectedTerritoryId) {
      localStorage.setItem(
        'flutter.arah_selected_territory_id',
        selectedTerritoryId,
      );
    }
  }, { selectedTerritoryId: territoryId });

  await page.goto('/', { waitUntil: 'networkidle' });
  await activateFlutterSemantics(page);

  await page.waitForFunction(
    (selectedTerritoryId) =>
      selectedTerritoryId
        ? window.location.pathname.endsWith('/home')
        : document.body.innerText.includes('Escolha seu território'),
    territoryId,
    { timeout: 15000 },
  );
  await page.evaluate(
    () =>
      new Promise((resolve) =>
        requestAnimationFrame(() => requestAnimationFrame(resolve)),
      ),
  );
}

module.exports = {
  FEED_ITEMS,
  GEO_POSITION,
  SHELL_CHAT_CHANNELS,
  SHELL_FEED_ITEMS,
  SHELL_NOTIFICATIONS,
  TERRITORIES,
  TILE_SVG,
  activateFlutterSemantics,
  grantOnboardingGeolocation,
  mockOnboardingBff,
  mockOnboardingJourney,
  mockShellFeedBff,
  mockTileServer,
  seedAuthenticatedSession,
};
