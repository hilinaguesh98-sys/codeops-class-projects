namespace AddisEats.Api.Models;

public sealed record OrderError(string Code, string Message);

public sealed class Result<T>
{
    private Result(T? value, OrderError? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public OrderError? Error { get; }
    public bool IsSuccess => Error is null;

    public static Result<T> Success(T value) => new(value, null);

    public static Result<T> Failure(string code, string message) =>
        new(default, new OrderError(code, message));
}
