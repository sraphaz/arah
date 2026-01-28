# Moeda Territorial - Documentação Funcional (Planejada)

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: ⏳ **PLANEJADA - NÃO IMPLEMENTADA**  
**Fase**: 22  
**Prioridade**: 🟡 Alta (Economia Local)  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## ⚠️ Status

Esta funcionalidade está **planejada** mas **ainda não implementada**. Detalhes podem mudar durante o desenvolvimento.

---

## 🎯 Visão Geral

O sistema de **Moeda Territorial** permite que cada território tenha sua própria moeda virtual, facilitando economia circular local e preparando para integração com tokens on-chain.

### Objetivo

Permitir que:
- **Territórios** tenham moeda virtual própria
- **Economia circular** seja facilitada
- **Transações locais** usem moeda territorial
- **Preparação** para tokens on-chain (Web3)

---

## 💼 Função de Negócio

### Para Usuários

- Ganhar moeda territorial (através de participação, vendas, etc.)
- Gastar moeda territorial (compras, serviços, etc.)
- Converter moeda territorial ↔ fiat (se habilitado)
- Visualizar saldo e histórico

### Para a Comunidade

- **Economia Circular**: Moeda circula dentro do território
- **Autonomia**: Comunidade controla sua moeda
- **Preparação Web3**: Base para tokens on-chain

---

## 🏗️ Elementos da Arquitetura (Planejados)

### Entidades Principais

#### TerritorialCurrency (Moeda Territorial)
- **Propósito**: Moeda virtual do território
- **Atributos**: Nome, símbolo, taxa de conversão (se aplicável)

#### CurrencyBalance (Saldo)
- **Propósito**: Saldo de moeda territorial por usuário
- **Atributos**: Quantidade, histórico de transações

#### CurrencyTransaction (Transação)
- **Propósito**: Transação em moeda territorial
- **Tipos**: EARNED, SPENT, TRANSFERRED, CONVERTED

---

## 🔄 Fluxos Funcionais (Planejados)

### Fluxo: Usar Moeda Territorial

```
Usuário → Ganha Moeda (participação, venda) → 
Saldo Atualizado → Usa Moeda (compra, serviço) → 
Transação Processada → Saldo Atualizado
```

---

## ⚙️ Regras de Negócio (Planejadas)

1. **Emissão**: Moeda territorial é emitida pelo território
2. **Conversão**: Pode ser convertida para fiat (se habilitado)
3. **Preparação Web3**: Base para tokens on-chain (Fases 36-40)

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Web3 e Blockchain](./20_WEB3_BLOCKCHAIN.md)** - Integração futura
- **[DAO e Tokenização](./21_DAO_TOKENIZACAO.md)** - Tokens on-chain

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: ⏳ Planejada - Não Implementada
