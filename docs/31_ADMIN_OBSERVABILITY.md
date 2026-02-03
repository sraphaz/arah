# Admin e Observabilidade

**Data**: 2026-01-28  
**Status**: 📋 Documentação Atualizada  
**Versão**: 2.0

## Visão geral
O módulo administrativo é **pós-MVP** e serve para observar a saúde do ecossistema territorial,
com foco em transparência operacional e suporte à moderação.

## Escopo do módulo admin [POST-MVP]
- Visão por território (atividade, número de vínculos, nível de reports).
- Painel de erros e alertas do sistema.
- Relatórios agregados de moderação.
- Acompanhamento de sanções (territoriais e globais).
- Indicadores de saúde do sistema (latência, falhas, filas).

## Base P0 já implementada
- **SystemConfig**: configurações globais calibráveis (sem segredos) via endpoints admin.
- **Work Queue**: fila genérica de itens de trabalho para suportar automação + fallback humano.
- **Observabilidade**: Logs (Serilog), Métricas (Prometheus), Tracing (OpenTelemetry).
- Ver detalhes em: `docs/33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md`

## 🖥️ Interface Web de Monitoramento

**Status**: ⏳ Planejado (Expansão da Fase 4)

A aplicação terá uma **interface web integrada** para monitoramento e auxílio à produção, acessível em `/admin/monitoring`.

### Funcionalidades

1. **Dashboard Principal** (`/admin/monitoring`)
   - Status geral do sistema
   - Métricas principais (request rate, error rate, latência)
   - Health checks visuais
   - Alertas ativos
   - Logs recentes

2. **Visualizador de Logs** (`/admin/monitoring/logs`)
   - Logs em tempo real (SignalR)
   - Filtros (nível, componente, período, busca)
   - Estatísticas (contagem por nível, top 10 erros)
   - Exportação (JSON, CSV)

3. **Métricas e Dashboards** (`/admin/monitoring/metrics`)
   - Gráficos em tempo real
   - Métricas de negócio e sistema
   - Dashboards customizáveis

4. **Health Checks** (`/admin/monitoring/health`)
   - Status detalhado de dependências
   - Tempo de resposta
   - Histórico de falhas

5. **Troubleshooting** (`/admin/monitoring/troubleshooting`)
   - Diagnóstico automático
   - Comandos úteis
   - Guia de resolução

### Arquitetura

A interface web funciona em **todas as arquiteturas**:

- ✅ **Monolito**: Interface integrada na própria aplicação
- ✅ **APIs Modulares**: Interface no Gateway (agregação de todas as APIs)
- ✅ **Microserviços**: Interface no Gateway (agregação global)

**Ver documentação completa**: [`LOGS_MONITORAMENTO_ARQUITETURA.md`](./LOGS_MONITORAMENTO_ARQUITETURA.md)

**Estimativa de Implementação**: 4 semanas (160 horas)

---

## Objetivos
- Dar visibilidade para a equipe sobre o estado dos territórios.
- Identificar padrões de risco e problemas operacionais.
- Suportar decisões de governança sem interferir na autonomia do território.
- Fornecer interface web para monitoramento e troubleshooting em produção.