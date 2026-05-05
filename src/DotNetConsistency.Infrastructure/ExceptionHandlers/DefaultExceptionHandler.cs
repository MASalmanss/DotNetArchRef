using Microsoft.AspNetCore.Http;

namespace DotNetConsistency.Infrastructure.ExceptionHandlers;

public class DefaultExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => true;

    public (int StatusCode, string Message) Handle(Exception exception)
        => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
}
