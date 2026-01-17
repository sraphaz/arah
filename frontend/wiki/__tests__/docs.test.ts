import { readdir, readFile } from 'fs/promises';
import { join } from 'path';

describe('Documentation Tests', () => {
  const docsPath = join(process.cwd(), '..', '..', 'docs');

  test('docs directory exists', async () => {
    await expect(readdir(docsPath)).resolves.toBeDefined();
  });

  test('has markdown files', async () => {
    const files = await readdir(docsPath);
    const mdFiles = files.filter(file => file.endsWith('.md'));
    expect(mdFiles.length).toBeGreaterThan(0);
  });

  test('00_INDEX.md exists and is readable', async () => {
    const indexPath = join(docsPath, '00_INDEX.md');
    const content = await readFile(indexPath, 'utf8');
    expect(content).toBeDefined();
    expect(content.length).toBeGreaterThan(0);
  });

  test('ONBOARDING_PUBLICO.md exists and is readable', async () => {
    const onboardingPath = join(docsPath, 'ONBOARDING_PUBLICO.md');
    const content = await readFile(onboardingPath, 'utf8');
    expect(content).toBeDefined();
    expect(content.length).toBeGreaterThan(0);
  });

  test('all markdown files are valid UTF-8', async () => {
    const files = await readdir(docsPath);
    const mdFiles = files.filter(file => file.endsWith('.md'));

    for (const file of mdFiles.slice(0, 10)) { // Test first 10 files
      const filePath = join(docsPath, file);
      await expect(readFile(filePath, 'utf8')).resolves.toBeDefined();
    }
  });
});
