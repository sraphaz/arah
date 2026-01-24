# Padrão de Exception Handling - Araponga

**Última Atualização**: 2025-01-23  
**Status**: ✅ Padrão Definido

---

## 📋 Resumo

Este documento descreve o padrão de tratamento de exceções usado no Araponga, garantindo consistência e melhor experiência para desenvolvedores e usuários.

---

## 🎯 Princípios

1. **Result<T> Pattern**: Services retornam `Result<T>` em vez de lançar exceções para erros de negócio
2. **Exceções Apenas para Erros Técnicos**: Exceções são lançadas apenas para erros inesperados (bugs, infraestrutura)
3. **Logging Estruturado**: Todas as exceções são logadas com contexto suficiente
4. **Mensagens Amigáveis**: Erros retornados ao usuário são claros e acionáveis

---

## 📐 Padrão Result<T>

### Quando Usar Result<T>

Use `Result<T>` para:
- ✅ Validações de negócio (ex: usuário não tem permissão)
- ✅ Regras de domínio (ex: post não encontrado)
- ✅ Operações que podem falhar de forma esperada (ex: email já existe)

**NÃO use** `Result<T>` para:
- ❌ Erros de infraestrutura (ex: banco de dados indisponível)
- ❌ Bugs no código (ex: NullReferenceException)
- ❌ Erros de configuração (ex: connection string inválida)

### Exemplo de Uso

```csharp
public async Task<Result<Post>> CreatePostAsync(
    Guid territoryId,
    Guid userId,
    string title,
    string content,
    CancellationToken cancellationToken)
{
    // Validações retornam Result<T>.Failure
    if (string.IsNullOrWhiteSpace(title))
    {
        return Result<Post>.Failure("Title is required.");
    }

    // Verificações de negócio retornam Result<T>.Failure
    var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
        userId, territoryId, cancellationToken);
    if (membership is null)
    {
        return Result<Post>.Failure("User is not a member of this territory.");
    }

    // Sucesso retorna Result<T>.Success
    var post = new Post(...);
    await _repository.AddAsync(post, cancellationToken);
    await _unitOfWork.CommitAsync(cancellationToken);
    
    return Result<Post>.Success(post);
}
```

---

## 🚨 Tratamento de Exceções

### No Controller

Controllers devem:
1. Chamar services que retornam `Result<T>`
2. Tratar exceções inesperadas (catch genérico)
3. Retornar status codes apropriados
4. Logar exceções com contexto

```csharp
[HttpPost]
public async Task<ActionResult<PostResponse>> CreatePost(
    [FromBody] CreatePostRequest request,
    CancellationToken cancellationToken)
{
    try
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _postService.CreatePostAsync(
            request.TerritoryId,
            userContext.User.Id,
            request.Title,
            request.Content,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(MapToResponse(result.Value!));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, 
            "Unexpected error creating post. UserId: {UserId}, TerritoryId: {TerritoryId}",
            userContext?.User?.Id, request?.TerritoryId);
        
        return StatusCode(500, new { error = "An unexpected error occurred." });
    }
}
```

### No Service

Services devem:
1. Usar `Result<T>` para erros de negócio
2. Deixar exceções técnicas propagarem (banco, rede, etc.)
3. Logar apenas exceções inesperadas (se necessário)

```csharp
public async Task<Result<Post>> CreatePostAsync(...)
{
    try
    {
        // Lógica de negócio com Result<T>
        var result = await ValidateAndCreateAsync(...);
        return result;
    }
    catch (DbUpdateException ex)
    {
        // Exceções de infraestrutura são logadas e relançadas
        _logger.LogError(ex, "Database error creating post");
        throw; // Propaga para middleware de exception handling
    }
}
```

---

## 🛡️ Exception Middleware

O Araponga usa exception middleware global para:
- Capturar exceções não tratadas
- Logar com contexto completo
- Retornar respostas padronizadas
- Mascarar detalhes em produção

```csharp
// Program.cs
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandler?.Error is not null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionHandler.Error, 
                "Unhandled exception: {Path}", context.Request.Path);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "An unexpected error occurred." };
            await context.Response.WriteAsJsonAsync(response);
        }
    });
});
```

---

## 📝 Tipos de Exceções

### 1. Domain Exceptions (Não Usadas)

**Não criamos** exceções de domínio. Usamos `Result<T>` em vez disso.

❌ **Evitar**:
```csharp
throw new PostNotFoundException(postId);
```

✅ **Preferir**:
```csharp
return Result<Post>.Failure("Post not found.");
```

### 2. Infrastructure Exceptions

Exceções de infraestrutura (banco, rede, etc.) são deixadas propagar naturalmente.

```csharp
// Não tratar - deixar propagar
await _dbContext.SaveChangesAsync(cancellationToken);
```

### 3. Validation Exceptions

Validações retornam `Result<T>.Failure` ou usam FluentValidation.

```csharp
// FluentValidation no controller
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

---

## 🔍 Logging de Exceções

### Estrutura de Log

```csharp
_logger.LogError(ex, 
    "Error message with context. UserId: {UserId}, PostId: {PostId}",
    userId, postId);
```

### Níveis de Log

- **Error**: Exceções inesperadas, erros de infraestrutura
- **Warning**: Situações recuperáveis, retries
- **Information**: Operações importantes (criação de entidades)
- **Debug**: Detalhes técnicos para debugging

---

## ✅ Checklist

Ao implementar um novo service:

- [ ] Métodos públicos retornam `Result<T>` para erros de negócio
- [ ] Validações retornam `Result<T>.Failure` com mensagens claras
- [ ] Exceções técnicas são deixadas propagar
- [ ] Exceções inesperadas são logadas com contexto
- [ ] Controllers tratam `Result<T>` e exceções
- [ ] Mensagens de erro são amigáveis ao usuário

---

## 📚 Referências

- [Result Pattern](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/)
- [Exception Handling Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)
- [Structured Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/)

---

**Nota**: Este padrão deve ser seguido em todos os novos services e controllers.
