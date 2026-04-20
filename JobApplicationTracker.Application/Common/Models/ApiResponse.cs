namespace JobApplicationTracker.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public object? Error { get; init; }

    public static ApiResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> Fail(object error) => new()
    {
        Success = false,
        Error = error
    };
}
