using DotNetArchRef.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DotNetArchRef.Api.ExceptionHandlers;

public class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is DomainException;

    public (int StatusCode, string Message) Handle(Exception exception)
        => (StatusCodes.Status422UnprocessableEntity, exception.Message);
}
