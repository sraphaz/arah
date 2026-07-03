# ADR-021: Convenção de sinalização de erros (Result, nullable, exceptions)

**Status**: accepted
**Data**: 2026-07-03
**Autor**: Solutions Architect (revisão Clean Architecture / Uncle Bob)
**Spec-Id**: —
**LikeC4 view**: —

---

## Contexto

A revisão de Clean Architecture (`docs/analysis/REVISAO_CLEAN_ARCHITECTURE_UNCLE_BOB.md`) apontou
**inconsistência** na forma de sinalizar falha na camada de aplicação: coexistem três estilos —
`Result<T>`/`OperationResult`, retorno `null` (via `T?`) e exceções de domínio. A regra do projeto
(`.cursorrules`, `backend-standards`) diz "NUNCA retorne `null` de métodos públicos".

Existem hoje ~35 métodos de consulta públicos em `Arah.Application/Services/**` com assinatura
`Task<T?>` onde `null` significa "não encontrado" (ex.: `GetByIdAsync`, `GetByKeyAsync`,
`GetActiveConfigAsync`). Convertê-los em massa para `Result<T>` alteraria o contrato de dezenas de
métodos, quebraria todos os call sites (controllers, outros services e testes) e representaria
*churn* de alto risco com benefício real baixo — exatamente o tipo de mudança que o próprio critério
Uncle Bob classifica como *needless complexity* / *viscosity* quando forçada sem necessidade.

## Decisão

Formalizar a convenção (em vez de reescrever), tornando o *smell* de inconsistência uma regra
explícita:

1. **`Result<T>` / `OperationResult`** — obrigatório para **comandos e operações que podem falhar
   com um motivo de negócio** que o chamador precisa distinguir (criar, atualizar, validar, aprovar,
   processar pagamento). O motivo da falha faz parte do contrato.
2. **`T?` (nullable reference type)** — **permitido** para **consultas simples "get-or-null"**, onde
   a ausência (`null`) é o único desfecho alternativo e não carrega motivo. `T?` é o idioma moderno
   do C# (nullable reference types habilitado) e comunica a ausência no próprio tipo. Isto **reinterpreta**
   a regra "nunca retorne null": ela vale para métodos onde `null` seria **ambíguo** (erro vs vazio vs
   não-inicializado), não para consultas cujo `T?` é semanticamente "não encontrado".
3. **Exceções de domínio** (`ValidationException`, `NotFoundException`, `ConflictException`,
   `DomainException`) — reservadas para o **excepcional** e para o *guard* de invariantes; mapeadas a
   status HTTP no `UseArahExceptionHandler`.

Regra prática: se o chamador precisa saber **por que** falhou → `Result<T>`. Se só precisa saber
**se existe** → `T?`. Se é uma violação de invariante/estado impossível → exceção.

## Consequências

**Positivas**
- Elimina a ambiguidade "qual estilo usar" sem *churn* de risco em 35+ arquivos e seus testes.
- Preserva comportamento e contratos atuais (nenhuma quebra).
- Alinha com nullable reference types (o compilador já força o tratamento de `T?`).

**Negativas / trade-offs**
- Mantém dois estilos de "não encontrado" no código (Result vs null), agora **por regra** e não por acaso.
- Exige revisão em PR para garantir que novos comandos usem `Result<T>` (não `null`).

## Alternativas consideradas

- **Converter tudo para `Result<T>`** (rejeitada): alto risco, quebra call sites/testes, adiciona
  indireção a consultas triviais — *needless complexity*.
- **Introduzir `Option<T>`/Maybe** (rejeitada por ora): mais um tipo/dependência conceitual sem ganho
  sobre `T?` do C# moderno; reavaliar só se surgir necessidade de encadeamento monádico amplo.

## Diagrama

—

## Referências

- [ADR Registry](../ADR-REGISTRY.yaml)
- [Revisão Clean Architecture / Uncle Bob](../../analysis/REVISAO_CLEAN_ARCHITECTURE_UNCLE_BOB.md)
- [10_ARCHITECTURE_DECISIONS.md](../10_ARCHITECTURE_DECISIONS.md)
