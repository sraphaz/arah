#!/usr/bin/env node
/**
 * Testes do MCP do Agent Graph (globToRegex + gating de regra por path).
 * Sem dependências: node scripts/agents/tests/mcp.test.mjs
 */
import assert from 'node:assert/strict';
import { globToRegex, matchRulesForPath } from '../agent-graph-mcp.mjs';

let pass = 0;
function ok(name, fn) {
  try { fn(); pass++; console.log(`  [ok] ${name}`); }
  catch (e) { console.error(`  [FAIL] ${name}\n     ${e.message}`); process.exitCode = 1; }
}

console.log('globToRegex');
ok('** casa qualquer caminho', () => assert.ok(globToRegex('**').test('a/b/c.cs')));
ok('backend/** casa subpaths', () => {
  const re = globToRegex('backend/**');
  assert.ok(re.test('backend/Arah.Core/Foo.cs'));
  assert.ok(!re.test('frontend/app.tsx'));
});
ok('* não cruza barra', () => {
  const re = globToRegex('docs/*.md');
  assert.ok(re.test('docs/x.md'));
  assert.ok(!re.test('docs/sub/x.md'));
});
ok('metacaracteres regex são escapados (sem crash)', () => {
  assert.doesNotThrow(() => globToRegex('a+b(c)/**'));
  assert.ok(globToRegex('a+b/**').test('a+b/x'));
});

console.log('matchRulesForPath (gating de evento)');
const graph = {
  nodes: {
    rules: [
      { id: 'core', when: 'on_change', paths: ['backend/**'], agents: [] },
      { id: 'pr-always', when: 'pull_request', paths: ['**'], agents: [] },
    ],
  },
};
ok('regra when=pull_request não casa por path local', () => {
  const ids = matchRulesForPath(graph, 'backend/Arah.Core/Foo.cs').map((r) => r.id);
  assert.deepEqual(ids, ['core']);
});
ok('path fora das rules não retorna nada (nem pr-always)', () => {
  assert.equal(matchRulesForPath(graph, 'random/file.txt').length, 0);
});

if (!process.exitCode) console.log(`\nOK — ${pass} asserts`);
