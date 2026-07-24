import { brand } from '../../shared/config/brand';

describe('brand config (shared)', () => {
  it('exposes canonical product name', () => {
    expect(brand.name).toBe('Arah');
  });

  it('links wiki, devportal and github', () => {
    expect(brand.urls.wiki).toMatch(/^https:\/\//);
    expect(brand.urls.devportal).toMatch(/^https:\/\//);
    expect(brand.urls.github).toContain('github.com');
  });
});
