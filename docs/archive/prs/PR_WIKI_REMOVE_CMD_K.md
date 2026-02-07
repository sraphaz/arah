# Remover Referências Visuais ao Atalho Cmd/Ctrl+K na Wiki

## Resumo

Remove apenas as referências visuais ao atalho `Cmd/Ctrl+K` na Wiki. A funcionalidade do atalho é mantida, funcionando silenciosamente sem indicações visuais.

## 🎯 Mudança

### Antes
- Atalho `Cmd/Ctrl+K` abria o diálogo de busca
- Botão exibia visualmente "⌘K" indicando o atalho
- Placeholder do input mencionava o atalho
- aria-label do botão mencionava o atalho

### Depois
- **Atalho `Cmd/Ctrl+K` continua funcionando** (funcionalidade mantida)
- Removidas todas as referências visuais ao atalho
- Botão não exibe mais "⌘K"
- Placeholder não menciona mais o atalho
- aria-label não menciona mais o atalho
- Atalho `Escape` ainda fecha o diálogo quando aberto

## 📝 Detalhes Técnicos

**Arquivos modificados:**
- `frontend/wiki/components/search/SearchTrigger.tsx`
- `frontend/wiki/components/search/SearchDialog.tsx`

**Alterações:**

1. **SearchTrigger.tsx**:
   - **Mantido event listener que captura `Cmd/Ctrl+K`** (funcionalidade preservada)
   - Removido elemento `<kbd>` que exibia "⌘K"
   - Atualizado `aria-label` do botão (removida referência ao atalho)
   - Mantido event listener para `Escape` (fechar diálogo)

2. **SearchDialog.tsx**:
   - Removida referência "(Cmd/Ctrl + K)" do placeholder

## ✅ Benefícios

- ✅ Simplifica interface (menos informações visuais)
- ✅ Atalho continua funcionando para usuários que conhecem
- ✅ Busca continua totalmente funcional via botão e atalho
- ✅ Mantém navegação por teclado dentro do diálogo (↑↓, Enter, Esc)

## 🧪 Testes

- [x] Botão de busca abre diálogo corretamente
- [x] Atalho Escape fecha o diálogo
- [x] Navegação por teclado funciona dentro do diálogo
- [x] Cmd/Ctrl+K continua abrindo o diálogo (funcionalidade mantida)

---

**Tipo**: Style  
**Escopo**: Wiki  
**Impacto**: Baixo (removendo apenas referências visuais, funcionalidade mantida)
