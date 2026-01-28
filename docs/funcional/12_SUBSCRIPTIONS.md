# Subscriptions - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

**Subscriptions** é o sistema de assinaturas e pagamentos recorrentes da plataforma. Permite sustentabilidade financeira mantendo acesso básico gratuito para todos.

### Objetivo

Permitir que usuários:
- **Assinem planos** pagos (opcional)
- **Acessem funcionalidades** baseadas no plano
- **Gerenciem assinaturas** (upgrade, downgrade, cancelamento)
- **Usem período de trial** (se disponível)

### Princípios

- ✅ **Acesso Básico Gratuito**: Funcionalidades essenciais sempre disponíveis
- ✅ **Inclusão**: Ninguém é excluído por não poder pagar
- ✅ **Transparência**: Status da assinatura sempre visível
- ✅ **Sustentabilidade**: Base para receitas recorrentes

---

## 💼 Função de Negócio

### Para o Usuário

**Plano FREE (Gratuito)**:
- Funcionalidades básicas sempre disponíveis
- Feed, posts, eventos, marketplace básico
- Sem necessidade de pagamento

**Planos Pagos**:
- Básico, Intermediário, Premium
- Funcionalidades progressivamente liberadas
- Pagamento recorrente (mensal, trimestral, anual)

### Para a Plataforma

- **Sustentabilidade**: Receitas recorrentes
- **Flexibilidade**: Múltiplos planos e opções
- **Confiabilidade**: Processamento robusto de renovações

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Subscription
- **Propósito**: Assinatura do usuário
- **Atributos**: Plano, status, data início/fim, trial

#### Plan
- **Propósito**: Plano de assinatura
- **Tipos**: FREE, BASIC, INTERMEDIATE, PREMIUM
- **Escopo**: Global ou Territorial

#### Coupon
- **Propósito**: Cupom de desconto
- **Tipos**: Percentual ou fixo

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Assinar Plano

```
Usuário → Seleciona Plano → 
Escolhe Ciclo (mensal/trimestral/anual) → 
Aplica Cupom (opcional) → 
Checkout → Pagamento → 
Assinatura Ativa → Funcionalidades Liberadas
```

### Fluxo 2: Renovação Automática

```
Assinatura Ativa → Data de Renovação → 
Sistema Processa Pagamento → 
Sucesso → Renovação Confirmada → 
Falha → Retry → 
Múltiplas Falhas → Suspensão
```

### Fluxo 3: Upgrade/Downgrade

```
Usuário → Gerencia Assinatura → 
Escolhe Novo Plano → 
Sistema Calcula Diferença → 
Processa Pagamento/Reembolso → 
Plano Atualizado
```

---

## ⚙️ Regras de Negócio

1. **Plano FREE**: Padrão, sempre disponível
2. **Planos Globais**: Aplicam a todos os territórios
3. **Planos Territoriais**: Específicos de um território (sobrescrevem globais)
4. **Renovação**: Automática, com retry em caso de falha
5. **Trial**: Opcional, conversão automática ao final
6. **Cupons**: Percentual ou fixo, com validade e limites

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Marketplace](./06_MARKETPLACE.md)** - Integração com pagamentos
- **[Fase 15 - Subscriptions](../backlog-api/FASE15.md)** - Detalhes técnicos

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
