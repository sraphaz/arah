namespace Araponga.Application.Common;

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// </summary>
public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);

    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Represents the result of an operation without a return value.
/// </summary>
public sealed class OperationResult
{
    private OperationResult(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public static OperationResult Success() => new(true, null);
    public static OperationResult Failure(string error) => new(false, error);
}
