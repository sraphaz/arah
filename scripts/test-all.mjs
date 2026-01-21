/**
 * Script mestre para rodar todos os testes
 * Executa validações de qualidade, acessibilidade e conformidade
 */

import { exec } from 'child_process';
import { promisify } from 'util';

const execAsync = promisify(exec);

const tests = [
  {
    name: 'WCAG AA Contrast',
    script: 'scripts/test-wcag-contrast.mjs',
    description: 'Valida contraste de cores conforme WCAG AA',
  },
  {
    name: 'CSS Variables',
    script: 'scripts/test-css-variables.mjs',
    description: 'Valida uso de variáveis CSS vs cores hardcoded',
  },
  {
    name: 'Duplicate IDs',
    script: 'scripts/check-duplicate-ids.js',
    description: 'Verifica IDs duplicados no DevPortal',
  },
  {
    name: 'API Links',
    script: 'scripts/validate-api-links.mjs',
    description: 'Valida links internos após subdivisão da API',
  },
];

async function runTest(test) {
  console.log(`\n🧪 ${test.name}`);
  console.log(`   ${test.description}`);
  console.log('─'.repeat(60));
  
  try {
    const { stdout, stderr } = await execAsync(`node ${test.script}`);
    if (stdout) console.log(stdout);
    if (stderr && !stderr.includes('warning')) console.error(stderr);
    return { name: test.name, passed: true };
  } catch (error) {
    console.error(`❌ ${test.name} falhou:`);
    if (error.stdout) console.log(error.stdout);
    if (error.stderr) console.error(error.stderr);
    return { name: test.name, passed: false, error: error.message };
  }
}

async function runAllTests() {
  console.log('╔══════════════════════════════════════════════════════════╗');
  console.log('║         Testes de Qualidade e Conformidade              ║');
  console.log('╚══════════════════════════════════════════════════════════╝');
  
  const results = [];
  
  for (const test of tests) {
    const result = await runTest(test);
    results.push(result);
  }
  
  console.log('\n╔══════════════════════════════════════════════════════════╗');
  console.log('║                    Resumo dos Testes                     ║');
  console.log('╚══════════════════════════════════════════════════════════╝\n');
  
  const passed = results.filter(r => r.passed).length;
  const failed = results.filter(r => !r.passed).length;
  
  results.forEach(result => {
    const status = result.passed ? '✅' : '❌';
    console.log(`${status} ${result.name}`);
    if (!result.passed && result.error) {
      console.log(`   Erro: ${result.error.substring(0, 100)}...`);
    }
  });
  
  console.log(`\n📊 Total: ${results.length} | ✅ Passou: ${passed} | ❌ Falhou: ${failed}`);
  
  if (failed > 0) {
    console.log('\n⚠️  Alguns testes falharam. Revise os erros acima.');
    process.exit(1);
  } else {
    console.log('\n✅ Todos os testes passaram!');
    process.exit(0);
  }
}

runAllTests().catch(error => {
  console.error('❌ Erro ao executar testes:', error);
  process.exit(1);
});
