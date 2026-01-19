# DevPortal - Explicação Completa da Navegação

**Data**: 2025-01-20
**Versão**: 1.0
**Status**: ✅ DOCUMENTAÇÃO COMPLETA - Lógica, estrutura e problemas documentados

---

## 📋 Sumário Executivo

**Testes Rodados**: ✅ 17 testes executados
**Testes Passando**: ✅ 15 testes (88%)
**Testes Falhando**: ❌ 2 testes (12% - IDs duplicados)

**Problemas Identificados**:
1. **2 IDs duplicados**: `id="admin"` e `id="eventos"` aparecem 2 vezes cada
2. **Conteúdo fora de phase-panels**: Algumas seções estão fora dos phase-panels correspondentes
3. **Phase-panels vazios**: Phase-panels `fundamentos` e `api-pratica` estão vazios

---

## 🔍 Lógica de Navegação

### Como o Conteúdo Aparece

**Fluxo quando clica em um link da sidebar:**

1. **Link clicado**: `<a href="#visao-geral" class="sidebar-link">`
2. **JavaScript intercepta**: `initSidebarNavigation()` em `devportal.js` captura o clique
3. **Mapeamento**: `sectionToPhase['visao-geral']` → retorna `'fundamentos'`
4. **Ativa phase-panel**: `window.switchPhase('fundamentos')` é chamado
5. **Esconde outros panels**: Todos os phase-panels são escondidos via CSS (`display: none`)
6. **Mostra panel alvo**: O `phase-panel[data-phase-panel="fundamentos"]` é mostrado (`display: block`)
7. **Exibe conteúdo**: Todo o conteúdo dentro desse phase-panel fica visível
8. **Scroll para topo**: Navega até o topo da página (não até a seção específica)

**Fluxo quando clica em um tab central:**

1. **Tab clicado**: `<button data-phase="fundamentos">`
2. **JavaScript intercepta**: `initPhaseNavigation()` em `devportal.js` captura o clique
3. **Ativa phase-panel**: `switchPhase('fundamentos')` é chamado diretamente
4. **Mesmo processo**: Esconde outros panels e mostra o alvo

### Função `switchPhase()`

```javascript
function switchPhase(phase) {
  // 1. Remove active de todos os tabs
  tabs.forEach(t => t.classList.remove('active'));

  // 2. Esconde TODOS os panels (via style inline)
  panels.forEach(p => {
    p.classList.remove('active');
    p.style.display = 'none';
    p.style.visibility = 'hidden';
    p.style.opacity = '0';
    p.style.height = '0';
    p.style.overflow = 'hidden';
  });

  // 3. Esconde TODAS as seções que são filhos diretos de main (fora de phase-panels)
  var mainSections = document.querySelectorAll('main > section:not(.phase-panel):not(.phase-panels)');
  mainSections.forEach(section => {
    section.style.display = 'none';
    section.style.visibility = 'hidden';
    section.style.height = '0';
    section.style.overflow = 'hidden';
    section.style.opacity = '0';
  });

  // 4. Ativa o panel alvo
  var targetTab = document.querySelector('[data-phase="' + phase + '"]');
  var targetPanel = document.querySelector('[data-phase-panel="' + phase + '"]');
  
  if (targetTab && targetPanel) {
    targetTab.classList.add('active');
    targetPanel.classList.add('active');
    targetPanel.style.display = 'block';
    targetPanel.style.visibility = 'visible';
    targetPanel.style.opacity = '1';
    targetPanel.style.height = 'auto';
    targetPanel.style.overflow = 'visible';
  }
}
```

**Regra Fundamental**: Apenas **UM** phase-panel pode estar ativo por vez. Quando um panel é ativado, todos os outros são escondidos.

---

## 📊 Diferença Contextual entre Menus

### Menu Lateral (Sidebar) - Navegação por Assunto

**Contexto**: **"O que estou procurando?"** - Navegação por **assunto/tema**

**Estrutura**:
- **Fundamentos**: Conceitos teóricos e visão geral
- **API & Referência**: Documentação técnica e exemplos
- **Funcionalidades**: Features específicas da plataforma
- **Recursos**: Informações avançadas e contribuição

**Comportamento**:
- Links usam `href="#section-id"` para referenciar seções específicas
- Quando clicado, ativa o **phase-panel inteiro** que contém aquela seção
- Exibe **TODO o conteúdo** daquele phase-panel (não apenas a seção clicada)

**Exemplo**:
- Clique em "Marketplace" → Ativa `phase-panel[data-phase-panel="funcionalidades"]` → Mostra TODO o conteúdo de funcionalidades

**Uso**: "Quero ver informações sobre o Marketplace" → Clique em Marketplace → Vê todo o conteúdo de Funcionalidades

### Menu Central (Tabs) - Navegação por Fase

**Contexto**: **"Em que estágio estou?"** - Navegação por **fase de aprendizado**

**Estrutura**:
- **Começando**: Primeira vez no portal
- **Fundamentos**: Aprendendo conceitos base
- **API Prática**: Implementando integração
- **Funcionalidades**: Explorando features
- **Avançado**: Aprofundamento e contribuição

**Comportamento**:
- Tabs usam `data-phase` para identificar phase-panels
- Quando clicado, ativa o **phase-panel correspondente**
- Exibe **TODO o conteúdo** daquele phase-panel

**Exemplo**:
- Clique em "Fundamentos" → Ativa `phase-panel[data-phase-panel="fundamentos"]` → Mostra TODO o conteúdo de fundamentos

**Uso**: "Estou na fase de fundamentos" → Clique em Fundamentos → Vê todo o conteúdo de Fundamentos

---

## 🗂️ Como o Conteúdo Foi Dividido

### Divisão Original (Antes)

O conteúdo original estava em um grande "linguiça de texto empilhado" - todas as seções estavam uma após a outra, sem organização por phase-panels.

### Divisão Atual (Depois)

O conteúdo foi dividido em **5 phase-panels** principais:

1. **Começando**: Introdução e boas-vindas
2. **Fundamentos**: Conceitos base
3. **API Prática**: Referência técnica
4. **Funcionalidades**: Features específicas
5. **Avançado**: Recursos avançados

### Como os Menus Fazem Referência ao Conteúdo

**Menu Lateral (Sidebar)**:
- Cada link aponta para uma seção específica (`href="#section-id"`)
- JavaScript mapeia essa seção para um phase-panel (`sectionToPhase[sectionId]`)
- Quando clicado, ativa o phase-panel correspondente

**Menu Central (Tabs)**:
- Cada tab aponta diretamente para um phase-panel (`data-phase="fundamentos"`)
- Quando clicado, ativa o phase-panel correspondente

---

## 📐 Critérios de Seleção de Conteúdo por Link

### Fundamentos

**Critério**: Conceitos base, visão geral, arquitetura - **"O que preciso entender ANTES de usar a API?"**

**Seções**:
- `visao-geral`: Visão geral da API e plataforma
- `como-funciona`: Como o Araponga funciona (do visitante ao morador)
- `territorios`: Conceito de território como unidade primária
- `conceitos`: Conceitos de produto e semântica de negócio
- `modelo-dominio`: Modelo de domínio e relacionamentos

**Lógica**: Tudo que é necessário entender **ANTES** de começar a usar a API

### API Prática

**Critério**: Referência técnica, exemplos de código, endpoints - **"O que preciso para IMPLEMENTAR a integração?"**

**Seções**:
- `fluxos`: Fluxos principais (diagramas de sequência)
- `casos-de-uso`: Casos de uso práticos e jornadas
- `openapi`: OpenAPI Explorer (Swagger UI)
- `erros`: Erros & convenções

**Lógica**: Tudo que é necessário para **IMPLEMENTAR** a integração com a API

### Funcionalidades

**Critério**: Features específicas da plataforma - **"Quais funcionalidades específicas existem?"**

**Seções**:
- `marketplace`: Marketplace e economia local territorial
- `eventos`: Eventos territoriais com data/hora e localização
- `payout-gestao-financeira`: Sistema de payout e gestão financeira
- `admin`: Admin & Filas (governança e administração)

**Lógica**: Tudo relacionado a **FUNCIONALIDADES** concretas da plataforma

### Avançado

**Critério**: Recursos avançados, versionamento, contribuição - **"O que preciso para APROFUNDAR ou CONTRIBUIR?"**

**Seções**:
- `capacidades-tecnicas`: Capacidades técnicas avançadas
- `versoes`: Versões & compatibilidade
- `roadmap`: Roadmap e estratégia
- `contribuir`: Contribuir no projeto

**Lógica**: Tudo para quem quer **APROFUNDAR** conhecimentos ou **CONTRIBUIR** no projeto

---

## ⚠️ Problemas Identificados

### 1. IDs Duplicados ❌

**Problema**: 2 IDs aparecem duplicados no HTML

**IDs Duplicados**:
1. **`id="admin"`**: 
   - **Localização 1**: Linha ~843 (dentro de `phase-panel[data-phase-panel="funcionalidades"]`)
   - **Localização 2**: Linha ~2190 (fora de phase-panel)

2. **`id="eventos"`**:
   - **Localização 1**: Linha ~781 (dentro de `phase-panel[data-phase-panel="funcionalidades"]`)
   - **Localização 2**: Linha ~2024 (fora de phase-panel)

**Causa**: Conteúdo duplicado - seções foram movidas para dentro do phase-panel, mas as versões antigas não foram removidas

**Impacto**: 
- HTML inválido (IDs devem ser únicos)
- Navegação pode falhar (JavaScript pode encontrar a seção errada)
- Testes falham (validação de IDs únicos)

**Solução**: Remover as seções duplicadas que estão fora dos phase-panels

### 2. Conteúdo Fora de Phase-Panels ⚠️

**Problema**: Algumas seções ainda estão fora dos phase-panels correspondentes

**Seções Encontradas Fora de Phase-Panels**:
- `section#eventos` (linha ~2024) - deveria estar dentro de `phase-panel[data-phase-panel="funcionalidades"]`
- `section#admin` (linha ~2190) - deveria estar dentro de `phase-panel[data-phase-panel="funcionalidades"]`

**Causa**: Conteúdo não foi completamente movido para dentro dos phase-panels

**Impacto**: 
- Quando `switchPhase()` é chamado, essas seções são escondidas (pois estão fora de phase-panels)
- O phase-panel pode aparecer vazio ou incompleto
- Navegação confusa

**Solução**: Remover as seções que estão fora dos phase-panels (elas já existem dentro)

### 3. Phase-Panels Vazios ⚠️

**Problema**: Phase-panels `fundamentos` e `api-pratica` estão vazios

**Phase-Panels Vazios**:
```html
<div class="phase-panel" data-phase-panel="fundamentos">
  <!-- Conteúdo será movido aqui -->
</div>

<div class="phase-panel" data-phase-panel="api-pratica">
  <!-- Conteúdo será movido aqui -->
</div>
```

**Causa**: Conteúdo não foi movido para dentro desses phase-panels

**Impacto**: 
- Quando usuário clica em "Fundamentos" ou "API Prática", não aparece conteúdo
- Usuário vê uma página vazia

**Solução**: Mover o conteúdo para dentro dos phase-panels correspondentes

---

## 🧪 Resultados dos Testes Automatizados

### Testes Implementados

**Arquivo**: `frontend/devportal/__tests__/content-navigation.test.js`

**Testes Criados**:
1. ✅ Validação de estrutura (sidebar e phase-tabs existem)
2. ✅ Validação de mapeamento (links apontam para seções existentes)
3. ✅ Validação de localização (seções estão dentro dos phase-panels corretos)
4. ✅ Validação de IDs únicos (não há IDs duplicados)
5. ✅ Validação de links (todos os links apontam para seções que existem)

### Resultados dos Testes

```
Test Suites: 1 failed, 1 total
Tests:       2 failed, 15 passed, 17 total
```

**Testes Passando (15)**:
- ✅ Estrutura de navegação (4 testes)
- ✅ Mapeamento de seções (2 testes)
- ✅ Validação de conteúdo por phase-panel (5 testes)
- ✅ Validação de links (2 testes)
- ✅ Critérios de organização (2 testes)

**Testes Falhando (2)**:
- ❌ Validação de IDs únicos - IDs duplicados encontrados
- ❌ Nenhum ID deve estar duplicado - 2 IDs duplicados

### Comando para Rodar os Testes

```bash
cd frontend/devportal
npm test -- __tests__/content-navigation.test.js
```

---

## ✅ Recomendações de Correção

### 1. Remover IDs Duplicados

**Ação**: Remover as seções duplicadas que estão fora dos phase-panels:

- Remover `section#admin` da linha ~2190 (já existe dentro do phase-panel)
- Remover `section#eventos` da linha ~2024 (já existe dentro do phase-panel)

### 2. Validar Phase-Panels Vazios

**Ação**: Mover conteúdo para dentro dos phase-panels vazios:

- Mover seções de `fundamentos` para `phase-panel[data-phase-panel="fundamentos"]`
- Mover seções de `api-pratica` para `phase-panel[data-phase-panel="api-pratica"]`

### 3. Rodar Testes Novamente

**Ação**: Após correções, rodar os testes novamente:

```bash
cd frontend/devportal
npm test -- __tests__/content-navigation.test.js
```

---

## 📝 Resumo Final

### Lógica de Navegação

**Menu Lateral (Sidebar)**:
- Navegação por **assunto/tema**
- Links apontam para seções específicas (`href="#section-id"`)
- JavaScript mapeia seção → phase-panel (`sectionToPhase`)
- Ativa phase-panel correspondente e mostra TODO o conteúdo

**Menu Central (Tabs)**:
- Navegação por **fase de aprendizado**
- Tabs apontam diretamente para phase-panels (`data-phase`)
- Ativa phase-panel correspondente e mostra TODO o conteúdo

### Diferença Contextual

**Sidebar**: "O que estou procurando?" → Navega por assunto → Ativa phase-panel correspondente

**Tabs**: "Em que estágio estou?" → Navega por fase → Ativa phase-panel correspondente

### Problemas Atuais

1. ❌ **2 IDs duplicados** (`admin` e `eventos`)
2. ⚠️ **Conteúdo fora de phase-panels** (seções duplicadas)
3. ⚠️ **Phase-panels vazios** (`fundamentos` e `api-pratica`)

### Validação

**Testes Automatizados**: ✅ 15/17 testes passando (88%)
**Problemas Identificados**: ✅ Todos documentados e com soluções propostas

---

**Última Atualização**: 2025-01-20
