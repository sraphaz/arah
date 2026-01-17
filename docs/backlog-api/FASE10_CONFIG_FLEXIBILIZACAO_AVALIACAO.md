# Avaliação de Flexibilização de Configurações

**Data**: 2026-01-17  
**Status**: 📋 Análise Completa  
**Objetivo**: Identificar funcionalidades com configurações fixas que merecem flexibilização via painel administrativo, similar à configuração de blob storage implementada na Fase 10.

---

## 🎯 Contexto

Recentemente implementamos a configuração explícita e aberta de blob storage para mídias via painel administrativo (`MediaStorageConfig`), permitindo que administradores configurem provedores de storage (Local, S3, AzureBlob) sem editar `appsettings.json`.

Este documento avalia outras funcionalidades do sistema que possuem configurações fixas e que se beneficiariam de flexibilização similar, integrando os itens de backlog nas fases existentes.

---

## 📊 Metodologia de Avaliação

### Critérios de Priorização

1. **Impacto no Negócio**: Configurações que afetam regras de negócio, limites territoriais ou comportamento da plataforma
2. **Frequência de Mudança**: Configurações que precisam ser ajustadas regularmente
3. **Multi-tenant**: Configurações que deveriam variar por território
4. **Segurança e Compliance**: Configurações relacionadas a segurança, retenção de dados, ou conformidade
5. **Experiência do Usuário**: Configurações que afetam diretamente a experiência do usuário

### Níveis de Prioridade

- **🔴 Alta**: Configurações críticas que bloqueiam customização territorial ou ajustes operacionais
- **🟡 Média**: Configurações importantes que melhorariam a flexibilidade operacional
- **🟢 Baixa**: Configurações que são nice-to-have mas não bloqueantes

---

## 📋 Configurações Identificadas

### 1. ⚙️ Rate Limiting e Proteção contra Abuso

**Situação Atual**:
- Configuração fixa em `appsettings.json`:
  ```json
  "RateLimiting": {
    "PermitLimit": 60,
    "WindowSeconds": 60,
    "QueueLimit": 0
  }
  ```

**Problema**:
- Valores globais não permitem ajustes por território ou por tipo de operação
- Dificulta ajustes em caso de ataques DDoS ou abuso localizado
- Não permite políticas diferentes para diferentes tipos de usuários (Resident vs Visitor)

**Solução Proposta**:
- Criar `RateLimitConfig` no domínio
- Permitir configuração por território (opcional) e por tipo de endpoint (posts, uploads, API geral)
- Interface administrativa para ajustar limites em tempo real

**Prioridade**: 🔴 Alta  
**Complexidade**: Média  
**Impacto**: Alto - Segurança e performance

**Item de Backlog**: Fase 15 (Segurança Avançada)

---

### 2. 📸 Limites de Mídia (Tamanho, Quantidade, Tipos MIME)

**Situação Atual**:
- Alguns limites em `MediaStorageOptions` (hardcoded)
- Limites específicos por tipo de conteúdo (Posts, Events, Marketplace, Chat) fixos no código
- Tipos MIME permitidos fixos em `MediaStorageOptions`

**Problema**:
- Limites fixos não permitem customização territorial
- Não permite ajustes baseados em infraestrutura disponível
- Tipos MIME fixos impedem suporte a novos formatos sem deploy

**Solução Proposta**:
- Extender `TerritoryMediaConfig` (já existe) para incluir limites de tamanho e tipos MIME
- Permitir override de limites globais por território
- Interface administrativa para gestão de configurações de mídia

**Status**: ⚠️ Parcialmente implementado (configuração de tipos de mídia existe, mas limites de tamanho/MIME ainda fixos)

**Prioridade**: 🟡 Média  
**Complexidade**: Baixa (reutilizar estrutura existente)  
**Impacto**: Médio - Flexibilidade territorial

**Item de Backlog**: Fase 10 (complementar implementação existente)

---

### 3. 🛡️ Thresholds de Moderação

**Situação Atual**:
- Thresholds fixos no código (`ReportService`):
  - Janela: **7 dias**
  - Threshold: **3 reports únicos**
- Ações automáticas fixas (ocultar post, etc.)

**Problema**:
- Thresholds globais não permitem políticas diferentes por território
- Não permite ajustes baseados em padrões de comunidade
- Dificulta experimentação com diferentes políticas de moderação

**Solução Proposta**:
- Criar `ModerationThresholdConfig` no domínio
- Configuração por território (opcional, com fallback global)
- Permitir configuração de janela de tempo, threshold de reports, e ações automáticas
- Interface administrativa para gestão de políticas de moderação

**Prioridade**: 🔴 Alta  
**Complexidade**: Média  
**Impacto**: Alto - Governança e moderação

**Item de Backlog**: Fase 11 (Moderação Avançada) ou Fase 15 (Segurança Avançada)

---

### 4. 💰 Configuração de Taxas da Plataforma (Marketplace)

**Situação Atual**:
- `PlatformFeeConfig` existe e já permite configuração por território
- **Porém**: Limites de valores mínimo/máximo e outras políticas fixas

**Problema**:
- Limites de taxa mínimo/máximo não configuráveis
- Políticas de retenção e payout fixas
- Não permite diferentes políticas por tipo de item ou categoria

**Solução Proposta**:
- Estender `PlatformFeeConfig` para incluir limites e políticas
- Adicionar configuração de retenção e payout por território (já existe `PayoutConfig`, mas poderia ser integrado)
- Interface administrativa para gestão financeira completa

**Status**: ⚠️ Parcialmente implementado (taxas configuráveis, mas limites e políticas ainda fixos)

**Prioridade**: 🟡 Média  
**Complexidade**: Baixa (estender modelo existente)  
**Impacto**: Médio - Flexibilidade financeira

**Item de Backlog**: Fase 12 (Marketplace e Gestão Financeira)

---

### 5. 📅 Políticas de Retenção de Dados

**Situação Atual**:
- Políticas de retenção fixas ou não explicitamente configuradas
- Exemplos: logs, mídias não utilizadas, reports arquivados, work items resolvidos

**Problema**:
- Não permite compliance com diferentes regulamentações (GDPR, LGPD)
- Retenção fixa pode gerar custos desnecessários ou riscos de compliance
- Dificulta políticas de backup e arquivamento

**Solução Proposta**:
- Criar `DataRetentionConfig` no domínio
- Configuração por tipo de entidade (Posts, Reports, Media, Logs, etc.)
- Permitir políticas de retenção diferentes por território (respeitando legislação local)
- Interface administrativa para gestão de retenção

**Prioridade**: 🔴 Alta  
**Complexidade**: Alta  
**Impacto**: Alto - Compliance e custos

**Item de Backlog**: Fase 16 (Compliance e Retenção de Dados) - Nova fase

---

### 6. 🔐 Configuração de Autenticação (JWT)

**Situação Atual**:
- Configuração em `appsettings.json`:
  ```json
  "Jwt": {
    "Issuer": "Araponga",
    "Audience": "Araponga",
    "ExpirationMinutes": 60
  }
  ```

**Problema**:
- Expiração fixa não permite diferentes políticas por tipo de aplicação (web, mobile)
- Issuer/Audience fixos dificultam multi-tenant avançado
- Não permite ajustes de segurança sem deploy

**Solução Proposta**:
- Criar `JwtConfig` no domínio (configuração global, não por território)
- Permitir configuração de expiração por tipo de token (access, refresh)
- Interface administrativa para ajustes de segurança

**Prioridade**: 🟡 Média  
**Complexidade**: Baixa  
**Impacto**: Médio - Segurança e flexibilidade

**Item de Backlog**: Fase 15 (Segurança Avançada)

---

### 7. 📍 Políticas de Presença (Presence Policy)

**Situação Atual**:
- Configuração fixa em `appsettings.json`:
  ```json
  "PresencePolicy": {
    "Policy": "ResidentOnly"
  }
  ```

**Problema**:
- Política global não permite diferentes comportamentos por território
- Dificulta experimentação com políticas mais abertas ou restritivas

**Solução Proposta**:
- Estender `TerritorySettings` ou criar `PresencePolicyConfig` por território
- Permitir políticas diferentes (ResidentOnly, VerifiedOnly, Public, etc.)
- Interface administrativa para gestão de políticas de presença

**Prioridade**: 🟢 Baixa  
**Complexidade**: Baixa  
**Impacto**: Baixo - Flexibilidade de governança

**Item de Backlog**: Fase 13 (Governança Territorial Avançada) ou Fase 15 (Segurança Avançada)

---

### 8. 🔔 Configuração de Notificações

**Situação Atual**:
- `UserPreferences` permite configuração de notificações por usuário
- **Porém**: Tipos de notificações e canais disponíveis fixos no código
- Templates de notificação fixos

**Problema**:
- Não permite adicionar novos tipos de notificação sem deploy
- Templates fixos dificultam customização territorial
- Canais de notificação fixos (email, push, etc.)

**Solução Proposta**:
- Criar `NotificationConfig` no domínio (global e por território)
- Permitir configuração de tipos de notificação, canais e templates
- Interface administrativa para gestão de notificações

**Prioridade**: 🟡 Média  
**Complexidade**: Média  
**Impacto**: Médio - Comunicação e engajamento

**Item de Backlog**: Fase 14 (Comunicação e Notificações) ou Fase 18 (Experiência do Usuário)

---

### 9. 🗺️ Configuração de Mapas e Geo-localização

**Situação Atual**:
- Raio de busca fixo no código
- Limites de distância fixos para "territórios próximos"
- Configuração de mapas (providers, zoom, bounds) fixa

**Problema**:
- Raio de busca fixo não permite ajustes por densidade territorial
- Dificulta integração com diferentes provedores de mapas
- Não permite configuração de áreas de interesse por território

**Solução Proposta**:
- Criar `MapConfig` no domínio (por território)
- Permitir configuração de raio de busca, limites de distância, providers de mapas
- Interface administrativa para gestão de configurações de mapa

**Prioridade**: 🟢 Baixa  
**Complexidade**: Baixa  
**Impacto**: Baixo - Melhoria de UX

**Item de Backlog**: Fase 4 (Mapas e Territórios) - complementar implementação existente

---

### 10. 📊 Configuração de Observabilidade (Logging, Metrics, Tracing)

**Situação Atual**:
- Configuração em `appsettings.json`:
  - Logging: Seq, File, Console
  - Metrics: Prometheus
  - Tracing: OpenTelemetry, Jaeger

**Problema**:
- Configuração fixa dificulta ajustes sem deploy
- Não permite diferentes níveis de logging por território
- Dificulta integração com diferentes provedores de observabilidade

**Solução Proposta**:
- Criar `ObservabilityConfig` no domínio (global)
- Permitir configuração de providers, níveis de log, métricas
- Interface administrativa para gestão de observabilidade

**Prioridade**: 🟢 Baixa  
**Complexidade**: Média  
**Impacto**: Baixo - Operações e DevOps

**Item de Backlog**: Fase 25 (DevOps e Observabilidade) ou pós-MVP

---

## 📈 Priorização Consolidada

### Prioridade Alta (🔴)

1. **Rate Limiting e Proteção contra Abuso** → Fase 15
2. **Thresholds de Moderação** → Fase 11 ou Fase 15
3. **Políticas de Retenção de Dados** → Fase 16 (nova fase)

### Prioridade Média (🟡)

4. **Limites de Mídia (complementar)** → Fase 10
5. **Configuração de Taxas (complementar)** → Fase 12
6. **Configuração de Autenticação (JWT)** → Fase 15
7. **Configuração de Notificações** → Fase 14 ou Fase 18

### Prioridade Baixa (🟢)

8. **Políticas de Presença** → Fase 13 ou Fase 15
9. **Configuração de Mapas** → Fase 4 (complementar)
10. **Configuração de Observabilidade** → Fase 25 ou pós-MVP

---

## 🗂️ Integração com Fases Existentes

### Fase 10: Mídias em Conteúdo

**Item Adicional**:
- 10.9 Configuração Avançada de Limites de Mídia
  - Estender `TerritoryMediaConfig` para incluir limites de tamanho e tipos MIME
  - Permitir override de limites globais por território
  - Interface administrativa para gestão

### Fase 11: Moderação Avançada

**Item Adicional**:
- 11.X Configuração de Thresholds de Moderação
  - Criar `ModerationThresholdConfig`
  - Configuração por território (com fallback global)
  - Interface administrativa para políticas de moderação

### Fase 12: Marketplace e Gestão Financeira

**Item Adicional**:
- 12.X Configuração Avançada de Taxas e Limites
  - Estender `PlatformFeeConfig` para incluir limites
  - Integrar com `PayoutConfig` existente
  - Interface administrativa para gestão financeira completa

### Fase 13: Governança Territorial Avançada

**Item Adicional**:
- 13.X Configuração de Políticas de Presença
  - Criar `PresencePolicyConfig` por território
  - Permitir diferentes políticas (ResidentOnly, VerifiedOnly, Public)
  - Interface administrativa para gestão

### Fase 14: Comunicação e Notificações

**Item Adicional**:
- 14.X Configuração Avançada de Notificações
  - Criar `NotificationConfig` (global e por território)
  - Permitir configuração de tipos, canais e templates
  - Interface administrativa para gestão

### Fase 15: Segurança Avançada

**Itens Adicionais**:
- 15.X Configuração de Rate Limiting
  - Criar `RateLimitConfig`
  - Configuração por território e por tipo de endpoint
  - Interface administrativa para ajustes em tempo real

- 15.Y Configuração de Autenticação (JWT)
  - Criar `JwtConfig`
  - Permitir configuração de expiração por tipo de token
  - Interface administrativa para ajustes de segurança

### Fase 16: Compliance e Retenção de Dados (NOVA FASE)

**Objetivo**: Implementar sistema completo de retenção de dados configurável.

**Itens**:
- 16.1 Modelo de Domínio `DataRetentionConfig`
- 16.2 Repositório e Serviço para gestão de retenção
- 16.3 Jobs assíncronos para limpeza automática
- 16.4 Interface administrativa para gestão de retenção
- 16.5 Integração com diferentes tipos de entidades
- 16.6 Documentação e DevPortal

**Dependências**: Fase 10 (mídias), Fase 11 (moderação)

---

## 📝 Recomendações de Implementação

### Padrão Arquitetural

Seguir o mesmo padrão implementado para `MediaStorageConfig`:

1. **Modelo de Domínio**: Criar entidade no `Araponga.Domain`
2. **Repositório**: Interface `IRepository` e implementação InMemory/Postgres
3. **Serviço**: `Service` para lógica de negócio
4. **API Controller**: Endpoint administrativo com permissão `SystemAdmin` ou `Curator` (conforme caso)
5. **Testes**: Testes de integração completos (como `MediaStorageConfigIntegrationTests`)
6. **Documentação**: Atualizar `FASE10.md`, DevPortal e documentação técnica

### Considerações Importantes

1. **Valores Padrão**: Sempre fornecer valores padrão razoáveis para evitar quebra de func
2. **Validação**: Validar limites mínimos/máximos para evitar configurações inválidas
3. **Auditoria**: Registrar mudanças de configuração via `IAuditLogger`
4. **Cache**: Invalidar cache quando configurações mudarem
5. **Segurança**: Secrets mascarados nas respostas da API
6. **Fallback**: Sempre ter fallback para `appsettings.json` durante migração

---

## 🎯 Resumo Executivo

### Total de Configurações Identificadas: 10

- **Alta Prioridade**: 3 configurações
- **Média Prioridade**: 4 configurações
- **Baixa Prioridade**: 3 configurações

### Fases Impactadas

- **Fase 10**: 1 item adicional (complementar)
- **Fase 11**: 1 item adicional (thresholds de moderação)
- **Fase 12**: 1 item adicional (limites de taxas)
- **Fase 13**: 1 item adicional (políticas de presença)
- **Fase 14**: 1 item adicional (notificações)
- **Fase 15**: 2 itens adicionais (rate limiting, JWT)
- **Fase 16**: Nova fase completa (retenção de dados)
- **Fase 4**: 1 item complementar (mapas)
- **Fase 25**: 1 item adicional (observabilidade)

### Próximos Passos

1. ✅ Documento de avaliação criado
2. ⏳ Revisar e aprovar priorizações
3. ⏳ Integrar itens nas fases correspondentes
4. ⏳ Planejar implementação incremental (começar pelas prioridades altas)
5. ⏳ Atualizar roadmap visual

---

**Documento criado em**: 2026-01-17  
**Última atualização**: 2026-01-17  
**Autor**: Sistema de Documentação Araponga
