using Microsoft.AspNetCore.Http;

namespace DotNetArchRef.Api.ExceptionHandlers;

public class DefaultExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => true;

    public (int StatusCode, string Message) Handle(Exception exception)
        => (StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu.");
}
