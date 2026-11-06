const { test, expect } = require('@playwright/test');

test('login screen shows key copy in mobile mode', async ({ page }) => {
  await page.goto('/');

  await page.waitForFunction(
    () => document.querySelector('flt-semantics-placeholder'),
    undefined,
    { timeout: 15000 },
  );
  await page.locator('flt-semantics-placeholder').evaluate((el) => el.click());
  await page.waitForFunction(
    () => document.body.innerText.includes('Arah'),
    undefined,
    { timeout: 15000 },
  );

  const appShell = page.locator('body');

  await expect(appShell).toContainText('Arah', { timeout: 15000 });
  await expect(appShell).toContainText('Entrar com Google');
  await expect(appShell).toContainText('Entrar com e-mail');
});
