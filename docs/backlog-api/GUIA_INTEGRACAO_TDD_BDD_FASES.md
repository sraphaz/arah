# Guia de Integração TDD/BDD nas Fases

**Versão**: 1.1  
**Data**: 2025-01-20  
**Status**: 📋 Guia Atualizado

---

## 🎯 Objetivo

Este guia garante que **TODAS as fases futuras** (10+) tenham integração homogênea e consistente de TDD (Test-Driven Development) e BDD (Behavior-Driven Development), seguindo o padrão estabelecido na **Fase 0: Fundação TDD/BDD**.

**Nota**: A Fase 9 já passou, então o trabalho de TDD/BDD é distribuído a partir da Fase 10.

---

## 📋 Regras Obrigatórias

### 1. Seção TDD/BDD Obrigatória

**TODAS as fases futuras (10+) DEVEM incluir** uma seção "🧪 Estratégia TDD/BDD" após "📋 Tarefas Detalhadas" e antes de "📊 Resumo da Fase".

**Template**: Ver `TEMPLATE_TDD_BDD_FASES.md`

### 2. Duração Ajustada

**TODAS as fases futuras (10+) DEVEM incluir** ajuste de duração para TDD/BDD:
- **+20% de tempo** para fases 10-15 (estabelecimento do padrão)
- **+15% de tempo** para fases 16+ (padrão já estabelecido)

**Exemplo**:
- Duração original: 15 dias
- Duração ajustada (Fase 10-15): 15 × 1.2 = 18 dias
- Duração ajustada (Fase 16+): 15 × 1.15 = 17.25 dias (arredondar para 17-18)

### 3. Cobertura Obrigatória

**TODAS as funcionalidades DEVEM ter**:
- ✅ Cobertura >90% (funcionalidades padrão)
- ✅ Cobertura >95% (funcionalidades críticas: segurança, pagamentos, blockchain, governança)

### 4. BDD para Funcionalidades de Negócio

**Funcionalidades de negócio críticas DEVEM ter**:
- ✅ Feature Gherkin (SpecFlow)
- ✅ Steps implementados
- ✅ Documentação viva

---

## 📊 Distribuição por Fases

Ver [Plano de Distribuição TDD/BDD](./PLANO_DISTRIBUICAO_TDD_BDD.md) para detalhamento completo.

**Resumo**:
- **Fases 10-12**: +20% tempo, foco em estabelecer padrão (mais BDD)
- **Fases 13-15**: +20% tempo, consolidação (BDD para governança e segurança)
- **Fases 16+**: +15% tempo, manutenção (TDD padrão, BDD seletivo)

---

## 📝 Checklist de Integração

### Para Cada Nova Fase (10+)

Ao criar ou atualizar uma fase, verificar:

- [ ] Seção "🧪 Estratégia TDD/BDD" incluída
- [ ] Duração ajustada conforme fase (+15-20%)
- [ ] Lista de funcionalidades que DEVEM ter BDD
- [ ] Checklist TDD/BDD por funcionalidade
- [ ] Métricas de sucesso definidas
- [ ] Referências ao plano TDD/BDD incluídas

---

## 🔄 Processo de Atualização

### 1. Identificar Fases sem TDD/BDD

```bash
# Buscar fases sem seção TDD/BDD (fases 10+)
grep -L "Estratégia TDD/BDD" docs/backlog-api/FASE1[0-9].md docs/backlog-api/FASE[2-9][0-9].md
```

### 2. Adicionar Seção TDD/BDD

1. Copiar template de `TEMPLATE_TDD_BDD_FASES.md`
2. Adaptar para a fase específica:
   - Listar funcionalidades que DEVEM ter BDD
   - Ajustar cobertura mínima (90% ou 95%)
   - Incluir exemplos específicos da fase
   - Referenciar `PLANO_DISTRIBUICAO_TDD_BDD.md`

### 3. Ajustar Duração

1. Verificar fase no `PLANO_DISTRIBUICAO_TDD_BDD.md`
2. Calcular duração ajustada conforme fase
3. Atualizar campo "Duração" no cabeçalho
4. Atualizar "Estimativa Total" se necessário

### 4. Validar

- [ ] Seção TDD/BDD completa
- [ ] Duração ajustada
- [ ] Referências corretas
- [ ] Checklist por funcionalidade

---

## 📊 Status de Integração

### Fases com TDD/BDD Integrado

- [x] Fase 0: Fundação TDD/BDD (completa)
- [x] Fase 10: Mídias em Conteúdo (integrado)
- [x] Fase 11: Edição e Gestão (integrado)
- [ ] Fase 12: Otimizações Finais (pendente)
- [ ] Fase 13: Conector de Emails (pendente)
- [ ] Fase 14: Governança Comunitária (pendente)
- [ ] Fase 15: Segurança Avançada (pendente)
- [ ] Fases 16-29: (pendente)

**Última Atualização**: 2025-01-20  
**Próxima Revisão**: Após integração das Fases 12-15

---

## 📚 Referências

- [Plano Completo TDD/BDD](../23_TDD_BDD_PLANO_IMPLEMENTACAO.md)
- [Plano de Distribuição TDD/BDD](./PLANO_DISTRIBUICAO_TDD_BDD.md)
- [Fase 0: Fundação TDD/BDD](./FASE0.md)
- [Template TDD/BDD para Fases](./TEMPLATE_TDD_BDD_FASES.md)
- [Análise de Coesão e Testes](../22_COHESION_AND_TESTS.md)

---

**Última Atualização**: 2025-01-20
