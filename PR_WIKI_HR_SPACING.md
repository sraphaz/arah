# Reduzir Espaçamento do HR na Wiki

## Resumo

Reduz o espaçamento vertical do elemento `<hr>` (separador horizontal) na Wiki pela metade, melhorando a densidade visual do conteúdo.

## 🎯 Mudança

### Antes
- `margin-top: 5rem` (80px)
- `margin-bottom: 5rem` (80px)
- Total: 160px de espaço vertical

### Depois
- `margin-top: 2.5rem` (40px)
- `margin-bottom: 2.5rem` (40px)
- Total: 80px de espaço vertical

## 📝 Detalhes Técnicos

**Arquivo modificado:**
- `frontend/wiki/app/globals.css`

**Alteração:**
```css
/* Antes */
@apply my-20 border-0; /* Mais espaço - quebra visual clara */

/* Depois */
@apply my-10 border-0; /* Espaçamento reduzido pela metade (2.5rem) */
```

## ✅ Benefícios

- ✅ Melhora densidade visual do conteúdo
- ✅ Reduz scroll desnecessário
- ✅ Mantém separação visual adequada
- ✅ Alinhado com padrões de design modernos

## 🧪 Testes

- [x] Verificado visualmente em modo light
- [x] Verificado visualmente em modo dark
- [x] Separador mantém funcionalidade e aparência

## 📸 Screenshots

_Adicionar screenshots antes/depois se necessário_

---

**Tipo**: Style  
**Escopo**: Wiki  
**Impacto**: Baixo (apenas ajuste visual)
