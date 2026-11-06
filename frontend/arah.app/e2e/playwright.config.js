const { defineConfig, devices } = require('@playwright/test');

const baseURL = process.env.PLAYWRIGHT_BASE_URL || 'http://127.0.0.1:4173';
const useExternalBaseUrl = Boolean(process.env.PLAYWRIGHT_BASE_URL);
const mobileProfile = {
  ...devices['iPhone 13'],
  viewport: { width: 390, height: 844 },
  deviceScaleFactor: 2,
  isMobile: true,
  hasTouch: true,
};

module.exports = defineConfig({
  testDir: './tests',
  fullyParallel: false,
  retries: process.env.CI ? 1 : 0,
  reporter: [
    ['list'],
    ['html', { open: 'never', outputFolder: 'playwright-report' }],
  ],
  use: {
    ...mobileProfile,
    baseURL,
    headless: true,
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },
  projects: [
    {
      name: 'mobile-chromium',
      use: {
        browserName: 'chromium',
      },
    },
  ],
  webServer: useExternalBaseUrl
    ? undefined
    : [
        {
          command: 'python3 -m http.server 4173 --bind 127.0.0.1 --directory ../build/web',
          port: 4173,
          reuseExistingServer: !process.env.CI,
        },
        {
          command: 'python3 -m http.server 4184 --bind 127.0.0.1 --directory ../../..',
          port: 4184,
          reuseExistingServer: !process.env.CI,
        },
      ],
});
