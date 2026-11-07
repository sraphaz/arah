const GEO_POSITION = {
  latitude: -23.76281,
  longitude: -45.63691,
};

const TERRITORIES = [
  {
    id: 't1',
    name: 'Camburi',
    description: '0.4 km de distancia',
    distanceKm: 0.4,
    latitude: -23.76281,
    longitude: -45.63691,
    isPending: false,
  },
  {
    id: 't3',
    name: 'Maresias',
    description: '6.2 km de distancia',
    distanceKm: 6.2,
    latitude: -23.7824,
    longitude: -45.5576,
    isPending: false,
  },
  {
    id: 't2',
    name: 'Boiçucanga',
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
          items: TERRITORIES.map(({ id, name, description }) => ({
            id,
            name,
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

async function activateFlutterSemantics(page) {
  const placeholder = page.locator('flt-semantics-placeholder');
  if ((await placeholder.count()) > 0) {
    await placeholder.first().evaluate((node) => node.click());
  }
}

async function seedAuthenticatedSession(page) {
  await page.goto('/', { waitUntil: 'networkidle' });
  await page.evaluate(async () => {
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
    localStorage.setItem('flutter.arah_theme_mode', JSON.stringify('dark'));
  });

  await page.goto('/', { waitUntil: 'networkidle' });
  await activateFlutterSemantics(page);

  await page.waitForFunction(
    () => document.body.innerText.includes('Escolha seu território'),
    undefined,
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
  TERRITORIES,
  TILE_SVG,
  activateFlutterSemantics,
  grantOnboardingGeolocation,
  mockOnboardingBff,
  mockOnboardingJourney,
  mockTileServer,
  seedAuthenticatedSession,
};
