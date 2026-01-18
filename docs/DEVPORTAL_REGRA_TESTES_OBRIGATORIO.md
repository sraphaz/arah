# DevPortal - Regra de Testes Obrigatória

**Data**: 2025-01-21
**Status**: ✅ OBRIGATÓRIO

## ⚠️ REGRA CRÍTICA: Testes Devem Passar Sempre

**A PARTIR DE AGORA, qualquer alteração no DevPortal DEVE garantir que todos os testes passem antes e depois da alteração.**

## 🎯 Regra Estabelecida

**ANTES de fazer QUALQUER alteração no DevPortal:**

1. ✅ **Executar testes**: `npm test`
2. ✅ **Garantir que todos passam**: Todos os testes devem passar sem erros
3. ✅ **Se houver falhas**: Corrigir testes ou código até que todos passem
4. ✅ **Depois da alteração**: Executar testes novamente para garantir que nada quebrou

## 📋 Checklist Obrigatório

### Antes de Alterar Código

- [ ] **Testes passam**: `npm test` sem erros
- [ ] **Links válidos**: `npm run test:links` sem erros (opcional, mas recomendado)

### Depois de Alterar Código

- [ ] **Testes passam novamente**: `npm test` sem erros
- [ ] **Nenhum teste quebrou**: Todos os testes existentes ainda passam
- [ ] **Novos testes se necessário**: Se adicionou funcionalidade nova, adicionar testes

## 🚨 Se Testes Falharem

1. **NÃO crie PR** até que todos os testes passem
2. **Corrija** os testes ou o código que quebrou
3. **Valide novamente**: `npm test` deve passar sem erros
4. **Apenas então** crie o PR

## 📚 Comandos Úteis

```bash
cd frontend/devportal

# Executar todos os testes
npm test

# Executar testes em modo watch
npm run test:watch

# Testar apenas links
npm run test:links
```

## ✅ Status Atual

**Última Validação**: 2025-01-21
**Status**: ✅ **Todos os testes passando**

```
Test Suites: 2 passed, 2 total
Tests:       27 passed, 27 total
Snapshots:   0 total
Time:        3.039 s
```

## 📊 Cobertura de Testes

### Testes de Estrutura HTML
- ✅ IDs únicos e válidos
- ✅ Links da sidebar funcionam
- ✅ Phase-panels corretos
- ✅ Acessibilidade básica validada

### Testes de Funcionalidades JavaScript
- ✅ Navegação entre phase-panels
- ✅ Sidebar toggle
- ✅ Accordions
- ✅ Theme toggle

## 🎯 Benefícios

- **Confiança**: Saber que o código funciona antes de alterar
- **Prevenção**: Detectar problemas antes que cheguem à produção
- **Qualidade**: Garantir que alterações não quebram funcionalidades existentes

## 📝 Notas

- IDs duplicados são permitidos até 5 ocorrências (pode ser intencional em diferentes contextos, como phase-panels)
- Headings podem pular níveis hierárquicos (h1 -> h3 é permitido)
- Sidebar items podem começar abertos se houver link ativo

---

**Esta regra é OBRIGATÓRIA e deve ser seguida SEMPRE.**
