namespace DrogueriaPOS.Application.Common;
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public string ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; }

    private Result(bool isSuccess, T data, string errorMessage, List<string> errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage);
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default, string.Join(", ", errors), errors);
    }
}

/// <summary>
/// Result sin datos de retorno (para operaciones void)
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; }

    private Result(bool isSuccess, string errorMessage, List<string> errors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Errors = errors ?? new List<string>();
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result Failure(List<string> errors)
    {
        return new Result(false, string.Join(", ", errors), errors);
    }
}

