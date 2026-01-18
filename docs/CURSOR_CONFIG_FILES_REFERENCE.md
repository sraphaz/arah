# Arquivos de Configuração para Referência no Cursor

**Versão**: 1.0  
**Data**: 2025-01-20  
**Baseado em**: `.cursorrules`, boas práticas do projeto, padrões Cursor

---

## 📋 Objetivo

Este documento lista **arquivos de configuração e documentação** que devem ser referenciados no `.cursorrules` para garantir que o Cursor tenha contexto completo do projeto.

---

## 1. Documentação de Regras e Padrões (Obrigatório)

### Design e Identidade Visual

```
docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md
docs/CURSOR_DESIGN_RULES.md
docs/ANALISE_DESIGN_SISTEMATICA_MELHORIAS.md
```

**Uso**: Regras de design, mobile-first, cores, tipografia, espaçamento, componentes.

---

### Regras de Documentação

```
docs/CURSOR_DOCUMENTATION_RULES.md
```

**Uso**: Mapeamento de mudanças → documentos, checklist obrigatório de documentação.

---

### Arquitetura e Decisões

```
docs/10_ARCHITECTURE_DECISIONS.md
docs/11_ARCHITECTURE_SERVICES.md
docs/12_DOMAIN_MODEL.md
docs/13_DOMAIN_ROUTING.md
```

**Uso**: Decisões arquiteturais (ADRs), estrutura de services, modelo de domínio.

---

### Visão e Negócio

```
docs/01_PRODUCT_VISION.md
docs/05_GLOSSARY.md
docs/60_API_LÓGICA_NEGÓCIO.md
```

**Uso**: Contexto de negócio, terminologia correta (territory, items, 29 fases), lógica de negócio.

---

### Padrões de Código

```
docs/21_CODE_REVIEW.md
docs/22_COHESION_AND_TESTS.md
docs/41_CONTRIBUTING.md
CONTRIBUTING.md
```

**Uso**: Padrões de código, cobertura de testes (>90%), padrões de contribuição.

---

## 2. Arquivos de Configuração Técnica

### Frontend (Next.js/TypeScript/Tailwind)

```
frontend/wiki/tsconfig.json
frontend/wiki/next.config.mjs
frontend/wiki/tailwind.config.ts
frontend/wiki/app/globals.css
frontend/portal/tsconfig.json
frontend/portal/next.config.mjs
```

**Uso**: Configuração TypeScript, Next.js, Tailwind, design tokens CSS.

---

### Backend (.NET/C#)

```
global.json
backend/Araponga.Api/Araponga.Api.csproj
backend/Araponga.Application/Araponga.Application.csproj
backend/Araponga.Domain/Araponga.Domain.csproj
backend/Araponga.Infrastructure/Araponga.Infrastructure.csproj
backend/Araponga.Tests/Araponga.Tests.csproj
backend/Araponga.Api/Program.cs
backend/Araponga.Api/appsettings.json
backend/Araponga.Api/appsettings.Development.json
```

**Uso**: Versão .NET, dependências, configuração de aplicação.

---

### Ferramentas e Configurações

```
.editorconfig
.gitignore
docker-compose.yml
Dockerfile
```

**Uso**: Formatação, exclusões git, containers.

---

## 3. Estrutura Sugerida para `.cursorrules`

### Seção de Referências (Adicionar ao Início)

```markdown
## 📚 Arquivos de Referência para Contexto

### Design e Identidade Visual
- docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md
- docs/CURSOR_DESIGN_RULES.md
- docs/ANALISE_DESIGN_SISTEMATICA_MELHORIAS.md

### Regras e Padrões
- docs/CURSOR_DOCUMENTATION_RULES.md (OBRIGATÓRIO - atualização de docs)
- docs/10_ARCHITECTURE_DECISIONS.md
- docs/11_ARCHITECTURE_SERVICES.md
- docs/12_DOMAIN_MODEL.md
- docs/05_GLOSSARY.md (terminologia: territory, items, 29 fases)

### Visão e Negócio
- docs/01_PRODUCT_VISION.md
- docs/60_API_LÓGICA_NEGÓCIO.md

### Configurações Técnicas
- frontend/wiki/app/globals.css (design tokens)
- frontend/wiki/tailwind.config.ts
- global.json (.NET version)
- backend/Araponga.Api/Program.cs (configuração)
```

---

## 4. Arquivos de Configuração Recomendados para Criar

### `.editorconfig` (Já Existe - Verificar Completo)

Garantir que inclui configurações para:
- C# (indent 4 espaços)
- TypeScript/JavaScript (indent 2 espaços)
- Markdown (trim trailing whitespace = false)
- Charset UTF-8
- End of line LF

---

### `.prettierrc.json` (Frontend - Recomendado)

Se não existir, criar em `frontend/wiki/` e `frontend/portal/`:

```json
{
  "semi": true,
  "trailingComma": "es5",
  "singleQuote": false,
  "printWidth": 100,
  "tabWidth": 2,
  "useTabs": false,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

**Benefício**: Formatação consistente automática.

---

### `.eslintrc.json` (Frontend - Opcional)

Se não existir, considerar criar:

```json
{
  "extends": [
    "next/core-web-vitals",
    "eslint:recommended"
  ],
  "rules": {
    "prefer-const": "warn",
    "no-unused-vars": "warn"
  }
}
```

**Benefício**: Detecção de problemas comuns.

---

### `.vscode/settings.json` (Opcional mas Útil)

Criar em `.vscode/settings.json`:

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "files.eol": "\n",
  "files.insertFinalNewline": true,
  "files.trimTrailingWhitespace": true,
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp"
  }
}
```

**Benefício**: Formatação automática consistente no VSCode.

---

### `.vscode/extensions.json` (Opcional mas Útil)

Criar em `.vscode/extensions.json`:

```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "esbenp.prettier-vscode",
    "bradlc.vscode-tailwindcss",
    "editorconfig.editorconfig"
  ]
}
```

**Benefício**: Recomenda extensões essenciais ao time.

---

## 5. Priorização para `.cursorrules`

### Prioridade Máxima (Sempre Incluir)

1. `docs/CURSOR_DOCUMENTATION_RULES.md` - Regra obrigatória de atualização de docs
2. `docs/CURSOR_DESIGN_RULES.md` - Mobile-first, cores, tipografia
3. `docs/05_GLOSSARY.md` - Terminologia correta (territory, items, 29 fases)
4. `docs/10_ARCHITECTURE_DECISIONS.md` - Decisões arquiteturais
5. `docs/01_PRODUCT_VISION.md` - Contexto de negócio

### Prioridade Alta (Incluir para Contexto)

6. `docs/12_DOMAIN_MODEL.md` - Modelo de domínio
7. `docs/60_API_LÓGICA_NEGÓCIO.md` - Lógica de negócio
8. `docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md` - Identidade visual completa
9. `frontend/wiki/app/globals.css` - Design tokens
10. `global.json` - Versão .NET

### Prioridade Média (Referenciar quando Relevante)

11. `docs/11_ARCHITECTURE_SERVICES.md` - Services
12. `docs/21_CODE_REVIEW.md` - Padrões de código
13. `docs/22_COHESION_AND_TESTS.md` - Testes
14. `docs/41_CONTRIBUTING.md` - Contribuição

---

## 6. Como Atualizar o `.cursorrules`

### Estrutura Recomendada

```markdown
# Araponga - Cursor Best Practices

## 📚 Arquivos de Referência para Contexto

[Inserir seção de referências aqui]

## 🎯 Princípios Fundamentais do Projeto

[Manter conteúdo atual...]

## 🏗️ Arquitetura e Estrutura

[Manter conteúdo atual...]

[... resto do conteúdo atual ...]
```

### Processo de Atualização

1. **Adicionar seção de referências** no início do `.cursorrules`
2. **Listar arquivos por categoria** (Design, Arquitetura, Config)
3. **Manter comentários** explicando uso de cada arquivo
4. **Revisar periodicamente** quando novos documentos importantes forem criados

---

## 7. Benefícios

### Para o Cursor

- ✅ Contexto completo do projeto
- ✅ Acesso a regras de design e padrões
- ✅ Terminologia correta
- ✅ Configurações técnicas conhecidas

### Para o Projeto

- ✅ Consistência nas decisões
- ✅ Menos erros de nomenclatura
- ✅ Design alinhado com identidade
- ✅ Código seguindo padrões arquiteturais

---

## 8. Checklist de Implementação

### Documentação

- [ ] Adicionar seção de referências no `.cursorrules`
- [ ] Listar documentos de design (DESIGN_RULES, etc.)
- [ ] Listar documentos de arquitetura (ADRs, etc.)
- [ ] Listar documentos de padrões (DOCUMENTATION_RULES, etc.)
- [ ] Listar glossário (05_GLOSSARY.md)

### Configurações Técnicas

- [ ] Referenciar `globals.css` (design tokens)
- [ ] Referenciar `tailwind.config.ts`
- [ ] Referenciar `global.json` (.NET)
- [ ] Referenciar `Program.cs` (configuração)

### Arquivos Opcionais (Recomendados)

- [ ] Verificar `.editorconfig` está completo
- [ ] Criar `.prettierrc.json` (frontend) - se não existir
- [ ] Criar `.vscode/settings.json` - se desejado
- [ ] Criar `.vscode/extensions.json` - se desejado

---

## 9. Manutenção

**Quando Atualizar**:
- Novo documento importante criado
- Novos padrões estabelecidos
- Configurações mudam significativamente

**Frequência**: A cada PR que adiciona documentação significativa ou muda padrões críticos.

---

**Este documento complementa o `.cursorrules` existente, fornecendo lista prática de arquivos que devem ser referenciados para contexto completo do projeto.**
