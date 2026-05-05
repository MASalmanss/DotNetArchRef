using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DotNetConsistency.Infrastructure.ExceptionHandlers;

public class ConflictExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
        => exception is InvalidOperationException or DbUpdateException;

    public (int StatusCode, string Message) Handle(Exception exception)
    {
        var message = exception is DbUpdateException
            ? "Operation violates a data integrity constraint."
            : exception.Message;

        return (StatusCodes.Status409Conflict, message);
    }
}
