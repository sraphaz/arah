# Padrão Result<T> - Araponga

**Última Atualização**: 2025-01-23  
**Status**: ✅ Padrão Definido e Implementado

---

## 📋 Resumo

O Araponga usa o padrão `Result<T>` para representar operações que podem falhar de forma esperada, evitando o uso excessivo de exceções para controle de fluxo.

---

## 🎯 Princípios

1. **Result<T> para Erros de Negócio**: Use `Result<T>` quando uma falha é esperada e faz parte do fluxo normal
2. **Exceções para Erros Técnicos**: Use exceções apenas para erros inesperados (bugs, infraestrutura)
3. **Mensagens Claras**: Erros devem ter mensagens descritivas e acionáveis
4. **Type Safety**: Evite usar `null` como indicador de erro

---

## 📐 Estrutura

### Result<T> Class

```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);
}
```

---

## 💡 Exemplos de Uso

### Service Method

```csharp
public async Task<Result<Post>> CreatePostAsync(
    Guid territoryId,
    Guid userId,
    string title,
    string content,
    CancellationToken cancellationToken)
{
    // Validação retorna Failure
    if (string.IsNullOrWhiteSpace(title))
    {
        return Result<Post>.Failure("Title is required.");
    }

    // Verificação de negócio retorna Failure
    var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
        userId, territoryId, cancellationToken);
    if (membership is null)
    {
        return Result<Post>.Failure("User is not a member of this territory.");
    }

    // Sucesso retorna Success
    var post = new Post(...);
    await _repository.AddAsync(post, cancellationToken);
    await _unitOfWork.CommitAsync(cancellationToken);
    
    return Result<Post>.Success(post);
}
```

### Controller Usage

```csharp
[HttpPost]
public async Task<ActionResult<PostResponse>> CreatePost(
    [FromBody] CreatePostRequest request,
    CancellationToken cancellationToken)
{
    var result = await _postService.CreatePostAsync(
        request.TerritoryId,
        userId,
        request.Title,
        request.Content,
        cancellationToken);

    if (result.IsFailure)
    {
        return BadRequest(new { error = result.Error });
    }

    return Ok(MapToResponse(result.Value!));
}
```

### Pattern Matching (C# 8+)

```csharp
var result = await _service.DoSomethingAsync(...);

var response = result switch
{
    { IsSuccess: true } => Ok(result.Value),
    { IsFailure: true } => BadRequest(new { error = result.Error }),
    _ => StatusCode(500)
};
```

---

## 🔄 Operações Sem Retorno

Para operações que não retornam valor, use `OperationResult`:

```csharp
public sealed class OperationResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    private OperationResult(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static OperationResult Success() => new(true, null);
    public static OperationResult Failure(string error) => new(false, error);
}
```

### Exemplo

```csharp
public async Task<OperationResult> DeletePostAsync(
    Guid postId,
    Guid userId,
    CancellationToken cancellationToken)
{
    var post = await _repository.GetByIdAsync(postId, cancellationToken);
    if (post is null)
    {
        return OperationResult.Failure("Post not found.");
    }

    if (post.AuthorUserId != userId)
    {
        return OperationResult.Failure("User is not the author of this post.");
    }

    await _repository.DeleteAsync(post, cancellationToken);
    await _unitOfWork.CommitAsync(cancellationToken);
    
    return OperationResult.Success();
}
```

---

## ✅ Quando Usar Result<T>

### ✅ Use Result<T> Para:

- Validações de entrada
- Regras de negócio (ex: usuário não tem permissão)
- Entidades não encontradas (quando é esperado)
- Operações que podem falhar de forma esperada
- Verificações de estado (ex: post já foi deletado)

### ❌ NÃO Use Result<T> Para:

- Erros de infraestrutura (banco indisponível) → Deixe exceção propagar
- Bugs no código (NullReferenceException) → Corrija o bug
- Erros de configuração → Use exceções
- Timeouts de rede → Deixe exceção propagar

---

## 🔍 Testes com Result<T>

### Teste de Sucesso

```csharp
[Fact]
public async Task CreatePostAsync_WhenValid_ReturnsSuccess()
{
    // Arrange
    var service = CreateService();
    
    // Act
    var result = await service.CreatePostAsync(...);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("Expected Title", result.Value.Title);
}
```

### Teste de Falha

```csharp
[Fact]
public async Task CreatePostAsync_WhenTitleEmpty_ReturnsFailure()
{
    // Arrange
    var service = CreateService();
    
    // Act
    var result = await service.CreatePostAsync(
        territoryId: Guid.NewGuid(),
        userId: Guid.NewGuid(),
        title: "", // Empty title
        content: "Content",
        cancellationToken: CancellationToken.None);
    
    // Assert
    Assert.True(result.IsFailure);
    Assert.Null(result.Value);
    Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
}
```

---

## 📊 Comparação: Result<T> vs Exceções

| Situação | Result<T> | Exceção |
|----------|-----------|---------|
| Validação de entrada | ✅ | ❌ |
| Regra de negócio | ✅ | ❌ |
| Entidade não encontrada (esperado) | ✅ | ❌ |
| Entidade não encontrada (bug) | ❌ | ✅ |
| Banco de dados indisponível | ❌ | ✅ |
| NullReferenceException | ❌ | ✅ (corrigir bug) |

---

## 🎨 Extensões Úteis

### Map Result

```csharp
public static Result<TOut> Map<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, TOut> mapper)
{
    return result.IsSuccess
        ? Result<TOut>.Success(mapper(result.Value!))
        : Result<TOut>.Failure(result.Error!);
}
```

### Bind (FlatMap)

```csharp
public static Result<TOut> Bind<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, Result<TOut>> binder)
{
    return result.IsSuccess
        ? binder(result.Value!)
        : Result<TOut>.Failure(result.Error!);
}
```

---

## ✅ Checklist

Ao implementar um novo service:

- [ ] Métodos públicos retornam `Result<T>` ou `OperationResult`
- [ ] Validações retornam `Failure` com mensagens claras
- [ ] Verificações de negócio retornam `Failure` quando apropriado
- [ ] Sucesso retorna `Success` com valor
- [ ] Testes cobrem casos de sucesso e falha
- [ ] Controllers tratam `Result<T>` adequadamente

---

## 📚 Referências

- [Functional Error Handling in C#](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/)
- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)
- [Result Pattern Implementation](https://github.com/ardalis/Result)

---

**Nota**: Todos os services do Araponga devem usar `Result<T>` para erros de negócio. Exceções são reservadas para erros técnicos inesperados.
