# Configuração de Fases do Projeto

**⚠️ IMPORTANTE**: Este arquivo é a **fonte única de verdade** para o número total de fases do projeto. Todas as referências a "número de fases" devem derivar deste arquivo ou ser calculadas dinamicamente.

## Total de Fases

O número total de fases é calculado dinamicamente contando os arquivos `FASE*.md` no diretório `docs/backlog-api/` (excluindo subdiretórios como `implementacoes/`).

**Atualização Automática**: O número de fases é atualizado automaticamente quando:
- Novos arquivos `FASE*.md` são adicionados ao diretório `docs/backlog-api/`
- Arquivos `FASE*.md` são removidos do diretório `docs/backlog-api/`

## Como Obter o Número de Fases

### Opção 1: Script Node.js (Recomendado para Build/Tooling)

```javascript
// scripts/get-phase-count.mjs
import { readdir } from 'fs/promises';
import { join } from 'path';

async function getPhaseCount() {
  const backlogPath = join(process.cwd(), 'docs', 'backlog-api');
  const files = await readdir(backlogPath);
  const phaseFiles = files.filter(f => 
    f.match(/^FASE\d+\.md$/) && !f.includes('/')
  );
  return phaseFiles.length;
}
```

### Opção 2: PowerShell (Para scripts Windows)

```powershell
# scripts/get-phase-count.ps1
$phaseFiles = Get-ChildItem -Path "docs/backlog-api" -Filter "FASE*.md" -File | Where-Object { $_.Name -match '^FASE\d+\.md$' }
$phaseCount = $phaseFiles.Count
Write-Output $phaseCount
```

### Opção 3: Bash/Shell (Para scripts Linux/Mac)

```bash
# scripts/get-phase-count.sh
cd docs/backlog-api
ls -1 FASE*.md | grep -E '^FASE[0-9]+\.md$' | wc -l
```

## Uso em Documentação Markdown

Para documentos Markdown, use:

```markdown
**Total de Fases**: [calcular dinamicamente ou referenciar este arquivo]
```

**Nota**: Atualize manualmente este arquivo quando o número de fases mudar, ou use um script de build para substituir automaticamente.

## Uso em Código TypeScript/JavaScript

```typescript
// lib/project-config.ts
import { readdir } from 'fs/promises';
import { join } from 'path';

let phaseCountCache: number | null = null;

export async function getTotalPhases(): Promise<number> {
  if (phaseCountCache !== null) {
    return phaseCountCache;
  }

  const backlogPath = join(process.cwd(), 'docs', 'backlog-api');
  const files = await readdir(backlogPath);
  const phaseFiles = files.filter(f => 
    /^FASE\d+\.md$/.test(f) && !f.includes('/')
  );
  phaseCountCache = phaseFiles.length;
  return phaseCountCache;
}
```

## Referências que DEVEM usar esta configuração

- `docs/STATUS_FASES.md` - Header "Total de Fases"
- `docs/00_INDEX.md` - Referências ao backlog
- `frontend/wiki/components/layout/Sidebar.tsx` - Descrição do link "Backlog"
- `docs/05_GLOSSARY.md` - Terminologia
- Qualquer outro documento ou código que mencione o número total de fases

## Atualização Quando Fases Mudam

1. **Adicionar nova fase**: Crie `FASE30.md` (ou próximo número) em `docs/backlog-api/`
2. **Remover fase**: Delete o arquivo `FASE*.md` correspondente
3. **Verificar**: Rode o script `get-phase-count` para confirmar o novo total

---

**Última Verificação Manual**: 2026-06-30 (61 fases: 1–51 comunitárias + 52–61 sustentação)
