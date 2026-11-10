const { test, expect } = require('@playwright/test');
const {
  grantOnboardingGeolocation,
  mockOnboardingJourney,
  seedAuthenticatedSession,
} = require('./support/onboarding_helpers');

test('onboarding journey enters the territory as visitor in mobile mode', async ({
  context,
  page,
  baseURL,
}) => {
  await grantOnboardingGeolocation(context, baseURL);
  await mockOnboardingJourney(page);
  await seedAuthenticatedSession(page);

  const appShell = page.locator('body');
  await expect(appShell).toContainText('Entrar no território');
  await expect(appShell).toContainText('Escolha seu território');
  await expect(appShell).toContainText('Localização ativa');
  await expect(appShell).toContainText('Continuar com Camburi');
  await expect(appShell).toContainText('Boiçucanga');

  await page.touchscreen.tap(195, 780);

  await page.waitForURL('**/home', { timeout: 15000 });
  await expect(appShell).toContainText('Você é visitante de Camburi');
  await expect(appShell).toContainText(
    'Confirme residência para ver posts e votar.',
  );
  await expect(appShell).toContainText('Camburi');
});
