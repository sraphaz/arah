// Jest setup file
// Add any global test setup here

// Suppress console warnings during tests if needed
global.console = {
  ...console,
  warn: jest.fn(),
  error: jest.fn(),
}
