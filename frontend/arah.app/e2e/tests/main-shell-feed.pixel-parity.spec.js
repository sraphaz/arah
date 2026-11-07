const fs = require('fs');
const { test, expect } = require('@playwright/test');
const pixelmatchModule = require('pixelmatch');
const { PNG } = require('pngjs');
const {
  activateFlutterSemantics,
  grantOnboardingGeolocation,
  mockShellFeedBff,
  mockTileServer,
  seedAuthenticatedSession,
} = require('./support/onboarding_helpers');

const pixelmatch = pixelmatchModule.default || pixelmatchModule;

const HANDOFF_URL =
  process.env.HANDOFF_BASE_URL ||
  'http://127.0.0.1:4184/design-system/ui_kits/app/index.html';
const MAX_DIFF_PIXEL_RATIO = 0.01;
const REGION_PIXELMATCH_THRESHOLD = 0.06;
const REGIONS = {
  topBar: {
    x: 0,
    y: 0,
    width: 390,
    height: 92,
    maxDiffRatio: 0.03,
    maxMeanColorDelta: 14,
  },
  filterBar: {
    x: 18,
    y: 92,
    width: 220,
    height: 44,
    maxDiffRatio: 0.04,
    maxMeanColorDelta: 12,
  },
  visitorBanner: {
    x: 18,
    y: 136,
    width: 354,
    height: 74,
    maxDiffRatio: 0.04,
    maxMeanColorDelta: 12,
  },
  firstPost: {
    x: 18,
    y: 214,
    width: 354,
    height: 356,
    maxDiffRatio: 0.045,
    maxMeanColorDelta: 14,
  },
  bottomNav: {
    x: 0,
    y: 740,
    width: 390,
    height: 104,
    maxDiffRatio: 0.03,
    maxMeanColorDelta: 10,
  },
};

async function stabilizePage(page) {
  await page.addStyleTag({
    content: `
      *, *::before, *::after {
        animation: none !important;
        transition: none !important;
        caret-color: transparent !important;
      }
      html { scroll-behavior: auto !important; }
    `,
  });
  await page.evaluate(() => document.fonts?.ready ?? Promise.resolve());
}

async function normalizeHandoffForComparison(page) {
  await page.waitForFunction(
    () => document.body.innerText.includes('Você é visitante de Camburi'),
    undefined,
    { timeout: 15000 },
  );

  await page.evaluate(() => {
    const root = document.getElementById('root');
    const device = root?.firstElementChild;
    if (!root || !device || device.children.length < 4) {
      throw new Error('Unable to locate the handoff device wrapper.');
    }

    const dynamicIsland = device.children[0];
    const statusBar = device.children[1];
    const appShell = device.children[2];
    const homeIndicator = device.children[3];

    dynamicIsland.remove();
    statusBar.remove();
    homeIndicator.remove();

    const topSpacer = appShell.querySelector('div[style*="height: 56px"]');
    topSpacer?.remove();

    Object.assign(document.documentElement.style, {
      background: 'transparent',
    });
    Object.assign(document.body.style, {
      margin: '0',
      padding: '0',
      background: 'transparent',
      minHeight: '844px',
      display: 'block',
    });
    Object.assign(root.style, {
      width: '390px',
      height: '844px',
    });
    Object.assign(device.style, {
      borderRadius: '0',
      boxShadow: 'none',
      overflow: 'hidden',
      background: 'transparent',
    });
    Object.assign(appShell.style, {
      height: '844px',
    });
  });
}

async function captureHandoff(page, outputPath) {
  await page.addInitScript(() => {
    localStorage.setItem(
      'arah_app_v1',
      JSON.stringify({
        app: {
          stage: 'app',
          territoryId: 't1',
          role: 'visitante',
          likes: {},
          notifReads: [],
        },
      }),
    );
  });
  await page.goto(HANDOFF_URL, { waitUntil: 'networkidle' });
  await stabilizePage(page);
  await normalizeHandoffForComparison(page);
  await page.locator('#root > div').screenshot({
    path: outputPath,
    animations: 'disabled',
  });
}

async function captureImplementation(page, context, baseURL, outputPath) {
  await grantOnboardingGeolocation(context, baseURL);
  await mockTileServer(context);
  await mockShellFeedBff(page);
  await seedAuthenticatedSession(page);
  await activateFlutterSemantics(page);
  await page.touchscreen.tap(195, 780);
  await page.waitForURL('**/home', { timeout: 15000 });
  await stabilizePage(page);
  await page.waitForFunction(() => {
    const flutterView =
      document.querySelector('flutter-view') ||
      document.querySelector('flt-glass-pane') ||
      document.querySelector('flt-scene-host');
    const fontsReady =
      document.fonts?.check('19px Sora') &&
      document.fonts?.check('14px Geist');
    return (
      !!flutterView &&
      !!fontsReady &&
      document.body.innerText.includes('Você é visitante de Camburi') &&
      document.body.innerText.includes('Roda de conversa: água e saneamento')
    );
  });
  await page.evaluate(
    () =>
      new Promise((resolve) =>
        requestAnimationFrame(() => requestAnimationFrame(resolve)),
      ),
  );
  await page.screenshot({
    path: outputPath,
    animations: 'disabled',
    fullPage: false,
  });
}

function compareImages(expectedPath, actualPath, diffPath) {
  const expected = PNG.sync.read(fs.readFileSync(expectedPath));
  const actual = PNG.sync.read(fs.readFileSync(actualPath));

  if (expected.width !== actual.width || expected.height !== actual.height) {
    throw new Error(
      `Image dimensions do not match: handoff ${expected.width}x${expected.height}, implementation ${actual.width}x${actual.height}.`,
    );
  }

  const diff = new PNG({ width: expected.width, height: expected.height });
  const mismatchedPixels = pixelmatch(
    expected.data,
    actual.data,
    diff.data,
    expected.width,
    expected.height,
    { threshold: 0.1 },
  );

  fs.writeFileSync(diffPath, PNG.sync.write(diff));

  return {
    width: expected.width,
    height: expected.height,
    mismatchedPixels,
    diffRatio: mismatchedPixels / (expected.width * expected.height),
  };
}

function extractRegion(image, { x, y, width, height }) {
  const region = new PNG({ width, height });
  for (let row = 0; row < height; row += 1) {
    for (let col = 0; col < width; col += 1) {
      const sourceIndex = ((y + row) * image.width + (x + col)) * 4;
      const regionIndex = (row * width + col) * 4;
      region.data[regionIndex] = image.data[sourceIndex];
      region.data[regionIndex + 1] = image.data[sourceIndex + 1];
      region.data[regionIndex + 2] = image.data[sourceIndex + 2];
      region.data[regionIndex + 3] = image.data[sourceIndex + 3];
    }
  }
  return region;
}

function meanColorDelta(expected, actual) {
  let total = 0;
  const pixels = expected.width * expected.height;
  for (let index = 0; index < expected.data.length; index += 4) {
    total += Math.abs(expected.data[index] - actual.data[index]);
    total += Math.abs(expected.data[index + 1] - actual.data[index + 1]);
    total += Math.abs(expected.data[index + 2] - actual.data[index + 2]);
  }
  return total / (pixels * 3);
}

function scaleRegionBounds(bounds, imageWidth) {
  const scale = imageWidth / 390;
  return {
    x: Math.round(bounds.x * scale),
    y: Math.round(bounds.y * scale),
    width: Math.round(bounds.width * scale),
    height: Math.round(bounds.height * scale),
  };
}

function compareRegions(expectedPath, actualPath) {
  const expected = PNG.sync.read(fs.readFileSync(expectedPath));
  const actual = PNG.sync.read(fs.readFileSync(actualPath));

  return Object.entries(REGIONS).map(([name, config]) => {
    const bounds = scaleRegionBounds(config, expected.width);
    const expectedRegion = extractRegion(expected, bounds);
    const actualRegion = extractRegion(actual, bounds);
    const diff = new PNG({ width: bounds.width, height: bounds.height });
    const mismatchedPixels = pixelmatch(
      expectedRegion.data,
      actualRegion.data,
      diff.data,
      bounds.width,
      bounds.height,
      { threshold: REGION_PIXELMATCH_THRESHOLD },
    );

    return {
      name,
      diffRatio: mismatchedPixels / (bounds.width * bounds.height),
      meanColorDelta: meanColorDelta(expectedRegion, actualRegion),
      maxDiffRatio: config.maxDiffRatio,
      maxMeanColorDelta: config.maxMeanColorDelta,
    };
  });
}

async function createComposite(browser, imagePaths, outputPath) {
  const [handoff, implementation, diff] = imagePaths.map((imagePath) =>
    fs.readFileSync(imagePath).toString('base64'),
  );
  const context = await browser.newContext({
    viewport: { width: 1320, height: 980 },
    deviceScaleFactor: 1,
  });
  const page = await context.newPage();
  await page.setContent(`
    <!doctype html>
    <html lang="pt-BR">
      <head>
        <meta charset="utf-8" />
        <style>
          body {
            margin: 0;
            background: #0a0b09;
            color: #e7eee7;
            font-family: Arial, sans-serif;
          }
          .wrap {
            padding: 28px;
            display: flex;
            gap: 28px;
            align-items: flex-start;
            justify-content: center;
          }
          .panel {
            display: flex;
            flex-direction: column;
            gap: 10px;
          }
          .label {
            font-size: 28px;
            font-weight: 700;
            letter-spacing: 0.2px;
          }
          .rule {
            height: 3px;
            border-radius: 999px;
            background: #81c784;
          }
          img {
            width: 390px;
            height: 844px;
            object-fit: contain;
            background: #0a0b09;
            box-shadow: 0 18px 42px rgba(0, 0, 0, 0.35);
          }
        </style>
      </head>
      <body>
        <div class="wrap">
          <div class="panel">
            <div class="label">Handoff</div>
            <div class="rule" style="width: 170px"></div>
            <img alt="Handoff" src="data:image/png;base64,${handoff}" />
          </div>
          <div class="panel">
            <div class="label">Implementação</div>
            <div class="rule" style="width: 220px"></div>
            <img alt="Implementação" src="data:image/png;base64,${implementation}" />
          </div>
          <div class="panel">
            <div class="label">Diff</div>
            <div class="rule" style="width: 90px"></div>
            <img alt="Diff" src="data:image/png;base64,${diff}" />
          </div>
        </div>
      </body>
    </html>
  `);
  await page.screenshot({ path: outputPath, fullPage: true });
  await context.close();
}

function exportArtifactsIfRequested(files) {
  const targetDir = process.env.PIXEL_EXPORT_DIR;
  if (!targetDir) return;
  fs.mkdirSync(targetDir, { recursive: true });
  for (const [sourcePath, outputName] of files) {
    fs.copyFileSync(sourcePath, `${targetDir}/${outputName}`);
  }
}

test('main shell feed stays pixel-perfect against the handoff mobile source of truth', async ({
  browser,
  context,
  page,
  baseURL,
}, testInfo) => {
  const handoffPath = testInfo.outputPath('handoff_main_shell_feed_mobile.png');
  const implementationPath = testInfo.outputPath('impl_main_shell_feed_mobile.png');
  const diffPath = testInfo.outputPath('main_shell_feed_diff_mobile.png');
  const comparePath = testInfo.outputPath('main_shell_feed_compare_mobile.png');
  const summaryPath = testInfo.outputPath('main_shell_feed_pixel_parity_summary.txt');

  const handoffPage = await context.newPage();

  try {
    await captureHandoff(handoffPage, handoffPath);
    await captureImplementation(page, context, baseURL, implementationPath);

    const comparison = compareImages(
      handoffPath,
      implementationPath,
      diffPath,
    );
    const regionComparisons = compareRegions(
      handoffPath,
      implementationPath,
    );
    const diffPercent = comparison.diffRatio * 100;

    fs.writeFileSync(
      summaryPath,
      [
        `handoff=${handoffPath}`,
        `implementation=${implementationPath}`,
        `diff=${diffPath}`,
        `maxDiffPixelRatio=${MAX_DIFF_PIXEL_RATIO}`,
        `mismatchedPixels=${comparison.mismatchedPixels}`,
        `totalPixels=${comparison.width * comparison.height}`,
        `diffPercent=${diffPercent.toFixed(2)}`,
        ...regionComparisons.map(
          (region) =>
            `${region.name}=diffRatio:${region.diffRatio.toFixed(4)},meanColorDelta:${region.meanColorDelta.toFixed(2)}`,
        ),
      ].join('\n'),
    );

    await createComposite(
      browser,
      [handoffPath, implementationPath, diffPath],
      comparePath,
    );

    await testInfo.attach('handoff-main-shell-feed-mobile', {
      path: handoffPath,
      contentType: 'image/png',
    });
    await testInfo.attach('implementation-main-shell-feed-mobile', {
      path: implementationPath,
      contentType: 'image/png',
    });
    await testInfo.attach('main-shell-feed-diff-mobile', {
      path: diffPath,
      contentType: 'image/png',
    });
    await testInfo.attach('main-shell-feed-compare-mobile', {
      path: comparePath,
      contentType: 'image/png',
    });
    await testInfo.attach('main-shell-feed-pixel-parity-summary', {
      path: summaryPath,
      contentType: 'text/plain',
    });
    exportArtifactsIfRequested([
      [handoffPath, 'main_shell_feed_handoff_mobile.png'],
      [implementationPath, 'main_shell_feed_implementation_mobile.png'],
      [diffPath, 'main_shell_feed_diff_mobile.png'],
      [comparePath, 'main_shell_feed_compare_mobile.png'],
      [summaryPath, 'main_shell_feed_pixel_summary.txt'],
    ]);

    const regionFailures = regionComparisons.filter(
      (region) =>
        region.diffRatio > region.maxDiffRatio ||
        region.meanColorDelta > region.maxMeanColorDelta,
    );

    expect(
      comparison.diffRatio,
      `Pixel parity diff ${diffPercent.toFixed(2)}% exceeded the allowed ${(MAX_DIFF_PIXEL_RATIO * 100).toFixed(2)}%. Regional checks: ${regionComparisons
        .map(
          (region) =>
            `${region.name}[ratio=${region.diffRatio.toFixed(4)}/${region.maxDiffRatio.toFixed(4)},color=${region.meanColorDelta.toFixed(2)}/${region.maxMeanColorDelta.toFixed(2)}]`,
        )
        .join(' ')}. See main_shell_feed_diff_mobile.png and main_shell_feed_compare_mobile.png.`,
    ).toBeLessThanOrEqual(MAX_DIFF_PIXEL_RATIO);
    expect(
      regionFailures,
      `Regional parity regressions detected: ${regionFailures
        .map(
          (region) =>
            `${region.name}[ratio=${region.diffRatio.toFixed(4)}/${region.maxDiffRatio.toFixed(4)},color=${region.meanColorDelta.toFixed(2)}/${region.maxMeanColorDelta.toFixed(2)}]`,
        )
        .join(' ')}`,
    ).toEqual([]);
  } finally {
    await handoffPage.close();
  }
});
