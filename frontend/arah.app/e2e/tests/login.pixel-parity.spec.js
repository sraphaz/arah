const fs = require('fs');
const { test, expect } = require('@playwright/test');
const pixelmatchModule = require('pixelmatch');
const { PNG } = require('pngjs');

const pixelmatch = pixelmatchModule.default || pixelmatchModule;

const HANDOFF_URL =
  process.env.HANDOFF_BASE_URL ||
  'http://127.0.0.1:4184/design-system/ui_kits/app/index.html';
const MAX_DIFF_PIXEL_RATIO = 0.01;

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
  await page.waitForFunction(() => {
    const logo = document.querySelector('#root img[alt="Arah"]');
    return logo && logo.naturalWidth > 0;
  });

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
  await page.goto(HANDOFF_URL, { waitUntil: 'networkidle' });
  await stabilizePage(page);
  await normalizeHandoffForComparison(page);
  await page.locator('#root > div').screenshot({
    path: outputPath,
    animations: 'disabled',
  });
}

async function captureImplementation(page, outputPath) {
  await page.goto('/', { waitUntil: 'networkidle' });
  await stabilizePage(page);
  await expect(page.locator('body')).toContainText('Entrar com Google');
  await page.screenshot({
    path: outputPath,
    animations: 'disabled',
    fullPage: false,
  });
}

function compareImages(expectedPath, actualPath, diffPath) {
  const expected = PNG.sync.read(fs.readFileSync(expectedPath));
  const actual = PNG.sync.read(fs.readFileSync(actualPath));

  if (
    expected.width !== actual.width ||
    expected.height !== actual.height
  ) {
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

test('login screen stays pixel-perfect against the handoff mobile source of truth', async ({
  browser,
  context,
  page,
}, testInfo) => {
  const handoffPath = testInfo.outputPath('handoff_login_mobile.png');
  const implementationPath = testInfo.outputPath('impl_login_mobile.png');
  const diffPath = testInfo.outputPath('login_diff_mobile.png');
  const comparePath = testInfo.outputPath('login_compare_mobile.png');
  const summaryPath = testInfo.outputPath('pixel-parity-summary.txt');

  const handoffPage = await context.newPage();

  try {
    await captureHandoff(handoffPage, handoffPath);
    await captureImplementation(page, implementationPath);

    const comparison = compareImages(
      handoffPath,
      implementationPath,
      diffPath,
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
      ].join('\n'),
    );

    await createComposite(
      browser,
      [handoffPath, implementationPath, diffPath],
      comparePath,
    );

    await testInfo.attach('handoff-login-mobile', {
      path: handoffPath,
      contentType: 'image/png',
    });
    await testInfo.attach('implementation-login-mobile', {
      path: implementationPath,
      contentType: 'image/png',
    });
    await testInfo.attach('login-diff-mobile', {
      path: diffPath,
      contentType: 'image/png',
    });
    await testInfo.attach('login-compare-mobile', {
      path: comparePath,
      contentType: 'image/png',
    });
    await testInfo.attach('pixel-parity-summary', {
      path: summaryPath,
      contentType: 'text/plain',
    });

    console.log(
      `Login pixel parity diff: ${diffPercent.toFixed(2)}% (${comparison.mismatchedPixels}/${comparison.width * comparison.height} pixels).`,
    );

    expect(
      comparison.diffRatio,
      `Pixel parity diff ${diffPercent.toFixed(2)}% exceeded the allowed ${(MAX_DIFF_PIXEL_RATIO * 100).toFixed(2)}%. See login_diff_mobile.png and login_compare_mobile.png.`,
    ).toBeLessThanOrEqual(MAX_DIFF_PIXEL_RATIO);
  } finally {
    await handoffPage.close();
  }
});
