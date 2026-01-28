# Alertas - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

**Alertas** são notificações de saúde pública e comunicação emergencial no território. Permitem que comunidades comuniquem informações urgentes e importantes de forma destacada.

### Objetivo

Permitir que usuários:
- **Reportem alertas** de saúde pública
- **Visualizem alertas** validados
- **Validem alertas** (curadores)
- **Recebam notificações** de alertas importantes

---

## 💼 Função de Negócio

### Para o Usuário

- Reportar alertas de saúde pública
- Visualizar alertas validados do território
- Receber notificações de alertas importantes

### Para a Comunidade

- **Comunicação Emergencial**: Alertar sobre situações urgentes
- **Saúde Pública**: Compartilhar informações de saúde
- **Validação**: Curadores validam antes de publicação

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Alert
- **Propósito**: Alerta de saúde pública
- **Atributos**: Título, descrição, status (PENDING, VALIDATED)
- **Características**: Cria post automático no feed quando validado

---

## 🔄 Fluxos Funcionais

### Fluxo: Reportar e Validar Alerta

```
Usuário → Reporta Alerta → Status: PENDING → 
Curador Revisa → Valida → Status: VALIDATED → 
Post automático criado no Feed → 
Notificações enviadas
```

---

## ⚙️ Regras de Negócio

1. **Permissão**: Visitantes e moradores podem reportar
2. **Validação**: Apenas curadores podem validar
3. **Post Automático**: Cria post ALERT no feed quando validado
4. **Feature Flag**: Requer flag ALERTPOSTS habilitada
5. **Visibilidade**: Apenas alertas validados são retornados

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Feed Comunitário](./03_FEED_COMUNITARIO.md)** - Posts automáticos
- **[API - Alertas](../api/60_07_API_ALERTAS.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
