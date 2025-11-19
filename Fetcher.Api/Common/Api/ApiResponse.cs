namespace Fetcher.Api.Common.Api;

public sealed record ApiError(string Code, string Message);
public sealed record ApiResponse<T>(
    bool IsSuccess,
    T Data,
    ApiError Errors,
    string TraceId
    )
{
    public static ApiResponse<T> Success(T data, string traceId) =>
        new(true, data, null, traceId);

    public static ApiResponse<T> Error(string code, string message, string traceId) =>
        new(false, default, new ApiError(code, message), traceId);

}
