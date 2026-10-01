namespace IdentityManagement.Application.Common.Models;

/// <summary>
/// Generic operation result used by handlers to convey success/failure and errors
/// without throwing for expected business failures.
/// </summary>
public class Result
{
    public bool Succeeded { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
    public static Result Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors.ToArray() };
}

/// <summary>
/// Generic operation result carrying a payload on success.
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static Result<T> Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
    public new static Result<T> Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors.ToArray() };
}
