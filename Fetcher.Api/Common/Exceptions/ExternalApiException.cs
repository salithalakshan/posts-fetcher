using System.Net;

namespace Fetcher.Api.Common.Exceptions;

public sealed class  ExternalApiException(string message) : AppException(message, HttpStatusCode.BadGateway);
