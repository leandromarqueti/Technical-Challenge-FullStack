namespace TechnicalChallenge.Shared.Results;


public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public IReadOnlyList<string> Errors { get; private set; }

    private Result(bool isSuccess, T? data, string? message, List<string> errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        Errors = errors;
    }

    public static Result<T> Success(T? data = default, string? message = null)
    {
        return new Result<T>(true, data, message, new List<string>());
    }

    public static Result<T> Failure(string message, List<string>? errors = null)
    {
        return new Result<T>(false, default, message, errors ?? new List<string> { message });
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default, string.Join(", ", errors), errors);
    }
}
