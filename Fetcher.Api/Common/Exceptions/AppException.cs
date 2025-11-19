using System.Net;

namespace Fetcher.Api.Common.Exceptions;

public abstract class AppException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
