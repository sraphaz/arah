/**
 * Script de teste de links pós-deploy
 * Verifica se todos os links funcionam na URL real da wiki
 */

import https from 'https';
import { URL } from 'url';

// Normaliza BASE_URL para garantir que sempre termine com /wiki
const rawBaseUrl = process.env.WIKI_URL || 'https://devportal.araponga.app/wiki';
const BASE_URL = rawBaseUrl.endsWith('/wiki')
  ? rawBaseUrl
  : rawBaseUrl.replace(/\/$/, '') + '/wiki';
const TIMEOUT = 10000;

// Links que devem funcionar (relativos ao BASE_URL, que agora sempre inclui /wiki)
const REQUIRED_LINKS = [
  '/',
  '/docs/',
  '/docs/ONBOARDING_PUBLICO/',
  '/docs/ONBOARDING_DEVELOPERS/',
  '/docs/ONBOARDING_ANALISTAS_FUNCIONAIS/',
  '/docs/00_INDEX/',
  '/docs/01_PRODUCT_VISION/',
  '/docs/DISCORD_SETUP/',
];

function fetch(url) {
  return new Promise((resolve, reject) => {
    const urlObj = new URL(url);
    const options = {
      hostname: urlObj.hostname,
      path: urlObj.pathname + urlObj.search,
      method: 'GET',
      headers: {
        'User-Agent': 'Araponga-Wiki-Link-Checker/1.0',
      },
      timeout: TIMEOUT,
    };

    const req = https.request(options, (res) => {
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      res.on('end', () => {
        resolve({
          status: res.statusCode,
          headers: res.headers,
          body: data,
        });
      });
    });

    req.on('error', (error) => {
      reject(error);
    });

    req.on('timeout', () => {
      req.destroy();
      reject(new Error(`Timeout after ${TIMEOUT}ms`));
    });

    req.end();
  });
}

async function testLink(url, expectedStatus = 200) {
  try {
    console.log(`Testing: ${url}`);
    const response = await fetch(url);

    if (response.status === expectedStatus || (expectedStatus === 200 && response.status >= 200 && response.status < 400)) {
      console.log(`  ✅ ${response.status} OK`);
      return { success: true, status: response.status, url };
    } else {
      console.log(`  ❌ ${response.status} - Expected ${expectedStatus}`);
      return { success: false, status: response.status, url, expected: expectedStatus };
    }
  } catch (error) {
    console.log(`  ❌ ERROR: ${error.message}`);
    return { success: false, error: error.message, url };
  }
}

async function extractLinksFromHTML(html, baseUrl) {
  const links = [];
  const linkRegex = /href=["']([^"']+)["']/gi;

  // Lista de esquemas de URL perigosos/bloqueados
  const dangerousSchemes = [
    'javascript:',
    'data:',
    'vbscript:',
    'file:',
    'about:',
  ];

  let match;
  while ((match = linkRegex.exec(html)) !== null) {
    let link = match[1];

    // Ignora anchors (#) e esquemas perigosos
    if (link.startsWith('#')) continue;

    // Verifica esquemas perigosos (case-insensitive)
    const lowerLink = link.toLowerCase().trim();
    const isDangerous = dangerousSchemes.some(scheme =>
      lowerLink.startsWith(scheme)
    );
    if (isDangerous) continue;

    // Converte links relativos para absolutos
    if (link.startsWith('/')) {
      link = new URL(link, baseUrl).href;
    } else if (!link.startsWith('http')) {
      link = new URL(link, baseUrl).href;
    }

    links.push(link);
  }

  return [...new Set(links)]; // Remove duplicates
}

async function testAllLinks() {
  console.log(`\n🔍 Testing Wiki Links at: ${BASE_URL}\n`);

  const results = {
    passed: [],
    failed: [],
  };

  // Test required internal links
  console.log('📋 Testing Required Internal Links:\n');
  for (const link of REQUIRED_LINKS) {
    // BASE_URL já inclui /wiki, então links relativos devem começar com /
    const fullUrl = link.startsWith('http') ? link : `${BASE_URL}${link}`;
    const result = await testLink(fullUrl);
    if (result.success) {
      results.passed.push(result);
    } else {
      results.failed.push(result);
    }
    await new Promise(resolve => setTimeout(resolve, 500)); // Small delay between requests
  }

  // Test main page and extract links from it
  console.log('\n📄 Extracting and testing links from main page:\n');
  try {
    const mainPageResponse = await fetch(`${BASE_URL}/`);
    if (mainPageResponse.status === 200) {
      const extractedLinks = await extractLinksFromHTML(mainPageResponse.body, BASE_URL);

      // Garantir que extractedLinks é um array
      if (!Array.isArray(extractedLinks)) {
        console.log(`  ⚠️  Could not extract links: expected array, got ${typeof extractedLinks}`);
        return;
      }

      console.log(`Found ${extractedLinks.length} links in main page`);

      // Test only wiki internal links
      const wikiLinks = extractedLinks.filter(link => link.includes('/wiki/'));
      console.log(`Testing ${wikiLinks.length} wiki internal links...\n`);

      for (const link of wikiLinks.slice(0, 10)) { // Limit to first 10 to avoid too many requests
        const result = await testLink(link);
        if (result.success) {
          results.passed.push(result);
        } else {
          results.failed.push(result);
        }
        await new Promise(resolve => setTimeout(resolve, 300));
      }
    }
  } catch (error) {
    console.log(`  ❌ Could not fetch main page: ${error.message}`);
  }

  // Summary
  console.log('\n' + '='.repeat(60));
  console.log('📊 Test Summary');
  console.log('='.repeat(60));
  console.log(`✅ Passed: ${results.passed.length}`);
  console.log(`❌ Failed: ${results.failed.length}`);

  if (results.failed.length > 0) {
    console.log('\n❌ Failed Links:');
    results.failed.forEach((result) => {
      console.log(`  - ${result.url} (Status: ${result.status || 'ERROR'})`);
    });
  }

  // Exit with error code if any tests failed
  process.exit(results.failed.length > 0 ? 1 : 0);
}

// Run tests
testAllLinks().catch((error) => {
  console.error('Fatal error:', error);
  process.exit(1);
});
