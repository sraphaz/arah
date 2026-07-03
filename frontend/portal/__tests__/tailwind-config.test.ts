import tailwindConfig from '../tailwind.config';

describe('portal tailwind config', () => {
  it('defines forest palette (sync with design-tokens.css)', () => {
    const colors = tailwindConfig.theme?.extend?.colors as Record<string, unknown>;
    expect(colors).toBeDefined();
    expect(colors.forest).toBeDefined();
    const forest = colors.forest as Record<string, string>;
    expect(forest['50']).toBe('#F1F8F4');
    expect(forest['900']).toBe('#173525');
  });
});
