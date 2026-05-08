using DotNetConsistency.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DotNetConsistency.Api.ExceptionHandlers;

public class DataCorruptionExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is DataCorruptionException;

    public (int StatusCode, string Message) Handle(Exception exception)
        => (StatusCodes.Status500InternalServerError, exception.Message);
}
