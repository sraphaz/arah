# Demandas e Ofertas - Documentação Funcional (Planejada)

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: ⏳ **PLANEJADA - NÃO IMPLEMENTADA**  
**Fase**: 19  
**Prioridade**: 🔴 Crítica (Economia Local)  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## ⚠️ Status

Esta funcionalidade está **planejada** mas **ainda não implementada**. Detalhes podem mudar durante o desenvolvimento.

---

## 🎯 Visão Geral

O sistema de **Demandas e Ofertas** complementa o Marketplace oferecendo um fluxo bidirecional: moradores cadastram necessidades (demandas) e outros fazem ofertas para suprir essas necessidades.

### Objetivo

Permitir que:
- **Moradores** cadastrem necessidades de itens ou serviços
- **Outros usuários** façam ofertas para suprir essas necessidades
- **Negociação** aconteça antes de aceitar oferta
- **Economia local** seja facilitada de forma bidirecional

### Diferenciação

| Funcionalidade | Direção | Foco |
|----------------|---------|------|
| **Marketplace** | Oferta → Procura | Vendedor oferece, comprador procura |
| **Demandas/Ofertas** | Procura → Oferta | Comprador precisa, vendedor oferece |
| **Trocas** | Troca Direta | Troca de item/serviço por outro |
| **Compra Coletiva** | Organização Coletiva | Compra em grupo de produtores |

---

## 💼 Função de Negócio

### Para Demandantes

- Cadastrar demandas de itens ou serviços
- Visualizar ofertas recebidas
- Negociar com ofertantes
- Aceitar/rejeitar ofertas
- Finalizar transação após aceitar

### Para Ofertantes

- Visualizar demandas ativas
- Fazer ofertas para demandas
- Negociar com demandantes
- Ajustar ofertas durante negociação
- Finalizar transação quando oferta aceita

### Para a Comunidade

- **Economia Bidirecional**: Complementa Marketplace (procura → oferta)
- **Autonomia**: Comunidade resolve suas próprias necessidades
- **Flexibilidade**: Negociação permite ajustes antes de aceitar

---

## 🏗️ Elementos da Arquitetura (Planejados)

### Entidades Principais

#### Demand (Demanda)
- **Propósito**: Necessidade de item ou serviço
- **Tipos**: ITEM, SERVICE
- **Status**: ACTIVE, FULFILLED, CANCELLED, EXPIRED
- **Visibilidade**: PUBLIC, RESIDENT_ONLY

#### Offer (Oferta)
- **Propósito**: Oferta para suprir uma demanda
- **Status**: PENDING, ACCEPTED, REJECTED, NEGOTIATING, CANCELLED
- **Atributos**: Preço proposto, prazo, condições especiais

#### Negotiation (Negociação)
- **Propósito**: Processo de negociação entre demandante e ofertante
- **Características**: Mensagens, contrapropostas, ajustes

---

## 🔄 Fluxos Funcionais (Planejados)

### Fluxo 1: Criar Demanda

```
Morador → Cria Demanda → Informa Título/Descrição/Tipo → 
Define Localização → (Opcional) Orçamento/Prazo → 
Publica → Demanda Ativa → Ofertantes Visualizam
```

### Fluxo 2: Fazer Oferta

```
Ofertante → Visualiza Demanda → Faz Oferta → 
Informa Preço/Prazo/Condições → Envia → 
Demandante Recebe Notificação → Revisa Oferta
```

### Fluxo 3: Negociar e Aceitar

```
Demandante → Recebe Oferta → Inicia Negociação → 
Troca Mensagens/Ajustes → Ofertante Ajusta Oferta → 
Demandante Aceita → Transação Criada → 
Pagamento → Entrega/Prestação
```

---

## ⚙️ Regras de Negócio (Planejadas)

1. **Permissões**:
   - Criar demanda: Apenas moradores verificados
   - Fazer oferta: Todos usuários autenticados (ou apenas moradores, configurável)
   - Visualizar: Visitantes veem apenas demandas públicas

2. **Status de Demanda**:
   - ACTIVE: Aceitando ofertas
   - FULFILLED: Oferta aceita, demanda atendida
   - CANCELLED: Cancelada pelo demandante
   - EXPIRED: Expirada (se tiver prazo)

3. **Negociação**:
   - Múltiplas ofertas podem ser recebidas
   - Negociação acontece antes de aceitar
   - Apenas uma oferta pode ser aceita por demanda

4. **Integração com Pagamentos**:
   - Após aceitar oferta, transação criada
   - Pagamento processado via sistema existente
   - Escrow opcional para segurança

---

## 🔗 Integrações Planejadas

### Com Funcionalidades Existentes

- **Marketplace**: Complementa sistema existente
- **Pagamentos**: Sistema completo de pagamentos (Fase 6-7)
- **Notificações**: Notificações de ofertas e negociações
- **Territórios**: Demandas vinculadas a territórios

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Marketplace](./06_MARKETPLACE.md)** - Sistema complementar
- **[Fase 19 - Demandas e Ofertas](../backlog-api/FASE19.md)** - Detalhes técnicos do planejamento

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: ⏳ Planejada - Não Implementada
