# Compra Coletiva - Documentação Funcional (Planejada)

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: ⏳ **PLANEJADA - NÃO IMPLEMENTADA**  
**Fase**: 17  
**Prioridade**: 🔴 Crítica (Economia Local)  
**Parte de**: [Documentação Funcional da Plataforma](funcional/00_PLATAFORMA_ARAPONGA.md)

---

## ⚠️ Status

Esta funcionalidade está **planejada** mas **ainda não implementada**. Detalhes podem mudar durante o desenvolvimento.

---

## 🎯 Visão Geral

O sistema de **Compra Coletiva** permite organizar compras comunitárias de alimentos e produtos locais, conectando produtores locais com consumidores do território através de rodadas de compra organizadas.

### Objetivo

Permitir que comunidades:
- **Organizem compras coletivas** de produtos locais
- **Conectem produtores** com consumidores do território
- **Tomem decisões coletivas** sobre o que comprar (via votação)
- **Reduzam custos** através de compras em grupo
- **Fortaleçam economia local** e soberania alimentar

---

## 💼 Função de Negócio

### Para Produtores

- Cadastrar produtos disponíveis
- Definir preços e quantidades
- Gerenciar disponibilidade sazonal
- Receber pedidos coletivos
- Integrar com sistema de entregas

### Para Consumidores

- Indicar interesse em produtos
- Participar de rodadas de compra
- Ver agenda de compras comunitárias
- Receber notificações sobre compras
- Pagar em moeda territorial ou fiat

### Para Organizadores

- Criar rodadas de compra coletiva
- Gerenciar agenda de compras
- Organizar entregas coletivas
- Integrar com sistema de votação para decisões

### Para a Comunidade

- **Soberania Alimentar**: Decidir coletivamente o que comprar
- **Economia Local**: Fortalecer produtores locais
- **Sustentabilidade**: Reduzir desperdício e transporte
- **Organização**: Facilitar compras comunitárias

---

## 🏗️ Elementos da Arquitetura (Planejados)

### Entidades Principais

#### Producer (Produtor)
- **Propósito**: Representar produtor local
- **Atributos**: Nome, localização, produtos, métodos de pagamento

#### Product (Produto)
- **Propósito**: Produto disponível para compra coletiva
- **Atributos**: Tipo, quantidade, preço, sazonalidade, disponibilidade

#### CollectivePurchase (Compra Coletiva)
- **Propósito**: Rodada de compra coletiva
- **Status**: PLANNING, COLLECTING_INTERESTS, CONFIRMED, IN_DELIVERY, COMPLETED, CANCELLED

#### PurchaseInterest (Interesse de Compra)
- **Propósito**: Interesse de usuário em produto
- **Atributos**: Quantidade desejada, status (opt-in/opt-out)

#### PurchaseSchedule (Agenda de Compras)
- **Propósito**: Calendário de compras comunitárias
- **Características**: Frequência (mensal, quinzenal, semanal)

---

## 🔄 Fluxos Funcionais (Planejados)

### Fluxo 1: Criar Rodada de Compra Coletiva

```
Organizador → Cria Rodada → Define Produtos Disponíveis → 
Define Prazo para Interesse → Define Quantidade Mínima → 
Publica Rodada → Usuários Indicam Interesse
```

### Fluxo 2: Participar de Compra Coletiva

```
Usuário → Visualiza Rodada Ativa → Indica Interesse → 
Define Quantidades → Confirma → 
Quantidade Mínima Atingida → Compra Confirmada → 
Pagamento → Entrega Organizada
```

### Fluxo 3: Integração com Votação

```
Organizador → Cria Votação → "Quais produtos comprar?" → 
Moradores Votam → Produtos Mais Votados → 
Rodada Criada com Produtos Escolhidos
```

### Fluxo 4: Integração com Entregas

```
Compra Confirmada → Organizador Organiza Entrega → 
Rota Otimizada → Pontos de Entrega Comunitários → 
Entregadores (podem ser participantes) → 
Distribuição Realizada
```

---

## 📖 Casos de Uso (Planejados)

### Caso de Uso 1: Organizador Cria Rodada de Compra

**Ator**: Organizador (morador)  
**Fluxo**:
1. Acessa sistema de compra coletiva
2. Cria nova rodada
3. Seleciona produtos do catálogo de produtores
4. Define prazo para indicação de interesse
5. Define quantidade mínima para viabilizar
6. Publica rodada
7. Usuários recebem notificação

### Caso de Uso 2: Usuário Participa de Compra

**Ator**: Morador ou Visitante  
**Fluxo**:
1. Visualiza rodadas ativas
2. Seleciona rodada de interesse
3. Indica interesse em produtos
4. Define quantidades desejadas
5. Confirma interesse
6. Aguarda confirmação da compra (quantidade mínima)
7. Recebe notificação quando confirmada
8. Realiza pagamento
9. Recebe produto na entrega organizada

---

## ⚙️ Regras de Negócio (Planejadas)

1. **Permissões**:
   - Organizadores: Moradores verificados
   - Participantes: Todos usuários autenticados
   - Produtores: Moradores ou visitantes

2. **Quantidade Mínima**:
   - Rodada só é confirmada se atingir quantidade mínima
   - Usuários podem cancelar interesse antes do prazo

3. **Integração com Votação**:
   - Produtos podem ser escolhidos via votação
   - Frequência de compras pode ser decidida coletivamente

4. **Integração com Moeda Territorial**:
   - Pagamento pode ser em moeda territorial
   - Desconto para pagamento em moeda territorial
   - Fundos territoriais podem subsidiar

5. **Gamificação**:
   - Participação gera contribuição territorial
   - Organizar compra gera mais pontos
   - Comprar de produtor local gera mais pontos

---

## 🔗 Integrações Planejadas

### Com Funcionalidades Existentes

- **Marketplace**: Produtores podem ter lojas no marketplace
- **Votação**: Decisões sobre produtos e frequência
- **Entregas**: Sistema de entregas territoriais (Fase 21)
- **Moeda Territorial**: Pagamentos em moeda local (Fase 22)
- **Gamificação**: Pontos por participação (Fase 42)

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](funcional/00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Marketplace](funcional/06_MARKETPLACE.md)** - Sistema de lojas existente
- **[Governança e Votação](funcional/13_GOVERNANCA_VOTACAO.md)** - Decisões coletivas
- **[Fase 17 - Compra Coletiva](backlog-api/FASE17.md)** - Detalhes técnicos do planejamento

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: ⏳ Planejada - Não Implementada
