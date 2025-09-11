

public  class Result<T>
{
  public bool IsSuccess { get; set; }

    public bool IsError { get; set; }
    public bool IsValidationError { get; set; }

    public bool IsSystemError { get; set; }

    public T Data { get; set; }
    public string Message { get; set; }

    public enumRespType RespType { get; set; }


    public static Result<T> Success(T data, string message = "")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
      };
    }

    public static Result<T> Error(T data, string message = "")
    {
        return new Result<T>
        {
            IsError = true,
            Data = data,
            Message = message,
        };
    }

    public static Result<T> SystemError(T data, string message = "")
    {
        return new Result<T>
        {
            IsSystemError = true,
            Data = data,
            Message = message,
        };
    }

    public static Result<T> ValidationError(T data, string message = "")
    {
        return new Result<T>
        {
            IsValidationError = true,
            Data = data,
            Message = message,
        };
    }


    public enum enumRespType
    {
        None, 
        Success, 
        Error,
        ValidationError,
        SystemError
    }


}