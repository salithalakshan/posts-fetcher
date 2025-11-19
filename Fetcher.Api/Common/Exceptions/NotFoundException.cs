using System.Net;

namespace Fetcher.Api.Common.Exceptions;

public sealed class NotFoundException(string message) : AppException(message, HttpStatusCode.NotFound);
