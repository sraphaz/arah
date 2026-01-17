# Fase 16: Compliance e Retenção de Dados

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 ALTA (Compliance e custos)  
**Depende de**: Fase 10 (Mídias), Fase 11 (Moderação)  
**Estimativa Total**: 168 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar sistema completo de **retenção de dados configurável** que permite:
- Políticas de retenção diferentes por tipo de entidade (Posts, Reports, Media, Logs, etc.)
- Configuração por território (respeitando legislação local: GDPR, LGPD, etc.)
- Limpeza automática de dados expirados
- Backup e arquivamento antes da exclusão (opcional)
- Auditoria completa de retenção e exclusão
- Conformidade com regulamentações de privacidade

**Princípios**:
- ✅ **Compliance**: Respeitar regulamentações de privacidade (GDPR, LGPD)
- ✅ **Flexibilidade**: Políticas diferentes por território e tipo de entidade
- ✅ **Transparência**: Usuários e administradores têm visibilidade das políticas
- ✅ **Segurança**: Backup antes de exclusão permanente (opcional)
- ✅ **Auditoria**: Todas as exclusões são auditadas

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de mídia implementado (Fase 8, Fase 10)
- ✅ Sistema de moderação implementado (Fase 11)
- ✅ Sistema de auditoria (`IAuditLogger`)
- ❌ Não existe política de retenção configurável
- ❌ Retenção fixa ou não explicitamente configurada
- ❌ Dificulta compliance com GDPR/LGPD

### Requisitos Funcionais

#### 1. Modelo de Domínio de Retenção
- ✅ `DataRetentionConfig` por tipo de entidade
- ✅ Configuração por território (opcional, com fallback global)
- ✅ Políticas de retenção:
  - `RetentionPeriodDays` (dias para retenção)
  - `ArchiveBeforeDeletion` (bool, se arquiva antes de deletar)
  - `ArchiveLocation` (string, local do arquivo)
  - `AnonymizeBeforeDeletion` (bool, se anonimiza antes de deletar)
- ✅ Tipos de entidade suportados: Posts, Reports, Media, Logs, WorkItems, AuditEntries, etc.

#### 2. Serviço de Retenção
- ✅ `DataRetentionService`:
  - `GetConfigAsync(Guid? territoryId, string entityType, CancellationToken)`
  - `CreateOrUpdateConfigAsync(DataRetentionConfig, CancellationToken)`
  - `EvaluateRetentionAsync(string entityType, Guid? territoryId, CancellationToken)` → retorna quais entidades devem ser deletadas/arquivadas
- ✅ Jobs assíncronos para limpeza automática (background workers)
- ✅ Processamento em lote para performance
- ✅ Auditoria de todas as exclusões

#### 3. Políticas de Retenção por Tipo de Entidade

##### Posts
- Configuração de retenção para posts deletados
- Anonimização opcional antes de exclusão permanente
- Arquivamento de posts históricos (opcional)

##### Reports
- Configuração de retenção para reports arquivados
- Compliance com regulamentações (GDPR: direito ao esquecimento)

##### Media (Mídias)
- Retenção de mídias não utilizadas (sem `MediaAttachment`)
- Limpeza de mídias temporárias
- Arquivamento de mídias antigas

##### Logs
- Retenção de logs de aplicação
- Configuração por nível de log (Error > Warning > Information)
- Arquivo compactado para logs antigos

##### Work Items
- Retenção de work items resolvidos
- Arquivamento de casos de moderação antigos

##### Audit Entries
- Retenção de entradas de auditoria
- Configuração especial (geralmente retenção longa para compliance)

#### 4. Arquivamento e Backup
- ✅ Serviço de arquivamento (`ArchiveService`)
- ✅ Armazenamento em blob storage configurado (reutilizar `MediaStorageConfig`)
- ✅ Formato de arquivo: JSON, CSV, ou formato comprimido (gzip)
- ✅ Metadados de arquivo: data de arquivamento, tipo de entidade, território
- ✅ Recuperação de arquivos (opcional, para restauração)

#### 5. Anonimização
- ✅ Serviço de anonimização (`AnonymizationService`)
- ✅ Anonimização de dados pessoais antes de exclusão
- ✅ Preservação de dados agregados para analytics
- ✅ Conformidade com GDPR/LGPD

---

## 📋 Tarefas Detalhadas

### Semana 17: Modelo de Domínio e Serviços

#### 16.1 Modelo de Domínio
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `DataRetentionConfig`:
  - [ ] `Id`, `TerritoryId` (nullable para config global)
  - [ ] `EntityType` (string, enum: Post, Report, Media, Log, WorkItem, AuditEntry, etc.)
  - [ ] `RetentionPeriodDays` (int, dias para retenção)
  - [ ] `ArchiveBeforeDeletion` (bool)
  - [ ] `ArchiveLocation` (string, nullable)
  - [ ] `AnonymizeBeforeDeletion` (bool)
  - [ ] `Enabled` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar enum `EntityType` para tipos suportados
- [ ] Validação: `RetentionPeriodDays` mínimo (ex: 7 dias)

**Arquivos a Criar**:
- `backend/Araponga.Domain/Compliance/DataRetentionConfig.cs`
- `backend/Araponga.Domain/Compliance/EntityType.cs`

**Critérios de Sucesso**:
- ✅ Modelo de domínio criado
- ✅ Validações implementadas
- ✅ Documentação do modelo

---

#### 16.2 Repositório e Serviço
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `IDataRetentionConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `DataRetentionConfigService`:
  - [ ] `GetConfigAsync(Guid? territoryId, string entityType, CancellationToken)`
  - [ ] `ListConfigsAsync(Guid? territoryId, CancellationToken)`
  - [ ] `CreateOrUpdateConfigAsync(DataRetentionConfig, CancellationToken)`
- [ ] Criar `DataRetentionService`:
  - [ ] `EvaluateRetentionAsync(string entityType, Guid? territoryId, DateTime beforeDate, CancellationToken)` → retorna IDs de entidades para processar
  - [ ] `ArchiveEntitiesAsync(string entityType, IEnumerable<Guid> entityIds, CancellationToken)`
  - [ ] `AnonymizeEntitiesAsync(string entityType, IEnumerable<Guid> entityIds, CancellationToken)`
  - [ ] `DeleteEntitiesAsync(string entityType, IEnumerable<Guid> entityIds, CancellationToken)`
- [ ] Criar `ArchiveService`:
  - [ ] `ArchiveAsync(string entityType, IEnumerable<object> entities, CancellationToken)` → salva em blob storage
  - [ ] `GetArchiveAsync(string archiveId, CancellationToken)`
- [ ] Criar `AnonymizationService`:
  - [ ] `AnonymizePostAsync(Post post, CancellationToken)`
  - [ ] `AnonymizeReportAsync(Report report, CancellationToken)`
  - [ ] `AnonymizeMediaAsync(MediaAsset media, CancellationToken)`

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/Compliance/IDataRetentionConfigRepository.cs`
- `backend/Araponga.Application/Services/Compliance/DataRetentionConfigService.cs`
- `backend/Araponga.Application/Services/Compliance/DataRetentionService.cs`
- `backend/Araponga.Application/Services/Compliance/ArchiveService.cs`
- `backend/Araponga.Application/Services/Compliance/AnonymizationService.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresDataRetentionConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataRetentionConfigRepository.cs`

**Critérios de Sucesso**:
- ✅ Repositório e serviços criados
- ✅ Lógica de retenção implementada
- ✅ Testes unitários passando

---

### Semana 18: Jobs Assíncronos e Processamento

#### 16.3 Jobs Assíncronos de Limpeza
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DataRetentionWorker` (background worker):
  - [ ] Executa periodicamente (ex: diariamente)
  - [ ] Processa cada tipo de entidade configurado
  - [ ] Usa `DataRetentionService` para avaliar e processar retenção
  - [ ] Processamento em lote para performance
  - [ ] Logging detalhado de operações
- [ ] Configuração de schedule via `appsettings.json` ou `SystemConfig`
- [ ] Tratamento de erros e retry policy
- [ ] Métricas de processamento (quantidade de entidades processadas, tempo de execução)

**Arquivos a Criar**:
- `backend/Araponga.Application/Workers/DataRetentionWorker.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (registrar worker)

**Critérios de Sucesso**:
- ✅ Worker executando periodicamente
- ✅ Processamento em lote funcionando
- ✅ Logging e métricas implementadas

---

#### 16.4 Integração com Tipos de Entidade
**Estimativa**: 28 horas (3.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Implementar avaliação de retenção para cada tipo:
  - [ ] Posts: buscar posts deletados/arquivados além do período
  - [ ] Reports: buscar reports arquivados além do período
  - [ ] Media: buscar mídias sem attachments além do período
  - [ ] Logs: buscar logs além do período (por nível)
  - [ ] WorkItems: buscar work items resolvidos além do período
  - [ ] AuditEntries: buscar entradas além do período
- [ ] Implementar arquivamento para cada tipo:
  - [ ] Serializar entidades para formato de arquivo (JSON/CSV)
  - [ ] Salvar em blob storage usando `ArchiveService`
  - [ ] Registrar metadados de arquivo
- [ ] Implementar anonimização para cada tipo:
  - [ ] Remover/anonimizar dados pessoais (nomes, emails, IPs, etc.)
  - [ ] Preservar dados agregados quando necessário
- [ ] Implementar exclusão para cada tipo:
  - [ ] Soft delete quando aplicável
  - [ ] Hard delete após período de graça (opcional)

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/Compliance/DataRetentionService.cs`
- `backend/Araponga.Application/Services/Compliance/AnonymizationService.cs`

**Critérios de Sucesso**:
- ✅ Todos os tipos de entidade suportados
- ✅ Arquivamento funcionando
- ✅ Anonimização funcionando
- ✅ Exclusão funcionando

---

### Semana 19: Interface Administrativa e Documentação

#### 16.5 API Controller
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DataRetentionConfigController`:
  - [ ] `GET /api/v1/territories/{territoryId}/retention-config` (Curator)
  - [ ] `PUT /api/v1/territories/{territoryId}/retention-config` (Curator)
  - [ ] `GET /api/v1/admin/retention-config` (listar todas, SystemAdmin)
  - [ ] `POST /api/v1/admin/retention-config` (criar, SystemAdmin)
  - [ ] `PUT /api/v1/admin/retention-config/{configId}` (atualizar, SystemAdmin)
  - [ ] `POST /api/v1/admin/retention/jobs/run` (executar manualmente, SystemAdmin)
- [ ] Criar `ArchiveController`:
  - [ ] `GET /api/v1/admin/archives` (listar arquivos, SystemAdmin)
  - [ ] `GET /api/v1/admin/archives/{archiveId}/download` (baixar arquivo, SystemAdmin)

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/DataRetentionConfigController.cs`
- `backend/Araponga.Api/Controllers/ArchiveController.cs`

**Critérios de Sucesso**:
- ✅ Endpoints funcionando
- ✅ Autorização implementada
- ✅ Validação de requests

---

#### 16.6 Interface Administrativa (DevPortal)
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Seção "Compliance e Retenção de Dados" no DevPortal
- [ ] Visualização de políticas de retenção por tipo de entidade
- [ ] Interface para configurar políticas (território e global)
- [ ] Visualização de estatísticas (entidades processadas, arquivos criados)
- [ ] Explicação de conformidade com GDPR/LGPD
- [ ] Documentação de políticas de retenção recomendadas

**Arquivos a Modificar**:
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Interface administrativa disponível
- ✅ Configuração intuitiva
- ✅ Documentação clara

---

#### 16.7 Testes e Documentação
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração para `DataRetentionConfigController`
- [ ] Testes de integração para `DataRetentionService`
- [ ] Testes de integração para `ArchiveService`
- [ ] Testes de integração para `AnonymizationService`
- [ ] Testes de `DataRetentionWorker`
- [ ] Documentação técnica completa
- [ ] Guia de conformidade GDPR/LGPD
- [ ] Atualizar `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md`

**Arquivos a Criar**:
- `backend/Araponga.Tests/Api/DataRetentionConfigIntegrationTests.cs`
- `backend/Araponga.Tests/Application/DataRetentionServiceTests.cs`
- `docs/COMPLIANCE_RETENTION.md`
- `docs/GDPR_LGPD_COMPLIANCE.md`

**Critérios de Sucesso**:
- ✅ Testes passando (>90% cobertura)
- ✅ Documentação completa
- ✅ Guia de conformidade disponível

---

## ✅ Critérios de Sucesso da Fase 16

### Funcionalidades
- ✅ Políticas de retenção configuráveis por tipo de entidade
- ✅ Configuração por território (respeitando legislação local)
- ✅ Limpeza automática de dados expirados
- ✅ Arquivamento antes de exclusão (opcional)
- ✅ Anonimização antes de exclusão (opcional)
- ✅ Conformidade com GDPR/LGPD

### Qualidade
- ✅ Cobertura de testes >90%
- ✅ Jobs assíncronos funcionando corretamente
- ✅ Auditoria completa de exclusões

### Documentação
- ✅ Documentação técnica completa
- ✅ Guia de conformidade GDPR/LGPD
- ✅ DevPortal atualizado

---

## 🔗 Dependências

- **Fase 10**: Mídias em Conteúdo (para retenção de mídias)
- **Fase 11**: Moderação Avançada (para retenção de reports)
- **Fase 8**: Infraestrutura de Mídia (para arquivamento em blob storage)

---

## 📝 Notas de Implementação

### Padrão Arquitetural

Seguir o mesmo padrão implementado para `MediaStorageConfig`:
- Modelo de domínio → Repositório → Serviço → API Controller → Testes → Documentação

### Conformidade GDPR/LGPD

- **Direito ao Esquecimento**: Permite exclusão de dados pessoais mediante solicitação
- **Retenção Mínima**: Políticas de retenção devem respeitar períodos mínimos legais
- **Anonimização**: Dados podem ser anonimizados antes de exclusão para preservar analytics
- **Auditoria**: Todas as exclusões devem ser auditadas

### Recomendações de Políticas Padrão

- **Posts deletados**: 30 dias (período de recuperação)
- **Reports arquivados**: 1 ano (compliance)
- **Mídias não utilizadas**: 90 dias
- **Logs (Information)**: 30 dias
- **Logs (Error)**: 1 ano
- **Work Items resolvidos**: 1 ano
- **Audit Entries**: 7 anos (compliance legal)

---

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo da avaliação de flexibilização de configurações.

---

**Status**: ⏳ Pendente  
**Última atualização**: 2026-01-17
