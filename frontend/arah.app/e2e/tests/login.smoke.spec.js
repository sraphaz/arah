const { test, expect } = require('@playwright/test');

test('login screen shows key copy and matches the baseline screenshot', async ({ page }) => {
  await page.goto('/');

  const appShell = page.locator('body');

  await expect(appShell).toContainText('Arah', { timeout: 15000 });
  await expect(appShell).toContainText('Entrar com Google');
  await expect(appShell).toContainText('Entrar com e-mail');

  await page.evaluate(() => document.fonts?.ready ?? Promise.resolve());
  await expect(page).toHaveScreenshot('login-screen.png', {
    animations: 'disabled',
    fullPage: true,
  });
});
