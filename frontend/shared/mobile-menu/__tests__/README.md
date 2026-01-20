# Testes do Módulo Compartilhado de Menu Mobile

## Estrutura

Os testes estão organizados em suites que cobrem todas as funcionalidades do módulo:

- **Inicialização**: Validação da inicialização e configuração
- **Abertura e Fechamento**: Testa a aplicação/remoção de classes CSS
- **Eventos de Clique**: Valida interações do usuário
- **Tecla ESC**: Testa fechamento via teclado
- **Resize para Desktop**: Valida comportamento responsivo
- **Callback onLinkClick**: Testa callbacks customizados
- **Overlay**: Valida criação e remoção do overlay
- **Acessibilidade**: Testa atributos ARIA e foco
- **Estados do Botão**: Valida classes CSS do botão
- **Edge Cases**: Testa casos extremos

## Como Executar

### Opção 1: Do diretório DevPortal (recomendado)

```bash
cd frontend/devportal
npm test -- mobile-menu
```

### Opção 2: Todos os testes do DevPortal

```bash
cd frontend/devportal
npm test
```

### Opção 3: Modo watch (desenvolvimento)

```bash
cd frontend/devportal
npm run test:watch
```

## Cobertura

Os testes cobrem:
- ✅ Inicialização com/sem elementos
- ✅ Abertura e fechamento do menu
- ✅ Aplicação de classes CSS (`.sidebar-mobile-open`, `.active`, etc.)
- ✅ Criação e remoção do overlay
- ✅ Eventos de clique (botão, links, fundo)
- ✅ Tecla ESC
- ✅ Resize para desktop
- ✅ Callbacks customizados
- ✅ Atributos ARIA
- ✅ Foco em primeiro link
- ✅ Edge cases

## Metodologia

Os testes seguem a abordagem de **classes CSS aplicadas via JS**:

1. **CSS define estilos**: Classes como `.sidebar-mobile-open` definem como a sidebar deve aparecer
2. **JS controla estado**: JavaScript adiciona/remove classes baseado em interações
3. **Testes validam classes**: Testes verificam que classes corretas são aplicadas/removidas

Isso torna os testes:
- **Fáceis de debugar**: Veja classes no DevTools
- **Manuteníveis**: Mudanças visuais = CSS, mudanças de lógica = JS
- **Previsíveis**: Classes têm nomes claros e específicos
