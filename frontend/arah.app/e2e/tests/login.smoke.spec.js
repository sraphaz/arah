const { test, expect } = require('@playwright/test');

test('login screen shows key copy in mobile mode', async ({ page }) => {
  await page.goto('/');

  const appShell = page.locator('body');

  await expect(appShell).toContainText('Arah', { timeout: 15000 });
  await expect(appShell).toContainText('Entrar com Google');
  await expect(appShell).toContainText('Entrar com e-mail');
});
