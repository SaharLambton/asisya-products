namespace Asisya.Products.Application.Common;

public class ServiceResult<T>
{
    public T? Data { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int StatusCode { get; private set; }

    private ServiceResult() { }

    public static ServiceResult<T> Success(T data, int statusCode = 200) =>
        new() { Data = data, IsSuccess = true, StatusCode = statusCode };

    public static ServiceResult<T> Failure(string errorMessage, int statusCode = 400) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };

    public static ServiceResult<T> NotFound(string message = "Resource not found") =>
        Failure(message, 404);
}

public class ServiceResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int StatusCode { get; private set; }

    private ServiceResult() { }

    public static ServiceResult Success(int statusCode = 204) =>
        new() { IsSuccess = true, StatusCode = statusCode };

    public static ServiceResult Failure(string errorMessage, int statusCode = 400) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };

    public static ServiceResult NotFound(string message = "Resource not found") =>
        Failure(message, 404);
}
