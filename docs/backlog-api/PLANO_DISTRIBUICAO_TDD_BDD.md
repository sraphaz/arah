# Plano de Distribuição TDD/BDD pelas Fases

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 📋 Plano de Distribuição

---

## 🎯 Objetivo

Distribuir o backlog de TDD/BDD de forma homogênea pelas fases seguintes (10+), garantindo que cada fase tenha uma carga equilibrada de trabalho de testes e documentação BDD.

**Contexto**: A Fase 9 já passou, então o trabalho de TDD/BDD será distribuído a partir da Fase 10.

---

## 📊 Distribuição por Fases

### Estratégia de Distribuição

**Princípios**:
1. **+15-20% de tempo** por fase para TDD/BDD (diluído)
2. **BDD obrigatório** para funcionalidades de negócio críticas
3. **Cobertura >90%** mantida em todas as fases
4. **Cobertura >95%** para funcionalidades críticas (segurança, pagamentos, blockchain)

**Distribuição**:
- **Fases 10-12**: Foco em estabelecer padrão TDD/BDD (mais BDD)
- **Fases 13-15**: Consolidação e expansão (BDD para governança e segurança)
- **Fases 16+**: Manutenção e otimização (TDD padrão, BDD seletivo)

---

## 📋 Detalhamento por Fase

### Fase 10: Mídias em Conteúdo

**Duração Original**: 20 dias (160h)  
**Duração Ajustada**: 24 dias (192h) - **+20% TDD/BDD**

**TDD/BDD Distribuído**:
- **+4 dias** para implementação TDD/BDD
- **TDD obrigatório** para todas as funcionalidades
- **BDD obrigatório** para:
  - Upload de mídia (imagens, vídeos, áudios)
  - Validação de mídia (tamanho, tipo, quantidade)
  - Mídias em posts/eventos/marketplace/chat

**Features BDD**:
- [ ] `Feature: Upload de Mídia` - Fluxo completo de upload
- [ ] `Feature: Validação de Mídia` - Validações de tamanho e tipo
- [ ] `Feature: Mídias em Posts` - Integração de mídias em posts
- [ ] `Feature: Mídias em Eventos` - Integração de mídias em eventos
- [ ] `Feature: Mídias em Marketplace` - Integração de mídias em items
- [ ] `Feature: Mídias em Chat` - Envio de imagens/áudios no chat

**Cobertura**: >90% para todas as funcionalidades

---

### Fase 11: Edição e Gestão

**Duração Original**: 15 dias (120h)  
**Duração Ajustada**: 18 dias (144h) - **+20% TDD/BDD**

**TDD/BDD Distribuído**:
- **+3 dias** para implementação TDD/BDD
- **TDD obrigatório** para todas as funcionalidades
- **BDD obrigatório** para:
  - Edição de posts/eventos
  - Sistema de avaliações
  - Busca no marketplace

**Features BDD**:
- [ ] `Feature: Editar Post` - Fluxo de edição com validações
- [ ] `Feature: Editar Evento` - Fluxo de edição e cancelamento
- [ ] `Feature: Avaliar Item` - Sistema de avaliações do marketplace
- [ ] `Feature: Buscar no Marketplace` - Busca full-text com filtros

**Cobertura**: >90% para todas as funcionalidades

---

### Fase 12: Otimizações Finais

**Duração Original**: 10 dias (80h)  
**Duração Ajustada**: 12 dias (96h) - **+20% TDD/BDD**

**TDD/BDD Distribuído**:
- **+2 dias** para implementação TDD/BDD
- **TDD obrigatório** para otimizações
- **BDD seletivo** para funcionalidades de negócio

**Features BDD**:
- [ ] `Feature: Cache de Feed` - Estratégia de cache
- [ ] `Feature: Paginação Otimizada` - Performance de paginação

**Cobertura**: >90% para todas as funcionalidades

---

### Fase 13: Conector de Emails

**Duração Original**: 14 dias (112h)  
**Duração Ajustada**: 17 dias (136h) - **+20% TDD/BDD**

**TDD/BDD Distribuído**:
- **+3 dias** para implementação TDD/BDD
- **TDD obrigatório** para todas as funcionalidades
- **BDD obrigatório** para:
  - Envio de emails
  - Processamento de emails recebidos
  - Templates de email

**Features BDD**:
- [ ] `Feature: Enviar Email` - Fluxo de envio de emails
- [ ] `Feature: Processar Email Recebido` - Processamento de emails
- [ ] `Feature: Templates de Email` - Sistema de templates

**Cobertura**: >90% para todas as funcionalidades

---

### Fase 14: Governança Comunitária

**Duração Original**: 21 dias (168h)  
**Duração Ajustada**: 25 dias (200h) - **+20% TDD/BDD**

**TDD/BDD Distribuído**:
- **+4 dias** para implementação TDD/BDD
- **TDD obrigatório** para todas as funcionalidades
- **BDD OBRIGATÓRIO** para regras de negócio críticas (governança)
- **Cobertura >95%** para funcionalidades críticas

**Features BDD** (CRÍTICO):
- [ ] `Feature: Criar Proposta de Votação` - **BDD OBRIGATÓRIO**
- [ ] `Feature: Votar em Proposta` - **BDD OBRIGATÓRIO**
- [ ] `Feature: Processar Resultado de Votação` - **BDD OBRIGATÓRIO**
- [ ] `Feature: Regras de Quórum` - **BDD OBRIGATÓRIO**
- [ ] `Feature: Moderação Comunitária` - **BDD OBRIGATÓRIO**

**Cobertura**: >95% para funcionalidades críticas de governança

---

### Fase 15: Segurança Avançada

**Duração Original**: 14 dias (112h)  
**Duração Ajustada**: 17 dias (136h) - **+20% TDD/BDD**

**TDD/BDD Distribuído**:
- **+3 dias** para implementação TDD/BDD
- **TDD obrigatório** para todas as funcionalidades
- **BDD OBRIGATÓRIO** para funcionalidades de segurança
- **Cobertura >95%** para funcionalidades críticas

**Features BDD** (CRÍTICO):
- [ ] `Feature: Autenticação 2FA` - **BDD OBRIGATÓRIO**
- [ ] `Feature: Rate Limiting` - **BDD OBRIGATÓRIO**
- [ ] `Feature: Validação de Segurança` - **BDD OBRIGATÓRIO**

**Cobertura**: >95% para funcionalidades críticas de segurança

---

### Fases 16-29: Distribuição Gradual

**Estratégia**: 
- **+15% de tempo** para TDD/BDD (reduzido, pois padrão já estabelecido)
- **TDD obrigatório** para todas as funcionalidades
- **BDD seletivo** apenas para funcionalidades de negócio críticas

**Fases com BDD Obrigatório**:
- **Fase 20+ (Web3/Blockchain)**: Cobertura >95%, BDD obrigatório para smart contracts
- **Fase 30+ (Features Complexas)**: BDD para regras de negócio complexas

---

## 📊 Resumo da Distribuição

| Fase | Duração Original | Duração Ajustada | TDD/BDD | Features BDD | Cobertura |
|------|------------------|------------------|---------|--------------|-----------|
| 10 | 20 dias | 24 dias (+20%) | +4 dias | 6 features | >90% |
| 11 | 15 dias | 18 dias (+20%) | +3 dias | 4 features | >90% |
| 12 | 10 dias | 12 dias (+20%) | +2 dias | 2 features | >90% |
| 13 | 14 dias | 17 dias (+20%) | +3 dias | 3 features | >90% |
| 14 | 21 dias | 25 dias (+20%) | +4 dias | 5 features | >95% |
| 15 | 14 dias | 17 dias (+20%) | +3 dias | 3 features | >95% |
| 16-29 | Variável | +15% | Variável | Seletivo | >90% |

**Total TDD/BDD Distribuído**: ~19 dias adicionais nas fases 10-15

---

## ✅ Checklist de Implementação

### Para Cada Fase (10+)

- [ ] Seção "🧪 Estratégia TDD/BDD" incluída
- [ ] Duração ajustada (+15-20% conforme fase)
- [ ] Lista de features BDD obrigatórias definida
- [ ] Checklist TDD/BDD por funcionalidade
- [ ] Métricas de sucesso definidas
- [ ] Referências ao plano TDD/BDD incluídas

---

## 📚 Referências

- [Plano Completo TDD/BDD](../23_TDD_BDD_PLANO_IMPLEMENTACAO.md)
- [Fase 0: Fundação TDD/BDD](./FASE0.md)
- [Template TDD/BDD para Fases](./TEMPLATE_TDD_BDD_FASES.md)
- [Guia de Integração TDD/BDD](./GUIA_INTEGRACAO_TDD_BDD_FASES.md)

---

**Última Atualização**: 2025-01-20
