namespace Application.Common;

public class Result<T>
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }
    public List<string>? Errors { get; private set; }

    private Result() { }

    public static Result<T> OK(T data, string? message = null)
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operación exitosa",
            Errors = null
        };
    }

    public static Result<T> Fail(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            Data = default
        };
    }

    public static Result<T> Fail(string message, string error)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = [error],
            Data = default
        };
    }
}
