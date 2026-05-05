using Microsoft.AspNetCore.Http;

namespace DotNetConsistency.Infrastructure.ExceptionHandlers;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is KeyNotFoundException;

    public (int StatusCode, string Message) Handle(Exception exception)
        => (StatusCodes.Status404NotFound, exception.Message);
}
