namespace IplMerchStore.Application.Common;

/// <summary>
/// Generic result wrapper for API responses
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static Result<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static Result<T> FailureResult(string message, IEnumerable<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Generic result wrapper without data payload
/// </summary>
public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static Result SuccessResult(string message = "Operation successful")
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    public static Result FailureResult(string message, IEnumerable<string>? errors = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
