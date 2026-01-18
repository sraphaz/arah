# Correção CSS DevPortal - Sobreposição de Conteúdo

**Arquivo**: `frontend/devportal/assets/css/devportal.css`  
**Linha**: ~597  
**Problema**: Conteúdo sobrepondo sidebar fixo à esquerda

---

## Problema

O `.layout` tem `padding-left` que não inclui o `left` offset do `.sidebar-container`.

### Estado Atual (Linha 597):
```css
@media (min-width: 1024px) {
  .layout {
    padding-left: calc(256px + clamp(var(--space-lg), 3vw, var(--space-xl)));
  }
}
```

### Sidebar (Linha 639):
```css
.sidebar-container {
  position: fixed;
  left: clamp(1.25rem, 4vw, 2.5rem);  /* ← Este offset NÃO está incluído no padding-left */
  width: 256px;
  ...
}
```

## Correção Necessária

**Alterar linha 597 de:**
```css
padding-left: calc(256px + clamp(var(--space-lg), 3vw, var(--space-xl)));
```

**Para:**
```css
padding-left: calc(256px + clamp(var(--space-lg), 3vw, var(--space-xl)) + clamp(1.25rem, 4vw, 2.5rem));
```

**E adicionar após a linha 598:**
```css
  /* Garantir que main e sections respeitam o padding */
  .layout > main,
  .layout > main > section {
    margin-left: 0;
    padding-left: 0;
  }
```

## Cálculo Completo

```
padding-left = 256px (largura do sidebar)
            + gap (clamp(1.5rem, 3vw, 2rem))
            + left offset do sidebar (clamp(1.25rem, 4vw, 2.5rem))
```

**Resultado**: `calc(256px + clamp(1.5rem, 3vw, 2rem) + clamp(1.25rem, 4vw, 2.5rem))`

---

**Aplicar manualmente no arquivo** `frontend/devportal/assets/css/devportal.css` na linha ~597.
