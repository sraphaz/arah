const { test, expect } = require('@playwright/test');
const {
  activateFlutterSemantics,
  grantOnboardingGeolocation,
  mockShellFeedBff,
  mockTileServer,
  seedAuthenticatedSession,
} = require('./support/onboarding_helpers');

async function tapTextCenter(page, locator) {
  const box = await locator.boundingBox();
  if (!box) {
    throw new Error('Unable to resolve a tappable bounding box.');
  }
  await page.touchscreen.tap(box.x + box.width / 2, box.y + box.height / 2);
}

test('main shell opens on the visitor feed with canonical navigation and filters', async ({
  context,
  page,
  baseURL,
}) => {
  await grantOnboardingGeolocation(context, baseURL);
  await mockTileServer(context);
  await mockShellFeedBff(page);
  await seedAuthenticatedSession(page);
  await activateFlutterSemantics(page);
  await page.touchscreen.tap(195, 780);

  await page.waitForURL('**/home', { timeout: 15000 });
  const appShell = page.locator('body');

  await expect(appShell).toContainText('Camburi');
  await expect(appShell).toContainText('Feed');
  await expect(appShell).toContainText('Explorar');
  await expect(appShell).toContainText('Publicar');
  await expect(appShell).toContainText('Serviços');
  await expect(appShell).toContainText('Perfil');
  await expect(appShell).toContainText('Você é visitante de Camburi');
  await expect(appShell).toContainText(
    'Confirme residência para ver posts e votar.',
  );

  await tapTextCenter(page, page.getByText('Avisos', { exact: true }).first());
  await expect(
    page.getByRole('group', { name: /Mutirão de limpeza no costão neste sábado/i }),
  ).toBeVisible();
  await expect(
    page.getByRole('group', { name: /Roda de conversa: água e saneamento/i }),
  ).toHaveCount(0);

  await tapTextCenter(page, page.getByText('Vizinhança', { exact: true }));
  await expect(
    page.getByRole('group', { name: /Chegou robalo fresco na peixaria/i }),
  ).toBeVisible();
  await expect(
    page.getByRole('group', { name: /Roda de conversa: água e saneamento/i }),
  ).toHaveCount(0);
});
