# Plano de Implantação - Fases 13 a 15

**Data de Criação**: 2026-01-25  
**Última Atualização**: 2026-01-25  
**Objetivo**: Plano detalhado de implementação das Fases 13, 14, 15 e 16 (Finalização)  
**Duração Total Estimada**: ~100 dias úteis (~5 meses)  
**Status**: 📋 Planejado

---

## 📊 Visão Geral

### Estado Atual do Projeto

| Componente | Status | Progresso |
|------------|--------|-----------|
| **Fases 1-8** | ✅ Completo | 100% |
| **Fases 9-12** | ✅ Completo | 100% (MVP Essencial) |
| **Fase 13** | ⏳ Pendente | 0% |
| **Fase 14** | ✅ Implementado | 100% |
| **Fase 15** | ⏳ Pendente | 0% |
| **Fase 16** | ⏳ Pendente | 0% |

### Objetivo das Fases 13-16

**Onda 2: Governança e Sustentabilidade** - Implementar base de governança participativa e sustentabilidade financeira.

---

## 🎯 Resumo Executivo

### Fases a Implementar

1. **Fase 13**: Conector de Envio de Emails (14 dias)
2. **Fase 14**: Governança/Votação (21 dias) - ✅ **JÁ IMPLEMENTADO**
3. **Fase 15**: Subscriptions & Recurring Payments (60 dias)
4. **Fase 16**: Finalização Completa Fases 1-15 (20 dias)

**Total**: ~95 dias úteis (~4.5 meses)

### Valor Entregue

- ✅ Comunicação robusta (emails)
- ✅ Governança participativa funcional
- ✅ Sustentabilidade financeira (receitas recorrentes)
- ✅ Base completa e validada para próximas fases

---

## 📅 Cronograma Detalhado

### Fase 13: Conector de Envio de Emails

**Duração**: 14 dias úteis (2 semanas)  
**Prioridade**: 🔴 P0 Crítica  
**Dependências**: Nenhuma (pode ser feito em paralelo)

#### Semana 1: Infraestrutura de Envio

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 13.1 Interface e Abstração | 8h (1 dia) | ⏳ Pendente | 🔴 Crítica |
| 13.2 Implementação SMTP | 12h (1.5 dias) | ⏳ Pendente | 🔴 Crítica |
| 13.3 Implementação SendGrid | 12h (1.5 dias) | ⏳ Pendente | 🟢 Opcional |
| 13.4 Sistema de Templates | 16h (2 dias) | ⏳ Pendente | 🔴 Crítica |

**Entregas da Semana 1**:
- ✅ Interface `IEmailSender` criada
- ✅ Implementação SMTP funcionando
- ✅ Sistema de templates funcionando
- ✅ Templates base criados (welcome, password-reset, event-reminder, marketplace-order, alert-critical)

#### Semana 2: Queue, Integração e Preferências

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 14.1 Queue de Envio Assíncrono | 16h (2 dias) | ⏳ Pendente | 🔴 Crítica |
| 14.2 Integração com Notificações | 12h (1.5 dias) | ⏳ Pendente | 🔴 Crítica |
| 14.3 Preferências de Email | 8h (1 dia) | ⏳ Pendente | 🟡 Importante |
| 14.4 Casos de Uso Específicos | 12h (1.5 dias) | ⏳ Pendente | 🔴 Crítica |
| 14.5 Testes e Documentação | 8h (1 dia) | ⏳ Pendente | 🟡 Importante |

**Entregas da Semana 2**:
- ✅ Queue de envio assíncrono funcionando
- ✅ Integração com sistema de notificações
- ✅ Preferências de email do usuário
- ✅ Casos de uso específicos implementados
- ✅ Testes e documentação completos

**Total Fase 13**: 80 horas (14 dias úteis)

---

### Fase 14: Governança/Votação

**Duração**: 21 dias úteis (3 semanas)  
**Prioridade**: 🔴 P0 Crítica  
**Status**: ✅ **JÁ IMPLEMENTADO**

**Nota**: Esta fase já foi implementada. Apenas validação e testes finais podem ser necessários na Fase 16.

---

### Fase 15: Subscriptions & Recurring Payments

**Duração**: 60 dias úteis (12 semanas)  
**Prioridade**: 🔴 P0 Crítica  
**Dependências**: Fase 6 (Pagamentos), Fase 7 (Payout)

#### Semanas 1-2: Modelo de Domínio e Planos

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 15.1 Modelo de Domínio - Assinaturas | 32h (4 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.2 Integração com Stripe Subscriptions | 40h (5 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.3 Webhooks do Stripe | 32h (4 dias) | ⏳ Pendente | 🔴 Crítica |

**Entregas das Semanas 1-2**:
- ✅ Modelos de domínio completos (SubscriptionPlan, Subscription, SubscriptionPayment, Coupon)
- ✅ Integração com Stripe Subscriptions funcionando
- ✅ Webhooks do Stripe sendo processados

#### Semanas 3-4: Serviços de Assinatura

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 15.4 Serviço de Assinaturas | 40h (5 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.5 Serviço de Cupons | 24h (3 dias) | ⏳ Pendente | 🟡 Importante |

**Entregas das Semanas 3-4**:
- ✅ Serviço de assinaturas completo
- ✅ Resolução de planos por território
- ✅ Atribuição automática de plano FREE
- ✅ Serviço de cupons funcionando

#### Semanas 5-6: Processamento e Gestão

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 15.6 Processamento de Renovações | 32h (4 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.7 Gestão de Trials | 24h (3 dias) | ⏳ Pendente | 🟡 Importante |
| 15.8 Sistema Administrativo de Planos | 40h (5 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.9 Sistema Administrativo de Cupons | 24h (3 dias) | ⏳ Pendente | 🟡 Importante |

**Entregas das Semanas 5-6**:
- ✅ Processamento de renovações automáticas
- ✅ Gestão de trials funcionando
- ✅ Sistema administrativo de planos completo
- ✅ Validações de integridade (funcionalidades básicas no FREE)
- ✅ Sistema administrativo de cupons

#### Semanas 7-8: Controllers e API

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 15.10 Sistema de Verificação de Funcionalidades | 24h (3 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.11 Controllers Administrativos | 32h (4 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.12 Controllers Públicos | 32h (4 dias) | ⏳ Pendente | 🔴 Crítica |
| 15.13 Dashboard de Assinantes | 32h (4 dias) | ⏳ Pendente | 🟡 Importante |

**Entregas das Semanas 7-8**:
- ✅ Sistema de verificação de funcionalidades
- ✅ Controllers administrativos (SystemAdmin e Curadores)
- ✅ Controllers públicos
- ✅ Dashboard de métricas (MRR, churn, etc.)

#### Semanas 9-10: Frontend e Notificações

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 15.14 Interface de Assinaturas (Frontend) | 40h (5 dias) | ⏳ Pendente | 🟡 Importante |
| 15.15 Interface Administrativa (Frontend) | 40h (5 dias) | ⏳ Pendente | 🔴 Crítica |

**Entregas das Semanas 9-10**:
- ✅ Interface pública de assinaturas
- ✅ Interface administrativa de planos (Global + Territorial)
- ✅ Seleção de funcionalidades por plano
- ✅ Validações em tempo real

#### Semanas 11-12: Testes e Documentação

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 15.16 Testes e Documentação | 40h (5 dias) | ⏳ Pendente | 🟡 Importante |

**Entregas das Semanas 11-12**:
- ✅ Testes de integração completos
- ✅ Testes de performance
- ✅ Testes de segurança
- ✅ Documentação técnica completa

**Total Fase 15**: 480 horas (60 dias úteis)

---

### Fase 16: Finalização Completa Fases 1-15

**Duração**: 20 dias úteis (4 semanas)  
**Prioridade**: 🔴 P0 Crítica  
**Dependências**: Fases 1-15

#### Semana 1: Sistema de Políticas de Termos

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 14.8.1 Modelo de Domínio - Termos | 8h (1 dia) | ⏳ Pendente | 🔴 Crítica |
| 14.8.2 Repositórios - Termos | 8h (1 dia) | ⏳ Pendente | 🔴 Crítica |
| 14.8.3 Service - Termos | 16h (2 dias) | ⏳ Pendente | 🔴 Crítica |
| 14.8.4 Controller - Termos | 12h (1.5 dias) | ⏳ Pendente | 🔴 Crítica |
| 14.8.5 Integração AuthService | 8h (1 dia) | ⏳ Pendente | 🔴 Crítica |
| 14.8.6 Notificações - Termos | 8h (1 dia) | ⏳ Pendente | 🟡 Importante |

**Entregas da Semana 1**:
- ✅ Sistema de Políticas de Termos completo
- ✅ Integração com autenticação
- ✅ Notificações de atualização de termos

#### Semana 2: Validação de Endpoints

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 14.8.7 Validação Fase 9 | 8h (1 dia) | ⏳ Pendente | 🟡 Importante |
| 14.8.8 Validação Fase 11 | 12h (1.5 dias) | ⏳ Pendente | 🟡 Importante |
| 14.8.9 Validação Fase 12 | 12h (1.5 dias) | ⏳ Pendente | 🟡 Importante |
| 14.8.10 Validação Fase 13 | 8h (1 dia) | ⏳ Pendente | 🟡 Importante |

**Entregas da Semana 2**:
- ✅ Todas as fases validadas
- ✅ Gaps identificados e documentados
- ✅ Correções aplicadas

#### Semana 3: Testes e Otimizações

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 14.8.11 Testes de Performance | 16h (2 dias) | ⏳ Pendente | 🟡 Importante |
| 14.8.12 Otimizações Finais | 16h (2 dias) | ⏳ Pendente | 🟡 Importante |

**Entregas da Semana 3**:
- ✅ Testes de performance implementados
- ✅ SLAs definidos e validados
- ✅ Otimizações aplicadas

#### Semana 4: Documentação e Finalização

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| 14.8.13 Documentação Operacional | 12h (1.5 dias) | ⏳ Pendente | 🟡 Importante |
| 14.8.14 Atualização Documentação | 8h (1 dia) | ⏳ Pendente | 🟡 Importante |
| 14.8.15 Testes Finais | 16h (2 dias) | ⏳ Pendente | 🔴 Crítica |

**Entregas da Semana 4**:
- ✅ Documentação operacional completa
- ✅ Documentação de fases atualizada
- ✅ Testes finais passando
- ✅ Validação completa

**Total Fase 16**: 160 horas (20 dias úteis)

---

## 📊 Cronograma Consolidado

### Timeline Geral

```
Mês 1 (Semanas 1-4):
├─ Fase 13: Conector de Emails (Semanas 1-2)
└─ Fase 15: Início - Modelo de Domínio (Semanas 3-4)

Mês 2 (Semanas 5-8):
└─ Fase 15: Serviços e Processamento (Semanas 5-8)

Mês 3 (Semanas 9-12):
└─ Fase 15: Controllers, Frontend e Testes (Semanas 9-12)

Mês 4 (Semanas 13-16):
└─ Fase 16: Finalização Completa (Semanas 13-16)
```

### Paralelização Possível

- **Fase 13** pode ser feita em paralelo com início da **Fase 15** (modelo de domínio)
- **Fase 14** já está implementada, apenas validação na Fase 16
- **Fase 15** (frontend) pode ser iniciada após controllers estarem prontos
- **Fase 16** (validações) pode ser feita em paralelo com testes da Fase 15

### Duração Total

- **Sequencial**: ~95 dias úteis (~4.5 meses)
- **Com Paralelização**: ~75 dias úteis (~3.5 meses)

---

## 🔗 Dependências e Bloqueios

### Dependências Críticas

| Fase | Depende de | Status | Impacto |
|------|------------|--------|---------|
| Fase 13 | Nenhuma | ✅ Pode iniciar | Nenhum |
| Fase 14 | Nenhuma | ✅ Implementado | Nenhum |
| Fase 15 | Fase 6, Fase 7 | ✅ Completo | Nenhum |
| Fase 16 | Fases 1-15 | ⚠️ Parcial | Bloqueia se não completo |

### Bloqueios Identificados

1. **Fase 15** requer Fases 6 e 7 (Pagamentos e Payout) - ✅ **Já completas**
2. **Fase 16** requer todas as fases anteriores - ⚠️ **Aguardar conclusão**

---

## 📋 Recursos Necessários

### Equipe

- **1 Desenvolvedor Backend** (full-time)
- **1 Desenvolvedor Frontend** (part-time, semanas 9-10 da Fase 15)
- **1 QA/Tester** (part-time, validações e testes)

### Infraestrutura

- **Stripe Account** (para Fase 15)
- **SMTP Server** (para Fase 13)
- **SendGrid Account** (opcional, para Fase 13)
- **Ambiente de Testes** (PostgreSQL, Redis, MinIO)

### Ferramentas

- **.NET 8 SDK**
- **Stripe.net** (NuGet package)
- **MailKit** ou **System.Net.Mail** (para SMTP)
- **FluentValidation** (já em uso)
- **Serilog** (já em uso)

---

## ⚠️ Riscos e Mitigações

### Riscos Técnicos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Integração Stripe complexa | Média | Alto | Testes incrementais, documentação Stripe |
| Performance de webhooks | Baixa | Médio | Queue assíncrona, retry policy |
| Validações de integridade complexas | Média | Médio | Testes unitários extensivos |
| Templates de email complexos | Baixa | Baixo | Templates simples, iterativo |

### Riscos de Negócio

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Mudança de requisitos | Média | Médio | Revisões semanais, documentação clara |
| Atraso em dependências | Baixa | Alto | Buffer de tempo, paralelização |
| Complexidade de planos | Média | Médio | Validações automáticas, testes |

### Riscos de Qualidade

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Cobertura de testes insuficiente | Baixa | Alto | Meta de 85%+, revisões de código |
| Bugs em produção | Baixa | Alto | Testes de integração, staging |
| Documentação incompleta | Média | Médio | Checklist de documentação |

---

## ✅ Critérios de Sucesso

### Fase 13: Conector de Emails

- ✅ Envio de emails funcionando (SMTP)
- ✅ Templates de email funcionando
- ✅ Queue de envio funcionando
- ✅ Preferências de email funcionando
- ✅ Integração com notificações funcionando
- ✅ Casos de uso específicos funcionando
- ✅ Cobertura de testes >80%
- ✅ Documentação completa

### Fase 14: Governança/Votação

- ✅ **Já implementado** - Apenas validação na Fase 16

### Fase 15: Subscriptions & Recurring Payments

- ✅ Plano FREE funcionando (padrão para todos)
- ✅ Funcionalidades básicas sempre acessíveis
- ✅ Sistema completo de assinaturas funcionando
- ✅ Sistema de verificação de funcionalidades funcionando
- ✅ Sistema administrativo completo
- ✅ Seleção de funcionalidades por plano
- ✅ Validações de integridade garantindo funcionalidades básicas no FREE
- ✅ Pagamentos recorrentes automáticos funcionando
- ✅ Integração com Stripe funcionando
- ✅ Webhooks sendo processados
- ✅ Upgrade/downgrade funcionando
- ✅ Cancelamento funcionando (volta para FREE)
- ✅ Trials funcionando
- ✅ Cupons funcionando
- ✅ Dashboard de métricas funcionando
- ✅ Cobertura de testes >85%
- ✅ Documentação completa

### Fase 16: Finalização Completa

- ✅ Sistema de Políticas de Termos implementado
- ✅ Todos os endpoints validados
- ✅ Integrações validadas
- ✅ Testes de performance implementados
- ✅ Otimizações aplicadas
- ✅ Documentação operacional completa
- ✅ Cobertura de testes >85%
- ✅ Conformidade legal (LGPD) validada

---

## 📈 Métricas de Acompanhamento

### Métricas de Progresso

- **Velocidade**: Tarefas completadas por semana
- **Cobertura de Testes**: % de código coberto
- **Bugs Encontrados**: Quantidade e severidade
- **Documentação**: % de documentação completa

### Métricas de Qualidade

- **Taxa de Sucesso de Testes**: Meta 100%
- **Cobertura de Testes**: Meta >85%
- **Code Review**: Todas as PRs revisadas
- **Performance**: SLAs atendidos

### Métricas de Negócio (Fase 15)

- **MRR** (Monthly Recurring Revenue): Acompanhar após lançamento
- **Churn Rate**: Meta <5% mensal
- **Taxa de Conversão**: FREE → Pago
- **Assinaturas Ativas**: Crescimento mensal

---

## 📚 Documentação de Referência

### Documentos Principais

- [FASE13.md](./backlog-api/FASE13.md) - Conector de Envio de Emails
- [FASE14.md](./backlog-api/FASE14.md) - Governança/Votação
- [FASE15.md](./backlog-api/FASE15.md) - Subscriptions & Recurring Payments
- [FASE14_8.md](./backlog-api/FASE14_8.md) - Finalização Completa

### Documentos de Apoio

- [README.md](./backlog-api/README.md) - Backlog completo
- [STATUS_FASES.md](./STATUS_FASES.md) - Status das fases
- [RESUMO_IMPLEMENTACAO_FASES_9_12.md](../RESUMO_IMPLEMENTACAO_FASES_9_12.md) - Resumo MVP

---

## 🎯 Próximos Passos Imediatos

### Semana 1 (Início)

1. **Iniciar Fase 13**:
   - [ ] Criar interface `IEmailSender`
   - [ ] Implementar `SmtpEmailSender`
   - [ ] Criar sistema de templates

2. **Preparar Fase 15**:
   - [ ] Revisar documentação da Fase 15
   - [ ] Configurar ambiente Stripe (test mode)
   - [ ] Preparar modelo de domínio

### Acompanhamento Semanal

- **Reunião de Progresso**: Toda segunda-feira
- **Review de Código**: PRs revisadas em até 24h
- **Atualização de Status**: Toda sexta-feira
- **Documentação**: Atualizada continuamente

---

## 📝 Notas Finais

### Princípios Fundamentais

1. **Acesso Básico Gratuito**: Funcionalidades essenciais sempre disponíveis
2. **Inclusão**: Ninguém é excluído por não poder pagar
3. **Transparência**: Usuário sempre sabe o status da assinatura
4. **Qualidade**: Cobertura de testes >85%, documentação completa
5. **Conformidade**: LGPD, Políticas de Termos implementadas

### Decisões Arquiteturais

- **Planos Globais vs Territoriais**: Hierarquia clara, territoriais sobrescrevem globais
- **Plano FREE**: Sempre disponível, funcionalidades básicas protegidas
- **Validações de Integridade**: Automáticas, impedem remoção de funcionalidades básicas
- **Queue de Emails**: Assíncrona, com retry policy e dead letter queue

---

**Status**: 📋 **PLANO CRIADO**  
**Próxima Revisão**: Após conclusão de cada fase  
**Responsável**: Equipe de Desenvolvimento  
**Aprovação**: Pendente

---

**Última Atualização**: 2026-01-25
