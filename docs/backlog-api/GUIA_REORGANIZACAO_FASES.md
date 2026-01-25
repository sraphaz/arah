# Guia Rápido: Reorganizar ou Adicionar Fases

**Versão**: 1.0  
**Objetivo**: Guia passo-a-passo simples para reorganizar ou adicionar fases

---

## 🎯 Princípio Fundamental

**Regra de Ouro**: Fase com número menor = implementada antes

**Exemplo**: Fase 15 deve ser implementada antes de Fase 20.

---

## ➕ Como Adicionar uma Nova Fase

### Passo 1: Escolher o Número

1. **Identificar posição desejada**:
   - Depois de qual fase? (ex: depois da Fase 20)
   - Antes de qual fase? (ex: antes da Fase 22)

2. **Verificar disponibilidade**:
   - Consultar `MAPA_FASES.md` para ver se o número está livre
   - Se ocupado, renumerar a fase existente ou escolher outro número

3. **Exemplo**: Nova fase "X" deve vir entre Fase 20 e 22
   - Se Fase 21 não existe → usar Fase 21
   - Se Fase 21 existe → renumerar ou usar outro número

---

### Passo 2: Criar o Arquivo

**Nome do arquivo**: `FASE[X].md` (ex: `FASE21.md`)

**Template mínimo**:

```markdown
# Fase X: Título da Fase

**Duração**: X semanas (Y dias úteis)  
**Prioridade**: 🔴/🟡/🟢 (P0/P1/P2)  
**Depende de**: Fase A, Fase B  
**Estimativa Total**: Z horas  
**Status**: ⏳ Pendente  

---

## 🎯 Objetivo

Implementar [descrição] que:
- [Funcionalidade 1]
- [Funcionalidade 2]

---

## 📋 Tarefas Detalhadas

### Semana 1: [Título]

#### X.1 [Tarefa]
**Estimativa**: X horas  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] [Tarefa 1]
- [ ] [Tarefa 2]

---

## 📊 Resumo da Fase X

| Tarefa | Estimativa | Status |
|--------|------------|--------|
| [Tarefa] | Xh | ❌ Pendente |
| **Total** | **Xh (Y dias)** | |

---

**Status**: ⏳ **FASE X PENDENTE**  
**Depende de**: Fases A, B
```

---

### Passo 3: Atualizar Referências

**Arquivos a atualizar**:

1. ✅ `docs/backlog-api/MAPA_FASES.md` - Adicionar na tabela
2. ✅ `docs/02_ROADMAP.md` - Adicionar na onda correspondente
3. ✅ `docs/backlog-api/ROADMAP_VISUAL.md` - Adicionar no diagrama
4. ✅ `docs/backlog-api/README.md` - Adicionar na lista

---

## 🔄 Como Reorganizar uma Fase Existente

### Passo 1: Decidir Nova Posição

**Pergunta**: Para onde mover?

- **Exemplo**: Mover "Compra Coletiva" de Fase 24 para Fase 17
- **Motivo**: Deve ser implementada antes de outras fases de economia local

---

### Passo 2: Atualizar o Arquivo

**No arquivo `FASE[X].md`**:

1. **Atualizar título**:
   ```markdown
   # Fase Y: Título  # Era: Fase X
   ```

2. **Atualizar numeração de tarefas**:
   ```markdown
   #### Y.1 Tarefa  # Era: #### X.1
   #### Y.2 Tarefa  # Era: #### X.2
   ```

3. **Atualizar resumo**:
   ```markdown
   ## 📊 Resumo da Fase Y  # Era: Fase X
   ```

4. **Atualizar referências internas**:
   - Buscar `Fase X` no arquivo
   - Substituir por `Fase Y`

---

### Passo 3: Atualizar Referências Externas

**Buscar e substituir**:

```bash
# Buscar todas as referências
grep -r "Fase X" docs/

# Arquivos principais a verificar:
# - docs/02_ROADMAP.md
# - docs/backlog-api/ROADMAP_VISUAL.md
# - docs/backlog-api/README.md
# - docs/backlog-api/MAPA_FASES.md
# - docs/backlog-api/FASE*.md (outros arquivos)
```

**Substituir**:
- `Fase X` → `Fase Y` (onde apropriado)
- `FASE[X].md` → `FASE[Y].md` (em links)

---

### Passo 4: Validar

**Checklist**:

- [ ] Fase Y tem número menor que fases que dependem dela?
- [ ] Fase Y tem número maior que fases das quais depende?
- [ ] Todas as referências foram atualizadas?
- [ ] Não há conflitos de numeração?

---

## 📋 Exemplo Completo: Mover Fase 24 → Fase 17

### Cenário
Mover "Compra Coletiva" de Fase 24 para Fase 17.

### Ações

1. **Atualizar `FASE24.md`** (ou `FASE17.md` se renomear):
   ```markdown
   # Fase 17: Sistema de Compra Coletiva  # Era: Fase 24
   
   ## 📋 Tarefas Detalhadas
   
   #### 17.1 Modelo de Domínio  # Era: 24.1
   #### 17.2 Sistema de Produtores  # Era: 24.2
   
   ## 📊 Resumo da Fase 17  # Era: Fase 24
   ```

2. **Atualizar dependências mencionadas**:
   - Se menciona "Fase 20 (Moeda Territorial)" → atualizar para "Fase 22"
   - Se menciona "Fase 17 (Gamificação)" → atualizar para "Fase 42"

3. **Atualizar `MAPA_FASES.md`**:
   ```markdown
   | **17** | Compra Coletiva | 🔴 P0 | 28d | FASE17.md | ⏳ Pendente |
   ```

4. **Atualizar `02_ROADMAP.md`**:
   ```markdown
   | **Fase 17** | Compra Coletiva | 🔴 P0 | 28 dias | ⏳ Planejado |
   ```

5. **Buscar referências em outros arquivos**:
   ```bash
   grep -r "Fase 24" docs/backlog-api/
   # Substituir por "Fase 17" onde apropriado
   ```

---

## ⚠️ Regras Importantes

### 1. Ordem de Dependências

**Regra**: Se Fase A depende de Fase B, então A > B

**Exemplo**:
- ✅ Fase 22 depende de Fase 6 → 22 > 6 (correto)
- ❌ Fase 15 depende de Fase 20 → 15 < 20 (errado, quebraria a regra)

### 2. Não Quebrar Fases Implementadas

**Regra**: Fases 1-8 já implementadas → manter numeração

**Exceção**: Apenas se houver motivo muito forte (raramente necessário)

### 3. Atualizar Todas as Referências

**Regra**: Sempre buscar e atualizar referências em:
- Roadmaps
- Outros arquivos FASE*.md
- Documentos de referência

---

## 🛠️ Comandos Úteis

### Buscar Referências

```bash
# Buscar todas as referências a uma fase
grep -r "Fase 20" docs/

# Buscar apenas em arquivos markdown
grep -r "Fase 20" docs/ --include="*.md"

# Buscar em arquivos específicos
grep "Fase 20" docs/backlog-api/*.md
```

### Validar Ordem

**Script mental**:
1. Listar todas as dependências de uma fase
2. Verificar se todas têm número menor
3. Listar todas as fases que dependem desta
4. Verificar se todas têm número maior

---

## 📝 Checklist Rápido

### Para Adicionar Nova Fase

- [ ] Escolher número disponível
- [ ] Criar arquivo `FASE[X].md` com template
- [ ] Atualizar `MAPA_FASES.md`
- [ ] Atualizar roadmaps
- [ ] Atualizar `README.md`

### Para Reorganizar Fase

- [ ] Decidir nova posição
- [ ] Verificar disponibilidade do número
- [ ] Atualizar título e numeração interna
- [ ] Atualizar referências externas
- [ ] Validar ordem de dependências
- [ ] Atualizar `MAPA_FASES.md`
- [ ] Atualizar roadmaps

---

## 🔗 Referências

- **Mapa Completo**: `docs/backlog-api/MAPA_FASES.md`
- **Reorganização Detalhada**: `docs/REORGANIZACAO_NUMERACAO_COERENTE.md`
- **Mapeamento Atualizado**: `docs/MAPEAMENTO_FASES_ATUALIZADO.md`

---

**Status**: ✅ **GUIA CRIADO**  
**Uso**: Consultar este guia sempre que precisar reorganizar ou adicionar fases
