# Proposta: Navegação Progressiva por Fases - DevPortal

**Data**: 2025-01-20  
**Versão**: 1.0

---

## 📋 Problema Atual

O DevPortal renderiza **todo o conteúdo de uma vez**, resultando em:
- **Scroll excessivo** (página muito longa)
- **Sobrecarga cognitiva** (muita informação simultânea)
- **Falta de progressão clara** (não há jornada estruturada)
- **Performance degradada** (DOM muito grande)

---

## 🎯 Objetivo

Reorganizar a navegação do DevPortal para **revelar conhecimento em fases progressivas**, criando uma **jornada clara** do básico ao avançado.

---

## 🏗️ Estrutura Proposta

### Fases de Conteúdo (Jornada Progressiva)

#### **Fase 1: Começando** 🚀
- Quickstart (5-10 comandos)
- Autenticação (JWT básico)
- Território & Headers (essencial)

**Objetivo**: Fazer funcionar em 10 minutos

#### **Fase 2: Fundamentos** 📚
- Visão geral
- Como o Arah funciona (visitor → resident)
- Territórios (conceito)
- Conceitos de produto (semântica)

**Objetivo**: Entender os conceitos centrais

#### **Fase 3: API Prática** 🔧
- Fluxos principais
- Casos de uso (cenários práticos)
- OpenAPI / Explorer
- Erros & convenções

**Objetivo**: Usar a API de forma prática

#### **Fase 4: Funcionalidades** ⚙️
- Marketplace
- Payout & Gestão Financeira
- Eventos
- Modelo de domínio (diagrama)

**Objetivo**: Entender funcionalidades específicas

#### **Fase 5: Recursos Avançados** 🎓
- Admin & filas
- Capacidades técnicas
- Versões & compatibilidade
- Roadmap
- Contribuir

**Objetivo**: Tópicos avançados e contribuição

---

## 🎨 Solução de Navegação

### Opção A: **Tabs Principais** (Recomendado)

**Vantagens:**
- ✅ Navegação visual clara
- ✅ Apenas uma fase visível por vez
- ✅ Progressão linear explícita
- ✅ Menor DOM ativo

**Implementação:**
```html
<div class="phase-tabs">
  <button class="phase-tab active" data-phase="comecando">🚀 Começando</button>
  <button class="phase-tab" data-phase="fundamentos">📚 Fundamentos</button>
  <button class="phase-tab" data-phase="api-pratica">🔧 API Prática</button>
  <button class="phase-tab" data-phase="funcionalidades">⚙️ Funcionalidades</button>
  <button class="phase-tab" data-phase="avancado">🎓 Avançado</button>
</div>

<div class="phase-content">
  <div class="phase-panel active" data-phase-panel="comecando">...</div>
  <div class="phase-panel" data-phase-panel="fundamentos">...</div>
  <div class="phase-panel" data-phase-panel="api-pratica">...</div>
  <div class="phase-panel" data-phase-panel="funcionalidades">...</div>
  <div class="phase-panel" data-phase-panel="avancado">...</div>
</div>
```

### Opção B: **Sidebar Hierárquica com Renderização Condicional**

**Vantagens:**
- ✅ Mantém sidebar atual
- ✅ Renderiza apenas seções necessárias
- ✅ Scroll menor por seção

**Implementação:**
- Sidebar organizada por fases
- Renderização via JavaScript (show/hide sections)
- URL hash (#fase1, #fase2, etc.)

---

## 📊 Comparação de Abordagens

| Aspecto | Opção A (Tabs) | Opção B (Sidebar Condicional) |
|---------|---------------|-------------------------------|
| **Clareza visual** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Scroll reduzido** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Manutenibilidade** | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Compatibilidade** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Progressão clara** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |

---

## 🎯 Recomendação: **Opção A (Tabs Principais)**

### Por quê?
1. **Progressão explícita**: Usuário vê claramente as fases
2. **Scroll mínimo**: Apenas conteúdo da fase ativa
3. **Jornada guiada**: "Começando" → "Fundamentos" → "API Prática" → etc.
4. **Performance**: DOM reduzido (apenas fase ativa renderizada)

---

## 🛠️ Implementação Proposta

### 1. Estrutura HTML

```html
<!-- Tabs de navegação principal -->
<nav class="phase-navigation" role="tablist" aria-label="Fases de conteúdo">
  <button class="phase-tab active" role="tab" aria-selected="true" aria-controls="panel-comecando" id="tab-comecando" data-phase="comecando">
    <span class="phase-icon">🚀</span>
    <span class="phase-title">Começando</span>
    <span class="phase-subtitle">10 minutos</span>
  </button>
  <button class="phase-tab" role="tab" aria-selected="false" aria-controls="panel-fundamentos" id="tab-fundamentos" data-phase="fundamentos">
    <span class="phase-icon">📚</span>
    <span class="phase-title">Fundamentos</span>
    <span class="phase-subtitle">Conceitos</span>
  </button>
  <!-- ... mais tabs ... -->
</nav>

<!-- Panels de conteúdo -->
<div class="phase-panels">
  <div class="phase-panel active" role="tabpanel" aria-labelledby="tab-comecando" id="panel-comecando" data-phase-panel="comecando">
    <!-- Conteúdo Fase 1 -->
  </div>
  <div class="phase-panel" role="tabpanel" aria-labelledby="tab-fundamentos" id="panel-fundamentos" data-phase-panel="fundamentos">
    <!-- Conteúdo Fase 2 -->
  </div>
  <!-- ... mais panels ... -->
</div>
```

### 2. CSS para Transições

```css
.phase-panel {
  display: none;
  opacity: 0;
  transform: translateY(10px);
  transition: opacity 0.3s ease, transform 0.3s ease;
}

.phase-panel.active {
  display: block;
  opacity: 1;
  transform: translateY(0);
}
```

### 3. JavaScript para Troca de Fases

```javascript
function initPhaseNavigation() {
  const tabs = document.querySelectorAll('.phase-tab');
  const panels = document.querySelectorAll('.phase-panel');
  
  tabs.forEach(tab => {
    tab.addEventListener('click', () => {
      const targetPhase = tab.dataset.phase;
      
      // Remove active de todos
      tabs.forEach(t => t.classList.remove('active'));
      panels.forEach(p => p.classList.remove('active'));
      
      // Adiciona active no alvo
      tab.classList.add('active');
      document.querySelector(`[data-phase-panel="${targetPhase}"]`).classList.add('active');
      
      // Atualiza URL (sem reload)
      history.pushState(null, '', `#${targetPhase}`);
    });
  });
}
```

---

## 📈 Benefícios Esperados

1. **Redução de Scroll**: ~80% menos scroll (apenas fase ativa)
2. **Clareza**: Jornada progressiva explícita
3. **Performance**: DOM ~70% menor
4. **UX**: Navegação intuitiva e guiada
5. **Manutenibilidade**: Conteúdo organizado por fase

---

## ✅ Critérios de Sucesso

- [ ] Scroll reduzido em ~80%
- [ ] Navegação clara entre fases
- [ ] Performance melhorada (DOM menor)
- [ ] Jornada progressiva explícita
- [ ] Mantém funcionalidade atual (sidebar, scroll sync, etc.)

---

## 🔄 Próximos Passos

1. **Validar proposta** com equipe
2. **Implementar Opção A** (Tabs Principais)
3. **Reorganizar conteúdo** em fases
4. **Testar navegação** e performance
5. **Ajustar CSS** para transições suaves
6. **Validar acessibilidade** (ARIA, keyboard navigation)
