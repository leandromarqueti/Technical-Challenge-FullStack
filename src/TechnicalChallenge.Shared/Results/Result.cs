namespace TechnicalChallenge.Shared.Results;


public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public Result() { }

    public Result(bool isSuccess, T? data, string? message, List<string> errors)
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
