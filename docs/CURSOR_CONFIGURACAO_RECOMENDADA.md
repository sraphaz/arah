# Configuração Recomendada do Cursor - Arquivos de Referência

**Versão**: 1.0  
**Data**: 2025-01-20  
**Objetivo**: Listar todos os arquivos de configuração recomendados para referência no `.cursorrules`

---

## 📋 Sumário

Este documento lista **todos os arquivos de configuração e documentação** que devem ser referenciados no `.cursorrules` para garantir que o Cursor tenha contexto completo do projeto, padrões e boas práticas.

---

## 1. Arquivos de Configuração Obrigatórios

### 1.1 Design e Identidade Visual

**Arquivos Recomendados**:
```
docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md
docs/CURSOR_DESIGN_RULES.md
docs/ANALISE_DESIGN_SISTEMATICA_MELHORIAS.md
```

**Por quê**: Define identidade visual completa, regras de design, mobile-first, cores, tipografia, espaçamento.

**Uso**: Referência para todas as decisões de UI/UX, design de componentes, escolhas visuais.

---

### 1.2 Documentação de Arquitetura

**Arquivos Recomendados**:
```
docs/10_ARCHITECTURE_DECISIONS.md
docs/11_ARCHITECTURE_SERVICES.md
docs/12_DOMAIN_MODEL.md
docs/13_DOMAIN_ROUTING.md
```

**Por quê**: Define decisões arquiteturais (ADRs), estrutura de services, modelo de domínio, organização de código.

**Uso**: Referência para decisões técnicas, padrões arquiteturais, organização de código.

---

### 1.3 Padrões de Código e Boas Práticas

**Arquivos Recomendados**:
```
docs/CURSOR_DOCUMENTATION_RULES.md
docs/21_CODE_REVIEW.md
docs/22_COHESION_AND_TESTS.md
docs/41_CONTRIBUTING.md
CONTRIBUTING.md
```

**Por quê**: Define padrões de código, regras de documentação, revisão de código, cobertura de testes, padrões de contribuição.

**Uso**: Referência para estilo de código, documentação, testes, contribuição.

---

### 1.4 Visão do Produto e Negócio

**Arquivos Recomendados**:
```
docs/01_PRODUCT_VISION.md
docs/02_ROADMAP.md
docs/03_BACKLOG.md
docs/05_GLOSSARY.md
docs/60_API_LÓGICA_NEGÓCIO.md
```

**Por quê**: Define propósito, roadmap, backlog, terminologia, lógica de negócio.

**Uso**: Referência para contexto de negócio, funcionalidades, terminologia correta.

---

### 1.5 Segurança e Configuração

**Arquivos Recomendados**:
```
docs/SECURITY_CONFIGURATION.md
docs/SECURITY_AUDIT.md
SECURITY.md
```

**Por quê**: Define políticas de segurança, configurações, auditoria.

**Uso**: Referência para decisões de segurança, validações, sanitizações.

---

## 2. Arquivos de Configuração Técnicos

### 2.1 Frontend (Next.js/React/TypeScript)

**Arquivos Recomendados**:
```
frontend/wiki/tsconfig.json
frontend/wiki/next.config.mjs
frontend/wiki/tailwind.config.ts
frontend/wiki/app/globals.css
frontend/portal/tsconfig.json
frontend/portal/next.config.mjs
```

**Por quê**: Define configuração TypeScript, Next.js, Tailwind CSS, tokens de design.

**Uso**: Referência para configuração de build, tipos, estilos, design tokens.

---

### 2.2 Backend (.NET/C#)

**Arquivos Recomendados**:
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

**Por quê**: Define versão .NET, dependências, configuração de projeto, settings.

**Uso**: Referência para versões, dependências, configuração de aplicação.

---

### 2.3 Ferramentas de Desenvolvimento

**Arquivos Recomendados**:
```
.gitignore
.editorconfig (se existir)
.prettierrc (se existir)
.eslintrc.json (se existir)
docker-compose.yml
Dockerfile
```

**Por quê**: Define padrões de formatação, linting, arquivos ignorados, containers.

**Uso**: Referência para formatação, linting, exclusões do git, containers.

---

## 3. Documentação de Onboarding

### 3.1 Para Desenvolvedores

**Arquivos Recomendados**:
```
docs/ONBOARDING_DEVELOPERS.md
docs/PROJECT_STRUCTURE.md
docs/ONBOARDING_FAQ.md
```

**Por quê**: Guia completo para desenvolvedores, estrutura do projeto, dúvidas frequentes.

**Uso**: Referência para onboarding de novos desenvolvedores, estrutura do código.

---

### 3.2 Para Analistas Funcionais

**Arquivos Recomendados**:
```
docs/ONBOARDING_ANALISTAS_FUNCIONAIS.md
docs/PRIORIZACAO_PROPOSTAS.md
```

**Por quê**: Guia para analistas, critérios de priorização.

**Uso**: Referência para análise funcional, priorização de funcionalidades.

---

## 4. Estrutura Recomendada do `.cursorrules`

### 4.1 Template Completo

```markdown
# Araponga - Cursor Rules

## Documentos de Referência Obrigatórios

### Design e Identidade Visual
- docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md
- docs/CURSOR_DESIGN_RULES.md
- docs/ANALISE_DESIGN_SISTEMATICA_MELHORIAS.md

### Arquitetura
- docs/10_ARCHITECTURE_DECISIONS.md
- docs/11_ARCHITECTURE_SERVICES.md
- docs/12_DOMAIN_MODEL.md
- docs/13_DOMAIN_ROUTING.md

### Padrões de Código
- docs/CURSOR_DOCUMENTATION_RULES.md
- docs/21_CODE_REVIEW.md
- docs/22_COHESION_AND_TESTS.md
- docs/41_CONTRIBUTING.md

### Visão e Negócio
- docs/01_PRODUCT_VISION.md
- docs/02_ROADMAP.md
- docs/03_BACKLOG.md
- docs/05_GLOSSARY.md
- docs/60_API_LÓGICA_NEGÓCIO.md

### Segurança
- docs/SECURITY_CONFIGURATION.md
- SECURITY.md

### Onboarding
- docs/ONBOARDING_DEVELOPERS.md
- docs/PROJECT_STRUCTURE.md

## Configurações Técnicas

### Frontend
- frontend/wiki/tsconfig.json
- frontend/wiki/tailwind.config.ts
- frontend/wiki/app/globals.css

### Backend
- global.json
- backend/Araponga.Api/Program.cs
- backend/Araponga.Api/appsettings.json

## Regras Fundamentais

[Seguem as regras do .cursorrules existente...]
```

---

## 5. Arquivos Adicionais Recomendados

### 5.1 Arquivos de Configuração que Devem Ser Criados

**Se não existirem, recomendamos criar**:

#### `.editorconfig`
```ini
root = true

[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true
indent_style = space
indent_size = 2

[*.{cs,csx,vb,vbx}]
indent_size = 4

[*.md]
trim_trailing_whitespace = false

[*.{json,yml,yaml}]
indent_size = 2
```

**Por quê**: Garante consistência de formatação entre editores.

---

#### `.prettierrc.json` (Frontend)
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

**Por quê**: Formatação consistente de código JavaScript/TypeScript.

---

#### `.eslintrc.json` (Frontend)
```json
{
  "extends": [
    "next/core-web-vitals",
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended"
  ],
  "rules": {
    "@typescript-eslint/no-unused-vars": "warn",
    "@typescript-eslint/explicit-module-boundary-types": "off",
    "prefer-const": "warn"
  }
}
```

**Por quê**: Linting consistente e detecção de problemas.

---

#### `.vscode/settings.json` (Opcional, mas recomendado)
```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  },
  "files.eol": "\n",
  "files.insertFinalNewline": true,
  "files.trimTrailingWhitespace": true,
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp"
  },
  "[typescript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[javascript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  }
}
```

**Por quê**: Configuração do editor para formatação automática.

---

#### `.vscode/extensions.json` (Opcional, mas recomendado)
```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "dbaeumer.vscode-eslint",
    "esbenp.prettier-vscode",
    "bradlc.vscode-tailwindcss",
    "editorconfig.editorconfig"
  ]
}
```

**Por quê**: Recomenda extensões essenciais para o projeto.

---

### 5.2 Arquivos de Configuração de Testes

**Arquivos Recomendados**:
```
backend/Araponga.Tests/Araponga.Tests.csproj
backend/Araponga.Tests/appsettings.json
```

**Por quê**: Configuração de testes, cobertura, configurações de teste.

---

## 6. Priorização de Referências

### 6.1 Prioridade Alta (Sempre Incluir)

1. **Design Rules**: `docs/CURSOR_DESIGN_RULES.md`
2. **Arquitetura**: `docs/10_ARCHITECTURE_DECISIONS.md`
3. **Visão**: `docs/01_PRODUCT_VISION.md`
4. **Documentação**: `docs/CURSOR_DOCUMENTATION_RULES.md`
5. **Glossário**: `docs/05_GLOSSARY.md`

### 6.2 Prioridade Média (Incluir quando relevante)

6. **Código**: `docs/21_CODE_REVIEW.md`
7. **Testes**: `docs/22_COHESION_AND_TESTS.md`
8. **Lógica de Negócio**: `docs/60_API_LÓGICA_NEGÓCIO.md`
9. **Estrutura**: `docs/PROJECT_STRUCTURE.md`

### 6.3 Prioridade Baixa (Referenciar quando necessário)

10. **Segurança**: `docs/SECURITY_CONFIGURATION.md`
11. **Roadmap**: `docs/02_ROADMAP.md`
12. **Onboarding**: `docs/ONBOARDING_DEVELOPERS.md`

---

## 7. Checklist de Configuração

Antes de considerar o `.cursorrules` completo, verificar:

### Documentação
- [ ] Design Rules incluído
- [ ] Arquitetura incluída
- [ ] Visão do Produto incluída
- [ ] Regras de Documentação incluídas
- [ ] Glossário incluído

### Configurações Técnicas
- [ ] TypeScript config (frontend)
- [ ] Tailwind config (frontend)
- [ ] Design tokens CSS (globals.css)
- [ ] .NET config (global.json)
- [ ] App settings (backend)

### Ferramentas (Opcional mas Recomendado)
- [ ] .editorconfig criado
- [ ] .prettierrc criado (frontend)
- [ ] .eslintrc criado (frontend)
- [ ] .vscode/settings.json criado
- [ ] .vscode/extensions.json criado

---

## 8. Como Atualizar o `.cursorrules`

### 8.1 Estrutura Recomendada

```markdown
# Araponga - Cursor Rules

## 📚 Documentos de Referência

### Design e UI/UX
[Incluir: DESIGN_SYSTEM_IDENTIDADE_VISUAL.md, CURSOR_DESIGN_RULES.md, etc.]

### Arquitetura
[Incluir: 10_ARCHITECTURE_DECISIONS.md, 12_DOMAIN_MODEL.md, etc.]

### Padrões e Boas Práticas
[Incluir: CURSOR_DOCUMENTATION_RULES.md, 21_CODE_REVIEW.md, etc.]

### Visão e Negócio
[Incluir: 01_PRODUCT_VISION.md, 05_GLOSSARY.md, etc.]

## 🔧 Configurações Técnicas

[Incluir referências a arquivos de config]

## 📋 Regras Fundamentais

[Regras específicas do projeto...]
```

### 8.2 Manutenção

**Quando Atualizar**:
- Novo documento importante criado
- Padrões novos estabelecidos
- Configurações mudam significativamente

**Frequência Recomendada**: A cada PR que adiciona documentação significativa ou muda padrões.

---

## 9. Benefícios desta Abordagem

### 9.1 Contexto Completo

O Cursor terá acesso a:
- ✅ Identidade visual completa
- ✅ Decisões arquiteturais
- ✅ Padrões de código
- ✅ Terminologia correta
- ✅ Configurações técnicas

### 9.2 Consistência

Garante que:
- ✅ Decisões seguem padrões estabelecidos
- ✅ Terminologia é consistente
- ✅ Design segue identidade visual
- ✅ Código segue arquitetura

### 9.3 Produtividade

Aumenta produtividade porque:
- ✅ Menos perguntas de contexto
- ✅ Menos decisões repetitivas
- ✅ Menos erros de nomenclatura
- ✅ Menos retrabalho

---

## 10. Referências Externas

### 10.1 Documentos Relacionados

- **Cursor Rules Atual**: `.cursorrules`
- **Guia de Design**: `docs/CURSOR_DESIGN_RULES.md`
- **Regras de Documentação**: `docs/CURSOR_DOCUMENTATION_RULES.md`

### 10.2 Ferramentas Recomendadas

- **EditorConfig**: Garante formatação consistente
- **Prettier**: Formatação automática (frontend)
- **ESLint**: Linting (frontend)
- **Editor**: VSCode com extensões recomendadas

---

**Este documento serve como guia completo para configurar o `.cursorrules` de forma profissional e completa, garantindo que o Cursor tenha todo o contexto necessário para trabalhar efetivamente no projeto Araponga.**
