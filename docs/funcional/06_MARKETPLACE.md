# Marketplace - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

O **Marketplace** é o sistema de trocas locais integrado ao território. Permite que moradores criem lojas, cadastrem produtos/serviços e realizem transações locais.

### Objetivo

Permitir que usuários:
- **Criem lojas** no território
- **Cadastrem produtos/serviços**
- **Naveguem e busquem** itens
- **Comprem e vendam** localmente
- **Gerenciem carrinho e checkout**

---

## 💼 Função de Negócio

### Para o Usuário

**Como Vendedor**:
- Criar loja (após verificação)
- Cadastrar produtos/serviços
- Receber inquiries (consultas)
- Gerenciar vendas e receber payouts

**Como Comprador**:
- Navegar lojas e itens
- Adicionar ao carrinho
- Finalizar compra (checkout)
- Fazer inquiries sobre itens

### Para a Comunidade

- **Economia Local**: Facilitar trocas comunitárias
- **Sustentabilidade**: Economia circular territorial
- **Autonomia**: Comércio local sem intermediários externos

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Store
- **Propósito**: Loja/comércio no território
- **Atributos**: Nome, descrição, contato, status, paymentsEnabled

#### StoreItem
- **Propósito**: Produto ou serviço
- **Tipos**: PRODUCT, SERVICE
- **Pricing**: FREE, FIXED, NEGOTIABLE
- **Media**: Múltiplas imagens (até 10), vídeos, áudios

#### Cart
- **Propósito**: Carrinho de compras
- **Características**: Por usuário e território

#### Checkout
- **Propósito**: Finalização de compra
- **Integração**: Stripe/Mercado Pago

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Criar Loja

```
Morador Verificado → Marketplace → Criar Loja → 
Informa Nome/Descrição/Contato → Publica → 
Loja criada (Status: ACTIVE)
```

### Fluxo 2: Cadastrar Item

```
Vendedor → Minha Loja → Cadastrar Item → 
Informa Título/Descrição/Tipo/Preço → Publica → 
Item disponível no marketplace
```

### Fluxo 3: Comprar Item

```
Comprador → Navega Marketplace → Seleciona Item → 
Adiciona ao Carrinho → Finaliza Compra → 
Checkout → Pagamento → Vendedor recebe Payout
```

---

## ⚙️ Regras de Negócio

1. **Permissão**: Apenas moradores verificados podem criar lojas
2. **Feature Flag**: MARKETPLACEENABLED controla habilitação
3. **Não vende Assets**: Items não podem vender TerritoryAssets
4. **Carrinho**: Por usuário e território
5. **Checkout**: Calcula taxas de plataforma

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Territórios e Memberships](./02_TERRITORIOS_MEMBERSHIPS.md)** - Verificação necessária
- **[Assets](./09_ASSETS.md)** - Diferenciação: Assets não são vendáveis
- **[API - Marketplace](../api/60_09_API_MARKETPLACE.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
