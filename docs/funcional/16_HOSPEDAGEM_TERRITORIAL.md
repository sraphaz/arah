# Hospedagem Territorial - Documentação Funcional (Planejada)

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: ⏳ **PLANEJADA - NÃO IMPLEMENTADA**  
**Fase**: 18  
**Prioridade**: 🔴 Crítica (Economia Local)  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## ⚠️ Status

Esta funcionalidade está **planejada** mas **ainda não implementada**. Detalhes podem mudar durante o desenvolvimento.

---

## 🎯 Visão Geral

O sistema de **Hospedagem Territorial** permite que moradores validados cadastrem propriedades privadas para hospedagem, oferecendo uma alternativa local e territorial ao Airbnb.

### Objetivo

Permitir que:
- **Moradores validados** cadastrem propriedades para hospedagem
- **Visitantes** solicitem estadias com aprovação humana
- **Comunidades** regulamentem hospedagem territorialmente
- **Economia local** seja fortalecida através de hospedagem territorial

### Diferenciais do Araponga

- **Território-first**: Não é global como Airbnb, focado no território
- **Morador validado**: Pré-requisito para cadastrar propriedades
- **Aprovação humana**: Sempre presente (com auto-aprovação condicional)
- **Privacidade por padrão**: Propriedades privadas até terem hospedagem ativa

---

## 💼 Função de Negócio

### Para Proprietários (Owners)

- Cadastrar propriedades privadas
- Configurar múltiplas formas de hospedagem (casa inteira, quarto, cama compartilhada)
- Gerenciar agenda de disponibilidade
- Aprovar/rejeitar solicitações de estadia
- Receber pagamentos com split (Owner, Limpeza, Plataforma)

### Para Anfitriões (Hosts)

- Gerenciar aprovações de estadias (pode ser delegado pelo Owner)
- Configurar políticas de aprovação (manual ou auto-aprovação condicional)
- Receber notificações de solicitações

### Para Limpeza (Cleaning)

- Ser designado para limpeza de propriedades
- Receber parte do pagamento (split)

### Para Visitantes

- Buscar propriedades disponíveis no território
- Solicitar estadias
- Realizar check-in/check-out
- Pagar estadias

### Para a Comunidade

- **Economia Local**: Fortalece circulação de recursos no território
- **Soberania Territorial**: Regulação territorial de hospedagem
- **Autonomia**: Comunidade decide regras de hospedagem

---

## 🏗️ Elementos da Arquitetura (Planejados)

### Entidades Principais

#### Property (Propriedade)
- **Propósito**: Propriedade privada cadastrada
- **Características**: Privada por padrão, visível apenas para Owner até ter hospedagem ativa

#### HostingConfiguration (Configuração de Hospedagem)
- **Propósito**: Configuração de hospedagem por propriedade
- **Tipos**: Casa Inteira, Quarto Privado, Cama Compartilhada
- **Modalidades**: Diária, Semanal, Mensal, Anual, Pacotes
- **Política de Aprovação**: Manual ou Auto-aprovação Condicional

#### HostingCalendar (Agenda - NÚCLEO)
- **Propósito**: Agenda de disponibilidade (núcleo do sistema)
- **Estados**: Available, BlockedByResident, PendingApproval, Reserved
- **Características**: Inicia totalmente bloqueada, Owner deve abrir datas explicitamente

#### HostingRole (Papéis Contextuais)
- **Tipos**: Owner, Host, Cleaning
- **Características**: Contextuais por HostingConfiguration, um morador pode acumular múltiplos papéis

#### StayRequest / Stay (Solicitação/Estadia)
- **Propósito**: Solicitação de estadia
- **Status**: PendingApproval, Approved, Rejected, CheckedIn, CheckedOut, Cancelled

---

## 🔄 Fluxos Funcionais (Planejados)

### Fluxo 1: Cadastrar Propriedade

```
Morador Validado → Cadastra Propriedade → 
Propriedade Privada (invisível publicamente) → 
Cria HostingConfiguration → Define Tipo/Modalidade → 
Configura Agenda (inicialmente bloqueada) → 
Abre Datas Disponíveis → Propriedade Visível Publicamente
```

### Fluxo 2: Solicitar Estadia

```
Visitante → Busca Propriedades Disponíveis → 
Seleciona Propriedade → Escolhe Datas → 
Cria StayRequest → Status: PendingApproval → 
Host Revisa → Aprova/Rejeita → 
Se Aprovado: Pagamento com Escrow → 
Check-in → Check-out → Pagamento Liberado
```

### Fluxo 3: Aprovação Automática Condicional

```
Visitante → Solicita Estadia → 
Sistema Verifica Condições (auto-aprovação) → 
Se Atende: Aprova Automaticamente → 
Se Não: Envia para Aprovação Manual
```

---

## ⚙️ Regras de Negócio (Planejadas)

1. **Permissões**:
   - Cadastrar propriedade: Apenas moradores validados
   - Host e Cleaning: Moradores validados do mesmo território
   - Solicitar estadia: Todos usuários autenticados

2. **Privacidade**:
   - Propriedade é privada por padrão
   - Visível apenas para Owner até ter hospedagem ativa
   - Pode existir indefinidamente sem hospedagem

3. **Agenda**:
   - Inicia totalmente bloqueada
   - Owner deve abrir datas explicitamente
   - Datas reservadas não podem ser sobrescritas
   - Padrões recorrentes (bloqueios, aberturas)

4. **Pagamentos**:
   - Escrow: Pagamento bloqueado até check-out
   - Split: Owner, Limpeza, Plataforma
   - Liberação: Após check-out confirmado

5. **Aprovação**:
   - Sempre requer consentimento (manual ou condicional)
   - Auto-aprovação baseada em condições configuráveis

---

## 🔗 Integrações Planejadas

### Com Funcionalidades Existentes

- **Pagamentos**: Sistema completo de pagamentos (Fase 6-7)
- **WorkItem**: Sistema de aprovação humana
- **Notificações**: Notificações de solicitações e aprovações
- **Feature Flags**: Controle de habilitação por território
- **Membership**: Validação de moradores

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Territórios e Memberships](./02_TERRITORIOS_MEMBERSHIPS.md)** - Validação de moradores
- **[Fase 18 - Hospedagem Territorial](../backlog-api/FASE18.md)** - Detalhes técnicos do planejamento

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: ⏳ Planejada - Não Implementada
